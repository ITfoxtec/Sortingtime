using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;

namespace Sortingtime.Infrastructure
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ExceptionHandling(ExceptionHandlingAttribute.Type.Api)]
    public class SecureApiController : SecureController
    {
        public SecureApiController(ApplicationDbContext dbContext) : base(dbContext)
        { }
    }
}
