using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс для подключения к БД
	/// </summary>
	public class ElectronicBoardDatabase : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
	{
		public ElectronicBoardDatabase()
		{
			//Database.EnsureDeleted(); // удаляем бд со старой схемой
			//Database.EnsureCreated(); // создаем бд с новой схемой
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (optionsBuilder.IsConfigured == false)
			{
				optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-Q2JQENQ\SQLEXPRESS;
                Initial Catalog=ElectronicBoardDatabase;
                Integrated Security=True;
                MultipleActiveResultSets=True;
				TrustServerCertificate=True;");
			}
			base.OnConfiguring(optionsBuilder);
		}
		public virtual DbSet<Aggregator> Aggregators { set; get; }
		public virtual DbSet<Article> Articles { set; get; }
		public virtual DbSet<ArticleAggregator> ArticleAggregators { set; get; }
		public virtual DbSet<Author> Authors { set; get; }
		public virtual DbSet<ArticleAuthor> ArticleAuthors { set; get; }
		public virtual DbSet<Block> Blocks { set; get; }
		public virtual DbSet<BlockEvent> BlockEvents { set; get; }
		public virtual DbSet<BlockParticipant> BlockParticipants { set; get; }
		public virtual DbSet<Board> Boards { set; get; }
		public virtual DbSet<Event> Events { set; get; }
		public virtual DbSet<File> Files { set; get; }
		public virtual DbSet<Grant> Grants { set; get; }
		public virtual DbSet<GrantParticipant> GrantParticipants { set; get; }
		public virtual DbSet<Participant> Participants { set; get; }
		public virtual DbSet<Project> Projects { set; get; }
		public virtual DbSet<ProjectParticipant> ProjectParticipants { set; get; }
		public virtual DbSet<SimpleElement> SimpleElements { set; get; }
		public virtual DbSet<Stage> Stages { set; get; }
		public virtual DbSet<Sticker> Stickers { set; get; }
		public virtual DbSet<UserLDAP> UserLDAPs { set; get; }
	}
}
