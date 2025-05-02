namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; set; } // Birincil anahtar
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public bool IsVerified { get; set; } = false;
        // Navigation Property
        public UserProfile UserProfile { get; set; } // Bire bir ilişki
        public List<RefreshToken> RefreshTokens { get; set; }


    }
}
