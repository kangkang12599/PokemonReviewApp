using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository: IPokemonRepository
    {
        private readonly ApplicationDbContext _context;

        public PokemonRepository(ApplicationDbContext context)
        {
            _context = context; 
        }

        public bool AddPokemonCategories(int pokemonId, ICollection<int> categoriesId)
        { 
            foreach (var categoryId in categoriesId)
            {
                var pokemonCategory = new PokemonCategory()
                {
                    PokemonId = pokemonId,
                    CategoryId = categoryId
                };
                _context.Add(pokemonCategory);
            }

            return Save();
        }

        public bool AddPokemonOwner(int ownerId, int pokemonId)
        {
            var pokemonOwner = new PokemonOwner()
            {
                PokemonId = pokemonId,
                OwnerId = ownerId,
            };
            _context.Add(pokemonOwner);
            return Save();
        }

        public bool CreatePokemon(ICollection<int> categoriesId, Pokemon pokemon)
        {
            foreach(var categoryId in categoriesId)
            {
                var pokemonCategory = new PokemonCategory()
                {
                    CategoryId = categoryId,
                    Pokemon = pokemon
                };
                _context.Add(pokemonCategory);
            }
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }

        public bool DeletePokemonCategories(int pokemonId)
        {
            var categories = _context.PokemonCategories.Where(pc => pc.PokemonId == pokemonId).ToList();
            _context.PokemonCategories.RemoveRange(categories);
            return Save();
        }

        public Pokemon? GetPokemon(int id)
        {
            return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon? GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokemonId)
        {
            var review = _context.Reviews.Where(p => p.Pokemon.Id == pokemonId);
            if(review.Count() <= 0)
            {
                return 0;
            }

            return review.Sum(r => r.Rating) / (decimal) review.Count();
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(p => p.Id).ToList();
        }

        public bool PokemonExists(int pokemonId) 
        {
            return _context.Pokemons.Any(p => p.Id == pokemonId);
        }

        public bool PokemonExists(string name)
        {
            return _context.Pokemons.Any(p => p.Name.Trim().ToUpper() == name.Trim().ToUpper());
        }

        public bool PokemonOwnerExists(int ownerId, int pokemonId)
        {
            return _context.PokemonOwners.Any(po => po.OwnerId == ownerId && po.PokemonId == pokemonId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
