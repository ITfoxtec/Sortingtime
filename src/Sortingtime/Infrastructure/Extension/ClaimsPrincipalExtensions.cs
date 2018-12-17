using System;
using System.Security.Claims;

namespace Sortingtime.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal principal)
        {
            try
            {
                var userIdValue = principal.GetFirstClaimValue(ClaimTypes.NameIdentifier);
                long userId;
                if (!long.TryParse(userIdValue, out userId))
                {
                    throw new Exception("Unable to parse NameIdentifier Claim Value");
                }
                return userId;
            }
            catch (Exception exc)
            {
                throw new Exception("Unable to read NameIdentifier claim.", exc);
            }
        }

        public static long GetPartitionId(this ClaimsPrincipal principal)
        {
            try
            {
                var partitionIdValue = principal.GetFirstClaimValue(CustomClaimTypes.Partition);
                long partitionId;
                if (!long.TryParse(partitionIdValue, out partitionId))
                {
                    throw new Exception("Unable to parse Partition Claim Value");
                }
                return partitionId;
            }
            catch (Exception exc)
            {
                throw new Exception("Unable to read Partition claim.", exc);
            }
        }

        public static string GetFirstClaimValue(this ClaimsPrincipal principal, string type)
        {
            try
            {
                return (principal.Identity as ClaimsIdentity).FindFirst(type)?.Value;
            }
            catch (Exception exc)
            {
                throw new Exception("Unable to read Claim Type: " + type, exc);
            }
        }

    }
}