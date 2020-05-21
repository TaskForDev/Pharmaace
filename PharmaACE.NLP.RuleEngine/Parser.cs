using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PharmaACE.Utility;

namespace PharmaACE.NLP.Framework
{
    public class Parser
    {
        const string maverickjoy_general_assigner = "*res*@3#>>*";
        const string maverickjoy_like_assigner = "*like*@3#>>*";

        public Database DatabaseObject { get; }
        public Dictionary<string, List<string>> DatabaseDico { get; set; }
        public List<string> AvgKeywords { get; set; }
        public List<string> SumKeywords { get; set; }
        public List<string> MaxKeywords { get; set; }
        public List<string> MinKeywords { get; set; }
        public List<string> CountKeywords { get; set; }
        public List<string> JunctionKeywords { get; set; }
        public List<string> DisjunctionKeywords { get; set; }
        public static List<string> GreaterKeywords { get; set; }
        public static List<string> LessKeywords { get; set; }
        public static List<string> GreaterThanKeywords { get; set; }
        public static List<string> LessThanKeywords { get; set; }
        public List<string> BetweenKeywords { get; set; }
        public List<string> OrderByKeywords { get; set; }
        public List<string> AscKeywords { get; set; }
        public List<string> DescKeywords { get; set; }
        public List<string> GroupByKeywords { get; set; }
        public static List<string> NegationKeywords { get; set; }
        public static List<string> EqualKeywords { get; set; }
        public static List<string> LikeKeywords { get; set; }
        public List<string> DistinctKeywords { get; set; }

        public Parser(Database database, LangConfig config)
        {
            DatabaseObject = database;
            DatabaseDico = DatabaseObject.GetTablesIntoDictionary();
            AvgKeywords = config.AvgKeywords;
            SumKeywords = config.SumKeywords;
            MaxKeywords = config.MaxKeywords;
            MinKeywords = config.MinKeywords;
            CountKeywords = config.CountKeywords;
            JunctionKeywords = config.JunctionKeywords;
            DisjunctionKeywords = config.DisjunctionKeywords;
            GreaterKeywords = config.GreaterKeywords;
            LessKeywords = config.LessKeywords;
            GreaterThanKeywords = config.GreaterThanKeywords;
            LessThanKeywords = config.LessThanKeywords;
            BetweenKeywords = config.BetweenKeywords;
            OrderByKeywords = config.OrderByKeywords;
            AscKeywords = config.AscKeywords;
            DescKeywords = config.DescKeywords;
            GroupByKeywords = config.GroupByKeywords;
            NegationKeywords = config.NegationKeywords;
            EqualKeywords = config.EqualKeywords;
            LikeKeywords = config.LikeKeywords;
            DistinctKeywords = config.DistinctKeywords;
        }

        static int SpecialCompare(string s1, string s2)
        {
            var s1Parts = s1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var s2Parts = s2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (s1Parts.Length == s2Parts.Length)
            {
                if (s1.Length >= s2.Length)
                    return 1;
                else
                    return -1;
            }
            else
            {
                if (s1Parts.Length > s2Parts.Length)
                    return 1;
                else
                    return -1;
            }
        }

        static void TransformationSort(List<string> transitionList)
        {
            //Sort on basis of two keys split length and then token lengths in the respective priority.
            transitionList.Sort(SpecialCompare);
        }

