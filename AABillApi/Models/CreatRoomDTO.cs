using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AABillApi.Models
{
    public class CreatRoomDTO
    {
        [Required]
        [FromForm(Name ="roomId")]
        public int RoomId { get; set; }

        [Required]
        [FromForm(Name = "roomPwd")]
        public int RoomPwd { get; set; }

        public string Token { get; set; }
    }
}
