using Sortingtime.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Sortingtime.Infrastructure
{
    [Authorize]
    public class SecureController : Controller
    {
        protected ApplicationDbContext DbContext { get; }

        public SecureController(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public long CurrentUserId
        {
            get
            {
                return HttpContext.User.GetUserId();
            }
        }

        public long CurrentPartitionId
        {
            get
            {
                return HttpContext.User.GetPartitionId();
            }
        }
        public string CurrentPartitionIdAsString
        {
            get
            {
                return Convert.ToString(CurrentPartitionId);
            }
        }

        protected async Task<U> MapDeltaValuesAndCleanWhiteSpace<U, T>(T source, U destination, string[] ignorePropertyNames = null, bool includeForeignId = false, bool updateNulls = false, bool cleanWhiteSpace = true)
        {
            return await Task.Run(() => 
            {
                // Go through all fields of source, if a value is not null, overwrite value on destination field.
                //foreach (var propertyName in source.GetType().GetProperties().Where(p => !p.PropertyType.IsGenericType).Select(p => p.Name))
                foreach (var propertyName in source.GetType().GetProperties().Select(p => p.Name)) // prøver at fjerne Is Genric Type begrænsningen...
                {
                    // If not an Id value
                    if (!(PropertyIsId(propertyName, includeForeignId) || IgnoreProperty(propertyName, ignorePropertyNames)))
                    {
                        var sourceValue = source.GetType().GetProperty(propertyName).GetValue(source, null);
                        if ((sourceValue != null || updateNulls) && (sourceValue == null || sourceValue.GetType() != typeof(DateTime) || (sourceValue.GetType() == typeof(DateTime) && (DateTime)sourceValue != DateTime.MinValue)))
                        {
                            if (cleanWhiteSpace && sourceValue is string)
                            {
                                sourceValue = ((string)sourceValue).CleanWhiteSpace();
                            }
                            var destinationProperty = destination.GetType().GetProperty(propertyName);
                            if (destinationProperty != null)
                            {
                                destinationProperty.SetValue(destination, sourceValue, null);
                            }
                        }
                        else
                        {
                            ModelState.ClearValidationState(propertyName);
                        }
                    }
                }

                return destination;
            });
        }

        protected async Task CleanWhiteSpace<T>(T source, string[] ignorePropertyNames = null)
        {
            await Task.Run(() =>
            {
                // Go through all fields of source, if a value is string.
                foreach (var propertyName in source.GetType().GetProperties().Where(p => !p.PropertyType.IsGenericType).Select(p => p.Name)) 
                {
                    // If not an Id value
                    if (!(PropertyIsId(propertyName, false) || IgnoreProperty(propertyName, ignorePropertyNames)))
                    {
                        var sourceValue = source.GetType().GetProperty(propertyName).GetValue(source, null);
                        if (sourceValue is string)
                        {
                            sourceValue = ((string)sourceValue).CleanWhiteSpace();
                        }
                    }
                }
            });
        }

        private bool IgnoreProperty(string propertyName, string[] ignorePropertyNames)
        {
            if(ignorePropertyNames != null)
            {
                foreach(var ignorePropertyName in ignorePropertyNames)
                {
                    if(propertyName.Equals(ignorePropertyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool PropertyIsId(string propertyName, bool includeForeignId)
        {
            return propertyName.Equals("id", StringComparison.InvariantCultureIgnoreCase) || (!includeForeignId && propertyName.EndsWith("Id", StringComparison.InvariantCulture));
        }
    }
}