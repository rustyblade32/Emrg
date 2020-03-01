using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using AutoMapper;

using Brotal.Extensions;

using Data.Core;

using Domain;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Web.Areas.Admin.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<CreateModel> _logger;
        private readonly IEmailSender _emailSender;

        public CreateModel(
            IMapper mapper,
            IUnitOfWork db,
            UserManager<AppUser> userManager,
            ILogger<CreateModel> logger,
            IEmailSender emailSender
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //ViewData["DepartmentId"] =
            //    new SelectList(
            //        await _db.Departments.GetAll(),
            //        nameof(Department.Id),
            //        nameof(Department.Name));
            ViewData["ProgramId"] =
                new SelectList(
                    await _db.Programs.GetAll(),
                    nameof(Domain.Program.Id),
                    nameof(Domain.Program.Name));
            return Page();
        }

        [BindProperty]
        public StudentInputModel StudentInput { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var program = await _db.Programs.GetById(StudentInput.ProgramId);

            var sId = new StudentId
            {
                Year = DateTime.Today.Year,
                Season = DateTime.Now.GetSeason(),
                Program = program.Code
            };
            var students = await _db.Students.Get(
                s => s.StudentId.AreBatchMates(sId),
                s => s.StudentId.Roll);
            sId.Roll = (students.FirstOrDefault()?.StudentId.Roll ?? 0) + 1;

            var student = _mapper.Map<Student>(StudentInput);
            student.AdmissionDate = DateTime.Today;
            student.StudentId = sId;
            student.DepartmentId = program.DepartmentId;
            student.Meta = Metadata.Created(User.Identity.Name);

            _db.Students.Add(student);
            await _db.CompleteAsync();

            // Create user
            var user = new AppUser
            {
                Role = Role.Student,
                UserName = student.StudentId.ToString(),
                Email = student.Email
            };
            //var password = Membership.GenerateRandomPassword(
            //    new Brotal.Extensions.PasswordOptions
            //    {
            //        RequiredLength = 8,
            //        RequireDigit = true,
            //        RequireLowercase = true,
            //        RequireUppercase = true,
            //        RequiredUniqueChars = 0,
            //        RequireNonAlphanumeric = true
            //    }
            //);
            var password = "#Arefin555";

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                _logger.LogInformation(
                    $"{User.Identity.Name} created a new student user " +
                    $"with id: {student.StudentId}, " +
                    $"password: {password}");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code },
                    protocol: Request.Scheme
                );

                await _emailSender.SendEmailAsync(student.Email, "Confirm your email",
                    $"Please confirm your account by " +
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" +
                    $"clicking here" +
                    $"</a>.<br/>" +
                    $"Your Student Id/ Username: {student.StudentId}" +
                    $"Your password is: {password}");

                //await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToPage("./Index");
        }
    }
}