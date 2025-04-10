﻿using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using CsharpBeadando1.UserControls;

namespace CsharpBeadando1.Windows;

public partial class NewMeres : Window
{
    public NewMeres(SQLiteConnection connection, UserWindow userWindow)
    {
        InitializeComponent();
        Connection = connection;
        UserWindow = userWindow;
        Datum.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    private SQLiteConnection Connection { get; }
    private UserWindow UserWindow { get; }

    private void OnSelectButtonClick(object sender, RoutedEventArgs e)
    {
        Connection.Open();
        object? paciensId = null;
        var selectCommand = Connection.CreateCommand();
        selectCommand.CommandText =
            "select id from paciensek where nev=@nev and szobaszam=@szobaszam";
        selectCommand.Parameters.AddWithValue("@nev", Nev.Text);
        selectCommand.Parameters.AddWithValue("@szobaszam", Szobaszam.Text);
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
            var datumInput = Datum.Text.Split(' ');
            insertCommand.Parameters.AddWithValue("@datum", datumInput[0]);
            insertCommand.Parameters.AddWithValue("@idopont", datumInput[1]);
            insertCommand.Parameters.AddWithValue("@pulzus", Pulzus.Text);
            insertCommand.Parameters.AddWithValue("@sys", Sys.Text);
            insertCommand.Parameters.AddWithValue("@dia", Dia.Text);
            insertCommand.Parameters.AddWithValue("@megjegyzes", Megjegyzes.Text);
            insertCommand.ExecuteNonQuery();
        }
        UserWindow.DisplayPatients(UserWindow.PatientsDataTable);
        Connection.Close();
        Close();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnApplicationClosing(object? sender, CancelEventArgs e)
    {
        UserWindow.IsNewMeresOpen = false;
    }
}