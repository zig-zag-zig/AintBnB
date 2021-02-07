﻿using AintBnB.Core.Models;
using AintBnB.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AintBnB.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        public UserViewModel ViewModel { get; } = new UserViewModel();
        public AuthenticationViewModel LoginViewModel { get; } = new AuthenticationViewModel();

        public UserInfoPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.UserId = await LoginViewModel.IdOfLoggedInUser();

            await ViewModel.GetAUser();
        }

        private void Button_Click_UpdateUser(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_ChangePass(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_DeleteUser(object sender, RoutedEventArgs e)
        {

        }
    }
}
