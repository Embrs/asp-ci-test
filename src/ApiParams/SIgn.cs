namespace MyApp.ApiParams;

public record SignInParams(
  string Email, 
  string Password
);

public record SignUpParams(
  string Email,
  string Password,
  string DisplayName
);
