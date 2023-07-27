using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country? GetCountry(int id);
        ICollection<Owner> GetOwnersByCountry(int countryId);
        public Country? GetCountryByOwner(int ownerId);
        bool CountryExists(int id);
        bool CountryExists(string name);
        bool CreateCountry(Country country);
        bool DeleteCountry(Country category);
        bool Save();
    }
}
