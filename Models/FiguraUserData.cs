using System.Text.Json.Serialization;

namespace MyDICollection.Models
{
    public class FiguraUserData
    {
        [JsonPropertyName("obtenido")]
        public bool Obtenido { get; set; }

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }

        [JsonPropertyName("nfcCodes")]
        public List<string> NfcCodes { get; set; } = new();
    }
}