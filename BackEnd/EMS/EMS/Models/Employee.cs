using System.ComponentModel.DataAnnotations;

namespace EMS.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string DOB { get; set; }
        public string Address { get; set; }
        public string Degree { get; set; }
        public string Specialization { get; set; }
        public string Skills { get; set; }
        public string Designation { get; set; }
        public long Salary { get; set; }

    }
}
