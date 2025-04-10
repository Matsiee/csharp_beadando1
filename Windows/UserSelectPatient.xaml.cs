using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using CsharpBeadando1.UserControls;

namespace CsharpBeadando1.Windows;

public partial class UserSelectPatient : Window
{
    public UserSelectPatient(SQLiteConnection connection, UserWindow userWindow)
     {
         InitializeComponent();
         Connection = connection;
         UserWindow = userWindow;
         DataGrid = userWindow.DataGrid;
     }
 
     private SQLiteConnection Connection { get; }
     private UserWindow UserWindow { get; }
     private DataGrid DataGrid { get; }
 
     private void OnSelectButtonClick(object sender, RoutedEventArgs e)
     {
         Connection.Open();
         var command = Connection.CreateCommand();
         command.CommandText =
             "select datum, idopont, pulzus, sys, dia, megjegyzes, meresek.id as meres_id, paciensek.id as paciens_id from paciensek join meresek on paciensek.id = meresek.paciens_id where nev=@nev and szobaszam=@szobaszam order by datum desc";
         command.Parameters.AddWithValue("@nev", Nev.Text);
         command.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
         using (var adapter = new SQLiteDataAdapter(command))
         {
             var dataTable = UserWindow.MeresDataTable;
             dataTable.Clear();
             adapter.Fill(dataTable);
             DataGrid.ItemsSource = dataTable.DefaultView;
             // Hiding meres and patient id's
             DataGrid.Columns[6].Visibility = Visibility.Hidden;
             DataGrid.Columns[7].Visibility = Visibility.Hidden;
         }
         Connection.Close();
         Close();
         UserWindow.RemoveMeres.IsEnabled = true;
         UserWindow.FilterComboBox.IsEnabled = false;
     }
 
     private void OnCancelButtonClick(object sender, RoutedEventArgs e)
     {
         Close();
     }
 }