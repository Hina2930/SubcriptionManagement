using Microsoft.EntityFrameworkCore;
using SubcriptionManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureService(builder.Services, builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureService(IServiceCollection services, ConfigurationManager configurationManager)
{
    services.AddDbContext<SubcriptionDbContext>(opts => { opts.UseNpgsql(configurationManager.GetConnectionString("AppDb")); AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); }, ServiceLifetime.Transient);
}