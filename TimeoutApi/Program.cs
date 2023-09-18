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
app.UseTimeout(TimeSpan.FromSeconds(10));


app.MapGet("/test", async (int delay, CancellationToken cancellationToken) =>
{
     if (delay <= 0)
        {
            return Results.BadRequest("Delay required.");
        }

        await Task.Delay(delay, cancellationToken);
        return Results.Ok($"Completed after {delay} milliseconds");

  
})
.WithName("LongRunningAction")
.WithOpenApi();

app.Run();

