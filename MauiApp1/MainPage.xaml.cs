using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
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
            // Populate the countries list
            countries.Add(new Country { Name = "Romania", Code = "+40", Flag = "ro.png" });
            countries.Add(new Country { Name = "United States", Code = "+1", Flag = "us.png" });
            countries.Add(new Country { Name = "United Kingdom", Code = "+44", Flag = "uk.png" });
            countries.Add(new Country { Name = "Germany", Code = "+49", Flag = "de.png" });
            countries.Add(new Country { Name = "France", Code = "+33", Flag = "fr.png" });

            BindingContext = this;
            selectedCountry = countries[0];
            SelectedFlag.Source = selectedCountry.Flag;
            PhoneNumberEntry.Placeholder = $"Your number";
            PhoneNumberEntry.Text = $"{selectedCountry.Code} ";
        }

        // Event handler for when the country button is clicked
        private void OnCountryButtonClicked(object sender, EventArgs e)
        {
            CountryListFrame.IsVisible = !CountryListFrame.IsVisible;
        }

        // Event handler for when the search entry text changes
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower() ?? "";
            if (string.IsNullOrEmpty(searchText))
            {
                CountryListView.ItemsSource = countries;
            }
            else
            {
                CountryListView.ItemsSource = countries.Where(c => c.Name.ToLower().Contains(searchText));
            }
        }

        // Event handler for when a country is selected
        private void OnCountrySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Country country)
            {
                selectedCountry = country;
                SelectedFlag.Source = selectedCountry.Flag;
                PhoneNumberEntry.Placeholder = $"Your number";
                PhoneNumberEntry.Text = $"{selectedCountry.Code} ";
                CountryListFrame.IsVisible = false;
            }
        }

        // Event handler for validating the phone number as the user types
        private void OnPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            string phoneNumber = e.NewTextValue ?? "";
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ValidationMessage.IsVisible = false;
                return;
            }

            // Validate the phone number format using a regular expression
            Regex regex = new(@"^\+?[0-9]{6,15}$");
            if (!regex.IsMatch(phoneNumber))
            {
                ValidationMessage.Text = "Invalid phone number format.";
                ValidationMessage.TextColor = Colors.Red;
                ValidationMessage.IsVisible = true;
                return;
            }

            // If validation passes
            ValidationMessage.Text = "Phone number is valid!";
            ValidationMessage.TextColor = Colors.Green;
            ValidationMessage.IsVisible = true;
        }

        // Event handler for the counter button
        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";
            // Announce the updated text for accessibility
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

    public class Country
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Flag { get; set; }
    }
}