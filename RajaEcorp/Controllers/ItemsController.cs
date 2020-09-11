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
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Items
        public async Task<IActionResult> Index(string searchString)
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
            var items = from m in _context.Item
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.Contains(searchString));
            }
            
            return View(await items.ToListAsync());
        }
        [Authorize]
        public ActionResult MyItems(string searchString)
        {
            var seller = _userManager.GetUserName(User);
            var Myitems = _context.Item
                .Where(m => m.Seller == seller);
            var buyer = _userManager.GetUserName(User);
            // taking only the buyer cart
            var cart = _context.Cart
               .Where(m => m.Buyer == buyer);
            System.Int32 Count = cart.Count();
            var sc = "Shopping Cart";
            if (Count != 0)
            {
                ViewBag.Count = Count;
                ViewBag.sc = sc;
            }
            var items = from m in Myitems
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.Contains(searchString));
            }
            return View("MyItems", items);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Date,Quantity,Price")] Item item)
        {
            if (ModelState.IsValid)
            {
                var seller = _userManager.GetUserName(User);
                item.Seller = seller;
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Date,Quantity,Price,Seller")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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
            return View(item);
        }

        // GET: Items/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public async Task<IActionResult> Shopping(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var buyer = _userManager.GetUserName(User);
            var items = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            var seller = items.Seller;
            if (items == null)
            {
                return NotFound();
            }
            
            else
            {
                return View(items);
            }

        }

        // POST: Items/Shopping/5
        [HttpPost, ActionName("Shopping")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShoppingCart([Bind("Item,Description,Price,Quantity,Totalprice,Seller")] Cart cart)
        {
            // get the buyer
            var buyer = _userManager.GetUserName(User);
            cart.Buyer = buyer;
            var items = await _context.Item
                .FirstOrDefaultAsync(m => m.Name == cart.Item);
            // purchase quantity more than available
            if (cart.Quantity > items.Quantity)
            {
                return View("Extra", items);
            }
            else
            {
                _context.Add(cart);


                if (items == null)
                {
                    return NotFound();
                }
                cart.Totalprice = items.Price * cart.Quantity;

            }


            // Save the changes
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.Id == id);
        }
    }
}
