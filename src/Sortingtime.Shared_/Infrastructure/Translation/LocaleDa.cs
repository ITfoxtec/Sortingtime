using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sortingtime.Infrastructure.Translation
{
    public class LocaleDa : Ilocale
    {
        private static JObject translations = Load();

        public static JObject Load()
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Sortingtime.Infrastructure.Translation.Da.json")))
            {
                return JObject.Parse(reader.ReadToEnd());
            }            
        }

        public string Translate(string key1, string key2)
        {
            try
            {
                return translations[key1][key2].Value<string>();
            }
            catch (Exception exc)
            {
                throw new Exception($"DK Translate missing [key1:'{key1}', key2:'{key2}']", exc);
            }
        }
    }
}
