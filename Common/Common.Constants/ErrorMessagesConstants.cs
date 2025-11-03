namespace Common.Constants;

public static class ErrorMessagesConstants
{
    #region Global errors

    public const string ErrorKey = "ErrorMessage.";
    public const string Unavailable = "The service is unavailable.";
    public const string TooManyRequests = "Too many requests. Wait 15 min.";
    public const string Unauthorized = "Unauthorized";
    public const string InternalServerError = "Internal server error.";

    #endregion

    #region Validation errors

    public const string NotFound = "Not found.";
    public const string NotValidModel = "Invalid model.";

    #endregion

    #region WebSocker errors

    public const string WebSocketConnection = "WebSocket connection required.";

    #endregion
}
