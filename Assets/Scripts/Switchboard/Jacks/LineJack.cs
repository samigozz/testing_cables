using UnityEngine;

public class LineJack : Jack
{
    public void Call()
    {
        Debug.Log($"<color=orange>{SocketID} receiving a call.</color>");
    }
}
