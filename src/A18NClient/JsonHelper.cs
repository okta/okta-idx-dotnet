using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace A18NAClient
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