        static string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormKD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolvedSentence">input sentence after dimension resolution</param>
        /// <param name="question"></param>
        /// <param name="stopWordsFilter"></param>
        /// <returns></returns>
        public List<Query> ParseSentence(string resolvedSentence, NaturalLanguageQuestion question, StopWordFilter stopWordsFilter = null)
        {
            int tableCount = 0;
            int selectCount = 0;
            int whereCount = 0;
            int lastTablePosition = 0;
            List<string> selectColumns = new List<string>();
            List<string> whereColumns = new List<string>();
            
            //single table optimization
            //if (DatabaseObject.IsSingleTable)
            //    sentence += " from " + DatabaseObject.Tables[0].Name;
            if (stopWordsFilter != null)
                resolvedSentence = stopWordsFilter.Filter(resolvedSentence);

            //-------------------------------------------------------Collect whereValueColumns-------------------------------------------------------------------
            string inputForFindingValue = resolvedSentence.TrimEnd(new char[] { '!', '#', '$', '%', '&', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>'
                , '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~' });
            List<string> whereValueColumns = new List<string>();
            var filterList = new List<string> { ",", "!" };

            foreach (var filterElement in filterList)
                inputForFindingValue = inputForFindingValue.Replace(filterElement, " ");

            var inputWordList = inputForFindingValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int tempWhereColumnCount = 0;
            int tempTableCount = 0;
            int tempLastTablePosition = 0;
            List<string> startPhase = new List<string> { "" };
            List<string> medPhase = new List<string> { "" };

            // TODO: merge this part of the algorithm (detection of values of where)
            // in the rest of the parsing algorithm (about line 725) '''

            for (int i = 0; i < inputWordList.Length; i++)
            {
                foreach (var kvp in DatabaseDico)
                {
                    string tableName = kvp.Key;
                    //if (String.Compare(inputWordList[i], tableName, true) == 0 || DatabaseObject.GetTableByName(tableName).Equivalences.Contains(inputWordList[i], StringComparer.OrdinalIgnoreCase))
                    if(Util.IsMatchingEntity(inputWordList[i], tableName, DatabaseObject.GetTableByName(tableName).Equivalences))
                    {
                        if (tempTableCount == 0)
                            startPhase = inputWordList.Take(i).ToList();
                        tempTableCount++;
                        tempLastTablePosition = i;
                    }

                    var columns = DatabaseObject.GetTableByName(tableName).Columns;
                    foreach (var column in columns)
                    {
                        //if (String.Compare(inputWordList[i], column.Name, true) == 0 || (column.Equivalences != null && column.Equivalences.Contains(inputWordList[i], StringComparer.OrdinalIgnoreCase)))
                        if(Util.IsMatchingEntity(inputWordList[i], column.Name, column.Equivalences))
                        {
                            if (tempWhereColumnCount == 0)
                                medPhase = inputWordList.Take(tempLastTablePosition + 1).Skip(startPhase.Count).ToList();
                            tempWhereColumnCount++;
                            break;
                        }
                        else
                        {
                            if (tempTableCount != 0 && tempWhereColumnCount == 0 && i == inputWordList.Length - 1)
                            {
                                medPhase = inputWordList.Skip(startPhase.Count).ToList();
                            }
                        }
                    }
                    if (tempWhereColumnCount == 0)
                        continue;
                    break;
                }
            }

            var endPhrase = inputWordList.Skip(startPhase.Count + medPhase.Count).ToList();
            List<string> parsedWhereColumns = null;
            FindWhereValueColumns(endPhrase, ref whereValueColumns, ref parsedWhereColumns);
            /// ----------------------------------------------------------------------------------------------------------- ///

            List<string> fromTables = new List<string>();
            List<string> selectPhrase = new List<string>();
            List<string> fromPhrase = new List<string>();
            List<string> wherePhrase = new List<string>();

            //match whole words which sometimes can contain PHRASE_MAKER
            var words = SplitSentence(resolvedSentence);
            for (int i = 0; i < words.Count; i++)
            {
                foreach (var kvp in DatabaseDico)
                {
                    string tableName = kvp.Key;
                    //if(String.Compare(words[i], tableName, true) == 0 || DatabaseObject.GetTableByName(tableName).Equivalences.Contains(words[i], StringComparer.OrdinalIgnoreCase))
                    if (Util.IsMatchingEntity(words[i], tableName, DatabaseObject.GetTableByName(tableName).Equivalences))
                    {
                        if (tableCount == 0)
                            selectPhrase = words.Take(i).ToList();
                        fromTables.Add(tableName);
                        tableCount++;
                        lastTablePosition = i;
                    }
                    var columns = DatabaseObject.GetTableByName(tableName).Columns;
                    foreach (var column in columns)
                    {
                        //if(String.Compare(words[i], column.Name, true) == 0 || (column.Equivalences != null && column.Equivalences.Contains(words[i], StringComparer.OrdinalIgnoreCase)))
                        if (Util.IsMatchingEntity(words[i], column.Name, column.Equivalences))
                        {
                            if(tableCount == 0)
                            {
                                selectColumns.Add(column.Name);
                                selectCount++;
                            }
                            else if(parsedWhereColumns.Contains(column.Name))
                            {
                                if(whereCount == 0)
                                {
                                    fromPhrase = words.Take(lastTablePosition + 1).Skip(selectPhrase.Count).ToList();
                                }
                                whereColumns.Add(column.Name);
                                whereCount++;
                            }
                            break;
                        }
                        else
                        {
                            if (tableCount != 0 && whereCount == 0 && i == words.Count - 1)
                                fromPhrase = words.Skip(selectPhrase.Count).ToList();
                        }
                    }
                }
            }

            wherePhrase = words.Skip(selectPhrase.Count + fromPhrase.Count).ToList();

            if (selectCount + tableCount + whereCount == 0)
                throw new Exception("No keyword found in sentence!");

            if(fromTables.Count > 0)
            {
                var fromPhrases = new List<List<string>>();
                int previousIndex = 0;
                for(int i = 0; i < fromPhrase.Count; i++)
                {
                    foreach (var tableName in fromTables)
                    {
                        //if(String.Compare(fromPhrase[i], table, true) == 0 || DatabaseObject.GetTableByName(table).Equivalences.Contains(fromPhrase[i], StringComparer.OrdinalIgnoreCase))
                        if (Util.IsMatchingEntity(fromPhrase[i], tableName, DatabaseObject.GetTableByName(tableName).Equivalences))
                        {
                            fromPhrases.Add(fromPhrase.Take(i + 1).Skip(previousIndex).ToList());
                            previousIndex = i + 1;
                        }
                    }
                }

                int lastJunctionWordIndex = -1;

                for(int i = 0; i < fromPhrases.Count; i++)
                {
                    int junctionWordsCount = 0;
                    int disjunctionWordsCount = 0;

                    foreach (var word in fromPhrases[i])
                    {
                        if(JunctionKeywords.Contains(word))
                            junctionWordsCount++;
                        if (DisjunctionKeywords.Contains(word))
                            disjunctionWordsCount++;
                    }
                    if (junctionWordsCount + disjunctionWordsCount > 0)
                        lastJunctionWordIndex = i;
                }
                if(lastJunctionWordIndex == -1)
                {
                    fromPhrase = fromPhrases.Take(1).SelectMany(x => x).ToList();
                    wherePhrase = fromPhrases.Skip(1).SelectMany(x => x).Concat(wherePhrase).ToList();
                }
                else
                {
                    fromPhrase = fromPhrases.Take(lastJunctionWordIndex + 1).SelectMany(x => x).ToList();
                    wherePhrase = fromPhrases.Skip(lastJunctionWordIndex + 1).SelectMany(x => x).Concat(wherePhrase).ToList();
                }
            }

            var realFromTables = new List<string>();
            foreach (var word in fromPhrase)
            {
                foreach (var tableName in fromTables)
                {
                    //if (String.Compare(word, table, true) == 0 || DatabaseObject.GetTableByName(table).Equivalences.Contains(word, StringComparer.OrdinalIgnoreCase))
                    if (Util.IsMatchingEntity(word, tableName, DatabaseObject.GetTableByName(tableName).Equivalences))
                        realFromTables.Add(tableName);
                }
            }

            fromTables = realFromTables;
            if (fromTables.Count == 0)
                throw new Exception("No table name found in sentence!");

            var groupByPhrase = new List<List<string>>();
            var orderByPhrase = new List<List<string>>();
            var newWherePhrase = new List<List<string>>();
            int prevIndex = 0;
            int prevPhraseType = 0;
            int yetWhere = 0;

            for(int i = 0; i < wherePhrase.Count; i++)
            {
                if (OrderByKeywords.Contains(wherePhrase[i]))
                {
                    if(yetWhere > 0)
                    {
                        if (prevPhraseType == 1)
                            orderByPhrase.Add(wherePhrase.Take(i).Skip(prevIndex).ToList());
                        else if(prevPhraseType == 2)
                            groupByPhrase.Add(wherePhrase.Take(i).Skip(prevIndex).ToList());
                    }
                    else
                        newWherePhrase.Add(wherePhrase.Take(i).Skip(prevIndex).ToList());

                    prevIndex = i;
                    prevPhraseType = 1;
                    yetWhere++;
                }
                if (GroupByKeywords.Contains(wherePhrase[i]))
                {
                    if(yetWhere > 0)
                    {
                        if(prevPhraseType == 1)
                            orderByPhrase.Add(wherePhrase.Take(i).Skip(prevIndex).ToList());
                        else if (prevPhraseType == 2)
                            groupByPhrase.Add(wherePhrase.Take(i).Skip(prevIndex).ToList());
                    }
                    else
                        newWherePhrase.Add(wherePhrase.Take(i).Skip(prevIndex).ToList());

                    prevIndex = i;
                    prevPhraseType = 2;
                    yetWhere++;
                }
            }
            if (prevPhraseType == 1)
                orderByPhrase.Add(wherePhrase.Skip(prevIndex).ToList());
            else if (prevPhraseType == 2)
                groupByPhrase.Add(wherePhrase.Skip(prevIndex).ToList());
            else
                newWherePhrase.Add(wherePhrase);
            
            try
            {
                var selectParser = new SelectParser(selectColumns, fromTables, selectPhrase, CountKeywords, SumKeywords, AvgKeywords, MaxKeywords, MinKeywords, DistinctKeywords, DatabaseDico, DatabaseObject);
                var fromParser = new FromParser(fromTables, selectColumns, whereColumns, DatabaseObject);
                var whereParser = new WhereParser(newWherePhrase, fromTables, whereValueColumns, CountKeywords, SumKeywords, AvgKeywords, MaxKeywords, 
                    MinKeywords, GreaterKeywords, LessKeywords, GreaterThanKeywords, LessThanKeywords, BetweenKeywords, NegationKeywords, JunctionKeywords, 
                    DisjunctionKeywords, LikeKeywords, DistinctKeywords, DatabaseDico, DatabaseObject);
                var groupByParser = new GroupByParser(groupByPhrase, fromTables, DatabaseDico, DatabaseObject);
                var orderByParser = new OrderByParser(orderByPhrase, fromTables, AscKeywords, DescKeywords, DatabaseDico, DatabaseObject);

                var queries = fromParser.Run();
                var selectObjects = selectParser.Run();
                var whereObjects = whereParser.Run();

                for (int i = 0; i < queries.Count; i++)
                {
                    var query = queries[i];
                    query.Select = selectObjects[i];
                    query.Where = whereObjects[i];
                }

                return queries;
            }
            catch(Exception ex)
            {
                throw new Exception(String.Format("Parsing error occured in thread: {0}", ex.ToString()));
            }
        }

