using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Models;
using MyBlog.Models.ArticleModels;
using MyBlog.Models.Home;

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

		public async Task<IActionResult> Index(string searchText, int page = 1)
        {
			int pageSize = 4;
			IQueryable<Article> articles = db.Articles.OrderByDescending(a => a.Date);
			if (searchText != null)
			{
				searchText = searchText.ToLower();
				articles = articles.Where(a => a.Text.ToLower().Contains(searchText) || a.Title.ToLower().Contains(searchText));
			}			
			int count = articles.Count();
			List<Article> items = await articles.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
			foreach (Article article in items)
			{
				if (article.Text.Length > 200)
				{
					article.Text = CutText(article.Text, 200);
				}
			}
			PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
			HomeIndexViewModel viewModel = new HomeIndexViewModel
			{
				PageViewModel = pageViewModel,
				Articles = items,
			};
			ViewBag.searchText = searchText;

			return View(viewModel);
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
