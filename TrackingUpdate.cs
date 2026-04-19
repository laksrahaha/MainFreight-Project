using System;

namespace MainfreightProject;

//construcor class to intialzie objects , to allow those objects
public class TrackingUpdate
{
    private string updateID;
    private DateTime timeStamp;
    private string updateMessage;

//contructor class
    public TrackingUpdate (string updateID, DateTime timeStamp, string updateMessage)
    {
        this.updateID = updateID;
        this.timeStamp = timeStamp;
        this.updateMessage = updateMessage;
    }

    public void RecordUpdate()
    {
        
    }

//Viewupdate returns the tracking updates detials to the user so they can see the vhanges made.
    public string ViewUpdate()
    {
        return "Time: " + timeStamp.ToString("dd/MM/yyyy h:mm tt") +
               "\nMessage: " + updateMessage;
    }

}