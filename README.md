# Legal Platform API

Production-grade ASP.NET Core 8 Web API for a legal services platform with secure authentication, role-based access, encrypted storage, and clean architecture.

## Tech Stack
- ASP.NET Core 8 (Controllers)
- Entity Framework Core 8 + PostgreSQL (Npgsql)
- JWT Authentication (roles: LocalPerson, Lawyer, Admin)
- BCrypt for password hashing
- AES encryption for files and secure chat
- FluentValidation for input validation
- Serilog for logging
- MailKit SMTP for OTP emails

## Project Structure (Clean Architecture)
```
LegalPlatform.sln
├─ LegalPlatform.Api/               # Presentation layer (controllers, Program.cs)
├─ LegalPlatform.Application/       # DTOs, interfaces, validators
├─ LegalPlatform.Domain/            # Entities
└─ LegalPlatform.Infrastructure/    # EF Core DbContext, repositories, services, utils
```

Key folders/files:
- `LegalPlatform.Domain/Entities/*` – `User`, `LawyerProfile`, `Document`, `ChatHistory`, `EmailOtp`
- `LegalPlatform.Infrastructure/Data/AppDbContext.cs` – EF Core context
- `LegalPlatform.Infrastructure/Repositories/*` – Repositories for Users, Lawyers, Documents, Chat, OTP
- `LegalPlatform.Infrastructure/Services/*` – Auth, Documents, Chat, Lawyers, Admin, News services
- `LegalPlatform.Infrastructure/Utils/*` – `JwtService`, `EncryptionService`, `EmailService`
- `LegalPlatform.Api/Controllers/*` – Controllers per feature
- `LegalPlatform.Api/Program.cs` – DI, CORS, JWT, Swagger, Serilog, HttpClients

## Prerequisites
- .NET 8 SDK
- PostgreSQL 14+ running and accessible
- SMTP account (for OTP emails) or use a dev mailbox service

## Configuration
Set values in `LegalPlatform.Api/appsettings.json` (or environment-specific appsettings):
- `ConnectionStrings:Default` – PostgreSQL connection string
- `Jwt` – `Issuer`, `Audience`, `Key` (32+ chars), `LifetimeMinutes`
- `Smtp` – `Host`, `Port`, `Username`, `Password`, `From`
- `Crypto:AesKey` – Strong secret for AES key derivation
- `ExternalApis` – Placeholder URLs for AI, News, ML model

Example (dev):
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=legal_platform;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "LegalPlatform",
    "Audience": "LegalPlatformClient",
    "Key": "CHANGE_ME_SUPER_SECRET_KEY_32+_CHARS",
    "LifetimeMinutes": "60"
  },
  "Smtp": { "Host": "smtp.example.com", "Port": "587", "Username": "user@example.com", "Password": "password", "From": "noreply@example.com" },
  "Crypto": { "AesKey": "CHANGE_ME_AES_MASTER_KEY" },
  "ExternalApis": { "AiBaseUrl": "https://ai.example.com", "NewsBaseUrl": "https://news.example.com", "MlBaseUrl": "https://ml.example.com" }
}
```

Security notes:
- Never commit real secrets. Use user-secrets or environment variables in production.
- Use HTTPS in production. Configure a reverse proxy or Kestrel certificates.

## Database & Migrations
Install EF tool:
```bash
dotnet tool install -g dotnet-ef
```
Create the initial migration and update database:
```bash
# From repository root
dotnet ef migrations add InitialCreate \
  -p LegalPlatform.Infrastructure \
  -s LegalPlatform.Api \
  -o Data/Migrations

dotnet ef database update \
  -p LegalPlatform.Infrastructure \
  -s LegalPlatform.Api
```

## Run the API
```bash
# Restore and build
dotnet restore
dotnet build -c Release

