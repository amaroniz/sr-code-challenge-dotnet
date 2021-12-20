using challenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Data
{
    public class EmployeeDataSeeder
    {
        private EmployeeContext _employeeContext;
        private const String EMPLOYEE_SEED_DATA_FILE = "resources/EmployeeSeedData.json";
        private const String COMPENSATION_SEED_DATA_FILE = "resources/CompensationSeedData.json";

        public EmployeeDataSeeder(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public async Task Seed()
        {
            if(!_employeeContext.Employees.Any())
            {
                List<Employee> employees = LoadEmployees();
                _employeeContext.Employees.AddRange(employees);

                await _employeeContext.SaveChangesAsync();
            }

            // Compensations reference Employees, do intialization after Employees.
            if (!_employeeContext.Compensations.Any())
            {
                List<Compensation> compensations = LoadCompensations(_employeeContext.Employees.ToList());
                _employeeContext.Compensations.AddRange(compensations);

                await _employeeContext.SaveChangesAsync();
            }
        }

        private List<Employee> LoadEmployees()
        {
            var employees = LoadModelsFromFile<Employee>(EMPLOYEE_SEED_DATA_FILE);
            FixUpReferences(employees);
            return employees;
        }

        private void FixUpReferences(List<Employee> employees)
        {
            var employeeIdRefMap = from employee in employees
                                select new { Id = employee.EmployeeId, EmployeeRef = employee };

            employees.ForEach(employee =>
            {
                
                if (employee.DirectReports != null)
                {
                    var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
                    employee.DirectReports.ForEach(report =>
                    {
                        var referencedEmployee = employeeIdRefMap.First(e => e.Id == report.EmployeeId).EmployeeRef;
                        referencedEmployees.Add(referencedEmployee);
                    });
                    employee.DirectReports = referencedEmployees;
                }
            });
        }

        private List<Compensation> LoadCompensations(List<Employee> employees)
        {
            var compensations = LoadModelsFromFile<Compensation>(COMPENSATION_SEED_DATA_FILE);
            FixUpReferences(compensations, employees);
            return compensations;
        }

        private void FixUpReferences(List<Compensation> compensations, List<Employee> employees)
        {
            var employeeIdRefMap = from employee in employees
                                   select new { Id = employee.EmployeeId, EmployeeRef = employee };

            compensations.ForEach(compensation =>
            {
                if (compensation.Employee != null)
                {
                    var referencedEmployee = employeeIdRefMap.First(e => e.Id == compensation.Employee.EmployeeId).EmployeeRef;
                    compensation.Employee = referencedEmployee;
                }
            });
        }

        private List<T> LoadModelsFromFile<T>(String seedDataFile)
        {
            using (FileStream fs = new FileStream(seedDataFile, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<T> models = serializer.Deserialize<List<T>>(jr);

                return models;
            }
        }
    }
}
