namespace Application.Utilities;

public static class MessageGenerator
{
    public static string NotFound(string entity) => $"{entity} not found";
    public static string DuplicateExists(string entity) => $"Another {entity.ToLower()} with the same name already exists";
    public static string AlreadyExists(string entity) => $"{entity} with the same name already exists";

    public static string InvalidEntities(string entity) => $"One or more {entity}s are invalid";
    public static string CreationFailed(string entity) => $"{entity} could not be created";
    public static string CreationSuccess(string entity) => $"{entity} added successfully";

    public static string DeletionFailed(string entity) => $"Failed to delete {entity.ToLower()}";
    public static string DeletionSuccess(string entity) => $"{entity} deleted successfully";

    public static string UpdateFailed(string entity) => $"Failed to update {entity.ToLower()}";
    public static string UpdateSuccess(string entity) => $"{entity} updated successfully";

    public static string InvalidCredentials() => "Invalid email or password";
    public static string LoginSuccess() => "Login successful";
    public static string LogoutSuccess() => "Logged out successfully";
    public static string RegistrationFailed(string errors) => $"Registration failed: {errors}";
    public static string RegistrationSuccess() => "Registration successful";
    public static string Unauthorized() => "Unauthorized";
}