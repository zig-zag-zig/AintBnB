﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AintBnB.ViewModels;
using AintBnB.Core.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using static AintBnB.CommonMethods.CommonViewMethods;

namespace AintBnB.Views
{
    public sealed partial class SearchPage : Page
    {
        public AccommodationViewModel AccommodationViewModel { get; } = new AccommodationViewModel();
        public EuropeViewModel EuropeViewModel { get; } = new EuropeViewModel();
        public BookingViewModel BookingViewModel { get; } = new BookingViewModel();
        public AuthenticationViewModel AuthenticationViewModel { get; } = new AuthenticationViewModel();
        private bool _skipSelectionChanged;
        public SearchPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await AuthenticationViewModel.IsAnyoneLoggedIn();

                ComboBoxCountries.ItemsSource = await EuropeViewModel.GetAllCountriesInEurope();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async void ComboBoxCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EuropeViewModel.Country = ComboBoxCountries.SelectedValue.ToString();

            ComboBoxCities.ItemsSource = await EuropeViewModel.GetAllCitiesOfACountry();
        }

        private void ComboBoxCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EuropeViewModel.City = ComboBoxCities.SelectedValue.ToString();
        }

        private async void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_skipSelectionChanged)
            {
                _skipSelectionChanged = false;

                return;
            }

            Accommodation acc = (Accommodation)listView.SelectedItem;

            List<BitmapImage> bmimg = new List<BitmapImage>();

            await ConvertBytesToBitmapImageList(acc.Picture, bmimg);

            listViewPicture.ItemsSource = bmimg;

            contentDialog.Visibility = Visibility.Visible;

            ContentDialogResult result = await contentDialog.ShowAsync();

            _skipSelectionChanged = true;
            listView.SelectedItem = null;

            if (result == ContentDialogResult.Primary)
            {
                BookingViewModel.StartDate = AccommodationViewModel.FromDate;
                BookingViewModel.Booking.BookedBy.Id = await AuthenticationViewModel.IdOfLoggedInUser();
                BookingViewModel.Nights = int.Parse(nights.Text);
                BookingViewModel.Booking.Accommodation.Id = acc.Id;
                try
                {
                    await BookingViewModel.BookAccommodation();
                    await new MessageDialog("Booking successful!").ShowAsync();
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                }
            }
        }

        private async void Button_Click_Search(object sender, RoutedEventArgs e)
        {
            AccommodationViewModel.Accommodation.Address.Country = EuropeViewModel.Country;

            AccommodationViewModel.Accommodation.Address.City = EuropeViewModel.City;

            try
            {
                listView.ItemsSource = await AccommodationViewModel.GetAvailable();

            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void MyDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            var date = MyDatePicker.Date;

            DateTime dt = date.Value.DateTime;

            AccommodationViewModel.FromDate = dt.ToString("yyyy-MM-dd");
        }
    }
}
