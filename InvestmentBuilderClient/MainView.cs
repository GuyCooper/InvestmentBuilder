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
    internal partial class MainView : Form
    {
        private InvestmentDataModel _dataModel;
        private List<IInvestmentBuilderView> _views;

        public MainView(InvestmentDataModel dataModel)
        {
            InitializeComponent();
            _dataModel = dataModel;
            _views = new List<IInvestmentBuilderView>();
        }

        private void UpdateValuationDate()
        {
            DateTime  dtValuation = (DateTime)cmboValuationDate.SelectedItem;
            foreach(var view in _views)
            {
                view.UpdateValuationDate(dtValuation);
            }
        }

        private void _AddView(Form view)
        {
            view.MdiParent = this;
            view.Show();
            var investmentView = view as IInvestmentBuilderView;
            if (investmentView != null)
            {
                _views.Add(investmentView);
            }
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            _AddView(new ReceiptDataView(_dataModel));
            _AddView(new PaymentsDataView(_dataModel));
                
            //this.LayoutMdi(MdiLayout.TileHorizontal);

            cmboValuationDate.Items.AddRange(_dataModel.GetValuationDates().Cast<object>().ToArray());
            cmboValuationDate.SelectedIndex = 0;
            UpdateValuationDate();
        }

        private void btnCommitData_Click(object sender, EventArgs e)
        {
            foreach (var view in _views)
            {
                view.CommitData();
            }
        }

        private void btnRunBuilder_Click(object sender, EventArgs e)
        {

        }

        private void OnValuationDateChanged(object sender, EventArgs e)
        {
            UpdateValuationDate();
        }
    }
}
