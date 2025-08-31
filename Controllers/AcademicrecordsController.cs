using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabAssignment6.DataAccess;

namespace LabAssignment6.Controllers
{
    internal class AcademicrecordComparer : IComparer<Academicrecord>
    {
        private readonly string _sortColumn;
        private readonly bool _ascending;

        public AcademicrecordComparer(string sortColumn, bool ascending)
        {
            _sortColumn = sortColumn;
            _ascending = ascending;
        }

        public int Compare(Academicrecord x, Academicrecord y)
        {
            if (x.Grade == null && y.Grade != null) return -1;
            if (x.Grade != null && y.Grade == null) return 1;

            int result = 0;
            switch (_sortColumn)
            {
                case "Course":
                    result = string.Compare(
                        x.CourseCodeNavigation?.Code,
                        y.CourseCodeNavigation?.Code,
                        StringComparison.OrdinalIgnoreCase);
                    break;
                case "Student":
                    result = string.Compare(
                        x.Student?.Id,
                        y.Student?.Id,
                        StringComparison.OrdinalIgnoreCase);
                    break;
            }

            return _ascending ? result : -result;
        }
    }

    public class AcademicrecordsController : Controller
    {
        private readonly StudentrecordContext _context;

        public AcademicrecordsController(StudentrecordContext context)
        {
            _context = context;
        }

        // GET: Academicrecords
        public async Task<IActionResult> Index(string sortOrder)
        {
            var studentrecordContext = await _context.Academicrecords.Include(a => a.CourseCodeNavigation).Include(a => a.Student)
                .ToListAsync();
            
            string sortColumn = sortOrder?.Replace("_desc", "") ?? "Course";
            bool ascending = !sortOrder?.EndsWith("_desc") ?? true;

            studentrecordContext.Sort(new AcademicrecordComparer(sortColumn, ascending));

            ViewData["CourseSortParam"] = sortOrder == "Course" ? "Course_desc" : "Course";
            ViewData["StudentSortParam"] = sortOrder == "Student" ? "Student_desc" : "Student";

            return View(studentrecordContext);
        }

        // GET: Academicrecords/Details/5
        /*
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (academicrecord == null)
            {
                return NotFound();
            }

            return View(academicrecord);
        }
        */

        // GET: Academicrecords/Create
        public IActionResult Create()
        {
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            return View();
        }

        // POST: Academicrecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseCode,StudentId,Grade")] Academicrecord academicrecord)
        {
            bool exists = await _context.Academicrecords
                .AnyAsync(ar => ar.StudentId == academicrecord.StudentId && ar.CourseCode == academicrecord.CourseCode);

            if (exists)
            {
                ModelState.AddModelError("", "This student already has a record for this course.");
                ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicrecord.CourseCode);
                ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicrecord.StudentId);
                return View(academicrecord);
            }

            if (ModelState.IsValid)
            {
                _context.Add(academicrecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicrecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicrecord.StudentId);
            return View(academicrecord);
        }

        // GET: Academicrecords/Edit/5
        public async Task<IActionResult> Edit(string courseCode, string studentId)
        {
            if (courseCode == null || studentId == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.CourseCode == courseCode && a.StudentId == studentId);

            if (academicrecord == null)
            {
                return NotFound();
            }
            //ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicrecord.CourseCode);
            //ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicrecord.StudentId);
            return View(academicrecord);
        }

        // POST: Academicrecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string courseCode, string studentId, [Bind("CourseCode,StudentId,Grade")] Academicrecord updatedRecord)
        {
            var record = await _context.Academicrecords
                .FirstOrDefaultAsync(a => a.CourseCode == courseCode && a.StudentId == studentId);

            if (record == null)
            {
                return NotFound();
            }

            record.Grade = updatedRecord.Grade;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes.");
            }

            record = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.CourseCode == courseCode && a.StudentId == studentId);

            return View(record);
        }

        // GET: Academicrecords/EditAll
        public async Task<IActionResult> EditAll(string sortOrder)
        {
            var records = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .ToListAsync();

            string sortColumn = sortOrder?.Replace("_desc", "") ?? "Course";
            bool ascending = !sortOrder?.EndsWith("_desc") ?? true;

            records.Sort(new AcademicrecordComparer(sortColumn, ascending));

            ViewData["CourseSortParam"] = sortOrder == "Course" ? "Course_desc" : "Course";
            ViewData["StudentSortParam"] = sortOrder == "Student" ? "Student_desc" : "Student";

            return View(records);
        }

        // POST: Academicrecords/EditAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(List<Academicrecord> updatedRecords)
        {
            for (int i = 0; i < updatedRecords.Count; i++)
            {
                if (!TryValidateModel(updatedRecords[i], prefix: $"updatedRecords[{i}]"))
                {
                    // TryValidateModel collects errros
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var record in updatedRecords)
                {
                    record.CourseCodeNavigation = await _context.Courses
                        .FirstOrDefaultAsync(c => c.Code == record.CourseCode);

                    record.Student = await _context.Students
                        .FirstOrDefaultAsync(s => s.Id == record.StudentId);
                }

                ViewData["CourseSortParam"] = "Course";
                ViewData["StudentSortParam"] = "Student";

                return View(updatedRecords);
            }

            foreach (var updated in updatedRecords)
            {
                var record = await _context.Academicrecords
                    .FirstOrDefaultAsync(a => a.CourseCode == updated.CourseCode && a.StudentId == updated.StudentId);

                if (record != null)
                {
                    record.Grade = updated.Grade;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Academicrecords/Delete/5
        /*
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (academicrecord == null)
            {
                return NotFound();
            }

            return View(academicrecord);
        }
        
        // POST: Academicrecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var academicrecord = await _context.Academicrecords.FindAsync(id);
            if (academicrecord != null)
            {
                _context.Academicrecords.Remove(academicrecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */

        private bool AcademicrecordExists(string id)
        {
            return _context.Academicrecords.Any(e => e.StudentId == id);
        }
    }
}