        internal static List<string> SplitSentence(string sentence)
        {
            return Regex.Matches(RemoveAccents(sentence), String.Format(@"[{0}\{1}{2}]+[{3}]", Util.PHRASE_MAKER, Util.DATE_MAKER, Util.WORD_MAKER_START, 
                Util.WORD_MAKER_END)).Cast<Match>().Select(m => m.Value).ToList();
        }

        internal static void FindWhereValueColumns(List<string> endPhrase, ref List<string> whereValueColumns, ref List<string> whereColumns)
        {
            whereColumns = new List<string>();
            whereValueColumns = new List<string>();
            var filterList = new List<string> { ",", "!" };
            var irext = String.Join(" ", endPhrase);

            //todo set this part of the algorithm (detection of values of where) in the WhereParser thread

            if (!String.IsNullOrEmpty(irext))
            {
                var filter_list = new List<string> { ",", "!" };
                foreach (var filterElement in filterList)
                {
                    irext = irext.Replace(filterElement, " ");
                }

                var assignmentList = new List<string>();
                assignmentList.AddRange(EqualKeywords);
                assignmentList.AddRange(LikeKeywords);
                assignmentList.AddRange(GreaterKeywords);
                assignmentList.AddRange(LessKeywords);
                assignmentList.AddRange(GreaterThanKeywords);
                assignmentList.AddRange(LessThanKeywords);
                assignmentList.AddRange(NegationKeywords);

                // As these words can also be part of assigners

                // custom operators added as they can be possibilities
                assignmentList.Add(":");
                assignmentList.Add("=");

                // Algorithmic logic for best substitution for extraction of values with the help of assigners.
                TransformationSort(assignmentList);

                for (int idx = 0; idx < assignmentList.Count; idx++)
                {
                    var assigner = assignmentList[idx];
                    if (LikeKeywords.Contains(assigner))
                    {
                        assigner = " " + assigner + " ";
                        irext = irext.Replace(assigner, " " + maverickjoy_like_assigner + " ");
                    }
                    else
                    {
                        assigner = " " + assigner + " ";
                        // Reason for adding " " these is according to the LOGIC implemented assigner operators help us extract the value,
                        // hence they should be independent entities not part of some other big entity else logic will fail.
                        // for eg -> "show data for city where cityName where I like to risk my life  is Pune" will end up extacting ,
                        // 'k' and '1' both. I know its a lame sentence but something like this could be a problem.
                        irext = irext.Replace(assigner, " " + maverickjoy_general_assigner + " ");
                    }
                }

                // replace all spaces from values to PHRASE_MAKER for proper value assignment in SQL
                // eg. (where name is 'abc def') -> (where name is abcPHRASE_MAKERdef)
                //foreach (Match match in Regex.Matches(irext, "(['\"].*?['\"])"))
                //{
                //    irext = irext.Replace(match.Value, match.Value.Replace(" ", "PHRASE_MAKER").Replace("'", String.Empty).Replace("\"", String.Empty));
                //}
                var irextList = irext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                
                for (int idx = 0; idx < irextList.Length; idx++)
                {
                    int index = idx + 1;
                    var x = irextList[idx];
                    if (String.Compare(x, maverickjoy_like_assigner) == 0)
                    {
                        if (index < irextList.Length && String.Compare(irextList[index], maverickjoy_like_assigner) != 0 && String.Compare(irextList[index], maverickjoy_general_assigner) != 0)
                        {
                            whereValueColumns.Add("%" + irextList[index] + "%");
                            whereColumns.Add(irextList[idx - 1]);
                        }
                    }
                    if (String.Compare(x, maverickjoy_general_assigner) == 0)
                    {
                        if (index < irextList.Length && String.Compare(irextList[index], maverickjoy_like_assigner) != 0 && String.Compare(irextList[index], maverickjoy_general_assigner) != 0)
                        {
                            whereValueColumns.Add(irextList[index]);
                            whereColumns.Add(irextList[idx - 1]);
                        }
                    }
                }
            }
        }
    }

