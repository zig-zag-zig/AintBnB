﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AintBnB.Core.Models
{
    public class Booking : INotifyPropertyChanged
    {
        private int _id;
        private User _bookedBy;
        private Accommodation _accommodation;
        private List<string> _dates;
        private int _price;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public User BookedBy
        {
            get { return _bookedBy; }
            set
            {
                _bookedBy = value;
                NotifyPropertyChanged("BookedBy");
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

        public List<string> Dates
        {
            get { return _dates; }
            set
            {
                _dates = value;
                NotifyPropertyChanged("Dates");
            }
        }

        public int Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyPropertyChanged("Price");
            }
        }

        public Booking(User bookedBy, Accommodation accommodation, List<string> dates, int price)
        {
            BookedBy = bookedBy;
            Accommodation = accommodation;
            Dates = dates;
            Price = price;
        }

        public Booking()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}