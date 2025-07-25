// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IEmployeeService employeeService, ILogger<HomeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var dashboardData = await _employeeService.GetDashboardDataAsync(searchTerm);
            return View(dashboardData);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrEmpty(term))
                return RedirectToAction("Index");

            var employees = await _employeeService.GetEmployeesAsync(1, 10, "Id", false, term);
            return Json(employees.Items.Select(e => new
            {
                id = e.Id,
                name = e.FullName,
                email = e.Email,
                department = e.Department?.Name
            }));
        }
    }
}
