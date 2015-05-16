using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PerformanceBuilderLib;

namespace InvestmentBuilderClient.View
{
    public partial class PerformanceChartView : Form
    {
        private IndexedRangeData _rangeData;

        public PerformanceChartView(IndexedRangeData rangeData)
        {
            InitializeComponent();
            _rangeData = rangeData;
            this.Text = _rangeData.Name;
        }

        private void OnChartViewLoad(object sender, EventArgs e)
        {
            this.performanceChart.Series.Clear();
            this.performanceChart.Legends.Clear();

            this.performanceChart.Legends["Default"].LegendStyle =
                System.Windows.Forms.DataVisualization.Charting.LegendStyle.Table;
            this.performanceChart.Legends["Default"].TableStyle =
                System.Windows.Forms.DataVisualization.Charting.LegendTableStyle.Auto;
            this.performanceChart.Legends["Default"].Alignment =
                StringAlignment.Center;
            //
            foreach (var index in _rangeData.Data)
            {
                System.Windows.Forms.DataVisualization.Charting.Legend legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
                System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series();

                legend.Name = index.Name;
                this.performanceChart.Legends.Add(legend);
                series.ChartArea = "ChartArea1";
                series.Legend = index.Name;
                series.Name = index.Name;
                series.Points.DataBindXY(index.Data, "Date", index.Data, "Price");
                series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                this.performanceChart.Series.Add(series);
            }
        }
    }
}
