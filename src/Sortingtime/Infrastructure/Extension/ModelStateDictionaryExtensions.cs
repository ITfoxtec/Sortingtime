using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Sortingtime.Infrastructure
{
    public static class ModelStateDictionaryExtensions
    {
        public static bool IsValidUpdated(this ModelStateDictionary modelState)
        {
            return modelState.Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Count() <= 0;
        }
    }
}
