using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CsharpBeadando1.UserControls;

namespace CsharpBeadando1.Windows;

public partial class AdminSelectPatient : Window
{
    public AdminSelectPatient(SQLiteConnection connection, AdminWindow adminWindow)
    {
        InitializeComponent();
        Connection = connection;
        AdminWindow = adminWindow;
        DataGrid = adminWindow.DataGrid;
    }

    private SQLiteConnection Connection { get; }
    private AdminWindow AdminWindow { get; }
    private DataGrid DataGrid { get; }

    private void OnSelectButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        string paciensId;
        var command = Connection.CreateCommand();
        command.CommandText = "select id from paciensek where nev = @nev and szobaszam = @szobaszam";
        command.Parameters.AddWithValue("@nev", Nev.Text);
        command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
        using (var reader = command.ExecuteReader())
        {
            paciensId = reader.GetString(0);
        }

        AdminWindow.DisplayMeresek(AdminWindow.MeresekDataTable, paciensId);
        Connection.Close();
        Close();
        //default values
        AdminWindow.RemoveMeres.IsEnabled = true;
        AdminWindow.RemovePatient.IsEnabled = false;
        AdminWindow.FilterComboBox.IsEnabled = true;
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}