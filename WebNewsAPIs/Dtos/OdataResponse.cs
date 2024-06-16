using System.Text.Json.Serialization;

namespace WebNewsAPIs.Dtos
{
    public class OdataResponse<T>
    {
        [JsonPropertyName("@odata.context")]
        public string context { get; set; }
        [JsonPropertyName("value")]
        public T data { get; set; }
    }
}
