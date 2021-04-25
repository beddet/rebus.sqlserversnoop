using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Snoop.Client
{
    public partial class MainWindow
    {
        private MainWindowViewModel _viewModel;
        private const string filePath = "sqlsnoopusersettings.txt";
        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel = new MainWindowViewModel();

            var settings = ReadSettings();
            if(settings.Any()) _viewModel.Connections.Clear();

            settings.ForEach(x => _viewModel.Connections.Add(new ConnectionViewModel(){ConnectionString = x}));

            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        private List<string> ReadSettings()
        {
            if(!File.Exists(filePath)) return new List<string>();

            return File.ReadAllLines(filePath).ToList();
        }

        private void SaveSettings()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var file = File.Create(filePath))
            using (var writer = new StreamWriter(file))
            {
                foreach (var connection in _viewModel.Connections)
                {
                    writer.WriteLine(connection.ConnectionString);
                }

                writer.Flush();
            }
        }

        private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox {Visibility: Visibility.Visible} textBox)
                textBox.Focus();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.OnLoadedCommand.Execute();
        }
    }
}
