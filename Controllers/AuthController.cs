using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrecker;

public class AuthController : Controller
{
    private readonly ApplicationContext _context;

    public AuthController(ApplicationContext context)
    {
        _context = context;
    }

    // Get метод для получения страницы с Авторизацией в систему 
    [HttpGet]
    public IActionResult Login() => View();

    // Post метод для авторизации пользователя в систему
    // В теле (body) передаётся два парметра: email и пароль
    // ToDo: сделать валидацию
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Проверка на существование пользователя в системе
        var user = _context.Users.SingleOrDefault(u => u.Email == email);

        if (user == null || user.Password != password)
        {
            ModelState.AddModelError("", "Неверные почта или пароль");
            return View();
        }

        // Авторизация в систему
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

        // Перенаправление (Redirect) на основную страницу
        return RedirectToAction("Index", "Home");
    }

    // Выход из аккаунта
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}
