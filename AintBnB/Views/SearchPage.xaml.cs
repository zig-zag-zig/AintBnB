﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AintBnB.ViewModels;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using static AintBnB.CommonMethodsAndProperties.CommonViewMethods;
using System.Threading.Tasks;

namespace AintBnB.Views
{
    public sealed partial class SearchPage : Page
    {
        public AccommodationViewModel AccommodationViewModel { get; } = new AccommodationViewModel();
        public UserViewModel UserViewModel { get; } = new UserViewModel();
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

                await FindUserType();


                ComboBoxCountries.ItemsSource = await EuropeViewModel.GetAllCountriesInEurope();

            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async Task FindUserType()
        {
            try
            {
                await AuthenticationViewModel.IsEmployeeOrAdmin();

                ComboBoxUsers.Visibility = Visibility.Visible;

                List<int> ids = new List<int>();

                foreach (var user in await UserViewModel.GetAllCustomers())
                    ids.Add(user.Id);

                ComboBoxUsers.ItemsSource = ids;
            }
            catch (Exception)
            {
            }
        }

        private void ComboBoxUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BookingViewModel.Booking.BookedBy.Id = int.Parse(ComboBoxUsers.SelectedValue.ToString());
            }
            catch (Exception)
            {
                throw;
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

            List<BitmapImage> bmimg = new List<BitmapImage>();

            int index = listView.SelectedIndex;

            await ConvertBytesToBitmapImageList(AccommodationViewModel.AvailableAccommodations[index].Picture, bmimg);

            listViewPicture.ItemsSource = bmimg;

            contentDialog.Visibility = Visibility.Visible;

            ContentDialogResult result = await contentDialog.ShowAsync();

            _skipSelectionChanged = true;
            listView.SelectedItem = null;

            if (result == ContentDialogResult.Primary)
            {
                await Book(index);
            }
        }

        private async Task Book(int index)
        {
            BookingViewModel.StartDate = AccommodationViewModel.FromDate;

            try
            {
                await AuthenticationViewModel.IsEmployeeOrAdmin();
            }
            catch (Exception)
            {
                BookingViewModel.Booking.BookedBy.Id = await AuthenticationViewModel.IdOfLoggedInUser();
            }

            BookingViewModel.Nights = int.Parse(nights.Text);
            BookingViewModel.Booking.Accommodation.Id = AccommodationViewModel.AvailableAccommodations[index].Id;

            try
            {
                await BookingViewModel.BookAccommodation();
                await new MessageDialog("Booking successful!").ShowAsync();
                Frame.Navigate(typeof(SearchPage));
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async void Button_Click_Search(object sender, RoutedEventArgs e)
        {
            AccommodationViewModel.Accommodation.Address.Country = EuropeViewModel.Country;

            AccommodationViewModel.Accommodation.Address.City = EuropeViewModel.City;

            await FillListView();
        }

        private async Task FillListView()
        {
            try
            {
                listView.ItemsSource = await AccommodationViewModel.GetAvailable();

                PriceAsc.Visibility = Visibility.Visible;
                SizeAsc.Visibility = Visibility.Visible;
                DistanceAsc.Visibility = Visibility.Visible;
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

        private async void Button_Click_SortByPriceAsc(object sender, RoutedEventArgs e)
        {
            await Sort("Price", "Ascending", PriceAsc, PriceDesc);
        }

        private async void Button_Click_SortByPriceDesc(object sender, RoutedEventArgs e)
        {
            await Sort("Price", "Descending", PriceDesc, PriceAsc);
        }
        private async void Button_Click_SortBySizeAsc(object sender, RoutedEventArgs e)
        {
            await Sort("Size", "Ascending", SizeAsc, SizeDesc);
        }
        private async void Button_Click_SortBySizeDesc(object sender, RoutedEventArgs e)
        {
            await Sort("Size", "Descending", SizeDesc, SizeAsc);
        }
        private async void Button_Click_SortByDistanceAsc(object sender, RoutedEventArgs e)
        {
            await Sort("Distance", "Ascending", DistanceAsc, DistanceDesc);
        }
        private async void Button_Click_SortByDistanceDesc(object sender, RoutedEventArgs e)
        {
            await Sort("Distance", "Descending", DistanceDesc, DistanceAsc);
        }

        private async Task Sort(string sortBy, string ascOrDesc, Button hide, Button show)
        {
            AccommodationViewModel.SortBy = sortBy;
            AccommodationViewModel.AscOrDesc = ascOrDesc;

            await AccommodationViewModel.SortAvailableList();

            listView.ItemsSource = AccommodationViewModel.AvailableAccommodations;

            hide.Visibility = Visibility.Collapsed;
            show.Visibility = Visibility.Visible;
        }
    }
}