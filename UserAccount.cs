using System;

namespace MainfreightProject;

public class UserAccount
{
    private string username;
    private string password;
    private Boolean isActive;
    

    public UserAccount(string username, string password, Boolean isActive)
    {
        this.username = username;
        this.password = password;
        this.isActive = isActive;

        
    }

    public void ResetPassword()
    {
        
    }

    public void Deactivate()
    {
        
    }


}
