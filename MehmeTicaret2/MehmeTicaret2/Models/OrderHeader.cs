using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace MehmeTicaret2.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Adres { get; set; }
        [Required]
        public string Semt { get; set; }
        [Required]
        public string Sehir { get; set; }
        [Required]
        public string PostaKodu { get; set; }
    }
} 
  
