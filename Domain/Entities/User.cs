namespace Domain.Entities
{
    public class User : BaseEntity
    { 
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public bool IsVerified { get; set; } = false;
        public ClientApp FromApp { get; set; }
        // Navigation Property
        public UserProfile UserProfile { get; set; } 


        // Gerekli görmediğim için kaldırdım.
        //public List<RefreshToken> RefreshTokens { get; set; }
        //public ICollection<OtpCode> OtpCodes { get; set; } = new List<OtpCode>();

    }
}
