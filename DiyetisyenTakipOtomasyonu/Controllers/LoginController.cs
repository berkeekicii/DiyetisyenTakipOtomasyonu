using DiyetisyenTakipOtomasyonu.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace DiyetisyenTakipOtomasyonu.Controllers
{
    /// <summary>Allows access only to a logged-in admin (session based).</summary>
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session != null && httpContext.Session["AdminID"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary(new { controller = "Login", action = "Index" }));
        }
    }

    /// <summary>PBKDF2 password hashing. Format: pbkdf2$iterations$salt$hash (Base64).</summary>
    public static class PasswordHasher
    {
        private const int Iterations = 10000;

        public static string Hash(string password)
        {
            var salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);
            var hash = Derive(password, salt, Iterations);
            return "pbkdf2$" + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
        }

        public static bool IsHashed(string stored)
        {
            return stored != null && stored.StartsWith("pbkdf2$");
        }

        public static bool Verify(string password, string stored)
        {
            if (stored == null || password == null) return false;
            if (!IsHashed(stored)) return stored == password; // legacy plain-text row, upgraded on login
            var parts = stored.Split('$');
            if (parts.Length != 4) return false;
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Derive(password, Convert.FromBase64String(parts[2]), int.Parse(parts[1]));
            if (expected.Length != actual.Length) return false;
            int diff = 0;
            for (int i = 0; i < expected.Length; i++) diff |= expected[i] ^ actual[i];
            return diff == 0;
        }

        private static byte[] Derive(string password, byte[] salt, int iterations)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, iterations))
                return kdf.GetBytes(32);
        }
    }

    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string Email, string pass)
        {
            using (var entities = new DiyetisyenTakipOtomasyonEntities2())
            {
                var admin = entities.Admin.FirstOrDefault(x => x.Email == Email);
                if (admin != null && PasswordHasher.Verify(pass, admin.Password))
                {
                    if (!PasswordHasher.IsHashed(admin.Password))
                    {
                        admin.Password = PasswordHasher.Hash(pass); // upgrade legacy plain-text password
                        entities.SaveChanges();
                    }
                    Session["AdminID"] = admin.UserID;
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "E-posta veya şifre hatalı.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

        // New admin accounts can be created by a logged-in admin, or when no admin exists yet.
        public ActionResult Create()
        {
            if (!CanCreateAdmin()) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Admin newAdmin)
        {
            if (!CanCreateAdmin()) return RedirectToAction("Index", "Login");
            using (var entities = new DiyetisyenTakipOtomasyonEntities2())
            {
                Admin admin = new Admin()
                {
                    Email = newAdmin.Email,
                    Password = PasswordHasher.Hash(newAdmin.Password ?? ""),
                    Name = newAdmin.Name,
                    Surname = newAdmin.Surname,
                };
                entities.Admin.Add(admin);
                entities.SaveChanges();
            }
            return RedirectToAction("Index", "Login");
        }

        private bool CanCreateAdmin()
        {
            if (Session["AdminID"] != null) return true;
            using (var entities = new DiyetisyenTakipOtomasyonEntities2())
                return !entities.Admin.Any();
        }
    }
}
