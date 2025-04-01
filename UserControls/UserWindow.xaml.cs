using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CsharpBeadando1;

public partial class UserWindow : System.Windows.Controls.UserControl
{
    public UserWindow(ContentControl contentControl, SQLiteConnection connection)
    {
        InitializeComponent();
        Connection = connection;
        ContentHolder = contentControl;
    }

    private SQLiteConnection Connection { get; }
    private ContentControl ContentHolder { get; }

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

    private void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        var dialogBox = new Login();
        if (dialogBox.ShowDialog() ?? false)
        {
            var adminWindow = new AdminWindow(Connection, ContentHolder);
            ContentHolder.Content = adminWindow;
        }
    }
}