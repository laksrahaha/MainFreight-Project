using System;
using System.Collections.Generic;
using System.IO;

namespace MainfreightProject;

// AuthenticationService validates Staff/Admin login details using the local accounts file.
// Login checking is separated from MainfreightForm so the boundary layer does not contain any of the account loading logic.
// This supports modularity and allows the account store to be replaced later without later changing shipment workflows.
public class AuthenticationService
{
    private string accountFilePath;

    public AuthenticationService(string accountFilePath)
    {
        this.accountFilePath = accountFilePath;
    }

    public UserAccount Authenticate(string username, string password)
    {
        List<UserAccount> accounts = LoadAccounts();

        foreach (UserAccount account in accounts)
        {
            if (account.MatchLogin(username, password))
            {
                return account;
            }
        }

        return null;
    }

    public List<UserAccount> LoadAccounts()
    {
        List<UserAccount> accounts = new List<UserAccount>();

        if (!File.Exists(accountFilePath))
        {
            return accounts;
        }

        string[] lines = File.ReadAllLines(accountFilePath);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            if (parts.Length == 5 &&
                Enum.TryParse(parts[2], out UserRole role) &&
                bool.TryParse(parts[4], out bool isActive))
            {
                UserAccount account = new UserAccount(
                    parts[0],
                    parts[1],
                    role,
                    parts[3],
                    isActive
                );

                accounts.Add(account);
            }
        }

        return accounts;
    }
}