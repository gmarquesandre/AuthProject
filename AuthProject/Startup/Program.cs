using AuthProject.Startup;
using NetDevPack.Identity.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAspNetUser, AspNetUser>();

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddJwksManager().UseJwtValidation().AddNetDevPackIdentity();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
DbMigrationHelpers.EnsureSeedData(app).Wait();

app.UseRouting();
app.UseAuthConfiguration();
app.UseJwksDiscovery();
app.MapControllers();

app.Run();


public partial class Program { }
