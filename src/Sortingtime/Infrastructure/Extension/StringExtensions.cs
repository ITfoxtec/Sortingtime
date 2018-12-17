using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Infrastructure
{
    public static class StringExtensions
    {
        public static string CleanWhiteSpace(this string value)
        {
            value = value?.Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                value = null;
            }

            return value;
        }

        public static string FirstLine(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var index = value.IndexOf('\n');
                if (index > 0)
                {
                    return value.Substring(0, index);
                }
            }

            return value;
        }

        public static string MaxLength(this string value, int length)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Length > length)
                {
                    return value.Substring(0, length);
                }
            }

            return value;
        }

        public static bool EmailsIsValid(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (var mailItem in value.Split(','))
            {
                if (!new EmailAddressAttribute().IsValid(mailItem?.Trim()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
