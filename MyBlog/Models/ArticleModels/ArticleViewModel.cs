using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models.ArticleModels
{
    public class ArticleViewModel
    {
		[Required]
		public string Title { get; set; }

		[Required]
		public string Text { get; set; }

		[Required]
		public DateTime Date { get; set; }

		public IFormFile Photo { get; set; }
	}
}
