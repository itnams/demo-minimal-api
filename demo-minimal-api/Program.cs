using Dapper;
using demo_minimal_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using var connection = new SqlConnection("Server=localhost;Initial Catalog=msdb;Persist Security Info=False;User ID=sa;Password=dev_2020!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");


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

app.MapPost("/articles/add", async (UpsertArticletCommand data) =>
{
    string Sql = "INSERT INTO Article values (@Title,@Url,@Content,@CreatedDate,@CreatedBy,@UpdatedDate);";
    var result = connection.ExecuteScalar(Sql, data);
    return Results.Ok(result);
}).WithName("AddArticle")
.WithOpenApi();

app.MapPut("/articles/{id}", async (UpsertArticletCommand data, int id) =>
{
    string Sql = $"UPDATE Article SET Title=@Title,Url=@Url,Content=@Content,CreatedDate=@CreatedDate,CreatedBy=@CreatedBy,UpdatedDate=@UpdatedDate WHERE Id = " + id;
    var result = connection.Execute(Sql, data);
    return Results.Ok(result);
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
