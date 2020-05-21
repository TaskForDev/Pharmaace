using System;
using System.Reflection;

namespace PharmaACE.NLP.QuestionAnswerService.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}