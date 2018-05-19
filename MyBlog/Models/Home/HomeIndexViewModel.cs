using MyBlog.Models.ArticleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models.Home
{
    public class HomeIndexViewModel
    {
		public List<Article> Articles { get; set; }
		public PageViewModel PageViewModel { get; set; }
	}
}
