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
using System.Windows.Forms.DataVisualization.Charting;

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
            var legend = this.performanceChart.Legends["Default"];
            var chartArea = this.performanceChart.ChartAreas["ChartArea1"];

            this.performanceChart.Palette = ChartColorPalette.Bright;

            legend.LegendStyle = LegendStyle.Table;
            legend.TableStyle = LegendTableStyle.Auto;
            legend.Alignment = StringAlignment.Center;
            legend.Docking = Docking.Right;

            chartArea.AxisY.Minimum = 0.8;
            chartArea.AxisX.Title = "Date";
            chartArea.AxisY.Title = "Unit Price";
            chartArea.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisX.MinorGrid.Enabled = true;
            chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisY.ScaleView.Zoomable = true;
            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorX.AutoScroll = true;
            chartArea.CursorY.AutoScroll = true;

            //
            this.performanceChart.BorderlineDashStyle = ChartDashStyle.Solid;
            this.performanceChart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            //this.performanceChart.BorderlineColor = Color.Red;

            foreach (var index in _rangeData.Data)
            {
                Series series = new Series();
                series.ChartArea = chartArea.Name;
                series.Legend = legend.Name;
                series.Name = index.Name;
                series.Points.DataBindXY(index.Data, "Date", index.Data, "Price");
                series.MarkerStyle = MarkerStyle.Square;
                series.MarkerSize = 2;
                series.ChartType = SeriesChartType.Line;
                series["PixelPointWidth"] = "20";
                this.performanceChart.Series.Add(series);
            }
        }

        private void btnResetZoom_Click(object sender, EventArgs e)
        {
            this.performanceChart.ChartAreas["ChartArea1"].AxisX.ScaleView.ZoomReset(0);
            this.performanceChart.ChartAreas["ChartArea1"].AxisY.ScaleView.ZoomReset(0);
        }
    }
}
