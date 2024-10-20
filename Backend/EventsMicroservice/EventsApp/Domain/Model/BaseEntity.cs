using System.ComponentModel.DataAnnotations;

namespace Domain.Model
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
