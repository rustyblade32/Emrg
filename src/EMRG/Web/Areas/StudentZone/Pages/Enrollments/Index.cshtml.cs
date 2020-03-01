using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Core;
using Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace Web.Areas.StudentZone.Pages.Enrollments
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _db;

        public IndexModel(IUnitOfWork db, UserManager<AppUser> usermanager)
        {
            _db = db;
            Usermanager = usermanager;
        }

        [BindProperty]
        public Student Student { get; set; }

        [BindProperty]
        public IList<Course> Courses { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        [BindProperty]
        public Section Section { get; set; }

        [BindProperty]
        public IList<Semester> Semesters { get; set; }

        [BindProperty]
        public Semester Semester { get; set; }

        [BindProperty]
        public CourseEnrollment Enrollment { get; set; }
        public UserManager<AppUser> Usermanager { get; }

        public async Task<IActionResult> OnGet()
        {
            var user = await Usermanager.GetUserAsync(User);
            Student = (await _db.Students.GetAll())
                            .Where(x => x.StudentId.ToString() == user.UserName)
                            .FirstOrDefault();


            Semesters = (await _db.Semesters.GetAll())
                            .Where(x => x.isActive == true).ToList();

            Courses = (await _db.Courses.GetAll()).ToList();

            if (Semesters.Count() == 0)
            {
                ViewData["Title"] = "Advising Not Started";
            }

            else
            {
                ViewData["Title"] = "Advising";
                ViewData["CourseId"] =
                    new SelectList(
                        Courses,
                        nameof(Course.Id),
                        nameof(Course.Code));

                ViewData["SemesterId"] =
                    new SelectList(
                        Semesters,
                        nameof(Semester.Id),
                        nameof(Semester.Name));
            }

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            Enrollment.StudentId = Student.Id;

            var original = await _db.Students.GetById(Student.Id);

            var originalSection = await _db.Sections.GetById(Enrollment.SectionId);

            if (originalSection.Seat <= 0)
            {
                return BadRequest();
            }

            originalSection.Seat--;

            original.Enrollments.Add(Enrollment);



            try
            {
                await _db.CompleteAsync();
            }
            catch (Exception)
            {
                if (!await StudentExistsAsync(Student.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteEnrollmentAsync()
        {


            await _db.CourseEnrollments.Delete(Enrollment.Id);
            var original = await _db.CourseEnrollments.GetById(Enrollment.Id);


            var originalSection = await _db.Sections.GetById(original.SectionId);

            originalSection.Seat++;

            try
            {
                await _db.CompleteAsync();
            }
            catch (Exception)
            {
                if (!await StudentExistsAsync(Student.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage();


        }


        private async Task<bool> StudentExistsAsync(int id)
        {
            return await _db.Students.Exists(id);
        }
    }
}