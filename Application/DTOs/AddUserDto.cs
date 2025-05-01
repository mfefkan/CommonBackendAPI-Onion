namespace Application.DTOs
{
    public class AddUserDto
    {

        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public UserProfileDto UserProfile { get; set; }
    }

    public class UserProfileDto
    {
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
