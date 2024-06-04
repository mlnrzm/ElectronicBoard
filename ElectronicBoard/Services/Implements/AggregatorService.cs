using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Агрегатор"
	/// </summary>
	public class AggregatorService : IAggregatorService
    {
		/// <summary>
		/// Метод для получения списка агрегаторов
		/// </summary>
		/// <returns></returns>
		public async Task<List<Aggregator>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Aggregators.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка агрегаторов по Id статьи
		/// </summary>
		/// <param name="ArticleId"></param>
		/// <returns></returns>
		public async Task<List<Aggregator>> GetFilteredList(int ArticleId)
        {
            using var context = new ElectronicBoardDatabase();
            var article_aggr = (await context.ArticleAggregators.ToListAsync()).Where(rec => rec.ArticleId == ArticleId).ToList();

            if (article_aggr == null)
            {
                return new List<Aggregator>();
            }
            List<Aggregator> aggregators = new List<Aggregator>();
            foreach (var aggr in article_aggr)
            {
                var agg = await context.Aggregators.FirstOrDefaultAsync(rec => rec.Id == aggr.AggregatorId);
                if (agg != null) aggregators.Add(agg);
            }
            return aggregators;
        }

		/// <summary>
		/// Метод для получения агрегатора по Id или наименованию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Aggregator?> GetElement(Aggregator model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Aggregators
            .FirstOrDefaultAsync(rec => rec.AggregatorName.Contains(model.AggregatorName) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Aggregator model)
        {
            using var context = new ElectronicBoardDatabase();
			var component = await context.Aggregators
	            .FirstOrDefaultAsync(rec => rec.AggregatorName.Contains(model.AggregatorName));
			if (component == null)
			{

				await context.Aggregators.AddAsync(CreateModel(model, new Aggregator()));
				await context.SaveChangesAsync();
			}
			else throw new Exception("Агрегатор уже существует");
        }

		/// <summary>
		/// Метод для редактирования агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Aggregator model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Aggregators.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.Aggregators.FirstOrDefaultAsync(rec => rec.AggregatorName.Contains(model.AggregatorName) && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Агрегатор не найден");
            }
			if (elementName == null)
			{
				CreateModel(model, element);
				await context.SaveChangesAsync();
			}
			else throw new Exception("Такой агрегатор уже существует");
		}

		/// <summary>
		/// Метод для удаления агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Aggregator model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Aggregators.FirstOrDefaultAsync(rec => rec.Id == model.Id);
                if (element != null)
                {
                    var article = await context.ArticleAggregators.FirstOrDefaultAsync(rec => rec.AggregatorId == model.Id);
                    if (article == null)
                    {
                        context.Aggregators.Remove(element);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Агрегатор не может быть удалён");
                    }
                }
                else
                {
                    throw new Exception("Агрегатор не найден");
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
		/// Метод для создания модели агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Aggregator CreateModel(Aggregator model, Aggregator aggregator)
        {
            aggregator.AggregatorName = model.AggregatorName;
            aggregator.AggregatorArticles = model.AggregatorArticles;

            return aggregator;
        }
        private static Aggregator CreateModel(Aggregator aggregator)
        {
            return new Aggregator
            {
                Id = aggregator.Id,

                AggregatorName = aggregator.AggregatorName,
                AggregatorArticles = aggregator.AggregatorArticles
            };
        }
    }
}