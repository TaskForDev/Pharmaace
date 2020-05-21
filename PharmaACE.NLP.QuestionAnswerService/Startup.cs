using System.Collections.Generic;
using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using PharmaACE.NLP.Framework;

[assembly: OwinStartup(typeof(PharmaACE.NLP.QuestionAnswerService.Startup))]

namespace PharmaACE.NLP.QuestionAnswerService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            LoadStaticCache();
        }

        private void LoadStaticCache()
        {
            string contentPath = HostingEnvironment.MapPath("~/Content/Store/");
            string databasePath = contentPath + "chartaudit_shmeasure.sql"; //"ChartAudit.sql";
            List<string> thesaurusPaths = new List<string> { contentPath + "th_english.dat", contentPath + "th_chartaudit.dat" };
            string langPath = contentPath + "english.csv";
            //string stopwordsPath = "english.txt";
            //keep the static data ready in-memory
            StaticResources.GetInstance(databasePath, langPath, thesaurusPaths, null);
        }
    }
}
