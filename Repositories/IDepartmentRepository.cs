using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<Department> CreateDepartmentAsync(Department department);
        Task<Department?> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<bool> HasEmployeesAsync(int departmentId);
    }
}
