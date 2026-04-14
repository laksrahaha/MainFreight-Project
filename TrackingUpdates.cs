using System;

namespace MainfreightProject;

public class TrackingUpdates
{
    private string updateID;
    private DateTime timeStamp;
    private string updateMessage;

    public TrackingUpdates (string updateID, DateTime timeStamp, string updateMessage)
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
        return "Update ID: " + updateID +
               "\nTime: " + timeStamp +
               "\nMessage: " + updateMessage;
    }

}
