using LegalPlatform.Domain.Entities;

namespace LegalPlatform.Application.DTOs;

public class SignUpLocalPersonRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class SignUpLawyerRequest
{
    public string? PhotoUrl { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string LawDegreeName { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string CurrentFirmOrPractice { get; set; } = string.Empty;
    public string? VideoIntroUrl { get; set; }

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class OtpVerifyRequest
{
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public Guid? UserId { get; set; }
    public UserRole? Role { get; set; }
}