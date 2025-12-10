using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject.Data;
using CourseProject.DataModels;

namespace CourseProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly EmploymentServiceContext _context;

        public UsersController(EmploymentServiceContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,RoleId,Username,Password,Name,Surname,Patronymic")] User user)
        {
            if (!UserExists(user.Username))
            {
                user.RoleId = 1;
                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже существует");
                return View();
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(u => u.Requests).ThenInclude(r => r.Event).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,RoleId,Username,Password,Name,Surname,Patronymic")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (!UserExists(user.Username))
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            else
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже существует");
                return View();
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private bool UserExists(string name)
        {
            return _context.Users.Any(e => e.Username == name);
        }
    }
}
