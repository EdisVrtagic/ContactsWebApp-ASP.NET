using ContactsWebApp.Models.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace ContactsWebApp.DAL
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
        {
        }
        //Tabele u bazi koje ce sadrzavati podatke o kontaktima, drzavama i gradovima
        //Ovaj upit omogucava pristup i manipulaciju podacima u tabelama
        //Takodjer DbSet je mapiranje na tabele u bazi i sve ovo pruza ORM - Object Relational Mapping funkcionalnost
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<City> Cities { get; set; }
    }
}
