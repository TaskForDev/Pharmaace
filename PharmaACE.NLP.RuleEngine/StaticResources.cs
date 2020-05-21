using System;
using System.Collections.Generic;
using PharmaACE.Utility;

namespace PharmaACE.NLP.Framework
{
    public class StaticResources
    {
        string databasePath;
        string languagePath;
        List<string> thesaurusPaths;
        string stopwordsPath;

        public Database Database { set; get; }
        public StopWordFilter StopWordFilter { set; get; }
        public LangConfig LanguageConfig { set; get; }

        private static StaticResources instance;

        public static StaticResources GetInstance(string databasePath, string languagaePath, List<string> thesaurusPaths, string stopwordsPath)
        {
            if (instance == null)
                instance = new StaticResources(databasePath, languagaePath, thesaurusPaths, stopwordsPath);

            return instance;
        }
        /// <summary>
        /// call this when startup staticloadcache is guaranteed
        /// </summary>
        /// <returns></returns>
        public static StaticResources GetInstance()
        {
            return instance;
        }

        private StaticResources(string databasePath, string languagaePath, List<string> thesaurusPaths, string stopwordsPath)
        {
            this.databasePath = databasePath;
            this.languagePath = languagaePath;
            this.stopwordsPath = stopwordsPath;
            this.thesaurusPaths = thesaurusPaths;

            Database = new Database();
            if (thesaurusPaths.AnyOrNotNull())
            {
                var thesaurus = new Thesaurus();
                thesaurus.Load(thesaurusPaths);
                Database.Thesaurus = thesaurus;
            }
            if (!String.IsNullOrEmpty(stopwordsPath))
            {
                StopWordFilter = new StopWordFilter();
                StopWordFilter.Load(stopwordsPath);
            }

            Database.Load(this.databasePath);
            LanguageConfig = new LangConfig();
            LanguageConfig.Load(languagaePath);
        }
    }
}
