using madame_moka.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 REGISTRAR `ApplicationDbContext` en los servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Asegurar que `ApplicationDbContext` está registrado como servicio
builder.Services.AddScoped<ApplicationDbContext>();

// 🔹 Agregar controladores con vistas
builder.Services.AddControllersWithViews();

// ✅ Habilitar sesiones en la aplicación
builder.Services.AddDistributedMemoryCache(); // Requerido para usar sesiones
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(20); // Tiempo de inactividad antes de expirar la sesión
	options.Cookie.HttpOnly = true; // Protección contra ataques XSS
	options.Cookie.IsEssential = true; // Necesario para que la sesión funcione en GDPR
});

var app = builder.Build();

// 🔹 Middleware
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.UseSession(); // ✅ Agregar uso de sesión

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // Necesario si usas API

app.Run();
