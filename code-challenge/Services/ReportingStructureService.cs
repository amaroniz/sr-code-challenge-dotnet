using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public ReportingStructureService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        public ReportingStructure GetById(string id)
        {
            var employee = _employeeRepository.GetWithDirectReportsById(id);

            int numberOfReports = 0;
            var newDirectReports = new List<Employee>();

            employee.DirectReports?.ForEach(reportingEmployee => 
            {
                // count this dependency
                numberOfReports++;
                // re-entrant to traverse the tree
                var reportingStructure = GetById(reportingEmployee.EmployeeId);
                // pick up sub structure of direct reports
                newDirectReports.Add(reportingStructure.Employee);
                // count dependencies of dependencies
                numberOfReports += reportingStructure.NumberOfReports;
            });

            employee.DirectReports = newDirectReports;

            return new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = numberOfReports
            };
        }
    }
}
