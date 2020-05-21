
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PharmaACE.NLP.ChartAudit.NLIDB;
using PharmaACE.NLP.Framework;
using PharmaACE.NLP.Responder;

namespace PharmaACE.NLP.Tests
{
    [TestClass]
    public class ChartAuditNLIDBTest
    {
        string contentPath;
        string databasePath;
        List<string> thesaurusPaths;
        string langPath;
        string stopwordsPath;

        public ChartAuditNLIDBTest()
        {
            contentPath = @"..\..\..\PharmaACE.NLP.QuestionAnswerService\Content\Store\";
            databasePath = contentPath + "chartaudit_shmeasure.sql"; //"ChartAudit.sql";
            thesaurusPaths = new List<string> { contentPath + "th_english.dat", contentPath + "th_chartaudit.dat" };
            langPath = contentPath + "english.csv";
            stopwordsPath = "english.txt";

            StaticResources.GetInstance(databasePath, langPath, thesaurusPaths, null);
        }

        [TestMethod]
        public void NoMeasureNoDimension()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "how are we doing?" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0); //count should be equal to pan tumor count as defined by domain
        }

        [TestMethod]
        public void NoMeasureNoDimensionTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "how's the trend?" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 3); //multiple tumor multiple regimen exception
            //shoot the same question asking to skip the error and get the result anyway
            question = new NaturalLanguageQuestion
            {
                Question = "how's the trend?"
               ,Options = new Options { SkipMultiDimError = true }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineMultiTumorMultiRegimen);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count > 0); //1L, 2L, 3L == 3 Tumors
            Assert.AreEqual((caData.Chart as SharesLineChartGroup).Caption, "Pan Tumor Share");
            Assert.IsFalse((caData.Chart as SharesLineChartGroup).LineCharts.Any(lc => lc.DataPoints.Count != 12)); //last year by default
        }

        [TestMethod]
        public void SingularMeasureNoOtherDimension()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "share" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0); //count should be equal to pan tumor count as defined by domain
        }

        [TestMethod]
        public void PluralMeasureNoOtherDimension()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "shares" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0); //count should be equal to pan tumor count as defined by domain
        }

        [TestMethod]
        public void PanTumorSingleRegimenSingleTime()
        {
            string monthyearLabel = DateTime.Now.AddYears(-1).ToString("MMMM yyyy");
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "Opdivo mono shares in " + monthyearLabel };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0); //count should be equal to pan tumor count as defined by domain
            Assert.IsFalse((caData.Chart as SharesStackedBarChart).StackedBars.Any(sb => sb.Stacks.Count > 2)); //max stack count = max regimen count + 'others'
        }

        [TestMethod]
        public void MultipleTumorMultipleRegimenTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "Opdivo and keytruda shares" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 3); //multiple tumor multiple regimen exception
            //shoot the same question asking to skip the error and get the result anyway
            question = new NaturalLanguageQuestion { Question = "Opdivo and keytruda shares"
            , Options = new Options { SkipMultiDimError = true }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineMultiTumorMultiRegimen);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count > 0); //1L, 2L, 3L == 3 Tumors
            Assert.AreEqual((caData.Chart as SharesLineChartGroup).Caption, "Pan Tumor shares");
            Assert.IsFalse((caData.Chart as SharesLineChartGroup).LineCharts.Any(lc => lc.DataPoints.Count != 12)); //last year by default
        }

        [TestMethod]
        public void PanTumorMultipleRegimenSingleTime()
        {
            string monthyearLabel = DateTime.Now.AddYears(-1).ToString("MMM yyyy");
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "Opdivo and keytruda shares for " + monthyearLabel };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0);
            //opdivo = 1, keytruda => k, k+y, k+c = 3, others = 1; opdivo => o, o+y, o+c; max stack count = max regimen count + 'others'
            Assert.IsFalse((caData.Chart as SharesStackedBarChart).StackedBars.Any(sb => sb.Stacks.Count > 7));
        }

        [TestMethod]
        public void PanTumorNoRegimenSingleTime()
        {
            string monthyearLabel = DateTime.Now.AddYears(-1).ToString("MMM yyyy");
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "shares for " + monthyearLabel };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0); //count should be equal to pan tumor count as defined by domain
        }

        [TestMethod]
        public void PanTumorNoRegimenTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "what are my shares last year?" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 3); //multiple tumor multiple regimen exception
        }

        [TestMethod]
        public void SingleTumorSingleRegimenSingleTime()
        {
            string monthyearLabel = DateTime.Now.AddYears(-1).ToString("MMM yyyy");
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "NSCLC 1L opdivo mono shares " + monthyearLabel };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == 2);
        }

        [TestMethod]
        public void SingleTumorGroupSingleRegimenSingleTime()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "NSCLC Opdivo mono shares latest month" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0); //1L, 2L, 3L == 3 Tumors
        }

        [TestMethod]
        public void SingleTumorGroupSingleRegimenTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "NSCLC only Opdivo shares" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleRegimen);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count > 0); //1L, 2L, 3L == 3 Tumors
            Assert.AreEqual((caData.Chart as SharesLineChartGroup).Caption, "Opdivo shares");
            Assert.IsFalse((caData.Chart as SharesLineChartGroup).LineCharts.Any(lc => lc.DataPoints.Count != 12)); //last year by default
        }

        [TestMethod]
        public void SingleTumorSingleTestStatusCompoundRegimenSingleTime()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "How is opdivo share in PDl1 positive pool in NSCLC 1L" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts.Count > 0); //1L, 2L, 3L == 3 Tumors
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == 4); //last year by default
        }

        [TestMethod]
        public void SingleTumorSingleTestStatusSingleTimeRate()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "What is the rate of positivity in lungs first line" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is IRPieChartGroup);
            Assert.IsTrue((caData.Chart as IRPieChartGroup).PieCharts.Count == 1);
            Assert.IsTrue((caData.Chart as IRPieChartGroup).PieCharts[0].Slices.Count == 2);
        }

        [TestMethod]
        public void MultipleTumorSingleTestStatusSingleTimeRate()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "Pdl1 positivity in lungs first line" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is IRPieChartGroup);
            Assert.IsTrue((caData.Chart as IRPieChartGroup).PieCharts.Count == 1);
            Assert.IsTrue((caData.Chart as IRPieChartGroup).PieCharts[0].Slices.Count == 2);
        }

        [TestMethod]
        public void SingleTumorMultipleTestStatusCompoundRegimenTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "Share in PDl1 positive, negative, UnTested patients in NSCLC 1L" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0); //1L, 2L, 3L == 3 Tumors
        }

        [TestMethod]
        public void SingleTumorSingleRegimenTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "NSCLC 1L Opdivo only shares" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleTumor);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count == 1);
            Assert.AreEqual((caData.Chart as SharesLineChartGroup).Caption, "NSCLC 1L shares");
            Assert.IsFalse((caData.Chart as SharesLineChartGroup).LineCharts.Any(lc => lc.DataPoints.Count != 12)); //last year by default
        }

        [TestMethod]
        public void SingleTumorMultipleRegimenTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "NSCLC 1L opdivo and keytruda shares" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleTumor);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count == 6); //opdivo => O, O+Y, O+Chemo; Keytruda => K, K+Y, k+C
            Assert.AreEqual((caData.Chart as SharesLineChartGroup).Caption, "NSCLC 1L shares");
            Assert.IsFalse((caData.Chart as SharesLineChartGroup).LineCharts.Any(lc => lc.DataPoints.Count != 12)); //last year by default
        }

        [TestMethod]
        public void SingleTumorNoRegimenTimeSeries()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "NSCLC 1L shares" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleTumor);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count > 0);
            Assert.AreEqual((caData.Chart as SharesLineChartGroup).Caption, "NSCLC 1L shares");
            Assert.IsFalse((caData.Chart as SharesLineChartGroup).LineCharts.Any(lc => lc.DataPoints.Count != 12)); //last year by default
        }

        [TestMethod]
        public void RefineSingleTumorSingleRegimenSingleTimeAddSameRegimen()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018 for vegf"
                ,RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "vegf" } }
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == 2);
        }

        [TestMethod]
        public void RefineSingleTumorSingleRegimenSingleTimeAddDifferentRegimen()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018 for vegf"
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "opdivo mono" } }
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == 3);
        }

        [TestMethod]
        public void RefineSingleTumorSingleRegimenSingleTimeAddDifferentRegimensOneByOne()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018 for vegf"
                , RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "opdivo mono" } }
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                , RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "keytruda mono" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == 4);
        }

        [TestMethod]
        public void RefineSingleTumorSingleRegimenSingleTimeAddDifferentRegimensTogether()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018 for vegf"
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "opdivo mono and keytruda mono" } }
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == 4);
        }

        [TestMethod]
        public void RefineSingleTumorSingleRegimenSingleTimeRemoveLastRegimen()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018 for vegf"
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Remove, Question = "vegf" } }
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 5);
        }

        [TestMethod]
        public void RefineSingleTumorCoupleRegimenSingleTimeRemoveSingleRegimen()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018 for vegf and opdivo mono"
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Remove, Question = "vegf" } }
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == 2);
        }

        [TestMethod]
        public void RefineSingleTumorNoRegimenSingleTimeRemoveSingleRegimen()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018"
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            int intialCount = (caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count;

            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Remove, Question = "vegf" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == intialCount - 1);
        }        

        [TestMethod]
        public void RefineSingleTumorNoRegimenSingleTimeRemoveDifferentRegimensOneByOne()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018"
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            var initialCount = (caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count;

            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Remove, Question = "opdivo mono" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == initialCount - 1);

            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Remove, Question = "keytruda mono" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == initialCount - 2);
        }

        [TestMethod]
        public void RefineSingleTumorNoRegimenSingleTimeAddDifferentRegimensTogether()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018"
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            int initialCount = (caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count;

            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "opdivo mono and keytruda mono" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == initialCount);
        }

        [TestMethod]
        public void RefineSingleTumorSingleRegimenSingleTimeAddSingleRegimenThenRemoveThatRegimen()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares march 2018 for vegf"
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            var initialCount = (caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count;

            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                , RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "opdivo mono" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == initialCount + 1);

            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Remove, Question = "opdivo mono" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is SharesPieChartGroup);
            Assert.IsTrue((caData.Chart as SharesPieChartGroup).PieCharts[0].Slices.Count == initialCount);
        }

        [TestMethod]
        public void RefineSingleTumorSingleRegimenSingleTimeRemoveSingleRegimenThenAddThatRegimen()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion
            {
                Question = "sclc 2l shares last 6 months"
            };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            var initialCount = (caData.Chart as SharesLineChartGroup).LineCharts.Count;
            
            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Remove, Question = "only keytruda" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleTumor);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count == initialCount - 1);

            question = new NaturalLanguageQuestion
            {
                Question = caData.RealizedQuery
                ,
                RelatedQuestions = new List<Refiner> { new Refiner { Operation = Operation.Add, Question = "only keytruda" } }
            };
            caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleTumor);
            Assert.IsTrue(caData.Chart is SharesLineChartGroup);
            Assert.IsTrue((caData.Chart as SharesLineChartGroup).LineCharts.Count == initialCount);
        }

        [TestMethod]
        public void ThesaurusTest()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "head + Neck shares last month" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is SharesStackedBarChart);
            Assert.IsFalse((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Count > 0);
            Assert.IsTrue((caData.Chart as SharesStackedBarChart).StackedBars.Any(sb => sb.Stacks.Count > 2)); //max stack count = max regimen count + 'others'
        }

        [TestMethod]
        public void SingleTumorSegmentSingleTimeTestingRate()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "what is the PDL1 testing rate in 2L lung" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is TRPieChartGroup);
            Assert.IsFalse((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as TRPieChartGroup).PieCharts.Count > 0);
            Assert.IsTrue((caData.Chart as TRPieChartGroup).PieCharts.Any(pie => pie.Slices.Count == 2));
        }

        [TestMethod]
        public void SingleTumorSegmentTimeSeriesTestingRate()
        {
            //measure plurality should prevail even in case of grammatical mistakes 
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "what is the PDL1 testing rates in 2L lung" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleTumor);
            Assert.IsTrue(caData.Chart is TRLineChartGroup);
            Assert.IsFalse((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as TRLineChartGroup).LineCharts.Count == 1);
            Assert.IsFalse((caData.Chart as TRLineChartGroup).LineCharts.Any(lc => lc.DataPoints.Count != 12)); //last year by default
        }

        [TestMethod]
        public void MultipleTumorSegmentSingleTimeTestingRate()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "what is the PDL1 testing rate in lung" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.Pie);
            Assert.IsTrue(caData.Chart is TRPieChartGroup);
            Assert.IsFalse((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as TRPieChartGroup).PieCharts[0].Slices.Count == 2);
        }

        [TestMethod]
        public void SingleTumorSingleTimeTestingRate()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "Rate of testing in nsclc 1l" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is TRStackedBarChart);
            Assert.IsFalse((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as TRStackedBarChart).StackedBars.Count > 0); //equal to number of segments
            Assert.IsTrue((caData.Chart as TRStackedBarChart).StackedBars.Any(sb => sb.Stacks.Count == 2)); //max stack count = tested + untested
        }

        [TestMethod]
        public void MultipleTumorSingleSegmentSingleTimeTestingRate()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "What is pdl1 testing rate in nsclc 1l and 2l" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.StackedBar);
            Assert.IsTrue(caData.Chart is TRStackedBarChart);
            Assert.IsFalse((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as TRStackedBarChart).StackedBars.Count > 0); //equal to number of segments
            Assert.IsTrue((caData.Chart as TRStackedBarChart).StackedBars.Any(sb => sb.Stacks.Count == 2)); //max stack count = tested + untested
        }

        [TestMethod]
        public void MultipleTumorTimeSeriesTestingRate()
        {
            NaturalLanguageQuestion question = new NaturalLanguageQuestion { Question = "what are the PDL1 testing rates in lung" };
            var caData = new ResponseBuilder(question).GetChartAuditData(question);
            Assert.AreEqual(caData.Status, 0);
            Assert.IsTrue(caData.Chart.ChartType == Visualization.LineSingleRegimen);
            Assert.IsTrue(caData.Chart is TRLineChartGroup);
            Assert.IsFalse((caData.Chart as CAChartBase).IsPanTumor);
            Assert.IsTrue((caData.Chart as TRLineChartGroup).LineCharts.Count > 0);
        }

        [TestMethod]
        public void RetrieveQuestionsWithAnswersTest()
        {
            var ruleEngine = new ResponseBuilder(null);
            var qaList = ruleEngine.RetrieveQuestionsWithAnswers(55);
        }

        [TestMethod]
        public void SuggestTest()
        {
            SuggestionInput phrase = new SuggestionInput { Key = "p", Count = 5 };
            var ruleEngine = new ResponseBuilder(null);
            var suggestions = ruleEngine.GetSuggestedQuestions(phrase?.Key);
        }

        [TestMethod]
        public void StoreTest()
        {
            string q = "keytruda share last month";
            var ruleEngine = new ResponseBuilder(new NaturalLanguageQuestion { Question = q });
            ruleEngine.StoreQuestionWithAnswer(new QuestionWithAnswer { Caption = "abc", Narrative = "def ghi", Question = q, Snapshot = "junk", UId = 1, UserId = 55 }); //TODO: return some status code
        }
    }
}

