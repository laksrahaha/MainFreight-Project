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

    public string ViewUpdate()
    {
        return updateMessage;
    }

}
