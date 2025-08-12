using FluentValidation;
using LegalPlatform.Application.DTOs;

namespace LegalPlatform.Application.Validation;

public class SignUpLocalPersonRequestValidator : AbstractValidator<SignUpLocalPersonRequest>
{
    public SignUpLocalPersonRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class SignUpLawyerRequestValidator : AbstractValidator<SignUpLawyerRequest>
{
    public SignUpLawyerRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.CNIC).NotEmpty();
        RuleFor(x => x.DateOfBirth).NotNull();
        RuleFor(x => x.LawDegreeName).NotEmpty();
        RuleFor(x => x.University).NotEmpty();
        RuleFor(x => x.GraduationYear).InclusiveBetween(1950, DateTime.UtcNow.Year);
        RuleFor(x => x.Specialization).NotEmpty();
        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentFirmOrPractice).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class OtpVerifyRequestValidator : AbstractValidator<OtpVerifyRequest>
{
    public OtpVerifyRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.OtpCode).NotEmpty().Length(6);
    }
}