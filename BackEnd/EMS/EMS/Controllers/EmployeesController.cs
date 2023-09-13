using EMS.Context;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly AddDbContext _addDbContext;
        public EmployeesController(AddDbContext addDbContext)
        {
            _addDbContext = addDbContext;
        }
        [HttpGet]
        public  async Task <IActionResult> GetAllEmployees()
        {
            var employees = await _addDbContext.Employees.ToListAsync();

            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody]Employee employeeRequest)
        {
            employeeRequest.Id = Guid.NewGuid();

            await _addDbContext.Employees.AddAsync(employeeRequest);
            await _addDbContext.SaveChangesAsync();

            return Ok(employeeRequest);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetEmployee([FromRoute]Guid id)
        {
            var employee = await _addDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, Employee updateEmployeeRequest)
        {
           var employee = await _addDbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }
            employee.Name = updateEmployeeRequest.Name;
            employee.Email = updateEmployeeRequest.Email;
            employee.Phone = updateEmployeeRequest.Phone;
            employee.Address = updateEmployeeRequest.Address;
            employee.Degree = updateEmployeeRequest.Degree;
            employee.DOB = updateEmployeeRequest.DOB;
            employee.Specialization = updateEmployeeRequest.Specialization;
            employee.Skills = updateEmployeeRequest.Skills;
            employee.Designation = updateEmployeeRequest.Designation;
            employee.Salary = updateEmployeeRequest.Salary;

            await _addDbContext.SaveChangesAsync();
            return Ok(employee);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            var employee = await _addDbContext.Employees.FindAsync(id);

            if(employee == null)
            {
                return NotFound();
            }
            _addDbContext.Employees.Remove(employee);
            await _addDbContext.SaveChangesAsync();
            return Ok(employee);
        }
    }
}
