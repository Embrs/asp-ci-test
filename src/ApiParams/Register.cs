namespace MyApp.ApiParams;

public record RegisterParams(
  string Email,
  string Password,
  string DisplayName
);
