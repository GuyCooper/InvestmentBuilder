using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Dtos
{
    public enum TransactionType
    {
        None,
        Buy,
        Sell,
        Change
    }

    internal class TradeItemDto : Dto
    {
        public string Currency { get; set; }
        public string Exchange { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double ScalingFactor { get; set; }
        public string Symbol { get; set; }
        public double TotalCost { get; set; }
        public DateTime? TransactionDate { get; set; }
        public TransactionType Action { get; set; }
    }
}
