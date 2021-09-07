using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.FileDtos
{
    public class DownloadFileByKeyRequestDto
    {
        /// <summary>
        /// 文件拥有者的 ID，为保持唯一性，使用 Guid
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// 文件关键字，与 OwnerId 搭配可确定需要查找的文件
        /// </summary>
        /// <value></value>
        [Required]
        public string Key { get; set; }
    }
}
