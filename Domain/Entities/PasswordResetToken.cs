using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public string Token { get; set; } 
        public int UserId { get; set; }   
        public DateTime ExpiresAt { get; set; } 
        public bool IsUsed { get; set; } = false;

        public User User { get; set; } 
    }
}
