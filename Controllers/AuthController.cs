using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrecker;
using TaskTrecker.Models;

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

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string confirmPassword)
    {
        // Проверка на совпадение паролей
        if (password != confirmPassword)
        {
            ModelState.AddModelError("", "Пароли не совпадают");
            return View();
        }

        // Проверка на существование пользователя с таким email
        var user = _context.Users.SingleOrDefault(u => u.Email == email);
        if (user != null)
        {
            ModelState.AddModelError("", "Пользователь с таким email уже существует");
            return View();
        }

        // Создание нового пользователя
        var newUser = new User
        {
            Email = email,
            Password = password // В реальном приложении пароль должен быть хеширован
        };

        // Сохранение пользователя в базе данных
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // Автоматическая авторизация после регистрации
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, newUser.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

        // Перенаправление на основную страницу
        return RedirectToAction("Index", "Home");
    }

    // Выход из аккаунта
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();

    public IActionResult Test1() => View();

    public IActionResult Test2() => View();
}
