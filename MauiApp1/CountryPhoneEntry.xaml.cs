using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using CountryData.Standard;

namespace MauiApp1
{
    public partial class CountryPhoneEntry : ContentView
    {
        private ObservableCollection<Country> _allCountries = new();
        private ObservableCollection<Country> _filteredCountries = new();
        private Country? _selectedCountry;
        private readonly CountryHelper _countryHelper;
        private bool _isInitialized;

        public static readonly BindableProperty SelectedCountryProperty =
            BindableProperty.Create(nameof(SelectedCountry), typeof(Country), typeof(CountryPhoneEntry), null, BindingMode.TwoWay);

        public Country SelectedCountry
        {
            get => (Country?)GetValue(SelectedCountryProperty);
            set
            {
                SetValue(SelectedCountryProperty, value);
                _selectedCountry = value;
                UpdateUI();
            }
        }

        public static readonly BindableProperty PhoneNumberProperty =
            BindableProperty.Create(nameof(PhoneNumber), typeof(string), typeof(CountryPhoneEntry), string.Empty, BindingMode.TwoWay);

        public string PhoneNumber
        {
            get => (string)GetValue(PhoneNumberProperty);
            set => SetValue(PhoneNumberProperty, value);
        }

        public static readonly BindableProperty IsPopupVisibleProperty =
            BindableProperty.Create(nameof(IsPopupVisible), typeof(bool), typeof(CountryPhoneEntry), false, BindingMode.TwoWay);

        public bool IsPopupVisible
        {
            get => (bool)GetValue(IsPopupVisibleProperty);
            set => SetValue(IsPopupVisibleProperty, value);
        }

        public ObservableCollection<Country> FilteredCountries
        {
            get => _filteredCountries;
            set
            {
                _filteredCountries = value;
                OnPropertyChanged(nameof(FilteredCountries));
            }
        }

        public CountryPhoneEntry()
        {
            InitializeComponent();
            _countryHelper = new CountryHelper();
            BindingContext = this;

            // Handle layout changes after the view is loaded
            Loaded += OnViewLoaded;
        }

        private void OnViewLoaded(object sender, EventArgs e)
        {
            if (!_isInitialized)
            {
                InitializeCountries();
                _isInitialized = true;
            }
        }

        private void InitializeCountries()
        {
            var countryData = _countryHelper.GetCountryData()
                .OrderBy(c => c.CountryName)
                .ToList();

            _allCountries = new ObservableCollection<Country>(countryData.Select(c => new Country
            {
                Name = c.CountryName,
                CallingCode = "+" + c.PhoneCode.TrimStart('+'),
                Alpha2 = c.CountryShortCode.ToLower(),
                FlagUrl = $"https://flagcdn.com/h20/{c.CountryShortCode.ToLower()}.png",
                SearchText = $"{c.CountryName.ToLower()} {c.PhoneCode}"
            }));

            FilteredCountries = new ObservableCollection<Country>(_allCountries);
            SelectedCountry = _allCountries.FirstOrDefault(c => c.Alpha2 == "us") ?? _allCountries.First();
            UpdateUI();
        }

        private void OnCountryButtonClicked(object sender, EventArgs e)
        {
            IsPopupVisible = !IsPopupVisible;
            if (IsPopupVisible)
            {
                SearchEntry.Text = string.Empty;
                FilteredCountries = new ObservableCollection<Country>(_allCountries);
                SearchEntry.Focus();

                // Position the dropdown frame
                PositionDropdown();
            }
        }

        private void PositionDropdown()
        {
            if (MainInputFrame != null && CountryDropdownFrame != null && DropdownContent != null)
            {
                // Set the dropdown width to match the main input frame
                CountryDropdownFrame.WidthRequest = MainInputFrame.Width;

                // Position the dropdown directly under the main input
                var dropdownMargin = CountryDropdownFrame.Margin;
                dropdownMargin.Top = MainInputFrame.Height + 5; // 5px spacing
                CountryDropdownFrame.Margin = dropdownMargin;
            }
        }

        private void OnPopupBackgroundTapped(object sender, EventArgs e)
        {
            IsPopupVisible = false;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower().Trim() ?? "";

            var filtered = string.IsNullOrEmpty(searchText)
                ? _allCountries
                : _allCountries.Where(c => c.SearchText.Contains(searchText));

            FilteredCountries = new ObservableCollection<Country>(filtered);
        }

