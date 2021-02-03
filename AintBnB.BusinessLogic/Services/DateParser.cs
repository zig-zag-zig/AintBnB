﻿using System;
using System.Collections.Generic;

namespace AintBnB.BusinessLogic.Services
{
    public static class DateParser
    {
        public static String DateFormatterTodaysDate()
        {
            return DateTime.Today.ToString("yyyy-MM-dd");
        }

        public static String DateFormatterCustomDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static bool AreDatesWithinRangeOfSchedule(SortedDictionary<string, bool> schedule, string fromDate, int nights)
        {
            string lastDate = DateFormatterCustomDate(DateTime.Parse(fromDate).AddDays(nights));

            if (!schedule.ContainsKey(fromDate) || !schedule.ContainsKey(lastDate))
                return false;

            return true;
        }

        public static bool AreAllDatesAvailable(SortedDictionary<string, bool> schedule, string fromDate, int nights)
        {
            if (AreDatesWithinRangeOfSchedule(schedule, fromDate, nights))
            {
                DateTime from = DateTime.Parse(fromDate);

                for (int i = 0; i < nights; i++)
                {
                    if (!schedule.ContainsKey(DateFormatterCustomDate(from.AddDays(i))))
                        return false;

                    if (schedule[DateFormatterCustomDate(from.AddDays(i))] == false)
                        return false;
                }

                return true;
            }
            return false;
        }
    }
}
