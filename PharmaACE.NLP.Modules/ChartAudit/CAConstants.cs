using System.Collections.Generic;
using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public class CAConstants
    {
        public const string DIMENSION2 = "Regimen";
        public const string DIMENSION1_COMPONENT1 = "Tumor";
        public const string DIMENSION1_COMPONENT2 = "Line";
        public const string DIMENSION1_COMPONENT3 = "Segment";
        public const string DIMENSION1_COMPONENT4 = "SubSegment";
        public const string DIMENSION1_COMPONENT5 = "Status";
        public const string DIMENSION4 = "Monthyear";

        public const string MEASURE1_FIELD = "Share";
        public const string MEASURE2_FIELD = "ShareR2M";
        public const string MEASURE3_FIELD = "ShareR3M";
        //displayname of measures
        public const string MEASURE1 = "Monthly Share"; //field name Share
        public const string MEASURE2 = "Share"; //field name ShareR2M
        public const string MEASURE3 = "ShareR3M"; //field name ShareR3M
        public const string MEASURE4 = "Monthly Testing Rate"; //field name Share
        public const string MEASURE5 = "Testing Rate"; //field name ShareR2M
        public const string MEASURE6 = "Testing Rate R3M"; //field name ShareR3M
        public const string MEASURE7 = "Monthly Rate"; //field name Share
        public const string MEASURE8 = "Rate"; //field name ShareR2M
        public const string MEASURE9 = "Rate R3M"; //field name ShareR3M
        public const string DEFAULT_MEASURE = MEASURE2;
        public const string DEFAULT_SEGMENT = "Total";
        public const string DEFAULT_SUBSEGMENT = "Total";
        public const string DEFAULT_TEST_STATUS = null;

        public const string PAN_TUMOR = "Pan Tumor";
        public const string RESIDUAL_SLICE = "Others";

        public const string SHARE_VIEW = "shmeasure";
        public const string TR_VIEW = "trmeasure";
        public const string ITR_VIEW = "itrmeasure";

        public static List<Measure> AllMeasures { get {
                return new List<Measure>
                {
                    new Share(),
                    new R2MShare(),
                    new R3MShare(),
                    new TR(),
                    new R2MTR(),
                    new R3MTR()
                }; } }

        public static List<string> INCLUDERS = new List<string> { "with", "including", "plus" };
        public static List<string> EXCLUDERS = new List<string> { "without", "excluding", "except", "minus" };

        public const int CHART_LABEL_PRECISION = 1;
    }
}
