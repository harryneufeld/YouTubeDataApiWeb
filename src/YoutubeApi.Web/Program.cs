using MudBlazor.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using YoutubeApi.Infrastructure.Persistence.Contexts;
using YoutubeApi.Web.Components;
using YoutubeApi.Application.UseCases;
using YoutubeApi.Domain.Interfaces;
using YoutubeApi.Infrastructure.Persistence.Repositories;
using YoutubeApi.Application.UseCases.VideoUseCases;
using YoutubeApi.Application.UseCases.CommentUseCases;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<VideoDbContext>();
builder.Services.AddTransient<GetVideosByUserIdUseCase>();
builder.Services.AddTransient<AddVideoUseCase>();
builder.Services.AddTransient<DeleteVideoUseCase>();
builder.Services.AddTransient<SaveChangesUseCase>();
builder.Services.AddTransient<ImportVideosUseCase>();
builder.Services.AddTransient<RemoveCommentUseCase>();
builder.Services.AddTransient<QueryAllVideosUseCase>();
builder.Services.AddTransient<GetCommentsUseCase>();
builder.Services.AddTransient<GetUserIdByVideoIdUseCase>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();

var app = builder.Build();

// Get DbContextFactory and migrate database
using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<VideoDbContext>>();
    using var dbContext = dbContextFactory.CreateDbContext();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();