using CaptoneProject_IOTS_API.Middleware;
using CaptoneProject_IOTS_BOs.DTO.CartDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"https://*:{port}");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var durationInMinutes = int.Parse(jwtSettings["DurationInMinutes"]);

//Configuration firebase storage
var firebaseStorageBucket = builder.Configuration["FirebaseStorage:Bucket"];

//Frontend Domain
var frontendDomain = builder.Configuration["FrontendDomain:Domain"];

var configuration = builder.Configuration;

// Configure DbContext with SQL Server
builder.Services.AddDbContext<IoTTraddingSystemContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Register UserDAO with a factory method to inject the connection string
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<UserRoleRepository>();
builder.Services.AddScoped<UserRequestRepository>();
builder.Services.AddScoped<ActivityLogRepository>();
builder.Services.AddScoped<MaterialCategoryRepository>();
builder.Services.AddScoped<IotsDeviceRepository>();
builder.Services.AddScoped<StoreRepository>();
builder.Services.AddScoped<StoreAttachmentRepository>();
builder.Services.AddScoped<AttachmentRepository>();
builder.Services.AddScoped<BusinessLicenseRepository>();
builder.Services.AddScoped<TrainerBusinessLicensesRepository>();
builder.Services.AddScoped<WalletRepository>();
builder.Services.AddScoped<MembershipPackageRepository>();
builder.Services.AddScoped<AccountMembershipPackageRepository>();
builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<LocationRepository>();
builder.Services.AddScoped<DistrictRepository>();


// Register Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MyHttpAccessor>();
builder.Services.AddScoped<IUserServices, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMaterialCategoryService, MaterialCategoryService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped(typeof(IMapService<,>), typeof(MapService<,>));
builder.Services.AddScoped<IUserRequestService, UserRequestService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStaffManagerService, StaffManagerService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAttachmentsService, AttachmentsService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IIotDevicesService, IotDeviceService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAccountMembershipPackageService, AccountMembershipPackageService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IBlogCategoryService, BlogCategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ILabService, LabService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IRatingService, RatingService>();

//GHTK sandbox
builder.Services.AddHttpClient<IGHTKService, GHTKService>();
builder.Services.AddScoped<IGHTKService, GHTKService>();

builder.Services.AddScoped<IFileService>(provider =>
{
    var bucket = configuration.GetConnectionString("Firebase-Storage-Bucket");
    return new FileService(bucket);
});
builder.Services.AddScoped<IEnvironmentService>(provider =>
{
    return new EnvironmentService(frontendDomain);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddScoped<ITokenServices>(provider =>
    new TokenService(secretKey, issuer, audience, durationInMinutes));

//Json Configuration
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kit Stem Shop API", Version = "v1" });

    //Swagger configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

//app.UseHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection();
app.UseMiddleware<AuthorizeMiddleware>();
app.UseCors("AllowAll");

//Register azure file storage
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
