namespace TodoApi.Models;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 60;
}
