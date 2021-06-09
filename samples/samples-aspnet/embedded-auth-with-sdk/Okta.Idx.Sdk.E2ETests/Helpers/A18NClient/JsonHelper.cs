using Newtonsoft.Json;

namespace embedded_auth_with_sdk.E2ETests.Helpers.A18NClient
{
    public static class JsonHelper
    {
        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, 
                    new JsonSerializerSettings 
                    { 
                        NullValueHandling = NullValueHandling.Ignore,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    });
        }

        public static T Deserialize<T>(string jsonValue)
        {
            return JsonConvert.DeserializeObject<T>(jsonValue);
        }
    }
}
