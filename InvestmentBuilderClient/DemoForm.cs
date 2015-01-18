using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

public class Form1 : System.Windows.Forms.Form
{
    private DataGridView dataGridView1 = new DataGridView();
    private BindingSource bindingSource1 = new BindingSource();
    private SqlDataAdapter dataAdapter = new SqlDataAdapter();
    private Button reloadButton = new Button();
    private Button submitButton = new Button();

    [STAThreadAttribute()]
    public static void Main()
    {
        Application.Run(new Form1());
    }

    // Initialize the form.
    public Form1()
    {
        dataGridView1.Dock = DockStyle.Fill;

        reloadButton.Text = "reload";
        submitButton.Text = "submit";
        reloadButton.Click += new System.EventHandler(reloadButton_Click);
        submitButton.Click += new System.EventHandler(submitButton_Click);

        FlowLayoutPanel panel = new FlowLayoutPanel();
        panel.Dock = DockStyle.Top;
        panel.AutoSize = true;
        panel.Controls.AddRange(new Control[] { reloadButton, submitButton });

        this.Controls.AddRange(new Control[] { dataGridView1, panel });
        this.Load += new System.EventHandler(Form1_Load);
        this.Text = "DataGridView databinding and updating demo";
    }

    private void Form1_Load(object sender, System.EventArgs e)
    {
        // Bind the DataGridView to the BindingSource
        // and load the data from the database.
        dataGridView1.DataSource = bindingSource1;
        GetData("select * from Customers");
    }

    private void reloadButton_Click(object sender, System.EventArgs e)
    {
        // Reload the data from the database.
        GetData(dataAdapter.SelectCommand.CommandText);
    }

    private void submitButton_Click(object sender, System.EventArgs e)
    {
        // Update the database with the user's changes.
        dataAdapter.Update((DataTable)bindingSource1.DataSource);
    }

    private void GetData(string selectCommand)
    {
        try
        {
            // Specify a connection string. Replace the given value with a 
            // valid connection string for a Northwind SQL Server sample
            // database accessible to your system.
            String connectionString =
                "Integrated Security=SSPI;Persist Security Info=False;" +
                "Initial Catalog=Northwind;Data Source=localhost";

            // Create a new data adapter based on the specified query.
            dataAdapter = new SqlDataAdapter(selectCommand, connectionString);

            // Create a command builder to generate SQL update, insert, and
            // delete commands based on selectCommand. These are used to
            // update the database.
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

            // Populate a new data table and bind it to the BindingSource.
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);
            bindingSource1.DataSource = table;

            // Resize the DataGridView columns to fit the newly loaded content.
            dataGridView1.AutoResizeColumns(
                DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }
        catch (SqlException)
        {
            MessageBox.Show("To run this example, replace the value of the " +
                "connectionString variable with a connection string that is " +
                "valid for your system.");
        }
    }

    public static SqlDataAdapter CreateCustomerAdapter(
    SqlConnection connection)
    {
        SqlDataAdapter adapter = new SqlDataAdapter();

        // Create the SelectCommand.
        SqlCommand command = new SqlCommand("SELECT * FROM Customers " +
            "WHERE Country = @Country AND City = @City", connection);

        // Add the parameters for the SelectCommand.
        command.Parameters.Add("@Country", SqlDbType.NVarChar, 15);
        command.Parameters.Add("@City", SqlDbType.NVarChar, 15);

        adapter.SelectCommand = command;

        // Create the InsertCommand.
        command = new SqlCommand(
            "INSERT INTO Customers (CustomerID, CompanyName) " +
            "VALUES (@CustomerID, @CompanyName)", connection);

        // Add the parameters for the InsertCommand.
        command.Parameters.Add("@CustomerID", SqlDbType.NChar, 5, "CustomerID");
        command.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 40, "CompanyName");

        adapter.InsertCommand = command;

        // Create the UpdateCommand.
        command = new SqlCommand(
            "UPDATE Customers SET CustomerID = @CustomerID, CompanyName = @CompanyName " +
            "WHERE CustomerID = @oldCustomerID", connection);

        // Add the parameters for the UpdateCommand.
        command.Parameters.Add("@CustomerID", SqlDbType.NChar, 5, "CustomerID");
        command.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 40, "CompanyName");
        SqlParameter parameter = command.Parameters.Add(
            "@oldCustomerID", SqlDbType.NChar, 5, "CustomerID");
        parameter.SourceVersion = DataRowVersion.Original;

        adapter.UpdateCommand = command;

        // Create the DeleteCommand.
        command = new SqlCommand(
            "DELETE FROM Customers WHERE CustomerID = @CustomerID", connection);

        // Add the parameters for the DeleteCommand.
        parameter = command.Parameters.Add(
            "@CustomerID", SqlDbType.NChar, 5, "CustomerID");
        parameter.SourceVersion = DataRowVersion.Original;

        adapter.DeleteCommand = command;

        return adapter;
    }
}