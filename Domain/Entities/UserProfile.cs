namespace Domain.Entities
{
    public class UserProfile
    {
        public int Id { get; set; } // Birincil anahtar (User.Id ile eşleşecek) 
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User User { get; set; } // Bire bir ilişki
    }
}