# Run API
dotnet run -p LegalPlatform.Api -c Release
```

- Swagger UI: http://localhost:5000/swagger (or shown port)
- CORS: permissive for dev. Restrict origins for production in `Program.cs` CORS policy.

## Seeding an Admin User (one-time)
Option A (quick): Sign up a user via API, then set their role and flags directly in DB.
```sql
-- once you have a user row (id = <GUID>), promote to Admin and activate
UPDATE "Users"
SET "Role" = 3, "EmailVerified" = true, "IsActive" = true
WHERE "Email" = 'admin@example.com';
```
Option B: Insert a new admin row with a BCrypt password hash. Generate a hash using any BCrypt generator (cost 10-12), then:
```sql
INSERT INTO "Users"("Id","Email","PasswordHash","Role","IsActive","EmailVerified","CreatedAtUtc")
VALUES(gen_random_uuid(),'admin@example.com','$2a$10$REPLACE_WITH_HASH',3,true,true,now());
```

## Authentication
- Sign up → OTP email → Verify → Login → Receive JWT
- Include JWT in Authorization header: `Authorization: Bearer <token>`
- Roles: `LocalPerson`, `Lawyer`, `Admin`
- Lawyers are inactive until Admin approval (`IsVerified=true` + `Status=Approved` → user activated)

## How to Make Common Changes
- Change DB connection: `LegalPlatform.Api/appsettings.json` → `ConnectionStrings:Default`
- Change JWT settings: `LegalPlatform.Api/appsettings.json` → `Jwt`
- Replace AES key: `LegalPlatform.Api/appsettings.json` → `Crypto:AesKey`
- SMTP provider: `LegalPlatform.Api/appsettings.json` → `Smtp`
- External services: `LegalPlatform.Api/appsettings.json` → `ExternalApis`
- Add validations: add or edit validators in `LegalPlatform.Application/Validation/Validators.cs`
- Add fields to entities: update `LegalPlatform.Domain/Entities/*` and create an EF migration

## API Testing Guide (curl examples)
Set a base URL environment variable for convenience:
```bash
BASE=http://localhost:5000
```

### 1) Sign Up
Local Person:
```bash
curl -X POST "$BASE/api/auth/signup/local" -H "Content-Type: application/json" -d '{
  "name":"John Doe","email":"john@example.com","phoneNumber":"+1234567","city":"Lahore","password":"Passw0rd!"
}'
```
Lawyer:
```bash
curl -X POST "$BASE/api/auth/signup/lawyer" -H "Content-Type: application/json" -d '{
  "photoUrl":"https://example.com/p.jpg","fullName":"Jane Lawyer","address":"123 St","phoneNumber":"+1234567",
  "city":"Karachi","CNIC":"12345-1234567-1","dateOfBirth":"1990-01-01","lawDegreeName":"LLB",
  "university":"PU","graduationYear":2012,"specialization":"Family Law","yearsOfExperience":8,
  "currentFirmOrPractice":"Firm X","videoIntroUrl":"https://example.com/v.mp4",
  "email":"jane@example.com","password":"Passw0rd!"
}'
```

### 2) Verify OTP
```bash
curl -X POST "$BASE/api/auth/verify-otp" -H "Content-Type: application/json" -d '{
  "email":"john@example.com","otpCode":"123456"
}'
```

### 3) Login (get JWT)
```bash
TOKEN=$(curl -s -X POST "$BASE/api/auth/login" -H "Content-Type: application/json" -d '{
  "email":"john@example.com","password":"Passw0rd!"
}' | jq -r .token)
echo $TOKEN
```

### 4) Chatbot
```bash
curl -X POST "$BASE/api/chat" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "query": "What is the process to file a case?"
}'
```

### 5) Documents
Upload (img/pdf/docx/xlsx/txt/ppt, up to 100MB):
```bash
curl -X POST "$BASE/api/documents/upload" -H "Authorization: Bearer $TOKEN" -F file=@/path/to/file.pdf
```
List:
```bash
curl -X GET "$BASE/api/documents" -H "Authorization: Bearer $TOKEN"
```
Download:
```bash
DOC_ID=<document-guid>
curl -L "$BASE/api/documents/$DOC_ID/download" -H "Authorization: Bearer $TOKEN" -o downloaded_file
```

### 6) Lawyers
Public approved lawyers:
```bash
curl "$BASE/api/lawyers"
```
Update lawyer profile (Lawyer role):
```bash
curl -X PUT "$BASE/api/lawyers/update" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "photoUrl":"https://example.com/p.jpg","fullName":"Jane Lawyer","address":"123 St","phoneNumber":"+1234567",
  "city":"Karachi","CNIC":"12345-1234567-1","dateOfBirth":"1990-01-01","lawDegreeName":"LLB",
  "university":"PU","graduationYear":2012,"specialization":"Family Law","yearsOfExperience":8,
  "currentFirmOrPractice":"Firm X","videoIntroUrl":"https://example.com/v.mp4",
  "email":"jane@example.com","password":"Passw0rd!"
}'
```

### 7) News + ML
Set interest:
```bash
curl -X POST "$BASE/api/news/interest" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "interest":"legal tech"
}'
```
Get news:
```bash
curl "$BASE/api/news" -H "Authorization: Bearer $TOKEN"
```

### 8) Secure Lawyer-Client Chat
Send secure text:
```bash
CID=$(curl -s -X POST "$BASE/api/chat/secure" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "query":"Hello, I need help with my case."
}' | jq -r .conversationId)
echo $CID
```
Send secure file:
```bash
curl -X POST "$BASE/api/chat/secure/file?conversationId=$CID" -H "Authorization: Bearer $TOKEN" -F file=@/path/to/file.png
```
Get conversation:
```bash
curl "$BASE/api/chat/secure/$CID" -H "Authorization: Bearer $TOKEN"
```

### 9) Admin – Lawyer Verification (Admin role only)
Get pending:
```bash
curl "$BASE/api/admin/lawyers/pending" -H "Authorization: Bearer $TOKEN"
```
Verify:
```bash
LAWYER_USER_ID=<guid>
curl -X PUT "$BASE/api/admin/lawyers/$LAWYER_USER_ID/verify" -H "Authorization: Bearer $TOKEN"
```
Reject:
```bash
curl -X PUT "$BASE/api/admin/lawyers/$LAWYER_USER_ID/reject" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "reason":"Insufficient documentation"
}'
```

## Troubleshooting
- DB connectivity: verify `ConnectionStrings:Default`, network access, and run migrations.
- JWT 401: ensure you verified OTP and (for Lawyers) that Admin approved the profile.
- Emails: use a reliable SMTP; in dev, consider a fake SMTP server (MailHog, Papercut).
- Large files: adjust `RequestSizeLimit` in `DocumentsController` if needed.
- CORS: update policy in `Program.cs` → `builder.Services.AddCors(...)`.

## Notes
- All user-linked data references the `Users.Id` GUID. Async/await is used throughout for DB I/O.
- Swagger is enabled in Development. For production, secure Swagger or disable it.