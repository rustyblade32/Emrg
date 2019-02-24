﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Data.Persistence;
using Domain;

namespace Web.Areas.Admin.Pages.Semesters
{
    public class IndexModel : PageModel
    {
        private readonly Data.Persistence.AppDbContext _context;

        public IndexModel(Data.Persistence.AppDbContext context)
        {
            _context = context;
        }

        public IList<Semester> Semester { get;set; }

        public async Task OnGetAsync()
        {
            Semester = await _context.Semesters.ToListAsync();
        }
    }
}
