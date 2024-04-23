﻿using Praksa_projectV1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Praksa_projectV1.Views
{
    /// <summary>
    /// Interaction logic for WorkersEditView.xaml
    /// </summary>
    public partial class WorkersEditView : Window
    {
        public WorkersEditView()
        {
            InitializeComponent();
            WorkersViewModel viewModel = new WorkersViewModel();
            DataContext = viewModel;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if the entered character is a digit or a decimal point
            e.Handled = !IsNumeric(e.Text) || (e.Text == "." && textBox.Text.Contains("."));
        }

        private bool IsNumeric(string text)
        {
            // Allow digits and one decimal point
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[0-9.]*$");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Validate the entire text for decimal format
            if (!IsValidDecimal(textBox.Text))
            {
                // Revert to the previous valid text
                textBox.Text = e.UndoAction == UndoAction.Undo ? e.OriginalSource.ToString() : string.Empty;
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private bool IsValidDecimal(string text)
        {
            // Allow empty string or valid decimal format
            return string.IsNullOrEmpty(text) || decimal.TryParse(text, out _);
        }
    }
}
