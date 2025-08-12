namespace LegalPlatform.Domain.Entities;

public class LawyerProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

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

    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? RejectionReason { get; set; }
    public DateTime? VerifiedOnUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}