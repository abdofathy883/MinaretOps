﻿namespace Core.DTOs.AuthDTOs
{
    public class LoginDTO
    {
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
