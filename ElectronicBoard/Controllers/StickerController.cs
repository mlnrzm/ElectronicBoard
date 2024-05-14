using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicBoard.Controllers
{
	public class StickerController : Controller
	{
		private readonly ILogger<StickerController> _logger;
		private readonly INotyfService _notyf;

		private readonly UserManager<IdentityUser<int>> _userManager;

		private readonly IStickerService stickerService;
		private readonly IProjectService projectService;
		private readonly ISimpleElementService elementService;
		private readonly IEventService eventService;
		private readonly IParticipantService participantService;
		private readonly IGrantService grantService;

		private readonly IBlockService blockService;
		private readonly IBoardService boardService;

		public StickerController(ILogger<StickerController> logger, INotyfService notyf, 
			IProjectService _projectService, ISimpleElementService _elementService, 
			IEventService _eventService, IParticipantService _participantService, IGrantService _grantService,
			IBlockService _blockService, IBoardService _boardService, IStickerService _stickerService, UserManager<IdentityUser<int>> userManager)
		{
			_logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			projectService = _projectService;
			boardService = _boardService;
			elementService = _elementService;
			eventService = _eventService;
			participantService = _participantService;
			grantService = _grantService;
			stickerService = _stickerService;
			_userManager = userManager;
		}

		// Добавление стикера
		[HttpGet]
		public async Task<IActionResult> AddSticker(string blockId, 
			string projectId, string elementId, string eventId, string partId, string grantId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			if (string.IsNullOrEmpty(projectId) && string.IsNullOrEmpty(elementId) && string.IsNullOrEmpty(eventId)
				&& string.IsNullOrEmpty(partId) && string.IsNullOrEmpty(grantId)) 
			{
				_notyf.Error("Ошибка добавления стикера");
				Response.Redirect($"/block/index?Id=" + blockId);
			}
			else 
			{
				ViewData["projectId"] = projectId;
				ViewData["elementId"] = elementId;
				ViewData["eventId"] = eventId;
				ViewData["partId"] = partId;
				ViewData["grantId"] = grantId;
			}

			// Передача id блока, на котором будет находится элемент
			ViewData["blockId"] = blockId;
			return View();
		}
		[HttpPost]
		public async Task AddSticker(string blockId,
			string projectId, string elementId, string eventId, string partId, string grantId, 
			string desc, IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(desc))
			{
				// ID блока
				int BlockId = Convert.ToInt32(blockId);

				int? ProjectId = null;
				int? ElementId = null;
				int? EventId = null;
				int? GrantId = null;
				int? PartId = null;

				if (Convert.ToInt32(projectId) != 0) ProjectId = Convert.ToInt32(projectId);
				if (Convert.ToInt32(elementId) != 0) ElementId = Convert.ToInt32(elementId);
				if (Convert.ToInt32(eventId) != 0) EventId = Convert.ToInt32(eventId);
				if (Convert.ToInt32(grantId) != 0) GrantId = Convert.ToInt32(grantId);
				if (Convert.ToInt32(partId) != 0) PartId = Convert.ToInt32(partId);

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
					// Добавление стикера и отображение элемента
					await stickerService.Insert(new Sticker
					{
						StickerDescription = desc,
						EventId = EventId,
						ProjectId = ProjectId,
						SimpleElementId = ElementId,
						GrantId = GrantId,
						ParticipantId = PartId,
						Picture = picture
					});

					// Отображение соответствующего элемента
					if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
					else if ( ElementId != null ) { Response.Redirect($"/element/index?Id={ElementId}"); }
					else if ( EventId != null ) { Response.Redirect($"/event/index?eventId={EventId}&blockId={BlockId}"); }
					else if ( GrantId != null ) { Response.Redirect($"/grant/index?Id={GrantId}"); }
					else if ( PartId != null ) { Response.Redirect($"/participant/index?partId={PartId}&blockId={BlockId}"); }
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
					else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
					else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={BlockId}"); }
					else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
					else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={BlockId}"); }
					else Response.Redirect($"/block/index?Id=" + blockId);
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/sticker/addsticker?blockId={blockId}&projectId={projectId}&elementId={elementId}&eventId={eventId}&partId={partId}&grantId={grantId}");
			}
		}

		// Редактирование стикера
		[HttpGet]
		public async Task<IActionResult> UpdSticker(string id, string blockId,
			string projectId, string elementId, string eventId, string partId, string grantId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			int StickerId = Convert.ToInt32(id);
			Sticker find_sticker = await stickerService.GetElement(new Sticker
			{
				Id = StickerId
			});
			if (string.IsNullOrEmpty(projectId) && string.IsNullOrEmpty(elementId) && string.IsNullOrEmpty(eventId)
					&& string.IsNullOrEmpty(partId) && string.IsNullOrEmpty(grantId))
			{
				_notyf.Error("Ошибка редактирования стикера");
				Response.Redirect($"/block/index?Id=" + blockId);
			}
			else
			{
				ViewData["projectId"] = projectId;
				ViewData["elementId"] = elementId;
				ViewData["eventId"] = eventId;
				ViewData["partId"] = partId;
				ViewData["grantId"] = grantId;
			}
			// Передача id блока, на котором находится элемент стикера
			ViewData["blockId"] = blockId;
			return View(find_sticker);
		}
		[HttpPost]
		public async Task UpdSticker(string id, string blockId,
			string projectId, string elementId, string eventId, string partId, string grantId,
			string desc, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(desc))
			{
				// ID блока
				int Id = Convert.ToInt32(id);
				int BlockId = Convert.ToInt32(blockId);

				int? ProjectId = null;
				int? ElementId = null;
				int? EventId = null;
				int? GrantId = null;
				int? PartId = null;

				if (Convert.ToInt32(projectId) != 0) ProjectId = Convert.ToInt32(projectId);
				if (Convert.ToInt32(elementId) != 0) ElementId = Convert.ToInt32(elementId);
				if (Convert.ToInt32(eventId) != 0) EventId = Convert.ToInt32(eventId);
				if (Convert.ToInt32(grantId) != 0) GrantId = Convert.ToInt32(grantId);
				if (Convert.ToInt32(partId) != 0) PartId = Convert.ToInt32(partId);

				try
				{
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
						picture = (await stickerService.GetElement(new Sticker { Id = Id })).Picture;
					}

					// Редактирование и отображение элемента
					await stickerService.Update(new Sticker
					{
						Id = Id,
						StickerDescription = desc,
						EventId = EventId,
						GrantId = GrantId,
						ParticipantId = PartId,
						ProjectId = ProjectId,
						SimpleElementId = ElementId,
						Picture = picture
					});
					// Отображение соответствующего элемента
					if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
					else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
					else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={BlockId}"); }
					else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
					else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={BlockId}"); }
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
					else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
					else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={BlockId}"); }
					else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
					else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={BlockId}"); }
					else Response.Redirect($"/block/index?Id=" + blockId);
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/sticker/updsticker?id={id}&blockId={blockId}&projectId={projectId}&elementId={elementId}&eventId={eventId}&partId={partId}&grantId={grantId}");
			}
		}

		// Удаление стикера
		[HttpGet]
		public async Task DeleteSticker(string id, string blockId)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId))
			{
				int block_id;
				int _id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_Id = int.TryParse(id, out _id);

				if (isNumeric_blockId && isNumeric_Id)
				{
					Sticker sticker = await stickerService.GetElement(new Sticker { Id = _id });
					int? ProjectId = sticker.ProjectId;
					int? ElementId = sticker.SimpleElementId;
					int? EventId = sticker.EventId;
					int? GrantId = sticker.GrantId;
					int? PartId = sticker.ParticipantId;
					try
					{
						await stickerService.Delete(new Sticker { Id = _id });
						// Отображение соответствующего элемента
						if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
						else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
						else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={block_id}"); }
						else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
						else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={block_id}"); }
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						if (ProjectId != null) { Response.Redirect($"/project/index?Id={ProjectId}"); }
						else if (ElementId != null) { Response.Redirect($"/element/index?Id={ElementId}"); }
						else if (EventId != null) { Response.Redirect($"/event/index?eventId={EventId}&blockId={block_id}"); }
						else if (GrantId != null) { Response.Redirect($"/grant/index?Id={GrantId}"); }
						else if (PartId != null) { Response.Redirect($"/participant/index?partId={PartId}&blockId={block_id}"); }
					}
				}
				else
				{
					_notyf.Error("Стикер не найден");
					Response.Redirect("javascript: history.go(-1)");
				}
			}
		}
	}
}
