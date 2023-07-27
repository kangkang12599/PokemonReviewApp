using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;
using PokemonReviewApp.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDTO>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDTO>>(_countryRepository.GetCountries());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(CountryDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            var country = _mapper.Map<CountryDTO>(_countryRepository.GetCountry(countryId));

            return Ok(country);
        }

        [HttpGet("owner/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(CountryDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnerCountry(int ownerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var country = _mapper.Map<CountryDTO>(_countryRepository.GetCountryByOwner(ownerId));
            if (country == null)
                return NotFound();

            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry(CreateCountryVM countryVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_countryRepository.CountryExists(countryVM.Name))
            {
                return StatusCode(422, new { statusCode = 422, message = "Country has existed" });
            }
            var newCountry = new Country()
            {
                Name = countryVM.Name,
            };

            if (!_countryRepository.CreateCountry(newCountry))
            {
                return StatusCode(500, new { statusCode = 500, message = "Fail to save the country." });
            }

            return Ok(new { StatusCode = "200", Message = "Successfully save the country."});
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int countryId, [FromBody] EditCountryVM countryVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var country = _countryRepository.GetCountry(countryId);
            if (country == null)
            {
                return NotFound();
            }

            country.Name = countryVM.Name;
            if (!_countryRepository.Save())
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the country." });
            return Ok(new { StatusCode = 200, Message = "Successfully update the country." });
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry([Required] int countryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var country = _countryRepository.GetCountry(countryId);
            if (country == null)
            {
                return NotFound();
            }

            if (!_countryRepository.DeleteCountry(country))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete the country." });
            }
            return Ok(new { StatusCode = 200, Message = "Successfully delete the country." });
        }


    }
}
