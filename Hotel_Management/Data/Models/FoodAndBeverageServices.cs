using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
	public class FoodAndBeverageServices
	{
        public int ServiceID { get; set; }
        public string ItemName { get; set; }
        public int ItemPrice { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public byte[]? ItemImage { get; set; }
        public bool IsAvailable { get; set; }

    }
}
