using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Автор"
	/// </summary>
	public class AuthorService : IAuthorService
    {
		/// <summary>
		/// Метод для получения списка авторов
		/// </summary>
		/// <returns></returns>
		public async Task<List<Author>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Authors.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка авторов по Id статьи
		/// </summary>
		/// <param name="ArticleId"></param>
		/// <returns></returns>
		public async Task<List<Author>> GetFilteredList(int ArticleId)
        {
            using var context = new ElectronicBoardDatabase();
            var article_aut = (await context.ArticleAuthors.ToListAsync()).Where(rec => rec.ArticleId == ArticleId).ToList();

            if (article_aut == null)
            {
                return new List<Author>();
            }
            List<Author> authors = new List<Author>();
            foreach (var auth in article_aut)
            {
                var agg = await context.Authors.FirstOrDefaultAsync(rec => rec.Id == auth.AuthorId);
                if (agg != null) authors.Add(agg);
            }
            return authors;
        }

		/// <summary>
		/// Метод для получения автора по Id или ФИО
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Author?> GetElement(Author model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            Author? component = null;
            if (model.Id > 0)
            {
                component = await context.Authors
                    .FirstOrDefaultAsync(rec => rec.Id == model.Id);
            }
            else if (!string.IsNullOrEmpty(model.AuthorFIO)) 
            {
				component = await context.Authors.FirstOrDefaultAsync(rec => rec.AuthorFIO.Contains(model.AuthorFIO) && rec.ParticipantId == model.ParticipantId);
			}
            else
            {
                component = await context.Authors.FirstOrDefaultAsync(rec => rec.ParticipantId == model.ParticipantId);
            }
			return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления автора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Author model)
        {
            using var context = new ElectronicBoardDatabase();
			var component = await context.Authors
	            .FirstOrDefaultAsync(rec => rec.AuthorFIO.Contains(model.AuthorFIO));
			if (component == null)
			{
				await context.Authors.AddAsync(CreateModel(model, new Author()));
				await context.SaveChangesAsync();
			}
			else throw new Exception("Автор " + model.AuthorFIO + " уже добавлен");
        }

		/// <summary>
		/// Метод для редактирования автора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Author model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Authors.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.Authors.FirstOrDefaultAsync(rec => rec.AuthorFIO.Contains(model.AuthorFIO) && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Автор не найден");
            }
			if (elementName == null)
			{
				CreateModel(model, element);
				await context.SaveChangesAsync();
			}
			else throw new Exception("Автор " + model.AuthorFIO + " уже существует");
		}

		/// <summary>
		/// Метод для удаления автора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Author model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Authors.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            var articles = await context.ArticleAuthors.FirstOrDefaultAsync(rec => rec.AuthorId == model.Id);
            if (element != null && articles == null)
            {
                context.Authors.Remove(element);
                await context.SaveChangesAsync();
            }
            else if (articles != null)
            {
                throw new Exception("Автор не может быть удалён");

            }
            else
            {
                throw new Exception("Автор не найден");
            }
        }

		/// <summary>
		/// Метод для создания модели автора
		/// </summary>
		/// <param name="model"></param>
		/// <param name="author"></param>
		/// <returns></returns>
		public Author CreateModel(Author model, Author author)
        {
            author.AuthorFIO = model.AuthorFIO;
            author.AuthorEmail = model.AuthorEmail;
            author.AuthorOrganization = model.AuthorOrganization;
            author.ParticipantId = model.ParticipantId;

            author.Participant = model.Participant;
            author.AuthorArticles = model.AuthorArticles;

            return author;
        }
        private static Author CreateModel(Author author)
        {
            return new Author
            {
                Id = author.Id,

                AuthorFIO = author.AuthorFIO,
                AuthorEmail = author.AuthorEmail,
                AuthorOrganization = author.AuthorOrganization,
                ParticipantId = author.ParticipantId,

                Participant = author.Participant,
                AuthorArticles = author.AuthorArticles
            };
        }
    }
}
