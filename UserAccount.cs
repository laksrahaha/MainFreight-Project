namespace MainfreightProject;

// UserAccount represents an authenticated internal system account.
// It stores login credentials, role information, linked user identity, and active status.
// This keeps account related data encapsulated instead of spreading login fields through the GUI casuing more dependency.
public class UserAccount
{
    private string username;
    private string password;
    private UserRole role;
    private string linkedUserID;
    private bool isActive;

    public UserAccount(string username, string password, UserRole role, string linkedUserID, bool isActive)
    {
        this.username = username;
        this.password = password;
        this.role = role;
        this.linkedUserID = linkedUserID;
        this.isActive = isActive;
    }

    public string Username
    {
        get { return username; }
    }

    public UserRole Role
    {
        get { return role; }
    }

    public string LinkedUserID
    {
        get { return linkedUserID; }
    }

    public bool IsActive
    {
        get { return isActive; }
    }

    public bool MatchLogin(string enteredUsername, string enteredPassword)
    {
        return isActive &&
               username == enteredUsername &&
               password == enteredPassword;
    }

    public void ResetPassword(string newPassword)
    {
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            password = newPassword;
        }
    }

    public void Deactivate()
    {
        isActive = false;
    }
}