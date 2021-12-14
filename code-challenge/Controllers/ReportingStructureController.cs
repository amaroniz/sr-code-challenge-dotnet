using System;
using Microsoft.AspNetCore.Mvc;

namespace challenge.Controllers
{
    [Route("api/reporting-structure")]
    public class ReportingStructureController
    {
        [HttpGet("{id}", Name = "getReportingStructureByEmployeeId")]
        public IActionResult GetReportingStructureByEmployeeId(String employeeId)
        {
            return null;
        }
    }
}
