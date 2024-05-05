using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Controllers
{
	public class FileController : Controller
	{
		IWebHostEnvironment Environment;
		private readonly ILogger<StickerController> _logger;
		private readonly INotyfService _notyf;

		private readonly IFileService fileService;

		private readonly IProjectService projectService;
		private readonly ISimpleElementService elementService;
		private readonly IEventService eventService;
		private readonly IParticipantService participantService;
		private readonly IGrantService grantService;
		private readonly IArticleService articleService;
		private readonly IStageService stageService;

		private readonly IBlockService blockService;

		public FileController(IWebHostEnvironment appEnvironment, ILogger<StickerController> logger, INotyfService notyf, IFileService _fileService,
			IProjectService _projectService, ISimpleElementService _elementService, IArticleService _articleService, IStageService _stageService,
			IEventService _eventService, IParticipantService _participantService, IGrantService _grantService,
			IBlockService _blockService)
		{
			_logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			projectService = _projectService;
			elementService = _elementService;
			eventService = _eventService;
			participantService = _participantService;
			grantService = _grantService;
			fileService = _fileService;
			articleService = _articleService;
			stageService = _stageService;
			Environment = appEnvironment;
		}

		// Добавление файла
		[HttpGet]
		public IActionResult AddFile(string blockId,
			string projectId, string elementId, string eventId, string partId, string grantId, string stageId, string articleId)
		{
			if (string.IsNullOrEmpty(projectId) && string.IsNullOrEmpty(elementId) && string.IsNullOrEmpty(eventId)
				&& string.IsNullOrEmpty(partId) && string.IsNullOrEmpty(grantId) && string.IsNullOrEmpty(stageId) && string.IsNullOrEmpty(articleId))
			{
				_notyf.Error("Ошибка добавления файла");
				Response.Redirect($"/block/index?Id=" + blockId);
			}
			else
			{
				ViewData["projectId"] = projectId;
				ViewData["elementId"] = elementId;
				ViewData["eventId"] = eventId;
				ViewData["partId"] = partId;
				ViewData["grantId"] = grantId;
				ViewData["stageId"] = stageId;
				ViewData["articleId"] = articleId;
			}
			// Передача id блока, на котором будет находится элемент c файлом
			ViewData["blockId"] = blockId;
			return View();
		}
		[HttpPost]
		public async Task AddFile(string blockId, IFormFile file,
			string projectId, string elementId, string eventId, string partId, string grantId, string stageId, string articleId)
		{
			if (file != null)
			{
				// ID блока
				int BlockId = Convert.ToInt32(blockId);

				int? ProjectId = null;
				int? ElementId = null;
				int? EventId = null;
				int? GrantId = null;
				int? PartId = null;
				int? StageId = null;
				int? ArticleId = null;

				if (Convert.ToInt32(projectId) != 0) ProjectId = Convert.ToInt32(projectId);
				if (Convert.ToInt32(elementId) != 0) ElementId = Convert.ToInt32(elementId);
				if (Convert.ToInt32(eventId) != 0) EventId = Convert.ToInt32(eventId);
				if (Convert.ToInt32(grantId) != 0) GrantId = Convert.ToInt32(grantId);
				if (Convert.ToInt32(partId) != 0) PartId = Convert.ToInt32(partId);
				if (Convert.ToInt32(stageId) != 0) StageId = Convert.ToInt32(stageId);
				if (Convert.ToInt32(articleId) != 0) ArticleId = Convert.ToInt32(articleId);

				try
				{
					//ТРАНЗАКЦИЯ

					// Добавление файла в БД
					await fileService.Insert(new File
					{
						FileName = file.FileName,
						Path = "",
						ContentType = file.ContentType,
						EventId = EventId,
						ProjectId = ProjectId,
						SimpleElementId = ElementId,
						GrantId = GrantId,
						ParticipantId = PartId,
						StageId = StageId,
						ArticleId = ArticleId
					});
					// Получение сущности добавленного файла
					File add_file = await fileService.GetElement(new File
					{
						FileName = file.FileName,
						Path = "",
						ContentType = file.ContentType,
						EventId = EventId,
						ProjectId = ProjectId,
						SimpleElementId = ElementId,
						GrantId = GrantId,
						ParticipantId = PartId,
						StageId = StageId,
						ArticleId = ArticleId
					});

					// Сохранение файла в папку Files с названием (id сущности файла)
					string wwwPath = this.Environment.WebRootPath;
					string contentPath = this.Environment.ContentRootPath;

					string path = Path.Combine(this.Environment.WebRootPath, "Files");
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					string[] name = file.FileName.Split('.');
					string fileName = add_file.Id + "." + name[name.Length - 1];
					using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
					{
						file.CopyTo(stream);
					}

					// Редактирование сущности файла с добавлением пути файла в папке
					await fileService.Update(new File 
					{
						Id = add_file.Id,
						FileName = file.FileName,
						Path = Path.Combine(path, fileName),
						ContentType = file.ContentType,
						EventId = EventId,
						ProjectId = ProjectId,
						SimpleElementId = ElementId,
						GrantId = GrantId,
						ParticipantId = PartId,
						StageId = StageId,
						ArticleId = ArticleId
					});

					// КОНЕЦ ТРАНЗАКЦИИ

					// Отображение соответствующего элемента
					if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
					else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
					else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={BlockId}"); }
					else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
					else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={BlockId}"); }
					else if (StageId != null) { Response.Redirect($"/stage/index?stageId={StageId}"); }
					else if (ArticleId != null) { Response.Redirect($"/article/index?articleId={ArticleId}&blockId={BlockId}"); }
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
					else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
					else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={BlockId}"); }
					else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
					else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={BlockId}"); }
					else if (StageId != null) { Response.Redirect($"/stage/index?stageId={StageId}"); }
					else if (ArticleId != null) { Response.Redirect($"/article/index?articleId={ArticleId}&blockId={BlockId}"); }
					else Response.Redirect($"/block/index?Id=" + blockId);
				}	
			}
			else
			{
				_notyf.Error("Загрузите файл");
				Response.Redirect($"/file/addfile?blockId={blockId}" +
					$"&projectId={projectId}&elementId={elementId}&eventId={eventId}&partId={partId}&grantId={grantId}&stageId={stageId}&articleId={articleId}");
			}
		}

		// Удаление файла
		[HttpGet]
		public async Task DeleteFile(string id, string blockId)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId))
			{
				int block_id;
				int _id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_Id = int.TryParse(id, out _id);

				if (isNumeric_blockId && isNumeric_Id)
				{
					File file = await fileService.GetElement(new File { Id = _id });
					int? ProjectId = file.ProjectId;
					int? ElementId = file.SimpleElementId;
					int? EventId = file.EventId;
					int? GrantId = file.GrantId;
					int? PartId = file.ParticipantId;
					int? StageId = file.StageId;
					int? ArticleId = file.ArticleId;
					try
					{
						await fileService.Delete(new File { Id = _id });
						// Удаление файла из каталога
						System.IO.File.Delete(file.Path);

						// Отображение соответствующего элемента
						if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
						else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
						else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={block_id}"); }
						else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
						else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={block_id}"); }
						else if (StageId != null) { Response.Redirect($"/stage/index?stageId={StageId}"); }
						else if (ArticleId != null) { Response.Redirect($"/article/index?articleId={ArticleId}&blockId={block_id}"); }
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
						else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
						else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={block_id}"); }
						else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
						else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={block_id}"); }
						else if (StageId != null) { Response.Redirect($"/stage/index?stageId={StageId}"); }
						else if (ArticleId != null) { Response.Redirect($"/article/index?articleId={ArticleId}&blockId={block_id}"); }
					}
				}
				else
				{
					_notyf.Error("Файл не найден");
					Response.Redirect("javascript: history.go(-1)");
				}
			}
			else
			{
				_notyf.Error("Файл не найден");
				Response.Redirect("javascript: history.go(-1)");
			}
		}

		// Отображение файла
		[HttpGet]
		public async Task<IActionResult> OpenFile(string id, string blockId) 
		{
			int _id;
			bool isNumeric_Id = int.TryParse(id, out _id);

			File file = await fileService.GetElement(new ElectronicBoard.Models.File { Id = _id });
			FileStream fs = new FileStream(file.Path, FileMode.Open);
			var file_result = File(fs, file.ContentType, file.Path);
			file_result.FileDownloadName = file.FileName;
			return file_result;
		}
	}
}