    internal class OrderByParser
    {
        private List<string> ascKeywords;
        private Dictionary<string, List<string>> databaseDico;
        private Database databaseObject;
        private List<string> descKeywords;
        private List<string> fromTables;
        private List<List<string>> orderByPhrase;

        public OrderByParser(List<List<string>> orderByPhrase, List<string> fromTables, List<string> ascKeywords, List<string> descKeywords, Dictionary<string, List<string>> databaseDico, Database databaseObject)
        {
            this.orderByPhrase = orderByPhrase;
            this.fromTables = fromTables;
            this.ascKeywords = ascKeywords;
            this.descKeywords = descKeywords;
            this.databaseDico = databaseDico;
            this.databaseObject = databaseObject;
        }
    }

    internal class GroupByParser
    {
        private Dictionary<string, List<string>> databaseDico;
        private Database databaseObject;
        private List<string> fromTables;
        private List<List<string>> groupByPhrase;

        public GroupByParser(List<List<string>> groupByPhrase, List<string> fromTables, Dictionary<string, List<string>> databaseDico, Database databaseObject)
        {
            this.groupByPhrase = groupByPhrase;
            this.fromTables = fromTables;
            this.databaseDico = databaseDico;
            this.databaseObject = databaseObject;
        }
    }

    internal class WhereParser
    {
        private List<string> avgKeywords;
        private List<string> betweenKeywords;
        private List<string> countKeywords;
        private Dictionary<string, List<string>> databaseDico;
        private Database databaseObject;
        private List<string> disjunctionKeywords;
        private List<string> distinctKeywords;
        private List<string> fromTables;
        private List<string> greaterKeywords;
        private List<string> greaterThanKeywords;
        private List<string> junctionKeywords;
        private List<string> lessKeywords;
        private List<string> lessThanKeywords;
        private List<string> likeKeywords;
        private List<string> maxKeywords;
        private List<string> minKeywords;
        private List<string> negationKeywords;
        private List<List<string>> Phrases;
        private List<string> sumKeywords;
        private List<string> whereValueColumns;

        private List<int> countKeywordOffset;
        private List<int> sumKeywordOffset;
        private List<int> avgKeywordOffset;
        private List<int> maxKeywordOffset;
        private List<int> minKeywordOffset;
        private List<int> greaterKeywordOffset;
        private List<int> greaterThanKeywordOffset;
        private List<int> lessKeywordOffset;
        private List<int> lessThanKeywordOffset;
        private List<int> betweenKeywordOffset;
        private List<int> junctionKeywordOffset;
        private List<int> disjunctionKeywordOffset;
        private List<int> negationKeywordOffset;
        private List<int> likeKeywordOffset;

        private List<Where> whereObjects;

