using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EmployeeManagerMVVM.Commands;
using EmployeeManagerMVVM.Models;
using EmployeeManagerMVVM.Services;
using EmployeeManagerMVVM.Views;

namespace EmployeeManagerMVVM.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly JsonFileService _fileService;
        private ObservableCollection<Employee> _allEmployees;
        private ObservableCollection<Employee> _filteredEmployees;
        private string _searchText = "";
        private Employee? _selectedEmployee;

        public ObservableCollection<Employee> FilteredEmployees
        {
            get => _filteredEmployees;
            set { _filteredEmployees = value; OnPropertyChanged(nameof(FilteredEmployees)); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilter();
            }
        }

        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(nameof(SelectedEmployee)); }
        }

        // Команды
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveToFileCommand { get; }

        public MainViewModel()
        {
            _fileService = new JsonFileService();

            try
            {
                var employees = _fileService.Load();
                _allEmployees = new ObservableCollection<Employee>(employees);
                FilteredEmployees = new ObservableCollection<Employee>(_allEmployees);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _allEmployees = new ObservableCollection<Employee>();
                FilteredEmployees = new ObservableCollection<Employee>();
            }

            AddCommand = new RelayCommand(_ => OpenDetailWindow(null));
            EditCommand = new RelayCommand(_ => OpenDetailWindow(SelectedEmployee), _ => SelectedEmployee != null);
            DeleteCommand = new RelayCommand(_ => DeleteEmployee(), _ => SelectedEmployee != null);
            SaveToFileCommand = new RelayCommand(_ => SaveToFile());
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredEmployees = new ObservableCollection<Employee>(_allEmployees);
            }
            else
            {
                var filtered = _allEmployees.Where(e =>
                    e.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    e.Position.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    e.Department.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                FilteredEmployees = new ObservableCollection<Employee>(filtered);
            }
        }

        private void OpenDetailWindow(Employee? employee)
        {
            var detailViewModel = new DetailViewModel(employee, _allEmployees.ToList());
            var detailWindow = new DetailWindow
            {
                DataContext = detailViewModel,
                Owner = Application.Current.MainWindow
            };

            detailViewModel.OnSaveComplete += (sender, args) =>
            {
                _allEmployees.Clear();
                foreach (var emp in detailViewModel.EmployeesList)
                {
                    _allEmployees.Add(emp);
                }
                ApplyFilter();
                detailWindow.Close();
            };

            detailWindow.ShowDialog();
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            var result = MessageBox.Show($"Удалить сотрудника {SelectedEmployee.FullName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _allEmployees.Remove(SelectedEmployee);
                ApplyFilter();
            }
        }

        private void SaveToFile()
        {
            try
            {
                _fileService.Save(_allEmployees.ToList());
                MessageBox.Show("Данные успешно сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}