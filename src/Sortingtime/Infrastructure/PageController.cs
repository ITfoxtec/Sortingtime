using Microsoft.AspNetCore.Mvc;

namespace Sortingtime.Infrastructure
{
    [ExceptionHandlingAttribute(ExceptionHandlingAttribute.Type.Page)]
    public class PageController : Controller
    {
        public PageController()
        { }

    }
}
