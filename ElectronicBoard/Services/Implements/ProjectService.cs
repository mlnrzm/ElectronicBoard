using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Проект"
	/// </summary>
	public class ProjectService : IProjectService
    {
        private IStageService stageService { get; set; }
		private IFileService fileService { get; set; }
        private IStickerService stickerService { get; set; }

		public ProjectService(IStageService _stageService, IFileService _fileService, IStickerService _stickerService)
		{
			stageService = _stageService;
			fileService = _fileService;
            stickerService = _stickerService;
		}

		public ProjectService() {}

		/// <summary>
		/// Метод для получения списка проектов
		/// </summary>
		/// <returns></returns>
		public async Task<List<Project>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Projects.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка проектов по Id блока
		/// </summary>
		/// <param name="BlockId"></param>
		/// <returns></returns>
		public async Task<List<Project>> GetFilteredList(int BlockId)
        {
            if (BlockId < 0)
            {
                return new List<Project>();
            }
            using var context = new ElectronicBoardDatabase();
            return (await context.Projects.ToListAsync())
            .Where(rec => rec.BlockId == BlockId)
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения проекта по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Project?> GetElement(Project model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();

            Project? component = null;
			if (model.Id > 0) 
            {
				component = await context.Projects
                    .FirstOrDefaultAsync(rec => rec.ProjectName.Contains(model.ProjectName) || rec.Id == model.Id);
			}
            else if (!string.IsNullOrEmpty(model.ProjectName) && model.BlockId > 0)
            {
				component = await context.Projects
                    .FirstOrDefaultAsync(rec => rec.ProjectName.Contains(model.ProjectName) && rec.BlockId == model.BlockId);
			}

            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления проекта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Project model)
        {
            using var context = new ElectronicBoardDatabase();
			var component = await context.Projects
	            .FirstOrDefaultAsync(rec => rec.ProjectName.Contains(model.ProjectName) && rec.BlockId == model.BlockId);
			if (component == null)
			{
				await context.Projects.AddAsync(CreateModel(model, new Project()));
                await context.SaveChangesAsync();
			}
			else throw new Exception("Проект с таким названием уже существует");
		}

		/// <summary>
		/// Метод для редактирования проекта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Project model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Projects.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.Projects.FirstOrDefaultAsync(rec => rec.ProjectName.Contains(model.ProjectName) && rec.BlockId == model.BlockId && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Проект не найден");
            }
			if (elementName == null)
			{
				CreateModel(model, element);
				await context.SaveChangesAsync();
			}
			else throw new Exception("Проект с таким названием уже существует");
        }

		/// <summary>
		/// Метод для удаления проекта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Project model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Projects.FirstOrDefaultAsync(rec => rec.Id == model.Id);
                if (element != null)
                {
                    // Удаление файлов
                    var files = (await context.Files.ToListAsync())
                        .Where(rec => rec.ProjectId == model.Id)
                        .Select(fileService.CreateModel)
                        .ToList();
                    foreach (var file in files)
                    {
                        context.Files.Remove(file);
                        await context.SaveChangesAsync();
                    }

                    // Удаление стикеров
                    var stickers = (await context.Stickers.ToListAsync())
                        .Where(rec => rec.ProjectId == model.Id)
                        .Select(stickerService.CreateModel)
                        .ToList();
                    foreach (var sticker in stickers)
                    {
                        context.Stickers.Remove(sticker);
                        await context.SaveChangesAsync();
                    }

                    // Удаление ответственных за проект
                    var respons = (await context.ProjectParticipants.ToListAsync()).Where(rec => rec.ProjectId == model.Id).ToList();
                    foreach (var respon in respons)
                    {
                        context.ProjectParticipants.Remove(respon);
                        await context.SaveChangesAsync();
                    }

                    // Удаление этапов проекта
                    var stages = (await context.Stages.ToListAsync()).Where(rec => rec.ProjectId == model.Id).ToList();
                    foreach (var stage in stages)
                    {
                        await stageService.Delete(stage);
                    }

                    // Удаление участника
                    context.Projects.Remove(element);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Проект не найден");
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
		/// Метод для прикрепления/открепления ответственного
		/// </summary>
		/// <param name="model"></param>
		/// <param name="project_id"></param>
		/// <returns></returns>
		public async Task GetResponsable(Participant model, int project_id)
        {
            using var context = new ElectronicBoardDatabase();
            var this_part = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            var this_project = await context.Projects.FirstOrDefaultAsync(rec => rec.Id == project_id);

            if (this_part != null && this_project != null)
            {
                var project_part = await context.ProjectParticipants
                    .FirstOrDefaultAsync(rec => rec.ParticipantId == model.Id
                    && rec.ProjectId == project_id);
                if (project_part == null)
                {
                    await context.ProjectParticipants.AddAsync(new ProjectParticipant
                    {
                        ParticipantId = model.Id,
                        ProjectId = project_id
                    });
                    await context.SaveChangesAsync();
                }
                else
                {
                    context.ProjectParticipants.Remove(project_part);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                throw new Exception("Назначить ответственного не удалось");
            }
        }
        public Project CreateModel(Project model, Project project)
        {
            project.ProjectName = model.ProjectName;
            project.ProjectText = model.ProjectText;
            project.ProjectDescription = model.ProjectDescription;
            project.BlockId = model.BlockId;

            project.Picture = model.Picture.CloneByteArray();
            project.Files = model.Files;
            project.Stikers = model.Stikers;
            project.ProjectParticipants = model.ProjectParticipants;
            project.Stages = model.Stages;

            return project;
        }
        private static Project CreateModel(Project project)
        {
            return new Project
            {
                Id = project.Id,

                ProjectName = project.ProjectName,
                ProjectText = project.ProjectText,
                ProjectDescription = project.ProjectDescription,
                BlockId = project.BlockId,
                Picture = project.Picture.CloneByteArray(),

                Files = project.Files,
                Stikers = project.Stikers,
                ProjectParticipants = project.ProjectParticipants,
                Stages = project.Stages
            };
        }
    }
}
