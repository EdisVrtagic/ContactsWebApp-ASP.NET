using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ContactsWebApp.Models.DBEntities
{
    public class Contact
    {
        //Kreiranje tabele u bazi za kontakte, gdje imam max 50 karaktera tipa varchar za Ime i Prezime, te imam dva foreign kljuca za Country i City
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        [ForeignKey("Country")]
        [DisplayName("Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

        [ForeignKey("City")]
        [DisplayName("City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }
    }
}
