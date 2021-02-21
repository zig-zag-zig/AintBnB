﻿using AintBnB.Core.Models;
using AintBnB.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AintBnB.CommonMethodsAndProperties;
using System.Text;

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

        public async Task MakeEmployee()
        {
            User.UserType = UserTypes.Employee;
            await UpdateAUser();
        }

        public void RequestToBecomeEmployee()
        {
            User.UserType = UserTypes.RequestToBeEmployee;
        }

        public async Task CreateTheUser()
        {
            if (User.Password != PasswordConfirm)
                throw new Exception("The passwords don't match!");

            string userJson = JsonConvert.SerializeObject(User);
            HttpResponseMessage response = await _clientProvider.client.PostAsync(
                _uri, new StringContent(userJson, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task GetAUser()
        {
            _uniquePartOfUri = User.Id.ToString();


            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            if (response.IsSuccessStatusCode)
            {
                string jsonUser = await response.Content.ReadAsStringAsync();
                _user = JsonConvert.DeserializeObject<User>(jsonUser);
                NotifyPropertyChanged("User");
            }
            else
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<List<User>> GetAllUsers()
        {
            List<User> _all = new List<User>();


            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri));
            if (response.IsSuccessStatusCode)
            {
                string jsonUsers = await response.Content.ReadAsStringAsync();
                _all = JsonConvert.DeserializeObject<List<User>>(jsonUsers);
                return _all;
            }
            throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<List<User>> GetAllCustomers()
        {
            List<User> _all = new List<User>();

            _uniquePartOfUri = "allcustomers";

            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            if (response.IsSuccessStatusCode)
            {
                string jsonUsers = await response.Content.ReadAsStringAsync();
                _all = JsonConvert.DeserializeObject<List<User>>(jsonUsers);
                return _all;
            }
            throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<List<User>> GetAllEmployeeRequests()
        {
            _uniquePartOfUri = "requests";

            HttpResponseMessage response = await _clientProvider.client.GetAsync(new Uri(_uri + _uniquePartOfUri));
            if (response.IsSuccessStatusCode)
            {
                string jsonUsers = await response.Content.ReadAsStringAsync();
                AllEmployeeRequests = JsonConvert.DeserializeObject<List<User>>(jsonUsers);
                return AllEmployeeRequests;
            }
            throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task DeleteAUser()
        {
            _uniquePartOfUri = User.Id.ToString();

            HttpResponseMessage response = await _clientProvider.client.DeleteAsync(new Uri(_uri + _uniquePartOfUri));
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }

        public async Task UpdateAUser()
        {
            _uniquePartOfUri = User.Id.ToString();

            string userJson = JsonConvert.SerializeObject(User);

            HttpResponseMessage response = await _clientProvider.client.PutAsync(
                new Uri(_uri + _uniquePartOfUri), new StringContent(userJson, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException(response.Content.ReadAsStringAsync().Result);
        }
    }
}
