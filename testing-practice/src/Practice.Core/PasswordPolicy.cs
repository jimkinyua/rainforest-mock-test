namespace Practice.Core;

public static class PasswordPolicy
{
    public static IReadOnlyList<string> Validate(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return ["Password is required."];
        }

        var errors = new List<string>();

        if (password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters.");
        }

        if (!password.Any(char.IsDigit))
        {
            errors.Add("Password must contain a digit.");
        }

        if (!password.Any(char.IsUpper))
        {
            errors.Add("Password must contain an uppercase letter.");
        }

        return errors;
    }
}
