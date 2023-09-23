using demerge_blog_API.DTOs;
using demerge_blog_API.Models;
using demerge_blog_API.Repository.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using static demerge_blog_API.Helpers.Responses;
namespace demerge_blog_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user")]
    //[ApiController]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public PostController(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        //get posts
        [HttpGet("posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPosts()
        {
            //get posts
            var posts = await _unitOfWork.Posts.FindAll();
            return Ok(new
            {
                status = 200,
                posts 
            });
        }

        //create post
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePost([FromForm] PostDTO postDTO)
        {
            //check form
            if(!ModelState.IsValid)
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
            //check category
            if (await _unitOfWork.Categories.FindOne(c => c.Id == postDTO.CategoryId) is null)
                return BadRequest(BadRequestResponse("Invalid Category Id"));
            //check user
            if(await _userManager.FindByIdAsync(postDTO.UserId!) is null)
                return BadRequest(BadRequestResponse("Invalid User Id"));
            //copy image to stream
            using var stream = new MemoryStream();
            await postDTO.Image!.CopyToAsync(stream);
            
            //create a new post
            Post post = new()
            {
                Title = postDTO.Title,
                Content = postDTO.Content,
                UserId = postDTO.UserId,
                CategoryId = postDTO.CategoryId,
                Image = stream.ToArray()
            };
            //add post
            await _unitOfWork.Posts.AddOneAsync(post);

            //return Ok(OKtResponse("Your post has been Added successfully..."));
            return CreatedAtRoute("GetPost", new { id = post.Id }, new
            {
                status = 201,
                message = "Your post has been Added successfully..."
            });
        }

        //get one post by id
        [HttpGet("{id:int}", Name = "GetPost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPost(int id)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));

            //find post
            var post = await _unitOfWork.Posts.FindOne(p => p.Id.Equals(id));
            //check post
            if(post is null)
            {
                return NotFound(NotFoundResponse($"Not Exist any Post by this Id <<{id}>>"));
            }

            return Ok(new
            {
                status = 200,
                post = new
                {
                    post.Id,
                    post.Title,
                    post.Content,
                    post.CategoryId,
                    post.UserId,
                    Image = post.PostImage,
                    post.CreatedAt,
                    post.UpdateAt
                }
            });
        }

        //update post
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePost([FromRoute]int id, [FromForm]UpdatePostDTO postDTO)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));

            //find post
            var post = await _unitOfWork.Posts.FindOne(p => p.Id.Equals(id));
            //check post
            if (post is null)
            {
                return NotFound(NotFoundResponse($"Not Exist any Post by this Id <<{id}>>"));
            }

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
            //check category
            if (await _unitOfWork.Categories.FindOne(c => c.Id == postDTO.CategoryId) is null)
                return BadRequest(BadRequestResponse("Invalid Category Id"));

            using MemoryStream stream = new();
            await postDTO.Image!.CopyToAsync(stream);

            post.Title = postDTO.Title;
            post.Content = postDTO.Content;
            post.CategoryId = postDTO.CategoryId;
            post.Image = stream.ToArray();
            post.UpdateAt = DateTime.UtcNow;
            await _unitOfWork.Posts.SaveChanges();
            return NoContent();
        }

        //delete post
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));

            //find post
            var post = await _unitOfWork.Posts.FindOne(p => p.Id.Equals(id));
            //check post
            if (post is null)
            {
                return NotFound(NotFoundResponse($"Not Exist any Post by this Id <<{id}>>"));
            }
            //delete post
            await _unitOfWork.Posts.DeleteOneAsync(post);
            return NoContent();
        }
    }
}
//In publishing and graphic design, Lorem ipsum is a placeholder text commonly used to demonstrate the visual form of a document or a typeface without relying on meaningful content. Lorem ipsum may be used as a placeholder before final copy is available.