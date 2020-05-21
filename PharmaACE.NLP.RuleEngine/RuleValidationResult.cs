using System;

namespace PharmaACE.NLP.Framework
{
    public class RuleValidationResult
    {
        public Exception Error { get; set; }
        public Visualization Viz { get; set; }
    }

    public enum Visualization
    {
        None,
        Table,
        LineSingleRegimen,
        LineSingleTumor,
        StackedBar,
        Pie,
        LineMultiTumorMultiRegimen
    }
}