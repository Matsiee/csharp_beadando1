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
    private SQLiteConnection Connection { get; }

    private void OnAddButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        string sql = "insert into apolok (nev, email, telefon, adoszam) values (@nev, @email, @telefon, @adoszam)";
        using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
        {
            command.Parameters.AddWithValue("@nev", Nev.Text);
            command.Parameters.AddWithValue("@email", Email.Text);
            command.Parameters.AddWithValue("@telefon", Telefon.Text);
            command.Parameters.AddWithValue("@adoszam", Adoszam.Text);
            command.ExecuteNonQuery();
        }
    }
}