using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Простой элемент"
	/// </summary>
	public class SimpleElementService : ISimpleElementService
    {
        private IFileService fileService { get; set; }
		private IStickerService stickerService { get; set; }

        public SimpleElementService(IFileService _fileService, IStickerService _stickerService)
        {
            fileService = _fileService;
            stickerService = _stickerService;
        }

		public SimpleElementService() {}

		/// <summary>
		/// Метод для получения списка элементов
		/// </summary>
		/// <returns></returns>
		public async Task<List<SimpleElement>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.SimpleElements.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка элементов по Id блока
		/// </summary>
		/// <param name="BlockId"></param>
		/// <returns></returns>
		public async Task<List<SimpleElement>> GetFilteredList(int BlockId)
        {
            if (BlockId < 0)
            {
                return new List<SimpleElement>();
            }
            using var context = new ElectronicBoardDatabase();
            return (await context.SimpleElements.ToListAsync())
            .Where(rec => rec.BlockId == BlockId)
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения элемента по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<SimpleElement?> GetElement(SimpleElement model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            SimpleElement? component = null;
            if (model.Id > 0)
            {
                component = await context.SimpleElements
                    .FirstOrDefaultAsync(rec => rec.Id == model.Id);
            }
            else
            {
                component = await context.SimpleElements
                .FirstOrDefaultAsync(rec => rec.SimpleElementName.Contains(model.SimpleElementName));
            }
            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления элемента
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(SimpleElement model)
        {
            using var context = new ElectronicBoardDatabase();
			var component = await context.SimpleElements
                .FirstOrDefaultAsync(rec => rec.SimpleElementName.Contains(model.SimpleElementName) && rec.BlockId == model.BlockId);
            if (component == null)
            {
				await context.SimpleElements.AddAsync(CreateModel(model, new SimpleElement()));
				await context.SaveChangesAsync();
			}
			else throw new Exception("В блоке уже есть элемент с таким названием");
		}

		/// <summary>
		/// Метод для редактирования элемента
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(SimpleElement model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.SimpleElements.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.SimpleElements.FirstOrDefaultAsync(rec => rec.SimpleElementName.Contains(model.SimpleElementName) && rec.BlockId == model.BlockId && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            if (elementName == null) 
            {
                CreateModel(model, element);
                await context.SaveChangesAsync();
            }
            else throw new Exception("В блоке уже есть элемент с таким названием");
		}

		/// <summary>
		/// Метод для удаления элемента
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(SimpleElement model)
        {
            using var context = new ElectronicBoardDatabase();

            // Удаление файлов элемента
            var files = await fileService.GetFilteredList("element", model.Id);
            foreach (var file in files)
            {
                await fileService.Delete(file);
            }
            // Удаление стикеров элемента
            var stickers = await stickerService.GetFilteredList("element", model.Id);
            foreach (var sticker in stickers)
            {
                await stickerService.Delete(sticker);
            }

            // Удаление элемента
            var element = await context.SimpleElements.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.SimpleElements.Remove(element);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Элемент не найден");
            }
        }
        public SimpleElement CreateModel(SimpleElement model, SimpleElement element)
        {
            element.BlockId = model.BlockId;
            element.SimpleElementName = model.SimpleElementName;
            element.SimpleElementText = model.SimpleElementText;
            element.Picture = model.Picture.CloneByteArray();

            element.Stikers = model.Stikers;
            element.Files = model.Files;

            return element;
        }
        private static SimpleElement CreateModel(SimpleElement element)
        {
            return new SimpleElement
            {
                Id = element.Id,

                BlockId = element.BlockId,
                SimpleElementName = element.SimpleElementName,
                SimpleElementText = element.SimpleElementText,
                Picture = element.Picture.CloneByteArray(),

                Stikers = element.Stikers,
                Files = element.Files
            };
        }
    }
}
