using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using CsharpBeadando1.Windows;

namespace CsharpBeadando1.UserControls;

public partial class AdminWindow : UserControl
{
    public AdminWindow(SQLiteConnection connection, ContentControl contentControl)
    {
        InitializeComponent();
        Connection = connection;
        ContentHolder = contentControl;
    }

    private SQLiteConnection Connection { get; }
    private ContentControl ContentHolder { get; set; }

    private void AccessDatabase()
    {
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText = "select nev from paciensek";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read()) Console.WriteLine(reader.GetString(0));
        }
        Connection.Close();
    }

    private void OnLogOutButtonClick(object sender, RoutedEventArgs e)
    {
        var userWindow = new UserWindow(ContentHolder, Connection);
        ContentHolder.Content = userWindow;
    }

    private void OnAddNurseButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new NewNurse(Connection);
        window.Show();
    }

    private void OnAddPatientButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new NewPatient(Connection);
        window.Show();
    }

    private void OnFindPatientButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new AdminSelectPatient(Connection, ListBox);
        window.Show();
    }
}