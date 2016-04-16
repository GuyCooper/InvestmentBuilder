using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderCore;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBuilderWeb.Models
{
    public enum TransactionType
    {
        BUY,
        SELL,
        CHANGE
    }

    [MetadataType(typeof(CompanyDataAttributes))]
    public class CompanyDataModel : CompanyData
    {
        //public TransactionType Action { get; set; }
    }

    public class CompanyDataAttributes
    {
        [MaxLength(30)]
        public object Name { get; set; }
        [DisplayFormat(DataFormatString="{0:d}")]
        public object LastBrought { get; set; }
        public object Quantity { get; set; }
        [DisplayFormat(DataFormatString="{0:0.00}")]
        public object AveragePricePaid { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object TotalCost { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object SharePrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object NetSellingValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object ProfitLoss { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object MonthChange { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object MonthChangeRatio { get; set; }
    }
}