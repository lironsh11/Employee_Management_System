// Controllers/DepartmentsController.cs
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentService.CreateDepartmentAsync(department);
                    TempData["SuccessMessage"] = "Department created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating department");
                    ModelState.AddModelError("", "An error occurred while creating the department.");
                }
            }

            return View(department);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentService.UpdateDepartmentAsync(department);
                    TempData["SuccessMessage"] = "Department updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating department with ID {DepartmentId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the department.");
                }
            }

            return View(department);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _departmentService.DeleteDepartmentAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Department deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot delete department - it still has employees assigned to it.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department with ID {DepartmentId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the department.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}