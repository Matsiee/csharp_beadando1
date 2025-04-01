using System.Data.SQLite;
using System.Windows;

namespace CsharpBeadando1.Windows;

public partial class NewNurse : Window
{
    public NewNurse(SQLiteConnection connection)
    {
        InitializeComponent();
        Connection = connection;
    }

    private SQLiteConnection Connection;

    private void OnAddButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        using (SQLiteCommand command = new SQLiteCommand(Connection))
        {
            string sql = $"insert into apolok (nev) values ({Nev.Text})";
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}