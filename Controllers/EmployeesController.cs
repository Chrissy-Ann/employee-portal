using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabAssignment6.DataAccess;
using LabAssignment6.Models.ViewModels;
using System.Data;

namespace LabAssignment6.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly StudentrecordContext _context;

        public EmployeesController(StudentrecordContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Roles)
                .ToListAsync();

            return View(employees);
        }

        // GET: Employees/Details/5
        /*
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        */

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            var allRoles = await _context.Roles
                .OrderBy(r => r.Role1)
                .Select(r => r.Role1)
                .ToListAsync();

            var viewModel = new EmployeeRoleSelectionCreate
            {
                Roles = allRoles
            };
            return View(viewModel);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeRoleSelectionCreate model)
        {
            if (model.SelectedRoles == null || !model.SelectedRoles.Any())
            {
                ModelState.AddModelError("SelectedRoles", "At least one job title must be selected.");
            }

            bool usernameExists = await _context.Employees
                .AnyAsync(e => e.UserName == model.UserName);

            if (usernameExists)
            {
                ModelState.AddModelError("UserName", "This Network ID is already in use.");
            }

            if (!ModelState.IsValid)
            {
                model.Roles = await _context.Roles
                    .OrderBy(r => r.Role1)
                    .Select(r => r.Role1)
                    .ToListAsync();
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var rolesToAssign = await _context.Roles
                    .Where(r => model.SelectedRoles.Contains(r.Role1))
                    .ToListAsync();

                var newEmployee = new Employee
                {
                    Name = model.Name,
                    UserName = model.UserName,
                    Password = model.Password,
                    Roles = rolesToAssign
                };

                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var employee = await _context.Employees
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var allRoles = await _context.Roles
                .OrderBy(r => r.Role1)
                .Select(r => r.Role1)
                .ToListAsync();

            var model = new EmployeeRoleSelectionEdit
            {
                Id = employee.Id,
                Name = employee.Name,
                UserName = employee.UserName,
                Password = employee.Password,
                SelectedRoles = employee.Roles.Select(r => r.Role1).ToList(),
                Roles = allRoles
            };

            return View(model);

        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeRoleSelectionEdit model)
        {
            if (model.SelectedRoles == null || !model.SelectedRoles.Any())
            {
                ModelState.AddModelError("SelectedRoles", "At least one job title must be selected.");
            }

            bool usernameExists = await _context.Employees
                .AnyAsync(e => e.UserName == model.UserName && e.Id != model.Id);

            if (usernameExists)
            {
                ModelState.AddModelError("UserName", "This Network ID is already in use.");
            }

            if (!ModelState.IsValid)
            {
                model.Roles = await _context.Roles
                    .OrderBy(r => r.Role1)
                    .Select(r => r.Role1)
                    .ToListAsync();
                return View(model);
            }

            var employee = await _context.Employees
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            if (employee == null) { return NotFound(); }

            employee.Name = model.Name;
            employee.UserName = model.UserName;
            employee.Password = model.Password;

            var selectedRoles = await _context.Roles
                .Where(r => model.SelectedRoles.Contains(r.Role1))
                .ToListAsync();

            employee.Roles = selectedRoles;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Delete/5
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
