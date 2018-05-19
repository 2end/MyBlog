using MyBlog.Models.ArticleModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class ArticleInitializer
    {
		public static async Task InitializeAsync(ApplicationContext db)
		{
			if (!db.Articles.Any())
			{
				string[] lines = File.ReadAllLines("Articles.txt");
				int photoIndex = 0;
				for (int i = 0; i < lines.Length; i += 2)
				{
					byte[] photo = null;
					using (FileStream fs = new FileStream($"wwwroot/images/{photoIndex}.jpeg", FileMode.OpenOrCreate))
					{
						using (BinaryReader binaryReader = new BinaryReader(fs))
						{
							photo = binaryReader.ReadBytes((int)fs.Length);
						}
					}
					Article article = new Article
					{
						Title = lines[i],
						Text = lines[i + 1],
						Date = DateTime.Now,
						Photo = photo
					};
					db.Articles.Add(article);
					++photoIndex;
				}
				await db.SaveChangesAsync();
			}
		}

	}
}
