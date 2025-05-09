﻿using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using CsharpBeadando1.UserControls;

namespace CsharpBeadando1.Windows;

public partial class NewMeres : Window
{
    public NewMeres()
    {
        InitializeComponent();
        Datum.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
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