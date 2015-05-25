using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvestmentBuilder;

namespace InvestmentBuilderClient.View
{
    public partial class AssetReportView : Form
    {
        private AssetReport _report;
        private static DataGridViewCellStyle _bold;
  
        public AssetReportView(AssetReport report)
        {
            InitializeComponent();
            _report = report;

             _bold = new DataGridViewCellStyle
                        {
                            Font =  new Font(gridAssetReport.Font, FontStyle.Bold)  
                        };
        }

        private void AddTagValue<T>(string tag, T Value, int row, int col,bool bold)
        {
            gridAssetReport.Rows[row].Cells[col++].Value = tag;
            gridAssetReport.Rows[row].Cells[col++].Value = Value;
           
            if (bold)
            {
                gridAssetReport.Rows[row].DefaultCellStyle = _bold;
            }
        }

        private void AssetReportView_Load(object sender, EventArgs e)
        {
            var assets  = _report.Assets.ToList();

            for (int i = 0; i < 12; i++)
            {
                gridAssetReport.Columns.Add(new DataGridViewColumn
                {
                    CellTemplate = new DataGridViewTextBoxCell()
                });
            }

            for (int i = 0; i < assets.Count + 20; i++ )
            {
                gridAssetReport.Rows.Add();
            }

            int row = 0;
            AddTagValue("Account Name", _report.AccountName, row++, 1, true);
            AddTagValue("Reporting Currency", _report.ReportingCurrency, row++, 1, true);
            AddTagValue("Valuation Date", _report.ValuationDate.ToShortDateString(), row++, 1, true);
            row++;
            //add asset headers
            var col = 1;
            gridAssetReport.Rows[row].DefaultCellStyle = _bold;
            gridAssetReport.Rows[row].Cells[col++].Value = "Company Name";
            gridAssetReport.Rows[row].Cells[col++].Value = "Last Bought Date";
            gridAssetReport.Rows[row].Cells[col++].Value = "Shares Held";
            gridAssetReport.Rows[row].Cells[col++].Value = "Average Buy Price";
            gridAssetReport.Rows[row].Cells[col++].Value = "Total Cost";
            gridAssetReport.Rows[row].Cells[col++].Value = "Selling Price / Share";
            gridAssetReport.Rows[row].Cells[col++].Value = "Net Selling Value";
            gridAssetReport.Rows[row].Cells[col++].Value = "Profit/Loss";
            gridAssetReport.Rows[row].Cells[col++].Value = "Month Change";
            gridAssetReport.Rows[row].Cells[col++].Value = "Month Change %";
            gridAssetReport.Rows[row].Cells[col++].Value = "Dividend";
            
            foreach(var asset in assets)
            {
                col = 1;
                row++;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.sName;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dtLastBrought.Value.ToShortDateString();
                gridAssetReport.Rows[row].Cells[col++].Value = asset.iNumberOfShares;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dAveragePricePaid;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dTotalCost;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dSharePrice;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dNetSellingValue;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dProfitLoss;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dMonthChange;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dMonthChangeRatio;
                gridAssetReport.Rows[row].Cells[col++].Value = asset.dDividend;
            }
            AddTagValue("Total Value of Investments", _report.TotalAssetValue, ++row, 6, true);
            gridAssetReport.Rows[row].Cells[9].Value = assets.Sum(a => a.dMonthChange);
            gridAssetReport.Rows[row].Cells[11].Value = assets.Sum(a => a.dDividend);
            AddTagValue("Bank Balance", _report.BankBalance, ++row, 6, true);
            AddTagValue("Total Assets", _report.TotalAssets, ++row, 6, true);
            AddTagValue("Total Liabilities", _report.TotalLiabilities, ++row, 6, true);
            AddTagValue("Net Assets", _report.NetAssets, ++row, 6, true);
            AddTagValue("Issued Units", _report.IssuedUnits, ++row, 6, true);
            AddTagValue("Value Per Unit", _report.ValuePerUnit, ++row, 6, true);
        }
    }
}
