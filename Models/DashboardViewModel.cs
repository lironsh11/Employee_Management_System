namespace EmployeeManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public List<DepartmentEmployeeCount> EmployeesByDepartment { get; set; } = new();
        public List<Employee> RecentHires { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class DepartmentEmployeeCount
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }
}