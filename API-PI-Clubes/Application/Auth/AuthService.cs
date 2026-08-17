using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Email;
using API_PI_Clubes.Application.Interfaces.IRepositories;

using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Infrastructure.Settings;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using API_PI_Clubes.Model.ValueObjects;
using System.Security.Claims;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Validators;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;


namespace API_PI_Clubes.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _repository;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IPlayerService _playerService;
        private readonly ICpfEncryptionService _cpfEncryptionService;

        public AuthService(
            IUserRepository repository,
            IUserService userService,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IEmailService emailService,
            IConfiguration config,
            IPlayerService playerService,
            ICpfEncryptionService cpfEncryptionService
            )
        {
            _repository = repository;
            _userService = userService;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _config = config;
            _playerService =  playerService;
            _cpfEncryptionService = cpfEncryptionService;
        }

        public async Task<User> LoginAsync(LoginDTO dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("User not found");

            if (!user.EmailVerification.IsConfirmed)
                throw new Exception("Email not verified");

            if (dto.Password == null)
                throw new Exception("Password is required");

            var validPassword = _passwordHasher.Verify(dto.Password, user.PasswordHash);

            if (!validPassword)
                throw new Exception("Invalid password");

            return user;
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
                Provider = "local",
                Role = RoleEnum.None,

                EmailVerification = EmailVerificationVO.Create()
            };


            var token = _tokenService.GenerateEmailVerificationToken(entity.Id);
            await _emailService.SendVerificationEmailAsync(entity.Email, entity.Name, token);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

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

        public async Task<bool> ResendEmailToken(string email)
        {
            var user =
                await _repository.GetByEmailAsync(email);

            if (user == null)
                throw new Exception("User not exists");

            if (user.EmailVerification.IsConfirmed)
                throw new Exception("Email already verified");

            var token = _tokenService.GenerateEmailVerificationToken(user.Id);
            await _emailService.SendVerificationEmailAsync(user.Email, user.Name, token);

            return true;
        }
        public async Task RequestResetPassword(string email)
        {
            var user =
                await _repository.GetByEmailAsync(email);

            if (user == null)
                throw new Exception("User not exists");


            var token = _tokenService.GenerateEmailResetPasswordToken(user.Id);
            await _emailService.SendResetPasswordAsync(user.Email, user.Name, token);

            user.ResetPassword = ResetPasswordVO.Create(token, DateTime.UtcNow.AddMinutes(15));
            _repository.Update(user);
            await _repository.SaveChangesAsync();

        }

        public async Task<bool> ResetPassword(string token, string password)
        {
            var principal = _tokenService.ValidateEmailResetPasswordToken(token);
            if (principal == null) return false;

            var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id)) return false;

            var user = await _repository.GetByIdAsync(Guid.Parse(id));
            if (user == null) return false;

            if (user.ResetPassword.PasswordResetToken != token) return false;

            if (user.ResetPassword.ResetTokenExpires < DateTime.UtcNow) return false;

            user.PasswordHash = _passwordHasher.Hash(password);
            _repository.Update(user);
            await _repository.SaveChangesAsync();

            return true;
        }
        public async Task<UserDTO> GetCurrentUserInfo(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("User not found");
            
            return new UserDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Role = entity.Role,
                PhoneNumber = entity.PhoneNumber,
                AvatarUrl = entity.AvatarUrl
            };
        }
        public async Task GoogleSignUp(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_config["Google:ClientId"]]
            };

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new Exception("Token do Google inválido ou expirado.");
            }

            if (!payload.EmailVerified)
                throw new Exception("E-mail do Google não verificado.");

            var userExists = await _repository.GetByEmailAsync(payload.Email);
            if (userExists != null)
                throw new Exception("User already exists");

            string? avatarUrl = null;
            if (!string.IsNullOrEmpty(payload.Picture))
            {
                try
                {
                    avatarUrl = await _userService.ProcessAvatarFromUrlAsync(payload.Picture);
                }
                catch
                {
                    avatarUrl = null;
                }
            }

            var entity = new User
            {
                Name = payload.Name,
                Email = payload.Email,
                Provider = "google",
                Role = RoleEnum.None,
                AvatarUrl = avatarUrl,
                EmailVerification = EmailVerificationVO.Confirm()
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

        }
        public async Task<User> GoogleLogin(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_config["Google:ClientId"]]
            };

            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new Exception("Token do Google inválido ou expirado.");
            }

            var user = await _repository.GetByEmailAsync(payload.Email);
            if (user is null)
                throw new Exception("Nenhuma conta encontrada com esse e-mail. Faça o cadastro primeiro.");

            return user; 
        }
        public async Task CompleteProfile(Guid userId, CompleteProfileDTO dto)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("Usuário não encontrado");

            if (user.Role != RoleEnum.None)
                throw new Exception("Perfil já foi completado anteriormente");

            if (!user.EmailVerification.IsConfirmed)
                throw new Exception("Confirme seu e-mail antes de completar o perfil");

            var cpfDigits = CpfValidator.Normalize(dto.Cpf);
            if (!CpfValidator.IsValid(cpfDigits))
                throw new Exception("CPF inválido");

            // var today = DateOnly.FromDateTime(DateTime.UtcNow);
            // var age = today.Year - dto.BirthDate.Year;
            // if (dto.BirthDate > today.AddYears(-age)) age--;
            // if (age < 16) 
            //     throw new Exception("Idade mínima não atendida");

            var cpfHash = _cpfEncryptionService.Hash(cpfDigits);
            var cpfInUse = await _repository.ExistsByCpfHashAsync(cpfHash);
            if (cpfInUse)
                throw new Exception("CPF já cadastrado em outra conta");

            user.PhoneNumber = dto.PhoneNumber;
            user.BirthDate = dto.BirthDate;
            user.CpfEncrypted = _cpfEncryptionService.Encrypt(cpfDigits);
            user.CpfHash = cpfHash;
            user.Role = RoleEnum.Player;

            _repository.Update(user);
            await _repository.SaveChangesAsync();

            await _playerService.Create(user.Id);
        }
    }
}
