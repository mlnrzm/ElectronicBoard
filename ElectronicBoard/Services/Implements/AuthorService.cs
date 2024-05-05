using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
    public class AuthorService : IAuthorService
    {
        // Получение всего списка авторов
        public async Task<List<Author>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Authors.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }
        // Получение авторов по id статьи
        public async Task<List<Author>> GetFilteredList(int ArticleId)
        {
            using var context = new ElectronicBoardDatabase();
            var article_aut = (await context.ArticleAuthors.ToListAsync()).Where(rec => rec.ArticleId == ArticleId).ToList();

            if (article_aut == null)
            {
                return null;
            }
            List<Author> authors = new List<Author>();
            foreach (var auth in article_aut)
            {
                var agg = await context.Authors.FirstOrDefaultAsync(rec => rec.Id == auth.AuthorId);
                authors.Add(agg);
            }
            return authors;
        }

        // Получение автора по id или ФИО
        public async Task<Author> GetElement(Author model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Authors
            .FirstOrDefaultAsync(rec => rec.ParticipantId == model.ParticipantId || rec.AuthorFIO.Contains(model.AuthorFIO) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

        // Добавление автора
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

        // Редактирование данных о статье
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

		// Удаление автора
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
