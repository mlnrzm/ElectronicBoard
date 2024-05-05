using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Services.Implements
{
    public class FileService : IFileService
    {
        // Получение всего списка файлов
        public async Task<List<File>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Files.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

        // Получение файлов по id элемента
        public async Task<List<File>> GetFilteredList(string name_element, int id)
        {
            if (id < 0)
            {
                return null;
            }

            using var context = new ElectronicBoardDatabase();
            switch (name_element)
            {
                case "event":
                    return (await context.Files.ToListAsync())
                    .Where(rec => rec.EventId == id)
                    .Select(CreateModel)
                    .ToList();

                case "element":
                    return (await context.Files.ToListAsync())
                    .Where(rec => rec.SimpleElementId == id)
                    .Select(CreateModel)
                    .ToList();

                case "participant":
                    return (await context.Files.ToListAsync())
                    .Where(rec => rec.ParticipantId == id)
                    .Select(CreateModel)
                    .ToList();

                case "article":
                    return (await context.Files.ToListAsync())
                    .Where(rec => rec.ArticleId == id)
                    .Select(CreateModel)
                    .ToList();

                case "project":
                    return (await context.Files.ToListAsync())
                    .Where(rec => rec.ProjectId == id)
                    .Select(CreateModel)
                    .ToList();

                case "stage":
                    return (await context.Files.ToListAsync())
                    .Where(rec => rec.StageId == id)
                    .Select(CreateModel)
                    .ToList();

                case "grant":
                    return (await context.Files.ToListAsync())
                    .Where(rec => rec.GrantId == id)
                    .Select(CreateModel)
                    .ToList();

                default:
                    return null;
            }
        }

        // Получение файла по id или названию
        public async Task<File> GetElement(File model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Files
            .FirstOrDefaultAsync(rec => (rec.FileName.Contains(model.FileName) && rec.Path.Contains(model.Path)) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

        // Добавление файла
        public async Task Insert(File model)
        {
            using var context = new ElectronicBoardDatabase();
            var file = await context.Files.AddAsync(CreateModel(model, new File()));
            await context.SaveChangesAsync();
        }

        // Редактирование данных о файле
        public async Task Update(File model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Files.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Файл не найден");
            }
            CreateModel(model, element);
            await context.SaveChangesAsync();
        }

        // Удаление элемента
        public async Task Delete(File model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Files.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Files.Remove(element);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Файл не найден");
            }
        }
        private static File CreateModel(File model, File file)
        {
            file.FileName = model.FileName;
            file.Path = model.Path;
            file.ContentType = model.ContentType;

            file.StageId = model.StageId;
            file.EventId = model.EventId;
            file.ArticleId = model.ArticleId;
            file.ParticipantId = model.ParticipantId;
            file.GrantId = model.GrantId;
            file.ProjectId = model.ProjectId;
            file.SimpleElementId = model.SimpleElementId;

            return file;
        }
        public File CreateModel(File file)
        {
            return new File
            {
                Id = file.Id,

                FileName = file.FileName,
                Path = file.Path,
                ContentType = file.ContentType,

                StageId = file.StageId,
                EventId = file.EventId,
                ArticleId = file.ArticleId,
                ParticipantId = file.ParticipantId,
                GrantId = file.GrantId,
                ProjectId = file.ProjectId,
                SimpleElementId = file.SimpleElementId
            };
        }
    }
}
