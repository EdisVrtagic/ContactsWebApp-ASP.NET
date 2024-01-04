using ContactsWebApp.DAL;
using ContactsWebApp.Models;
using ContactsWebApp.Models.DBEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ContactsWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContactDbContext _context;

        public HomeController(ContactDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            Contact _loginC = new Contact();
            return View(_loginC);
        }
        [HttpPost]
        public async Task<IActionResult> Login(Contact _loginC)
        {
            var user = await _context.Contacts.SingleOrDefaultAsync(m => m.Email == _loginC.Email && m.Password == _loginC.Password);

            if (user == null)
            {
                ViewBag.LoginStatus = 0;
                return View(_loginC);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.Roles)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (user.Roles == "user")
            {
                return RedirectToAction("ListUser", "Home");
            }
            else if (user.Roles == "admin")
            {
                return RedirectToAction("List", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //Prikaz liste kontakata u tabeli na glavnoj formi
        [Authorize(Roles = "admin")]
        public IActionResult List()
        {
            List<Contact> contacts = _context.Contacts
                .Include(c => c.Country)
                .Include(cy => cy.City)
                .ToList();
            return View(contacts);
        }
        [Authorize(Roles = "user")]
        public IActionResult ListUser()
        {
            // Dobij email trenutno prijavljenog korisnika
            var userEmail = User.Identity.Name;

            // Dohvati kontakte samo za trenutno prijavljenog korisnika
            List<Contact> contacts = _context.Contacts
                .Include(c => c.Country)
                .Include(cy => cy.City)
                .Where(c => c.Email == userEmail)
                .ToList();

            return View(contacts);
        }


        //List koji se koristi za dohvatanje drzava iz baze i njihov prikaz u drop-down listi
        private List<SelectListItem> GetCountries()
        {
            var listCountries = new List<SelectListItem>();
            List<Country> Countries = _context.Countries.ToList();
            listCountries = Countries.Select(ct => new SelectListItem()
            {
                Value = ct.CountryId.ToString(),
                Text = ct.CountryName
            }).ToList();
            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "--Select Country--"
            };
            listCountries.Insert(0, defItem);
            return listCountries;
        }
        //List koji se koristi za dohvatanje gradova iz baze tek nakon sto se odabere odgovarajuca drzava
        private List<SelectListItem> GetCities(int countryId = 1)
        {
            List<SelectListItem> listCities = _context.Cities
                .Where(c => c.CountryId == countryId)
                .OrderBy(n => n.CityName)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.CityId.ToString(),
                    Text = n.CityName
                }).ToList();
            return listCities;
        }
        //Metod get zahtjeva za registrovanje profila
        [HttpGet]
        public IActionResult Register()
        {
            Contact Contact = new Contact();
            ViewBag.CountryId = GetCountries();
            ViewBag.CityId = new List<SelectListItem>();
            return View(Contact);
        }
        //Paznja obracena na Cross Site Request Forgery u slucaju da korisnik zeli izvrsiti neku nedozvoljenu akciju putem nekog drugog korisnika
        //Takodjer metod post koji ce izvrsiti promjene u bazi
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Register(Contact contact)
        {
            _context.Add(contact);
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }
        //Metod get zahtjeva za kreiranje kontakta
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Create()
        {
            Contact Contact = new Contact();
            ViewBag.CountryId = GetCountries();
            ViewBag.CityId = new List<SelectListItem>();
            return View(Contact);
        }
        //Paznja obracena na Cross Site Request Forgery u slucaju da korisnik zeli izvrsiti neku nedozvoljenu akciju putem nekog drugog korisnika
        //Takodjer metod post koji ce izvrsiti promjene u bazi
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Contact contact)
        {
            _context.Add(contact);
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }
        //Ovdje dohvatam gradove putem get metode i koristim listu kako bi kasnije gradovi ispravno bili prikazani, vraca Json format
        [HttpGet]
        public JsonResult GetCitiesByCountry(int countryId)
        {
            List<SelectListItem> cities = GetCities(countryId);
            return Json(cities);
        }
        //Dohvatanje kontakata putem Id identifikatora
        private Contact GetContact(int Id)
        {
            Contact contact = _context.Contacts
                .Where(c => c.Id == Id).FirstOrDefault();
            return contact;
        }
        //Get metoda za dohvatanje kontakta za izmjenu koju izvrsavam
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Contact contact = GetContact(Id);
            ViewBag.CountryId = GetCountries();
            ViewBag.CityId = GetCities(contact.CountryId);
            return View(contact);
        }
        //Takodjer metod post i ponovo Cross Site Request Forgery u slucaju da korisnik zeli izvrsiti neku nedozvoljenu akciju putem nekog drugog korisnika
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(Contact contact)
        {
            _context.Attach(contact);
            _context.Entry(contact).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }
        //Get metoda za dohvatanje kontakta za izmjenu koju izvrsavam
        [Authorize(Roles = "user")]
        [HttpGet]
        public IActionResult EditProfile(int Id)
        {
            Contact contact = GetContact(Id);
            ViewBag.CountryId = GetCountries();
            ViewBag.CityId = GetCities(contact.CountryId);
            return View(contact);
        }
        //Takodjer metod post i ponovo Cross Site Request Forgery u slucaju da korisnik zeli izvrsiti neku nedozvoljenu akciju putem nekog drugog korisnika
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult EditProfile(Contact updatedContact)
        {
            var existingContact = _context.Contacts.Find(updatedContact.Id);

            if (existingContact == null)
            {
                return NotFound();
            }
            existingContact.FirstName = updatedContact.FirstName;
            existingContact.LastName = updatedContact.LastName;
            existingContact.PhoneNumber = updatedContact.PhoneNumber;
            existingContact.Gender = updatedContact.Gender;
            existingContact.Email = updatedContact.Email;
            existingContact.Password = updatedContact.Password;
            existingContact.DateOfBirth = updatedContact.DateOfBirth;
            existingContact.Age = updatedContact.Age;
            existingContact.CountryId = updatedContact.CountryId;
            existingContact.CityId = updatedContact.CityId;
            _context.Entry(existingContact).Property(x => x.Roles).IsModified = false;
            _context.SaveChanges();
            return RedirectToAction(nameof(ListUser));
        }
        //Metod get za dohvatanje kontakta kojeg zelim obrisati, klasicni nacin dohvatanja podataka koji ce biti prikazani prije potvrde brisanja
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Contact contact = GetContact(Id);
            ViewBag.CountryId = GetCountries();
            ViewBag.CityId = GetCities(contact.CountryId);
            ViewBag.CountryName = GetCountryName(contact.CountryId);
            ViewBag.CityName = GetCityName(contact.CityId);
            return View(contact);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Contact contact)
        {
            if (User.IsInRole("admin") && User.Identity.Name == contact.Email)
            {
                //Odjava korisnika nakon brisanja profila
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            _context.Attach(contact);
            _context.Entry(contact).State = EntityState.Deleted;
            _context.SaveChanges();
            //Ako je korisnik bio admin i obrisao svoj profil, prebacuje ga na login
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Home");
            }

            return RedirectToAction(nameof(List));
        }
        //Metoda za dohvatanje naziva drzave na temelju njenog identifikatora. Takodjer vraca naziv drzave ako je nadjena, a ako nije onda null
        private string GetCountryName(int CountryId)
        {
            string countryName = _context.Countries.Where(ct => ct.CountryId == CountryId).SingleOrDefault().CountryName;
            return countryName;
        }
        //Metoda za dohvatanje naziva grada na temelju njegovog identifikatora. Takodjer vraca naziv grada ako je nadjen, a ako nije onda null
        private string GetCityName(int CityId)
        {
            string cityName = _context.Cities.Where(ct => ct.CityId == CityId).SingleOrDefault().CityName;
            return cityName;
        }
        //Logout sa profila
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Home");
        }
    }
}
