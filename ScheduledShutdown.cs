using System;
using System.Collections.Generic;
using System.Configuration;

namespace ScheduledShutdown
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<DayOfWeek, List<DateTime>> shutdownTimes = new Dictionary<DayOfWeek, List<DateTime>>();

            // Load the shutdown times from the config file into the dictionary. 
            // Multiple shutdown times in the same day can be done by separating the times with a comma.
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                // Get the setting for the current day of the week, if it exists.
                String shutdownTimeParamValue = ConfigurationManager.AppSettings[dayOfWeek + "ShutdownTime"];
                if (shutdownTimeParamValue == null)
                {
                    continue;
                }

                // Parse the setting value and add the times to the dictionary.
                String[] shutdownTimeStringParts = shutdownTimeParamValue.Split(',');
                List<DateTime> curShutdownTimes = new List<DateTime>();
                foreach (String shutdownTimeString in shutdownTimeStringParts)
                {
                    DateTime curShutdownTime = DateTime.ParseExact(shutdownTimeString, "hh:mm tt", null);
                    curShutdownTimes.Add(curShutdownTime);
                }

                shutdownTimes.Add(dayOfWeek, curShutdownTimes);
            }


            // Poll the time periodically. Shutdown if the previous time was before a shutdown time and the current
            // time is after that shutdown time.
            int pollTimeSeconds = 10;
            DateTime previousTime = DateTime.Now;
            while (true)
            {
                DateTime currentTime = DateTime.Now;

                // Get all of the shutdown times for today.
                List<DateTime> todaysShutdownTimes = new List<DateTime>();
                if (shutdownTimes.ContainsKey(currentTime.DayOfWeek))
                {
                    todaysShutdownTimes = shutdownTimes[currentTime.DayOfWeek];
                }

                // Go through each shutdown time and see if it is between the previous time and the current time.
                foreach (DateTime shutdownTime in todaysShutdownTimes)
                {
                    // Set the year, month, and day of the shutdown time to the current year, month, and day.
                    DateTime realShutdownTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, shutdownTime.Hour, shutdownTime.Minute, shutdownTime.Second);
                    if (realShutdownTime > previousTime && realShutdownTime <= currentTime)
                    {
                        Console.WriteLine("shutting down");
                        System.Diagnostics.Process.Start("shutdown", "/s /t 0");
                    }
                }

                // Set the previous time and sleep.
                previousTime = currentTime;
                System.Threading.Thread.Sleep(pollTimeSeconds * 1000);
            }
        }
    }
}