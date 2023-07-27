using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int id);
        ICollection<Review> GetPokemonReviews(int pokemonId);
        bool ReviewExists(int reviewId);
        bool CreateReview(Review review);
        bool DeleteReview(Review review);
        bool Save();
    }
}
