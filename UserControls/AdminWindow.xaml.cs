using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using CsharpBeadando1.Windows;

namespace CsharpBeadando1;

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

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        var userWindow = new UserWindow(ContentHolder, Connection);
        ContentHolder.Content = userWindow;
    }

    private void OnAddNurseButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new NewNurse(Connection);
        window.Show();
    }
}