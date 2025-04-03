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
    private ContentControl ContentHolder { get; }

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
        var window = new AdminSelectPatient(Connection, DataGrid);
        window.Show();
    }
}