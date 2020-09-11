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
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carts
        [Authorize]
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
            return View(await _context.Cart.ToListAsync());
        }

        // GET: Carts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Item,Description,Quantity,Price,Totalprice,Buyer,Seller")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Item,Description,Quantity,Price,Totalprice,Buyer,Seller")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            return View(cart);
        }

        // GET: Carts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // buy all the items in the cart
        [Authorize]
        public async Task<IActionResult> BuyAll()
        {
            var byer = _userManager.GetUserName(User);
            var cart = _context.Cart.
                Where(m => m.Buyer == byer);

            foreach (Cart Cart in cart)
            {
                // find the item
                var items = await _context.Item
                    .FirstOrDefaultAsync(m => m.Name == Cart.Item);

                // update the quantity
                items.Quantity -= Cart.Quantity;
                _context.Update(items);
                var buyer = Cart.Buyer;
                var seller = Cart.Seller;
                var item = Cart.Item;
                var price = Cart.Price;
                var totalprice = Cart.Totalprice;
                var quantity = Cart.Quantity;
                var desc = Cart.Description;

                Sale sale = new Sale { Buyer = buyer, Item = item, Quantity = quantity, Seller = seller, Price = price, TotalPrice = totalprice, Description = desc };
                _context.Update(sale);
                _context.Cart.Remove(Cart);
                // remove item if quantity become 0
                if (items.Quantity == 0)
                {
                    _context.Item.Remove(items);
                }

            }
            // Save the changes
            await _context.SaveChangesAsync();
            return View("Index", cart);


        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
    }
}
