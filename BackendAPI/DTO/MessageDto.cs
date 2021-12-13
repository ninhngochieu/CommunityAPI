using System;
using BackendAPI.Models;

namespace BackendAPI.DTO
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        
        public Guid SenderUserId { get; set; } //Người gửi
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }
        
        public Guid RecipientUserId { get; set; } // Người nhận
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}