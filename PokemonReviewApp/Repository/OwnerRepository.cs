using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerRepository(ApplicationDbContext context) { 
            _context = context; 
        }

        public bool CreateOwner(Owner owner)
        {
            _context.Add(owner);
            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Include(o => o.Country).Where(o => o.Id == ownerId).FirstOrDefault();
        }

        public ICollection<Pokemon> GetOwnerPokemon(int ownerId)
        {
            return _context.PokemonOwners.Where(po => po.OwnerId == ownerId).Select(po => po.Pokemon).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.Include(o => o.Country).ToList();
        }

        public ICollection<Owner> GetPokemonOwners(int pokemonId)
        {
            return _context.PokemonOwners.Where(po => po.PokemonId == pokemonId).Select(po => po.Owner).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return _context.Owners.Any(o => o.Id == ownerId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
