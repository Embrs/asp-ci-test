namespace MyApp.Config;

public class JwtConfig {
  public string Issuer { get; set; } = default!;
  public string Audience { get; set; } = default!;
  public string SecretKey { get; set; } = default!;
  public int ExpireMinutes { get; set; }
}
