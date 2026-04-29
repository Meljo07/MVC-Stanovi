using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_Stanovi.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System;

namespace MVC_Stanovi.Controllers
{
    public class HomeController : Controller
    {
        string konekcija = "server=localhost;database=stanovi;user=root;password=;";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // 🔍 INDEX (filteri)
        public IActionResult Index(string tip, string kvad, string cena, string vrsta)
        {
            List<Stan> stanovi = new List<Stan>();

            using (MySqlConnection conn = new MySqlConnection(konekcija))
            {
                conn.Open();

                string query = "SELECT * FROM stanovi WHERE 1=1";

                if (!string.IsNullOrEmpty(tip))
                    query += " AND ulica = @tip";

                if (!string.IsNullOrEmpty(kvad))
                    query += " AND kvadratura = @kvad";

                if (!string.IsNullOrEmpty(vrsta))
                    query += " AND vrsta = @vrsta";

                if (!string.IsNullOrEmpty(cena))
                {
                    if (cena == "1")
                        query += " AND cena BETWEEN 100000 AND 115000";
                    else if (cena == "2")
                        query += " AND cena BETWEEN 115000 AND 130000";
                    else if (cena == "3")
                        query += " AND cena BETWEEN 130000 AND 160000";
                    else if (cena == "4")
                        query += " AND cena BETWEEN 160000 AND 200000";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);

                if (!string.IsNullOrEmpty(tip))
                    cmd.Parameters.AddWithValue("@tip", tip);

                if (!string.IsNullOrEmpty(kvad))
                    cmd.Parameters.AddWithValue("@kvad", Convert.ToInt32(kvad));

                if (!string.IsNullOrEmpty(vrsta))
                    cmd.Parameters.AddWithValue("@vrsta", vrsta);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    stanovi.Add(new Stan
                    {
                        id = Convert.ToInt32(reader["id"]),
                        naziv = reader["naziv"].ToString(),
                        ulica = reader["ulica"].ToString(),
                        kvadratura = Convert.ToInt32(reader["kvadratura"]),
                        cena = Convert.ToInt32(reader["cena"]),
                        vrsta = reader["vrsta"].ToString(),
                        slika = reader["slika"].ToString() // 👈 dodato
                    });
                }
            }

            return View(stanovi);
        }

        // 🔐 LOGIN / REGISTER
        public IActionResult Login() => View();
        public IActionResult Register() => View();
        public IActionResult Kontakt() => View();
        public IActionResult Lokacije() => View();

        // ➕ STRANICA ZA DODAVANJE
        public IActionResult Dodavanje()
        {
            return View();
        }

        // ➕ DODAJ STAN U BAZU
        [HttpPost]
        public IActionResult DodajStan(string naziv, string ulica, int kvadratura, int cena, string vrsta, string slika)
        {
            using (MySqlConnection conn = new MySqlConnection(konekcija))
            {
                conn.Open();

                string query = "INSERT INTO stanovi (naziv, ulica, kvadratura, cena, vrsta, slika) " +
                               "VALUES (@naziv, @ulica, @kvad, @cena, @vrsta, @slika)";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@naziv", naziv);
                cmd.Parameters.AddWithValue("@ulica", ulica);
                cmd.Parameters.AddWithValue("@kvad", kvadratura);
                cmd.Parameters.AddWithValue("@cena", cena);
                cmd.Parameters.AddWithValue("@vrsta", vrsta);
                cmd.Parameters.AddWithValue("@slika", slika);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Admin"); 
        }

        public IActionResult User()
        {
            if (HttpContext.Session.GetString("user") == null)
                return RedirectToAction("Login");

            List<Stan> stanovi = new List<Stan>();

            using (MySqlConnection conn = new MySqlConnection(konekcija))
            {
                conn.Open();

                string query = "SELECT * FROM stanovi";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    stanovi.Add(new Stan
                    {
                        id = Convert.ToInt32(reader["id"]),
                        naziv = reader["naziv"].ToString(),
                        ulica = reader["ulica"].ToString(),
                        kvadratura = Convert.ToInt32(reader["kvadratura"]),
                        cena = Convert.ToInt32(reader["cena"]),
                        vrsta = reader["vrsta"].ToString(),
                        slika = reader["slika"].ToString() 
                    });
                }
            }

            return View(stanovi);
        }

        public IActionResult Admin()
        {
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("Login");

            List<Stan> stanovi = new List<Stan>();

            using (MySqlConnection conn = new MySqlConnection(konekcija))
            {
                conn.Open();

                string query = "SELECT * FROM stanovi";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    stanovi.Add(new Stan
                    {
                        id = Convert.ToInt32(reader["id"]),
                        naziv = reader["naziv"].ToString(),
                        ulica = reader["ulica"].ToString(),
                        kvadratura = Convert.ToInt32(reader["kvadratura"]),
                        cena = Convert.ToInt32(reader["cena"]),
                        vrsta = reader["vrsta"].ToString(),
                        slika = reader["slika"].ToString()
                    });
                }
            }

            return View(stanovi);
        }

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(konekcija))
            {
                conn.Open();

                string query = "INSERT INTO users (username, password, role) VALUES (@u, @p, 'user')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(konekcija))
            {
                conn.Open();

                string query = "SELECT * FROM users WHERE username=@u AND password=@p";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string role = reader["role"].ToString();

                    HttpContext.Session.SetString("user", reader["username"].ToString());
                    HttpContext.Session.SetString("role", role);

                    if (role == "admin")
                        return RedirectToAction("Admin");
                    else
                        return RedirectToAction("User");
                }
            }

            ViewBag.Error = "Pogrešan login!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}