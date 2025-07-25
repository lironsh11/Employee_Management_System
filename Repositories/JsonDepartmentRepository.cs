using EmployeeManagementSystem.Models;
using Newtonsoft.Json;

namespace EmployeeManagementSystem.Repositories
{
    public class JsonDepartmentRepository : IDepartmentRepository
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _fileLock = new(1, 1);
        private readonly ILogger<JsonDepartmentRepository> _logger;
        private readonly IEmployeeRepository _employeeRepository;

        public JsonDepartmentRepository(IWebHostEnvironment environment, ILogger<JsonDepartmentRepository> logger, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
            var appDataPath = Path.Combine(environment.ContentRootPath, "App_Data");
            Directory.CreateDirectory(appDataPath);
            _filePath = Path.Combine(appDataPath, "departments.json");

            // Initialize with default departments if file doesn't exist
            InitializeDefaultDepartments().Wait();
        }

        private async Task InitializeDefaultDepartments()
        {
            if (!File.Exists(_filePath))
            {
                var defaultDepartments = new List<Department>
                {
                    new Department { Id = 1, Name = "Human Resources", Description = "Manages employee relations and policies" },
                    new Department { Id = 2, Name = "Information Technology", Description = "Handles technology infrastructure and development" },
                    new Department { Id = 3, Name = "Finance", Description = "Manages financial operations and accounting" },
                    new Department { Id = 4, Name = "Marketing", Description = "Handles marketing and promotional activities" },
                    new Department { Id = 5, Name = "Operations", Description = "Manages day-to-day business operations" }
                };

                await SaveDepartmentsAsync(defaultDepartments);
            }
        }

        private async Task<List<Department>> LoadDepartmentsAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Department>();

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<List<Department>>(json) ?? new List<Department>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading departments from file");
                return new List<Department>();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private async Task SaveDepartmentsAsync(List<Department> departments)
        {
            await _fileLock.WaitAsync();
            try
            {
                var json = JsonConvert.SerializeObject(departments, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving departments to file");
                throw;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await LoadDepartmentsAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            var departments = await LoadDepartmentsAsync();
            return departments.FirstOrDefault(d => d.Id == id);
        }

        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            var departments = await LoadDepartmentsAsync();
            department.Id = departments.Any() ? departments.Max(d => d.Id) + 1 : 1;
            departments.Add(department);
            await SaveDepartmentsAsync(departments);
            return department;
        }

        public async Task<Department?> UpdateDepartmentAsync(Department department)
        {
            var departments = await LoadDepartmentsAsync();
            var existingDepartment = departments.FirstOrDefault(d => d.Id == department.Id);

            if (existingDepartment == null)
                return null;

            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;

            await SaveDepartmentsAsync(departments);
            return existingDepartment;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            // Check if department has employees
            if (await HasEmployeesAsync(id))
                return false;

            var departments = await LoadDepartmentsAsync();
            var department = departments.FirstOrDefault(d => d.Id == id);

            if (department == null)
                return false;

            departments.Remove(department);
            await SaveDepartmentsAsync(departments);
            return true;
        }

        public async Task<bool> HasEmployeesAsync(int departmentId)
        {
            var employees = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentId);
            return employees.Any();
        }
    }
}