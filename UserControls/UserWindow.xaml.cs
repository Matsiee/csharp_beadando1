﻿using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CsharpBeadando1.DialogBoxes;
using CsharpBeadando1.Windows;

namespace CsharpBeadando1.UserControls;

public partial class UserWindow : UserControl
{
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
        string? nev = null;
        string? szobaszam = null;
        var window = new NewDiagnose();

        if (window.ShowDialog() == true)
        {
            Connection.Open();
            object? paciensId = null;
            var selectCommand = Connection.CreateCommand();
            selectCommand.CommandText =
                "select id from paciensek where nev=@nev and szobaszam=@szobaszam";
            selectCommand.Parameters.AddWithValue("@nev", window.Nev.Text);
            selectCommand.Parameters.AddWithValue("@szobaszam", window.Szobaszam.Text);
            using (var reader = selectCommand.ExecuteReader())
            {
                while (reader.Read()) paciensId = reader.GetValue(0);
            }

            if (paciensId == null)
            {
                MessageBox.Show("Could not find patient.");
                return;
            }

            var sqlInsert =
                "insert into meresek (paciens_id, datum, idopont, pulzus, sys, dia, megjegyzes) values (@paciens_id, @datum, @idopont, @pulzus, @sys, @dia, @megjegyzes)";
            using (var insertCommand = new SQLiteCommand(sqlInsert, Connection))
            {
                insertCommand.Parameters.AddWithValue("@paciens_id", paciensId);
                var datumInput = window.Datum.Text.Split(' ');
                insertCommand.Parameters.AddWithValue("@datum", datumInput[0]);
                insertCommand.Parameters.AddWithValue("@idopont", datumInput[1]);
                insertCommand.Parameters.AddWithValue("@pulzus", window.Pulzus.Text);
                insertCommand.Parameters.AddWithValue("@sys", window.Sys.Text);
                insertCommand.Parameters.AddWithValue("@dia", window.Dia.Text);
                insertCommand.Parameters.AddWithValue("@megjegyzes", window.Megjegyzes.Text);
                insertCommand.ExecuteNonQuery();
            }

            DisplayPatients(PatientsDataTable);
            Connection.Close();
        }
    }

    public void DisplayPatients(DataTable dataTable)
    {
        RemoveMeres.IsEnabled = false;
        FilterComboBox.IsEnabled = true;
        EditMeres.IsEnabled = false;
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
        var window = new UserSelectPatient(Connection);
        if (window.ShowDialog() == true)
        {
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText =
                "select datum, idopont, pulzus, sys, dia, megjegyzes, meresek.id as meres_id, paciensek.id as paciens_id from paciensek join meresek on paciensek.id = meresek.paciens_id where nev=@nev and szobaszam=@szobaszam order by datum desc";
            command.Parameters.AddWithValue("@nev", window.Nev.Text);
            command.Parameters.AddWithValue("@szobaszam", window.Szobaszam.Text);
            using (var adapter = new SQLiteDataAdapter(command))
            {
                var dataTable = MeresDataTable;
                dataTable.Clear();
                adapter.Fill(dataTable);
                DataGrid.ItemsSource = dataTable.DefaultView;
                // Hiding meres and patient id's
                DataGrid.Columns[6].Visibility = Visibility.Hidden;
                DataGrid.Columns[7].Visibility = Visibility.Hidden;
            }

            Connection.Close();
        }

        RemoveMeres.IsEnabled = true;
        FilterComboBox.IsEnabled = false;
        EditMeres.IsEnabled = true;
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

    private void OnDataGridRowDoubleMouseClick(object sender, MouseButtonEventArgs e)
    {
        if (DataGrid.SelectedItem == null) return;
        var row = DataGrid.SelectedItem as DataRowView;
        if (!row.Row.Table.Columns.Contains("Név") || !row.Row.Table.Columns.Contains("Szobaszám")) return;
        var nev = row["Név"].ToString();
        var szobaszam = row["Szobaszám"].ToString();

        Connection.Open();
        var command = Connection.CreateCommand();
        command.CommandText =
            "select datum, idopont, pulzus, sys, dia, megjegyzes, meresek.id as meres_id, paciensek.id as paciens_id from paciensek join meresek on paciensek.id = meresek.paciens_id where nev=@nev and szobaszam=@szobaszam order by datum desc";
        command.Parameters.AddWithValue("@nev", nev);
        command.Parameters.AddWithValue("@szobaszam", szobaszam);
        using (var adapter = new SQLiteDataAdapter(command))
        {
            var dataTable = MeresDataTable;
            dataTable.Clear();
            adapter.Fill(dataTable);
            DataGrid.ItemsSource = dataTable.DefaultView;
            // Hiding meres and patient id's
            DataGrid.Columns[6].Visibility = Visibility.Hidden;
            DataGrid.Columns[7].Visibility = Visibility.Hidden;
        }

        Connection.Close();
        RemoveMeres.IsEnabled = true;
        FilterComboBox.IsEnabled = false;
        EditMeres.IsEnabled = true;
    }

    private void OnEditMeresClick(object sender, RoutedEventArgs e)
    {
        var window = new EditDiagnose();
        if (DataGrid.SelectedItem == null) return;
        var row = DataGrid.SelectedItem as DataRowView;
        if (!row.Row.Table.Columns.Contains("datum") || !row.Row.Table.Columns.Contains("sys")) return;
        var meresId = row["meres_id"].ToString();
        var paciensId = row["paciens_id"].ToString();
        var sys = row["sys"].ToString();
        var dia = row["dia"].ToString();
        var pulzus = row["pulzus"].ToString();
        var megjegyzes = row["megjegyzes"].ToString();

        window.Pulzus.Text = pulzus;
        window.Sys.Text = sys;
        window.Dia.Text = dia;
        window.Megjegyzes.Text = megjegyzes;
        if (window.ShowDialog() == true)
        {
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText =
                "update meresek set pulzus = @pulzus, sys = @sys, dia = @dia, megjegyzes = @megjegyzes where id = @id";
            command.Parameters.AddWithValue("@id", meresId);
            command.Parameters.AddWithValue("@pulzus", window.Pulzus.Text);
            command.Parameters.AddWithValue("@sys", window.Sys.Text);
            command.Parameters.AddWithValue("@dia", window.Dia.Text);
            command.Parameters.AddWithValue("@megjegyzes", window.Megjegyzes.Text);
            command.ExecuteNonQuery();
            Connection.Close();

            Connection.Open();
            var command2 = Connection.CreateCommand();
            command2.CommandText =
                "select datum, idopont, pulzus, sys, dia, megjegyzes, meresek.id as meres_id, paciensek.id as paciens_id from paciensek join meresek on paciensek.id = meresek.paciens_id where meresek.paciens_id = @id order by datum desc";
            command2.Parameters.AddWithValue("@id", paciensId);
            using (var adapter = new SQLiteDataAdapter(command2))
            {
                var dataTable = MeresDataTable;
                dataTable.Clear();
                adapter.Fill(dataTable);
                DataGrid.ItemsSource = dataTable.DefaultView;
                // Hiding meres and patient id's
                DataGrid.Columns[6].Visibility = Visibility.Hidden;
                DataGrid.Columns[7].Visibility = Visibility.Hidden;
            }

            Connection.Close();
            RemoveMeres.IsEnabled = true;
            FilterComboBox.IsEnabled = false;
            EditMeres.IsEnabled = true;
        }
    }
}