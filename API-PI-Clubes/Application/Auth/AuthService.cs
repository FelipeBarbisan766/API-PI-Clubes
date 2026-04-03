using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Email;
using API_PI_Clubes.Application.Interfaces.IRepositories;

using API_PI_Clubes.Infrastructure.Security.Interfaces;

using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using API_PI_Clubes.Model.ValueObjects;
using System.Security.Claims;


namespace API_PI_Clubes.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _repository;
        private readonly IEmailService _emailService;


        public AuthService(
            IUserRepository repository,
        ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IEmailService emailService)
        {
            _repository = repository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<string> LoginAsync(LoginDTO dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("User not found");

            var validPassword = _passwordHasher.Verify(dto.Password, user.PasswordHash);

            if (!validPassword)
                throw new Exception("Invalid password");

            var token = _tokenService.GenerateToken(user);

            return token;
        }
        public async Task Register(CreatUserDTO dto)
        {
            var userExists =
                await _repository.GetByEmailAsync(dto.Email);

            if (userExists != null)
                throw new Exception("User already exists");

            var entity = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                PhoneNumber = dto.PhoneNumber,

                Role = RoleEnum.None,

                EmailVerification = EmailVerificationVO.Create()
            };

            await _repository.AddAsync(entity);

            await _repository.SaveChangesAsync();

            var token = _tokenService.GenerateEmailVerificationToken(entity.Id);
            await _emailService.SendVerificationEmailAsync(entity.Email,entity.Name,token);

        }

        public async Task<bool> ValidateEmailToken(string token)
        {
            var principal = _tokenService.ValidateEmailVerificationToken(token);

            if (principal == null) return false;

            var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(id)) return false;

            var user = await _repository.GetByIdAsync(Guid.Parse(id));

            if (user == null) return false;

            
            if (user.EmailVerification.IsConfirmed) return true; 

            user.EmailVerification = EmailVerificationVO.Confirm();

            _repository.Update(user);
            await _repository.SaveChangesAsync();

            return true;
        }

    }
}
