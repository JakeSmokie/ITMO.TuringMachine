using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace Turing_Machine
{
    public partial class MainWindow
    {
        private readonly Dictionary<char, int> _symToIndex = new Dictionary<char, int>
        {
            ['S'] = 0,
            ['0'] = 1,
            ['1'] = 2,
            ['2'] = 3,
        };

        private readonly TextBox[,] _ruleBoxes = new TextBox[4, 4];

        private int _state = 1;
        private string _tape;
        private int _step = 1;

        public MainWindow()
        {
            InitializeComponent();

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    var item = new TextBox
                    {
                        Background = Brushes.AliceBlue,
                    };

                    Grid.SetColumn(item, i + 1);
                    Grid.SetRow(item, j + 1);

                    _ruleBoxes[i, j] = item;
                    Matrix.Children.Add(item);
                }
            }
        }

        private bool DoStep()
        {
            if (!int.TryParse(Iterations.Text, out var iterationsAmount))
            {
                Iterations.Text = "1000";
                iterationsAmount = 1000;
                MessageBox.Show("Iterations amount set to 1000.");
            }

            if (_step > iterationsAmount)
                return false;

            if (_step == 1)
            {
                _tape = Word.Text;
                History.Items.Add($"0. {_tape}");
            }

            var head = _tape.IndexOf('>');

            if (head == -1)
            {
                MessageBox.Show("Enter head symbol: '>'");
                return false;
            }

            if (_state == 0)
                return false;

            if (head + 1 >= _tape.Length)
                _tape = _tape.Insert(_tape.Length, "S");

            var symbol = _tape[head + 1];
            var command = _ruleBoxes[_symToIndex[symbol], _state - 1].Text.Split(' ');

            var prevState = _state;
            switch (command[0])
            {
                case "q0":
                    _state = 0;
                    break;
                case "q1":
                    _state = 1;
                    break;
                case "q2":
                    _state = 2;
                    break;
                case "q3":
                    _state = 3;
                    break;
                case "q4":
                    _state = 4;
                    break;
                default:
                    MessageBox.Show("Wrong state symbol");
                    return false;
            }

            var parts = _tape.Split('>');
            parts[1] = parts[1].Remove(0, 1).Insert(0, command[1]);

            switch (command[2])
            {
                case "L":
                    if (parts[0].Length - 1 < 0)
                        parts[0] = "S" + parts[0];

                    parts[0] = parts[0].Insert(parts[0].Length - 1, ">");
                    break;
                case "R":
                    parts[1] = parts[1].Insert(1, ">");
                    break;
                case "C":
                    parts[1] = parts[1].Insert(0, ">");
                    break;
                default:
                    MessageBox.Show("Wrong move symbol");
                    return false;
            }

            _tape = parts[0] + parts[1];

            if (ReverseOut.IsChecked == false)
                History.Items.Insert(0,
                    $"{_step}. '{_tape}' ({symbol}, q{prevState})({_ruleBoxes[_symToIndex[symbol], prevState - 1].Text})");
            else
                History.Items.Add(
                    $"{_step}. '{_tape}' ({symbol}, q{prevState})({_ruleBoxes[_symToIndex[symbol], prevState - 1].Text})");

            _step += 1;
            return true;
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            History.Items.Clear();

            _step = 1;
            _state = 1;
        }

        private void Step(object sender, RoutedEventArgs e)
        {
            DoStep();
        }

        private void Finish(object sender, RoutedEventArgs e)
        {
            while (DoStep())
            {
            }
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    _ruleBoxes[i, j].Text = "";
                }
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = ".txt"
            };

            if (dialog.ShowDialog() == false)
                return;

            using (var reader = new StreamReader(dialog.OpenFile()))
            {
                for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    _ruleBoxes[i, j].Text = reader.ReadLine() ?? throw new InvalidOperationException();

                Word.Text = reader.ReadLine() ?? throw new InvalidOperationException();
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".txt"
            };

            if (dialog.ShowDialog() == false)
                return;

            using (var writer = new StreamWriter(dialog.OpenFile()))
            {
                for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    writer.WriteLine(_ruleBoxes[i, j].Text);

                writer.WriteLine(Word.Text);
            }
        }
    }
}