using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPracticWebStore_Models
{
    public class Product
    {
        public Product()
        {
            TempCount = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Price")]
        public double Price { get; set; }
        public string Image { get; set; }

        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [NotMapped]
        [Range(1, 10000)]
        [Display(Name = "Count")]
        public int TempCount { get; set; }

    }
}
