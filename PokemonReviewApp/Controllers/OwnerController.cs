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
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository; 
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<OwnerDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDTO>>(_ownerRepository.GetOwners());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(OwnerDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();
            var owner = _mapper.Map<OwnerDTO>(_ownerRepository.GetOwner(ownerId));

            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemons")]
        [ProducesResponseType(200, Type = typeof(List<PokemonDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonsByOwner(int ownerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var pokemons = _mapper.Map<List<PokemonDTO>>(_ownerRepository.GetOwnerPokemon(ownerId));

            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromBody]CreateOwnerVM ownerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!_countryRepository.CountryExists(ownerVM.CountryId))
            {
                return StatusCode(422, new { statusCode = 422, message = "There is no country associated with this countryId" });
            }
            
            var newOwner = new Owner()
            {
                FirstName = ownerVM.FirstName,
                LastName = ownerVM.LastName,
                Gym = ownerVM.Gym,
                CountryId = ownerVM.CountryId,
            };

            if (!_ownerRepository.CreateOwner(newOwner))
            {
                return StatusCode(500, new { statusCode = 500, message = "Fail to create the owner." });
            }

            return Ok(new { StatusCode = "200", Message = "Successfully create the owner." });
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] EditOwnerVM ownerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = _ownerRepository.GetOwner(ownerId);
            if (owner == null)
            {
                return NotFound();
            }

            if (!_countryRepository.CountryExists(ownerVM.CountryId))
            {
                return StatusCode(422, new { statusCode = 422, message = "There is no country associated with this countryId" });
            }

            owner.FirstName = ownerVM.FirstName;
            owner.LastName = ownerVM.LastName;
            owner.Gym = ownerVM.Gym;
            owner.CountryId = ownerVM.CountryId;
            if (!_ownerRepository.Save())
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the owner." });
            return Ok(new { StatusCode = 200, Message = "Successfully update the owner." });
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner([Required] int ownerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = _ownerRepository.GetOwner(ownerId);
            if (owner == null)
            {
                return NotFound();
            }

            if (!_ownerRepository.DeleteOwner(owner))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete the owner." });
            }
            return Ok(new { StatusCode = 200, Message = "Successfully delete the owner." });
        }
    }
}
