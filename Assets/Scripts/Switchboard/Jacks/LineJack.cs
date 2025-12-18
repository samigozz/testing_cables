using UnityEngine;

public class LineJack : Jack
{
    [SerializeField, ColorUsage(false, true)] 
    private Color ringingColor = Color.red;

    public Call activeCall = null;
    public bool IsOccupied => currentPlug != null && activeCall != null;
    
    public void InitCall(Call call)
    {
        activeCall = call;

        if (activeCall == null)
            return;
        
        lightIndicator?.UpdateLight(ringingColor, true);
    }
    
    public void TimeOutCall()
    {
        activeCall = null;
        lightIndicator?.UpdateLight(offColor, true);
    }
}
