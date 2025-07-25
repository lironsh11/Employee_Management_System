using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;

namespace EmployeeManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        public async Task<PagedResult<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy = "Id", bool descending = false, string searchTerm = "")
        {
            var result = await _employeeRepository.GetEmployeesAsync(page, pageSize, sortBy, descending, searchTerm);

            // Load department information for each employee
            var departments = await _departmentRepository.GetAllDepartmentsAsync();
            foreach (var employee in result.Items)
            {
                employee.Department = departments.FirstOrDefault(d => d.Id == employee.DepartmentId);
            }

            return result;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee != null)
            {
                employee.Department = await _departmentRepository.GetDepartmentByIdAsync(employee.DepartmentId);
            }
            return employee;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            if (!await ValidateEmployeeAsync(employee))
                throw new ArgumentException("Invalid employee data");

            return await _employeeRepository.CreateEmployeeAsync(employee);
        }

        public async Task<Employee?> UpdateEmployeeAsync(Employee employee)
        {
            if (!await ValidateEmployeeAsync(employee))
                throw new ArgumentException("Invalid employee data");

            return await _employeeRepository.UpdateEmployeeAsync(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await _employeeRepository.DeleteEmployeeAsync(id);
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string searchTerm = "")
        {
            var totalEmployees = await _employeeRepository.GetTotalEmployeesAsync();
            var recentHires = await _employeeRepository.GetRecentHiresAsync(30);
            var departments = await _departmentRepository.GetAllDepartmentsAsync();

            var employeesByDepartment = new List<DepartmentEmployeeCount>();
            foreach (var department in departments)
            {
                var employeeCount = (await _employeeRepository.GetEmployeesByDepartmentAsync(department.Id)).Count;
                employeesByDepartment.Add(new DepartmentEmployeeCount
                {
                    DepartmentName = department.Name,
                    EmployeeCount = employeeCount
                });
            }

            // Load department info for recent hires
            foreach (var employee in recentHires)
            {
                employee.Department = departments.FirstOrDefault(d => d.Id == employee.DepartmentId);
            }

            return new DashboardViewModel
            {
                TotalEmployees = totalEmployees,
                EmployeesByDepartment = employeesByDepartment,
                RecentHires = recentHires,
                SearchTerm = searchTerm
            };
        }

        public async Task<bool> ValidateEmployeeAsync(Employee employee)
        {
            // Check if hire date is not in the future
            if (employee.HireDate > DateTime.Now.Date)
                return false;

            // Check if department exists
            var department = await _departmentRepository.GetDepartmentByIdAsync(employee.DepartmentId);
            if (department == null)
                return false;

            // Additional validations can be added here
            return true;
        }
    }
}
