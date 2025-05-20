using System.IO;
using System.Text.Json;
using System.Windows;

namespace CsharpBeadando1;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
    }

    private void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        var projectDirectory = Path.GetFullPath(@"..\..\..\");
        string json = File.ReadAllText(projectDirectory + "config.json");

        using JsonDocument doc = JsonDocument.Parse(json);
        string pass = doc.RootElement.GetProperty("pass").GetString();

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