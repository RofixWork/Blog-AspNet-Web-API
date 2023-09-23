using demerge_blog_API.DTOs;
using demerge_blog_API.Helpers;
using demerge_blog_API.Models;
using demerge_blog_API.Repository.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static demerge_blog_API.Helpers.Responses;

namespace demerge_blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //get all
        [HttpGet]
        [Route("Categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories()
        {
            //all categories
            var categories = await _unitOfWork.Categories.FindAll();
            return Ok(new {status=StatusCodes.Status200OK, categories});
        }
        //get one
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategory(int id)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));
            //find category
            var category = await _unitOfWork.Categories.FindOne(p => p.Id.Equals(id));
            //not exist this category
            if (category is null)
                return NotFound(NotFoundResponse($"Not Exist any category by this Id <<{id}>>"));
            //ok
            return Ok(category);
        }
        //create one
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            //check category (exist or not)
            if(await _unitOfWork.Categories.FindOne(c => c.Name!.Trim().ToLower() == categoryDTO.Name.Trim().ToLower()) is not null) 
            {
                return BadRequest(BadRequestResponse($"This Category <<{categoryDTO.Name}>> already Exist!"));
            }
            //create new catregory
            Category category = new()
            {
                Name = categoryDTO.Name.ToLower()
            };
            await _unitOfWork.Categories.AddOneAsync(category);
            return CreatedAtRoute("GetCategory", new {id = category.Id}, category);
        }

        //update category
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory([FromRoute]int id , [FromBody]CategoryDTO categoryDTO)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));
            //check category (exist or not)
            var category = await _unitOfWork.Categories.FindOne(c => c.Id.Equals(id));
            if (category is null)
            {
                return NotFound(NotFoundResponse($"Not Exist any category by this Id <<{id}>>"));
            }
            //update category name
            category.Name = categoryDTO.Name.Trim().ToLower();
            await _unitOfWork.Categories.SaveChanges();
            return NoContent();
        }

        //delete category
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            //check id
            if (id <= 0) return BadRequest(BadRequestResponse("Please Enter value grather than zero"));
            //check category (exist or not)
            var category = await _unitOfWork.Categories.FindOne(c => c.Id.Equals(id));
            if (category is null)
            {
                return NotFound(NotFoundResponse($"Not Exist any category by this Id <<{id}>>"));
            }
            //delete category
            await _unitOfWork.Categories.DeleteOneAsync(category);
            return NoContent();
        }
    }
}
