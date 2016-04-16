using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBuilderWeb.Models
{
    [MetadataType(typeof(TransactionModelAttributes))]
    public class TransactionModel
    {
        public DateTime? TransactionDate { get; set; }
        public string ParameterType { get; set; }
        public string Parameter { get; set; }
        public double Amount { get; set; }
    }

    public class TransactionModelAttributes
    {
        [DisplayFormat(DataFormatString = "{0:d}")]
        public object TransactionDate { get; set; }
        [MaxLength(30)]
        public object Parameter { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object Amount { get; set; }
    }
}