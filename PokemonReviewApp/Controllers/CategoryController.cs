using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDTO>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDTO>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(CategoryDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
            var category = _mapper.Map<CategoryDTO>(_categoryRepository.GetCategory(categoryId));

            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PokemonDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategory(int categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemons = _mapper.Map<List<PokemonDTO>>(_categoryRepository.GetPokemonByCategory(categoryId));
            if(pokemons.Count <= 0)
            {
                return NotFound();
            }
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_categoryRepository.CategoryExists(categoryVM.Name))
            {
                return StatusCode(422, new { StatusCode = 422, Message = "Category has exists" });
            }
            var newCategory = new Category() 
            {
                Name = categoryVM.Name,
            };

            if (!_categoryRepository.CreateCategory(newCategory))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Fail to save the category." });
            }

            return Ok(new {StatusCode = 200, Message = "Successfully save the category."});
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int categoryId, [FromBody]EditCategoryVM categoryVM)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _categoryRepository.GetCategory(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = categoryVM.Name;
            if (!_categoryRepository.Save())
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the category." });

            return Ok(new { StatusCode = 200, Message = "Successfully update the category." });
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory([Required]int categoryId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _categoryRepository.GetCategory(categoryId);
            if(category == null)
            {
                return NotFound();
            }

            if (!_categoryRepository.DeleteCategory(category))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete the category." });
            }
            return Ok(new { StatusCode = 200, Message = "Successfully delete the category." });
        }
    }
}
