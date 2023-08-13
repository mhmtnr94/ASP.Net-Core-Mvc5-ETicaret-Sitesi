using MehmeTicaret2.Data;
using MehmeTicaret2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace MehmeTicaret2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public ShoppingCardVM ShoppingCardVM { get; set; }
        public CardController(UserManager<IdentityUser> userManager,IEmailSender emailSender,ApplicationDbContext db)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCardVM = new ShoppingCardVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCard = _db.ShoppingCards.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            foreach (var item in ShoppingCardVM.ListCard) 
            {
                item.Price = item.Product.Price;
                ShoppingCardVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price); 
            }
            return View(ShoppingCardVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCardVM model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCardVM.ListCard = _db.ShoppingCards.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product);
            ShoppingCardVM.OrderHeader.OrderStatus = Diger.Durum_Beklemede;
            ShoppingCardVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCardVM.OrderHeader.OrderDate = DateTime.Now;
            _db.OrderHeaders.Add(ShoppingCardVM.OrderHeader);
            _db.SaveChanges();
            foreach (var item in ShoppingCardVM.ListCard)
            {
                item.Price = item.Product.Price;
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCardVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                ShoppingCardVM.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                model.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                _db.OrderDetails.Add(orderDetails);
            }
            _db.ShoppingCards.RemoveRange(ShoppingCardVM.ListCard);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.ssShoppingCard, 0);
            return RedirectToAction("SiparisTamam");
        }
        public IActionResult SiparisTamam()
        {
            return View();
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCardVM = new ShoppingCardVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCard = _db.ShoppingCards.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            ShoppingCardVM.OrderHeader.OrderTotal = 0;
            ShoppingCardVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            foreach (var item in ShoppingCardVM.ListCard)
            {
                ShoppingCardVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCardVM);
        }
        public IActionResult Add(int cardId)
        {
            var card = _db.ShoppingCards.FirstOrDefault(i => i.Id == cardId);
            card.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Decrease(int cardId)
        {
            var card = _db.ShoppingCards.FirstOrDefault(i => i.Id == cardId);
            if(card.Count == 1)
            {
                var count = _db.ShoppingCards.Where(u => u.ApplicationUserId == card.ApplicationUserId).ToList().Count();
                _db.ShoppingCards.Remove(card);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShoppingCard, count - 1);
            }
            else
            {
                card.Count -= 1;
                _db.SaveChanges();
            }
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cardId)
        {
            var card = _db.ShoppingCards.FirstOrDefault(i => i.Id == cardId);
            
                var count = _db.ShoppingCards.Where(u => u.ApplicationUserId == card.ApplicationUserId).ToList().Count();
                _db.ShoppingCards.Remove(card);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShoppingCard, count - 1);
            
            

            return RedirectToAction(nameof(Index));
        }

    }
}
