using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CsharpBeadando1.UserControls;

namespace CsharpBeadando1.Windows;

public partial class AdminSelectPatient : Window
{
    public AdminSelectPatient()
    {
        InitializeComponent();
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