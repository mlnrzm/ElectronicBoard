using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Статья"
	/// </summary>
	public class ArticleService : IArticleService
    {
        private IAggregatorService aggregatorService { get; set; }
        private IAuthorService authorService { get; set; }
		private IFileService fileService { get; set; }

		public ArticleService(IAggregatorService _aggregatorService, IAuthorService _authorService, IFileService fileService)
		{
			aggregatorService = _aggregatorService;
			authorService = _authorService;
			this.fileService = fileService;
		}

		public ArticleService(){}

		/// <summary>
		/// Метод для получения списка статей
		/// </summary>
		/// <returns></returns>
		public async Task<List<Article>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
			var list = await context.Articles.ToListAsync();
			return list
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка статей по Id события
		/// </summary>
		/// <param name="event_id"></param>
		/// <returns></returns>
		public async Task<List<Article>> GetFilteredList(int event_id)
        {
            using var context = new ElectronicBoardDatabase();
            var list = await context.Articles.ToListAsync();
			return list
            .Where(rec => rec.EventId == event_id)
            .Select(CreateModel)
			.ToList();            
        }

		/// <summary>
		/// Метод для получения статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Article?> GetElement(Article model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
			var list = await context.Articles.ToListAsync();
            Article? component = null;
            if (model.Id > 0)
            {
                component = list
                    .FirstOrDefault(rec => rec.Id == model.Id);
            }
            else 
            {
				component = list
	                .FirstOrDefault(rec => rec.ArticleName.Contains(model.ArticleName) &&
	                rec.ArticlePlaceOfPublication.Contains(model.ArticlePlaceOfPublication) &&
	                rec.ArticleStatus == model.ArticleStatus);
			}

            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Article model)
        {
            using var context = new ElectronicBoardDatabase();
			var list = await context.Articles.ToListAsync();
			var component = list
	            .FirstOrDefault(rec => rec.ArticleName.Contains(model.ArticleName) && rec.EventId == model.EventId);
			if (component == null)
			{
				await context.Articles.AddAsync(CreateModel(model, new Article()));
				await context.SaveChangesAsync();
			}
			else throw new Exception("Статья с данным названием уже добавлена");
        }

		/// <summary>
		/// Метод для редактирования статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Article model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Articles.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.Articles.FirstOrDefaultAsync(rec => rec.ArticleName.Contains(model.ArticleName) && rec.EventId == model.EventId && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Статья не найдена");
            }
			if (elementName == null)
			{
				CreateModel(model, element);
				await context.SaveChangesAsync();
			}
			else throw new Exception("Статья с данным названием уже добавлена");
		}

		/// <summary>
		/// Метод для удаления статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Article model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Articles.FirstOrDefaultAsync(rec => rec.Id == model.Id);
                var aggregators = (await context.ArticleAggregators.ToListAsync()).Where(rec => rec.ArticleId == model.Id);
                var authors = (await context.ArticleAuthors.ToListAsync()).Where(rec => rec.ArticleId == model.Id);

                if (element != null)
                {
					// Удаление файлов
					var files = (await context.Files.ToListAsync())
						.Where(rec => rec.ArticleId == model.Id)
						.Select(fileService.CreateModel)
						.ToList();
					foreach (var file in files)
					{
						context.Files.Remove(file);
						await context.SaveChangesAsync();
					}

					// Удаление связи с агрегаторами
					foreach (var aggr in aggregators)
                    {
                        context.ArticleAggregators.Remove(aggr);
						await context.SaveChangesAsync();
                    }

                    // Удаление связи с авторами
                    foreach (var auth in authors)
                    {
                        context.ArticleAuthors.Remove(auth);
                        await context.SaveChangesAsync();
                    }

                    // Удаление участника
                    context.Articles.Remove(element);
					await context.SaveChangesAsync();
				}
                else
                {
                    throw new Exception("Участник не найден");
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

		/// <summary>
		/// Метод для прикрепления/открепления агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <param name="article_id"></param>
		/// <returns></returns>
		public async Task GetAggregator(Aggregator model, int article_id)
        {
            using var context = new ElectronicBoardDatabase();
            var this_aggregator = await context.Aggregators.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (this_aggregator == null)
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    Aggregator aggr_to_add = aggregatorService.CreateModel(model, new Aggregator());
                    await context.Aggregators.AddAsync(aggr_to_add);
                    await context.SaveChangesAsync();

                    await context.ArticleAggregators.AddAsync(new ArticleAggregator
                    {
                        AggregatorId = aggr_to_add.Id,
                        ArticleId = article_id
                    });
					await context.SaveChangesAsync();
					await transaction.CommitAsync();
                }
                catch
                {
					await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                var art_aggreg = await context.ArticleAggregators
                    .FirstOrDefaultAsync(rec => rec.AggregatorId == model.Id
                    && rec.ArticleId == article_id);
                if (art_aggreg == null)
                {
					await context.ArticleAggregators.AddAsync(new ArticleAggregator
                    {
                        AggregatorId = model.Id,
                        ArticleId = article_id
                    });
					await context.SaveChangesAsync();
                }
                else
                {
					context.ArticleAggregators.Remove(art_aggreg);
					await context.SaveChangesAsync();
                }
            }
        }

		/// <summary>
		/// Метод для прикрепления/открепления автора
		/// </summary>
		/// <param name="model"></param>
		/// <param name="article_id"></param>
		/// <returns></returns>
		public async Task GetAuthor(Author model, int article_id)
        {
            using var context = new ElectronicBoardDatabase();
            var this_author = await context.Authors.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (this_author == null)
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    Author auth_to_add = authorService.CreateModel(model, new Author());
					await context.Authors.AddAsync(auth_to_add);
					await context.SaveChangesAsync();

					await context.ArticleAuthors.AddAsync(new ArticleAuthor
                    {
                        AuthorId = auth_to_add.Id,
                        ArticleId = article_id
                    });
					await context.SaveChangesAsync();

					await transaction.CommitAsync();
                }
                catch
                {
					await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                var art_auth = await context.ArticleAuthors
                    .FirstOrDefaultAsync(rec => rec.AuthorId == model.Id
                    && rec.ArticleId == article_id);
                if (art_auth == null)
                {
					await context.ArticleAuthors.AddAsync(new ArticleAuthor
                    {
                        AuthorId = model.Id,
                        ArticleId = article_id
                    });
					await context.SaveChangesAsync();
				}
                else
                {
					context.ArticleAuthors.Remove(art_auth);
					await context.SaveChangesAsync();
				}
            }
        }

		/// <summary>
		/// Метод для создания модели статьи
		/// </summary>
		/// <param name="article"></param>
		/// <returns></returns>
		public Article CreateModel(Article model, Article article)
        {
            article.ArticleName = model.ArticleName;
            article.ArticleText = model.ArticleText;
            article.ArticleKeyWords = model.ArticleKeyWords;
            article.ArticleAnnotation = model.ArticleAnnotation;
            article.ArticlePlaceOfPublication = model.ArticlePlaceOfPublication;
            article.EventId = model.EventId;
            article.ArticleStatus = model.ArticleStatus;
            article.Picture = model.Picture;

            article.Files = model.Files;
            article.ArticleAuthors = model.ArticleAuthors;
            article.ArticleAggregators = model.ArticleAggregators;

            return article;
        }
        public Article CreateModel(Article article)
        {
            return new Article
            {
                Id = article.Id,

                ArticleName = article.ArticleName,
                ArticleText = article.ArticleText,
                ArticleKeyWords = article.ArticleKeyWords,
                ArticleAnnotation = article.ArticleAnnotation,
                ArticlePlaceOfPublication = article.ArticlePlaceOfPublication,
                EventId = article.EventId,
                ArticleStatus = article.ArticleStatus,
                Picture = article.Picture,

                Files = article.Files,
                ArticleAuthors = article.ArticleAuthors,
                ArticleAggregators = article.ArticleAggregators
            };
        }

		/// <summary>
		/// Метод для получения списка статей автора
		/// </summary>
		/// <param name="author_id"></param>
		/// <returns></returns>
		public async Task<List<Article>> GetArticlesAuthor(int author_id)
		{
            List<Article> find_articles = new List<Article>();

			using var context = new ElectronicBoardDatabase();
            var author = await context.Authors.FirstOrDefaultAsync(rec => rec.Id == author_id);
            if (author != null)
            {
                List<ArticleAuthor> articles = (await context.ArticleAuthors.ToListAsync()).Where(rec => rec.AuthorId == author_id).ToList();

                foreach (var art in articles)
                {
                    Article? article = await context.Articles.FirstOrDefaultAsync(rec => rec.Id == art.ArticleId);
                    if (article != null) { find_articles.Add(article); }
                }
            }
            else throw new Exception("Автор не найден");
            return find_articles;
		}
	}
}
