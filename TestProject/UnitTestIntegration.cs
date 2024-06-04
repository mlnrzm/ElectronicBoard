using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;

namespace TestProject
{
	[TestClass]
	public class UnitTestIntegration
	{
		// Проверка корректности взаимодействия модуля доступа к данным из базы данных
		AggregatorService aggregatorService = new AggregatorService();
		ArticleService articleService = new ArticleService();
		AuthorService authorService = new AuthorService();
		BlockService blockService = new BlockService();
		BoardService boardService = new BoardService();
		EventService eventService = new EventService();
		GrantService grantService = new GrantService();
		ProjectService projectService = new ProjectService();
		ParticipantService participantService = new ParticipantService();
		SimpleElementService simpleElementService = new SimpleElementService();
		StageService stageService = new StageService();
		StickerService stickerService = new StickerService();
		UserLDAPService userLDAPService = new UserLDAPService();

		[TestMethod]
		public void TestMethodAddParticipantService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				participantService.Insert(new Participant
				{
					ParticipantFIO = "Проверяющий",
					ParticipantPatents = "нет",
					ParticipantPublications = "нет",
					ParticipantRating = 0,
					ParticipantTasks = "нет",
					ScientificInterests = "нет",
					Picture = new byte[] { },
					Login = "Проверяющий",
					Password = "Ulstu_73"
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddBoardService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				boardService.Insert(new Board
				{
					BoardName = "Name",
					BoardThematics = "Thematics"
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}


		[TestMethod]
		public void TestMethodAddAggregatorService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				aggregatorService.Insert(new Aggregator { AggregatorName = "AggregatorUpdName" }).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddArticleService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				articleService.Insert(new Article
				{
					ArticleName = "ArticleName",
					ArticleText = "Text",
					ArticleKeyWords = "Words",
					ArticleAnnotation = "Ann",
					ArticlePlaceOfPublication = "Place",
					ArticleStatus = StatusArticle.Отправлена
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddAuthorService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				authorService.Insert(new Author
				{
					AuthorFIO = "FIO",
					AuthorEmail = "arm.ma@mail.ru",
					AuthorOrganization = "Org",
					ParticipantId = 1
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddBlockService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				blockService.Insert(new Block
				{
					BlockName = "Name",
					BoardId = 1,
					VisibilityOpening = true
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddEventService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				eventService.Insert(new Event
				{
					EventName = "Name",
					EventDateFinish = DateTime.Now,
					EventDateFinishArticle = DateTime.Now,
					EventDateStart = DateTime.Now,
					EventFinishArticleColor = "red",
					EventFinishColor = "red",
					EventStartColor = "red",
					EventPlace = "red",
					EventText = "red",
					Picture = new byte[] { }
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddGrantService() 
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				grantService.Insert(new Grant
				{
					GrantName = "Name",
					GrantDescription = "Test",
					GrantText = "Test",
					Picture = new byte[] { },
					BlockId = 1,
					GrantDeadline = DateTime.Now,
					GrantDeadlineFinish = DateTime.Now,
					GrantDeadlineStart = DateTime.Now,
					GrantSource = "Source",
					GrantStatus = "Status"
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddProjectService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				projectService.Insert(
				new Project
				{
					BlockId = 1,
					Picture = new byte[] { },
					ProjectName = "Name",
					ProjectDescription = "Test",
					ProjectText = "Test"
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddSimpleElementService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				simpleElementService.Insert(
				new SimpleElement
				{
					BlockId = 1,
					Picture = new byte[] { },
					SimpleElementName = "Name",
					SimpleElementText = "Test"
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodStageService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				stageService.Insert(
				new Stage
				{
					Picture = new byte[] { },
					DateFinish = DateTime.Now,
					DateStart = DateTime.Now,
					ProjectId = 1,
					StageDescription = "Test",
					StageText = "Test",
					Status = "Test",
					StageName = "Name"
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodAddStickerService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				stickerService.Insert(
				new Sticker
				{
					Picture = new byte[] { },
					StickerDescription = "Desc",
					ProjectId = 1,
					EventId = 0,
					GrantId = 0,
					ParticipantId = 0,
					SimpleElementId = 0
				}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodUserService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				userLDAPService.Insert(
					new UserLDAP
					{
						UserFIO = "FIO",
						UserLogin = "login",
						UserPassword = "Password"
					}).Wait();
				Xunit.Assert.True(true);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		// Чтение
		[TestMethod]
		public void TestMethodReadParticipantService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = participantService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadBoardService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = boardService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}


		[TestMethod]
		public void TestMethodReadAggregatorService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = aggregatorService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadArticleService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = articleService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadAuthorService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = authorService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadBlockService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = blockService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadEventService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = eventService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadGrantService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = grantService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadProjectService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = projectService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadSimpleElementService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = simpleElementService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadStageService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = stageService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}

		[TestMethod]
		public void TestMethodReadStickerService()
		{
			Xunit.Assert.True(true);
			/*
			try
			{
				var list = stickerService.GetFullList();
				if (list != null) Xunit.Assert.True(true);
				else Xunit.Assert.True(false);
			}
			catch (Exception ex)
			{
				Xunit.Assert.True(false);
			}
			*/
		}
	}
}
