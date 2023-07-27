namespace PokemonReviewApp.ViewModel
{
    public class CreatePokemonVM
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<int> CategoriesId { get; set; }
    }
}
