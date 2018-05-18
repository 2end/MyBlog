using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Models;
using MyBlog.Models.ArticleModels;

namespace MyBlog.Controllers
{
    public class HomeController : Controller
    {
		private readonly ApplicationContext db;

		public HomeController(ApplicationContext db)
		{
			this.db = db;
		}

		public IActionResult ErrorStatus(string id)
		{
			return View("ErrorStatus", id);
		}

		private string CutText(string text, int length)
		{
			text = text.Substring(0, length);
			int indexLastWord = text.LastIndexOf(' ');
			text = text.Remove(indexLastWord);
			text += "...";

			return text;
		}

		public async Task<IActionResult> Index()
        {
			List<Article> articles = await db.Articles.OrderByDescending(a => a.Date).ToListAsync();
			foreach (Article article in articles)
			{
				if (article.Text.Length > 200)
				{
					article.Text = CutText(article.Text, 200);
				}
			}
            return View(articles);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
