<? xml version = "1.0" encoding = "utf-8" ?>
< ContentPage xmlns = "http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns: x = "http://schemas.microsoft.com/winfx/2009/xaml"
             x: Class = "MauiApp1.MainPage"
             BackgroundColor = "#FFFFFF" >
    < ScrollView >
        < VerticalStackLayout Padding = "30" Spacing = "20" >
            < Label Text = "Home"
                   FontSize = "36"
                   FontAttributes = "Bold"
                   TextColor = "Black"
                   HorizontalOptions = "Center" />
            < Grid ColumnDefinitions = "Auto,*" RowDefinitions = "Auto" BackgroundColor = "#FFFFFF" Padding = "10" >
                < Border Grid.Column = "0"
                        StrokeShape = "RoundRectangle 10"
                        Stroke = "LightGray"
                        BackgroundColor = "White"
                        Padding = "5" >
                    < Grid ColumnDefinitions = "Auto,Auto" RowDefinitions = "Auto" >
                        < Label x: Name = "SelectedFlag" Grid.Column = "0" Margin = "5" WidthRequest = "30" HeightRequest = "20" />
                        < Button x: Name = "CountryButton" Text = "▼" FontSize = "Small" Clicked = "OnCountryButtonClicked" Grid.Column = "1" TextColor = "Black" BackgroundColor = "Transparent" />
                    </ Grid >
                </ Border >
                < Entry x: Name = "PhoneNumberEntry"
                       Grid.Column = "1"
                       Placeholder = "Your number"
                       Keyboard = "Telephone"
                       TextChanged = "OnPhoneNumberTextChanged"
                       TextColor = "Black"
                       BackgroundColor = "White" />
            </ Grid >
            < Border x: Name = "CountryListBorder"
                    IsVisible = "False"
                    Stroke = "LightGray"
                    StrokeShape = "RoundRectangle 12"
                    BackgroundColor = "#FFFFFF"
                    Padding = "10" >
                < VerticalStackLayout >
                    < Entry x: Name = "SearchEntry" Placeholder = "Search" TextChanged = "OnSearchTextChanged" BackgroundColor = "#FFFFFF" TextColor = "Black" />
                    < CollectionView x: Name = "CountryListView" ItemsSource = "{Binding countries}" SelectionMode = "Single" SelectionChanged = "OnCountrySelectionChanged" >
                        < CollectionView.ItemTemplate >
                            < DataTemplate >
                                < Grid ColumnDefinitions = "Auto,*,Auto" Padding = "10" >
                                    < Label Text = "{Binding Flag}" Grid.Column = "0" WidthRequest = "30" HeightRequest = "20" />
                                    < Label Text = "{Binding Name}" Grid.Column = "1" TextColor = "Black" FontSize = "16" />
                                    < Label Text = "{Binding CallingCode}" Grid.Column = "2" TextColor = "Gray" HorizontalOptions = "End" />
                                </ Grid >
                            </ DataTemplate >
                        </ CollectionView.ItemTemplate >
                    </ CollectionView >
                </ VerticalStackLayout >
            </ Border >
            < Label x: Name = "ValidationMessage"
                   TextColor = "Red"
                   FontAttributes = "Bold"
                   FontSize = "14"
                   IsVisible = "False" />
            < Button x: Name = "CounterBtn"
                    Text = "Click me"
                    BackgroundColor = "Purple"
                    TextColor = "White"
                    CornerRadius = "15"
                    HeightRequest = "50"
                    FontAttributes = "Bold"
                    Clicked = "OnCounterClicked"
                    HorizontalOptions = "Fill" />
        </ VerticalStackLayout >
    </ ScrollView >
</ ContentPage >



