
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<IDbService,DbService>(); 

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();