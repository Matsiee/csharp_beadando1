using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace CsharpBeadando1.Windows;

public partial class AdminSelectPatient : Window
{
    public AdminSelectPatient(SQLiteConnection connection, ListBox listBox)
    {
        InitializeComponent();
        Connection = connection;
        ListBox = listBox;
    }

    private SQLiteConnection Connection { get; }
    private ListBox ListBox { get; }

    private void OnSelectButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "select nev, pulzus, sys, dia from meresek join paciensek on meresek.paciens_id = paciensek.id where nev=@nev and szobaszam=@szobaszam and datum=@datum";
        command.Parameters.AddWithValue("@nev", Nev.Text);
        command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
        command.Parameters.AddWithValue("@datum", Datum.Text);
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                ListBox.Items.Add(reader.GetString(0));
                ListBox.Items.Add(reader.GetString(1));
            }
        }

        Connection.Close();
        Close();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}