using System.Text.Json.Serialization;

namespace RestWithASP_NET5.Data.VO
{
    public class PersonVO 
    {
        [JsonPropertyName("code")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string CompletName { get; set; }     
        [JsonPropertyName("email")]
        public string Email { get; set; }

    }
}