        private void OnCountrySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Country country)
            {
                SelectCountry(country);
            }
        }

        private void SelectCountry(Country country)
        {
            _selectedCountry = country;
            UpdateUI();
            IsPopupVisible = false;
            CountryListView.SelectedItem = null;

            // Preserve existing number if any, but update country code
            string existingNumber = PhoneNumberEntry.Text ?? "";
            string digitsOnly = Regex.Replace(existingNumber, @"[^\d]", "");
            if (digitsOnly.Length > 0)
            {
                PhoneNumberEntry.Text = _selectedCountry.CallingCode + " " + digitsOnly;
            }
            else
            {
                PhoneNumberEntry.Text = _selectedCountry.CallingCode + " ";
            }

            PhoneNumberEntry.CursorPosition = PhoneNumberEntry.Text.Length;
            ValidatePhoneNumber(PhoneNumberEntry.Text);
        }

        private void UpdateUI()
        {
            if (_selectedCountry == null) return;

            SelectedFlagImage.Source = _selectedCountry.FlagUrl;

            if (PhoneNumberEntry.Text == null || !PhoneNumberEntry.Text.StartsWith(_selectedCountry.CallingCode))
            {
                PhoneNumberEntry.Text = _selectedCountry.CallingCode + " ";
                PhoneNumberEntry.CursorPosition = PhoneNumberEntry.Text.Length;
            }
        }

        private void OnPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedCountry == null) return;

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                PhoneNumberEntry.Text = _selectedCountry.CallingCode + " ";
                PhoneNumberEntry.CursorPosition = PhoneNumberEntry.Text.Length;
                return;
            }

            string newText = e.NewTextValue.TrimStart();
            int cursorPosition = PhoneNumberEntry.CursorPosition;

            // If someone tries to modify the country code, prevent it
            if (!newText.StartsWith(_selectedCountry.CallingCode))
            {
                string remainingText = newText.TrimStart('+').Trim();
                // Remove any non-numeric characters
                remainingText = Regex.Replace(remainingText, @"[^\d]", "");

                PhoneNumberEntry.Text = _selectedCountry.CallingCode + " " + remainingText;
                PhoneNumberEntry.CursorPosition = cursorPosition;
                return;
            }

            // Get the part after country code
            string numberPart = newText.Substring(_selectedCountry.CallingCode.Length).Trim();

            // Remove any non-numeric characters from the input
            string cleanNumberPart = Regex.Replace(numberPart, @"[^\d]", "");

            // Format the number with proper spacing
            string formattedNumber = FormatPhoneNumber(cleanNumberPart);

            // Combine country code with formatted number
            PhoneNumberEntry.Text = _selectedCountry.CallingCode + " " + formattedNumber;

            // Try to maintain cursor position while accounting for formatting
            int newPosition = Math.Min(cursorPosition, PhoneNumberEntry.Text.Length);
            PhoneNumberEntry.CursorPosition = newPosition;

            ValidatePhoneNumber(PhoneNumberEntry.Text);
        }

        private string FormatPhoneNumber(string digits)
        {
            // Add spaces every 3 digits for better readability
            var parts = new List<string>();
            for (int i = 0; i < digits.Length; i += 3)
            {
                parts.Add(digits.Substring(i, Math.Min(3, digits.Length - i)));
            }
            return string.Join(" ", parts);
        }

        private void ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || _selectedCountry == null)
            {
                return;
            }

            string digitsOnly = Regex.Replace(
                phoneNumber.Substring(_selectedCountry.CallingCode.Length),
                @"[^\d]",
                ""
            );

            // Check if there are any non-numeric characters in the input (excluding spaces)
            string nonSpaceChars = Regex.Replace(
                phoneNumber.Substring(_selectedCountry.CallingCode.Length),
                @"[\d\s]",
                ""
            );

            if (nonSpaceChars.Length > 0)
            {
                // Handle validation error
                return;
            }

            if (digitsOnly.Length < 6)
            {
                // Handle validation error
                return;
            }

            if (digitsOnly.Length > 15)
            {
                // Handle validation error
                return;
            }

            // Valid phone number
        }
    }

    public class Country
    {
        public required string Name { get; set; }
        public required string CallingCode { get; set; }
        public required string Alpha2 { get; set; }
        public required string FlagUrl { get; set; }
        public required string SearchText { get; set; }
    }
}