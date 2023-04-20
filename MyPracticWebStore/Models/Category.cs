using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyPracticWebStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

    }
}
