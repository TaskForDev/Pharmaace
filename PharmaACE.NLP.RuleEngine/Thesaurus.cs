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
    public class Thesaurus
    {
        private const string THESAURUS_ENCODER = "~|~"; //nose and moustache
        public Dictionary<string, List<string>> Dictionary { get; set; }

        public Thesaurus()
        {
            Dictionary = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
        }

        void AddSynonyms(string word, List<string> synonyms)
        {
            if (Dictionary.ContainsKey(word))
                Dictionary[word].AddRange(synonyms);
            else
                Dictionary.Add(word, synonyms);
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

        public void Load(List<string> paths)
        {
            foreach (string path in paths)
            {
                List<string> synonyms = null;
                var content = File.ReadAllLines(GeneratePath(path));
                //we jump content[0] because it is the encoding-type line : useless to parse
                for (int lineId = 1; lineId < content.Length; lineId++)
                {
                    if (!content[lineId].StartsWith("("))
                    {
                        var lineParts = content[lineId].Split(new char[] { '|' });
                        var word = RemoveAccents(lineParts[0]);

                        while (lineId < content.Length - 1 && content[lineId + 1].StartsWith("(noun)"))
                        {
                            lineId++;
                            synonyms = RemoveAccents(content[lineId]).Split(new char[] { '|' }).ToList();
                            synonyms.RemoveAt(0);
                            AddSynonyms(word, synonyms);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// encodes synonym phrases found in a sentence to make them look like a sigle word
        /// e.g. head and neck -> head~|~and~|~neck
        /// </summary>
        /// <param name="originalSentence"></param>
        /// <returns>modified sentence after encoding the synonym phrases in it</returns>
        public string EncodePhrases(string originalSentence)
        {
            string sentence = originalSentence;
            string encodedMatch = null;

            //traverse all synonym keys
            var matchedKeys = Dictionary.Keys.
                Where(k => sentence.
                IndexOf(k, StringComparison.CurrentCultureIgnoreCase) > -1).
                ToList();
            foreach (var matchedKey in matchedKeys)
            {
                if (matchedKey != null)
                {
                    var matchedKeySplit = matchedKey.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (matchedKeySplit.Length > 1)
                    {
                        encodedMatch = String.Join(THESAURUS_ENCODER, matchedKeySplit);
                        sentence = String.Join(encodedMatch, Regex.Split(sentence, matchedKey));
                    }
                }
            }

            //traverse all synonym values
            var matchedValues = Dictionary.Values.
                SelectMany(v => v).
                Where(v => sentence.
                IndexOf(v, StringComparison.CurrentCultureIgnoreCase) > -1).
                ToList();
            foreach (var matchedValue in matchedValues)
            {
                if (matchedValue != null)
                {
                    var matchedValueSplit = matchedValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (matchedValueSplit.Length > 1)
                    {
                        encodedMatch = String.Join(THESAURUS_ENCODER, matchedValueSplit);
                        sentence = String.Join(encodedMatch, Regex.Split(originalSentence, matchedValue, RegexOptions.IgnoreCase));
                    }
                }
            }

            return sentence;
        }

        public string Decode(string originalSentence)
        {
            return String.Join(" ", originalSentence.Split(new string[] { THESAURUS_ENCODER }, StringSplitOptions.None));
        }
    }
}
