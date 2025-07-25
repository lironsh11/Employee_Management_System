using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Repositories
{
    public interface IEmployeeRepository
    {
        Task<PagedResult<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy = "Id", bool descending = false, string searchTerm = "");
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee?> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<List<Employee>> GetRecentHiresAsync(int days = 30);
        Task<int> GetTotalEmployeesAsync();
        Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    }
}
