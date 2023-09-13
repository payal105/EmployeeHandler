using System.ComponentModel.DataAnnotations;

namespace EMS.Models
{
    public class Feedback
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string Message { get; set; }
    }
}
