using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category? GetCategory(int id);
        ICollection<Pokemon> GetPokemonByCategory(int categoryId);
        bool CategoryExists(int id);
        bool CategoryExists(string name);
        bool CreateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
