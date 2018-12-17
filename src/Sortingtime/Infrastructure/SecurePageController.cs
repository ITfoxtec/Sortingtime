using Sortingtime.Models;

namespace Sortingtime.Infrastructure
{
    [ExceptionHandlingAttribute(ExceptionHandlingAttribute.Type.SecurePage)]
    public class SecurePageController : SecureController
    {
        public SecurePageController(ApplicationDbContext dbContext) : base(dbContext)
        { }

    }
}