        public WhereParser(List<List<string>> phrases, List<string> fromTables, List<string> whereValueColumns, List<string> countKeywords, 
            List<string> sumKeywords, List<string> avgKeywords, List<string> maxKeywords, List<string> minKeywords, List<string> greaterKeywords, 
            List<string> lessKeywords, List<string> greaterThanKeywords, List<string> lessThanKeywords, List<string> betweenKeywords, 
            List<string> negationKeywords, List<string> junctionKeywords, List<string> disjunctionKeywords, List<string> likeKeywords, 
            List<string> distinctKeywords, Dictionary<string, List<string>> databaseDico, 
            Database databaseObject)
        {
            this.Phrases = phrases;
            this.fromTables = fromTables;
            this.whereValueColumns = whereValueColumns;
            this.countKeywords = countKeywords;
            this.sumKeywords = sumKeywords;
            this.avgKeywords = avgKeywords;
            this.maxKeywords = maxKeywords;
            this.minKeywords = minKeywords;
            this.greaterKeywords = greaterKeywords;
            this.lessKeywords = lessKeywords;
            this.greaterThanKeywords = greaterThanKeywords;
            this.lessThanKeywords = lessThanKeywords;
            this.betweenKeywords = betweenKeywords;
            this.negationKeywords = negationKeywords;
            this.junctionKeywords = junctionKeywords;
            this.disjunctionKeywords = disjunctionKeywords;
            this.likeKeywords = likeKeywords;
            this.distinctKeywords = distinctKeywords;
            this.databaseDico = databaseDico;
            this.databaseObject = databaseObject;

            countKeywordOffset = new List<int>();
            sumKeywordOffset = new List<int>();
            avgKeywordOffset = new List<int>();
            maxKeywordOffset = new List<int>();
            minKeywordOffset = new List<int>();
            greaterKeywordOffset = new List<int>();
            lessKeywordOffset = new List<int>();
            greaterThanKeywordOffset = new List<int>();
            lessThanKeywordOffset = new List<int>();
            betweenKeywordOffset = new List<int>();
            junctionKeywordOffset = new List<int>();
            disjunctionKeywordOffset = new List<int>();
            negationKeywordOffset = new List<int>();
            likeKeywordOffset = new List<int>();

            whereObjects = new List<Where>();
        }

        public List<Where> Run()
        {
            int whereColumnCount = 0;
            List<string> whereColumns = new List<string>();
            var offsetOf = new Dictionary<string, int>();
            var columnOffset = new List<int>();

            for (int j = 0; j < Phrases.Count; j++)
            {
                var phrase = Phrases[j];
                string phraseOffset = String.Empty;
                //for (int i = 0; i < phrase.Count; i++)
                //{
                //    foreach (var kvp in databaseDico)
                //    {
                //        string tableName = kvp.Key;
                //        var columns = databaseObject.GetTableByName(tableName).Columns;
                //        foreach (var column in columns)
                //        {
                //            //if (String.Compare(phrase[i], column.Name, true) == 0 || (column.Equivalences != null && column.Equivalences.Contains(phrase[i], StringComparer.OrdinalIgnoreCase)))
                //            if (Util.IsMatchingEntity(phrase[i], column.Name, column.Equivalences))
                //            {
                //                whereColumnCount++;
                //                whereColumns.Add(column.Name);
                //                offsetOf[phrase[i]] = i;
                //                columnOffset.Add(i);
                //                break;
                //            }
                //        }
                //        if (whereColumnCount == 0)
                //            continue;
                //        break;
                //    }
                //}
                
                //the following is no more needed with the advent of RuleEngine
                //for (int i = 1; i < columnOffset.Count; i++)
                //{
                //    int offset = columnOffset[i];
                //    int lastOffset = columnOffset[i - 1];
                //    if (String.Compare(phrase[offset], phrase[lastOffset], true) == 0)
                //    {
                //        List<string> parentChain = CollectParents(phrase, lastOffset);
                //        if (parentChain != null && parentChain.Count > 0)
                //        {
                //            phrase.InsertRange(offset, parentChain);
                //            //update columnOffset
                //            for (int k = 0; k < columnOffset.Count; k++)
                //            {
                //                if (columnOffset[k] > offset)
                //                    columnOffset[k] += parentChain.Count;
                //            }
                //        }
                //    }
                //}

                whereColumnCount = 0;
                List<string> parsedWhereColumns = null;
                whereColumns = new List<string>();
                offsetOf = new Dictionary<string, int>();
                columnOffset = new List<int>();
                //refresh whereValueColumns
                Parser.FindWhereValueColumns(phrase, ref whereValueColumns, ref parsedWhereColumns);

                for (int i = 0; i < phrase.Count; i++)
                {
                    foreach (var kvp in databaseDico)
                    {
                        string tableName = kvp.Key;
                        var columns = databaseObject.GetTableByName(tableName).Columns;
                        foreach (var column in columns)
                        {
                            //if (String.Compare(phrase[i], column.Name, true) == 0 || (column.Equivalences != null && column.Equivalences.Contains(phrase[i], StringComparer.OrdinalIgnoreCase)))
                            if (Util.IsMatchingEntity(phrase[i], column.Name, column.Equivalences) && parsedWhereColumns.Contains(column.Name, StringComparer.OrdinalIgnoreCase))
                            {
                                whereColumnCount++;
                                whereColumns.Add(column.Name);
                                offsetOf[phrase[i]] = i;
                                columnOffset.Add(i);
                                break;
                            }
                        }
                        if (whereColumnCount == 0)
                            continue;
                        break;
                    }

                    string phraseKeyword = phrase[i].ToLower(); //for robust keyword matching
                    phraseOffset += phraseKeyword + " ";

                    foreach (var keyword in countKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                countKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in sumKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                sumKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in avgKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                avgKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in maxKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                maxKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in minKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                minKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in greaterKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                greaterKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in lessKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                lessKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in greaterThanKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                greaterThanKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in lessThanKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                lessThanKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in betweenKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                betweenKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in junctionKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length == phraseOffset.Length)
                                junctionKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in disjunctionKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length == phraseOffset.Length)
                                disjunctionKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in negationKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword, StringComparison.InvariantCultureIgnoreCase);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                negationKeywordOffset.Add(i);
                        }
                    }

