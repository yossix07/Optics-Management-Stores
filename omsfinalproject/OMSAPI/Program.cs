using MongoDB.Driver;
using OMSAPI.DatabaseSettings;
using OMSAPI.Services;
using Microsoft.Extensions.Options;
using OMSAPI.AccessDatabase;
using OMSAPI.Services.ServicesInterfaces;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using MongoDB.Bson.Serialization;
using OMSAPI.General;
using OMSAPI.Roles;
using OMSAPI.Services.StoreServices;
using OMSAPI.Services.MailServices;
using OMSAPI.Services.EntitiesServices;
using OMSAPI.Services.AppointmentsServices;


var builder = WebApplication.CreateBuilder(args);

// Adding Tenant Services
// Getting the section name from appsettings.json and mapping them to TenantDatabaseSettings class
builder.Services.Configure<AdminDatabaseSettings>(builder.Configuration.GetSection(nameof(AdminDatabaseSettings)));

// Adding a conneciton such that for each time that IDatabaseSettings is needed, an instance of TenantDatabaseSettings will be placed.
builder.Services.AddScoped<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<AdminDatabaseSettings>>().Value);

// Adding connection such that each time IMongoClient needed, Connection string will be used.
builder.Services.AddScoped<IMongoClient>(s => new MongoClient(builder.Configuration.GetValue<string>("AdminDatabaseSettings:ConnectionString")));


builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddScoped<IAppointmentSettingsServices, AppointmentSettingsServices>();
builder.Services.AddScoped<IAppointmentServices, AppointmentServices>();
builder.Services.AddScoped<ITenantServices,TenantServices>();
builder.Services.AddScoped<IUserServices,UserServices>();
builder.Services.AddScoped<IDatabaseServices,DatabaseServices>();
builder.Services.AddScoped<IAuthServices,AuthServices>();
builder.Services.AddScoped<AppointmentServices>();
builder.Services.AddScoped<AppointmentSettingsServices>();
builder.Services.AddScoped<DatabaseServices>();
builder.Services.AddScoped<AdminServices>();
builder.Services.AddScoped<IAdminServices, AdminServices>();
builder.Services.AddScoped<ProductServices>();
builder.Services.AddScoped<OrderServices>();
builder.Services.AddScoped<EmailServices>();
builder.Services.AddScoped<IEntityServices,EntityServices>();
builder.Services.AddScoped<IStatisticsServices, StatisticsServices>();
builder.Services.AddScoped<PredictionServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();



//Adding Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddLogging();

builder.Logging.AddSerilog(); // Use serilog along with built-in logger.


// Add services to the container.
builder.Services.AddControllers( options =>
    {
        options.Filters.Add(typeof(ValidationFilter)); // Comment this row to avoid validations
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());

    });

// adiing BsonSerializer for TimeOnly and TimeSpan object.
BsonSerializer.RegisterSerializer(new TimeOnlySerializer());


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();

});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AuthSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Roles.Admin, policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy(Roles.Tenant, policy => policy.RequireRole(Roles.Admin, Roles.Tenant));
    options.AddPolicy(Roles.User, policy => policy.RequireRole(Roles.Admin, Roles.Tenant, Roles.User));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

