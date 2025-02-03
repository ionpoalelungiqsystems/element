using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using CountryData.Standard;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Country> Countries { get; set; }
        private Country selectedCountry;
        private int count = 0;

        public MainPage()
        {
            InitializeComponent();

            var countryHelper = new CountryHelper();
            var allCountries = countryHelper.GetCountries().ToList();

            Countries = new ObservableCollection<Country>(
                allCountries.Select(c =>
                {
                    var parts = c.Split('|');
                    if (parts.Length < 3) return null;
                    return new Country
                    {
                        Name = parts[0],
                        CallingCode = "+" + parts[1],
                        Alpha2 = parts[2]
                    };
                }).Where(c => c != null)!
            );

            if (Countries.Count == 0)
            {
                Countries = new ObservableCollection<Country>
{
    new Country { Name = "Moldova", CallingCode = "+373", Alpha2 = "MD" },
    new Country { Name = "United Kingdom", CallingCode = "+44", Alpha2 = "GB" },
    new Country { Name = "Germany", CallingCode = "+49", Alpha2 = "DE" },
    new Country { Name = "France", CallingCode = "+33", Alpha2 = "FR" },
    new Country { Name = "United States", CallingCode = "+1", Alpha2 = "US" },
    new Country { Name = "Canada", CallingCode = "+1", Alpha2 = "CA" },
    new Country { Name = "India", CallingCode = "+91", Alpha2 = "IN" },
    new Country { Name = "China", CallingCode = "+86", Alpha2 = "CN" },
    new Country { Name = "Japan", CallingCode = "+81", Alpha2 = "JP" },
    new Country { Name = "Brazil", CallingCode = "+55", Alpha2 = "BR" },
    new Country { Name = "Australia", CallingCode = "+61", Alpha2 = "AU" },
    new Country { Name = "Italy", CallingCode = "+39", Alpha2 = "IT" },
    new Country { Name = "Spain", CallingCode = "+34", Alpha2 = "ES" },
    new Country { Name = "Russia", CallingCode = "+7", Alpha2 = "RU" },
    new Country { Name = "South Africa", CallingCode = "+27", Alpha2 = "ZA" },
    new Country { Name = "Argentina", CallingCode = "+54", Alpha2 = "AR" },
    new Country { Name = "Mexico", CallingCode = "+52", Alpha2 = "MX" },
    new Country { Name = "Netherlands", CallingCode = "+31", Alpha2 = "NL" },
    new Country { Name = "Sweden", CallingCode = "+46", Alpha2 = "SE" },
    new Country { Name = "Switzerland", CallingCode = "+41", Alpha2 = "CH" },
    new Country { Name = "Austria", CallingCode = "+43", Alpha2 = "AT" },
    new Country { Name = "Belgium", CallingCode = "+32", Alpha2 = "BE" },
    new Country { Name = "Denmark", CallingCode = "+45", Alpha2 = "DK" },
    new Country { Name = "Finland", CallingCode = "+358", Alpha2 = "FI" },
    new Country { Name = "Norway", CallingCode = "+47", Alpha2 = "NO" },
    new Country { Name = "Poland", CallingCode = "+48", Alpha2 = "PL" },
    new Country { Name = "Turkey", CallingCode = "+90", Alpha2 = "TR" }
};
            }

            BindingContext = this;

            // Setăm Moldova ca țară implicită
            selectedCountry = Countries.FirstOrDefault(c => c.Alpha2 == "MD") ?? Countries.First();
            UpdateUI();
        }

        private void UpdateUI()
        {
            Console.WriteLine($"Selected Country: {selectedCountry.Name} - {selectedCountry.CallingCode}");
            SelectedFlag.Source = $"{selectedCountry.Alpha2.ToLower()}.png";
            PhoneNumberEntry.Text = $"{selectedCountry.CallingCode} ";
        }

        private void OnCountryButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Opening Country List");
            CountryListBorder.IsVisible = !CountryListBorder.IsVisible;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower() ?? "";
            CountryListView.ItemsSource = string.IsNullOrEmpty(searchText)
                ? Countries
                : Countries.Where(c => c.Name.ToLower().Contains(searchText)).ToList();
        }

        private void OnCountrySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Country country)
            {
                selectedCountry = country;
                UpdateUI();
                CountryListBorder.IsVisible = false;
                CountryListView.SelectedItem = null;
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

            bool isValid = IsPhoneNumberValid(phoneNumber);
            ValidationMessage.Text = isValid ? "Valid number!" : "Invalid phone number!";
            ValidationMessage.TextColor = isValid ? Colors.Green : Colors.Red;
            ValidationMessage.IsVisible = true;
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            string countryCallingCode = selectedCountry.CallingCode.TrimStart('+');
            Regex regex = new Regex(@"^\+?\d{6,15}$");
            return regex.IsMatch(phoneNumber);
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            CounterBtn.Text = $"Clicked {count} times";
        }
    }

    public class Country
    {
        public string Name { get; set; }
        public string CallingCode { get; set; }
        public string Alpha2 { get; set; }
    }
}
