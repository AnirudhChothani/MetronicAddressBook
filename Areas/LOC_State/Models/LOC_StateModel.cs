using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MetronicAddressBook.Areas.LOC_State.Models
{
    public class LOC_StateModel
    {
        public int? StateID { get; set; }
        [Required]
        [DisplayName("State Name")]
        public string StateName { get; set; }
        [Required]
        [DisplayName("Country Name")]
        public int CountryID { get; set; }
        [Required]
        [DisplayName("State Code")]
        public string StateCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        
    }

    public class LOC_StateDropDownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
