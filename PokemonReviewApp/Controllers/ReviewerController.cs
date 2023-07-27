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
    public class ReviewerController : ControllerBase
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDTO>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDTO>>(_reviewerRepository.GetReviewers());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(ReviewerDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            var reviewer = _mapper.Map<ReviewerDTO>(_reviewerRepository.GetReviewer(reviewerId));

            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(List<ReviewDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));

            if(reviews.Count <= 0)
            {
                return NotFound();
            }
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer(CreateReviewerVM reviewerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newReviewer = new Reviewer()
            {
                FirstName = reviewerVM.FirstName,
                LastName = reviewerVM.LastName
            };

            if (!_reviewerRepository.CreateReviewer(newReviewer))
            {
                return StatusCode(500, new { statusCode = 500, Message = "Fail to save new reviewer." });
            }

            return Ok(new { statusCode = 200, Message = "Successfully save new reviewer." });
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] EditReviewerVM reviewerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);
            if (reviewer == null)
            {
                return NotFound();
            }

            reviewer.FirstName = reviewerVM.FirstName;
            reviewer.LastName = reviewerVM.LastName;

            if (!_reviewerRepository.Save())
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the reviewer." });
            return Ok(new { StatusCode = 200, Message = "Successfully update the reviewer." });
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer([Required] int reviewerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);
            if (reviewer == null)
            {
                return NotFound();
            }

            if (!_reviewerRepository.DeleteReviewer(reviewer))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete the reviewer." });
            }
            return Ok(new { StatusCode = 200, Message = "Successfully delete the reviewer." });
        }
    }
}
