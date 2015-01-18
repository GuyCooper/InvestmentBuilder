using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace InvestmentBuilderClient
{
    class InvestmentDataModel
    {
        private SqlConnection _connection;

        public InvestmentDataModel() 
        {
            _connection = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["InvestmentBuilderTestConnectionString"]);
            DataSource = new DataSet();
        }

        public DataSet DataSource { get; private set; }

        public IEnumerable<DateTime?> GetValuationDates()
        {
            using(var command = new SqlCommand("SELECT Valuation_Date FROM Valuations ORDER BY Valuation_Date ASC", _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetDateTime(1);
                    }
                }
            }
        }

        public void GetData(DateTime? dtPreviousDate, DateTime? dtNextDate)
        {
            var sqlCommand = new SqlCommand("SELECT * FROM CASH_ACCOUNT", _connection);
            SqlDataAdapter adaptor = new SqlDataAdapter(sqlCommand);
            var commandBuilder = new SqlCommandBuilder(adaptor);

            adaptor.Fill(DataSource);
        }
    }
}
