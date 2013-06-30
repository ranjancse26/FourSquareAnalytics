using FourSquareExplorer.Helper;
using FourSquareExplorer.Models;
using FourSquareLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FourSquareExplorer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new FourSquareVenueViewModel());
        }

        [HttpPost]
        public ActionResult Index(FourSquareVenueViewModel viewModel)
        {
            viewModel.FourSquareVenueList.Clear();
            SpotInfo sinfo = new SpotInfo();
            sinfo.Address = viewModel.FourSquareVenueEntity.LocationName.Trim();
            Geocoder geo = new Geocoder();
            GeoResult result = geo.GetGeoResult(sinfo);
            if(result.Status == GeoResultStatus.OK){
                string authToken = ConfigurationManager.AppSettings["authToken"].ToString();
                string lat = result.Results[0].Geometry.Location.Lat.ToString();
                string lang = result.Results[0].Geometry.Location.Lng.ToString();
                string latlang = lat + "," + lang;
                var recommentedVenues = NetSquare.VenueExplore(latlang, "", "", "", "10000", "", viewModel.FourSquareVenueEntity.Query, "", "", authToken);
                foreach (var place in recommentedVenues.places)
                {
                    var recommendedValues = ((List<NetSquare.FourSquareRecommendedVenues.recommends>)place.Value);
                    foreach (var recommendedValue in recommendedValues)
                    {
                        viewModel.FourSquareVenueList.Add(new NetSquare.FourSquareVenue
                        {
                            url = recommendedValue.venue.url,
                            name = recommendedValue.venue.name,
                            location = new NetSquare.FourSquareLocation{
                                Lat = recommendedValue.venue.location.Lat,
                                Long = recommendedValue.venue.location.Long 
                            },
                            stats = new NetSquare.FourSquareStats
                            {
                                checkinsCount = recommendedValue.venue.stats.checkinsCount,
                                usersCount = recommendedValue.venue.stats.usersCount,
                            }
                        });
                    }
                }
            }

            if (viewModel.FourSquareVenueList.Count > 0)
            {
                var fourSquareHelper = new FourSquareVenueHelper();
                viewModel.FourSquareVenueForChart = fourSquareHelper.GetViewModel(viewModel.FourSquareVenueList);
            }
            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Design and Developed by Ranjan Dailata";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
