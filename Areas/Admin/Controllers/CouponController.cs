﻿using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoodFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var coupons = _context.Coupons.ToList();
            return View(coupons);
        }
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                _context.Coupons.Add(coupon);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(coupon);
        }

        public IActionResult Edit(int id)
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon == null) return NotFound();
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                _context.Coupons.Update(coupon);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(coupon);
        }

        public IActionResult Delete(int id) 
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon == null) return NotFound();

            _context.Coupons.Remove(coupon);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
