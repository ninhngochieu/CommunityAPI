using System;

namespace BackendAPI.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        
        public Guid SenderUserId { get; set; } //Người gửi
        public AppUser SenderUser { get; set; }
        public string SenderUsername { get; set; }

        public Guid RecipientUserId { get; set; } // Người nhận
        public AppUser RecipientUser { get; set; }
        public string RecipientUsername { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        
    }
}