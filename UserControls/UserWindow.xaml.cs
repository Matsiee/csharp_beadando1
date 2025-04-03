using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using CsharpBeadando1.Windows;

namespace CsharpBeadando1.UserControls;

public partial class UserWindow : UserControl
{
    public UserWindow(ContentControl contentControl, SQLiteConnection connection)
    {
        InitializeComponent();
        Connection = connection;
        ContentHolder = contentControl;
    }

    private SQLiteConnection Connection { get; }
    private ContentControl ContentHolder { get; }
    public bool isNewMeresOpen;

    private void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        var dialogBox = new Login();
        if (dialogBox.ShowDialog() ?? false)
        {
            var adminWindow = new AdminWindow(Connection, ContentHolder);
            ContentHolder.Content = adminWindow;
        }
    }

    private void OnNewMeresClick(object sender, RoutedEventArgs e)
    {
        if (isNewMeresOpen) return;
        var window = new NewMeres(Connection, this);
        window.Show();
        isNewMeresOpen = true;
    }
}