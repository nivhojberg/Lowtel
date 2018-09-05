﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EF.AspNetCore.Models;
using Lowtel.Models;
using Microsoft.AspNetCore.Http;

namespace Lowtel.Controllers
{
    public class UsersController : Controller
    {
        public const string SessionName = "UserName";     
        private readonly LotelContext _context;
        public static bool isLogin = false;
        public static string userName = "";

        public UsersController(LotelContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserName == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserName,Password")] User user)
        {
            if (id != user.UserName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserName))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserName == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.UserName == id);
        }

        // This function return the view of login page
        public IActionResult Login()
        {
            return View();
        }

        // This function signing in the user by checking if the user exist
        // parm: User obj
        // return: redirection to homePage if the user success to login, error if not
        public IActionResult CheckAuthenticate([Bind("UserName,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                if (UserExists(user.UserName))
                {
                    HttpContext.Session.SetString(SessionName, user.UserName);
                    isLogin = true;
                    return RedirectToAction("Index", "Home");
                }
            }
            isLogin = false;
            return NotFound();
        }

        // This function loging off a user
        // return: redirection to homePage
        public IActionResult LogOff()
        {

            HttpContext.Session.Clear();
            isLogin = false;
            return RedirectToAction("Index", "Home");
        }

        // This function check if there is a user who has already logged in
        // return : json of array with userName and isLogin boolean
        [HttpGet]
        public JsonResult checkSession()
        {
            var userName = HttpContext.Session.GetString(UsersController.SessionName);
            List<dynamic> data = new List<dynamic>();
            data.Add(userName);

            if (userName != null)
            {
                isLogin = true;
            }
            else
            {
                isLogin = false;

            }
            data.Add(isLogin);
            return Json(new
            {
                isLogin = isLogin,
                userName = userName
            });
        }

    }
}