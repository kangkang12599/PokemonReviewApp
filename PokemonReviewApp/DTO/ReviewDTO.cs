using System.Text.Json;
using PokemonReviewApp.Models;
using System.Text.Json.Serialization;

namespace PokemonReviewApp.DTO
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PokemonDTO Pokemon { get; set; }
    }
}
