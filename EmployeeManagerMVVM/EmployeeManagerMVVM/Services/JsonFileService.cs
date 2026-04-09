using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EmployeeManagerMVVM.Models;

namespace EmployeeManagerMVVM.Services
{
    public class JsonFileService
    {
        private readonly string _filePath;

        public JsonFileService(string filePath = "Data/employees.json")
        {
            _filePath = filePath;
        }

        public List<Employee> Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    // Создаём файл с тестовыми данными
                    var defaultEmployees = GetDefaultEmployees();
                    Save(defaultEmployees);
                    return defaultEmployees;
                }

                string json = File.ReadAllText(_filePath);
                var employees = JsonSerializer.Deserialize<List<Employee>>(json);
                return employees ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        public void Save(List<Employee> employees)
        {
            try
            {
                string directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения данных: {ex.Message}");
            }
        }

        private List<Employee> GetDefaultEmployees()
        {
            return new List<Employee>
            {
                new Employee { Id = 1, FullName = "Иванов Иван Иванович", Position = "Разработчик", Department = "IT", Age = 30 },
                new Employee { Id = 2, FullName = "Петрова Мария Сергеевна", Position = "Тестировщик", Department = "QA", Age = 25 },
                new Employee { Id = 3, FullName = "Сидоров Алексей Владимирович", Position = "Менеджер", Department = "Управление", Age = 40 }
            };
        }
    }
}