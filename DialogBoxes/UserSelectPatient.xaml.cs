using System.Data.SQLite;
using System.Windows;

namespace CsharpBeadando1;

public partial class UserSelectPatient : Window
{
    public UserSelectPatient(SQLiteConnection connection)
    {
        Connection = connection;
        InitializeComponent();
    }

    public SQLiteConnection Connection { get; }

    private void OnSelectButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "select nev, szobaszam from paciensek where nev = @nev and szobaszam = @szobaszam";
        command.Parameters.AddWithValue("@nev", Nev.Text);
        command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
        using (var reader = command.ExecuteReader())
        {
            if (!reader.Read())
            {
                MessageBox.Show("Invalid patient");
                Connection.Close();
                DialogResult = false;
                return;
            }
        }

        Connection.Close();

        DialogResult = true;
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}