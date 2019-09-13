using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using studentManager.Models;

namespace studentManager.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentContext _context;

        public StudentController(StudentContext context)
        {
            _context = context;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            return View(await _context.Student.ToListAsync());
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,DayOfBirth")] Student student, IFormFile ProfilePic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                
                if (ProfilePic != null)
                {
                    if (ProfilePic.FileName != null)
                    {
                        var imageName = $"{student.Id}" + Path.GetExtension(ProfilePic.FileName);
                        var imageFolder = "Images";
                        var imageFolderPath = Path.GetFullPath($"./wwwroot/{imageFolder}");
                        var imagePath = Path.Join(imageFolderPath, imageName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            var task = ProfilePic.CopyToAsync(stream);
                            await task;
                            if (!task.IsFaulted)
                            {
                                student.ProfilePicPath = imageFolder + "/" + imageName;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,DayOfBirth,ProfilePicPath")] Student student, IFormFile ProfilePic)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ProfilePic != null)
                    {
                        if (ProfilePic.FileName != null)
                        {
                            var imageName = $"{student.Id}" + Path.GetExtension(ProfilePic.FileName);
                            var imageFolder = "Images";
                            var imageFolderPath = Path.GetFullPath($"./wwwroot/{imageFolder}");
                            var imagePath = Path.Join(imageFolderPath, imageName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                var task = ProfilePic.CopyToAsync(stream);
                                await task;
                                System.Diagnostics.Debug.WriteLine($"\n\n{task.IsFaulted}\n");
                                if (!task.IsFaulted)
                                {
                                    if (Path.GetExtension(ProfilePic.FileName) != Path.GetExtension(student.ProfilePicPath))
                                    {
                                        var path = Path.Join("./wwwroot", student.ProfilePicPath);
                                        if (System.IO.File.Exists(path))
                                        {
                                            System.IO.File.Delete(path);
                                        }
                                    }
                                    student.ProfilePicPath = imageFolder + "/" + imageName;
                                }
                            }
                        }
                    }
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            
            var imagePath = Path.Join("./wwwroot", student.ProfilePicPath);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }

        [HttpGet("query")]
        public string Query()
        {
            var results = _context.Student.FromSql("SELECT * FROM STUDENT").ToArray();
            return $"{results}";
        }
    }
}
