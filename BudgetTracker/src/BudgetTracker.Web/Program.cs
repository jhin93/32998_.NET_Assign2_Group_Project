using BudgetTracker.Web.Components;
using BudgetTracker.Data;
using BudgetTracker.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Initialize repositories as singleton
builder.Services.AddSingleton(new InMemoryRepository<Category>());
builder.Services.AddSingleton(new InMemoryRepository<Transaction>());
builder.Services.AddSingleton(new InMemoryRepository<Budget>());

var app = builder.Build();

// Initialize data
using (var scope = app.Services.CreateScope())
{
    var categoryRepo = scope.ServiceProvider.GetRequiredService<InMemoryRepository<Category>>();
    var transactionRepo = scope.ServiceProvider.GetRequiredService<InMemoryRepository<Transaction>>();
    var budgetRepo = scope.ServiceProvider.GetRequiredService<InMemoryRepository<Budget>>();

    SeedData.SeedAllData(categoryRepo, transactionRepo, budgetRepo);
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
