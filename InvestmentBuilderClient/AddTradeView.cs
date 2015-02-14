using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvestmentBuilderClient
{
    internal partial class AddTradeView : Form
    {
        public AddTradeView()
        {
            InitializeComponent();
        }

        public string GetTransactionDate()
        {
            return dteTransactionDate.Value.ToShortDateString();   
        }

        public string GetName()
        {
            return txtName.Text;
        }

        public TradeType GetTradeType()
        {
            return (TradeType)Enum.Parse(typeof(TradeType),(string)cmboType.SelectedItem);
        }

        public int GetAmount()
        {
            return (int)nmrcNumber.Value;
        }

        public string GetSymbol()
        {
            return txtSymbol.Text;
        }

        public string GetCurrency()
        {
            return txtCcy.Text;
        }

        public int GetScalingFactor()
        {
            return (int)nmrcScaling.Value;
        }

        public double GetTotalCost()
        {
            return Double.Parse(txtTotalCost.Text);
        }
    }
}
