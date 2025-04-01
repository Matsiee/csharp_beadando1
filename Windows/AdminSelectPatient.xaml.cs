using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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
            "select datum from paciensek join meresek on paciensek.id = meresek.paciens_id where nev=@nev and szobaszam=@szobaszam";
        command.Parameters.AddWithValue("@nev", Nev.Text);
        command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
        var item = "";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                item = reader.GetString(0) + ", " + reader.GetString(1);
            }

            ListBox.Items.Add(item);
        }

        Connection.Close();
        Close();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}