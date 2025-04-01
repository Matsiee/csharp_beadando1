using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace CsharpBeadando1;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
    }

    private void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        string pass = "pass";
        if (TextBox.Text == pass)
        {
            DialogResult = true;
        }
        else
        {
            DialogResult = false;
            MessageBox.Show("Incorrect password");
        }
        Close();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}