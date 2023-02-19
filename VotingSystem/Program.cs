using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Abstraction;
using VotingSystem.Services;
using VotingSystem.Services.Abstraction;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Data Access
builder.Services.AddSingleton<IUserDataAccess, UserDataAccess>();
builder.Services.AddSingleton<IElectionDataAccess, ElectionDataAccess>();
builder.Services.AddSingleton<IVoteDataAccess, VoteDataAccess>();

// Services
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IElectionService, ElectionService>();
builder.Services.AddSingleton<IVoteService, VoteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();