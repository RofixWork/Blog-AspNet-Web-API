using demerge_blog_API.DTOs;
using demerge_blog_API.Models;
using demerge_blog_API.Repository.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static demerge_blog_API.Helpers.Responses;
namespace demerge_blog_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user")]
    //[ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public CommentController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        //get comments
        [HttpGet("Comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllComments()
        {
            //all comments
            var comments = await _unitOfWork.Comments.GetAllComments();
            return Ok(new
            {
                status = 200,
                comments
            });
        }

        //get comment by id
        [HttpGet("{id:int}", Name = "GetComment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComemnt(int id)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));

            //find comment by id
            var comment = await _unitOfWork.Comments.GetCommentDetails(id);

            //check comment (exist or not)
            if (comment is null)
            {
                return NotFound(NotFoundResponse($"Not Exist any category by this Id <<{id}>>"));
            }

            //reponse
            return Ok(new
            {
                status = 200,
                comment
            });
        }

        //create comment
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateComment([FromBody] CommentDTO commentDTO)
        {
            //check form
            if (!ModelState.IsValid)
            {
                //get all errors
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage);
                //print errors
                return BadRequest(new
                {
                    status = 400,
                    errors
                });
            }

            //check post
            if (await _unitOfWork.Posts.FindOne(p => p.Id == commentDTO.PostId) is null)
                return BadRequest(BadRequestResponse("Invalid Post Id"));

            //check user
            if (await _userManager.FindByIdAsync(commentDTO.UserId!) is null)
                return BadRequest(BadRequestResponse("Invalid User Id"));

            //create comment
            Comment comment = new()
            {
                CommentText = commentDTO.CommentText,
                UserId = commentDTO.UserId!,
                PostId = commentDTO.PostId
            };
            //add comment
            await _unitOfWork.Comments.AddOneAsync(comment);

            return CreatedAtRoute("GetComment", new { id = comment.Id }, OKtResponse("Comment has been added successfully."));
        }

        //delet comment
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));

            //get comment
            var comment = await _unitOfWork.Comments.FindOne(c => c.Id.Equals(id));

            if (comment is null)
                return NotFound(NotFoundResponse($"Not Exist any category by this Id <<{id}>>"));

            //delete comment
            await _unitOfWork.Comments.DeleteOneAsync(comment);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDTO updateCommentDTO)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));

            //check body
            if (!ModelState.IsValid)
            {
                //get all errors
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage);
                //print errors
                return BadRequest(new
                {
                    status = 400,
                    errors
                });
            }
            //find comment by id
            var comment = await _unitOfWork.Comments.FindOne(c => c.Id == id);
            //check comment (exist pr not)
            if (comment is null)
                return NotFound(NotFoundResponse($"Not Exist any category by this Id <<{id}>>"));

            //update comment
            comment.CommentText = updateCommentDTO.CommentText;
            comment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Comments.SaveChanges();

            return NoContent();

        }
    }
}
