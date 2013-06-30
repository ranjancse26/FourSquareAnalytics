using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FourSquareExplorer.Models
{
    public class FourSquareVenueVenueEntity
    {
        [Required]
        [Display(Name = "Address")]
        public string LocationName { get; set; }
        [Required]
        [Display(Name = "Search Text")]
        public string Query { get; set; }
    }

    public class FourSquareVenueViewModel
    {
        public FourSquareVenueViewModel()
        {
            this.FourSquareVenueEntity = new FourSquareVenueVenueEntity();
            this.FourSquareVenueList = new List<NetSquare.FourSquareVenue>();
            this.FourSquareVenueForChart = null;
        }
        public FourSquareVenueVenueEntity FourSquareVenueEntity { get; set; }
        public List<NetSquare.FourSquareVenue> FourSquareVenueList { get; set; }
        public FourSquareVenueForChart FourSquareVenueForChart { get; set; }
    }

    public class FourSquareVenueForChart
    {
        public string Labels { get; set; }
        public string CheckIn { get; set; }       
    }
}