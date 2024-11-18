using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MetronicAddressBook.Areas.LOC_City.Models
{
    public class LOC_CityModel
    {
        public int? CityID { get; set; }

        [Required]
        [DisplayName("City Name")]
        public string CityName { get; set; }

        [Required]
        [DisplayName("Country Name")]
        public int CountryID { get; set; }

        [Required]
        [DisplayName("State Name")]
        public int StateID { get; set; }

        [Required]
        [DisplayName("City Code")]
        public string CityCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class LOC_CityDropDownModel
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
    }
}
