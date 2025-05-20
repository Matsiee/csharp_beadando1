using System.Windows;

namespace CsharpBeadando1;

public partial class NewNurse : Window
{
    public NewNurse()
    {
        InitializeComponent();
    }

    private void OnAddButtonClick(object sender, RoutedEventArgs e)
    {
        if ((Telefon.Text.Length != 12 && Telefon.Text[0] == '+') || Telefon.Text.Length != 11)
        {
            MessageBox.Show("Invalid Telefon");
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