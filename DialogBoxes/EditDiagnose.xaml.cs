using System.Windows;

namespace CsharpBeadando1.DialogBoxes;

public partial class EditDiagnose : Window
{
    public EditDiagnose()
    {
        InitializeComponent();
    }

    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
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