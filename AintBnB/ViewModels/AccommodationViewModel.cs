﻿using AintBnB.CommonMethodsAndProperties;
using AintBnB.Core.Models;
using AintBnB.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static AintBnB.CommonMethodsAndProperties.CommonViewModelMethods;

namespace AintBnB.ViewModels
{
    public class AccommodationViewModel : Observable
    {
        private HttpClientProvider _clientProvider = new HttpClientProvider();
        private string _uri;
        private string _uniquePartOfUri;
        private int _userId;
        private Accommodation _accommodation = new Accommodation { Address = new Address(), Picture = new List<byte[]>() };
        private int _daysSchedule;
        private int _expandScheduleByDays;
        private string _fromDate;
        private int _nights;
        private List<Accommodation> _availableAccommodations;
        private string _sortBy = "";
        private string _ascOrDesc = "";
        public int UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                NotifyPropertyChanged("UserId");
            }
        }

        public Accommodation Accommodation
        {
            get { return _accommodation; }
            set
            {
                _accommodation = value;
                NotifyPropertyChanged("Accommodation");
            }
        }

        public int DaysSchedule
        {
            get { return _daysSchedule; }
            set
            {
                _daysSchedule = value;
                NotifyPropertyChanged("DaysSchedule");
            }
        }

        public int ExpandScheduleByDays
        {
            get { return _expandScheduleByDays; }
            set
            {
                _expandScheduleByDays = value;
                NotifyPropertyChanged("ExpandScheduleByDays");
            }
        }

        public string FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                NotifyPropertyChanged("FromDate");
            }
        }

        public int Nights
        {
            get { return _nights; }
            set
            {
                _nights = value;
                NotifyPropertyChanged("Nights");
            }
        }

        public List<Accommodation> AvailableAccommodations
        {
            get { return _availableAccommodations; }
            set
            {
                _availableAccommodations = value;
                NotifyPropertyChanged("AvailableAccommodations");
            }
        }

        public string SortBy
        {
            get { return _sortBy; }
            set
            {
                _sortBy = value;
                NotifyPropertyChanged("SortBy");
            }
        }

        public string AscOrDesc
        {
            get { return _ascOrDesc; }
            set
            {
                _ascOrDesc = value;
                NotifyPropertyChanged("AscOrDesc");
            }
        }

        public AccommodationViewModel()
        {
            _clientProvider.ControllerPartOfUri = "api/accommodation/";
            _uri = _clientProvider.LocalHostAddress + _clientProvider.LocalHostPort + _clientProvider.ControllerPartOfUri;
        }

        public async Task CreateAccommodationAsync()
        {
            _uniquePartOfUri = DaysSchedule.ToString() + "/" + UserId.ToString();
            var accJson = JsonConvert.SerializeObject(Accommodation);
            var response = await _clientProvider.client.PostAsync(
                new Uri(_uri + _uniquePartOfUri), new StringContent(accJson, Encoding.UTF8, "application/json"));
            ResponseChecker(response);
        }

        public async Task GetAccommodationAsync()
        {
            _uniquePartOfUri = Accommodation.Id.ToString();

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonAcc = await response.Content.ReadAsStringAsync();
            _accommodation = JsonConvert.DeserializeObject<Accommodation>(jsonAcc);
            NotifyPropertyChanged("Accommodation");

        }

        public async Task<List<Accommodation>> GetAllAccommodationsAsync()
        {

            var response = await _clientProvider.client.GetAsync(new Uri(_uri));
            ResponseChecker(response);
            var jsonAccList = await response.Content.ReadAsStringAsync();
            var all = JsonConvert.DeserializeObject<List<Accommodation>>(jsonAccList);
            return all;
        }

        public async Task<List<Accommodation>> GetAllAccommodationsOfAUserAsync()
        {
            _uniquePartOfUri = UserId.ToString() + "/allaccommodations";


            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonAccList = await response.Content.ReadAsStringAsync();
            var all = JsonConvert.DeserializeObject<List<Accommodation>>(jsonAccList);
            return all;
        }

        public async Task<List<Accommodation>> GetAvailableAsync()
        {
            _uniquePartOfUri = _accommodation.Address.Country + "/" + _accommodation.Address.City + "/" + FromDate + "/" + Nights.ToString();

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonAcc = await response.Content.ReadAsStringAsync();
            AvailableAccommodations = JsonConvert.DeserializeObject<List<Accommodation>>(jsonAcc);
            return AvailableAccommodations;
        }

        public async Task SortAvailableListAsync()
        {
            _uniquePartOfUri = "sort/" + SortBy + "/" + AscOrDesc;

            var availableJson = JsonConvert.SerializeObject(AvailableAccommodations);
            var response = await _clientProvider.client.PostAsync(
                new Uri(_uri + _uniquePartOfUri), new StringContent(availableJson, Encoding.UTF8, "application/json"));
            ResponseChecker(response);
            AvailableAccommodations = JsonConvert.DeserializeObject<List<Accommodation>>(await response.Content.ReadAsStringAsync());
        }

        public async Task DeleteAccommodation()
        {
            _uniquePartOfUri = Accommodation.Id.ToString();
            var response = await _clientProvider.client.DeleteAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
        }

        public async Task UpdateAccommodation()
        {
            _uniquePartOfUri = Accommodation.Id.ToString();

            var accJson = JsonConvert.SerializeObject(Accommodation);

            var response = await _clientProvider.client.PutAsync(
                new Uri(_uri + _uniquePartOfUri), new StringContent(accJson, Encoding.UTF8, "application/json"));
            ResponseChecker(response);
        }

        public async Task ExpandScheduleOfAccommodation()
        {
            _uniquePartOfUri = Accommodation.Id.ToString() + "/" + ExpandScheduleByDays.ToString();

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
        }
    }
}
