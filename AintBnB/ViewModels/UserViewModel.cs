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
    public class UserViewModel : Observable
    {
        private User _user = new User();
        private HttpClientProvider _clientProvider = new HttpClientProvider();
        private string _uri;
        private string _uniquePartOfUri;
        private string _passwordConfirm;
        private List<User> _allEmployeeRequests;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                NotifyPropertyChanged("User");
            }
        }

        public string PasswordConfirm
        {
            get { return _passwordConfirm; }
            set
            {
                _passwordConfirm = value;
                NotifyPropertyChanged("PasswordConfirm");
            }
        }

        public List<User> AllEmployeeRequests
        {
            get { return _allEmployeeRequests; }
            set
            {
                _allEmployeeRequests = value;
                NotifyPropertyChanged("AllEmployeeRequests");
            }
        }

        public UserViewModel()
        {
            _clientProvider.ControllerPartOfUri = "api/user/";
            _uri = _clientProvider.LocalHostAddress + _clientProvider.LocalHostPort + _clientProvider.ControllerPartOfUri;
        }

        public async Task MakeEmployeeAsync()
        {
            User.UserType = UserTypes.Employee;
            await UpdateAUserAsync();
        }

        public void RequestToBecomeEmployee()
        {
            User.UserType = UserTypes.RequestToBeEmployee;
        }

        public async Task CreateTheUserAsync()
        {
            if (User.Password != PasswordConfirm)
                throw new Exception("The passwords don't match!");

            var userJson = JsonConvert.SerializeObject(User);
            var response = await _clientProvider.client.PostAsync(
                _uri, new StringContent(userJson, Encoding.UTF8, "application/json"));
            ResponseChecker(response);
        }

        public async Task GetAUserAsync()
        {
            _uniquePartOfUri = User.Id.ToString();

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonUser = await response.Content.ReadAsStringAsync();
            _user = JsonConvert.DeserializeObject<User>(jsonUser);
            NotifyPropertyChanged("User");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var response = await _clientProvider.client.GetAsync(new Uri(_uri));
            ResponseChecker(response);
            var jsonUsers = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<User>>(jsonUsers);
        }

        public async Task<List<User>> GetAllCustomersAsync()
        {
            _uniquePartOfUri = "allcustomers";

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonUsers = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<User>>(jsonUsers);
        }

        public async Task<List<User>> GetAllEmployeeRequestsAsync()
        {
            _uniquePartOfUri = "requests";

            var response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
            var jsonUsers = await response.Content.ReadAsStringAsync();
            AllEmployeeRequests = JsonConvert.DeserializeObject<List<User>>(jsonUsers);
            return AllEmployeeRequests;
        }

        public async Task DeleteAUserAsync()
        {
            _uniquePartOfUri = User.Id.ToString();

            var response = await _clientProvider.client.DeleteAsync(new Uri(_uri + _uniquePartOfUri));
            ResponseChecker(response);
        }

        public async Task UpdateAUserAsync()
        {
            _uniquePartOfUri = User.Id.ToString();

            var userJson = JsonConvert.SerializeObject(User);

            var response = await _clientProvider.client.PutAsync(
                new Uri(_uri + _uniquePartOfUri), new StringContent(userJson, Encoding.UTF8, "application/json"));
            ResponseChecker(response);
        }
    }
}
