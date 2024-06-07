using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace ReviewReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var Review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(Review);
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var Reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(Reviews);
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
            var review = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(review);
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto ReviewCreate)
        {
            if (ReviewCreate == null)
            {
                return BadRequest(ModelState);
            }
            var Reviews = _reviewRepository.GetReviews().Where(r => r.Title.Trim().ToUpper() == ReviewCreate.Title.Trim().ToUpper()).FirstOrDefault();

            if (Reviews != null)
            {
                ModelState.AddModelError("", "Review Already Exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewMap = _mapper.Map<Review>(ReviewCreate);

            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
            if (!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }


        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReviewer(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null)
            {
                return BadRequest(ModelState);
            }

            if (reviewId != updatedReview.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var ReviewMap = _mapper.Map<Review>(updatedReview);

            if (!_reviewRepository.UpdateReview(ReviewMap))
            {
                ModelState.AddModelError("", "Something went wrong updating Review");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound("Review Dose not exists");
            }

            var reviewToDelete = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
            }
            return NoContent();
        }
    }
}
