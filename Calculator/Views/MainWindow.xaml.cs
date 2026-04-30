using System;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Calculator.Views
{
    public partial class MainWindow : Window
    {
        #region Fields
        private double _firstNumber = 0;
        private string _currentOperator = string.Empty;
        private bool _isNewNumber = true;
        private readonly ObservableCollection<string> _history = [];
        private HistoryWindow _historyWindow;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Clicks
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var value = (sender as Button)?.Content.ToString();

            if (_isNewNumber)
            {
                Display.Text = value == "." ? "0." : value;
                _isNewNumber = false;
                return;
            }

            if (value == "." && Display.Text.Contains("."))
                return;

            Display.Text += value;
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            _firstNumber = double.Parse(Display.Text, CultureInfo.InvariantCulture);
            _currentOperator = (sender as Button)?.Content.ToString();
            _isNewNumber = true;
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(_currentOperator))
                return;

            try
            {
                double secondNumber = double.Parse(Display.Text, CultureInfo.InvariantCulture);
                double result = Calculate(_firstNumber, secondNumber, _currentOperator);

                Display.Text = result.ToString("0.########", CultureInfo.InvariantCulture);
                _isNewNumber = true;
                AddToHistory(_firstNumber, secondNumber, _currentOperator, result); 

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Display.Text = "0";
                _isNewNumber = true;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
            _firstNumber = 0;
            _currentOperator = string.Empty;
            _isNewNumber = true;
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (_historyWindow == null || !_historyWindow.IsVisible)
            {
                _historyWindow = new HistoryWindow(_history)
                {
                    Owner = this
                };
                _historyWindow.Show();
            }
            else
            {
                _historyWindow.Activate();
            }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewNumber)
                return;

            if (Display.Text.Length > 1)
            {
                Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
            }
            else
            {
                Display.Text = "0";
                _isNewNumber = true;
            }
        }
        #endregion

        #region Methods
        private double Calculate(double a, double b, string op)
        {
            return op switch
            {
                "+" => a + b,
                "-" => a - b,
                "x" => a * b,
                "/" when b != 0 => a / b,
                "/" => throw new Exception("Divisão por zero não é permitida!"),
                _ => throw new Exception("Operador desconhecido!")
            };
        }
        private void AddToHistory(double a, double b, string op, double result)
        {

            _history.Add(
                $"{a.ToString("0.########", CultureInfo.InvariantCulture)} " +
                $"{op} " +
                $"{b.ToString("0.########", CultureInfo.InvariantCulture)} = " +
                $"{result.ToString("0.########", CultureInfo.InvariantCulture)}"
            );
        }
        #endregion

    }

}
