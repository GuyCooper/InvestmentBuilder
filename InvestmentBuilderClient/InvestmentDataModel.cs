using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace InvestmentBuilderClient
{
    class InvestmentDataModel : IDisposable
    {
        private SqlConnection _connection;

        public InvestmentDataModel() 
        {
            var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            _connection = new SqlConnection(connectstr);
            DataSource = new DataTable();
            _connection.Open();
        }

        public DataTable DataSource { get; private set; }

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
            var sqlCommand = new SqlCommand("SELECT * FROM CashAccount", _connection);
            sqlCommand.CommandType = CommandType.Text;
            SqlDataAdapter adaptor = new SqlDataAdapter(sqlCommand);
            var commandBuilder = new SqlCommandBuilder(adaptor);

            adaptor.Fill(DataSource);
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
