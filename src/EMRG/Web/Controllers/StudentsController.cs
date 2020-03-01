using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Core;
using Domain;
using Brotal.Extensions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        public StudentsController(IUnitOfWork db)
        {
            _db = db;
        }
        // GET: api/Students
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            IList<Student> student = (await _db.Students.GetAll())
                                    .ToList();
            student.ForEach(s => {
                s.Department = null;
                s.Program = null;
                s.Enrollments.ForEach(e =>
                {
                    e.Student = null;
                });
            });

            return new JsonResult(student);
        }

        // GET: api/Students/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<JsonResult> GetAsync(int id)
        {
            var student = await _db.Students.GetById((int)id);

            return new JsonResult(student);
        }

        // POST: api/Students
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
