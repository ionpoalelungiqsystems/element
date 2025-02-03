using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions; // Add this line
using Microsoft.Maui.Controls;
using CountryData.Standard;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Country> countries = new();
        private Country selectedCountry;
        private int count = 0;

        public MainPage()
        {
            InitializeComponent();

            // Initialize country data using CountryData.Standard
            var countryHelper = new CountryHelper();
            var allCountries = countryHelper.GetCountries().ToList();

            // Parse strings into custom Country model
            countries = new ObservableCollection<Country>(
                allCountries.Select(c =>
                {
                    var parts = c.Split('|');
                    if (parts.Length < 3) // Skip invalid entries
                    {
                        Console.WriteLine($"Skipping invalid country data: {c}");
                        return null;
                    }

                    return new Country
                    {
                        Name = parts[0],
                        CallingCode = "+" + parts[1],
                        Alpha2 = parts[2]
                    };
                }).Where(c => c != null)! // Filter out null entries
            );

            // Ensure the collection is not empty
            if (countries.Count == 0)
            {
                Console.WriteLine("No valid countries were loaded. Using default data.");

                countries = new ObservableCollection<Country>
                {
                    new Country { Name = "United States", CallingCode = "+1", Alpha2 = "US" },
                    new Country { Name = "United Kingdom", CallingCode = "+44", Alpha2 = "GB" },
                    new Country { Name = "Canada", CallingCode = "+1", Alpha2 = "CA" },
                    new Country { Name = "Germany", CallingCode = "+49", Alpha2 = "DE" },
                    new Country { Name = "France", CallingCode = "+33", Alpha2 = "FR" },
                    new Country { Name = "India", CallingCode = "+91", Alpha2 = "IN" },
                    new Country { Name = "Japan", CallingCode = "+81", Alpha2 = "JP" }
                };
            }

            BindingContext = this;

            // Select default country (GB) or first country in the list
            selectedCountry = countries.FirstOrDefault(c => c.Alpha2 == "GB") ?? countries.First();

            // Update UI with selected country's flag and calling code
            SelectedFlag.Source = $"{selectedCountry.Alpha2.ToLower()}.png";
            PhoneNumberEntry.Text = $"{selectedCountry.CallingCode} ";
        }

        private void OnCountryButtonClicked(object sender, EventArgs e)
        {
            CountryListBorder.IsVisible = !CountryListBorder.IsVisible;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower() ?? "";
            CountryListView.ItemsSource = string.IsNullOrEmpty(searchText)
                ? countries
                : countries.Where(c => c.Name.ToLower().Contains(searchText)).ToList();
        }

        private void OnCountrySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Country country)
            {
                selectedCountry = country;
                SelectedFlag.Source = $"{selectedCountry.Alpha2.ToLower()}.png";
                PhoneNumberEntry.Text = $"{selectedCountry.CallingCode} ";
                CountryListBorder.IsVisible = false;
                CountryListView.SelectedItem = null; // Clear selection
            }
        }

        private void OnPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            string phoneNumber = e.NewTextValue ?? "";
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ValidationMessage.IsVisible = false;
                return;
            }

            Regex regex = new(@"^\+?[0-9]{6,15}$");
            if (!regex.IsMatch(phoneNumber))
            {
                ValidationMessage.Text = "Invalid phone number format.";
                ValidationMessage.TextColor = Colors.Red;
                ValidationMessage.IsVisible = true;
                return;
            }

            ValidationMessage.Text = "Phone number is valid!";
            ValidationMessage.TextColor = Colors.Green;
            ValidationMessage.IsVisible = true;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            CounterBtn.Text = count == 1 ? "Clicked 1 time" : $"Clicked {count} times";
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

    // Custom Country model
    public class Country
    {
        public required string Name { get; set; } // Required property for country name
        public required string CallingCode { get; set; } // Required property for calling code
        public required string Alpha2 { get; set; } // Required property for Alpha2 code
    }
}