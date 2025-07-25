// Controllers/EmployeesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string sortBy = "Id", bool desc = false, string searchTerm = "")
        {
            var employees = await _employeeService.GetEmployeesAsync(page, pageSize, sortBy, desc, searchTerm);

            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentDesc = desc;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.PageSize = pageSize;

            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Server-side validation
                    if (employee.HireDate > DateTime.Now.Date)
                    {
                        ModelState.AddModelError("HireDate", "Hire date cannot be in the future.");
                    }
                    else
                    {
                        await _employeeService.CreateEmployeeAsync(employee);
                        TempData["SuccessMessage"] = "Employee created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee");
                    ModelState.AddModelError("", "An error occurred while creating the employee.");
                }
            }

            await LoadDepartmentsSelectList(employee.DepartmentId);
            return View(employee);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            await LoadDepartmentsSelectList(employee.DepartmentId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Server-side validation
                    if (employee.HireDate > DateTime.Now.Date)
                    {
                        ModelState.AddModelError("HireDate", "Hire date cannot be in the future.");
                    }
                    else
                    {
                        await _employeeService.UpdateEmployeeAsync(employee);
                        TempData["SuccessMessage"] = "Employee updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee with ID {EmployeeId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the employee.");
                }
            }

            await LoadDepartmentsSelectList(employee.DepartmentId);
            return View(employee);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _employeeService.DeleteEmployeeAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Employee deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Employee not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID {EmployeeId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the employee.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDepartmentsSelectList(int? selectedDepartmentId = null)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", selectedDepartmentId);
        }
    }
}
