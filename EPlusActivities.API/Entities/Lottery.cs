using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Lottery
    {
        [Key]
        public Guid? Id { get; set; }

        [Required]
        public bool IsLucky { get; set; }

        [Required]
        public string Channel { get; set; }

        [Required]
        public int UsedCredit { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public ApplicationUser User { get; set; }

        public Guid? PrizeItemId { get; set; }

        public PrizeItem PrizeItem { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Activity Activity { get; set; }

        public Guid? PrizeTypeId { get; set; }

        public PrizeType PrizeType { get; set; }
    }
}
