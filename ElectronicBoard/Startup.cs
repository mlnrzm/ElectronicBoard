using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using ElectronicBoard.Services.Implements;
using ElectronicBoard.Services.ServiceContracts;

namespace ElectronicBoard
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			//services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
			//services.AddAuthorizationBuilder();
			//services.AddEndpointsApiExplorer();


			services.AddTransient<IAggregatorService, AggregatorService>();
			services.AddTransient<IArticleService, ArticleService>();
			services.AddTransient<IAuthorService, AuthorService>();
			services.AddTransient<IBlockService, BlockService>();
			services.AddTransient<IBoardService, BoardService>();
			services.AddTransient<IEventService, EventService>();
			services.AddTransient<IFileService, FileService>();
			services.AddTransient<IGrantService, GrantService>();
			services.AddTransient<IParticipantService, ParticipantService>();
			services.AddTransient<IProjectService, ProjectService>();
			services.AddTransient<ISimpleElementService, SimpleElementService>();
			services.AddTransient<IStageService, StageService>();
			services.AddTransient<IStickerService, StickerService>();

			services.AddControllersWithViews();
			services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//var app = builder.Build(); app.MapGroup("/my-identity-api").MapIdentityApi<IdentityUser>(); app.MapGet("/", (ClaimsPrincipaluser) => $"Hello {user.Identity!.Name}").RequireAuthorization();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Board/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseNotyf();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
				  name: "default",
				  pattern: "{controller=Participant}/{action=Enter}/{id?}");
			});
		}
	}
}