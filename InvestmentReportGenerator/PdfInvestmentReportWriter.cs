using System;
using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using NLog;

namespace InvestmentReportGenerator
{
    internal class AssetListParts
    {
        public IEnumerable<CompanyData> Companies { get; set; }
        public Unit Height { get; set; }
        public bool LastPart { get; set; }
    }

    /// <summary>
    /// Class generates the pdf investment report.
    /// </summary>
    public class PdfInvestmentReportWriter : IDisposable
    {
        #region Private classes

        private class DocumentObjectPlacement
        {
            public DocumentObjectPlacement(DocumentObject documentObject,
                                           XUnit xpos,
                                           XUnit ypos,
                                           XUnit width)
            {
                DocumentObject = documentObject;
                XPos = xpos;
                YPos = ypos;
                Width = width;
            }

            public DocumentObject DocumentObject { get; private set; }
            public XUnit XPos { get; private set; }

            public XUnit YPos { get; private set; }
            
            public XUnit Width { get; private set; }
        }
        #endregion

        #region Public Methods

        public PdfInvestmentReportWriter(string title)
        {
            _pdfDocument = new PdfDocument();
            _pdfDocument.Info.Title = title;
            //_document.DefaultPageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
            _DefineStyles();
        }

        public static string GetPdfReportFile(DateTime ValuationDate)
        {
            return $"ValuationReport-{ValuationDate.ToString("MMM-yyyy")}.pdf";
        }

        /// <summary>
        /// SaveReport. Save Report to output path
        /// </summary>
        public void SaveReport(string outputPath, DateTime dtValuation)
        {
            var reportFileName = Path.Combine(outputPath, GetPdfReportFile(dtValuation));
            if (File.Exists(reportFileName))
                File.Delete(reportFileName);

            _pdfDocument.Save(reportFileName);
        }

