using VetCRM.Api.Middleware;
using VetCRM.Modules.Clients;
using VetCRM.Modules.Pets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPetsModule(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddClientModule(builder.Configuration.GetConnectionString("Default"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
