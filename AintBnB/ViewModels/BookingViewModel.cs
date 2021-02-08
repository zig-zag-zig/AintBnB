﻿using AintBnB.Core.Models;
using AintBnB.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AintBnB.Services;
using System.Text;

namespace AintBnB.ViewModels
{
    public class BookingViewModel : Observable
    {
        private string _startDate;
        private int _night;
        private HttpClientProvider _clientProvider = new HttpClientProvider();
        private string _uri;
        private string _uniquePartOfUri;
        private Booking _booking = new Booking {BookedBy = new User(), Accommodation = new Accommodation(), Dates = new List<string>()};
        private int _userId;

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

        public BookingViewModel()
        {
            _clientProvider.ControllerPartOfUri = "api/booking/";
            _uri = _clientProvider.LocalHostAddress + _clientProvider.LocalHostPort + _clientProvider.ControllerPartOfUri;
        }

        public async Task BookAccommodation()
        {
            _uniquePartOfUri = StartDate + "/" + Booking.BookedBy.Id.ToString() + "/" + Nights.ToString() + "/" + Booking.Accommodation.Id.ToString();

            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            if (response.IsSuccessStatusCode)
            {
                string jsonBooking = await response.Content.ReadAsStringAsync();
                Booking = JsonConvert.DeserializeObject<Booking>(jsonBooking);
                NotifyPropertyChanged("Booking");
            }
            else
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task GetABooking()
        {
            _uniquePartOfUri = Booking.Id.ToString();


            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            if (response.IsSuccessStatusCode)
            {
                string jsonBooking = await response.Content.ReadAsStringAsync();
                Booking = JsonConvert.DeserializeObject<Booking>(jsonBooking);
                NotifyPropertyChanged("Booking");
            }
            else
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<List<Booking>> GetAllBookings()
        {
            List<Booking> all = new List<Booking>();

            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri));
            if (response.IsSuccessStatusCode)
            {
                string jsonBookings = await response.Content.ReadAsStringAsync();
                all = JsonConvert.DeserializeObject<List<Booking>>(jsonBookings);
                return all;
            }
            else
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<List<Booking>> GetAllBookingsOfOwnedAccommodations()
        {
            List<Booking> all = new List<Booking>();

            _uniquePartOfUri = UserId.ToString() + "/" + "bookingsownaccommodation";

            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            if (response.IsSuccessStatusCode)
            {
                string jsonBookings = await response.Content.ReadAsStringAsync();
                all = JsonConvert.DeserializeObject<List<Booking>>(jsonBookings);
                return all;
            }
            else
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task DeleteABooking()
        {
            _uniquePartOfUri = Booking.Id.ToString();

            HttpResponseMessage response = await _clientProvider.client.DeleteAsync(new Uri(_uri + _uniquePartOfUri));
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }
    }
}
