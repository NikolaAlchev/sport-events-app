using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IMatchesRepo), typeof(MatchesRepo));
builder.Services.AddScoped(typeof(ICompetitionsRepo), typeof(CompetitionsRepo));
builder.Services.AddScoped(typeof(ITeamRepo), typeof(TeamRepo));
builder.Services.AddScoped(typeof(IPlayerRepo), typeof(PlayerRepo));

builder.Services.AddTransient<IMatchesService, MatchesService>();
builder.Services.AddTransient<ICompetitionsService, CompetitionsService>();
builder.Services.AddTransient<ITeamService, TeamService>();
builder.Services.AddTransient<IPlayerService, PlayerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
