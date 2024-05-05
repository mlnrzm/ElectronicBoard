using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicBoard.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.CreateTable(
				name: "Aggregators",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					AggregatorName = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Aggregators", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Boards",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					BoardName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					BoardThematics = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Boards", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Events",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					EventText = table.Column<string>(type: "nvarchar(max)", nullable: false),
					EventPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
					EventDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
					EventStartColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
					EventDateFinish = table.Column<DateTime>(type: "datetime2", nullable: false),
					EventFinishColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
					EventDateFinishArticle = table.Column<DateTime>(type: "datetime2", nullable: false),
					EventFinishArticleColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Events", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Participants",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ParticipantFIO = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ScientificInterests = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ParticipantTasks = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ParticipantRating = table.Column<float>(type: "real", nullable: false),
					ParticipantPublications = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ParticipantArticle = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ParticipantPatents = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Participants", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Stages",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					StageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					StageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
					StageDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
					DateStart = table.Column<DateTime>(type: "datetime2", nullable: true),
					DateFinish = table.Column<DateTime>(type: "datetime2", nullable: true),
					Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Stages", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Blocks",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					BoardId = table.Column<int>(type: "int", nullable: false),
					BlockName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					VisibilityOpening = table.Column<bool>(type: "bit", nullable: false),
					ElementsLocation = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Blocks", x => x.Id);
					table.ForeignKey(
						name: "FK_Blocks_Boards_BoardId",
						column: x => x.BoardId,
						principalTable: "Boards",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Authors",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ParticipantId = table.Column<int>(type: "int", nullable: false),
					AuthorFIO = table.Column<string>(type: "nvarchar(max)", nullable: false),
					AuthorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
					AuthorOrganization = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Authors", x => x.Id);
					table.ForeignKey(
						name: "FK_Authors_Participants_ParticipantId",
						column: x => x.ParticipantId,
						principalTable: "Participants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "BlockEvents",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					EventId = table.Column<int>(type: "int", nullable: false),
					BlockId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_BlockEvents", x => x.Id);
					table.ForeignKey(
						name: "FK_BlockEvents_Blocks_BlockId",
						column: x => x.BlockId,
						principalTable: "Blocks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_BlockEvents_Events_EventId",
						column: x => x.EventId,
						principalTable: "Events",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "BlockParticipants",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ParticipantId = table.Column<int>(type: "int", nullable: false),
					BlockId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_BlockParticipants", x => x.Id);
					table.ForeignKey(
						name: "FK_BlockParticipants_Blocks_BlockId",
						column: x => x.BlockId,
						principalTable: "Blocks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_BlockParticipants_Participants_ParticipantId",
						column: x => x.ParticipantId,
						principalTable: "Participants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Grants",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					BlockId = table.Column<int>(type: "int", nullable: false),
					GrantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					GrantText = table.Column<string>(type: "nvarchar(max)", nullable: false),
					GrantDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
					GrantSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
					GrantDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
					GrantStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
					GrantDeadlineStart = table.Column<DateTime>(type: "datetime2", nullable: false),
					GrantDeadlineFinish = table.Column<DateTime>(type: "datetime2", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Grants", x => x.Id);
					table.ForeignKey(
						name: "FK_Grants_Blocks_BlockId",
						column: x => x.BlockId,
						principalTable: "Blocks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Projects",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					BlockId = table.Column<int>(type: "int", nullable: false),
					ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ProjectText = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ProjectDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Projects", x => x.Id);
					table.ForeignKey(
						name: "FK_Projects_Blocks_BlockId",
						column: x => x.BlockId,
						principalTable: "Blocks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "SimpleElements",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					BlockId = table.Column<int>(type: "int", nullable: false),
					SimpleElementName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					SimpleElementText = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SimpleElements", x => x.Id);
					table.ForeignKey(
						name: "FK_SimpleElements_Blocks_BlockId",
						column: x => x.BlockId,
						principalTable: "Blocks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "GrantParticipants",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ParticipantId = table.Column<int>(type: "int", nullable: false),
					GrantId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_GrantParticipants", x => x.Id);
					table.ForeignKey(
						name: "FK_GrantParticipants_Grants_GrantId",
						column: x => x.GrantId,
						principalTable: "Grants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_GrantParticipants_Participants_ParticipantId",
						column: x => x.ParticipantId,
						principalTable: "Participants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ProjectParticipants",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ParticipantId = table.Column<int>(type: "int", nullable: false),
					ProjectId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProjectParticipants", x => x.Id);
					table.ForeignKey(
						name: "FK_ProjectParticipants_Participants_ParticipantId",
						column: x => x.ParticipantId,
						principalTable: "Participants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ProjectParticipants_Projects_ProjectId",
						column: x => x.ProjectId,
						principalTable: "Projects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ProjectStages",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					StageId = table.Column<int>(type: "int", nullable: false),
					ProjectId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProjectStages", x => x.Id);
					table.ForeignKey(
						name: "FK_ProjectStages_Projects_ProjectId",
						column: x => x.ProjectId,
						principalTable: "Projects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ProjectStages_Stages_StageId",
						column: x => x.StageId,
						principalTable: "Stages",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Files",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
					SimpleElementId = table.Column<int>(type: "int", nullable: true),
					EventId = table.Column<int>(type: "int", nullable: true),
					ParticipantId = table.Column<int>(type: "int", nullable: true),
					ProjectId = table.Column<int>(type: "int", nullable: true),
					GrantId = table.Column<int>(type: "int", nullable: true),
					StageId = table.Column<int>(type: "int", nullable: true),
					ArticleId = table.Column<int>(type: "int", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Files", x => x.Id);
					table.ForeignKey(
						name: "FK_Files_Events_EventId",
						column: x => x.EventId,
						principalTable: "Events",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Files_Grants_GrantId",
						column: x => x.GrantId,
						principalTable: "Grants",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Files_Participants_ParticipantId",
						column: x => x.ParticipantId,
						principalTable: "Participants",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Files_Projects_ProjectId",
						column: x => x.ProjectId,
						principalTable: "Projects",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Files_SimpleElements_SimpleElementId",
						column: x => x.SimpleElementId,
						principalTable: "SimpleElements",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Files_Stages_StageId",
						column: x => x.StageId,
						principalTable: "Stages",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Stickers",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					StickerDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
					SimpleElementId = table.Column<int>(type: "int", nullable: true),
					EventId = table.Column<int>(type: "int", nullable: true),
					ParticipantId = table.Column<int>(type: "int", nullable: true),
					ProjectId = table.Column<int>(type: "int", nullable: true),
					GrantId = table.Column<int>(type: "int", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Stickers", x => x.Id);
					table.ForeignKey(
						name: "FK_Stickers_Events_EventId",
						column: x => x.EventId,
						principalTable: "Events",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Stickers_Grants_GrantId",
						column: x => x.GrantId,
						principalTable: "Grants",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Stickers_Participants_ParticipantId",
						column: x => x.ParticipantId,
						principalTable: "Participants",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Stickers_Projects_ProjectId",
						column: x => x.ProjectId,
						principalTable: "Projects",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Stickers_SimpleElements_SimpleElementId",
						column: x => x.SimpleElementId,
						principalTable: "SimpleElements",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Articles",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ArticleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ArticleText = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ArticleAnnotation = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ArticlePlaceOfPublication = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ArticleKeyWords = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ArticleStatus = table.Column<int>(type: "int", nullable: false),
					Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
					ArticleId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Articles", x => x.Id);
					table.ForeignKey(
						name: "FK_Articles_Files_ArticleId",
						column: x => x.ArticleId,
						principalTable: "Files",
						principalColumn: "Id",
						onDelete: ReferentialAction.SetNull);
				});

			migrationBuilder.CreateTable(
				name: "ArticleAggregators",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ArticleId = table.Column<int>(type: "int", nullable: false),
					AggregatorId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ArticleAggregators", x => x.Id);
					table.ForeignKey(
						name: "FK_ArticleAggregators_Aggregators_AggregatorId",
						column: x => x.AggregatorId,
						principalTable: "Aggregators",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ArticleAggregators_Articles_ArticleId",
						column: x => x.ArticleId,
						principalTable: "Articles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ArticleAuthors",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ArticleId = table.Column<int>(type: "int", nullable: false),
					AuthorId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ArticleAuthors", x => x.Id);
					table.ForeignKey(
						name: "FK_ArticleAuthors_Articles_ArticleId",
						column: x => x.ArticleId,
						principalTable: "Articles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ArticleAuthors_Authors_AuthorId",
						column: x => x.AuthorId,
						principalTable: "Authors",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ArticleAggregators_AggregatorId",
				table: "ArticleAggregators",
				column: "AggregatorId");

			migrationBuilder.CreateIndex(
				name: "IX_ArticleAggregators_ArticleId",
				table: "ArticleAggregators",
				column: "ArticleId");

			migrationBuilder.CreateIndex(
				name: "IX_ArticleAuthors_ArticleId",
				table: "ArticleAuthors",
				column: "ArticleId");

			migrationBuilder.CreateIndex(
				name: "IX_ArticleAuthors_AuthorId",
				table: "ArticleAuthors",
				column: "AuthorId");

			migrationBuilder.CreateIndex(
				name: "IX_Articles_ArticleId",
				table: "Articles",
				column: "ArticleId");

			migrationBuilder.CreateIndex(
				name: "IX_Authors_ParticipantId",
				table: "Authors",
				column: "ParticipantId");

			migrationBuilder.CreateIndex(
				name: "IX_BlockEvents_BlockId",
				table: "BlockEvents",
				column: "BlockId");

			migrationBuilder.CreateIndex(
				name: "IX_BlockEvents_EventId",
				table: "BlockEvents",
				column: "EventId");

			migrationBuilder.CreateIndex(
				name: "IX_BlockParticipants_BlockId",
				table: "BlockParticipants",
				column: "BlockId");

			migrationBuilder.CreateIndex(
				name: "IX_BlockParticipants_ParticipantId",
				table: "BlockParticipants",
				column: "ParticipantId");

			migrationBuilder.CreateIndex(
				name: "IX_Blocks_BoardId",
				table: "Blocks",
				column: "BoardId");

			migrationBuilder.CreateIndex(
				name: "IX_Files_EventId",
				table: "Files",
				column: "EventId");

			migrationBuilder.CreateIndex(
				name: "IX_Files_GrantId",
				table: "Files",
				column: "GrantId");

			migrationBuilder.CreateIndex(
				name: "IX_Files_ParticipantId",
				table: "Files",
				column: "ParticipantId");

			migrationBuilder.CreateIndex(
				name: "IX_Files_ProjectId",
				table: "Files",
				column: "ProjectId");

			migrationBuilder.CreateIndex(
				name: "IX_Files_SimpleElementId",
				table: "Files",
				column: "SimpleElementId");

			migrationBuilder.CreateIndex(
				name: "IX_Files_StageId",
				table: "Files",
				column: "StageId");

			migrationBuilder.CreateIndex(
				name: "IX_GrantParticipants_GrantId",
				table: "GrantParticipants",
				column: "GrantId");

			migrationBuilder.CreateIndex(
				name: "IX_GrantParticipants_ParticipantId",
				table: "GrantParticipants",
				column: "ParticipantId");

			migrationBuilder.CreateIndex(
				name: "IX_Grants_BlockId",
				table: "Grants",
				column: "BlockId");

			migrationBuilder.CreateIndex(
				name: "IX_ProjectParticipants_ParticipantId",
				table: "ProjectParticipants",
				column: "ParticipantId");

			migrationBuilder.CreateIndex(
				name: "IX_ProjectParticipants_ProjectId",
				table: "ProjectParticipants",
				column: "ProjectId");

			migrationBuilder.CreateIndex(
				name: "IX_Projects_BlockId",
				table: "Projects",
				column: "BlockId");

			migrationBuilder.CreateIndex(
				name: "IX_ProjectStages_ProjectId",
				table: "ProjectStages",
				column: "ProjectId");

			migrationBuilder.CreateIndex(
				name: "IX_ProjectStages_StageId",
				table: "ProjectStages",
				column: "StageId");

			migrationBuilder.CreateIndex(
				name: "IX_SimpleElements_BlockId",
				table: "SimpleElements",
				column: "BlockId");

			migrationBuilder.CreateIndex(
				name: "IX_Stickers_EventId",
				table: "Stickers",
				column: "EventId");

			migrationBuilder.CreateIndex(
				name: "IX_Stickers_GrantId",
				table: "Stickers",
				column: "GrantId");

			migrationBuilder.CreateIndex(
				name: "IX_Stickers_ParticipantId",
				table: "Stickers",
				column: "ParticipantId");

			migrationBuilder.CreateIndex(
				name: "IX_Stickers_ProjectId",
				table: "Stickers",
				column: "ProjectId");

			migrationBuilder.CreateIndex(
				name: "IX_Stickers_SimpleElementId",
				table: "Stickers",
				column: "SimpleElementId");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleAggregators");

            migrationBuilder.DropTable(
                name: "ArticleAuthors");

            migrationBuilder.DropTable(
                name: "BlockEvents");

            migrationBuilder.DropTable(
                name: "BlockParticipants");

            migrationBuilder.DropTable(
                name: "GrantParticipants");

            migrationBuilder.DropTable(
                name: "ProjectParticipants");

            migrationBuilder.DropTable(
                name: "ProjectStages");

            migrationBuilder.DropTable(
                name: "Stickers");

            migrationBuilder.DropTable(
                name: "Aggregators");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Grants");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "SimpleElements");

            migrationBuilder.DropTable(
                name: "Stages");

            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "Boards");
        }
    }
}
