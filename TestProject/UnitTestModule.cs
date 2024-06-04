using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;
using System;
using Xunit;

namespace TestProject
{
	[TestClass]
	public class UnitTestModule
	{
		// Модуль сервисов
		// Тестирование функций создания и редактирования моделей

		AggregatorService aggregatorService = new AggregatorService();
		ArticleService articleService = new ArticleService();//
		AuthorService authorService = new AuthorService();
		BlockService blockService = new BlockService();
		BoardService boardService = new BoardService();
		EventService eventService = new EventService();//
		GrantService grantService = new GrantService();
		ProjectService projectService = new ProjectService();//
		ParticipantService participantService = new ParticipantService();//
		SimpleElementService simpleElementService = new SimpleElementService();//
		StageService stageService = new StageService();//
		StickerService stickerService = new StickerService();
		UserLDAPService userLDAPService = new UserLDAPService();//
		
		[TestMethod]
		public void TestMethodAggregatorService()
		{
			Aggregator agr = aggregatorService.CreateModel(new Aggregator { AggregatorName = "AggregatorUpdName" }, new Aggregator { Id = 1, AggregatorName = "AggregatorName" });

			if (agr.AggregatorName.Contains("AggregatorUpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodArticleService()
		{
			Article art = articleService.CreateModel(
				new Article { ArticleName = "ArticleUpdName", ArticleText = "Text", ArticleKeyWords = "Words", 
					ArticleAnnotation = "Ann", ArticlePlaceOfPublication = "Place", ArticleStatus = StatusArticle.Отправлена },
				new Article { ArticleName = "ArticleName", ArticleText = "Text", ArticleKeyWords = "Words", 
					ArticleAnnotation = "Ann", ArticlePlaceOfPublication = "Place", ArticleStatus = StatusArticle.Опубликована });

			if (art.ArticleName.Contains("ArticleUpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodAuthorService()
		{
			Author aut = authorService.CreateModel(
				new Author
				{
					AuthorFIO = "UpdFIO",
					AuthorEmail = "arm.ma@mail.ru",
					AuthorOrganization = "Org",
					ParticipantId = 1
				},
				new Author
				{
					AuthorFIO = "FIO",
					AuthorEmail = "arm.ma@mail.ru",
					AuthorOrganization = "Org",
					ParticipantId = 1
				});

			if (aut.AuthorFIO.Contains("UpdFIO")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodBlockService()
		{
			Block block = blockService.CreateModel(
				new Block
				{
					BlockName = "UpdName",
					BoardId = 1,
					VisibilityOpening = true
				},
				new Block
				{
					BlockName = "Name",
					BoardId = 1,
					VisibilityOpening = true
				});

			if (block.BlockName.Contains("UpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodBoardService()
		{
			Board board = boardService.CreateModel(
				new Board
				{
					BoardName = "UpdName",
					BoardThematics = "Them"
				},
				new Board
				{
					BoardName = "Name",
					BoardThematics = "Them"
				});

			if (board.BoardName.Contains("UpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodEventService()
		{
			Event element = eventService.CreateModel(
				new Event
				{
					EventName = "UpdName",
					EventDateFinish = DateTime.Now,
					EventDateFinishArticle = DateTime.Now,
					EventDateStart = DateTime.Now,
					EventFinishArticleColor = "red",
					EventFinishColor = "red",
					EventStartColor = "red",
					EventPlace = "red",
					EventText = "red",
					Picture = new byte[] { }
				},
				new Event
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
				});

			if (element.EventName.Contains("UpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodGrantService()
		{
			Grant element = grantService.CreateModel(
				new Grant
				{
					GrantName = "UpdName",
					GrantDescription = "Test",
					GrantText = "Test",
					Picture = new byte[] { },
					BlockId = 1,
					GrantDeadline = DateTime.Now,
					GrantDeadlineFinish = DateTime.Now,
					GrantDeadlineStart = DateTime.Now,
					GrantSource = "Source",
					GrantStatus = "Status"
				},
				new Grant
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
				});

			if (element.GrantName.Contains("UpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodProjectService()
		{
			Project element = projectService.CreateModel(
				new Project
				{
					BlockId = 1,
					Picture = new byte[] { },
					ProjectName = "UpdName",
					ProjectDescription = "Test",
					ProjectText = "Test"
				},
				new Project
				{
					BlockId = 1,
					Picture = new byte[] { },
					ProjectName = "Name",
					ProjectDescription = "Test",
					ProjectText = "Test"
				});

			if (element.ProjectName.Contains("UpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodSimpleElementService()
		{
			SimpleElement element = simpleElementService.CreateModel(
				new SimpleElement
				{
					BlockId = 1,
					Picture = new byte[] { },
					SimpleElementName = "UpdName",
					SimpleElementText = "Test"
				},
				new SimpleElement
				{
					BlockId = 1,
					Picture = new byte[] { },
					SimpleElementName = "Name",
					SimpleElementText = "Test"
				});

			if (element.SimpleElementName.Contains("UpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodStageService()
		{
			Stage element = stageService.CreateModel(
				new Stage
				{
					Picture = new byte[] { },
					DateFinish = DateTime.Now,
					DateStart = DateTime.Now,
					ProjectId = 1,
					StageDescription = "Test",
					StageText = "Test",
					Status = "Test",
					StageName = "UpdName"
				},
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
				});

			if (element.StageName.Contains("UpdName")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodStickerService()
		{
			Sticker element = stickerService.CreateModel(
				new Sticker
				{
					Picture = new byte[] { },
					StickerDescription = "UpdDesc",
					ProjectId = 1,
					EventId = 1,
					GrantId = 1,
					ParticipantId = 1,
					SimpleElementId = 1
				},
				new Sticker
				{
					Picture = new byte[] { },
					StickerDescription = "Test",
					ProjectId = 1,
					EventId = 1,
					GrantId = 1,
					ParticipantId = 1,
					SimpleElementId = 1
				});

			if (element.StickerDescription.Contains("UpdDesc")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodUserService()
		{
			UserLDAP element = userLDAPService.CreateModel(
				new UserLDAP
				{
					UserFIO = "UpdFIO",
					UserLogin = "login",
					UserPassword = "Password"
				},
				new UserLDAP
				{
					UserFIO = "FIO",
					UserLogin = "login",
					UserPassword = "Password"
				});

			if (element.UserFIO.Contains("UpdFIO")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}

		[TestMethod]
		public void TestMethodParticipantService()
		{
			Participant element = participantService.CreateModel(
				new Participant
				{
					ParticipantFIO = "UpdFIO",
					ParticipantPatents = "нет",
					ParticipantPublications = "нет",
					ParticipantRating = 0,
					ParticipantTasks = "нет",
					ScientificInterests = "нет",
					Picture = new byte[] { },
					Login = "Проверяющий",
					Password = "Ulstu_73"
				},
				new Participant
				{
					ParticipantFIO = "FIO",
					ParticipantPatents = "нет",
					ParticipantPublications = "нет",
					ParticipantRating = 0,
					ParticipantTasks = "нет",
					ScientificInterests = "нет",
					Picture = new byte[] { },
					Login = "Проверяющий",
					Password = "Ulstu_73"
				});

			if (element.ParticipantFIO.Contains("UpdFIO")) Xunit.Assert.True(true);
			else Xunit.Assert.True(false);
		}
	}
}