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
        //public SqlConnection Connection { get; set; }
        public string ConnectionStr { get; set; }

        protected T GetDBValue<T>(string name, SqlDataReader reader, T defaultVal = default(T))
        {
            var result = reader[name];
            if (result.GetType() != typeof(System.DBNull))
            {
                return (T)result;
            }

            return defaultVal;
        }

        protected SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(ConnectionStr);
            connection.Open();
            return connection;
        }
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
            ConnectionStr = settings.DatasourceString;
            //Connection = new SqlConnection(settings.DatasourceString);
            //Connection.Open();
            _cashAccountData = new SQLServerCashAccountData(ConnectionStr);
            _clientData = new SQLServerClientData(ConnectionStr);
            _investmentRecordData = new SQLServerInvestmentRecordData(ConnectionStr);
            _userAccountData = new SQLServerUserAccountData(ConnectionStr);
            _historicalData = new SQLServerHistoricalData(ConnectionStr);
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
            ConnectionStr = datasource;
            //Connection.Close();
            //Connection = new SqlConnection(datasource);
            //Connection.Open();
            _clientData.ConnectionStr = ConnectionStr;
            _cashAccountData.ConnectionStr = ConnectionStr;
            _investmentRecordData.ConnectionStr = ConnectionStr;
            _userAccountData.ConnectionStr = ConnectionStr;
            _historicalData.ConnectionStr = ConnectionStr;
        }

        public void Dispose()
        {
            //Connection.Close();
        }
    }
}
