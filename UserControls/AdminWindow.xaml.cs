using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

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
    public DataTable NursesDataTable { get; } = new();

    private void OnLogOutButtonClick(object sender, RoutedEventArgs e)
    {
        var userWindow = new UserWindow(ContentHolder, Connection);
        ContentHolder.Content = userWindow;
    }

    private void OnAddNurseButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new NewNurse();
        if (window.ShowDialog() == true)
        {
            Connection.Open();
            var sql = "insert into apolok (nev, email, telefon, adoszam) values (@nev, @email, @telefon, @adoszam)";
            using (var command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("@nev", window.Nev.Text);
                command.Parameters.AddWithValue("@email", window.Email.Text);
                command.Parameters.AddWithValue("@telefon", window.Telefon.Text);
                command.Parameters.AddWithValue("@adoszam", window.Adoszam.Text);
                command.ExecuteNonQuery();
            }

            Connection.Close();
        }
    }

    private void OnAddPatientButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new NewPatient();
        if (window.ShowDialog() == true)
        {
            var szuletesiDatum = window.Szuletesi_Datum.SelectedDate.Value.ToString("yyyy-MM-dd");
            Connection.Open();
            var sql =
                "insert into paciensek (nev, szobaszam, telefon, szuletesi_datum) values (@nev, @szobaszam, @telefon, @szuletesi_datum)";
            using (var command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("@nev", window.Nev.Text);
                command.Parameters.AddWithValue("@szobaszam", window.Szobaszam.Text);
                command.Parameters.AddWithValue("@telefon", window.Telefon.Text);
                command.Parameters.AddWithValue("@szuletesi_datum", szuletesiDatum);
                command.ExecuteNonQuery();
            }

            Connection.Close();
        }
    }

    private void OnFindPatientButtonClick(object sender, RoutedEventArgs e)
    {
        var window = new AdminSelectPatient(Connection);
        if (window.ShowDialog() == true)
        {
            Connection.Open();
            string paciensId;
            var command = Connection.CreateCommand();
            command.CommandText = "select id from paciensek where nev = @nev and szobaszam = @szobaszam";
            command.Parameters.AddWithValue("@nev", window.Nev.Text);
            command.Parameters.AddWithValue("@szobaszam", window.Szobaszam.Text);
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                paciensId = reader.GetValue(0).ToString();
            }

            DisplayMeresek(MeresekDataTable, paciensId);
            Connection.Close();
            //default values
            RemoveMeres.IsEnabled = true;
            RemovePatient.IsEnabled = false;
            FilterComboBox.IsEnabled = true;
            RemoveNurse.IsEnabled = false;
            Export.IsEnabled = false;
        }
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

    private void DisplayMeresek(DataTable dataTable, string paciensId)
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
        RemoveNurse.IsEnabled = false;
        Export.IsEnabled = false;

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

    public void DisplayNurses(DataTable dataTable)
    {
        var selectCommand = Connection.CreateCommand();
        selectCommand.CommandText =
            "select nev, email, telefon, adoszam, id from apolok";
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
                Export.IsEnabled = true;
                break;
            case "Esti mérések":
                MeresekDataTable.DefaultView.RowFilter = "idopont > '12:00'";
                Export.IsEnabled = true;
                break;
            case "Összes":
                MeresekDataTable.DefaultView.RowFilter = string.Empty;
                Export.IsEnabled = true;
                break;
        }
    }

    private void OnViewNursesClick(object sender, RoutedEventArgs e)
    {
        RemoveMeres.IsEnabled = false;
        RemovePatient.IsEnabled = false;
        FilterComboBox.IsEnabled = false;
        RemoveNurse.IsEnabled = true;
        Export.IsEnabled = false;

        Connection.Open();
        DisplayNurses(NursesDataTable);
        Connection.Close();
    }

    private void OnDataGridRowDoubleMouseClick(object sender, MouseButtonEventArgs e)
    {
        if (DataGrid.SelectedItem == null) return;
        var row = DataGrid.SelectedItem as DataRowView;
        if (!row.Row.Table.Columns.Contains("nev") || !row.Row.Table.Columns.Contains("szobaszam")) return;
        var nev = row["nev"].ToString();
        var szobaszam = row["szobaszam"].ToString();

        Connection.Open();
        string paciensId;
        var command = Connection.CreateCommand();
        command.CommandText = "select id from paciensek where nev = @nev and szobaszam = @szobaszam";
        command.Parameters.AddWithValue("@nev", nev);
        command.Parameters.AddWithValue("@szobaszam", szobaszam);
        using (var reader = command.ExecuteReader())
        {
            reader.Read();
            paciensId = reader.GetValue(0).ToString();
        }

        DisplayMeresek(MeresekDataTable, paciensId);
        Connection.Close();
        //default values
        RemoveMeres.IsEnabled = true;
        RemovePatient.IsEnabled = false;
        FilterComboBox.IsEnabled = true;
        RemoveNurse.IsEnabled = false;
        Export.IsEnabled = false;
    }

    private void OnRemoveNurseClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null) return;
        var item = DataGrid.SelectedCells[0].Item as DataRowView;
        if (item == null) return;
        var row = item.Row;
        var id = row["id"];
        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "delete from apolok where id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        DisplayNurses(NursesDataTable);
        Connection.Close();
    }

    private void OnExportClick(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            FileName = "paciens_adatok",
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
            DefaultExt = ".xml",
            Title = "Páciens adatok exportálása"
        };

        bool? result = dialog.ShowDialog();
        if (result == true)
        {
            string filePath = dialog.FileName;

            if (DataGrid.ItemsSource is DataView dataView)
            {
                var dataTable = dataView.ToTable();
                dataTable.TableName = "data";
                dataTable.WriteXml(filePath, XmlWriteMode.WriteSchema);
            }
        }
    }
}