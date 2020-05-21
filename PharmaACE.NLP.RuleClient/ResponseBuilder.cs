using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using PharmaACE.NLP.ChartAudit.NLIDB;
using PharmaACE.NLP.Framework;
using PharmaACE.Utility;

namespace PharmaACE.NLP.Responder
{
    public class ResponseBuilder
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        RuleEngineBase ruleEngine;
        ChartEngineBase chartEngine;
        NaturalLanguageQuestion questionModel;
        StaticResources staticResources;
        RecognitionOutput recognitionOutput;
        string DbQuery { get; set; }
        string RealizedQuery { get; set; }

        public ResponseBuilder(NaturalLanguageQuestion questionModel)
        {
            this.staticResources = StaticResources.GetInstance();
            this.questionModel = questionModel;
            DimensionFactory dimFactory = Prober.Probe(questionModel?.Question);
            ruleEngine = dimFactory.RuleEngine;
            chartEngine = dimFactory.ChartEngine;
        }
                
        void PredictQuery(NaturalLanguageQuestion inputSentence)
        {
            var parser = new Parser(staticResources.Database, staticResources.LanguageConfig);
            //backup original question with the aspiration to resuse the question object with as many as in-place string modifications possible
            //i.e. without doing too many string mutations
            string originalQuestion = inputSentence.Question;
            //parse question
            //entity recognition
            recognitionOutput = ruleEngine.ResolveDimensions(inputSentence, staticResources.Database);
            bool skipMultiDimError = inputSentence.Options == null ? false : inputSentence.Options.SkipMultiDimError;
            var reevaluatedFragments = recognitionOutput.SentenceFragments;
            (ruleEngine as CARuleEngine).TumorNames = GetTumorNames(reevaluatedFragments);
            recognitionOutput.SentenceFragments = reevaluatedFragments;
            recognitionOutput.ParsableSentence = (ruleEngine as CARuleEngine).BuildQuestionInKnownFormat(staticResources.Database, reevaluatedFragments);
            var queries = parser.ParseSentence(recognitionOutput.ParsableSentence, inputSentence, staticResources.StopWordFilter);
            RealizedQuery = inputSentence.Question;
            //set the original question back
            inputSentence.Question = originalQuestion;

            DbQuery = String.Empty;
            foreach (var query in queries)
            {
                DbQuery += query.ToString();
                //Console.WriteLine(query);
            }
        }

        public List<string> GetPhraseHints(string thesaurusPath = null)
        {
            return ruleEngine.GetAllEntityPhrases();
        }

        List<string> GetTumorNames(DataTable dt)
        {
            return dt.Rows.Cast<DataRow>().Select(dr => GetTumorNameFromData(dr)).ToList();
        }

        string GetTumorNameFromData(DataRow row)
        {
            string tumorName = String.Empty;
            if (row.Table.Columns.Contains(CAConstants.DIMENSION1_COMPONENT1))
            {
                string tumorGroupName = row[CAConstants.DIMENSION1_COMPONENT1]?.ToString().Trim();
                if (!String.IsNullOrEmpty(tumorGroupName))
                {
                    tumorName += tumorGroupName;
                    if (row.Table.Columns.Contains(CAConstants.DIMENSION1_COMPONENT2))
                    {
                        string lineName = row[CAConstants.DIMENSION1_COMPONENT2]?.ToString().Trim();
                        if (!String.IsNullOrEmpty(lineName))
                            tumorName += " " + lineName;
                    }
                    if (row.Table.Columns.Contains(CAConstants.DIMENSION1_COMPONENT3))
                    {
                        string segmentName = row[CAConstants.DIMENSION1_COMPONENT3]?.ToString().Trim();
                        if (!String.IsNullOrEmpty(segmentName) && String.Compare(segmentName, CAConstants.DEFAULT_SEGMENT, true) != 0)
                            tumorName += " " + segmentName;
                    }
                    if (row.Table.Columns.Contains(CAConstants.DIMENSION1_COMPONENT4))
                    {
                        string subSegmentName = row[CAConstants.DIMENSION1_COMPONENT4]?.ToString().Trim();
                        if (!String.IsNullOrEmpty(subSegmentName) && String.Compare(subSegmentName, CAConstants.DEFAULT_SUBSEGMENT, true) != 0)
                            tumorName += " " + subSegmentName;
                    }
                    if (row.Table.Columns.Contains(CAConstants.DIMENSION1_COMPONENT5))
                    {
                        string testStatus = row[CAConstants.DIMENSION1_COMPONENT5]?.ToString().Trim();
                        if (!String.IsNullOrEmpty(testStatus) && String.Compare(testStatus, CAConstants.DEFAULT_TEST_STATUS, true) != 0)
                            tumorName += " " + testStatus;
                    }
                }
            }

            return tumorName;
        }

