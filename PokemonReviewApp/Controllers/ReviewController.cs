using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDTO>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(ReviewDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            var review = _mapper.Map<ReviewDTO>(_reviewRepository.GetReview(reviewId));

            return Ok(review);
        }

        [HttpGet("Pokemon/{pokemonId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonReviews(int pokemonId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewRepository.GetPokemonReviews(pokemonId));

            if(reviews.Count <= 0)
            {
                return NotFound();
            }
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromBody] CreateReviewVM reviewVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemon = _pokemonRepository.GetPokemon(reviewVM.PokemonId);
            if (pokemon == null)
            {
                return StatusCode(422, new { statusCode = 422, message = "There is no pokemon associated with this pokemonId" });
            }

            var reviewer = _reviewerRepository.GetReviewer(reviewVM.ReviewerId);
            if (reviewer == null)
            {
                return StatusCode(422, new { statusCode = 422, message = "There is no reviewer associated with this reviewerId" });
            }

            var newReview = new Review()
            {
                Title = reviewVM.Title,
                Text = reviewVM.Text,
                Rating = reviewVM.Rating,
                Pokemon = pokemon,
                Reviewer = reviewer
            };

            if (!_reviewRepository.CreateReview(newReview))
            {
                return StatusCode(500, new { statusCode = 500, message = "Fail to create new review." });
            }

            return Ok(new { statusCode = 200, Message = "Successfully create new review." });
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReview(int reviewId, [FromBody] EditReviewVM reviewVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = _reviewRepository.GetReview(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            review.Title = reviewVM.Title;
            review.Text = reviewVM.Text;
            review.Rating = reviewVM.Rating;

            if (!_reviewRepository.Save())
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update the review." });
            return Ok(new { StatusCode = 200, Message = "Successfully update the review." });
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview([Required] int reviewId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = _reviewRepository.GetReview(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            if (!_reviewRepository.DeleteReview(review))
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete the review." });
            }
            return Ok(new { StatusCode = 200, Message = "Successfully delete the review." });
        }
    }
}
