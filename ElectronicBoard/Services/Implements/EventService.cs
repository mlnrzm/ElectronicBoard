﻿using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Мероприятие"
	/// </summary>
	public class EventService : IEventService
    {
        private IArticleService articleService { get; set; }
        private IFileService fileService { get; set; }
		private IStickerService stickerService { get; set; }

		public EventService(IArticleService _articleService, IFileService _fileService, IStickerService _stickerService)
        {
            articleService = _articleService;
            fileService = _fileService;
            stickerService = _stickerService;
        }

		public EventService() {}

		/// <summary>
		/// Метод для получения списка мероприятий
		/// </summary>
		/// <returns></returns>
		public async Task<List<Event>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Events.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка мероприятий по Id блока
		/// </summary>
		/// <param name="model"></param>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public async Task<List<Event>> GetFilteredList(int? blockId)
        {
            List<Event> events = new List<Event>();
            using var context = new ElectronicBoardDatabase();
            if (blockId == null)
            {
                return new List<Event>();
            }
            else
            {
                var block_events = (await context.BlockEvents.ToListAsync()).Where(rec => rec.BlockId == blockId);
                if (block_events != null)
                {
                    foreach (var block_event in block_events)
                    {
                        var event_ = await context.Events.FirstOrDefaultAsync(rec => rec.Id == block_event.EventId);
                        if (event_ != null) events.Add(event_);
                    }
                }
                return events;
            }
        }

		/// <summary>
		/// Метод для получения мероприятия по Id или наименованию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Event?> GetElement(Event model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Events
            .FirstOrDefaultAsync(rec => rec.EventName.Contains(model.EventName) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления мероприятия
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Event model)
        {
            var ev = await GetElement(model);
            if (ev == null)
            {
                using var context = new ElectronicBoardDatabase();
                await context.Events.AddAsync(CreateModel(model, new Event()));
                await context.SaveChangesAsync();
            }
            else throw new Exception("Мероприятие с таким наименованием уже существует");
        }

		/// <summary>
		/// Метод для редактирования мероприятия
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Event model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Events.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.Events.FirstOrDefaultAsync(rec => rec.EventName.Contains(model.EventName) && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Событие не найдено");
			}
			if (elementName == null)
			{
				CreateModel(model, element);
				await context.SaveChangesAsync();
			}
			else throw new Exception("Мероприятие с таким наименованием уже существует");
        }

		/// <summary>
		/// Метод для удаления мероприятия
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Event model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Events.FirstOrDefaultAsync(rec => rec.Id == model.Id);
                if (element != null)
                {
                    // Удаление файлов
                    var files = (await context.Files.ToListAsync())
                        .Where(rec => rec.ParticipantId == model.Id)
                        .Select(fileService.CreateModel)
                        .ToList();
                    foreach (var file in files)
                    {
                        context.Files.Remove(file);
                        await context.SaveChangesAsync();
                    }

                    // Удаление стикеров
                    var stickers = (await context.Stickers.ToListAsync())
                        .Where(rec => rec.ParticipantId == model.Id)
                        .Select(stickerService.CreateModel)
                        .ToList();
                    foreach (var sticker in stickers)
                    {
                        context.Stickers.Remove(sticker);
                        await context.SaveChangesAsync();
                    }

                    // Удаление статей
                    var articles = (await context.Articles.ToListAsync())
                        .Where(rec => rec.EventId == model.Id)
                        .Select(articleService.CreateModel)
                        .ToList();
                    foreach (var article in articles)
                    {
                        await articleService.Delete(article);
                        await context.SaveChangesAsync();
                    }

                    // Удаление сущностей-связей
                    foreach (var article in articles)
                    {
                        foreach (var aggrart in
                            (await context.ArticleAggregators.ToListAsync())
                            .Where(rec => rec.ArticleId == model.Id))
                        {
                            context.ArticleAggregators.Remove(aggrart);
                            await context.SaveChangesAsync();
                        }

                        foreach (var authart in
                            (await context.ArticleAuthors.ToListAsync())
                            .Where(rec => rec.ArticleId == model.Id))
                        {
                            context.ArticleAuthors.Remove(authart);
                            await context.SaveChangesAsync();
                        }

                        context.Articles.Remove(article);
                        await context.SaveChangesAsync();
                    }

                    // Удаление события
                    context.Events.Remove(element);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Событие не найдено");
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public Event CreateModel(Event model, Event event_)
        {
            event_.EventName = model.EventName;
            event_.EventPlace = model.EventPlace;
            event_.EventText = model.EventText;
            event_.EventDateStart = model.EventDateStart;
            event_.EventDateFinish = model.EventDateFinish;
            event_.EventDateFinishArticle = model.EventDateFinishArticle;
            event_.EventFinishColor = model.EventFinishColor;
            event_.EventFinishArticleColor = model.EventFinishArticleColor;
            event_.EventStartColor = model.EventStartColor;

            event_.Files = model.Files;
            event_.Picture = model.Picture.CloneByteArray();
            event_.Stikers = model.Stikers;

            return event_;
        }
        private static Event CreateModel(Event event_)
        {
            return new Event
            {
                Id = event_.Id,

                EventName = event_.EventName,
                EventPlace = event_.EventPlace,
                EventText = event_.EventText,
                EventDateStart = event_.EventDateStart,
                EventDateFinish = event_.EventDateFinish,
                EventDateFinishArticle = event_.EventDateFinishArticle,
                EventFinishColor = event_.EventFinishColor,
                EventFinishArticleColor = event_.EventFinishArticleColor,
                EventStartColor = event_.EventStartColor,

                Files = event_.Files,
                Picture = event_.Picture.CloneByteArray(),
                Stikers = event_.Stikers
            };
        }
    }
}