        /// <summary>
        /// Write asset report to pdf.
        /// </summary>
        public void WriteAssetReport(AssetReport report, ProgressCounter progress)
        {
            logger.Log(LogLevel.Info, "Writing pdf asset report");

            //before we can render the table, we need to break down the
            //company data information into sizes that can fit onto a page
            var parts = _breakdownAssets(report.Assets);

            progress.Initialise("writing pdf asset report", parts.Count);

            foreach (var part in parts)
            {
                var document = new Document();
                Section section = createSection(document);
                var heading = createHeading(section, report.AccountName.Name, report.ValuationDate, report.ReportingCurrency);

                Table table = CreatePdfTable(section, _headerNames);

                Row row;

                //now populate the rows
                foreach (var asset in part.Companies)
                {
                    row = table.AddRow();
                    int cell = 0;
                    _AddCellEntry(row, cell++, asset.Name);
                    _AddCellEntry(row, cell++, asset.LastBrought.ToShortDateString());
                    _AddCellEntry(row, cell++, asset.Quantity.ToString());
                    _AddCellEntry(row, cell++, asset.AveragePricePaid.ToString("N2"));
                    _AddCellEntry(row, cell++, asset.TotalCost.ToString("N2"));
                    _AddCellEntry(row, cell++, asset.SharePrice.ToString("N3"));
                    _AddCellEntry(row, cell++, asset.NetSellingValue.ToString("N2"));
                    _AddCellEntry(row, cell++, asset.ProfitLoss.ToString("N2"));
                    _AddCellEntry(row, cell++, asset.TotalReturn.ToString("N2"));
                    _AddCellEntry(row, cell++, asset.MonthChange.ToString("N2"));
                    _AddCellEntry(row, cell++, asset.MonthChangeRatio.ToString("N1"));
                    _AddCellEntry(row, cell++, asset.Dividend.ToString("N2"));
                }

                Table totalsTable = null;
                Table summaryTable = null;

                if (part.LastPart == true)
                {
                    totalsTable = _CreateInfoTable(section);
                    for (int i = 0; i < 6; ++i)
                    {
                        totalsTable.AddColumn(Unit.FromCentimeter(dataCellWidth));
                    }

                    row = totalsTable.AddRow();
                    row.Cells[0].AddParagraph().AddFormattedText("Total Asset Value", TextFormat.Bold);
                    row.Cells[1].AddParagraph().AddFormattedText(report.TotalAssetValue.ToString("N2"));
                    row.Cells[2].AddParagraph().AddFormattedText(report.Assets.Sum(x => x.ProfitLoss).ToString("N2"));
                    row.Cells[4].AddParagraph().AddFormattedText(report.Assets.Sum(x => x.MonthChange).ToString("N2"));
                    row.Cells[6].AddParagraph().AddFormattedText(report.Assets.Sum(x => x.Dividend).ToString("N2"));

                    summaryTable = _CreateInfoTable(section);
                    summaryTable.AddColumn(Unit.FromCentimeter(dataCellWidth));
                    _AddAmountRow(summaryTable, "Bank Balance", report.BankBalance);
                    _AddAmountRow(summaryTable, "Total Assets", report.TotalAssets);
                    _AddAmountRow(summaryTable, "Net Assets", report.NetAssets);
                    _AddAmountRow(summaryTable, "Issued Units", report.IssuedUnits);
                    _AddAmountRow(summaryTable, "Dividends Tax Year", report.DividendsTaxYear);
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

            _WriteRedemptionsList(report);

            logger.Log(LogLevel.Info, "Finished writing pdf asset report");

        }

        /// <summary>
        /// Methods persists all the performance data to a pdf document.
        /// </summary>
        public void WritePerformanceData(IList<IndexedRangeData> data, ProgressCounter progress)
        {
            progress.Initialise("writing performance data to pdf report", data.Count + 2);                     

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

            progress.Increment();
        }

        public void WriteAnalyticData(AnalyticData data,  ProgressCounter progress)
        {
            progress.Initialise("writing analytic data to pdf report", 2);

            progress.Increment();

            var xPos = XUnit.FromCentimeter(1);
            var yPos = XUnit.FromCentimeter(1);
            var width = XUnit.FromCentimeter(20);
            var documentObjects = new List<DocumentObjectPlacement>();

            var document = new Document();
            Section section = createSection(document);
            var heading = createHeading(section, data.Title, data.ValuationDate, data.ReportingCurrency);
            documentObjects.Add(new DocumentObjectPlacement(heading, xPos, yPos, width));

            yPos += XUnit.FromCentimeter(3);

            var summaryTable = _CreateInfoTable(section, 5.4);

            summaryTable.AddColumn(Unit.FromCentimeter(dataCellWidth));
            _AddAmountRow(summaryTable, "Performance YTD", data.PerformanceYTD);
            _AddAmountRow(summaryTable, "Average Yield", data.AverageYield);

            documentObjects.Add(new DocumentObjectPlacement(summaryTable, xPos, yPos, width));

            yPos += (2 * Unit.FromCentimeter(RowHeight));
            yPos += Unit.FromCentimeter(1);

            //now populate the projections
            var projections = data.ProjectedNAVs;
            if(projections != null)
            {
                var projectionCellWidth = 3d;
                var headers = projections.Ranges.Select(r =>
                               new KeyValuePair<string, double>(r, projectionCellWidth)).ToList();
                headers.Insert(0, new KeyValuePair<string, double>("Projection", projectionCellWidth));

                Table table = CreatePdfTable(section, headers);
                
                foreach (var rateProjection in projections.Projections)
                {
                    int column = 0;
                    Row  row = table.AddRow();
                    _AddCellEntry(row, column++, rateProjection.Key);
                    var rateValues = rateProjection.Value;
                    foreach(var rateValue in rateValues)
                    {
                        _AddCellEntry(row, column++, rateValue.ToString("N2"));
                    }
                }

                documentObjects.Add(new DocumentObjectPlacement(table, xPos, yPos, width));
            }

            RenderDocumentObjects(document, documentObjects);

            progress.Increment();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _pdfDocument.Dispose();
        }

        #endregion

        #region Private Methods

        private void _DefineStyles()
        {
        }

        /// <summary>
        /// Helper method to set a cell value.
        /// </summary>
        private void _AddCellEntry(Row row, int column, string data)
        {
            row.Cells[column].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[column].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[column].AddParagraph(data);
        }

        /// <summary>
        /// Helper method to create an info table.
        /// </summary>
        private Table _CreateInfoTable(Section section, double headerColWidth = 4.2)
        {
            Table table = CreatePdfTable(section, null);
            table.Rows.Alignment = RowAlignment.Left;
            Column col = table.AddColumn(Unit.FromCentimeter(headerColWidth));
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
                if (result.Count > 0)
                {
                    result.Last().LastPart = true;
                }
            }

            return result;
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

        /// <summary>
        /// Create a section and setup the margins etc...
        /// </summary>
        private Section createSection(Document document)
        {
            Section section = document.AddSection();
            section.PageSetup.HeaderDistance = Unit.FromCentimeter(0);
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.FooterDistance = Unit.FromCentimeter(0);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);
            section.PageSetup.Orientation = Orientation.Landscape;
            return section;
        }

        /// <summary>
        /// Helper method to create a heading
        /// </summary>
        private Paragraph createHeading(Section section, string title, DateTime valuationDate, string currency)
        {
            Paragraph heading = section.AddParagraph();
            heading.Format.SpaceBefore = Unit.FromCentimeter(1);
            heading.Format.SpaceAfter = Unit.FromCentimeter(1);
            heading.AddFormattedText(title, TextFormat.Bold);
            heading.AddLineBreak();
            heading.AddFormattedText(string.Format("Valuation Date: {0}", valuationDate.ToShortDateString()));
            heading.AddLineBreak();
            heading.AddFormattedText(string.Format("Reporting Currency: {0}", currency));
            return heading;
        }

        /// <summary>
        /// Helper method for creting a pdf Table
        /// </summary>
        /// <returns></returns>
        private Table createTable(Section section, string topBorderWidth, string bottomBorderWidth, string leftBorderWidth, string rightBorderWidth, Color borderColor)
        {
            Table table = section.AddTable();
            table.Style = "Table";
            table.Borders.Top.Width = topBorderWidth;
            table.Borders.Bottom.Width = bottomBorderWidth;
            table.Borders.Left.Width = leftBorderWidth;
            table.Borders.Right.Width = rightBorderWidth;
            table.Rows.LeftIndent = 0;
            table.Borders.Color = borderColor;
            return table;
        }

        /// <summary>
        /// Helper method to create a header row on a table, headers parameter is the list of header names
        /// to use including the width for each column
        /// </summary>
        private Row createTableHeader(Table table, Color color, List<KeyValuePair<string, double>> headers)
        {
            foreach (var cell in headers)
            {
                Column column = table.AddColumn(Unit.FromCentimeter(cell.Value));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            //add header row
            Row header = table.AddRow();
            header.HeadingFormat = true;
            header.Format.Alignment = ParagraphAlignment.Center;
            header.Format.Font.Bold = true;
            header.Shading.Color = color;

            for (int i = 0; i < headers.Count; ++i)
            {
                header.Cells[i].AddParagraph(headers[i].Key);
                header.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                header.Cells[i].VerticalAlignment = VerticalAlignment.Center;
            }

            return header;
        }

        /// <summary>
        /// Write redemptions list if there are any redemptions.
        /// </summary>
        private void _WriteRedemptionsList(AssetReport report)
        {
            if(report.Redemptions != null && report.Redemptions.Any())
            {
                var yPos = XUnit.FromCentimeter(1);
                var xPos = XUnit.FromCentimeter(1);
                var width = XUnit.FromCentimeter(20);

                var document = new Document();
                Section section = createSection(document);
                var heading = createHeading(section, "Redemptions", report.ValuationDate, report.ReportingCurrency);

                Table table = createTable(section, "0.25", "0.25", "0.5", "0.5", Colors.CornflowerBlue);

                //add header row
                var header = createTableHeader(table, Colors.LightBlue, _redemptionHeaders);

                foreach(var redemption in report.Redemptions)
                {
                    int cell = 0;
                    var row = table.AddRow();
                    _AddCellEntry(row, cell++, redemption.User);
                    _AddCellEntry(row, cell++, redemption.Amount.ToString("#.##"));
                    _AddCellEntry(row, cell++, redemption.TransactionDate.ToShortDateString());
                    _AddCellEntry(row, cell++, redemption.RedeemedUnits.ToString("#.##"));
                    _AddCellEntry(row, cell++, redemption.Status.ToString());
                }

                MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(document);
                docRenderer.PrepareDocument();

                PdfPage page = _pdfDocument.AddPage();

                page.Size = PageSize.A4;
                page.Orientation = PageOrientation.Landscape;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                gfx.MUH = PdfFontEncoding.Unicode;
                gfx.MFEH = PdfFontEmbedding.Default;

                docRenderer.RenderObject(gfx, xPos, yPos, width, heading);
                yPos += XUnit.FromCentimeter(2);
                //assetTable.Rows.Height = Unit.FromCentimeter(20);

                docRenderer.RenderObject(gfx, xPos, yPos, width, table);
            }
        }

        private void RenderDocumentObjects(Document document,
                                           List<DocumentObjectPlacement> documentObjects
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

            foreach(var documentObject in documentObjects )
            {
                docRenderer.RenderObject(gfx,
                                         documentObject.XPos,
                                         documentObject.YPos,
                                         documentObject.Width,
                                         documentObject.DocumentObject);

            }
        }

        /// <summary>
        /// Render the asset report items to the pdf
        /// </summary>
        private void _RenderAssetTable(Document document, 
                                       Paragraph heading,
                                       Table assetTable,
                                       Table totalsTable,
                                       Table summaryTable,
                                       Unit tableHeight
                                       )
        {
            var xPos = XUnit.FromCentimeter(1);
            var yPos = XUnit.FromCentimeter(1);
            var width = XUnit.FromCentimeter(20);
            var documentObjects = new List<DocumentObjectPlacement>();
            documentObjects.Add(new DocumentObjectPlacement(heading, xPos, yPos, width));
            yPos += XUnit.FromCentimeter(2);
            documentObjects.Add(new DocumentObjectPlacement(assetTable, xPos, yPos, width));

            xPos += Unit.FromCentimeter(10.2d);
            if (totalsTable != null)
            {
                yPos += tableHeight;
                documentObjects.Add(new DocumentObjectPlacement(totalsTable, xPos, yPos, width));
            }
            if (summaryTable != null)
            {
                yPos += Unit.FromCentimeter(0.6d);
                documentObjects.Add(new DocumentObjectPlacement(summaryTable, xPos, yPos, width));
            }

            RenderDocumentObjects(document, documentObjects);
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
                    index.Data = DatasetNormaliser.NormaliseDataset<HistoricalData>(index.Data, MaxXLabelCount);
                }
            }
        }

