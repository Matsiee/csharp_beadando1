using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using CsharpBeadando1.Windows;

namespace CsharpBeadando1.UserControls;

public partial class AdminWindow : UserControl
{
    public AdminWindow(SQLiteConnection connection, ContentControl contentControl)
    {
        InitializeComponent();
        Connection = connection;
        ContentHolder = contentControl;
    }

    private SQLiteConnection Connection { get; }
    private ContentControl ContentHolder { get; }
    public DataTable MeresekDataTable { get; } = new();
    public DataTable PatientsDataTable { get; } = new();

    private void OnLogOutButtonClick(object sender, RoutedEventArgs e)
    {
        var userWindow = new UserWindow(ContentHolder, Connection);
        ContentHolder.Content = userWindow;
    }

    private void OnAddNurseButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new NewNurse(Connection);
        window.Show();
    }

    private void OnAddPatientButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new NewPatient(Connection);
        window.Show();
    }

    private void OnFindPatientButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new AdminSelectPatient(Connection, this);
        window.Show();
    }

    private void OnRemoveMeresClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null) return;
        var item = DataGrid.SelectedCells[0].Item as DataRowView;
        if (item == null) return;
        var row = item.Row;
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "delete from meresek where id = @id";
        command.Parameters.AddWithValue("@id", row["meres_id"]);
        command.ExecuteNonQuery();
        DisplayMeresek(MeresekDataTable, row["paciens_id"].ToString());
        Connection.Close();
    }

    public void DisplayMeresek(DataTable dataTable, string paciensId)
    {
        var selectCommand = Connection.CreateCommand();
        selectCommand.CommandText =
            "select datum, idopont, pulzus, sys, dia, megjegyzes, meresek.id as meres_id, paciensek.id as paciens_id from paciensek join meresek on paciensek.id = meresek.paciens_id where paciensek.id = @id order by datum desc";
        selectCommand.Parameters.AddWithValue("@id", paciensId);
        using (var adapter = new SQLiteDataAdapter(selectCommand))
        {
            dataTable.Clear();
            adapter.Fill(dataTable);
            DataGrid.ItemsSource = dataTable.DefaultView;
            // Hiding meres and patient id's
            DataGrid.Columns[6].Visibility = Visibility.Hidden;
            DataGrid.Columns[7].Visibility = Visibility.Hidden;
        }
    }

    private void OnViewPatientsClick(object sender, RoutedEventArgs e)
    {
        RemoveMeres.IsEnabled = false;
        RemovePatient.IsEnabled = true;
        FilterComboBox.IsEnabled = false;

        Connection.Open();
        DisplayPatients(PatientsDataTable);
        Connection.Close();
    }

    private void OnRemovePatientClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null) return;
        var item = DataGrid.SelectedCells[0].Item as DataRowView;
        if (item == null) return;
        var row = item.Row;
        var id = row["paciens_id"];
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "delete from paciensek where id = @id; delete from meresek where paciens_id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        DisplayPatients(PatientsDataTable);
        Connection.Close();
    }

    public void DisplayPatients(DataTable dataTable)
    {
        var selectCommand = Connection.CreateCommand();
        selectCommand.CommandText =
            "select nev, szobaszam, telefon, szuletesi_datum, id as paciens_id from paciensek";
        using (var adapter = new SQLiteDataAdapter(selectCommand))
        {
            dataTable.Clear();
            adapter.Fill(dataTable);
            DataGrid.ItemsSource = dataTable.DefaultView;
            DataGrid.Columns[4].Visibility = Visibility.Hidden;
        }
    }

    private void OnFilterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = FilterComboBox.SelectedItem as ComboBoxItem;
        if (item == null) return;
        switch (item.Content)
        {
            case "Reggeli mérések":
                MeresekDataTable.DefaultView.RowFilter = "idopont < '12:00'";
                break;
            case "Esti mérések":
                MeresekDataTable.DefaultView.RowFilter = "idopont > '12:00'";
                break;
            case "Összes":
                MeresekDataTable.DefaultView.RowFilter = string.Empty;
                break;
        }
    }
}