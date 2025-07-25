using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;

namespace EmployeeManagementSystem.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
        {
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllDepartmentsAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _departmentRepository.GetDepartmentByIdAsync(id);
        }

        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            return await _departmentRepository.CreateDepartmentAsync(department);
        }

        public async Task<Department?> UpdateDepartmentAsync(Department department)
        {
            return await _departmentRepository.UpdateDepartmentAsync(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            return await _departmentRepository.DeleteDepartmentAsync(id);
        }
    }
}