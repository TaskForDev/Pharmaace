using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using NLog;
using PharmaACE.NLP.Framework;
using PharmaACE.NLP.Responder;

namespace PharmaACE.ChartAudit.Reports.Controllers
{
    [Authorize]
    [RoutePrefix("api/qa")]
    public class QAController : ApiController
    {
        string databasePath;
        List<string> thesaurusPaths;
        string langPath;
        string stopwordsPath;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public QAController()
        {
            string contentPath = HostingEnvironment.MapPath("~/Content/Store/");
            databasePath = contentPath + "chartaudit_shmeasure.sql"; //"ChartAudit.sql";
            thesaurusPaths = new List<string> { contentPath + "th_english.dat", contentPath + "th_chartaudit.dat" };
            langPath = contentPath + "english.csv";
            stopwordsPath = "english.txt";
        }

        [HttpPost]
        //[Route("AskIva")]
        public ChartResult Post([FromBody]NaturalLanguageQuestion question)
        {
            string userName = ControllerContext.RequestContext.Principal.Identity.GetUserName();
            int userId = ControllerContext.RequestContext.Principal.Identity.GetUserId<int>();
            logger.Info(question.Question);
            var ruleEngine = new ResponseBuilder(question);
            var caData = ruleEngine.GetChartAuditData(question);
            return caData;
        }

        [HttpPost]
        [Route("Suggest")]
        public List<string> Post([FromBody]SuggestionInput phrase)
        {
            var ruleEngine = new ResponseBuilder(null);
            var suggestions = ruleEngine.GetSuggestedQuestions(phrase?.Key);
            return suggestions;
        }

        [HttpPost]
        [Route("Store")]
        public void Post([FromBody]QuestionWithAnswer qa)
        {
            var ruleEngine = new ResponseBuilder(new NaturalLanguageQuestion { Question = qa.Question });
            ruleEngine.StoreQuestionWithAnswer(qa); //TODO: return some status code
        }
        
        [Route("RetrieveQuestionsWithAnswers/{userId}")]
        public List<QuestionWithAnswer> Get(int userId)
        {
            var ruleEngine = new ResponseBuilder(null);
            var qaList = ruleEngine.RetrieveQuestionsWithAnswers(userId);
            return qaList;
        }
        
        [Route("getthumbnail/{qaIDs}")]
        public List<string> Get(string qaIDs)
        {
            var ruleEngine = new ResponseBuilder(null);
            var thumbnails = ruleEngine.GetThumbnail(qaIDs);
            return thumbnails;
        }
        
        [Route("speechtotexthints")]
        public List<string> GetSpeechToTextHints()
        {
            var ruleEngine = new ResponseBuilder(null);
            var suggestions = ruleEngine.GetPhraseHints();
            return suggestions;
        }
    }
}
