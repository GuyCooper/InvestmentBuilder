using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderCore;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBuilderWeb.Models
{
    [MetadataType(typeof(TradeItemValidation))]
    public class TradeItemModel : Stock
    { 
        public TradeItemModel()
        {
            Action = TransactionType.NONE;
        }

        public TransactionType Action { get; set; }
        public double? ManualPrice { get; set; }
        public bool IsCreate { get; private set; }
    }

    public class TradeItemValidation
    {
        [Required(ErrorMessage = "A Name must be specified")]
        [MaxLength(50, ErrorMessage = "Maximum Length for name is 50 characters")]
        public object Name { get; set; }
        [MaxLength(3, ErrorMessage="Max length for currency is 3 characters")]
        public object Currency { get; set; }
        [MaxLength(10, ErrorMessage="Max length for exchange is 10 characters")]
        public object Exchange { get; set; }
        [MaxLength(10,ErrorMessage="Max length for symbolis 10 characters")]
        public object Symbol { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object TotalCost { get; set; }
    }
}