using System;

namespace Lab8Csharp.model
{
    [Serializable]
    public class User : Entity<long>
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }

        public User() {}

        public User(string username, string hashedPassword)
        {
            Username = username;
            HashedPassword = hashedPassword;
        }
    }
}