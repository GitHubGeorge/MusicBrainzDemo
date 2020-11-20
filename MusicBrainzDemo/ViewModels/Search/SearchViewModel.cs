using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicBrainzDemo.ViewModels.Search
{
    public class SearchViewModel
    {
        [Required] 
        [Display(Name = "Artist name")]
        public string ArtistName { get; set; }
    }
}