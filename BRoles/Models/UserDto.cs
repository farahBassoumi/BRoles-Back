namespace BRoles.Models
{
    public class UserDto
    {


        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public long Salary { get; set; }


        public string Token { get; set; }

        public string Role { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
       public bool Desactivated { get; set; }=false;
        
        public string? ResetPasswordToken { get; set; }
        public DateTime ResetPasswordExpiry { get; set; } 

        
    }
}