                    foreach (var keyword in likeKeywords)
                    {
                        int keywordIndex = phraseOffset.LastIndexOf(keyword);
                        if (keywordIndex >= 0) //before the column
                        {
                            if (keywordIndex + keyword.Length + 1 == phraseOffset.Length)
                                likeKeywordOffset.Add(i);
                        }
                    }
                }
            }

            foreach (var fromTable in fromTables)
            {
                var whereObject = new Where();
                for(int i = 0; i < columnOffset.Count; i++)
                {
                    int current = columnOffset[i];
                    int prev = 0;
                    int next = 0;
                    if (i == 0)
                        prev = 0;
                    else
                        prev = columnOffset[i - 1];
                    if (i == columnOffset.Count - 1)
                        next = 999;//int.MaxValue; //arbitrary max number
                    else
                        next = columnOffset[i + 1];

                    string junction = PredictJunction(prev, current);
                    string column = GetColumnNameWithAliasTable(whereColumns[i], fromTable);
                    string operationType = PredictOperationType(prev, current);

                    string value = String.Empty;
                    if (whereValueColumns.Count > i)
                        value = whereValueColumns[whereValueColumns.Count - whereColumns.Count + i];
                    else
                        value = "OOV"; //Out Of Vocabulary: default value

                    string oprtr = PredictOperator(current, next);
                    whereObject.Conditions.Add(new Tuple<string, Condition>(junction, new Condition {
                        Column = column, ColumnType = operationType, Operator = oprtr, Value = value }));
                }
                whereObjects.Add(whereObject);
            }

            return whereObjects;
        }

        private string PredictOperator(int current, int next)
        {
            List<int> intervalOffset = new List<int>();
            for (int i = current; i < next; i++)
                intervalOffset.Add(i);
            if (Intersect(intervalOffset, negationKeywordOffset).Count > 0 && Intersect(intervalOffset, greaterKeywordOffset).Count > 0)
                return "<";
            if (Intersect(intervalOffset, negationKeywordOffset).Count > 0 && Intersect(intervalOffset, lessKeywordOffset).Count > 0)
                return ">";
            if (Intersect(intervalOffset, greaterKeywordOffset).Count > 0)
                return ">";
            if (Intersect(intervalOffset, lessKeywordOffset).Count > 0)
                return "<";
            if (Intersect(intervalOffset, greaterThanKeywordOffset).Count > 0)
                return ">=";
            if (Intersect(intervalOffset, lessThanKeywordOffset).Count > 0)
                return "<=";
            if (Intersect(intervalOffset, betweenKeywordOffset).Count > 0)
                return "BETWEEN";
            if (Intersect(intervalOffset, negationKeywordOffset).Count > 0)
                return "!=";
            if (Intersect(intervalOffset, likeKeywordOffset).Count > 0)
                return "LIKE";

            return "=";
        }

        private string PredictOperationType(int prev, int current)
        {
            var intervalOffset = new List<int>();
            for (int i = prev; i < current; i++)
                intervalOffset.Add(i);
            if (Intersect(intervalOffset, countKeywordOffset).Count > 0)
                return "COUNT";
            if (Intersect(intervalOffset, sumKeywordOffset).Count > 0)
                return "SUM";
            if (Intersect(intervalOffset, avgKeywordOffset).Count > 0)
                return "AVG";
            if (Intersect(intervalOffset, maxKeywordOffset).Count > 0)
                return "MAX";
            if (Intersect(intervalOffset, minKeywordOffset).Count > 0)
                return "MIN";

            return null;
        }

        private List<string> GetTablesOfColumn(string column)
        {
            List<string> tables = new List<string>();
            foreach (var kvp in databaseDico)
            {
                string table = kvp.Key;
                if (databaseDico[table].Contains(column))
                    tables.Add(table);
            }

            return tables;
        }

        private string GetColumnNameWithAliasTable(string column, string fromTable)
        {
            var oneTableOfColumn = GetTablesOfColumn(column)[0];
            var tablesOfColumn = GetTablesOfColumn(column);
            if (tablesOfColumn.Contains(fromTable))
                return fromTable + "." + column;
            else
                return oneTableOfColumn + "." + column;
        }

        private string PredictJunction(int prev, int current)
        {
            var intervalOffset = new List<int>();
            for(int i = prev; i < current; i++)
            {
                intervalOffset.Add(i);
            }

            List<int> disjunctionIntersect = Intersect(intervalOffset, disjunctionKeywordOffset);
            if (disjunctionIntersect.Count > 0)
                return "OR";
            else
            {
                //List<int> junctionIntersect = Intersect(intervalOffset, junctionKeywordOffset);
                //if (junctionIntersect.Count > 0)
                    return "AND";
            }

            int firstEncounteredJunctionOffset = -1;
            int firstEncounteredDisjunctionOffset = -1;

            foreach (var offset in junctionKeywordOffset)
            {
                if (offset >= current)
                {
                    firstEncounteredJunctionOffset = offset;
                    break;
                }
            }
            foreach (var offset in disjunctionKeywordOffset)
            {
                if (offset >= current)
                {
                    firstEncounteredDisjunctionOffset = offset;
                    break;
                }
            }

            if (firstEncounteredJunctionOffset >= firstEncounteredDisjunctionOffset)
                return "AND";
            else
                return "OR";
        }

        private List<int> Intersect(List<int> intervalOffset, List<int> disjunctionKeywordOffset)
        {
            return intervalOffset.Intersect(disjunctionKeywordOffset).Distinct().ToList();
        }
    }

    class FromParser
    {
        private Database databaseObject;
        private List<string> fromTables;
        private List<string> selectColumns;
        private List<string> whereColumns;
        private Dictionary<string, List<string>> databaseDico;
        private List<Query> queries;

        public FromParser(List<string> fromTables, List<string> selectColumns, List<string> whereColumns, Database databaseObject)
        {
            this.fromTables = fromTables;
            this.selectColumns = selectColumns;
            this.whereColumns = whereColumns;
            this.databaseObject = databaseObject;
            this.databaseDico = databaseObject.GetTablesIntoDictionary();
            this.queries = new List<Query>();
        }

        List<string> GetTablesOfColumn(string column)
        {
            var tempTable = new List<string>();
            foreach (var kvp in databaseDico)
            {
                var table = kvp.Key;
                if (kvp.Value.Contains(column))
                    tempTable.Add(table);
            }
            return tempTable;
        }
        
        List<Tuple<string, string>> IsDirectJoinPossible(string srcTable, string trgtTable)
        {
            var srcTableFKColumns = databaseObject.GetForeignKeysOfTable(srcTable);
            var trgtTableFKColumns = databaseObject.GetForeignKeysOfTable(trgtTable);
            foreach (var column in srcTableFKColumns)
            {
                if (column.Foreign != null && String.Compare(column.Foreign.ForeignTable, trgtTable, true) == 0)
                    return new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>(srcTable, column.Name),
                        new Tuple<string, string>(trgtTable, column.Foreign.ForeignColumn)
                    };
            }
            foreach (var column in trgtTableFKColumns)
            {
                if (column.Foreign != null && String.Compare(column.Foreign.ForeignTable, srcTable, true) == 0)
                    return new List<Tuple<string, string>>
                    {   
                        new Tuple<string, string>(srcTable, column.Foreign.ForeignColumn),
                        new Tuple<string, string>(trgtTable, column.Name)
                    };
            }
            // pk_table_src = self.database_object.get_primary_key_names_of_table(table_src)
            // pk_table_trg = self.database_object.get_primary_key_names_of_table(table_trg)
            // match_pk_table_src_with_table_trg = self.intersect(pk_table_src, self.database_dico[table_trg])
            // match_pk_table_trg_with_table_src = self.intersect(pk_table_trg, self.database_dico[table_src])

            // if len(match_pk_table_src_with_table_trg) >= 1:
            //     return [(table_trg, match_pk_table_src_with_table_trg[0]), (table_src, match_pk_table_src_with_table_trg[0])]
            // elif len(match_pk_table_trg_with_table_src) >= 1:
            // return [(table_trg, match_pk_table_trg_with_table_src[0]),
            // (table_src, match_pk_table_trg_with_table_src[0])]

            return null;
        }

        List<List<Tuple<string, string>>> GetDirectlyRelatedTables(string srcTable)
        {
            var links = new List<List<Tuple<string, string>>>();
            foreach (var kvp in databaseDico)
            {
                var trgtTable = kvp.Key;
                if (String.Compare(trgtTable, srcTable, true) != 0)
                {
                    var link = IsDirectJoinPossible(srcTable, trgtTable);
                    if (link != null)
                        links.Add(link);
                }
            }

            return links;
        }

        KeyValuePair<int, List<Tuple<string, string>>> IsJoin(List<string> historic, string srcTable, string trgtTable)
        {
            var links = GetDirectlyRelatedTables(srcTable);
            var differences = new List<List<Tuple<string, string>>>();

            foreach (var join in links)
            {
                if (!historic.Contains(join[0].Item1))
                    differences.Add(join);
            }
            links = differences;

            foreach (var join in links)
            {
                if (String.Compare(join[1].Item1, trgtTable, true) == 0)
                {
                    var res = new KeyValuePair<int, List<Tuple<string, string>>>(0, join);
                    return res;
                }
            }

            var path = new KeyValuePair<int, List<Tuple<string, string>>>();
            historic.Add(srcTable);

            foreach (var join in links)
            {
                var result = IsJoin(historic, join[1].Item1, trgtTable);
                if(result.Value != null || result.Value.Count > 0)
                {
                   path = new KeyValuePair<int, List<Tuple<string, string>>>(1, join);
                }
            }
            return path;
        }

        List<Tuple<string, string>> GetLink(string srcTable, string trgtTable)
        {
            var path = IsJoin(new List<string>(), srcTable, trgtTable);
            path.Value.Reverse();

            return path.Value;
        }
        
        public List<Query> Run()
        {
            foreach (var fromTable in fromTables)
            {
                var links = new List<List<Tuple<string, string>>>();
                var query = new Query();
                query.From = new From { Table = fromTable };
                var joinObject = new Join();

                foreach (var column in selectColumns)
                {
                    if (!databaseDico[fromTable].Contains(column))
                    {
                        var foreignTable = GetTablesOfColumn(column)[0];
                        joinObject.Tables.Add(foreignTable);
                        var link = GetLink(fromTable, foreignTable);

                        if (link == null)
                            throw new Exception("There is at least column `" + column + "` that is unreachable from table `" + fromTable.ToUpper() + "`!");
                        else
                            links.Add(link);
                    }
                }

                foreach (var column in whereColumns)
                {
                    if (!databaseDico[fromTable].Contains(column))
                    {
                        var foreignTable = GetTablesOfColumn(column)[0];
                        joinObject.Tables.Add(foreignTable);
                        var link = GetLink(fromTable, foreignTable);

                        if (link == null)
                            throw new Exception("There is at least column `" + column + "` that is unreachable from table `" + fromTable.ToUpper() + "`!");
                        else
                            links.Add(link);
                    }
                }

                joinObject.Links = links;
                query.Join = joinObject;
                queries.Add(query);
            }

            return queries;
        }

    }

    class SelectParser
    {
        private List<string> avgKeywords;
        private List<string> countKeywords;
        private Dictionary<string, List<string>> databaseDico;
        private Database databaseObject;
        private List<string> distinctKeywords;
        private List<string> fromTables;
        private List<string> maxKeywords;
        private List<string> minKeywords;
        private List<string> selectColumns;
        private List<string> selectPhrase;
        private List<string> sumKeywords;
        private List<Select> selectObjects;

        public SelectParser(List<string> selectColumns, List<string> fromTables, List<string> selectPhrase, List<string> countKeywords, List<string> sumKeywords, List<string> avgKeywords, List<string> maxKeywords, List<string> minKeywords, List<string> distinctKeywords, Dictionary<string, List<string>> databaseDico, Database databaseObject)
        {
            this.selectColumns = selectColumns;
            this.fromTables = fromTables;
            this.selectPhrase = selectPhrase;
            this.countKeywords = countKeywords;
            this.sumKeywords = sumKeywords;
            this.avgKeywords = avgKeywords;
            this.maxKeywords = maxKeywords;
            this.minKeywords = minKeywords;
            this.distinctKeywords = distinctKeywords;
            this.databaseDico = databaseDico;
            this.databaseObject = databaseObject;
            this.selectObjects = new List<Select>();
        }

        List<string> GetTablesOfColumn(string column)
        {
            List<string> tempTable = new List<string>();
            foreach (var kvp in databaseDico)
            {
                var table = kvp.Key;
                if (kvp.Value.Contains(column))
                    tempTable.Add(table);
            }

            return tempTable;
        }

        string GetColumnNameWithAliasTable(string column, string fromTable)
        {
            var oneTableOfColumn = GetTablesOfColumn(column)[0];
            var tablesOfColumn = GetTablesOfColumn(column);
            if (tablesOfColumn.Contains(fromTable))
                return fromTable + "." + column;
            else
                return oneTableOfColumn + "." + column;
        }

        List<T> Uniquify<T>(List<T> list)
        {
            List<T> already = new List<T>();
            foreach (var element in list)
            {
                if (!already.Contains(element))
                    already.Add(element);
            }

            return already;
        }

        public List<Select> Run()
        {
            foreach (var fromTable in fromTables)
            {
                var selectObject = new Select();
                selectColumns = Uniquify<string>(selectColumns);
                if(selectColumns.Count == 0)
                {
                    var selectType = new List<string>();
                    foreach (var countKeyword in countKeywords)
                    {
                        //if count_keyword in (word.lower() for word in self.phrase):
                        // so that it matches multiple words too in keyword synonymn in .lang rather than just single word for COUNT
                        // (e.g. QUERY-> "how many city there are in which the employe name is aman ?" )
                        var lowerPhrase = String.Join(" ", selectPhrase.Select(word => word.ToLower()));
                        if (lowerPhrase.Contains(countKeyword))
                            selectType.Add("COUNT");
                    }
                    selectObject.AddColumn(null, Uniquify<string>(selectType));
                }
                else
                {
                    var selectPhrases = new List<List<string>>();
                    int prevIndex = 0;
                    for(int i = 0; i < selectPhrase.Count; i++)
                    {
                        foreach (var columnName in selectColumns)
                        {
                            Column column = databaseObject.GetColumnByName(columnName);
                            //if (String.Compare(selectPhrase[i], columnName, true) == 0 || (column != null && column.Equivalences != null && column.Equivalences.Contains(selectPhrase[i], StringComparer.OrdinalIgnoreCase)))
                            if (Util.IsMatchingEntity(selectPhrase[i], column.Name, column.Equivalences))
                            {
                                selectPhrases.Add(selectPhrase.Take(i + 1).Skip(prevIndex).ToList());
                                prevIndex = i + 1;
                            }
                        }
                    }

                    selectPhrases.Add(selectPhrase.Skip(prevIndex).ToList());

                    for(int i = 0; i < selectPhrases.Count; i++) //for each select phrase (i.e. column processing)
                    {
                        List<string> selectType = new List<string>();
                        var phrase = selectPhrases[i].Select(word => word.ToLower()).ToList();

                        foreach (var keyword in avgKeywords)
                        {
                            if (phrase.Contains(keyword))
                                selectType.Add("AVG");
                        }
                        foreach (var keyword in countKeywords)
                        {
                            if (phrase.Contains(keyword))
                                selectType.Add("COUNT");
                        }
                        foreach (var keyword in maxKeywords)
                        {
                            if (phrase.Contains(keyword))
                                selectType.Add("MAX");
                        }
                        foreach (var keyword in minKeywords)
                        {
                            if (phrase.Contains(keyword))
                                selectType.Add("MIN");
                        }
                        foreach (var keyword in sumKeywords)
                        {
                            if (phrase.Contains(keyword))
                                selectType.Add("SUM");
                        }
                        foreach (var keyword in distinctKeywords)
                        {
                            if (phrase.Contains(keyword))
                                selectType.Add("DISTINCT");
                        }

                        if(i != selectPhrases.Count - 1)
                        {
                            var column = GetColumnNameWithAliasTable(selectColumns[i], fromTable);
                            selectObject.AddColumn(column, Uniquify<string>(selectType));
                        }
                    }
                }
                selectObjects.Add(selectObject);
            }

            return selectObjects;
        }

    }
}
