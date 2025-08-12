using BCrypt.Net;
using LegalPlatform.Application.DTOs;
using LegalPlatform.Application.Interfaces;
using LegalPlatform.Domain.Entities;

namespace LegalPlatform.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ILawyerRepository _lawyers;
    private readonly IEmailOtpRepository _otps;
    private readonly IJwtService _jwt;
    private readonly IEmailService _email;

    public AuthService(IUserRepository users, ILawyerRepository lawyers, IEmailOtpRepository otps, IJwtService jwt, IEmailService email)
    {
        _users = users;
        _lawyers = lawyers;
        _otps = otps;
        _jwt = jwt;
        _email = email;
    }

    public async Task<AuthResponse> SignUpLocalPersonAsync(SignUpLocalPersonRequest request, CancellationToken cancellationToken)
    {
        var existing = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (existing != null)
        {
            return new AuthResponse { Success = false, Message = "Email already exists" };
        }

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            City = request.City,
            Role = UserRole.LocalPerson,
            IsActive = false,
            EmailVerified = false
        };
        await _users.AddAsync(user, cancellationToken);

        var otpCode = GenerateOtp();
        var otp = new EmailOtp
        {
            UserId = user.Id,
            Code = otpCode,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        };
        await _otps.AddAsync(otp, cancellationToken);
        await _email.SendOtpAsync(user.Email, otpCode, cancellationToken);

        return new AuthResponse { Success = true, Message = "Signup successful. Verify email with OTP." };
    }

    public async Task<AuthResponse> SignUpLawyerAsync(SignUpLawyerRequest request, CancellationToken cancellationToken)
    {
        var existing = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (existing != null)
        {
            return new AuthResponse { Success = false, Message = "Email already exists" };
        }

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Lawyer,
            IsActive = false,
            EmailVerified = false
        };
        await _users.AddAsync(user, cancellationToken);

        var profile = new LawyerProfile
        {
            UserId = user.Id,
            PhotoUrl = request.PhotoUrl,
            FullName = request.FullName,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            City = request.City,
            CNIC = request.CNIC,
            DateOfBirth = request.DateOfBirth,
            LawDegreeName = request.LawDegreeName,
            University = request.University,
            GraduationYear = request.GraduationYear,
            Specialization = request.Specialization,
            YearsOfExperience = request.YearsOfExperience,
            CurrentFirmOrPractice = request.CurrentFirmOrPractice,
            VideoIntroUrl = request.VideoIntroUrl,
            IsVerified = false,
            Status = "Pending"
        };
        await _lawyers.AddAsync(profile, cancellationToken);

        var otpCode = GenerateOtp();
        var otp = new EmailOtp
        {
            UserId = user.Id,
            Code = otpCode,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        };
        await _otps.AddAsync(otp, cancellationToken);
        await _email.SendOtpAsync(user.Email, otpCode, cancellationToken);

        return new AuthResponse { Success = true, Message = "Signup successful. Verify email with OTP. Admin will review profile." };
    }

    public async Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user == null)
        {
            return new AuthResponse { Success = false, Message = "Invalid email" };
        }
        var otp = await _otps.GetActiveByUserAsync(user.Id, cancellationToken);
        if (otp == null || otp.Code != request.OtpCode)
        {
            return new AuthResponse { Success = false, Message = "Invalid or expired OTP" };
        }

        await _otps.MarkUsedAsync(otp, cancellationToken);
        user.EmailVerified = true;
        user.IsActive = user.Role != UserRole.Lawyer; // lawyers remain inactive until admin approval
        await _users.UpdateAsync(user, cancellationToken);

        return new AuthResponse { Success = true, Message = "Email verified" };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user == null) return new AuthResponse { Success = false, Message = "Invalid credentials" };
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return new AuthResponse { Success = false, Message = "Invalid credentials" };

        if (!user.EmailVerified) return new AuthResponse { Success = false, Message = "Email not verified" };
        if (!user.IsActive) return new AuthResponse { Success = false, Message = "Account not active" };

        var token = _jwt.GenerateToken(user);
        return new AuthResponse { Success = true, Message = "Login successful", Token = token, UserId = user.Id, Role = user.Role };
    }

    private static string GenerateOtp()
    {
        var rnd = Random.Shared.Next(100000, 999999);
        return rnd.ToString();
    }
}