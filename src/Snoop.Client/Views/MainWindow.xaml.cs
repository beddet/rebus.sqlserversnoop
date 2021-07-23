using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Serilog;
using Snoop.Client.ViewModels;

namespace Snoop.Client.Views
{
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _viewModel;
        private const string FilePath = "sqlsnoopusersettings.txt";

        public MainWindow()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                //.MinimumLevel.Error()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

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
            if(!File.Exists(FilePath)) return new List<string>();

            return File.ReadAllLines(FilePath).ToList();
        }

        private void SaveSettings()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            using (var file = File.Create(FilePath))
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
