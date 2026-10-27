using Conkist.GDK.Serialization;

namespace Champions.Data.DTOs
{
    /// <summary>Request body for POST /login.</summary>
    [System.Serializable]
    public class LoginRequest : JsonSerializableBase<LoginRequest>
    {
        public string login;
        public string password;
    }

    /// <summary>Response from POST /login with JWT pair.</summary>
    [System.Serializable]
    public class AuthResponse : JsonSerializableBase<AuthResponse>
    {
        public string access_token;
        public string refresh_token;
        public string token_type;
        public int expires_in;
    }

    /// <summary>Request body for POST /signup.</summary>
    [System.Serializable]
    public class RegisterRequest : JsonSerializableBase<RegisterRequest>
    {
        public string email;
        public string password;
        public string username;
        public string phone; // optional
    }

    /// <summary>Request body for POST /forgot-password.</summary>
    [System.Serializable]
    public class ForgotPasswordRequest : JsonSerializableBase<ForgotPasswordRequest>
    {
        public string email;
    }

    /// <summary>Request body for PATCH /update-account.</summary>
    [System.Serializable]
    public class UpdateAccountRequest : JsonSerializableBase<UpdateAccountRequest>
    {
        public string username;
        public string email;
        public string phone;
        public string password; // current password, required for confirmation
        public string new_password;
        public string new_password_confirmation;
    }
}
