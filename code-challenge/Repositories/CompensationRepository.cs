using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<CompensationRepository> _logger;

        public CompensationRepository(ILogger<CompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            var employeeId = compensation.Employee?.EmployeeId ?? null;

            if (employeeId == null)
                return null;

            var employeeRef = _employeeContext.Employees.Find(employeeId);

            if (employeeRef == null)
                return null;

            compensation.CompensationId = Guid.NewGuid().ToString();
            compensation.Employee = employeeRef;

            _employeeContext.Compensations.Add(compensation);

            return compensation;
        }

        public Compensation GetById(string id)
        {
            return _employeeContext.Compensations.SingleOrDefault(e => e.Employee.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
