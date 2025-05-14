namespace MyApp.Dtos;

public record RegisterDto(
  string Email,
  string Password,
  string DisplayName
);