        private Table CreatePdfTable(Section section, List<KeyValuePair<string, double>> headers)
        {
            var table = createTable(section, "0.25", "0.25", "0.5", "0.5", Colors.CornflowerBlue);
            if(headers != null)
            {
                createTableHeader(table, Colors.LightBlue, headers);
            }
            
            return table;
        }
        
        #endregion

        #region Private Data

        //private MigraDoc.DocumentObjectModel.Document _document = null;
        private readonly PdfDocument _pdfDocument = null;
        private const double dataCellWidth = 2.1;
        private const double HeaderRowHeight = 1.3d;
        private const double RowHeight = 0.45d;
        private const double DoubleRowHeight = 0.9d;
        private const int MaxAssetsPerChart = 17;

        private const int MaxXLabelCount = 12;

        private static readonly Unit _MaxTableHeight = Unit.FromCentimeter(15);

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly List<KeyValuePair<string, double>> _headerNames = new List<KeyValuePair<string, double>>
        {
            new KeyValuePair<string, double>("Investment",3.5),
            new KeyValuePair<string, double>("Last Bought", 2.5 ),
            new KeyValuePair<string, double>("Quantity",dataCellWidth ),
            new KeyValuePair<string, double>( @"Price Paid \Share", dataCellWidth ),
            new KeyValuePair<string, double>( "Total Cost",dataCellWidth ),
            new KeyValuePair<string, double>( @"Selling Price \Share",dataCellWidth ),
            new KeyValuePair<string, double>( "Net Selling Value",dataCellWidth ),
            new KeyValuePair<string, double>( "PnL",dataCellWidth ),
            new KeyValuePair<string, double>( "Total Return%",dataCellWidth ),
            new KeyValuePair<string, double>( @"Change \Month",dataCellWidth ),
            new KeyValuePair<string, double>( @"%\Month",dataCellWidth ),
            new KeyValuePair<string, double>( "Dividends",dataCellWidth )
        };

        private static readonly List<KeyValuePair<string, double>> _redemptionHeaders = new List<KeyValuePair<string, double>>
        {
            new KeyValuePair<string, double>("User",9.0),
            new KeyValuePair<string, double>("Amount", 2.5 ),
            new KeyValuePair<string, double>("Date",2.5 ),
            new KeyValuePair<string, double>("Units", 2.5 ),
            new KeyValuePair<string, double>( "Status", 3 )
        };

        #endregion
    }
}
