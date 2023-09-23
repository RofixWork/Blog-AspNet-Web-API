using demerge_blog_API.DTOs;
using demerge_blog_API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static demerge_blog_API.Helpers.Responses;
namespace demerge_blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //get all roles
        [HttpGet("Roles"), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return Ok(new
            {
                status = 200,
                roles
            });
        }

        //craete role
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if(await _roleManager.RoleExistsAsync(roleName.ToLower()))
                return BadRequest(BadRequestResponse($"this Role <<{roleName}>> already Exist"));

            var is_created = await _roleManager.CreateAsync(new IdentityRole { Name = roleName.ToLower(), ConcurrencyStamp = Guid.NewGuid().ToString() });

            if (is_created.Succeeded)
                return Ok(OKtResponse($"Role <<{roleName.ToLower()}>> has been Created Successfully"));

            return BadRequest(BadRequestResponse("This role has been not Created"));
        }

        //delete role
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return NotFound(NotFoundResponse("Not exist this role"));

            var is_deleted = await _roleManager.DeleteAsync(role);
            if(is_deleted.Succeeded)
                return NoContent();

            return BadRequest();
        }
        //get all users
        [HttpGet("GetUserRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            //find user
            var user = await _userManager.FindByEmailAsync(email);

            //check user
            if (user is null)
                return BadRequest(BadRequestResponse("invalid email"));

            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                status = 200,
                userRoles
            });
        }

        //add user role
        [HttpPost("AddRoleToUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddRoleToUser(RoleToUserDTO roleToUserDTO)
        {
            //find use by email
            var user = await _userManager.FindByEmailAsync(roleToUserDTO.Email!);
            //check user
            if (user is null)
                return BadRequest(BadRequestResponse("invalid email"));

            //check role
            if(!await _roleManager.RoleExistsAsync(roleToUserDTO.RoleName!.ToLower()))
                return NotFound(NotFoundResponse($"this role <<{roleToUserDTO.RoleName}>> not exist"));
            //get user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            //user have this role or not
            if(userRoles.Contains(roleToUserDTO.RoleName!.ToLower())) 
                return BadRequest(BadRequestResponse($"thid user already have this role <<{roleToUserDTO.RoleName}>>"));
            //add role to user
            var addRoleUser = await _userManager.AddToRoleAsync(user, roleToUserDTO.RoleName.ToLower());

            if (addRoleUser.Succeeded)
                return Ok(OKtResponse($"role <<{roleToUserDTO.RoleName}>> has been added"));

            return BadRequest();
        }

        //delete user from role
        [HttpDelete("DeleteUserFromRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserFromRole(RoleToUserDTO roleToUserDTO)
        {
            //find use by email
            var user = await _userManager.FindByEmailAsync(roleToUserDTO.Email!);
            //check user
            if (user is null)
                return BadRequest(BadRequestResponse("invalid email"));

            //check role
            if (!await _roleManager.RoleExistsAsync(roleToUserDTO.RoleName!.ToLower()))
                return NotFound(NotFoundResponse($"this role <<{roleToUserDTO.RoleName}>> not exist"));
            //get user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            //user have this role or not
            if (!userRoles.Contains(roleToUserDTO.RoleName!.ToLower()))
                return BadRequest(BadRequestResponse($"this user not have this role <<{roleToUserDTO.RoleName}>>"));

            var is_deleted = await _userManager.RemoveFromRoleAsync(user, roleToUserDTO.RoleName.ToLower());

            if (is_deleted.Succeeded)
                return Ok(OKtResponse($"role <<{roleToUserDTO.RoleName}>> has been removed"));

            return BadRequest();
        }
    }
}
