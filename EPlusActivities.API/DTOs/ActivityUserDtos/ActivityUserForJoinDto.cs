using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs.ActivityUserDtos
{
    public class ActivityUserForJoinDto
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
