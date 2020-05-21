using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    public class StopWordFilter
    {
        public List<string> StopWordList { get; set; }

        public StopWordFilter()
        {
            StopWordList = new List<string>();
        }

        public string Filter(string sentence)
        {
            string tempSentence = String.Empty;
            var matches = Regex.Matches(RemoveAccents(sentence), @"[\w]+");
            foreach (Match match in matches)
            {
                string word = RemoveAccents(match.Value);
                if (StopWordList.Contains(word, StringComparer.OrdinalIgnoreCase))
                    sentence = sentence.Replace(match.Value, String.Empty);
            }

            return String.Join(" ", sentence.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim();
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
            var words = File.ReadAllLines(path);
            foreach (var word in words)
            {
                var stopword = RemoveAccents(word).ToLower();
                StopWordList.Add(stopword);
            }
        }
    }
}
