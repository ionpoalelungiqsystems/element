﻿using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            CounterBtn.Text = count == 1 ? "Clicked 1 time" : $"Clicked {count} times";
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}