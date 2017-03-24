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
    internal class AssetListParts
    {
        public IEnumerable<CompanyData> Companies { get; set; }
        public Unit Height { get; set; }
        public bool LastPart { get; set; }
    }

    public class PdfInvestmentReportWriter : IInvestmentReportWriter, IDisposable
    {
        //private MigraDoc.DocumentObjectModel.Document _document = null;
        private PdfDocument _pdfDocument = null;
        private const double dataCellWidth = 2.1;
        private const double HeaderRowHeight = 1.3d;
        private const double RowHeight = 0.45d;
        private const double DoubleRowHeight = 0.9d;
        private const int MaxAssetsPerChart = 17;

        private const int MaxXLabelCount = 12;

        private static readonly Unit _MaxTableHeight = Unit.FromCentimeter(15);

        private static readonly List<KeyValuePair<string,double>> _headerNames = new List<KeyValuePair<string,double>>
        {
            new KeyValuePair<string, double>("Investment",3.5),
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

        public static string GetPdfReportFile(string outputPath, DateTime ValuationDate)
        {
            return string.Format(@"{0}\ValuationReport-{1}.pdf", outputPath, ValuationDate.ToString("MMM-yyyy"));
        }

        public string GetReportFileName(string outputPath, DateTime ValuationDate)
        {
            return GetPdfReportFile(outputPath, ValuationDate);
        }

        public void Dispose()
        {
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
            //table.Rows.LeftIndent = Unit.FromCentimeter(10);
            table.Borders.Color = Colors.CornflowerBlue;
            Column col = table.AddColumn(Unit.FromCentimeter(4.2));
            col.Shading.Color = Colors.LightBlue;
            return table;
        }

        private void _AddAmountRow(Table table, string name, double amount)
        {
            Row row = table.AddRow();
            row.Cells[0].AddParagraph().AddFormattedText(name, TextFormat.Bold);
            row.Cells[1].AddParagraph(amount.ToString("#.##"));
        }

        /// <summary>
        /// break down the reports asset list into smaller parts so they
        /// can fit onto the page for the asset report
        /// </summary>
        /// <param name="companyData"></param>
        /// <returns></returns>
        private List<AssetListParts> _breakdownAssets(IEnumerable<CompanyData> companyData)
        {
            var result = new List<AssetListParts>();
            var partList = new List<CompanyData>();
            Unit height = Unit.FromCentimeter(HeaderRowHeight);
            foreach (var company in companyData)
            {
                if (company.Name.Contains(" ") && company.Name.Length > 12)
                {
                    height += Unit.FromCentimeter(DoubleRowHeight);
                }
                else
                {
                    height += Unit.FromCentimeter(RowHeight);
                }

                partList.Add(company);
                if(height > _MaxTableHeight)
                {
                    result.Add(
                        new AssetListParts
                        {
                            Companies = partList,
                            Height = height,
                            LastPart = false
                        });
                    height = Unit.FromCentimeter(HeaderRowHeight);
                    partList = new List<CompanyData>();
                }
            }

            if (partList.Count != 0)
            {
                result.Add(new AssetListParts
                {
                    Companies = partList,
                    Height = height,
                    LastPart = true
                });
            }
            else
            {
                result.Last().LastPart = true;
            }

            return result;
        }

        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath, ProgressCounter progress)
        {
            var reportFileName = GetReportFileName(outputPath, report.ValuationDate);
            if (File.Exists(reportFileName))
                File.Delete(reportFileName);

            string title = string.Format("Valuation Report For {0} - {1}", report.AccountName, report.ValuationDate.ToShortDateString());
            _CreateDocument(title);

            //before we can render the table, we need to break down the
            //company data information into sizes that can fit onto a page
            var parts = _breakdownAssets(report.Assets);

            progress.Initialise("writing pdf asset report", parts.Count);

            foreach (var part in parts)
            {
                var document = new MigraDoc.DocumentObjectModel.Document();
                Section section = document.AddSection();
                section.PageSetup.HeaderDistance = Unit.FromCentimeter(0);
                section.PageSetup.TopMargin = Unit.FromCentimeter(1);
                section.PageSetup.FooterDistance = Unit.FromCentimeter(0);
                section.PageSetup.BottomMargin = Unit.FromCentimeter(1);
                section.PageSetup.Orientation = Orientation.Landscape;
                Paragraph heading = section.AddParagraph();
                heading.Format.SpaceBefore = Unit.FromCentimeter(1);
                heading.Format.SpaceAfter = Unit.FromCentimeter(1);
                heading.AddFormattedText(report.AccountName, TextFormat.Bold);
                heading.AddLineBreak();
                heading.AddFormattedText(string.Format("Valuation Date: {0}", report.ValuationDate.ToShortDateString()));
                heading.AddLineBreak();
                heading.AddFormattedText(string.Format("Reporting Currency: {0}", report.ReportingCurrency));

                Table table = section.AddTable();
                table.Style = "Table";
                table.Borders.Width = "0.25";
                table.Borders.Left.Width = "0.5";
                table.Borders.Right.Width = "0.5";
                table.Rows.LeftIndent = 0;
                table.Borders.Color = Colors.CornflowerBlue;

                foreach (var cell in _headerNames)
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

                for (int i = 0; i < _headerNames.Count; ++i)
                {
                    header.Cells[i].AddParagraph(_headerNames[i].Key);
                    header.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                    header.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                }

                //table.SetEdge(0, 0, 6, 2, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
                Row row;

                //now populate the rows
                foreach (var asset in part.Companies)
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

                Table totalsTable = null;
                Table summaryTable = null;

                if (part.LastPart == true)
                {
                    totalsTable = _CreateInfoTable(section);
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

                    summaryTable = _CreateInfoTable(section);
                    summaryTable.AddColumn(Unit.FromCentimeter(dataCellWidth));
                    _AddAmountRow(summaryTable, "Bank Balance", report.BankBalance);
                    _AddAmountRow(summaryTable, "Total Assets", report.TotalAssets);
                    _AddAmountRow(summaryTable, "Total Liabilities", report.TotalLiabilities);
                    _AddAmountRow(summaryTable, "Net Assets", report.NetAssets);
                    _AddAmountRow(summaryTable, "Issued Units", report.IssuedUnits);
                    _AddAmountRow(summaryTable, "Value Per Unit", report.ValuePerUnit);
                    _AddAmountRow(summaryTable, "YTD", report.YearToDatePerformance);
                }

                _RenderAssetTable(document,
                                    heading,
                                    table,
                                    totalsTable,
                                    summaryTable,
                                    part.Height);

                progress.Increment();
            }
            //_pdfDocument.Save(_reportFileName);
            //_pdfDocument.Close();
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


        private void _RenderAssetTable(MigraDoc.DocumentObjectModel.Document document, 
                                       Paragraph heading,
                                       Table assetTable,
                                       Table totalsTable,
                                       Table summaryTable,
                                       Unit tableHeight
                                       )
        {
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(document);
            docRenderer.PrepareDocument();

            PdfPage page = _pdfDocument.AddPage();
           
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Landscape;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;

            var yPos = XUnit.FromCentimeter(1);
            var xPos = XUnit.FromCentimeter(1);
            var width = XUnit.FromCentimeter(20);
            docRenderer.RenderObject(gfx, xPos, yPos, width, heading);
            yPos += XUnit.FromCentimeter(2);
            //assetTable.Rows.Height = Unit.FromCentimeter(20);
     
            docRenderer.RenderObject(gfx, xPos, yPos, width, assetTable);

            xPos += Unit.FromCentimeter(10.2d);
            if (totalsTable != null)
            {
                yPos += tableHeight;
                docRenderer.RenderObject(gfx, xPos, yPos, width, totalsTable);
            }
            if (summaryTable != null)
            {
                yPos += Unit.FromCentimeter(0.6d); //_GetTableHeight(totalsTable);
                docRenderer.RenderObject(gfx, xPos, yPos, width, summaryTable);
            }
        }

        /// <summary>
        /// the pdf generator has a bit of a problem just rendering
        /// a single asset onto a graph so this method ensures at least 3
        /// curves are rendered (unless there is only a single asset)
        /// </summary>
        /// <param name="assetCount"></param>
        /// <returns></returns>
        private int _DetermineMaxAssetsPerChart(int assetCount)
        {
            if (assetCount <= MaxAssetsPerChart)
                return MaxAssetsPerChart;

            var maxCalculatedValue = MaxAssetsPerChart;
            var modVal = assetCount % maxCalculatedValue;
            if(modVal > 0)
            {
                while(modVal < 3)
                {
                    maxCalculatedValue--;
                    modVal = assetCount % maxCalculatedValue;
                }
            }
            return maxCalculatedValue;
        }

        /// <summary>
        /// this method breaks down the number of individual indexes for each graph
        /// in the performance charts. each index is defined by a single asset. this is done because there is only
        /// a limited number of indexes that can be rendered on each graph
        /// </summary>
        /// <param name="rangeData"></param>
        /// <returns></returns>
        private IEnumerable<IndexedRangeData> _breakdownIndexData(IndexedRangeData rangeData) 
        {
            var result = new List<IndexedRangeData>();
            var tempRange = rangeData.CreateFromTemplate();
            int count = 0;
            int maxCount1 = _DetermineMaxAssetsPerChart(rangeData.Data.Count);
            foreach (IndexData index in rangeData.Data)
            {
                int subCount = 0;
                var tempSubRange = index.CreateFromTemplate();
                int maxCount2 = _DetermineMaxAssetsPerChart(index.Data.Count);
                foreach (var subIndex in index.Data)
                {
                    tempSubRange.Data.Add(subIndex);
                    if(++subCount > maxCount2)
                    {
                        if (rangeData.IsHistorical == false)
                        {
                            tempRange.Data.Add(tempSubRange);
                            result.Add(tempRange);
                            tempSubRange = index.CreateFromTemplate();
                            tempRange = rangeData.CreateFromTemplate();
                            subCount = 0;
                        }
                    }
                }
                if(subCount > 0)
                {
                    tempRange.Data.Add(tempSubRange);
                }

                if (++count > maxCount1)
                {
                    result.Add(tempRange);
                    tempRange = rangeData.CreateFromTemplate();
                    count = 0;
                }
            }

            if (tempRange.Data.Count > 0)
            {
                result.Add(tempRange);
            }

            return result;
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
        //the available graph size. this is used to normalise graphs of
        //historical data indexes that go back several years otherwise it is
        //not possible to render all the data points on the xaxis
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
                        for (int i = 0; i < index.Data.Count; i += increment)
                        {
                            result.Add(index.Data[i]);
                        }
                    }
                    index.Data = result;
                }
            }
        }

        public void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation, ProgressCounter progress)
        {
            progress.Initialise("writing performance data to pdf report", data.Count+2);
            string title = string.Format(@"{0}\Performance Report-{1}", outputPath, dtValuation);
            var reportFileName = GetReportFileName(outputPath, dtValuation);
 
            _CreateDocument(title);

            progress.Increment();
            foreach (var rangeIndex in data.Reverse())
            {
                if (rangeIndex.Data.Count == 0)
                {
                    continue;
                }

                bool bRepeated = false;
                var subRangeData = _breakdownIndexData(rangeIndex);
                foreach (var subRangeIndex in subRangeData)
                {
                    PdfSharp.Charting.ChartType chartType = subRangeIndex.IsHistorical == true ?
                    PdfSharp.Charting.ChartType.Line : PdfSharp.Charting.ChartType.Column2D;

                    var yAxisKey = subRangeIndex.IsHistorical ? "Unit Price" : subRangeIndex.Name;
                    var xAxisKey = subRangeIndex.IsHistorical ? "Date" : subRangeIndex.KeyName;

                    if (subRangeIndex.IsHistorical == true)
                    {
                        _NormaliseData(subRangeIndex.Data);
                    }

                    var xAxisValues = subRangeIndex.Data[0].Data.Select(x =>
                    {
                        return x.Date.HasValue ? x.Date.Value.ToString("MM-yy") : x.Key;
                    }).ToList();

                    var chart = _CreateChart(chartType, xAxisKey, yAxisKey, xAxisValues, subRangeIndex.Data, subRangeIndex.MinValue,
                                             subRangeIndex.IsHistorical);

                    string fullTitle = string.Format("{0} {1} {2}", subRangeIndex.Title, subRangeIndex.Name, bRepeated == true ? "Contd..." : "");
                    _RenderChart(_pdfDocument, fullTitle, chart);
                    bRepeated = true;
                }
                progress.Increment();
            }

            _pdfDocument.Save(reportFileName);
            _pdfDocument.Dispose();
            _pdfDocument = null;

            progress.Increment();
        }
    }
}
