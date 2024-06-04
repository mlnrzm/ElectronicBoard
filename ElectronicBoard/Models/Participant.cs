using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Участник"
	/// </summary>
	public class Participant
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// ФИО участника
		/// </summary>
		public string ParticipantFIO { get; set; }

		/// <summary>
		/// Научные интересы
		/// </summary>
		public string ScientificInterests { get; set; }

		/// <summary>
		/// Задачи, выполняемые участником на кафедре
		/// </summary>
		public string ParticipantTasks { get; set; }

		/// <summary>
		/// Рейтинг научной активности (индекс Хирша)
		/// </summary>
		public int ParticipantRating { get; set; }

		/// <summary>
		/// Перечисление публикаций участника
		/// </summary>
		public string ParticipantPublications { get; set; }

		/// <summary>
		/// Перечисление патентов участника
		/// </summary>
		public string ParticipantPatents { get; set; }

		/// <summary>
		/// Идентификатор аккаунта пользователя
		/// </summary>
		public int IdentityId { get; set; }

		/// <summary>
		/// Логин
		/// </summary>
		public string Login { get; set; }

		/// <summary>
		/// Пароль
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Стикеры, прикрепленные к участнику
		/// </summary>
		[ForeignKey("ParticipantId")]
        public virtual List<Sticker> Stikers { get; set; }

		/// <summary>
		/// Блоки, в которых состоит участник
		/// </summary>
		[ForeignKey("ParticipantId")]
        public virtual List<BlockParticipant> ParticipantsBlocks { get; set; }

		/// <summary>
		/// Файлы, прикрепленные к участнику
		/// </summary>
		[ForeignKey("ParticipantId")]
		public virtual List<File> Files { get; set; }
	}
}
