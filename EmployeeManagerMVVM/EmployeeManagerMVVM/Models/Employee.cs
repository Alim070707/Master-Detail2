using System.ComponentModel;

namespace EmployeeManagerMVVM.Models
{
    public class Employee : INotifyPropertyChanged, IDataErrorInfo
    {
        private int _id;
        private string _fullName = "";
        private string _position = "";
        private string _department = "";
        private int _age;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(nameof(Position)); }
        }

        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(nameof(Department)); }
        }

        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(nameof(Age)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Валидация
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(FullName):
                        if (string.IsNullOrWhiteSpace(FullName))
                            error = "ФИО обязательно для заполнения";
                        break;
                    case nameof(Position):
                        if (string.IsNullOrWhiteSpace(Position))
                            error = "Должность обязательна для заполнения";
                        break;
                    case nameof(Department):
                        if (string.IsNullOrWhiteSpace(Department))
                            error = "Отдел обязателен для заполнения";
                        break;
                    case nameof(Age):
                        if (Age < 18 || Age > 100)
                            error = "Возраст должен быть от 18 до 100 лет";
                        break;
                }
                return error;
            }
        }
    }
}