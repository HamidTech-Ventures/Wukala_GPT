using LegalPlatform.Application.DTOs;
using LegalPlatform.Application.Interfaces;
using LegalPlatform.Domain.Entities;

namespace LegalPlatform.Infrastructure.Services;

public class LawyerService : ILawyerService
{
    private readonly ILawyerRepository _lawyers;
    private readonly IUserRepository _users;

    public LawyerService(ILawyerRepository lawyers, IUserRepository users)
    {
        _lawyers = lawyers;
        _users = users;
    }

    public async Task<IEnumerable<object>> GetApprovedLawyersAsync(CancellationToken cancellationToken)
    {
        var lawyers = await _lawyers.GetApprovedAsync(cancellationToken);
        return lawyers.Select(lp => new
        {
            lp.UserId,
            lp.FullName,
            lp.City,
            lp.Specialization,
            lp.YearsOfExperience,
            lp.CurrentFirmOrPractice,
            lp.PhotoUrl,
            lp.VideoIntroUrl
        });
    }

    public async Task UpdateLawyerProfileAsync(Guid userId, SignUpLawyerRequest request, CancellationToken cancellationToken)
    {
        var profile = await _lawyers.GetByUserIdAsync(userId, cancellationToken) ?? throw new KeyNotFoundException("Profile not found");
        profile.PhotoUrl = request.PhotoUrl;
        profile.FullName = request.FullName;
        profile.Address = request.Address;
        profile.PhoneNumber = request.PhoneNumber;
        profile.City = request.City;
        profile.CNIC = request.CNIC;
        profile.DateOfBirth = request.DateOfBirth;
        profile.LawDegreeName = request.LawDegreeName;
        profile.University = request.University;
        profile.GraduationYear = request.GraduationYear;
        profile.Specialization = request.Specialization;
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.CurrentFirmOrPractice = request.CurrentFirmOrPractice;
        profile.VideoIntroUrl = request.VideoIntroUrl;
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await _lawyers.UpdateAsync(profile, cancellationToken);
    }
}

public class AdminService : IAdminService
{
    private readonly ILawyerRepository _lawyers;
    private readonly IUserRepository _users;
    private readonly IEmailService _email;

    public AdminService(ILawyerRepository lawyers, IUserRepository users, IEmailService email)
    {
        _lawyers = lawyers;
        _users = users;
        _email = email;
    }

    public async Task<IEnumerable<object>> GetPendingLawyersAsync(CancellationToken cancellationToken)
    {
        var pending = await _lawyers.GetPendingAsync(cancellationToken);
        return pending.Select(p => new
        {
            p.UserId,
            p.FullName,
            p.City,
            p.Specialization,
            p.YearsOfExperience,
            p.CurrentFirmOrPractice
        });
    }

    public async Task VerifyLawyerAsync(Guid lawyerUserId, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(lawyerUserId, cancellationToken) ?? throw new KeyNotFoundException("User not found");
        if (user.Role != UserRole.Lawyer) throw new InvalidOperationException("Not a lawyer");
        var profile = await _lawyers.GetByUserIdAsync(lawyerUserId, cancellationToken) ?? throw new KeyNotFoundException("Profile not found");
        profile.IsVerified = true;
        profile.Status = "Approved";
        profile.VerifiedOnUtc = DateTime.UtcNow;
        user.IsActive = true; // activate after admin approval
        await _lawyers.UpdateAsync(profile, cancellationToken);
        await _users.UpdateAsync(user, cancellationToken);
        await _email.SendLawyerApprovalAsync(user.Email, cancellationToken);
    }

    public async Task RejectLawyerAsync(Guid lawyerUserId, string reason, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(lawyerUserId, cancellationToken) ?? throw new KeyNotFoundException("User not found");
        if (user.Role != UserRole.Lawyer) throw new InvalidOperationException("Not a lawyer");
        var profile = await _lawyers.GetByUserIdAsync(lawyerUserId, cancellationToken) ?? throw new KeyNotFoundException("Profile not found");
        profile.IsVerified = false;
        profile.Status = "Rejected";
        profile.RejectionReason = reason;
        await _lawyers.UpdateAsync(profile, cancellationToken);
        await _email.SendLawyerRejectionAsync(user.Email, reason, cancellationToken);
    }
}