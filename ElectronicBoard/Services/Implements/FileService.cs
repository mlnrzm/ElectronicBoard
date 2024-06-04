using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Файл"
	/// </summary>
	public class FileService : IFileService
    {
		/// <summary>
		/// Метод для получения списка файлов
		/// </summary>
		/// <returns></returns>
		public async Task<List<File>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Files.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка файлов по названию и Id элемента, к которому прикреплен файл
		/// </summary>
		/// <param name="name_element"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<List<File>> GetFilteredList(string name_element, int id)
        {
            if (id < 0)
            {
                return new List<File>();
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
                    return new List<File>();
			}
        }

		/// <summary>
		/// Метод для получения файла по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<File?> GetElement(File model)
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

		/// <summary>
		/// Метод для добавления файла
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(File model)
        {
            using var context = new ElectronicBoardDatabase();
            var file = await context.Files.AddAsync(CreateModel(model, new File()));
            await context.SaveChangesAsync();
		}

        /// <summary>
		/// Метод для редактирования файла
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Метод для удаления файла
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Метод для создания модели файла
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
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
    }
}
