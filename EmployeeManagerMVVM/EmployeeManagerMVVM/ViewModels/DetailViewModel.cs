using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using EmployeeManagerMVVM.Commands;
using EmployeeManagerMVVM.Models;

namespace EmployeeManagerMVVM.ViewModels
{
    public class DetailViewModel : INotifyPropertyChanged
    {
        private Employee _currentEmployee;
        private string _errorMessage = "";
        private bool _isEditMode;

        public ObservableCollection<Employee> EmployeesList { get; set; }

        public Employee CurrentEmployee
        {
            get => _currentEmployee;
            set { _currentEmployee = value; OnPropertyChanged(nameof(CurrentEmployee)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnSaveComplete;

        public DetailViewModel(Employee? employee, System.Collections.Generic.List<Employee> employeesList)
        {
            EmployeesList = new ObservableCollection<Employee>(employeesList);
            _isEditMode = employee != null;

            if (_isEditMode && employee != null)
            {
                CurrentEmployee = new Employee
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    Position = employee.Position,
                    Department = employee.Department,
                    Age = employee.Age
                };
            }
            else
            {
                int newId = EmployeesList.Count > 0 ? EmployeesList.Max(e => e.Id) + 1 : 1;
                CurrentEmployee = new Employee
                {
                    Id = newId,
                    FullName = "",
                    Position = "",
                    Department = "",
                    Age = 18
                };
            }

            SaveCommand = new RelayCommand(_ => Save(), _ => IsValid());
            CancelCommand = new RelayCommand(_ => OnSaveComplete?.Invoke(this, EventArgs.Empty));
        }

        private bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(CurrentEmployee.FullName) &&
                   !string.IsNullOrWhiteSpace(CurrentEmployee.Position) &&
                   !string.IsNullOrWhiteSpace(CurrentEmployee.Department) &&
                   CurrentEmployee.Age >= 18 && CurrentEmployee.Age <= 100;
        }

        private void Save()
        {
            if (!IsValid())
            {
                ErrorMessage = "Пожалуйста, заполните все поля корректно";
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    var existing = EmployeesList.FirstOrDefault(e => e.Id == CurrentEmployee.Id);
                    if (existing != null)
                    {
                        existing.FullName = CurrentEmployee.FullName;
                        existing.Position = CurrentEmployee.Position;
                        existing.Department = CurrentEmployee.Department;
                        existing.Age = CurrentEmployee.Age;
                    }
                }
                else
                {
                    EmployeesList.Add(CurrentEmployee);
                }

                OnSaveComplete?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка сохранения: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}