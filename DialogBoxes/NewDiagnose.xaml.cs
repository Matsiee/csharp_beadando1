using System.Windows;

namespace CsharpBeadando1.Windows;

public partial class NewDiagnose : Window
{
    public NewDiagnose()
    {
        InitializeComponent();
        Datum.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    private void OnSelectButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}