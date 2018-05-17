using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

		public IActionResult Create()
        {
			return View();
        }

		[HttpPost]
		public async Task<IActionResult> Create(ArticleViewModel model)
		{
			if (ModelState.IsValid)
			{
				byte[] photo = null;
				if (model.Photo != null)
				{
					using (BinaryReader binaryReader = new BinaryReader(model.Photo.OpenReadStream()))
					{
						photo = binaryReader.ReadBytes((int)model.Photo.Length);
					}
				}
				else
				{
					using (FileStream fs = new FileStream("wwwroot/images/default.jpg", FileMode.OpenOrCreate))
					{
						using (BinaryReader binaryReader = new BinaryReader(fs))
						{
							photo = binaryReader.ReadBytes((int)fs.Length);
						}
					}
				}
				Article article = new Article
				{
					Title = model.Title,
					Text = model.Text,
					Date = model.Date,
					Photo = photo
				};
				db.Articles.Add(article);
				await db.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}

			return View(model);
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
    }
}