namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель для передачи настроек JWT
	/// </summary>
	public class JWTSettings
	{
		public string SecretKey { get; set; }
		public string Issuer { get; set; }
		public string Audience {  get; set; }
	}
}
