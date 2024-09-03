using EDUHUNT_BE.Data;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Repositories;
using EDUHUNT_BE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection String is not found"))
        );


        services.Configure<IdentityOptions>(opts => opts.SignIn.RequireConfirmedEmail = true);

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddRoles<IdentityRole>()
            .AddDefaultTokenProviders();

        services.AddSignalR();

        services.AddMemoryCache();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IAnswer, AnswerRepository>();
        services.AddScoped<IQuestion, QuestionRepository>();
        services.AddScoped<IStudentType, StudentTypeRepository>();
        services.AddScoped<ISurvey, SurveyRepository>();
        services.AddScoped<IScholarship, ScholarshipRepository>();
        services.AddScoped<IApplication, ApplicationRepository>();
        services.AddScoped<ICertificate, CertificateRepository>();
        services.AddScoped<IUserAccount, AccountRepository>();
        services.AddScoped<ICodeVerifies, CodeVerifiesRepository>();
        services.AddScoped<IMessages, MessagesRepository>();
        services.AddScoped<IProfiles, ProfileRepository>();
        services.AddScoped<IQas, QAsRepository>();
        services.AddScoped<IRoadMap, RoadMapRepository>();
        services.AddScoped<IScholarshipInfoes, ScholarshipInfoesRepository>();


        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
        });

        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!))
            };
        });

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowSpecificOrigin");

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<ChatHub>("chatHub");
        });
    }
}
