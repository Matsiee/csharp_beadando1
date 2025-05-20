using System.Data.SQLite;
using System.Windows;

namespace CsharpBeadando1;

public partial class NewPatient : Window
{
    public NewPatient()
    {
        InitializeComponent();
    }
    
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
        
        DialogResult = true;
        Close();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
