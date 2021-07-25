using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.UserDtos
{
    public class UserDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        [Phone]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号必须为 11 位数字。")]
        public string PhoneNumber { get; set; }

        public bool IsMember { get; set; }

        public string MemberId { get; set; }

        public int Credit { get; set; }

        // 注册渠道
        public ChannelCode RegisterChannel { get; set; }

        // 登录渠道
        [Required]
        public ChannelCode LoginChannel { get; set; }

        // 注册日期
        public DateTime RegisterDate { get; set; }

        // 上次登录日期
        public DateTime? LastActiveDate { get; set; }

        // 签到天数
        public int AttendanceDays { get; set; }

        // 连续签到天数
        public int SequentialAttendanceDays { get; set; }
    }
}
