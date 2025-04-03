using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CsharpBeadando1.Windows;

public partial class AdminSelectPatient : Window
{
    public AdminSelectPatient(SQLiteConnection connection, DataGrid dataGrid)
    {
        InitializeComponent();
        Connection = connection;
        DataGrid = dataGrid;
    }

    private SQLiteConnection Connection { get; }
    private DataGrid DataGrid { get; }

    private void OnSelectButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "select datum, pulzus, sys, dia, megjegyzes from paciensek join meresek on paciensek.id = meresek.paciens_id where nev=@nev and szobaszam=@szobaszam order by datum desc";
        command.Parameters.AddWithValue("@nev", Nev.Text);
        command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
        using (var adapter = new SQLiteDataAdapter(command))
        {
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGrid.ItemsSource = dataTable.DefaultView;
        }
        Connection.Close();
        Close();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}