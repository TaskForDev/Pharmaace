using System.Web.Http;
using FluentValidation.WebApi;
using Microsoft.Owin.Security.OAuth;
using PharmaACE.NLP.QuestionAnswerService.Filters;
using PharmaACE.NLP.QuestionAnswerService.Validation;

namespace PharmaACE.NLP.QuestionAnswerService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.EnableCors();

            //register validation factory, in our case the concrete validation factory is ninjectvalidationfactory
            FluentValidationModelValidatorProvider.Configure(config, x => x.ValidatorFactory = new NinjectValidatorFactory(config));
            //register bad request filter that will make the controller decoupled from the validation logic
            config.Filters.Add(new BadRequestFilter());

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
