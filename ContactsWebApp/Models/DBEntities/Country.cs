using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Country
{
    //Kreiranje tabele u bazi za drzave, naziv za drzave ogranicen na 50 karaktera tipa varchar
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CountryId { get; set; }

    [Column(TypeName = "varchar(50)")]
    public string CountryName { get; set; }
}
