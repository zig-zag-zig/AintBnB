﻿using Microsoft.AspNetCore.Mvc;
using System;
using AintBnB.BusinessLogic.Services;
using AintBnB.BusinessLogic.DependencyProviderFactory;
using AintBnB.Core.Models;

namespace AintBnB.WebApi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IDeletionService _deletionService;

        public UserController()
        {
            _userService = ProvideDependencyFactory.userService;
            _deletionService = ProvideDependencyFactory.deletionService;
        }

        [HttpPost]
        [Route("api/[controller]")]
        public IActionResult CreateUser(User user)
        {
            try
            {
                User newUser = _userService.CreateUser(user.UserName, user.Password, user.FirstName, user.LastName);
                return Created(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path + "/" + newUser.Id, newUser);
            }
            catch (Exception)
            {
                return NotFound("User could not be created");
            }
        }

        [HttpPut]
        [Route("api/[controller]/{id}")]
        public IActionResult UpdateUser([FromRoute] int id, User user)
        {
            try
            {
                _userService.UpdateUser(id, user);
                return Ok(user);
            }
            catch (Exception)
            {
                return NotFound($"User with id {id} could not be updated");
            }
        }

        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }

        [HttpGet]
        [Route("api/[controller]/{id}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            try
            {
                return Ok(_userService.GetUser(id));
            }
            catch (Exception)
            {
                return NotFound($"User with id {id} could not be found");
            }
        }

        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public IActionResult DeleteUser([FromRoute] int id)
        {
            try
            {
                _deletionService.DeleteUser(id);
                return Ok("Deletion ok");
            }
            catch (Exception)
            {
                return NotFound($"User with id {id} could not be deleted");
            }
        }
    }
}