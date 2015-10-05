using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBuilderWeb.Models
{
    [MetadataType (typeof (ItineraryItemAttributes))]
    public class ItineraryItem
    {
    public DateTime? When { get; set; }
    public string Description { get; set; }
    public int? Duration { get; set; }
    public bool IsActive { get; set; }
    public bool? Confirmed { get; set; }
    public string ContactNumber { get; set; }
    }

    //note: datatypes of properties in meta attribute class are important. framework just matches
    //up the names. any properties specified in meta attribute must also be specified in the model
    public class ItineraryItemAttributes
    {
        [Required(ErrorMessage =
        "You must specify when this event will occur")]
        [Remote("VerifyAvailability", "Itinerary",
        AdditionalFields = "Description")]
        public object When { get; set; }
        [Required(ErrorMessage = "You must enter a description")]
        [MaxLength(140, ErrorMessage =
        "The description must be less than 140 characters.")]
        public object Description { get; set; }
        [Required(ErrorMessage =
        "You must specify how long the event will last")]
        [Range(1, 120, ErrorMessage =
        "Events should last between one minute and 2 hours")]
        [RegularExpression(@"\d{1,3}",
        ErrorMessage = "Only numbers are allowed in the duration")]
        public object Duration { get; set; }
        [UIHint("Phone")]
        public object ContactNumber { get; set; }
    }
}