using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IEmployeeService
    {
        Task<PagedResult<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy = "Id", bool descending = false, string searchTerm = "");
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee?> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<DashboardViewModel> GetDashboardDataAsync(string searchTerm = "");
        Task<bool> ValidateEmployeeAsync(Employee employee);
    }
}

