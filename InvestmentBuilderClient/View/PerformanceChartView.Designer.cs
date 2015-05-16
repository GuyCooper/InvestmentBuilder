namespace InvestmentBuilderClient.View
{
    partial class PerformanceChartView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.performanceChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.performanceChart)).BeginInit();
            this.SuspendLayout();
            // 
            // performanceChart
            // 
            this.performanceChart.BorderlineColor = System.Drawing.Color.Aqua;
            this.performanceChart.BorderlineWidth = 2;
            chartArea1.Name = "ChartArea1";
            this.performanceChart.ChartAreas.Add(chartArea1);
            this.performanceChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.performanceChart.Location = new System.Drawing.Point(0, 0);
            this.performanceChart.Name = "performanceChart";
            this.performanceChart.Size = new System.Drawing.Size(985, 366);
            this.performanceChart.TabIndex = 0;
            title1.Name = "Title1";
            this.performanceChart.Titles.Add(title1);
            // 
            // PerformanceChartView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 366);
            this.Controls.Add(this.performanceChart);
            this.Name = "PerformanceChartView";
            this.Text = "PerformanceChartView";
            this.Load += new System.EventHandler(this.OnChartViewLoad);
            ((System.ComponentModel.ISupportInitialize)(this.performanceChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart performanceChart;
    }
}