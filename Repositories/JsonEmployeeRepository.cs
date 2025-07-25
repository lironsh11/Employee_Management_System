using EmployeeManagementSystem.Models;
using Newtonsoft.Json;

namespace EmployeeManagementSystem.Repositories
{
    public class JsonEmployeeRepository : IEmployeeRepository
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _fileLock = new(1, 1);
        private readonly ILogger<JsonEmployeeRepository> _logger;

        public JsonEmployeeRepository(IWebHostEnvironment environment, ILogger<JsonEmployeeRepository> logger)
        {
            _logger = logger;
            var appDataPath = Path.Combine(environment.ContentRootPath, "App_Data");
            Directory.CreateDirectory(appDataPath);
            _filePath = Path.Combine(appDataPath, "employees.json");
        }

        private async Task<List<Employee>> LoadEmployeesAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Employee>();

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<List<Employee>>(json) ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employees from file");
                return new List<Employee>();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private async Task SaveEmployeesAsync(List<Employee> employees)
        {
            await _fileLock.WaitAsync();
            try
            {
                var json = JsonConvert.SerializeObject(employees, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving employees to file");
                throw;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<PagedResult<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy = "Id", bool descending = false, string searchTerm = "")
        {
            var employees = await LoadEmployeesAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                employees = employees.Where(e =>
                    e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Apply sorting
            employees = sortBy.ToLower() switch
            {
                "firstname" => descending ? employees.OrderByDescending(e => e.FirstName).ToList() : employees.OrderBy(e => e.FirstName).ToList(),
                "lastname" => descending ? employees.OrderByDescending(e => e.LastName).ToList() : employees.OrderBy(e => e.LastName).ToList(),
                "email" => descending ? employees.OrderByDescending(e => e.Email).ToList() : employees.OrderBy(e => e.Email).ToList(),
                "hiredate" => descending ? employees.OrderByDescending(e => e.HireDate).ToList() : employees.OrderBy(e => e.HireDate).ToList(),
                "salary" => descending ? employees.OrderByDescending(e => e.Salary).ToList() : employees.OrderBy(e => e.Salary).ToList(),
                _ => descending ? employees.OrderByDescending(e => e.Id).ToList() : employees.OrderBy(e => e.Id).ToList()
            };

            var totalCount = employees.Count;
            var pagedEmployees = employees.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<Employee>
            {
                Items = pagedEmployees,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            var employees = await LoadEmployeesAsync();
            return employees.FirstOrDefault(e => e.Id == id);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            var employees = await LoadEmployeesAsync();
            employee.Id = employees.Any() ? employees.Max(e => e.Id) + 1 : 1;
            employees.Add(employee);
            await SaveEmployeesAsync(employees);
            return employee;
        }

        public async Task<Employee?> UpdateEmployeeAsync(Employee employee)
        {
            var employees = await LoadEmployeesAsync();
            var existingEmployee = employees.FirstOrDefault(e => e.Id == employee.Id);

            if (existingEmployee == null)
                return null;

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.HireDate = employee.HireDate;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.DepartmentId = employee.DepartmentId;

            await SaveEmployeesAsync(employees);
            return existingEmployee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employees = await LoadEmployeesAsync();
            var employee = employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return false;

            employees.Remove(employee);
            await SaveEmployeesAsync(employees);
            return true;
        }

        public async Task<List<Employee>> GetRecentHiresAsync(int days = 30)
        {
            var employees = await LoadEmployeesAsync();
            var cutoffDate = DateTime.Now.AddDays(-days);
            return employees.Where(e => e.HireDate >= cutoffDate).OrderByDescending(e => e.HireDate).ToList();
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            var employees = await LoadEmployeesAsync();
            return employees.Count;
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            var employees = await LoadEmployeesAsync();
            return employees.Where(e => e.DepartmentId == departmentId).ToList();
        }
    }
}