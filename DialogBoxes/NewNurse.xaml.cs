using System.Windows;

namespace CsharpBeadando1.Windows;

public partial class NewNurse : Window
{
    public NewNurse()
    {
        InitializeComponent();
    }

    private void OnAddButtonClick(object sender, RoutedEventArgs e)
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