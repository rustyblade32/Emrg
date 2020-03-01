using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Data.Persistence;
using Domain;

namespace Web.Areas.StudentZone.Pages.Sections
{
    public class IndexModel : PageModel
    {
        private readonly Data.Persistence.AppDbContext _context;

        public IndexModel(Data.Persistence.AppDbContext context)
        {
            _context = context;
        }

        public IList<Section> Section { get; set; }

        public async Task OnGetAsync()
        {
            Section = await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Faculty)
                .Include(s => s.Room)
                .Include(s => s.Semester)
                .Where(s => s.Semester.isActive == true)
                .ToListAsync();
        }
    }
}