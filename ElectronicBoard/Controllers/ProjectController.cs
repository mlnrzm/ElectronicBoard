using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicBoard.Controllers
{
	/// <summary>
	/// Контроллер, обрабатывающий запросы касающиеся проектов
	/// </summary>
	[Authorize]
	public class ProjectController : Controller
	{
		private readonly ILogger<ProjectController> _logger;
		private readonly IdnMapping idn;

		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly IParticipantService participantService;

		private readonly IProjectService projectService;
		private readonly IStageService stageService;
		private readonly IStickerService stickerService;
		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IFileService fileService;
		private readonly INotyfService _notyf;

		public ProjectController(ILogger<ProjectController> logger,
			INotyfService notyf, IProjectService _projectService, 
			IBoardService _boardService, IBlockService _blockService, IStageService _stageService, IStickerService _stickerService, IFileService _fileService,
			UserManager<IdentityUser<int>> userManager, IParticipantService _participantService)
		{
			_logger = logger;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			projectService = _projectService;
			stageService = _stageService;
			stickerService = _stickerService;
			fileService = _fileService;
			_userManager = userManager;
			participantService = _participantService;
			idn = new IdnMapping();
		}

		/// <summary>
		/// Метод для отображения страницы с информацией о проекте
		/// </summary>
		/// <param name="project"></param>
		/// <returns></returns>
		public async Task<IActionResult> Index(Project project)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Project? find_project = await projectService.GetElement(new Project { Id = project.Id, BlockId = project.BlockId });
				if (find_project != null)
				{
					// Конвертация изображения
					if (find_project.Picture.Length > 0)
					{
						ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_project.Picture);
					}

					// Стикеры
					List<Sticker> stickers = await stickerService.GetFilteredList("project", find_project.Id);
					ViewBag.Stickers = stickers;

					// Файлы
					List<File> files = await fileService.GetFilteredList("project", find_project.Id);
					ViewBag.Files = files;

					// Этапы проекта
					List<Stage> added_stages = new List<Stage>();
					foreach (var b in await stageService.GetFilteredList(find_project.Id)) { added_stages.Add(b); }
					ViewBag.Stages = added_stages;

					// Блок, на котором находится элемент
					Block? find_block = await blockService.GetElement(new Block { Id = find_project.BlockId });
					ViewBag.Block = find_block;

					// Доска, на которой находится блок
					Board? board = await boardService.GetElement(new Board { Id = find_block.BoardId });
					List<Block> added_blocks = new List<Block>();
					foreach (var b in await blockService.GetFilteredList(new Block { BoardId = board.Id })) { added_blocks.Add(b); }
					ViewBag.Board = new Board
					{
						Id = board.Id,
						BoardName = board.BoardName,
						BoardThematics = board.BoardThematics,
						Blocks = added_blocks
					};
					return View(find_project);
				}
				else
				{
					_notyf.Error("Ошибка");
					return Redirect("javascript: history.go(-1)");
				}
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}

		/// <summary>
		/// Метод для отображения страницы добавления проекта
		/// </summary>
		/// <param name="project"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddProject(string blockId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Block? block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				if (block != null)
				{

					// Передача id блока, на котором будет находиться проект
					ViewData["blockId"] = blockId;
					return View();
				}
				else
				{
					_notyf.Error("Ошибка");
					return Redirect("javascript: history.go(-1)");
				}
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task AddProject(string blockId, string name, string text, string desc, IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(desc))
			{
				Block? block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				if (block != null)
				{
					try
					{
						// Изображение
						byte[] picture = new byte[] { };
						if (pict != null)
						{
							using (var target = new MemoryStream())
							{
								await pict.CopyToAsync(target);
								picture = target.ToArray();
							}
						}

						// ID блока
						int BlockId = Convert.ToInt32(blockId);

						// Добавление и отображение элемента блока
						await projectService.Insert(new Project { ProjectName = name, ProjectText = text, ProjectDescription = desc, BlockId = BlockId, Picture = picture });
						Project? new_project = await projectService.GetElement(new Project
						{
							ProjectName = name,
							ProjectText = text,
							ProjectDescription = desc,
							BlockId = BlockId
						});

						Response.Redirect($"/project/index?" +
							$"Id={new_project.Id}" +
							$"&BlockId={new_project.BlockId}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/project/addproject?blockId=" + blockId);
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
				Response.Redirect($"/project/addproject?blockId=" + blockId);
			}
		}

		/// <summary>
		/// Метод для отображения страницы редактирования проекта
		/// </summary>
		/// <param name="project"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> UpdProject(Project project)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Project? find_project = await projectService.GetElement(new Project
				{
					Id = project.Id
				});
				if (find_project != null)
				{
					return View(find_project);
				}
				else
				{
					_notyf.Error("Ошибка");
					return Redirect("javascript: history.go(-1)");
				}
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task UpdProject(string id, string blockId, 
			string name, string text, string desc, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
			{
				Project? pr = await projectService.GetElement(new Project { Id = Convert.ToInt32(id) });
				Block? bl = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				if (pr != null && bl != null)
				{
					try
					{
						// ID элемента
						int ElementId = Convert.ToInt32(id);
						// ID блока
						int BlockId = Convert.ToInt32(blockId);

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
							var proj = await projectService.GetElement(new Project { Id = ElementId });
							if (proj != null) picture = proj.Picture;
						}

						// Редактирование и отображение проекта
						await projectService.Update(new Project { Id = ElementId, ProjectName = name, ProjectText = text, ProjectDescription = desc, BlockId = BlockId, Picture = picture });
						Response.Redirect($"/project/index?" +
							$"Id={ElementId}" +
							$"&BlockId={BlockId}" +
							$"&ProjectName={idn.GetAscii(name)}" +
							$"&ProjectText={idn.GetAscii(text)}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/project/updproject?" +
							$"Id={id}" +
							$"&BlockId={blockId}" +
							$"&ProjectName={idn.GetAscii(name)}" +
							$"&ProjectText={idn.GetAscii(text)}");
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
				Response.Redirect($"/project/updproject?" +
					$"Id={id}" +
					$"&BlockId={blockId}" +
					$"&ProjectName={idn.GetAscii(name)}" +
					$"&ProjectText={idn.GetAscii(text)}");
			}
		}

		/// <summary>
		/// Метод для удаления проекта
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="projectId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task DeleteProject(string blockId, string projectId)
		{
			if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(blockId))
			{
				Project? pr = await projectService.GetElement(new Project { Id = Convert.ToInt32(projectId) });
				Block? bl = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				if (pr != null && bl != null)
				{
					try
					{
						await projectService.Delete(new Project { Id = pr.Id });
						Response.Redirect($"/block/index?Id=" + bl.Id);
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/project/index?Id=" + pr.Id);
					}
				}
				else
				{
					_notyf.Error("Проект не найден");
					Response.Redirect($"/project/index?Id=" + pr.Id);
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
