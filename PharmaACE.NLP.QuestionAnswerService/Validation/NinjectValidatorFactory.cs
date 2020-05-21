using System;
using FluentValidation;
using System.Linq;
using System.Web.Http;

namespace PharmaACE.NLP.QuestionAnswerService.Validation
{
    /// <summary>
    /// Validation factory that uses ninject to create validators  
    /// </summary>
    public class NinjectValidatorFactory : ValidatorFactoryBase
    {
        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectValidatorFactory"/> class.
        /// </summary>
        /// <param name="configuration">http configuration of the service</param>
        public NinjectValidatorFactory(HttpConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Creates an instance of a validator with the given type using ninject.
        /// </summary>
        /// <param name="validatorType">Type of the validator.</param>
        /// <returns>The newly created validator</returns>
        public override IValidator CreateInstance(Type validatorType)
        {
            try
            {
                return _configuration.DependencyResolver.GetService(validatorType) as IValidator;
            }
            catch
            {
                return null;
            }
        }
    }
    
}
