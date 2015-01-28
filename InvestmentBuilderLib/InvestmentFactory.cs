using System;
using ExcelAccountsManager;
using System.IO;
using System.Data.SqlClient;

namespace InvestmentBuilder
{
    /// <summary>
    /// factory classes for creating either excel or database builders
    /// </summary>
    interface IInvestmentFactory
    {
        InvestmentRecordBuilder CreateInvestmentRecordBuilder();
        ICompanyDataReader CreateCompanyDataReader();
        AssetStatementWriter CreateAssetStatementWriter();
        ICashAccountReader CreateCashAccountReader();
        void CommitData();
        void Close();
    }

    /// <summary>
    /// factory class for building excel builders
    /// </summary>
    class ExcelInvestmentFactory : IInvestmentFactory
    {
        private ExcelBookHolder _bookHolder;
        private Microsoft.Office.Interop.Excel.Application _app = new Microsoft.Office.Interop.Excel.Application();

        public ExcelInvestmentFactory(string path, DateTime dtValuation, bool bTest)
        {
            _app.DisplayAlerts = false;

            string ext = bTest ? "Test" : dtValuation.Year.ToString();
            string assetSheetLocation = _CreateFormattedFileCopy(path, ExcelBookHolder.MonthlyAssetName, ext);
            string cashAccountLocation = _CreateFormattedFileCopy(path, ExcelBookHolder.CashAccountName, ext);
            string investmentRecordLocation = _CreateFormattedFileCopy(path, ExcelBookHolder.InvestmentRecordName, ext);
            string templateLocation = string.Format("{0}Template.xls", path);

            _bookHolder = new ExcelBookHolder(_app, investmentRecordLocation, assetSheetLocation, templateLocation, cashAccountLocation, path);
        }

        public virtual InvestmentRecordBuilder CreateInvestmentRecordBuilder()
        {
            return new InvestmentRecordBuilderExcel(_bookHolder);
        }

        public virtual ICompanyDataReader CreateCompanyDataReader()
        {
            return new CompanyDataReaderExcel(_bookHolder);
        }

        public virtual AssetStatementWriter CreateAssetStatementWriter()
        {
            return new AssetStatementWriterExcel(_bookHolder);
        }

        public virtual ICashAccountReader CreateCashAccountReader()
        {
            return new CashAccountReaderExcel(_bookHolder);
        }

        public void CommitData()
        {
            _bookHolder.SaveBooks();
        }

        public virtual void Close()
        {
            _app.Workbooks.Close();
        }

        protected ExcelBookHolder _GetBookholder()
        {
            return _bookHolder;
        }

        //rather than update the original spreadsheet, create a copy and update the copy. user verification will then be 
        //required
        private string _CreateFormattedFileCopy(string path, string filename, string ext)
        {
            string originalFile = string.Format("{0}{1}-{2}.xls", path, filename, ext);
            string newFile = string.Format("{0}{1}-{2}.Impl.xls", path, filename, ext);

            if (File.Exists(originalFile) == false)
                return null;

            File.Copy(originalFile, newFile, true);
            return newFile;
        }
    }

    /// <summary>
    /// factory class for building database builders. overrides excel builders because it still builds 
    /// the assetstatement excel sheet.
    /// </summary>
    class DatabaseInvestmentFactory : ExcelInvestmentFactory
    {
        private SqlConnection _conn;

        public DatabaseInvestmentFactory(string path, string connectionstr, DateTime dtValuation, bool bTest) :
            base(path, dtValuation, bTest)
        {
            _conn = new SqlConnection(connectionstr);
            _conn.Open();
        }

        public override InvestmentRecordBuilder CreateInvestmentRecordBuilder()
        {
            return new InvestmentRecordBuilderDatabase(_conn);
        }

        public override ICompanyDataReader CreateCompanyDataReader()
        {
            return new CompanyDataReaderDatabase(_conn);
        }

        public override AssetStatementWriter CreateAssetStatementWriter()
        {
            return new AssetStatementWriterDatabase(_conn, _GetBookholder());
        }

        public override ICashAccountReader CreateCashAccountReader()
        {
            return new CashAccountReaderDatabase(_conn);
        }     

        public override void Close()
        {
            _conn.Close();
            base.Close();
        }

        
    }
}
