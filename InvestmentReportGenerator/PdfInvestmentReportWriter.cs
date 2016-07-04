using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace InvestmentReportGenerator
{
    public class PdfInvestmentReportWriter : IInvestmentReportWriter, IDisposable
    {
        //private MigraDoc.DocumentObjectModel.Document _document = null;
        private PdfDocument _pdfDocument;

        private const double dataCellWidth = 2.1;

        private const int MaxXLabelCount = 12;

        private static readonly List<KeyValuePair<string,double>> _headerNames = new List<KeyValuePair<string,double>>
        {
            new KeyValuePair<string, double>("Investment",3),
            new KeyValuePair<string, double>("Last Bought", 2.5 ),
            new KeyValuePair<string, double>("Quantity",dataCellWidth ),
            new KeyValuePair<string, double>( @"Price Paid \Share", dataCellWidth ),
            new KeyValuePair<string, double>( "Total Cost",dataCellWidth ),
            new KeyValuePair<string, double>( @"Selling Price \Share",dataCellWidth ),
            new KeyValuePair<string, double>( "Net Selling Value",dataCellWidth ),
            new KeyValuePair<string, double>( "PnL",dataCellWidth ),
            new KeyValuePair<string, double>( @"Change \Month",dataCellWidth ),
            new KeyValuePair<string, double>( @"%\Month",dataCellWidth ),
            new KeyValuePair<string, double>( "Dividends",dataCellWidth )
        };

        public PdfInvestmentReportWriter()
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void _DefineStyles()
        {
        }

        private void _CreateDocument(string title)
        {
            if (_pdfDocument == null)
            {
                _pdfDocument = new PdfDocument();
                _pdfDocument.Info.Title = title;
                //_document.DefaultPageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                _DefineStyles();
            }
        }

        //private void _SaveDocument(string filename)
        //{
        //    if (_document != null)
        //    {
        //        MigraDoc.Rendering.PdfDocumentRenderer renderer = new MigraDoc.Rendering.PdfDocumentRenderer(false, PdfSharp.Pdf.PdfFontEmbedding.Always);
        //        renderer.Document = _document;
        //        renderer.RenderDocument();
        //        renderer.PdfDocument.Save(filename);
        //    }
        //}

        private void _AddColumnEntry(Row row, int column, string data)
        {
            row.Cells[column].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[column].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[column].AddParagraph(data);
        }

        private Table _CreateInfoTable(Section section)
        {
            Table table = section.AddTable();
            table.Style = "Table";
            table.Borders.Width = "0.25";
            table.Borders.Left.Width = "0.5";
            table.Borders.Right.Width = "0.5";
            table.Rows.Alignment = RowAlignment.Left;
            table.Rows.LeftIndent = Unit.FromCentimeter(10);
            table.Borders.Color = Colors.CornflowerBlue;
            Column col = table.AddColumn(Unit.FromCentimeter(4));
            col.Shading.Color = Colors.LightBlue;
            return table;
        }

        private void _AddAmountRow(Table table, string name, double amount)
        {
            Row row = table.AddRow();
            row.Cells[0].AddParagraph().AddFormattedText(name, TextFormat.Bold);
            row.Cells[1].AddParagraph(amount.ToString("#.##"));
        }

        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath)
        {
            var reportFileName = string.Format(@"{0}\ValuationReport-{1}.pdf", outputPath, report.ValuationDate.ToString("MMM-yyyy"));
            if (File.Exists(reportFileName))
                File.Delete(reportFileName);

            string title = string.Format("Monthly Report For {0} - {1}", report.AccountName, report.ValuationDate.ToShortDateString());
            _CreateDocument(title);

            var document = new MigraDoc.DocumentObjectModel.Document();
            Section section = document.AddSection();
            section.PageSetup.HeaderDistance = Unit.FromCentimeter(0);
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.FooterDistance = Unit.FromCentimeter(0);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);
            section.PageSetup.Orientation = Orientation.Landscape;
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = Unit.FromCentimeter(1);
            paragraph.Format.SpaceAfter = Unit.FromCentimeter(1);
            paragraph.AddFormattedText(report.AccountName, TextFormat.Bold);
            paragraph.AddLineBreak();
            paragraph.AddFormattedText(string.Format("Valuation Date: {0}", report.ValuationDate.ToShortDateString()));

            Table table = section.AddTable();
            table.Style = "Table";
            table.Borders.Width = "0.25";
            table.Borders.Left.Width = "0.5";
            table.Borders.Right.Width = "0.5";
            table.Rows.LeftIndent = 0;
            table.Borders.Color = Colors.CornflowerBlue;
            
            foreach(var cell in _headerNames)
            {
                Column column = table.AddColumn(Unit.FromCentimeter(cell.Value));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            //add header row
            Row header = table.AddRow();
            header.HeadingFormat = true;
            header.Format.Alignment = ParagraphAlignment.Center;
            header.Format.Font.Bold = true;
            header.Shading.Color = Colors.LightBlue;

            for(int i = 0; i < _headerNames.Count; ++i)
            {
                header.Cells[i].AddParagraph(_headerNames[i].Key);
                header.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                header.Cells[i].VerticalAlignment = VerticalAlignment.Center;
            }

            //table.SetEdge(0, 0, 6, 2, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            Row row;
            //now populate the rows
            foreach (var asset in report.Assets)
            {
                row = table.AddRow();
                int cell = 0;
                _AddColumnEntry(row, cell++, asset.Name);
                _AddColumnEntry(row, cell++, asset.LastBrought.ToShortDateString());
                _AddColumnEntry(row, cell++, asset.Quantity.ToString());
                _AddColumnEntry(row, cell++, asset.AveragePricePaid.ToString("#.##"));
                _AddColumnEntry(row, cell++, asset.TotalCost.ToString("#.##"));
                _AddColumnEntry(row, cell++, asset.SharePrice.ToString("#.###"));
                _AddColumnEntry(row, cell++, asset.NetSellingValue.ToString("#.##"));
                _AddColumnEntry(row, cell++, asset.ProfitLoss.ToString("#.##"));
                _AddColumnEntry(row, cell++, asset.MonthChange.ToString("#.##"));
                _AddColumnEntry(row, cell++, asset.MonthChangeRatio.ToString("#.#"));
                _AddColumnEntry(row, cell++, asset.Dividend.ToString("#.##"));
            }

            Table totalsTable = _CreateInfoTable(section);
            for (int i = 0; i < 5; ++i)
            {
                totalsTable.AddColumn(Unit.FromCentimeter(dataCellWidth));
            }
            
            row = totalsTable.AddRow();
            row.Cells[0].AddParagraph().AddFormattedText("Total Asset Value", TextFormat.Bold);
            row.Cells[1].AddParagraph().AddFormattedText(report.TotalAssetValue.ToString("#.##"));
            row.Cells[2].AddParagraph().AddFormattedText(report.Assets.Sum(x => x.ProfitLoss).ToString("#.##"));
            row.Cells[3].AddParagraph().AddFormattedText(report.Assets.Sum(x => x.MonthChange).ToString("#.##"));
            row.Cells[5].AddParagraph().AddFormattedText(report.Assets.Sum(x => x.Dividend).ToString("#.##"));

            Table amountsTable = _CreateInfoTable(section);
            amountsTable.AddColumn(Unit.FromCentimeter(dataCellWidth));
            _AddAmountRow(amountsTable, "Bank Balance", report.BankBalance);
            _AddAmountRow(amountsTable, "Total Assets", report.TotalAssets);
            _AddAmountRow(amountsTable, "Total Liabilities", report.TotalLiabilities);
            _AddAmountRow(amountsTable, "Net Assets", report.NetAssets);
            _AddAmountRow(amountsTable, "Issued Units", report.IssuedUnits);
            _AddAmountRow(amountsTable, "Value Per Unit", report.ValuePerUnit);

            //_SaveDocument(reportFileName);

        }

        private PdfSharp.Charting.Chart _CreateChart(PdfSharp.Charting.ChartType chartType,
                                                     string xAxis, string yAxis, 
                                                     IList<string> xAxisValues,
                                                     IList<IndexData> indexes,
                                                     double dMinScale,
                                                     bool bIsHistorical)
        {
            PdfSharp.Charting.Chart chart = new PdfSharp.Charting.Chart(chartType);

            //historical price charts have an x axis 
            if (bIsHistorical == true || xAxisValues.Count < 5)
            {
                PdfSharp.Charting.XSeries xvalues = chart.XValues.AddXSeries();
                xvalues.Add(xAxisValues.ToArray());
                foreach (var index in indexes)
                {
                    PdfSharp.Charting.Series yvalues = chart.SeriesCollection.AddSeries();
                    yvalues.Name = index.Name;
                    yvalues.Add(index.Data.Select(x => x.Price).ToArray());
                }
            }
            else
            { 
                foreach (var item in indexes[0].Data)
                {
                    PdfSharp.Charting.Series yvalues = chart.SeriesCollection.AddSeries();
                    yvalues.Name = item.Key;
                    yvalues.Add(item.Price);
                }
            }
            
            chart.XAxis.MajorTickMark = PdfSharp.Charting.TickMarkType.Outside;
            chart.XAxis.Title.Caption = xAxis;
            chart.XAxis.Title.Alignment = PdfSharp.Charting.HorizontalAlignment.Left;
            chart.XAxis.HasMajorGridlines = true;
            
            chart.YAxis.MajorTickMark = PdfSharp.Charting.TickMarkType.Outside;
            chart.YAxis.Title.Caption = yAxis;
            chart.YAxis.HasMajorGridlines = true;
            chart.YAxis.MinimumScale = dMinScale;

            //chart.PlotArea.LineFormat.Color = XColors.DarkGray;
            chart.PlotArea.LineFormat.Width = 1;
            chart.PlotArea.LineFormat.Visible = true;

            chart.Legend.Docking = PdfSharp.Charting.DockingType.Right;

            chart.DataLabel.Type = PdfSharp.Charting.DataLabelType.Value;
            chart.DataLabel.Position = PdfSharp.Charting.DataLabelPosition.OutsideEnd;
            
            chart.HasDataLabel = true;

            return chart;
        }

        private void _RenderAssetTable(PdfDocument document)
        {
            //// Create a renderer and prepare (=layout) the document
            //Document doc = new Document();
            //MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            //docRenderer.PrepareDocument();

            //PdfPage page = document.AddPage();
            //page.Size = PageSize.A4;
            //page.Orientation = PageOrientation.Landscape;
            //XGraphics gfx = XGraphics.FromPdfPage(page);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }

        //each chart is rendered on a seperate page
        private void _RenderChart(PdfDocument document,
                                  string title,
                                  PdfSharp.Charting.Chart chart)
        {
            PdfPage page = document.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Landscape;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Verdana", 13, XFontStyle.Bold);
            gfx.DrawString(title, font, XBrushes.Black,
                                    new System.Drawing.PointF(50, 30));
                                    //XStringFormats.TopCenter);

            var chartFrame = new PdfSharp.Charting.ChartFrame();
            chartFrame.Location = new XPoint(50, 50 );
            chartFrame.Size = new XSize(750, 450);

            chartFrame.Add(chart);
            chartFrame.Draw(gfx);
        }
     
        //method returns a subset of the data (if required) that will fit into
        //the available graph size
        private void _NormaliseData(IList<IndexData> indexData)
        {
            foreach (var index in indexData)
            {
                if (index.Data.Count > MaxXLabelCount)
                {
                    IList<HistoricalData> result = index.Data.Select(x => x).ToList();
                    int increment = 1;
                    while (result.Count > MaxXLabelCount)
                    {
                        increment++;
                        result.Clear();
                        for (int i = increment; i < index.Data.Count; i += increment)
                        {
                            result.Add(index.Data[i - 1]);
                        }
                    }
                    index.Data = result;
                }
            }
        }

        public void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation)
        {
            string title = string.Format(@"{0}\Performance Report-{1}", outputPath, dtValuation);
            var reportFileName = string.Format(@"{0}\ValuationReport-{1}.pdf", outputPath, dtValuation.ToString("MMM-yyyy"));

            if (File.Exists(reportFileName))
                File.Delete(reportFileName);

            PdfDocument document = new PdfDocument(reportFileName);

            foreach (var rangeIndex in data)
            {
                if (rangeIndex.Data.Count == 0)
                {
                    continue;
                }

                PdfSharp.Charting.ChartType chartType = rangeIndex.IsHistorical == true ?
                    PdfSharp.Charting.ChartType.Line : PdfSharp.Charting.ChartType.Column2D;


                var yAxisKey = rangeIndex.IsHistorical ? "Unit Price" : rangeIndex.Name;
                var xAxisKey = rangeIndex.IsHistorical ? "Date" : rangeIndex.KeyName;

                if (rangeIndex.IsHistorical == true)
                {
                    _NormaliseData(rangeIndex.Data);
                }

                var xAxisValues = rangeIndex.Data[0].Data.Select(x =>
                {
                    return x.Date.HasValue ? x.Date.Value.ToString("MM-yy") : x.Key;
                }).ToList();

                var chart = _CreateChart(chartType, xAxisKey, yAxisKey, xAxisValues, rangeIndex.Data, rangeIndex.MinValue,
                                         rangeIndex.IsHistorical);

                string fullTitle = string.Format("{0} {1}", rangeIndex.Title, rangeIndex.Name);
                _RenderChart(document, fullTitle, chart);
            }

            document.Close();
        }
    }
}
