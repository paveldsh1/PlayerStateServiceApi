using Microsoft.EntityFrameworkCore;
using PlayerStateServiceApi.Data;
using PlayerStateServiceApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PlayerStateContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=playerstate.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlayerStateContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/playerstates/{nickname}", async (string nickname, PlayerStateContext db) =>
{
    if (string.IsNullOrWhiteSpace(nickname))
    {
        return Results.BadRequest(new { error = "Nickname is required" });
    }

    string key = nickname.Trim();
    var state = await db.PlayerStates.AsNoTracking().FirstOrDefaultAsync(x => x.Nickname == key);
    return state is null ? Results.NotFound() : Results.Ok(state);
});

app.MapPut("/playerstates/{nickname}", async (string nickname, PlayerStateUpsertRequest request, PlayerStateContext db) =>
{
    if (string.IsNullOrWhiteSpace(nickname))
    {
        return Results.BadRequest(new { error = "Nickname is required" });
    }

    string key = nickname.Trim();
    var state = await db.PlayerStates.FirstOrDefaultAsync(x => x.Nickname == key);
    if (state is null)
    {
        state = new PlayerState
        {
            Nickname = key,
            PlayerX = request.PlayerX,
            PlayerY = request.PlayerY,
            PlayerA = request.PlayerA,
            UpdatedAt = DateTime.UtcNow
        };
        db.PlayerStates.Add(state);
    }
    else
    {
        state.PlayerX = request.PlayerX;
        state.PlayerY = request.PlayerY;
        state.PlayerA = request.PlayerA;
        state.UpdatedAt = DateTime.UtcNow;
    }

    await db.SaveChangesAsync();
    return Results.Ok(state);
});

app.Run();

internal record PlayerStateUpsertRequest(float PlayerX, float PlayerY, float PlayerA);
