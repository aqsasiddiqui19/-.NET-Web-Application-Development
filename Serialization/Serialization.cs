using Newtonsoft.Json;

namespace Ecommerce_API_Project.Serialization
{
    public static class JsonSerializationHelper
    {
        public static string SerializeObjectWithSettings(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Avoid circular references
                Formatting = Formatting.Indented // Pretty format the output
            };

            // Serialize the object and return JSON string
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
