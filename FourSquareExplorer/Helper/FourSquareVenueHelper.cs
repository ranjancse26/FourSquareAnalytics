using FourSquareExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FourSquareExplorer.Helper
{
    public class FourSquareVenueHelper
    {
        FourSquareVenueViewModel fourSquareViewModel;
        public FourSquareVenueHelper()
        {
            fourSquareViewModel = new FourSquareVenueViewModel();           
        }

        private static string PutIntoQuotes(string value)
        {
            return "\"" + value + "\"";
        }

        public FourSquareVenueForChart GetViewModel(List<NetSquare.FourSquareVenue> FourSquareVenueList)
        {
            var chart = new FourSquareVenueForChart();
            // Get Labels
            string lables = "[";
            string checkInData = "[";

            foreach (NetSquare.FourSquareVenue venue in FourSquareVenueList)
            {
                lables = lables + PutIntoQuotes(venue.name) + ",";
                checkInData = checkInData + venue.stats.checkinsCount.ToString() + ",";
            }
            lables = lables.Substring(0, lables.Length - 1) + "]";
            checkInData = checkInData.Substring(0, checkInData.Length - 1) + "]";

            chart.Labels = lables.Replace(@"\", " ");
            chart.CheckIn = checkInData;

            return chart;
        }
    }
}