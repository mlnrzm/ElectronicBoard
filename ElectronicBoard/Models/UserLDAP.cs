namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Пользователь"
	/// </summary>
	public class UserLDAP
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// ФИО пользователя
		/// </summary>
		public string UserFIO { get; set; }

		/// <summary>
		/// Логин прользователя
		/// </summary>
		public string UserLogin { get; set; }

		/// <summary>
		/// Пароль прользователя
		/// </summary>
		public string UserPassword { get; set; }
	}
}
