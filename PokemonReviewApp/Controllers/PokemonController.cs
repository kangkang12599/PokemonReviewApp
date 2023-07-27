using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;
using PokemonReviewApp.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController: ControllerBase 
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, ICategoryRepository categoryRepository, IOwnerRepository owmerRepository, IMapper mapper) { 
            _pokemonRepository = pokemonRepository;
            _categoryRepository = categoryRepository;
            _ownerRepository = owmerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PokemonDTO>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDTO>>(_pokemonRepository.GetPokemons());

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpGet("{pokemonId}")]
        [ProducesResponseType(200, Type = typeof(PokemonDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokemonId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound();
            var pokemon = _mapper.Map<PokemonDTO>(_pokemonRepository.GetPokemon(pokemonId));
            
            return Ok(pokemon);
        }

        [HttpGet("{pokemonId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokemonId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokemonId);

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon(CreatePokemonVM pokemonVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pokemonVM.CategoriesId == null || pokemonVM.CategoriesId.Count <= 0)
            {
                return BadRequest(new { StatusCode = 400, Message = "Missing categories id." });
            }

            foreach (var categoryId in pokemonVM.CategoriesId)
            {
                if (!_categoryRepository.CategoryExists(categoryId))
                {
                    return StatusCode(422, new { statusCode = 422, message = "There is no category associated with the category Id " + categoryId + "." });
                }
            }

            if (_pokemonRepository.PokemonExists(pokemonVM.Name))
            {
                return StatusCode(422, new { statusCode = 422, message = "Pokemon has existed" });
            }

            var newPokemon = new Pokemon()
            {
                Name = pokemonVM.Name,
                BirthDate = pokemonVM.BirthDate
            };

            if (!_pokemonRepository.CreatePokemon(pokemonVM.CategoriesId, newPokemon))
            {
                return StatusCode(500, new { statusCode = 500, message = "Fail to create new pokemon." });
            }

            return Ok(new { statusCode = 200, Message = "Successfully create new pokemon." });
        }

        [HttpGet("AddPokemonOwner")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult AddPokemonOwner([Required]int ownerId, [Required]int pokemonId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!_ownerRepository.OwnerExists(ownerId))
            {
                return StatusCode(422, new { statusCode = 422, message = "There is no owner associated with the ownerId." });
            }

            if (!_pokemonRepository.PokemonExists(pokemonId))
            {
                return StatusCode(422, new { statusCode = 422, message = "There is no pokemon associated with the pokemonId." });
            }

            if (_pokemonRepository.PokemonOwnerExists(ownerId, pokemonId))
            {
                return StatusCode(422, new { statusCode = 422, message = "The owner has already owned the same pokemon." });
            }

            if(!_pokemonRepository.AddPokemonOwner(ownerId, pokemonId))
            {
                return StatusCode(500, new { statusCode = 500, message = "Fail to add new pokemon owner." });
            }

            return Ok(new { statusCode = 200, Message = "Successfully add new pokemon owner." });
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdatePokemon(int pokemonId, [FromBody]EditPokemonVM pokemonVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemon = _pokemonRepository.GetPokemon(pokemonId);
            if (pokemon == null)
            {
                return NotFound();
            }

            if(!_pokemonRepository.DeletePokemonCategories(pokemonId))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the pokemon." });
            }

            if(!_pokemonRepository.AddPokemonCategories(pokemonId, pokemonVM.CategoriesId))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the pokemon." });
            }

            pokemon.Name = pokemonVM.Name;
            pokemon.BirthDate = pokemonVM.BirthDate;

            if (!_pokemonRepository.Save())
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the pokemon." });
            return Ok(new { StatusCode = 200, Message = "Successfully update the pokemon." });
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon([Required] int pokemonId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemon = _pokemonRepository.GetPokemon(pokemonId);
            if (pokemon == null)
            {
                return NotFound();
            }

            if (!_pokemonRepository.DeletePokemon(pokemon))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete the pokemon." });
            }
            return Ok(new { StatusCode = 200, Message = "Successfully delete the pokemon." });
        }
    }
}
