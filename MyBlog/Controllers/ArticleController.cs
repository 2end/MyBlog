using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Models;
using MyBlog.Models.ArticleModels;

namespace MyBlog.Controllers
{
	public class ArticleController : Controller
    {
		private readonly ApplicationContext db;

		public ArticleController(ApplicationContext db)
		{
			this.db = db;
		}

		public async Task<IActionResult> Index(int? id)
		{
			if (id != null)
			{
				Article article = await db.Articles.FirstOrDefaultAsync(a => a.Id == id);
				if (article != null)
				{
					return View(article);
				}
			}

			return NotFound();
		}

		public IActionResult Create()
        {
			return View();
        }

		[HttpPost]
		public async Task<IActionResult> Create(Article article)
		{
			if (ModelState.IsValid)
			{
				if (article.FormFile != null)
				{
					using (BinaryReader binaryReader = new BinaryReader(article.FormFile.OpenReadStream()))
					{
						article.Photo = binaryReader.ReadBytes((int)article.FormFile.Length);
					}
				}
				else
				{
					using (FileStream fs = new FileStream("wwwroot/images/default.jpg", FileMode.OpenOrCreate))
					{
						using (BinaryReader binaryReader = new BinaryReader(fs))
						{
							article.Photo = binaryReader.ReadBytes((int)fs.Length);
						}
					}
				}
				db.Articles.Add(article);
				await db.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}

			return View(article);
		}

		[HttpGet]
		[ActionName("Delete")]
		public async Task<IActionResult> ConfirmDelete(int? id)
		{
			if (id != null)
			{
				Article article = await db.Articles.FirstOrDefaultAsync(a => a.Id == id);
				if (article != null)
				{
					return View(article);
				}
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id != null)
			{
				Article article = await db.Articles.FirstOrDefaultAsync(a => a.Id == id);
				if (article != null)
				{
					db.Articles.Remove(article);
					await db.SaveChangesAsync();
					return RedirectToAction("Index", "Home");
				}
			}

			return NotFound();
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id != null)
			{
				Article article = await db.Articles.FirstOrDefaultAsync(a => a.Id == id);
				if (article != null)
				{
					return View(article);
				}
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Article article)
		{
			if (ModelState.IsValid)
			{
				if (article.FormFile != null)
				{
					using (BinaryReader binaryReader = new BinaryReader(article.FormFile.OpenReadStream()))
					{
						article.Photo = binaryReader.ReadBytes((int)article.FormFile.Length);
					}
				}
				else
				{
					Article oldArticle = await db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == article.Id);
					article.Photo = oldArticle.Photo;
				}
				db.Articles.Update(article);
				await db.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}

			return View(article);
		}
    }
}