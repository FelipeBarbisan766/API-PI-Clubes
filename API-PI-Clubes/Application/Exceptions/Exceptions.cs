namespace API_PI_Clubes.Application.Exceptions
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        protected AppException(string errorCode, string message, int statusCode)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string entity, object id)
            : base("NOT_FOUND", $"{entity} com id '{id}' não foi encontrado.", 404)
        {
        }

        public NotFoundException(string message)
            : base("NOT_FOUND", message, 404)
        {
        }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Você não tem permissão para executar esta ação.")
            : base("FORBIDDEN", message, 403)
        {
        }
    }

    public class ValidationException : AppException
    {
        public IDictionary<string, string[]>? Errors { get; }

        public ValidationException(string message)
            : base("VALIDATION_ERROR", message, 400)
        {
        }
        public ValidationException(string message, IDictionary<string, string[]> errors)
            : base("VALIDATION_ERROR", message, 400)
        {
            Errors = errors;
        }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base("CONFLICT", message, 409)
        {
        }
    }
}