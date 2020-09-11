using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RajaEcorp.Data;
using RajaEcorp.Models;

namespace RajaEcorp.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SalesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Sales
        
        public async Task<IActionResult> Index()
        {
            var buyer = _userManager.GetUserName(User);
            var cart = _context.Cart
               .Where(m => m.Buyer == buyer);
            System.Int32 Count = cart.Count();
            var sc = "Shopping Cart";
            if (Count != 0)
            {
                ViewBag.Count = Count;
                ViewBag.sc = sc;
            }
            return View(await _context.Sale.ToListAsync());
        }
        // items bought by a buyer
        [Authorize]
        public ActionResult MyPurchase()
        {
            var seller = _userManager.GetUserName(User);
            var buyer = _userManager.GetUserName(User);
            var mysales = _context.Sale
                .Where(m => m.Buyer == buyer);
            
            var cart = _context.Cart
               .Where(m => m.Buyer == buyer);
            System.Int32 Count = cart.Count();
            var sc = "Shopping Cart";
            if (Count != 0)
            {
                ViewBag.Count = Count;
                ViewBag.sc = sc;
            }
            var sales = from m in mysales
                        select m;

            
            return View("MyPurchase", sales);
        }
        // sales made by seller
        [Authorize]
        public ActionResult MySales()
        {
            var seller = _userManager.GetUserName(User);
            var buyer = _userManager.GetUserName(User);
            var mysales = _context.Sale
                .Where(m => m.Seller == seller);
           
            var cart = _context.Cart
               .Where(m => m.Buyer == buyer);
            System.Int32 Count = cart.Count();
            var sc = "Shopping Cart";
            if (Count != 0)
            {
                ViewBag.Count = Count;
                ViewBag.sc = sc;
            }
            var sales = from m in mysales
                        select m;


            return View("MySales", sales);
        }


        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sales/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Item,Description,Quantity,Price,TotalPrice,Buyer,Seller")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sale);
        }

        // GET: Sales/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Item,Description,Quantity,Price,TotalPrice,Buyer,Seller")] Sale sale)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id))
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
            return View(sale);
        }

        // GET: Sales/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sale.FindAsync(id);
            _context.Sale.Remove(sale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sale.Any(e => e.Id == id);
        }
    }
}
