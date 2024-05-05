using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ElectronicBoard.Services.Implements
{
    public class GrantService : IGrantService
    {
        // Получение всего списка грантов
        public async Task<List<Grant>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Grants.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

        // Получение грантов по id блока (!!!)
        public async Task<List<Grant>> GetFilteredList(int BlockId)
        {
            if (BlockId < 0)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            return (await context.Grants.ToListAsync())
            .Where(rec => rec.BlockId == BlockId)
            .Select(CreateModel)
            .ToList();
        }

        // Получение гранта по id или названию
        public async Task<Grant> GetElement(Grant model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Grants
            .FirstOrDefaultAsync(rec => rec.GrantName.Contains(model.GrantName) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

        // Добавление гранта
        public async Task Insert(Grant model)
        {
            using var context = new ElectronicBoardDatabase();
			var component = await context.Grants
	                .FirstOrDefaultAsync(rec => rec.GrantName.Contains(model.GrantName) && rec.BlockId == model.BlockId);
			if (component == null)
			{
				await context.Grants.AddAsync(CreateModel(model, new Grant()));
				await context.SaveChangesAsync();
			}
			else throw new Exception("Грант с таким названием уже существует");
        }

        // Редактирование данных о гранте
        public async Task Update(Grant model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Grants.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			var elementName = await context.Grants.FirstOrDefaultAsync(rec => rec.GrantName.Contains(model.GrantName) && rec.BlockId == model.BlockId && rec.Id != model.Id);
			if (element == null)
            {
                throw new Exception("Грант не найден");
            }
			if (elementName == null)
			{
				CreateModel(model, element);
				await context.SaveChangesAsync();
			}
			else throw new Exception("Грант с таким названием уже существует");
		}

		// Удаление гранта
		public async Task Delete(Grant model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Grants.FirstOrDefaultAsync(rec => rec.Id == model.Id);
                if (element != null)
                {
                    var parts = (await context.GrantParticipants.ToListAsync()).Where(rec => rec.GrantId == model.Id);
                    foreach (var part in parts)
                    {
                        context.GrantParticipants.Remove(part);
                        await context.SaveChangesAsync();
                    }
                    context.Grants.Remove(element);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Грант не найден");
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Привязка и отвязка участника от гранта 
        public async Task GetParticipant(Participant model, int grant_id)
        {
            using var context = new ElectronicBoardDatabase();
            var this_part = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            var this_grant = await context.Grants.FirstOrDefaultAsync(rec => rec.Id == grant_id);
            if (this_part != null && this_grant != null)
            {
                var grant_part = await context.GrantParticipants
                    .FirstOrDefaultAsync(rec => rec.ParticipantId == model.Id
                    && rec.GrantId == grant_id);
                if (grant_part == null)
                {
                    await context.GrantParticipants.AddAsync(new GrantParticipant
                    {
                        ParticipantId = model.Id,
                        GrantId = grant_id
                    });
                    await context.SaveChangesAsync();
                }
                else
                {
                    context.GrantParticipants.Remove(grant_part);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                if (this_part == null)
                    throw new Exception("Участник не найден");
                if (this_grant == null)
                    throw new Exception("Грант не найден");
            }
        }
        // Получение участников гранта
        public async Task<List<Participant>> GetParticipants(int grantId) 
        {
            List<Participant> participants = new List<Participant>();
			using var context = new ElectronicBoardDatabase();
			var this_grant = await context.Grants.FirstOrDefaultAsync(rec => rec.Id == grantId);
 
            if (this_grant != null)
            {
                var grant_parts = (await context.GrantParticipants.ToListAsync()).Where(rec => rec.GrantId == this_grant.Id);
                foreach (var grant_part in grant_parts) 
                {
                    var part = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == grant_part.ParticipantId);
                    if (part != null) participants.Add(part);
				}
            }
            return participants;
		}

		private static Grant CreateModel(Grant model, Grant grant)
        {
            grant.BlockId = model.BlockId;
            grant.GrantName = model.GrantName;
            grant.GrantText = model.GrantText;
            grant.GrantSource = model.GrantSource;
            grant.GrantStatus = model.GrantStatus;
            grant.GrantDescription = model.GrantDescription;
            grant.GrantDeadline = model.GrantDeadline;
            grant.GrantDeadlineStart = model.GrantDeadlineStart;
            grant.GrantDeadlineFinish = model.GrantDeadlineFinish;
            grant.Picture = model.Picture.CloneByteArray();

            grant.Files = model.Files;
            grant.Stikers = model.Stikers;
            grant.GrantParticipants = model.GrantParticipants;

            return grant;
        }
        private static Grant CreateModel(Grant grant)
        {
            return new Grant
            {
                Id = grant.Id,

                BlockId = grant.BlockId,
                GrantName = grant.GrantName,
                GrantText = grant.GrantText,
                GrantSource = grant.GrantSource,
                GrantStatus = grant.GrantStatus,
                GrantDescription = grant.GrantDescription,
                GrantDeadline = grant.GrantDeadline,
                GrantDeadlineStart = grant.GrantDeadlineStart,
                GrantDeadlineFinish = grant.GrantDeadlineFinish,
                Picture = grant.Picture.CloneByteArray(),

                Files = grant.Files,
                Stikers = grant.Stikers,
                GrantParticipants = grant.GrantParticipants
            };
        }
    }
}
