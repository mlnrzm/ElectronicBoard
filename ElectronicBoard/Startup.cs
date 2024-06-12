using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.CookiePolicy;
using System.Net;

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
			services.AddCors();

			services.AddTransient<IConnectionAccountsLDAP, ConnectionAccountsLDAP>();
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
			services.AddTransient<IUserLDAPService, UserLDAPService>();


			services.AddDbContext<ElectronicBoardDatabase>(ServiceLifetime.Transient);
			services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options => { options.ClaimsIdentity.UserIdClaimType = "UserID"; }).AddEntityFrameworkStores<ElectronicBoardDatabase>();
			services.Configure<IdentityOptions>(options =>
			{
				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЬЫЪЭЮЯабвгдеёжзийклмнопрстуфхцчшщьыъэюя0123456789-._@+";
			});

			services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));

			var secretKey = Configuration.GetSection("JWTSettings:SecretKey").Value;
			var issuer = Configuration.GetSection("JWTSettings:Issuer").Value;
			var audience = Configuration.GetSection("JWTSettings:Audience").Value;
			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

			services.AddAuthentication(options => 
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = true;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = issuer,
					ValidateAudience = true,
					ValidAudience = audience,
					ValidateLifetime = true,
					IssuerSigningKey = signingKey,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.FromSeconds(1800.0)
			};
			});

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

			app.UseCors(x => x
				.WithOrigins(Configuration.GetSection("Address").Value) 
				.AllowCredentials()
				.AllowAnyMethod()
				.AllowAnyHeader());
			app.UseCookiePolicy(new CookiePolicyOptions
			{
				MinimumSameSitePolicy = SameSiteMode.Strict,
				HttpOnly = HttpOnlyPolicy.Always,
				Secure = CookieSecurePolicy.Always
			});

			AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);

			app.Use(async (context, next) =>
			{
				var token = context.Request.Cookies[".AspNetCore.Application.Id"];
				if (!string.IsNullOrEmpty(token))
				{
					context.Request.Headers.Append("Authorization", "Bearer " + token);
					context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
					context.Response.Headers.Append("X-Xss-Protection", "1");
					context.Response.Headers.Append("X-Frame-Options", "DENY");
				}
				await next();
			});

			app.UseStatusCodePages(async context => {
				var request = context.HttpContext.Request;
				var response = context.HttpContext.Response;
				if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
				// you may also check requests path to do this only for specific methods
				// && request.Path.Value.StartsWith("/specificPath")
				{
					response.Redirect("/participant/enter");
				}
			});

			app.UseAuthentication();
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