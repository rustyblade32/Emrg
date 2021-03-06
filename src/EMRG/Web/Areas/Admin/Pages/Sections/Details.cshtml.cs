﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Data.Persistence;
using Domain;

namespace Web.Areas.Admin.Pages.Sections
{
    public class DetailsModel : PageModel
    {
        private readonly Data.Persistence.AppDbContext _context;

        public DetailsModel(Data.Persistence.AppDbContext context)
        {
            _context = context;
        }

        public Section Section { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Section = await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Faculty)
                .Include(s => s.Room)
                .Include(s => s.Semester).FirstOrDefaultAsync(m => m.Id == id);

            if (Section == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
