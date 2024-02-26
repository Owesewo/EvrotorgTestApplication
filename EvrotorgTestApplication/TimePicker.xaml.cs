using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace EvrotorgTestApplication
{
    /// <summary>
    /// Логика взаимодействия для TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
        }

        private void textBoxHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxHours.Text = validateHours(textBoxHours.Text);
        }

        private void textBoxMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxMinutes.Text = validateMinutesOrSeconds(textBoxMinutes.Text);
        }

        private void textBoxSeconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxSeconds.Text = validateMinutesOrSeconds(textBoxSeconds.Text);
        }

        private string validateHours(string strHours) =>
            Regex.IsMatch(strHours, @"(?:[01]?[0-9]|2[0-3])")
            ? strHours
            : "00";

        private string validateMinutesOrSeconds(string strMinutesOrSeconds) =>
            !Regex.IsMatch(strMinutesOrSeconds, @"(?:[0-5][0-9])")
            ? strMinutesOrSeconds
            : "00";

        public string GetTime() =>
            $"{validateHours(textBoxHours.Text)}:" +
            $"{validateMinutesOrSeconds(textBoxMinutes.Text)}:" +
            $"{textBoxSeconds.Text}";


    }
}
