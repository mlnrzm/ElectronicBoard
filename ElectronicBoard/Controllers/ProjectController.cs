using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicBoard.Controllers
{
	public class ProjectController : Controller
	{
		private readonly ILogger<ProjectController> _logger;
		private readonly IProjectService projectService;
		private readonly IStageService stageService;
		private readonly IStickerService stickerService;
		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IFileService fileService;
		private readonly INotyfService _notyf;

		public ProjectController(ILogger<ProjectController> logger,
			INotyfService notyf, IProjectService _projectService, 
			IBoardService _boardService, IBlockService _blockService, IStageService _stageService, IStickerService _stickerService, IFileService _fileService)
		{
			_logger = logger;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			projectService = _projectService;
			stageService = _stageService;
			stickerService = _stickerService;
			fileService = _fileService;
		}
		// Отображение страницы с информацией о проекте
		public async Task<IActionResult> Index(Project project)
		{
			Project find_project = await projectService.GetElement(new Project { Id = project.Id, BlockId = project.BlockId });

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
			return View(find_project);
		}

		// Добавление проекта
		[HttpGet]
		public IActionResult AddProject(string blockId)
		{
			// Передача id блока, на котором будет находиться проект
			ViewData["blockId"] = blockId;
			return View();
		}
		[HttpPost]
		public async Task AddProject(string blockId, string name, string text, string desc, IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(desc))
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
					Project new_project = await projectService.GetElement(new Project
					{
						ProjectName = name,
						ProjectText = text,
						ProjectDescription = desc,
						BlockId = BlockId
					});

					Response.Redirect($"/project/index?" +
						$"Id={new_project.Id}" +
						$"&ProjectName={new_project.ProjectName}" +
						$"&ProjectText={new_project.ProjectText}" +
						$"&ProjectDescription={new_project.ProjectDescription}" +
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
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/project/addproject?blockId=" + blockId);
			}
		}

		// Редактирование проекта
		[HttpGet]
		public async Task<IActionResult> UpdProject(Project project)
		{
			Project find_project = await projectService.GetElement(new Project
			{
				Id = project.Id,
				ProjectName = project.ProjectName,
				ProjectText = project.ProjectText,
				ProjectDescription = project.ProjectDescription,
				BlockId = project.BlockId
			});
			return View(find_project);
		}
		[HttpPost]
		public async Task UpdProject(string id, string blockId, 
			string name, string text, string desc, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
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
						picture = (await projectService.GetElement(new Project { Id = ElementId })).Picture;
					}

					// Редактирование и отображение проекта
					await projectService.Update(new Project { Id = ElementId, ProjectName = name, ProjectText = text, ProjectDescription = desc, BlockId = BlockId, Picture = picture });
					Response.Redirect($"/project/index?" +
						$"Id={ElementId}" +
						$"&BlockId={BlockId}" +
						$"&ProjectName={name}" +
						$"&ProjectText={text}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/project/updproject?" +
						$"Id={id}" +
						$"&BlockId={blockId}" +
						$"&ProjectName={name}" +
						$"&ProjectText={text}");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/project/updproject?" +
					$"Id={id}" +
					$"&BlockId={blockId}" +
					$"&ProjectName={name}" +
					$"&ProjectText={text}");
			}
		}

		// Удаление проекта
		[HttpGet]
		public async Task DeleteProject(string blockId, string projectId)
		{
			if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(blockId))
			{
				int block_id;
				int element_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_elementId = int.TryParse(projectId, out element_id);
				if (isNumeric_blockId && isNumeric_elementId)
				{
					try
					{
						await projectService.Delete(new Project { Id = element_id });
						Response.Redirect($"/block/index?Id=" + block_id);
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/project/index?Id=" + element_id);
					}
				}
				else
				{
					_notyf.Error("Проект не найден");
					Response.Redirect($"/project/index?Id=" + element_id);
				}
			}
		}

	}
}
