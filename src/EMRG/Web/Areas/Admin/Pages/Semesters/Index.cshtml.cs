using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data.Core;

using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Web.Areas.Admin.Pages.Semesters
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _db;

        public IndexModel(IUnitOfWork db)
        {
            _db = db;
        }

        public IList<Semester> Semester { get; set; }

        [BindProperty]
        public Semester semester { get; set; }

        public async Task OnGetAsync()
        {
            Semester = (await _db.Semesters.GetAll()).ToList();
        }


        public async Task<IActionResult> OnPostSartSemesterAsync()
        {
            var original = await _db.Semesters.GetById(semester.Id);
            original.isActive = true;

            try
            {
                await _db.CompleteAsync();
            }
            catch (Exception)
            {
                if (!await SemesterExistsAsync(semester.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }


        public async Task<IActionResult> OnPostEndSemesterAsync()
        {
            var original = await _db.Semesters.GetById(semester.Id);
            original.isActive = false;

            try
            {
                await _db.CompleteAsync();
            }
            catch (Exception)
            {
                if (!await SemesterExistsAsync(semester.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> SemesterExistsAsync(int id)
        {
            return await _db.Programs.Exists(id);
        }
    }
}
