﻿using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ElectronicBoard.Services.Implements
{
    public class StickerService : IStickerService
    {
        // Получение всего списка стикеров
        public async Task<List<Sticker>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Stickers.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

        // Получение стикеров по id элемента (!!!)
        public async Task<List<Sticker>> GetFilteredList(string name_element, int id)
        {
            if (id < 0)
            {
                return null;
            }

            using var context = new ElectronicBoardDatabase();
            switch (name_element)
            {
                case "event":
                    return (await context.Stickers.ToListAsync())
                    .Where(rec => rec.EventId == id)
                    .Select(CreateModel)
                    .ToList();

                case "element":
                    return (await context.Stickers.ToListAsync())
                    .Where(rec => rec.SimpleElementId == id)
                    .Select(CreateModel)
                    .ToList();

                case "participant":
                    return (await context.Stickers.ToListAsync())
                    .Where(rec => rec.ParticipantId == id)
                    .Select(CreateModel)
                    .ToList();

                case "project":
                    return (await context.Stickers.ToListAsync())
                    .Where(rec => rec.ProjectId == id)
                    .Select(CreateModel)
                    .ToList();

                case "grant":
                    return (await context.Stickers.ToListAsync())
                    .Where(rec => rec.GrantId == id)
                    .Select(CreateModel)
                    .ToList();

                default:
                    return null;
            }
        }

        // Получение стикера по id
        public async Task<Sticker> GetElement(Sticker model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Stickers
            .FirstOrDefaultAsync(rec => rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

        // Добавление стикера
        public async Task Insert(Sticker model)
        {
            using var context = new ElectronicBoardDatabase();
            await context.Stickers.AddAsync(CreateModel(model, new Sticker()));
            await context.SaveChangesAsync();
        }

        // Редактирование данных о стикере
        public async Task Update(Sticker model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Stickers.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Стикер не найден");
            }
            CreateModel(model, element);
            await context.SaveChangesAsync();
        }

        // Удаление стикера
        public async Task Delete(Sticker model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Stickers.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Stickers.Remove(element);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Стикер не найден");
            }
        }
        private static Sticker CreateModel(Sticker model, Sticker sticker)
        {
            sticker.StickerDescription = model.StickerDescription;
            sticker.Picture = model.Picture.CloneByteArray();

            sticker.ParticipantId = model.ParticipantId;
            sticker.EventId = model.EventId;
            sticker.ProjectId = model.ProjectId;
            sticker.GrantId = model.GrantId;
            sticker.SimpleElementId = model.SimpleElementId;

            return sticker;
        }
        public Sticker CreateModel(Sticker sticker)
        {
            return new Sticker
            {
                Id = sticker.Id,

                StickerDescription = sticker.StickerDescription,
                Picture = sticker.Picture.CloneByteArray(),

                ParticipantId = sticker.ParticipantId,
                EventId = sticker.EventId,
                ProjectId = sticker.ProjectId,
                GrantId = sticker.GrantId,
                SimpleElementId = sticker.SimpleElementId
            };
        }
    }
}
