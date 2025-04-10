using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using CsharpBeadando1.Windows;

namespace CsharpBeadando1.UserControls;

public partial class UserWindow : UserControl
{
    public bool IsNewMeresOpen;

    public UserWindow(ContentControl contentControl, SQLiteConnection connection)
    {
        InitializeComponent();
        Connection = connection;
        ContentHolder = contentControl;
        Connection.Open();
        DisplayPatients(PatientsDataTable);
        Connection.Close();
    }

    public DataTable PatientsDataTable { get; } = new();
    public DataTable MeresDataTable { get; } = new();
    private SQLiteConnection Connection { get; }
    private ContentControl ContentHolder { get; }

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
        if (IsNewMeresOpen) return;
        var window = new NewMeres(Connection, this);
        window.Show();
        IsNewMeresOpen = true;
    }

    public void DisplayPatients(DataTable dataTable)
    {
        RemoveMeres.IsEnabled = false;
        FilterComboBox.IsEnabled = true;
        var exceptionsCommand = Connection.CreateCommand();
        exceptionsCommand.CommandText =
            "select nev as Név, szobaszam as Szobaszám, 'nem' as mertek from paciensek where id not in (select paciens_id from meresek)";
        using (var adapter = new SQLiteDataAdapter(exceptionsCommand))
        {
            dataTable.Clear();
            adapter.Fill(dataTable);
            DataGrid.ItemsSource = dataTable.DefaultView;
        }

        var command = Connection.CreateCommand();
        command.CommandText =
            "select nev as Név, szobaszam as Szobaszám, (case when max(case when date(datum) = @current_date then 1 else 0 end) = 1 then 'igen' else 'nem' end) as 'mertek' from meresek join paciensek on meresek.paciens_id = paciensek.id group by nev order by mertek";
        command.Parameters.AddWithValue("@current_date", DateTime.Now.ToString("yyyy-MM-dd"));
        using (var adapter = new SQLiteDataAdapter(command))
        {
            adapter.Fill(dataTable);
            DataGrid.ItemsSource = dataTable.DefaultView;
        }
    }

    private void OnFilterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = FilterComboBox.SelectedItem as ComboBoxItem;
        if (item == null) return;
        switch (item.Content)
        {
            case "Ma mérték":
                PatientsDataTable.DefaultView.RowFilter = "mertek = 'igen'";
                break;
            case "Ma még nem mérték":
                PatientsDataTable.DefaultView.RowFilter = "mertek = 'nem'";
                break;
            case "Összes":
                PatientsDataTable.DefaultView.RowFilter = string.Empty;
                break;
        }
    }

    private void OnSelectPatientClick(object sender, RoutedEventArgs e)
    {
        var window = new UserSelectPatient(Connection, this);
        window.Show();
    }

    private void OnRemoveMeresClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null) return;
        var item = DataGrid.SelectedCells[0].Item as DataRowView;
        if (item == null) return;
        var row = item.Row;
        var id = row["meres_id"];
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "delete from meresek where id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        var selectCommand = Connection.CreateCommand();
        selectCommand.CommandText =
            "select datum, idopont, pulzus, sys, dia, megjegyzes, meresek.id as meres_id, paciensek.id as paciens_id from paciensek join meresek on paciensek.id = meresek.paciens_id where paciensek.id = @id order by datum desc";
        selectCommand.Parameters.AddWithValue("@id", row["paciens_id"]);
        using (var adapter = new SQLiteDataAdapter(selectCommand))
        {
            MeresDataTable.Clear();
            adapter.Fill(MeresDataTable);
            DataGrid.ItemsSource = MeresDataTable.DefaultView;
            // Hiding meres and patient id's
            DataGrid.Columns[6].Visibility = Visibility.Hidden;
            DataGrid.Columns[7].Visibility = Visibility.Hidden;
        }

        Connection.Close();
    }

    private void OnViewPatientsClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        DisplayPatients(PatientsDataTable);
        Connection.Close();
    }
}