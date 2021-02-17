﻿using AintBnB.Core.Models;
using AintBnB.BusinessLogic.Repository;
using System;
using AintBnB.BusinessLogic.DependencyProviderFactory;
using static AintBnB.BusinessLogic.Services.DateParser;
using static AintBnB.BusinessLogic.Services.UpdateScheduleInDatabase;
using static AintBnB.BusinessLogic.Services.AuthenticationService;
using AintBnB.BusinessLogic.CustomExceptions;

namespace AintBnB.BusinessLogic.Services
{
    public class DeletionService : IDeletionService
    {
        private IRepository<User> _iUserRepository;
        private IRepository<Accommodation> _iAccommodationRepository;
        private IRepository<Booking> _iBookingRepository;

        public IRepository<User> IUserRepository
        {
            get { return _iUserRepository; }
            set
            {
                if (value == null)
                    throw new ArgumentException("IUserRepository cannot be null");
                _iUserRepository = value;
            }
        }
        public IRepository<Accommodation> IAccommodationRepository
        {
            get { return _iAccommodationRepository; }
            set
            {
                if (value == null)
                    throw new ArgumentException("IAccommodationRepository cannot be null");
                _iAccommodationRepository = value;
            }
        }

        public IRepository<Booking> IBookingRepository
        {
            get { return _iBookingRepository; }
            set
            {
                if (value == null)
                    throw new ArgumentException("IBookingRepository cannot be null");
                _iBookingRepository = value;
            }
        }

        public DeletionService()
        {
            _iUserRepository = ProvideDependencyFactory.userRepository;
            _iAccommodationRepository = ProvideDependencyFactory.accommodationRepository;
            _iBookingRepository = ProvideDependencyFactory.bookingRepository;
        }

        public DeletionService(IRepository<User> userRepo, IRepository<Accommodation> accommodationRepo, IRepository<Booking> bookingRepo)
        {
            _iUserRepository = userRepo;
            _iAccommodationRepository = accommodationRepo;
            _iBookingRepository = bookingRepo;
        }

        public void DeleteUser(int id)
        {
            if (CorrectUserOrAdmin(id))
            {
                CheckIfUserCanBeDeleted(_iUserRepository.Read(id));
                DeleteUsersAccommodations(id);
                DeleteUsersBookings(id);
                _iUserRepository.Delete(id);
            }
            else
                throw new AccessException($"Administrator or user with ID {id} only!");
        }

        private static void CheckIfUserCanBeDeleted(User user)
        {
            if (user == null)
                throw new IdNotFoundException("User", user.Id);

            if (user.UserType == UserTypes.Admin)
                throw new AccessException("Admin cannot be deleted!");
        }

        private void DeleteUsersAccommodations(int id)
        {
            foreach (Accommodation accommodation in _iAccommodationRepository.GetAll())
            {
                if (accommodation.Owner == _iUserRepository.Read(id))
                {
                    DeleteAccommodation(accommodation.Id);
                }
            }
        }

        private void DeleteUsersBookings(int id)
        {
            foreach (Booking booking in _iBookingRepository.GetAll())
            {
                if (booking.BookedBy == _iUserRepository.Read(id))
                {
                    try
                    {
                        DeleteBooking(booking.Id);
                    }
                    catch (Exception)
                    {
                        throw new CancelBookingException("user", id);
                    }
                }
            }
        }

        public void DeleteAccommodation(int id)
        {
            Accommodation accommodation = _iAccommodationRepository.Read(id);

            CanAccommodationBeDeleted(accommodation);

            DeleteAccommodationBookings(accommodation.Id);

            _iAccommodationRepository.Delete(id);
        }

        private void CanAccommodationBeDeleted(Accommodation accommodation)
        {
            if (accommodation == null)
                throw new IdNotFoundException("Accommodation", accommodation.Id);

            if (!CorrectUserOrAdminOrEmployee(accommodation.Owner.Id))
                throw new AccessException($"Administrator, employee or user with ID {accommodation.Owner.Id} only!");
        }

        private void DeleteAccommodationBookings(int id)
        {
            foreach (Booking booking in _iBookingRepository.GetAll())
            {
                if (booking.Accommodation == _iAccommodationRepository.Read(id))
                {
                    try
                    {
                        DeleteBooking(booking.Id);
                    }
                    catch (Exception)
                    {
                        throw new CancelBookingException("accommodation", booking.Id);
                    }
                }
            }
        }

        public void DeleteBooking(int id)
        {
            Booking booking = _iBookingRepository.Read(id);

            if (booking == null)
                throw new IdNotFoundException("Booking", id);

            if (CorrectUserOrOwnerOrAdminOrEmployee(booking.Accommodation.Owner.Id, booking.BookedBy.Id))
                CancelationDeadlineCheck(id);
            else
                throw new AccessException();
        }

        private void CancelationDeadlineCheck(int id)
        {
            string firstDateBooked = _iBookingRepository.Read(id).Dates[0];
            string today = DateFormatterTodaysDate();

            if (DateTime.Parse(today) < DateTime.Parse(firstDateBooked).AddDays(-4))
            {
                ResetAvailableStatusAfterDeletingBooking(id);
                _iBookingRepository.Delete(id);
            }
            else
                throw new CancelBookingException(id);
        }

        private void ResetAvailableStatusAfterDeletingBooking(int id)
        {
            Booking booking = _iBookingRepository.Read(id);
            foreach (string datesBooked in _iBookingRepository.Read(id).Dates)
            {
                if (booking.Accommodation.Schedule.ContainsKey(datesBooked))
                    booking.Accommodation.Schedule[datesBooked] = true;
            }
            UpdateScheduleInDb(booking.Accommodation.Id, booking.Accommodation.Schedule);
        }
    }
}