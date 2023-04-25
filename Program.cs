var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration;

configuration = builder.Configuration;


// Add services to the container.
builder.Services.AddControllersWithViews();

//Setting up base api url
builder.Services.AddHttpClient("myApi", options =>
{
	options.BaseAddress = new Uri(configuration["MyApi:API"]);
});
builder.Services.AddSession(options =>{ 
options.IdleTimeout = TimeSpan.FromMinutes(5);
});

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
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
