﻿// <auto-generated />
using System;
using ElectronicBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ElectronicBoard.Migrations
{
    [DbContext(typeof(ElectronicBoardDatabase))]
    partial class ElectronicBoardDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ElectronicBoard.Models.Aggregator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AggregatorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Aggregators");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ArticleAnnotation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("ArticleKeyWords")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ArticleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ArticlePlaceOfPublication")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ArticleStatus")
                        .HasColumnType("int");

                    b.Property<string>("ArticleText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("ElectronicBoard.Models.ArticleAggregator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AggregatorId")
                        .HasColumnType("int");

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AggregatorId");

                    b.HasIndex("ArticleId");

                    b.ToTable("ArticleAggregators");
                });

            modelBuilder.Entity("ElectronicBoard.Models.ArticleAuthor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("AuthorId");

                    b.ToTable("ArticleAuthors");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthorFIO")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthorOrganization")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ParticipantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Block", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BlockName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BoardId")
                        .HasColumnType("int");

                    b.Property<int>("ElementsLocation")
                        .HasColumnType("int");

                    b.Property<bool>("VisibilityOpening")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("ElectronicBoard.Models.BlockEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlockId")
                        .HasColumnType("int");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.HasIndex("EventId");

                    b.ToTable("BlockEvents");
                });

            modelBuilder.Entity("ElectronicBoard.Models.BlockParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlockId")
                        .HasColumnType("int");

                    b.Property<int>("ParticipantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("BlockParticipants");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Board", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BoardName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BoardThematics")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EventDateFinish")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EventDateFinishArticle")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EventDateStart")
                        .HasColumnType("datetime2");

                    b.Property<int>("EventFinishArticleColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EventFinishColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventPlace")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EventStartColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("ElectronicBoard.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GrantId")
                        .HasColumnType("int");

                    b.Property<int?>("ParticipantId")
                        .HasColumnType("int");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("SimpleElementId")
                        .HasColumnType("int");

                    b.Property<int?>("StageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("GrantId");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SimpleElementId");

                    b.HasIndex("StageId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Grant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlockId")
                        .HasColumnType("int");

                    b.Property<DateTime>("GrantDeadline")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("GrantDeadlineFinish")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("GrantDeadlineStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("GrantDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GrantName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GrantSource")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GrantStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GrantText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.ToTable("Grants");
                });

            modelBuilder.Entity("ElectronicBoard.Models.GrantParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GrantId")
                        .HasColumnType("int");

                    b.Property<int>("ParticipantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GrantId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("GrantParticipants");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Participant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ParticipantArticle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParticipantFIO")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParticipantPatents")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParticipantPublications")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("ParticipantRating")
                        .HasColumnType("real");

                    b.Property<string>("ParticipantTasks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ScientificInterests")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlockId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ProjectDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ElectronicBoard.Models.ProjectParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ParticipantId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectParticipants");
                });

            modelBuilder.Entity("ElectronicBoard.Models.ProjectStage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("StageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("StageId");

                    b.ToTable("ProjectStages");
                });

            modelBuilder.Entity("ElectronicBoard.Models.SimpleElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlockId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("SimpleElementName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SimpleElementText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.ToTable("SimpleElements");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Stage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DateFinish")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateStart")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("StageDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StageText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Stages");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Sticker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EventId")
                        .HasColumnType("int");

                    b.Property<int?>("GrantId")
                        .HasColumnType("int");

                    b.Property<int?>("ParticipantId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Picture")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("SimpleElementId")
                        .HasColumnType("int");

                    b.Property<string>("StickerDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("GrantId");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SimpleElementId");

                    b.ToTable("Stickers");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Article", b =>
                {
                    b.HasOne("ElectronicBoard.Models.File", "File")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("ElectronicBoard.Models.ArticleAggregator", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Aggregator", "Aggregator")
                        .WithMany()
                        .HasForeignKey("AggregatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElectronicBoard.Models.Article", "Article")
                        .WithMany("ArticleAggregators")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aggregator");

                    b.Navigation("Article");
                });

            modelBuilder.Entity("ElectronicBoard.Models.ArticleAuthor", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Article", "Article")
                        .WithMany("ArticleAuthors")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElectronicBoard.Models.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Author", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Participant", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Block", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Board", "Board")
                        .WithMany("Blocks")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");
                });

            modelBuilder.Entity("ElectronicBoard.Models.BlockEvent", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Block", "Block")
                        .WithMany("BlockEvents")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElectronicBoard.Models.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Block");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("ElectronicBoard.Models.BlockParticipant", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Block", "Block")
                        .WithMany("BlockParticipants")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElectronicBoard.Models.Participant", "Participant")
                        .WithMany("ParticipantsBlocks")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Block");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("ElectronicBoard.Models.File", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Event", null)
                        .WithMany("Files")
                        .HasForeignKey("EventId");

                    b.HasOne("ElectronicBoard.Models.Grant", null)
                        .WithMany("Files")
                        .HasForeignKey("GrantId");

                    b.HasOne("ElectronicBoard.Models.Participant", null)
                        .WithMany("Files")
                        .HasForeignKey("ParticipantId");

                    b.HasOne("ElectronicBoard.Models.Project", null)
                        .WithMany("Files")
                        .HasForeignKey("ProjectId");

                    b.HasOne("ElectronicBoard.Models.SimpleElement", null)
                        .WithMany("Files")
                        .HasForeignKey("SimpleElementId");

                    b.HasOne("ElectronicBoard.Models.Stage", null)
                        .WithMany("Files")
                        .HasForeignKey("StageId");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Grant", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Block", null)
                        .WithMany("BlockGrants")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ElectronicBoard.Models.GrantParticipant", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Grant", "Grant")
                        .WithMany("GrantParticipants")
                        .HasForeignKey("GrantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElectronicBoard.Models.Participant", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grant");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Project", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Block", null)
                        .WithMany("BlockProjects")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ElectronicBoard.Models.ProjectParticipant", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Participant", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElectronicBoard.Models.Project", "Project")
                        .WithMany("ProjectParticipants")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ElectronicBoard.Models.ProjectStage", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Project", "Project")
                        .WithMany("ProjectStages")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElectronicBoard.Models.Stage", "Stage")
                        .WithMany()
                        .HasForeignKey("StageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("Stage");
                });

            modelBuilder.Entity("ElectronicBoard.Models.SimpleElement", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Block", null)
                        .WithMany("BlockSimpleElements")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ElectronicBoard.Models.Sticker", b =>
                {
                    b.HasOne("ElectronicBoard.Models.Event", null)
                        .WithMany("Stikers")
                        .HasForeignKey("EventId");

                    b.HasOne("ElectronicBoard.Models.Grant", null)
                        .WithMany("Stikers")
                        .HasForeignKey("GrantId");

                    b.HasOne("ElectronicBoard.Models.Participant", null)
                        .WithMany("Stikers")
                        .HasForeignKey("ParticipantId");

                    b.HasOne("ElectronicBoard.Models.Project", null)
                        .WithMany("Stikers")
                        .HasForeignKey("ProjectId");

                    b.HasOne("ElectronicBoard.Models.SimpleElement", null)
                        .WithMany("Stikers")
                        .HasForeignKey("SimpleElementId");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Article", b =>
                {
                    b.Navigation("ArticleAggregators");

                    b.Navigation("ArticleAuthors");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Block", b =>
                {
                    b.Navigation("BlockEvents");

                    b.Navigation("BlockGrants");

                    b.Navigation("BlockParticipants");

                    b.Navigation("BlockProjects");

                    b.Navigation("BlockSimpleElements");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Board", b =>
                {
                    b.Navigation("Blocks");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Event", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("Stikers");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Grant", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("GrantParticipants");

                    b.Navigation("Stikers");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Participant", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("ParticipantsBlocks");

                    b.Navigation("Stikers");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Project", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("ProjectParticipants");

                    b.Navigation("ProjectStages");

                    b.Navigation("Stikers");
                });

            modelBuilder.Entity("ElectronicBoard.Models.SimpleElement", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("Stikers");
                });

            modelBuilder.Entity("ElectronicBoard.Models.Stage", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
