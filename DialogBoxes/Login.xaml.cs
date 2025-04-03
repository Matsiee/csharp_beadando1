using System.Windows;

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
        var pass = "pass";
        if (TextBox.Password == pass)
        {
            DialogResult = true;
            Close();
        }
        else
        {
            DialogResult = false;
            MessageBox.Show("Incorrect password");
        }

    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}