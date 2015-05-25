using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data.SqlClient;

namespace SQLServerDataLayer
{
    public class SQLServerBase
    {
        public SqlConnection Connection { get; set; }
    }

    public class SQLServerDataLayer : SQLServerBase, IDataLayer, IDisposable
    {
        private SQLServerCashAccountData _cashAccountData;
        private SQLServerClientData _clientData;
        private SQLServerInvestmentRecordData _investmentRecordData;
        private SQLServerUserAccountData _userAccountData;
        private SQLServerHistoricalData _historicalData;

        public SQLServerDataLayer(IConfigurationSettings settings)
        {
            Connection = new SqlConnection(settings.DatasourceString);
            Connection.Open();
            _cashAccountData = new SQLServerCashAccountData(Connection);
            _clientData = new SQLServerClientData(Connection);
            _investmentRecordData = new SQLServerInvestmentRecordData(Connection);
            _userAccountData = new SQLServerUserAccountData(Connection);
            _historicalData = new SQLServerHistoricalData(Connection);
        }

        public IClientDataInterface ClientData
        {
            get { return _clientData; }
        }

        public IInvestmentRecordInterface InvestmentRecordData
        {
            get { return _investmentRecordData; }
        }

        public ICashAccountInterface CashAccountData
        {
            get { return _cashAccountData; }
        }

        public IUserAccountInterface UserAccountData
        {
            get { return _userAccountData; }
        }

        public IHistoricalDataReader HistoricalData
        {
            get { return _historicalData; }
        }

        public void ConnectNewDatasource(string datasource)
        {
            Connection.Close();
            Connection = new SqlConnection(datasource);
            Connection.Open();
            _clientData.Connection = Connection;
            _cashAccountData.Connection = Connection;
            _investmentRecordData.Connection = Connection;
            _userAccountData.Connection = Connection;
            _historicalData.Connection = Connection;
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
