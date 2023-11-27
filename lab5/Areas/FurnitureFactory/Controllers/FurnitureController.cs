using lab5.Areas.FurnitureFactory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab5.Areas.FurnitureFactory.Models;
using lab5.Areas.FurnitureFactory.ViewModels;
using lab5.Areas.FurnitureFactory.Filters;

using System.Drawing.Printing;
using lab5.Areas.FurnitureFactory.Services;

namespace lab5.Areas.FurnitureFactory.Controllers
{
    [Area("FurnitureFactory")]
    public class FurnitureController : Controller
    {
        private readonly AcmeDataContext _context;
        private readonly int pageSize = 8;

        public FurnitureController(AcmeDataContext context)
        {
            _context = context;
        }

        // GET: FurnitureFactory/Furniture
        public IActionResult Index(int page = 1)
        {
            FurnitureViewModel furnitures;
            var furniture = HttpContext.Session.Get<FurnitureViewModel>("Furniture");
            if (furniture == null)
            {
                furniture = new FurnitureViewModel();
            }
            IQueryable<Furniture> furnituresDbContext = _context.Furnitures;
            furnituresDbContext = Sort_Search(furnituresDbContext, furniture.FurnitureName, furniture.Description
                , furniture.MaterialType, furniture.Price, furniture.QuantityOnHand);
            // Разбиение на страницы
            var count = furnituresDbContext.Count();
            furnituresDbContext = furnituresDbContext.Skip((page - 1) * pageSize).Take(pageSize);


            furnitures = new FurnitureViewModel
            {
                Furnitures = furnituresDbContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                FurnitureName = furniture.FurnitureName,
                Description = furniture.Description,
                MaterialType = furniture.MaterialType,
                Price = furniture.Price,
                QuantityOnHand = furniture.QuantityOnHand

            };
            return View(furnitures);
        }
        [HttpPost]
        public IActionResult Index(FurnitureViewModel furniture)
        {

            HttpContext.Session.Set("Furniture", furniture);

            return RedirectToAction("Index");
        }


        // GET: FurnitureFactory/Furniture/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Furnitures == null)
            {
                return NotFound();
            }

            var furniture = await _context.Furnitures
                .FirstOrDefaultAsync(m => m.FurnitureId == id);
            if (furniture == null)
            {
                return NotFound();
            }

            return View(furniture);
        }

        // GET: FurnitureFactory/Furniture/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FurnitureFactory/Furniture/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FurnitureId,FurnitureName,Description,MaterialType,Price,QuantityOnHand")] Furniture furniture)
        {
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            if (ModelState.IsValid)
            {
                _context.Add(furniture);
                await _context.SaveChangesAsync();
                cache.SetFurnitures();
                return RedirectToAction(nameof(Index));
            }
            return View(furniture);
        }

        // GET: FurnitureFactory/Furniture/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Furnitures == null)
            {
                return NotFound();
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();
            var furniture = cache.GetFurnitures().FirstOrDefault(f => f.FurnitureId == id);
            if (furniture == null)
            {
                return NotFound();
            }
            return View(furniture);
        }

        // POST: FurnitureFactory/Furniture/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FurnitureId,FurnitureName,Description,MaterialType,Price,QuantityOnHand")] Furniture furniture)
        {
            if (id != furniture.FurnitureId)
            {
                return NotFound();
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(furniture);
                    await _context.SaveChangesAsync();
                    cache.SetFurnitures();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FurnitureExists(furniture.FurnitureId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(furniture);
        }

        // GET: FurnitureFactory/Furniture/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Furnitures == null)
            {
                return NotFound();
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();
            var furniture = cache.GetFurnitures().FirstOrDefault(f => f.FurnitureId == id);
            if (furniture == null)
            {
                return NotFound();
            }

            return View(furniture);
        }

        // POST: FurnitureFactory/Furniture/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Furnitures == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Furnitures'  is null.");
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();
            var furniture = cache.GetFurnitures().FirstOrDefault(f => f.FurnitureId == id); ;
            if (furniture != null)
            {
                _context.Furnitures.Remove(furniture);
            }
            
            await _context.SaveChangesAsync();
            cache.SetFurnitures();
            return RedirectToAction(nameof(Index));
        }

        private bool FurnitureExists(int id)
        {
          return (_context.Furnitures?.Any(e => e.FurnitureId == id)).GetValueOrDefault();
        }
        private IQueryable<Furniture> Sort_Search(IQueryable<Furniture> furnitures, string FurnitureName, string Description,
    string MaterialType, decimal Price, int QuantityOnHand)
        {

            furnitures = furnitures
            .Where(c => c.FurnitureName.Contains(FurnitureName ?? "")
            && c.Description.Contains(Description ?? "")
            && c.MaterialType.Contains(MaterialType ?? "")
            && (c.Price == Price || Price ==0)
            && (c.QuantityOnHand == QuantityOnHand || QuantityOnHand == 0));
            return furnitures;
        }
    }
}
