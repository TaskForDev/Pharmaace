using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaACE.NLP.Framework
{
    public class NaturalLanguageQuestion
    {
        public string Question { get; set; }
        public List<Refiner> RelatedQuestions { get; set; }
        public int UserId { get; set; }
        public Options Options { get; set; }
    }

    public class Options
    {
        public bool SkipMultiDimError { get; set; }
    }

    public class SuggestionInput
    {
        public string Key { get; set; }
        public int Count { get; set; }
    }

    public class QuestionWithAnswer
    {
        public int UId { get; set; }
        public int UserId { get; set; }
        public string Question { get; set; }
        public string Snapshot { get; set; }
        public string Caption { get; set; }
        public string Narrative { get; set; }
    }
}