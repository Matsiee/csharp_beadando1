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
        if (Nev.Text == "" || Nev.Text.Length > 32)
        {
            MessageBox.Show("Invalid name");
            return;
        }
        if (Szobaszam.Text == "" || Szobaszam.Text.Length > 6)
        {
            MessageBox.Show("Invalid Szobaszam");
            return;
        }

        if (Telefon.Text.Length != 12 && Telefon.Text[0] == '+' || Telefon.Text.Length != 11)
        {
            MessageBox.Show("Invalid Telefon");
            return;
        }
        if (Szuletesi_Datum.SelectedDate == null)
        {
            MessageBox.Show("Invalid date");
            return;
        }
        string szuletesiDatum = Szuletesi_Datum.SelectedDate.Value.ToString("yyyy-MM-dd");
        Connection.Open();
        var sql = "insert into paciensek (nev, szobaszam, telefon, szuletesi_datum) values (@nev, @szobaszam, @telefon, @szuletesi_datum)";
        using (var command = new SQLiteCommand(sql, Connection))
        {
            command.Parameters.AddWithValue("@nev", Nev.Text);
            command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
            command.Parameters.AddWithValue("@telefon", Telefon.Text);
            command.Parameters.AddWithValue("@szuletesi_datum", szuletesiDatum);
            command.ExecuteNonQuery();
        }
        Connection.Close();
        Close();
    }
}
