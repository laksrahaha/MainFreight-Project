using System;

namespace MainfreightProject;

    //this is the user class whihc is used to stotre common user details, for all the different users in the mainfreihgt system
    public class User
    {
        // i have used private to make sure that external perosnalle cannot enter or view user informaiton, for privacu reasons
        private string userID;
        private string name;
        private string email;

        //this will intialise the user object
        public User(string userID, string name, string email)
        {
            this.userID = userID;
            this.name = name;
            this.email = email;
        }

        public string ViewProfile()
        {
            return "User ID: " + userID +
             "\nName" + name + 
             "\nEmail: " + email ;
            // returns the  user detials in one go
    }
}

  