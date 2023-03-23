using GestionProductos.Middlewares;
using GestionProductos.Persistence;
using GestionProductos.Persistence.Repositories;
using GestionProductos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => {
	options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
		Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
		In = ParameterLocation.Header,
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey
	});

	options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => {
		options.TokenValidationParameters = new TokenValidationParameters {
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Token:Key").Value)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});

var connectionString = builder.Configuration.GetConnectionString("AplicacionDb");

builder.Services.AddDbContext<AplicacionDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<DbContext, AplicacionDbContext>();

builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<RoleRepository>();
builder.Services.AddTransient<ProductoRepository>();
builder.Services.AddTransient<ReservaRepository>();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<RoleService>();
builder.Services.AddTransient<ProductoService>();
builder.Services.AddTransient<ReservaService>();


builder.Services.AddCors(options => options.AddPolicy("AllowAny",
	policy => {
		policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
	}));

var app = builder.Build();

if(app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAny");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//logger
app.UseUsersLoggerMiddleware();

app.Run();
