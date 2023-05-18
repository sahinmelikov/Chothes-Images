using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WebFrontToBack.Areas.Admin.ViewModels;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;
using WebFrontToBack.Utilities;
using WebFrontToBack.Utilities.Constans;

namespace WebFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _context.Services.Where(
                      s => !s.IsDeleted)
                  .OrderByDescending(s => s.Id)                 //////son yuklenenleri getirir
                  .Take(8)
                  .Include(s => s.Category)
                  .Include(s => s.ServiceImages)
                  .ToListAsync());
            //ICollection<TeamMember> CreateTeamMemberVM = await _context.TeamMembers.ToListAsync();
            //return View(CreateTeamMemberVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CreateTeamMemberVM CreateTeamMemberVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!CreateTeamMemberVM.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Phote", $"{CreateTeamMemberVM.Photo.FileName} {Messages.FileTypeMustBeImage}");
                return View();
            }
            if (!CreateTeamMemberVM.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Phote", "image Length to enter!");
                return View();

            }
        
            string root =Path.Combine( _webHostEnvironment.WebRootPath,"assets","img");

            string FileName = await CreateTeamMemberVM.Photo.SaveAsync(root);

            TeamMember teamMember = new TeamMember
            {
                FullName = CreateTeamMemberVM.FullName,
                ImagePath = FileName,
                Profession = CreateTeamMemberVM.Profession
            };

            await _context.TeamMembers.AddAsync(teamMember);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


            //bool isExists = await _context.TeamMembers.AnyAsync(c =>
            //c.FullName.ToLower().Trim() == CreateTeamMemberVM.FullName.ToLower().Trim());

            //if (isExists)
            //{
            //    ModelState.AddModelError("FullName", "FullName name already exists");
            //    return View();
            //}
            //await _context.TeamMembers.AddAsync(teamMembers);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));

        }

            public IActionResult Update(int Id)
            {
                TeamMember? teamMember = _context.TeamMembers.Find(Id);

                if (teamMember == null)
                {
                    return NotFound();
                }

                return View(teamMember);
            }

            [HttpPost]
            public IActionResult Update(TeamMember teamMember)
            {
                TeamMember? editedTeamMember = _context.TeamMembers.Find(teamMember.Id);
                if (editedTeamMember == null)
                {
                    return NotFound();
                }
                editedTeamMember.FullName = teamMember.FullName;
                _context.TeamMembers.Update(editedTeamMember);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            public IActionResult Delete(int Id)
            {
                TeamMember? teamMember = _context.TeamMembers.Find(Id);
                if (teamMember == null)
                {
                    return NotFound();
                }
                _context.TeamMembers.Remove(teamMember);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
       
    }
}

