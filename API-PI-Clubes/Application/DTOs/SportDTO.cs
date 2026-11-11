namespace API_PI_Clubes.Application.DTOs;

public class ResponseSportDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
public class SaveSportDTO
{
    public string Name { get; set; } = string.Empty;
}

public class ResponseSportBackofficeDTO : ResponseSportDTO
{
    public bool IsActive { get; set; }
}