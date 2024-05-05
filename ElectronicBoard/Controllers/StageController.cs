using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicBoard.Controllers
{
	public class StageController : Controller
	{
		private readonly ILogger<StageController> _logger;
		private readonly IProjectService projectService;
		private readonly IStageService stageService;
		private readonly IBlockService blockService;
		private readonly IBoardService boardService;
		private readonly IFileService fileService;
		private readonly INotyfService _notyf;
		public StageController(ILogger<StageController> logger,
			INotyfService notyf, IProjectService _projectService, IStageService _stageService, IBlockService _blockService, IBoardService _boardService, IFileService _fileService)
		{
			_logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			projectService = _projectService;
			stageService = _stageService;
			boardService = _boardService;
			fileService = _fileService;
		}
		// Отображение страницы с информацией о статье
		[HttpGet]
		public async Task<IActionResult> Index(string stageId)
		{
			int StageId = Convert.ToInt32(stageId);

			Stage find_stage = await stageService.GetElement(new Stage { Id = StageId });

			// Конвертация изображения
			if (find_stage.Picture.Length > 0)
			{
				ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_stage.Picture);
			}

			// Файлы
			List<File> files = await fileService.GetFilteredList("stage", StageId);
			ViewBag.Files = files;

			// Проект, в котором находится этап
			Project find_project = await projectService.GetElement(new Project { Id = find_stage.ProjectId });
			ViewBag.Project = find_project;

			// Блок, на котором находится проект
			Block find_block = await blockService.GetElement(new Block { Id = find_project.BlockId });
			ViewBag.Block = find_block;

			// Доска, на которой находится блок
			Board board = await boardService.GetElement(new Board { Id = find_block.BoardId });
			List<Block> added_blocks = new List<Block>();
			foreach (var b in await blockService.GetFilteredList(new Block { BoardId = board.Id })) { added_blocks.Add(b); }
			ViewBag.Board = new Board
			{
				Id = board.Id,
				BoardName = board.BoardName,
				BoardThematics = board.BoardThematics,
				Blocks = added_blocks
			};
			return View(find_stage);
		}

		// Добавление/создание этапа проекта
		[HttpGet]
		public IActionResult AddStage(string blockId, string projectId)
		{
			// Передача id блока, на котором будет находиться проект 
			ViewData["blockId"] = blockId;

			// Передача id проекта, на котором будет находиться этап
			ViewData["projectId"] = projectId;

			return View();
		}
		[HttpPost]
		public async Task AddStage(string blockId, string projectId, 
			string name, string text, string desc, 
			string start, string finish, string status, IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(projectId)
				&& !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(desc)
				&& !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(finish) && !string.IsNullOrEmpty(status))
			{
				try
				{
					// Изображение
					byte[] picture = new byte[] { };
					if (pict != null)
					{
						using (var target = new MemoryStream())
						{
							pict.CopyTo(target);
							picture = target.ToArray();
						}
					}

					int BlockId = Convert.ToInt32(blockId);
					int ProjectId = Convert.ToInt32(projectId);

					DateTime DateStart = DateTime.ParseExact(start, "yyyy-M-dd", null);
					DateTime DateFinish = DateTime.ParseExact(finish, "yyyy-M-dd", null);

					if (DateStart <= DateFinish) {
						// Добавление и отображение этапа
						await stageService.Insert(new Stage
						{
							StageName = name,
							StageDescription = desc,
							StageText = text,
							ProjectId = ProjectId,
							Status = status,
							DateStart = DateStart,
							DateFinish = DateFinish,
							Picture = picture
						});
						Stage new_stage = await stageService.GetElement(new Stage
						{
							StageName = name,
							StageText = text,
							StageDescription = desc,
							Status = status,

							ProjectId = ProjectId,
							Picture = picture
						});

						Response.Redirect($"/stage/index?" +
							$"&stageId={new_stage.Id}");
					}
					else 
					{
						_notyf.Error("Дата начала должна быть раньше даты окончания.");
						Response.Redirect($"/stage/addstage?blockId=" + blockId + "&projectId=" + projectId);
					}
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/stage/addstage?blockId=" + blockId + "&projectId=" + projectId);
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/stage/addstage?blockId=" + blockId + "&projectId=" + projectId);
			}
		}

		// Редактирование этапа проекта
		[HttpGet]
		public async Task<IActionResult> UpdStage(string stageId)
		{
			int StageId = Convert.ToInt32(stageId);

			Stage find_stage = await stageService.GetElement(new Stage { Id = StageId });

			Project find_project = await projectService.GetElement(new Project { Id = find_stage.ProjectId });
			ViewData["projectId"] = find_project.Id;

			// Блок, на котором находится элемент
			Block find_block = await blockService.GetElement(new Block { Id = find_project.BlockId });
			ViewData["blockId"] = find_block.Id;

			return View(find_stage);
		}
		[HttpPost]
		public async Task UpdStage(string id, string blockId, string projectId,
			string name, string text, string desc, string start, string finish, 
			string status, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(projectId)
				&& !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(desc)
				&& !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(finish) && !string.IsNullOrEmpty(status))
			{
				try
				{
					int Id = Convert.ToInt32(id);
					int BlockId = Convert.ToInt32(blockId);
					int ProjectId = Convert.ToInt32(projectId);

					// Изображение
					bool del = true;
					switch (delpic)
					{
						case "on":
							del = true;
							break;
						case null:
							del = false;
							break;
					}
					byte[] picture = new byte[] { };
					if (pict != null)
					{
						using (var target = new MemoryStream())
						{
							await pict.CopyToAsync(target);
							picture = target.ToArray();
						}
					}
					else if (!del)
					{
						picture = (await stageService.GetElement(new Stage { Id = Id })).Picture;
					}

					DateTime DateStart = DateTime.ParseExact(start, "yyyy-M-dd", null);
					DateTime DateFinish = DateTime.ParseExact(finish, "yyyy-M-dd", null);

					if (DateStart <= DateFinish)
					{
						// Редактирование этапа проекта
						await stageService.Update(new Stage
						{
							Id = Id,
							StageName = name,
							StageDescription = desc,
							DateStart = DateStart,
							DateFinish = DateFinish,
							ProjectId = ProjectId,
							StageText = text,
							Status = status,
							Picture = picture
						});
						Stage this_stage = await stageService.GetElement(new Stage
						{
							StageName = name,
							ProjectId = ProjectId,
							DateStart = DateStart,
							DateFinish = DateFinish
						});

						// Отображение этапа проекта
						Response.Redirect($"/stage/index?" +
							$"&stageId={this_stage.Id}");
					}
					else
					{
						_notyf.Error("Дата начала должна быть раньше даты окончания.");
						Response.Redirect($"/stage/updstage?" +
							$"stageId={id}");
					}
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/stage/updstage?" +
						$"stageId={id}");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/stage/updstage?" +
					$"stageId={id}");
			}
		}

		// Удаление этапа проекта
		[HttpGet]
		public async Task DeleteStage(string id, string blockId, string projectId)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(blockId))
			{
				int _id;
				int block_id;
				int project_id;

				bool isNumeric_Id = int.TryParse(id, out _id);
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_projectId = int.TryParse(projectId, out project_id);

				if (isNumeric_blockId && isNumeric_Id && isNumeric_projectId)
				{
					try
					{
						await stageService.Delete(new Stage { Id = _id });
						Response.Redirect($"/project/index?Id={project_id}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/stage/index?&stageId={_id}");
					}
				}
				else
				{
					_notyf.Error("Статья не найдена");
					Response.Redirect($"/stage/index?&stageId={id}");
				}
			}
		}
	}
}
