using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class City
{
    //Kreiranje tabele u bazi za gradove koja u sebi sadrzi foreign kljuc Country, naziv za gradove ogranicen na 50 karaktera tipa varchar
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CityId { get; set; }

    [Column(TypeName = "varchar(50)")]
    public string CityName { get; set; }

    [ForeignKey("Country")]
    public int CountryId { get; set; }
    public virtual Country Country { get; set; }
}
