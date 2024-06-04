using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Этап"
	/// </summary>
	public class StageService : IStageService
    {
		private IFileService fileService { get; set; }
        public StageService(IFileService _fileService) 
        {
            fileService = _fileService;
        }

		public StageService() {}

		/// <summary>
		/// Метод для получения списка этапов
		/// </summary>
		/// <returns></returns>
		public async Task<List<Stage>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Stages.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка этапов по Id проекта
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public async Task<List<Stage>> GetFilteredList(int ProjectId)
        {
            if (ProjectId < 1)
            {
                return new List<Stage>();
            }
            using var context = new ElectronicBoardDatabase();
            return (await context.Stages.ToListAsync())
            .Where(rec => rec.ProjectId == ProjectId)
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения этапа по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Stage?> GetElement(Stage model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Stages
            .FirstOrDefaultAsync(rec => rec.StageName.Contains(model.StageName) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления этапа
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Stage model)
        {
            using var context = new ElectronicBoardDatabase();
			var component = await context.Stages
	               .FirstOrDefaultAsync(rec => rec.StageName.Contains(model.StageName) && rec.ProjectId == model.ProjectId);
            if (component == null)
            {
				await context.Stages.AddAsync(CreateModel(model, new Stage()));
				await context.SaveChangesAsync();
			}
			else throw new Exception("В проекте уже есть этап с таким названием");
		}

		/// <summary>
		/// Метод для редактирования этапа
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Stage model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Stages.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.Stages.FirstOrDefaultAsync(rec => rec.StageName.Contains(model.StageName) && rec.ProjectId == model.ProjectId && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Этап не найден");
            }
			if (elementName == null)
			{
				CreateModel(model, element);
                await context.SaveChangesAsync();
		    }
            else throw new Exception("В проекте уже есть этап с таким названием");
	    }

		/// <summary>
		/// Метод для удаления этапа
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Stage model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Stages.FirstOrDefaultAsync(rec => rec.Id == model.Id);

                if (element != null)
                {
                    // Удаление файлов
                    var files = (await context.Files.ToListAsync())
                        .Where(rec => rec.StageId == model.Id)
                        .Select(fileService.CreateModel)
                        .ToList();
                    foreach (var file in files)
                    {
                        context.Files.Remove(file);
                        await context.SaveChangesAsync();
                    }

                    // Удаление этапа
                    context.Stages.Remove(element);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Этап не найден");
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public Stage CreateModel(Stage model, Stage stage)
        {
            stage.ProjectId = model.ProjectId;
            stage.StageName = model.StageName;
            stage.StageText = model.StageText;
            stage.DateStart = model.DateStart;
            stage.DateFinish = model.DateFinish;
            stage.StageDescription = model.StageDescription;
            stage.Status = model.Status;
            stage.Picture = model.Picture.CloneByteArray();

            stage.Files = model.Files;

            return stage;
        }
        private static Stage CreateModel(Stage stage)
        {
            return new Stage
            {
                Id = stage.Id,

                ProjectId = stage.ProjectId,
                StageName = stage.StageName,
                StageText = stage.StageText,
                DateStart = stage.DateStart,
                DateFinish = stage.DateFinish,
                StageDescription = stage.StageDescription,
                Status = stage.Status,
                Picture = stage.Picture.CloneByteArray(),

                Files = stage.Files
            };
        }
    }
}
