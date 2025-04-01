using System.Data.SQLite;
using System.Windows;

namespace CsharpBeadando1.Windows;

public partial class NewPatient : Window
{
    public NewPatient(SQLiteConnection connection)
    {
        InitializeComponent();
        Connection = connection;
    }
    
    private SQLiteConnection Connection { get; }
    private void OnAddButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        var sql = "insert into paciensek (nev, szobaszam, telefon, szuletesi_datum) values (@nev, @szobaszam, @telefon, @szuletesi_datum)";
        using (var command = new SQLiteCommand(sql, Connection))
        {
            command.Parameters.AddWithValue("@nev", Nev.Text);
            command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
            command.Parameters.AddWithValue("@telefon", Telefon.Text);
            command.Parameters.AddWithValue("@szuletesi_datum", Szuletesi_Datum.Text);
            command.ExecuteNonQuery();
        }
        Connection.Close();
    }
}