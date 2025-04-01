using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace CsharpBeadando1.Windows;

public partial class SelectPatient : Window
{
    public SelectPatient(SQLiteConnection connection, ListBox listBox)
    {
        InitializeComponent();
        Connection = connection;
        ListBox = listBox;
    }

    private SQLiteConnection Connection { get; }
    private ListBox ListBox { get; set; }

    private void OnSelectButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText = "select datum from meresek join paciensek on meresek.paciens_id = paciensek.id where nev=@nev and szobaszam=@szobaszam";
        command.Parameters.AddWithValue("@nev", Nev.Text);
        command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
        using (var reader = command.ExecuteReader())
        {
            int n = 0;
            while (reader.Read())
            {
                ListBox.Items.Add(reader.GetString(0));
                ListBox.Items.Add(reader.GetString(1));
                n++;
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