        List<string> GetRegimenNames(DataTable dt)
        {
            if (dt.Columns.Contains(CAConstants.DIMENSION2))
                return dt.Rows.Cast<DataRow>().Select(dr => dr[CAConstants.DIMENSION2].SafeTrim()).ToList();
            return null;
        }

        private void UpdateLinesForTumorsFromDatabase(Dictionary<string, List<string>> allTumorLines,
            List<SentenceFragment> sentenceFragments)
        {
            //tumorLinesFromDatabase: tumor line combo from database to validate tumor-line association predicted from question
            var tumorLinesFromDatabase = new Dictionary<string, List<string>>();
            //allTumorLines: the tumor line combos predicted from question
            var allTumorNames = allTumorLines.Keys.Select(k => k.Trim()).ToList();
            var lineLessTumorNames = new HashSet<string>(allTumorLines.Where(tl => tl.Value.Count == 0).Select(kvp => kvp.Key.Trim()));
            if (!allTumorNames.AnyOrNotNull())
                return;
            int i = 0;
            string commaSeparatedTumors = String.Join(",", allTumorNames.Select(tl => "@tumor" + i++));
            string sql = String.Format("select distinct tumor, line from {0} where tumor in ({1}) group by tumor, line order by tumor, line", ruleEngine.DbEntity,
                commaSeparatedTumors);
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
            using (var conn = new SqlConnection(conStr))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    for (int j = 0; j < allTumorNames.Count; j++)
                    {
                        cmd.Parameters.AddWithValue("tumor" + j, allTumorNames[j]);
                    }
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tumorName = reader["tumor"]?.ToString().Trim();
                            string lineName = reader["line"]?.ToString().Trim();
                            if (!String.IsNullOrWhiteSpace(tumorName) && allTumorLines.ContainsKey(tumorName))
                            {
                                if (lineLessTumorNames.Contains(tumorName))
                                    allTumorLines[tumorName].Add(lineName);
                                else
                                {
                                    if (!tumorLinesFromDatabase.ContainsKey(tumorName))
                                        tumorLinesFromDatabase.Add(tumorName, new List<string>());
                                    tumorLinesFromDatabase[tumorName].Add(lineName);
                                }
                            }
                        }
                    }
                }
            }

            //validate line names predicted from the question -> remove the invalid ones
            foreach (var kvp in tumorLinesFromDatabase)
            {
                var tumorName = kvp.Key;
                if (allTumorLines.ContainsKey(tumorName))
                {
                    var invalidTumorLines = allTumorLines[tumorName].Except(tumorLinesFromDatabase[tumorName]).ToList();
                    foreach (var invalidTumorLine in invalidTumorLines)
                    {
                        allTumorLines[tumorName].Remove(invalidTumorLine);
                        //remove invalid lines from sentence fragmnents as well
                        foreach (var sf in sentenceFragments)
                        {
                            bool isTumorExisting = sf.RecognizedEntities.
                               Any(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT1) == 0 &&
                                               String.Compare(re.Value.SafeTrim(), tumorName, true) == 0);
                            if (isTumorExisting)
                            {
                                sf.RecognizedEntities.
                                   RemoveAll(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT2) == 0 &&
                                                   String.Compare(re.Value.SafeTrim(), invalidTumorLine, true) == 0);
                            }
                        }
                    }
                }
            }
        }

        public ChartResult GetChartAuditData(NaturalLanguageQuestion question)
        {
            var caResult = new ChartResult();
            try
            {
                logger.Info("trying to predict query");
                ruleEngine.TimeSeriesBoundary = GetMonthPastDataAvailableTill();
                PredictQuery(question);
                caResult.RealizedQuery = RealizedQuery;
                using (var dt = ExecuteQuery(DbQuery, chartEngine))
                {
                    var dataSlices = GetDataSlicesFromDataTable(dt);
                    if (!dataSlices.AnyOrNotNull())
                        throw new ChartAuditNoDataException();
                    Visualization chartType = Visualization.None;
                    try
                    {
                        chartType = ruleEngine.ResolveChartTypeFromActualData(dataSlices);
                    }
                    catch(ChartAuditMultiTumorMultiRegimenTimeSeriesException ex)
                    {
                        if (question.Options != null && question.Options.SkipMultiDimError)
                            chartType = Visualization.LineMultiTumorMultiRegimen;
                        else
                            throw ex;
                    }
                    var chart = chartEngine.GetChart(chartType) as CAChartBase;
                    chart.IsPanTumor = (ruleEngine as CARuleEngine).IsPanTumor;
                    chart.TumorNames = GetTumorNames(dt);
                    chart.RegimenNames = GetRegimenNames(dt);
                    chart.Populate(dataSlices);
                    caResult.Status = 0;
                    caResult.Chart = chart;
                }
            }
            catch (ChartAuditExceptionBase caEx)
            {
                caResult.Status = (int)caEx.ErrorCode;
            }
            catch (Exception ex)
            {
                caResult.Status = -1;
                //caResult.RealizedQuery = ex.Message;
            }

            return caResult;
        }

        DataTable ExecuteQuery(string sql, ChartEngineBase chartEngine)
        {
            logger.Info(sql);
            DataTable dt = null;
            sql = sql.Replace("\n", " ").Trim(new char[] { ' ', ';' });
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql + " " + ruleEngine.GetDefaultDimensionOrder(), conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        if (dt.Rows == null || dt.Rows.Count == 0)
                            throw new ChartAuditNoDataException();
                    }
                }
            }

            return dt;
        }

        private List<SentenceFragment> GetDataSlicesFromDataTable(DataTable dt)
        {
            var slices = new List<SentenceFragment>();
            foreach (DataRow row in dt.Rows)
            {
                SentenceFragment sf = new SentenceFragment();
                var ners = new List<RecognizedEntity>();
                if (dt.Columns.Contains(CAConstants.DIMENSION1_COMPONENT1))
                    ners.Add(new RecognizedEntity { Entity = new Tumor(), Value = row[CAConstants.DIMENSION1_COMPONENT1].SafeTrim() });
                if (dt.Columns.Contains(CAConstants.DIMENSION1_COMPONENT2))
                    ners.Add(new RecognizedEntity { Entity = new TumorLine(), Value = row[CAConstants.DIMENSION1_COMPONENT2].SafeTrim() });
                if (dt.Columns.Contains(CAConstants.DIMENSION1_COMPONENT3))
                    ners.Add(new RecognizedEntity { Entity = new TumorSegment(CAConstants.DEFAULT_SEGMENT), Value = row[CAConstants.DIMENSION1_COMPONENT3].SafeTrim() });
                if (dt.Columns.Contains(CAConstants.DIMENSION1_COMPONENT4))
                    ners.Add(new RecognizedEntity { Entity = new TumorSubsegment(CAConstants.DEFAULT_SUBSEGMENT), Value = row[CAConstants.DIMENSION1_COMPONENT4].SafeTrim() });
                if (dt.Columns.Contains(CAConstants.DIMENSION1_COMPONENT5))
                    ners.Add(new RecognizedEntity { Entity = new TumorTestStatus(), Value = row[CAConstants.DIMENSION1_COMPONENT5].SafeTrim() });
                if (dt.Columns.Contains(CAConstants.DIMENSION2))
                    ners.Add(new RecognizedEntity { Entity = new Regimen(), Value = row[CAConstants.DIMENSION2].SafeTrim() });
                if (dt.Columns.Contains(CAConstants.DIMENSION4))
                    ners.Add(new RecognizedEntity { Entity = new Time(), Value = row[CAConstants.DIMENSION4].SafeTrim() });

                var measureRE = recognitionOutput.SentenceFragments[NERClause.EQUAL][0].RecognizedEntities.
                    Where(re => re.IsMeasure).
                    FirstOrDefault().
                    Clone() as RecognizedEntity;
                var measureField = measureRE.Entity.FieldName;
                double measureVal = Math.Round(row[measureField].SafeToDouble() * 100, CAConstants.CHART_LABEL_PRECISION);
                measureRE.Value = measureVal;
                measureRE.RecognizedValue = measureVal;
                ners.Add(measureRE);

                if (ners.AnyOrNotNull())
                    sf.AddOrReplaceNERs(ners);
                slices.Add(sf);
            }

            return slices;
        }

        /// <summary>
        /// gets the month of latest available data, subject to a maximum of current month
        /// </summary>
        /// <returns></returns>
        DateRange GetMonthPastDataAvailableTill()
        {
            DateTime dataAvailableTill = DateTime.Now;
            string sql = String.Format("select max(monthyear) from {0}", ruleEngine.DbEntity);
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
            using (var conn = new SqlConnection(conStr))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    var obj = cmd.ExecuteScalar();
                    if (!DateTime.TryParse(obj?.ToString(), out dataAvailableTill))
                        dataAvailableTill = DateTime.Now;
                }
            }

            return new DateRange(null, dataAvailableTill); //TODO: also populate data beginning
        }

        List<string> GetTumorNames(Dictionary<NERClause, List<SentenceFragment>> sentenceFragments)
        {
            var includeedTumorNames = GetTumorNamesFromClausedFragments(sentenceFragments[NERClause.EQUAL]);
            var excludeedTumorNames = GetTumorNamesFromClausedFragments(sentenceFragments[NERClause.NOTEQUAL]);
            return includeedTumorNames.Except(excludeedTumorNames).ToList();
        }

        List<string> GetTumorNamesFromClausedFragments(List<SentenceFragment> sentenceFragments)
        {
            Dictionary<string, List<string>> tumorLines = GetTumorWithLines(sentenceFragments);

            var res = sentenceFragments.SelectMany(sf => GetTumorLineCombos(sf, tumorLines).
                                        Select(tumorLine => tumorLine + " " +
                                        sf.RecognizedEntities.Where(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT3, true) == 0).
                                            FirstOrDefault()?.RecognizedValue.ToString() + " " +
                                        sf.RecognizedEntities.Where(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT4, true) == 0).
                                            FirstOrDefault()?.RecognizedValue.ToString() + " " +
                                        sf.RecognizedEntities.Where(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT5, true) == 0).
                                            FirstOrDefault()?.RecognizedValue.ToString())).
                                      Distinct().
                                      ToList();

            return res;
        }

        List<string> GetTumorLineCombos(SentenceFragment sf, Dictionary<string, List<string>> tumorLines)
        {
            var tumorLineCombos = new List<string>();
            string lineName = sf.RecognizedEntities.
                Where(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT2, true) == 0).
                FirstOrDefault()?.
                Value.
                ToString();
            string tumorName = sf.RecognizedEntities.
                Where(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT1, true) == 0).
                FirstOrDefault()?.
                Value.
                ToString();

            if (!String.IsNullOrWhiteSpace(lineName))
                tumorLineCombos.Add(tumorName + " " + lineName);
            else if (!String.IsNullOrWhiteSpace(tumorName) && tumorLines.ContainsKey(tumorName))
            {
                foreach (var line in tumorLines[tumorName])
                {
                    if (String.IsNullOrWhiteSpace(line))
                        tumorLineCombos.Add(tumorName);
                    else
                        tumorLineCombos.Add(tumorName + " " + line);
                }
            }

            return tumorLineCombos;
        }

        Dictionary<string, List<string>> GetTumorWithLines(List<SentenceFragment> sentenceFragments)
        {
            var res = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var sf in sentenceFragments)
            {
                sf.RecognizedEntities = sf.RecognizedEntities.OrderBy(re => re.Order).ToList();
                string tumorName = null;
                foreach (var re in sf.RecognizedEntities)
                {
                    //if tumor
                    if (String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT1, true) == 0)
                    {
                        tumorName = re.Value?.ToString();
                        if (!String.IsNullOrWhiteSpace(tumorName) && !res.ContainsKey(tumorName))
                            res.Add(tumorName, new List<string>());
                    }
                    //if line
                    if (!String.IsNullOrWhiteSpace(tumorName) && String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT2, true) == 0)
                    {
                        string lineName = re.Value?.ToString();
                        if (!String.IsNullOrWhiteSpace(lineName) && !res[tumorName].Contains(lineName))
                            res[tumorName].Add(lineName);
                    }
                }
            }
            UpdateLinesForTumorsFromDatabase(res, sentenceFragments);

            return res;
        }


        public List<string> GetSuggestedQuestions(string phrase)
        {
            return GetPhraseSuggestions(phrase);
        }
        
        public List<string> GetThumbnail(string qaIDs)
        {
            List<string> res = new List<string>();
            string spName = "[dbo].[uspGetThumbnailsByID]";
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
            using (var conn = new SqlConnection(conStr))
            {
                using (var cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var param1 = cmd.Parameters.Add("@IDs", SqlDbType.Int);
                    param1.Direction = ParameterDirection.Input;
                    param1.Value = qaIDs;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string base64Str = null;
                            try
                            {
                                base64Str = Convert.ToBase64String((byte[])(reader["Thumbnail"]));
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Could not cast thumbnail to string : " + ex.ToString());
                            }
                            if (!String.IsNullOrEmpty(base64Str))
                                res.Add(base64Str);
                        }
                    }
                }
            }

            return res;
        }

        public void StoreQuestionWithAnswer(QuestionWithAnswer qa)
        {
            //string spName = "[dbo].[uspInsertUserSearchDetails]";
            string spName = "[dbo].[uspInsertUserSearchDetailsNarrativeThumbnail]";
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    using (SqlCommand cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param1 = cmd.Parameters.Add("@UserID", SqlDbType.Int);
                        param1.Direction = ParameterDirection.Input;
                        param1.Value = qa.UserId;
                        var param2 = cmd.Parameters.Add("@SearchContent", SqlDbType.NVarChar, 1000);
                        param2.Direction = ParameterDirection.Input;
                        param2.Value = qa.Question;
                        var param3 = cmd.Parameters.Add("@Narrative", SqlDbType.NVarChar);
                        param3.Direction = ParameterDirection.Input;
                        param3.Value = qa.Narrative;
                        var param4 = cmd.Parameters.Add("@Thumbnail", SqlDbType.VarBinary);
                        param4.Direction = ParameterDirection.Input;
                        param4.Value = Convert.FromBase64String(qa.Snapshot);
                        var param5 = cmd.Parameters.Add("@Caption", SqlDbType.NVarChar, 1000);
                        param5.Direction = ParameterDirection.Input;
                        param5.Value = qa.Caption;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                //just log and continue
                //throw ex;
            }
        }

        public List<QuestionWithAnswer> RetrieveQuestionsWithAnswers(int userId)
        {
            var res = new List<QuestionWithAnswer>();
            //string spName = "[dbo].[uspGetThumbnails]";
            string spName = "[dbo].[uspGetQuestionAnswerDetails]";
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
            using (var conn = new SqlConnection(conStr))
            {
                using (var cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var param1 = cmd.Parameters.Add("@UserID", SqlDbType.Int);
                    param1.Direction = ParameterDirection.Input;
                    param1.Value = userId;
                    var param2 = cmd.Parameters.Add("@IsGlobal", SqlDbType.Int);
                    param2.Direction = ParameterDirection.Input;
                    param2.Value = 1;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new QuestionWithAnswer
                            {
                                UId = reader["ID"].SafeToNum(),
                                Question = reader["Caption"]?.ToString().Trim(),
                                Narrative = reader["Narrative"]?.ToString().Trim(),
                                Snapshot = String.Empty//Convert.ToBase64String((byte[])(reader["Thumbnail"]))
                            });
                        }
                    }
                }
            }

            return res;
        }

        public List<string> GetPhraseSuggestions(string phrase)
        {
            var suggestions = new HashSet<string>();
            string spName = "[dbo].[uspGetPhraseForAutoComplete]";
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
            try
            {
                using (DataSet ds = new DataSet())
                {
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        using (SqlCommand cmd = new SqlCommand(spName, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            var param1 = cmd.Parameters.Add("@Phrase", SqlDbType.NVarChar, 1000);
                            param1.Direction = ParameterDirection.Input;
                            param1.Value = phrase;
                            var param2 = cmd.Parameters.Add("@Count", SqlDbType.Int);
                            param2.Direction = ParameterDirection.Input;
                            param2.Value = 10; //put a const instead

                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(ds);
                                if (ds.Tables != null && ds.Tables.Count > 0)
                                {
                                    foreach (DataRow row in ds.Tables[0].Rows)
                                    {
                                        string suggestion = row["KeyPhrase"]?.ToString();
                                        if (!String.IsNullOrWhiteSpace(suggestion) && !suggestions.Contains(suggestion))
                                            suggestions.Add(suggestion);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //just log and continue
                //throw ex;
            }

            if (!suggestions.AnyOrNotNull())
            {
                var lastWordMatch = new Regex(@"(\w+)$").Match(phrase);
                if (lastWordMatch.Success)
                {
                    suggestions = GetWordSuggestions(lastWordMatch.Value);
                }
            }

            return suggestions.ToList();
        }

        private HashSet<string> GetWordSuggestions(string word)
        {
            var suggestions = new HashSet<string>();
            string spName = "[dbo].[uspGetWordForAutoComplete]";
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    using (SqlCommand cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param1 = cmd.Parameters.Add("@Word", SqlDbType.NVarChar, 100);
                        param1.Direction = ParameterDirection.Input;
                        param1.Value = word;
                        var param2 = cmd.Parameters.Add("@Count", SqlDbType.Int);
                        param2.Direction = ParameterDirection.Input;
                        param2.Value = 10; //put a const instead

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(ds);
                            if (ds.Tables != null && ds.Tables.Count > 0)
                            {
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    string suggestion = row["KeyPhrase"]?.ToString();
                                    if (!String.IsNullOrWhiteSpace(suggestion) && !suggestions.Contains(suggestion))
                                        suggestions.Add(suggestion);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //just log and continue
                //throw ex;
            }

            return suggestions;
        }
        
    }
}
