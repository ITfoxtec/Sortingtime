using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace Sortingtime.Infrastructure
{
    public static class JsonExtensions
    {
        public static async Task<string> ToJson<T>(this T data)
        {
            return await Task.FromResult(JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            }));
        }

        public static async Task<T> FromJson<T>(this string data)
        {
            return await Task.FromResult(JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            }));
        }
    }
}
