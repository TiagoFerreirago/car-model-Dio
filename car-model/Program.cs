var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (Dominio.Dtos.LoginDto loginDto) =>
{
    // Dummy authentication logic
    if (loginDto.Username == "admin" && loginDto.Password == "password")
    {
        return Results.Ok("Login feito com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
});

   app.Run();

