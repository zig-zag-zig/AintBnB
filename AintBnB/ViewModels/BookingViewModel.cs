﻿using AintBnB.CommonMethodsAndProperties;
using AintBnB.Core.Models;
using AintBnB.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AintBnB.CommonMethodsAndProperties.CommonViewModelMethods;

namespace AintBnB.ViewModels
{
    public class BookingViewModel : Observable
    {
        private string _startDate;
        private int _night;
        private HttpClientProvider _clientProvider = new HttpClientProvider();
        private string _uri;
        private string _uniquePartOfUri;
        private Booking _booking = new Booking { BookedBy = new User(), Accommodation = new Accommodation(), Dates = new List<string>() };
        private int _userId;
        List<Booking> _allBookingsOfOwnedAccommodations;

        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        public int Nights
        {
            get { return _night; }
            set
            {
                _night = value;
                NotifyPropertyChanged("Nights");
            }
        }

        public Booking Booking
        {
            get { return _booking; }
            set
            {
                _booking = value;
                NotifyPropertyChanged("Booking");
            }
        }

        public int UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                NotifyPropertyChanged("UserId");
            }
        }

        public List<Booking> AllBookingsOfOwnedAccommodations
        {
            get { return _allBookingsOfOwnedAccommodations; }
            set
            {
                _allBookingsOfOwnedAccommodations = value;
                NotifyPropertyChanged("AllBookingsOfOwnedAccommodations");
            }
        }

        public BookingViewModel()
        {
            _clientProvider.ControllerPartOfUri = "api/booking/";
            _uri = _clientProvider.LocalHostAddress + _clientProvider.LocalHostPort + _clientProvider.ControllerPartOfUri;
        }

        public async Task BookAccommodationAsync()
        {
            _uniquePartOfUri = StartDate + "/" + Booking.BookedBy.Id + "/" + Nights + "/" + Booking.Accommodation.Id;

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonBooking = await response.Content.ReadAsStringAsync();
            Booking = JsonConvert.DeserializeObject<Booking>(jsonBooking);
            NotifyPropertyChanged("Booking");
        }

        public async Task UpdateBookingAsync()
        {
            _uniquePartOfUri = StartDate + "/" + Nights + "/" + Booking.Id;

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonBooking = await response.Content.ReadAsStringAsync();
            Booking = JsonConvert.DeserializeObject<Booking>(jsonBooking);
            NotifyPropertyChanged("Booking");
        }

        public async Task RateAsync()
        {
            _uniquePartOfUri = "rate/" + Booking.Id + "/" + Booking.Rating;

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
        }

        public async Task GetABookingAsync()
        {
            _uniquePartOfUri = Booking.Id.ToString();

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonBooking = await response.Content.ReadAsStringAsync();
            Booking = JsonConvert.DeserializeObject<Booking>(jsonBooking);
            NotifyPropertyChanged("Booking");
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            var response = await _clientProvider.client.GetAsync(new Uri(_uri));
            ResponseChecker(response);
            var jsonBookings = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Booking>>(jsonBookings);
        }

        public async Task<List<Booking>> GetAllBookingsOfOwnedAccommodationsAsync()
        {
            _uniquePartOfUri = UserId + "/" + "bookingsownaccommodation";

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonBookings = await response.Content.ReadAsStringAsync();
            AllBookingsOfOwnedAccommodations = JsonConvert.DeserializeObject<List<Booking>>(jsonBookings);
            return AllBookingsOfOwnedAccommodations;
        }

        public async Task DeleteABookingAsync()
        {
            _uniquePartOfUri = Booking.Id.ToString();

            var response = await _clientProvider.client.DeleteAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
        }
    }
}
