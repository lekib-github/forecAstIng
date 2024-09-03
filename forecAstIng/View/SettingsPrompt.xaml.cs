using CommunityToolkit.Maui.Views;
using forecAstIng.Services;

namespace forecAstIng.View
{
    public class SettingsConverter : IMultiValueConverter
    {
        private AppTheme GetThemeFromString(string? theme)
        {
            return theme switch
            {
                null => App.Current.UserAppTheme,
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                "System theme" => AppTheme.Unspecified
            };
        }
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // null tests in each converter results in current value = no change, because null is the default in combo box
            string unit = values[1] is null ? DataService.UNIT : ((string)values[1]).ToLower();
            int interval = values[2] is null ? TimeSeriesData.DAYS_OF_HISTORY : int.Parse(((string)values[2])[0].ToString());

            return (GetThemeFromString((string)values[0]), unit, interval);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class SettingsPrompt : Popup
    {
        public SettingsPrompt()
        {
            InitializeComponent();
        }
    }
}
