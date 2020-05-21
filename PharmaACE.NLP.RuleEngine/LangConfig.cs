using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PharmaACE.NLP.Framework
{
    public class LangConfig
    {
        public List<string> AvgKeywords { get; set; }
        public List<string> SumKeywords { get; set; }
        public List<string> MaxKeywords { get; set; }
        public List<string> MinKeywords { get; set; }
        public List<string> CountKeywords { get; set; }
        public List<string> JunctionKeywords { get; set; }
        public List<string> DisjunctionKeywords { get; set; }
        public List<string> GreaterKeywords { get; set; }
        public List<string> LessKeywords { get; set; }
        public List<string> GreaterThanKeywords { get; set; }
        public List<string> LessThanKeywords { get; set; }
        public List<string> BetweenKeywords { get; set; }
        public List<string> OrderByKeywords { get; set; }
        public List<string> AscKeywords { get; set; }
        public List<string> DescKeywords { get; set; }
        public List<string> GroupByKeywords { get; set; }
        public List<string> NegationKeywords { get; set; }
        public List<string> EqualKeywords { get; set; }
        public List<string> LikeKeywords { get; set; }
        public List<string> DistinctKeywords { get; set; }

        public LangConfig()
        {
            AvgKeywords = new List<string>();
            SumKeywords = new List<string>();
            MaxKeywords = new List<string>();
            MinKeywords = new List<string>();
            CountKeywords = new List<string>();
            JunctionKeywords = new List<string>();
            DisjunctionKeywords = new List<string>();
            GreaterKeywords = new List<string>();
            LessKeywords = new List<string>();
            GreaterThanKeywords = new List<string>();
            LessThanKeywords = new List<string>();
            BetweenKeywords = new List<string>();
            OrderByKeywords = new List<string>();
            AscKeywords = new List<string>();
            DescKeywords = new List<string>();
            GroupByKeywords = new List<string>();
            NegationKeywords = new List<string>();
            EqualKeywords = new List<string>();
            LikeKeywords = new List<string>();
            DistinctKeywords = new List<string>();
        }

        string RemoveAccents(string text)
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

        static string GeneratePath(string path)
        {
            //get the full location of the assembly with DaoTests in it
            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Thesaurus)).Location;

            //get the folder that's in
            string theDirectory = Path.GetDirectoryName(fullPath);

            string fileName = Path.Combine(theDirectory, path);
            return fileName;
        }

        public void Load(string path)
        {
            var content = File.ReadAllLines(path);
            int counter = 0;
            AvgKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            SumKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            MaxKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            MinKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            CountKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            JunctionKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(" " + s.Trim() + " ")).ToList();
            DisjunctionKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(" " + s.Trim() + " ")).ToList();
            GreaterKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            LessKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            GreaterThanKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            LessThanKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            BetweenKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            OrderByKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            AscKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            DescKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            GroupByKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            NegationKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            EqualKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            LikeKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
            DistinctKeywords = content[counter++].Replace(':', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => RemoveAccents(s.Trim())).ToList();
        }
    }
}
