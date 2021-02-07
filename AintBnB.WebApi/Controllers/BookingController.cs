﻿using AintBnB.Core.Models;
using AintBnB.BusinessLogic.DependencyProviderFactory;
using AintBnB.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AintBnB.WebApi.Controllers
{
    [ApiController]
    public class BookingController : ControllerBase
    {
        private IBookingService _bookingService;
        private IDeletionService _deletionService;

        public BookingController()
        {
            _bookingService = ProvideDependencyFactory.bookingService;
            _deletionService = ProvideDependencyFactory.deletionService;
        }

        [HttpGet]
        [Route("api/[controller]/{startDate}/{bookerId}/{nights}/{accommodationId}")]
        public IActionResult Book([FromRoute] string startDate, [FromRoute] int bookerId, [FromRoute] int nights, [FromRoute] int accommodationId)
        {
            try
            {
                User booker = ProvideDependencyFactory.userService.GetUser(bookerId);
                Accommodation accommodation = ProvideDependencyFactory.accommodationService.GetAccommodation(accommodationId);
                Booking booking = _bookingService.Book(startDate, booker, nights, accommodation);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/[controller]/{id}")]
        public IActionResult GetBooking([FromRoute] int id)
        {
            try
            {
                return Ok(_bookingService.GetBooking(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult GetAllBookings()
        {
            try
            {
                return Ok(_bookingService.GetAllBookings());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public IActionResult DeleteBooking([FromRoute] int id)
        {
            try
            {
                _deletionService.DeleteBooking(id);
                return Ok("Deletion ok");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
