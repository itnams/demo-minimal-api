using Dapper;
using demo_minimal_api;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var configBuilder = builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process)}.json", optional: true);
IConfigurationRoot config = configBuilder.Build();
var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddDbContext<MsSqlDbContext>(options => options.UseSqlServer(config.GetConnectionString("MsSql")));
services.AddDbContext<PgDbContext>(options => options.UseNpgsql(config.GetConnectionString("PgSql")));

if (config.GetValue<string>("DbProvider") == "Ms")
    services.AddScoped<IAppDbContext>(provider => provider.GetService<MsSqlDbContext>());
else
    services.AddScoped<IAppDbContext>(provider => provider.GetService<PgDbContext>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using var connection = new SqlConnection(config.GetConnectionString("MsSql"));


app.MapGet("/articles", async (int? pageSize, int? pageIndex) =>
{
    if (pageSize is null)
    {
        pageSize = 10;
    }

    if (pageIndex is null)
    {
        pageIndex = 0;
    }
    var listArticle = connection.Query<Article>(@"SELECT* FROM Article ORDER BY Id OFFSET " + pageIndex + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY;");
    return Results.Ok(listArticle);
})
.WithName("ListArticle")
.WithOpenApi();

app.MapGet("/articles/{id}", async (int id) =>
{
    string Sql = "Select * From Article Where Id = " + id;
    var result = connection.QueryFirst<Article>(Sql);
    return Results.Ok(result);
})
.WithName("DetailArticle")
.WithOpenApi();

app.MapPost("/articles/add", async (UpsertArticleCommand data) =>
{
    string Sql = "INSERT INTO Article values (@Title,@Url,@Content,@CreatedDate,@CreatedBy,@UpdatedDate);";
    var result = connection.ExecuteScalar(Sql, data);
    return Results.Ok(result);
}).WithName("AddArticle")
.WithOpenApi();

app.MapPut("/articles/{id}", async (UpsertArticleCommand data, int id) =>
{
    string Sql = $"UPDATE Article SET Title=@Title,Url=@Url,Content=@Content,CreatedDate=@CreatedDate,CreatedBy=@CreatedBy,UpdatedDate=@UpdatedDate WHERE Id = " + id;
    var validator = new UpsertArticleCommandValidation();
    var validationResult = validator.Validate(data);
    if (validationResult.IsValid)
    {
        var result = connection.Execute(Sql, data);
        return Results.Ok(result);
    }
    else
    {
        return Results.BadRequest(validationResult.Errors);
    }
}).WithName("UpdateArticle")
.WithOpenApi();

app.MapDelete("/articles/{id}", async (int id) =>
{
    string Sql = "DELETE FROM Article WHERE Id = " + id;
    var result = connection.Execute(Sql, id);
    return Results.Ok();
}).WithName("DeleteArticle")
.WithOpenApi();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
