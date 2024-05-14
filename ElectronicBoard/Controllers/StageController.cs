using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace ElectronicBoard.Controllers
{
	public class StageController : Controller
	{
		private readonly ILogger<StageController> _logger;

		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly IParticipantService participantService;

		private readonly IProjectService projectService;
		private readonly IStageService stageService;
		private readonly IBlockService blockService;
		private readonly IBoardService boardService;
		private readonly IFileService fileService;
		private readonly INotyfService _notyf;
		public StageController(ILogger<StageController> logger,
			INotyfService notyf, IProjectService _projectService, 
			IStageService _stageService, IBlockService _blockService, 
			IBoardService _boardService, IFileService _fileService,
			UserManager<IdentityUser<int>> userManager, IParticipantService _participantService)
		{
			_logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			projectService = _projectService;
			stageService = _stageService;
			boardService = _boardService;
			fileService = _fileService;
			_userManager = userManager;
			participantService = _participantService;
		}
		// Отображение страницы с информацией о статье
		[HttpGet]
		public async Task<IActionResult> Index(string stageId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			int StageId = Convert.ToInt32(stageId);

			Stage find_stage = await stageService.GetElement(new Stage { Id = StageId });

			if (find_stage != null)
			{
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
			else
			{
				_notyf.Error("Этап не найден");
				return Redirect("javascript: history.go(-1)");
			}
		}

		// Добавление/создание этапа проекта
		[HttpGet]
		public async Task<IActionResult> AddStage(string blockId, string projectId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Block block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
			Project project = await projectService.GetElement(new Project { Id = Convert.ToInt32(projectId) });
			if (block != null && project != null)
			{
				// Передача id блока, на котором будет находиться проект 
				ViewData["blockId"] = block.Id;
				// Передача id проекта, на котором будет находиться этап
				ViewData["projectId"] = project.Id;
				return View();
			}
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}
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
				Block block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				Project project = await projectService.GetElement(new Project { Id = Convert.ToInt32(projectId) });
				if (block != null && project != null)
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

						if (DateStart <= DateFinish)
						{
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
					_notyf.Error("Ошибка");
					Response.Redirect("javascript: history.go(-1)");
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
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			int StageId = Convert.ToInt32(stageId);

			Stage find_stage = await stageService.GetElement(new Stage { Id = StageId });

			if (find_stage != null)
			{
				Project find_project = await projectService.GetElement(new Project { Id = find_stage.ProjectId });
				ViewData["projectId"] = find_project.Id;

				// Блок, на котором находится элемент
				Block find_block = await blockService.GetElement(new Block { Id = find_project.BlockId });
				ViewData["blockId"] = find_block.Id;

				return View(find_stage);
			}
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}
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
				Block block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				Project project = await projectService.GetElement(new Project { Id = Convert.ToInt32(projectId) });
				Stage stage = await stageService.GetElement(new Stage { Id = Convert.ToInt32(id) });

				if (block != null && project != null && stage != null)
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
					_notyf.Error("Ошибка");
					Response.Redirect("javascript: history.go(-1)");
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
				Block block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				Project project = await projectService.GetElement(new Project { Id = Convert.ToInt32(projectId) });
				Stage stage = await stageService.GetElement(new Stage { Id = Convert.ToInt32(id) });

				if (block != null && project != null && stage != null)
				{
					try
					{
						await stageService.Delete(new Stage { Id = stage.Id });
						Response.Redirect($"/project/index?Id={project.Id}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/stage/index?&stageId={stage.Id}");
					}
				}
				else
				{
					_notyf.Error("Статья не найдена");
					Response.Redirect($"/stage/index?&stageId={id}");
				}
			}
			else
			{
				_notyf.Error("Ошибка");
				Response.Redirect("javascript: history.go(-1)");
			}
		}
	}
}
