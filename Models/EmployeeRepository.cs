using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeApi
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _appDBContext;
        public EmployeeRepository(AppDbContext appDbContext)
        {
            this._appDBContext = appDbContext;
        }
        public async Task<Employee> AddEmployee(Employee employee)
        {
            if(employee.Department != null)
            {
                _appDBContext.Entry(employee.Department).State = EntityState.Unchanged;
            }
            var result = await _appDBContext.Employees.AddAsync(employee);
            await _appDBContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteEmployee(int employeeId)
        {
            var result = await _appDBContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if(result !=null)
            {
                _appDBContext.Employees.Remove(result);
                await _appDBContext.SaveChangesAsync();
            }

        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            return await _appDBContext.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            return await _appDBContext.Employees
                 .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await _appDBContext.Employees.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> Search(string name, Gender? gender)
        {
            IQueryable<Employee> query = _appDBContext.Employees;

            if(!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName.Contains(name)
                        || e.LastName.Contains(name));
            }

            if (gender != null)
            {
                query = query.Where(e => e.Gender == gender);
            }

            return await query.ToListAsync();
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            var result = await _appDBContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

            if(result != null)
            {
                result.FirstName = employee.FirstName;
                result.LastName = employee.LastName;
                result.Email = employee.Email;
                result.DateOfBirth = employee.DateOfBirth;
                result.Gender = employee.Gender;
                if(employee.DepartmentId != 0)
                {
                    result.DepartmentId = employee.DepartmentId;
                }
                else if(employee.Department != null)
                {
                    result.DepartmentId = employee.Department.DepartmentId;
                }

                await _appDBContext.SaveChangesAsync();
                return result;
            }

            return null;
        }
    }
}
