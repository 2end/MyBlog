using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models.ArticleModels
{
    public class Article
    {
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public string Text { get; set; }
		[Required]
		public DateTime Date { get; set; }
		public byte[] Photo { get; set; }
		
		[NotMapped]
		public IFormFile FormFile { get; set; }
	}
}
