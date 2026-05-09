using Microsoft.AspNetCore.Authentication.Cookies;
using SistemaRRHH.Datos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Registramos la clase de conexión para que esté disponible en todo el proyecto
builder.Services.AddSingleton<ConexionDb>();

//Usamos Cookies para el login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Acceso/Login"; // A dónde mandarlo si no está logueado
		options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Tiempo de sesión
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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
