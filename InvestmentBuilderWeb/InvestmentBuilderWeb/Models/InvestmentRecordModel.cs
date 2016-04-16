using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBuilderWeb.Models
{
    [MetadataType(typeof(InvestmentRecordAttributes))]
    public class InvestmentRecordModel
    {
        public InvestmentRecordModel()
        {
            Price = 0;
        }

        public InvestmentRecordModel(string name, int quantity, double price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
        }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class InvestmentRecordAttributes
    {
        [Required(ErrorMessage = "You must enter a Name")]
        [MaxLength(50, ErrorMessage =
        "The Name must be less than 50 characters.")]
        public object Name { get; set; }
        [Range(1,999999999, ErrorMessage = "quantity must be greater than 0")]
        [RegularExpression(@"\d{1,3}",
        ErrorMessage = "Only numbers are allowed in the Quantity")]
        public object  Quantity {get;set;}
        [RegularExpression(@"\d{1,3}",
        ErrorMessage = "Only numbers are allowed in the price")]
        public object Price {get;set;}
    }

}