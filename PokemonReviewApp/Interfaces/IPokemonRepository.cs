using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon? GetPokemon(int id);
        Pokemon? GetPokemon(string name);
        decimal GetPokemonRating(int pokemonId);
        bool PokemonExists(int pokemonId);
        bool PokemonExists(string name);
        bool CreatePokemon(ICollection<int> categoriesId, Pokemon pokemon);
        bool AddPokemonCategories(int pokemonId, ICollection<int> categoriesId);
        bool DeletePokemonCategories(int pokemonId);
        bool AddPokemonOwner(int ownerId, int pokemonId);
        bool DeletePokemon(Pokemon pokemon);
        bool PokemonOwnerExists(int ownerId, int pokemonId);
        bool Save();
    }
}
