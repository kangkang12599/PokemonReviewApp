using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IOwnerRepository
    {
        ICollection<Owner> GetOwners();
        Owner GetOwner(int ownerId);
        ICollection<Owner> GetPokemonOwners(int pokemonId);
        ICollection<Pokemon> GetOwnerPokemon(int ownerId);
        bool OwnerExists(int ownerId);
        bool CreateOwner(Owner owner);
        bool DeleteOwner(Owner owner);
        bool Save();
    }
}
