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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/[controller]/change/{elements}")]
        public IActionResult GetUser([FromRoute] string elements)
        {
            try
            {
                string[] words = elements.Split(' ');

                _userService.ChangePassword(words[0], Int32.Parse(words[1]), words[2], words[3]);

                return Ok("Password change ok!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(_userService.GetAllUsers());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/[controller]/{id}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            try
            {
                return Ok(_userService.GetUser(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}