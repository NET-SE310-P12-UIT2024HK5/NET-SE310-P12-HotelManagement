﻿namespace Data.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}