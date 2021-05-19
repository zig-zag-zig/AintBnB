﻿using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AintBnB.App.CommonMethodsAndProperties
{
    internal static class CommonViewMethods
    {
        public static void WhenNavigatedToView(NavigationEventArgs e, ComboBox comboBox)
        {
            if (e.Parameter != null)
            {
                var parameter = int.Parse(e.Parameter.ToString());

                comboBox.SelectedIndex = parameter;
            }
        }

        public static async Task<IUICommand> DialogeMessageAsync(string message, string buttonText)
        {
            var dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand { Label = buttonText, Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
            var res = await dialog.ShowAsync();

            return res;
        }

        public static string DatePickerParser(CalendarDatePicker datePicker)
        {
            var date = datePicker.Date;

            var dt = date.Value.DateTime;

            return dt.ToString("yyyy-MM-dd");
        }
    }
}
