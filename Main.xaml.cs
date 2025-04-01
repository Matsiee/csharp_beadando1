using System.Data.SQLite;
using System.IO;
using System.Windows;
using CsharpBeadando1.UserControls;

namespace CsharpBeadando1;

public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
        var projectDirectory = Path.GetFullPath(@"..\..\..\");
        var path = projectDirectory + "identifier.sqlite";
        var connectionString = $"Data Source={path};Version=3;";
        Connection = new SQLiteConnection(connectionString);
        ContentHolder.Content = new UserWindow(ContentHolder, Connection);
    }

    private SQLiteConnection Connection { get; }
}