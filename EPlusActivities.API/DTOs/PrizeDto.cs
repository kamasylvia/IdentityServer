using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs
{
    public class PrizeDto
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public decimal UnitPrice { get; set; }
        public int RequiredCredit { get; set; }
        public string PictureUrl { get; set; }
        public Lottery Lottery { get; set; }
    }
}