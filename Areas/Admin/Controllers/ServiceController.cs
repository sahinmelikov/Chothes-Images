using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using WebFrontToBack.Areas.Admin.ViewModels;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;
using WebFrontToBack.Utilities;
using WebFrontToBack.Utilities.Constans;

namespace WebFrontToBack.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private List<Category> _categories;
        private readonly IWebHostEnvironment _environment;
        public ServiceController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _categories = _context.Categories.ToList();
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            //ICollection<WorkService> workservices = await _context.WorkServices.ToListAsync();
            return View(
                await _context.Services.Where(
                    s => !s.IsDeleted)
                .OrderByDescending(s => s.Id)                 //////son yuklenenleri getirir
                .Take(8)
                .Include(s => s.Category)
                .Include(s => s.ServiceImages)
                .ToListAsync()

                );
        }


        public async Task<IActionResult> Create()
        {
            CreateServiceVM createServiceVM = new CreateServiceVM()
            {
                Categories = _categories
            };
            return View(createServiceVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceVM createServiceVM)
        {
            createServiceVM.Categories = _categories;
            if (!ModelState.IsValid)
            {
                return View(createServiceVM);
            }

            foreach (var photo in createServiceVM.Photos)
            {
                if (!photo.CheckContentType("image/"))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}{Messages.FileTypeMustBeImage}");
                    return View(createServiceVM);
                }

                if (!photo.CheckFileSize(4))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}{Messages.FileTypeMustBeLength}");
                    return View(createServiceVM);

                }
            }
          

            List<ServiceImage> serviceImages=new List<ServiceImage>();

            foreach (var photo in createServiceVM.Photos)
            {
                string rootPath = Path.Combine(_environment.WebRootPath, "assets", "img");
                string fileName = await photo.SaveAsync(rootPath);
                ServiceImage serviceImg = new ServiceImage()
                {
                    Path = fileName
                };
                if (!serviceImages.Any(i=>i.IsActive))
                {
                    serviceImg.IsActive = true;
                }
                serviceImages.Add(serviceImg);
            }
         
            return RedirectToAction(nameof(Index));
            //    bool isExists = await _context.WorkCategories.AnyAsync(c =>
            //    c.Name.ToLower().Trim() == workservice.Name.ToLower().Trim());

            //    if (isExists)
            //    {
            //        ModelState.AddModelError("Name", "Category name already exists");
            //        return View();
            //    }
            //    await _context.WorkServices.AddAsync(workservice);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}


            //public IActionResult Update(int Id)
            //{
            //    WorkService? workService = _context.WorkServices.Find(Id);

            //    if (workService == null)
            //    {
            //        return NotFound();
            //    }

            //    return View(workService);
            //}

            //[HttpPost]
            //public IActionResult Update(WorkService workService)
            //{
            //    WorkService? editedWorkService = _context.WorkServices.Find(workService.Id);
            //    if (editedWorkService == null)
            //    {
            //        return NotFound();
            //    }
            //    editedWorkService.Name = workService.Name;
            //    _context.WorkServices.Update(editedWorkService);
            //    _context.SaveChanges();
            //    return RedirectToAction(nameof(Index));
            //}

            //public IActionResult Delete(int Id)
            //{
            //    WorkService? workService = _context.WorkServices.Find(Id);
            //    if (workService == null)
            //    {
            //        return NotFound();
            //    }
            //    _context.WorkServices.Remove(workService);
            //    _context.SaveChanges();
            //    return RedirectToAction(nameof(Index));
            //}
        }
    }
}

