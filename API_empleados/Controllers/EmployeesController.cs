using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API_empleados.Models;
using API_empleados.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API_empleados.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : Controller
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> Get() =>
        await _employeeService.GetEmployees();

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public async Task<ActionResult<Employee>> Get(string id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return employee;
        }

        [HttpGet("search", Name = "SearchEmployees")]
        public async Task<object> GetEmployeesBySearch(

        [FromQuery] string? nombre,
        [FromQuery] string? cargo,
        [FromQuery] string? departamento,
        [FromQuery] string? fechaInicial,
        [FromQuery] string? fechaFinal,
        [FromQuery] int pagina,
        [FromQuery] int porPagina
        )
        {
            return await _employeeService.GetEmployeesBySearch(nombre, cargo, departamento, fechaInicial, fechaFinal, pagina, porPagina);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            var createEmployee = await _employeeService.CreateEmployee(employee);

            return CreatedAtRoute("GetProduct", new { id = createEmployee.Id }, createEmployee);
        }

    }
}