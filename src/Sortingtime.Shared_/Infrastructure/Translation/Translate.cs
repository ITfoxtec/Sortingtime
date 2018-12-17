using System;
using System.Globalization;

namespace Sortingtime.Infrastructure.Translation
{
    public class Translate
    {
        public Ilocale Locale { get; private set; }

        public Translate()
        {
            Load();
        }

        public static string CultureName
        {
            get
            {
                return CultureInfo.CurrentCulture.Name;
            }
        }

        public static string ParentCultureName
        {
            get
            {
                return string.IsNullOrEmpty(CultureInfo.CurrentCulture.Parent?.Name) ? CultureInfo.CurrentCulture.Name : CultureInfo.CurrentCulture.Parent.Name;
            }
        }

        public string Get(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException("key");
                }
                var keySplit = key.Split('.');
                if (keySplit.Length != 2)
                {
                    throw new ArgumentException("The key do not contain exactly two key elements divided by a dot.");
                }
                var value = Locale.Translate(keySplit[0], keySplit[1]);
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("No translation value returned for key: " + key);
                }
                return value;
            }
            catch (Exception exc)
            {
                // TODO loggin this exception
                return key;
            }

        }

        private void Load()
        {
            if (ParentCultureName.Equals("da", StringComparison.OrdinalIgnoreCase))
            {
                Locale = new LocaleDa();
            }
            else
            {
                Locale = new LocaleEn();
            }
        }

    }
}
