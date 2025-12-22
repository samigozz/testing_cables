using System;
using TMPro;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private int charLimit = 9;

    private int _charAmount = 0;

    private void Start()
    {
        displayText.text = "";
    }
    
    public void AddNum(string number)
    {
        if (_charAmount == charLimit)
            return;
        
        displayText.text += number;
        _charAmount++;
    }

    public void DeleteChar()
    {
        if (_charAmount == 0)
            return;
        
        displayText.text = displayText.text.Remove(displayText.text.Length - 1, 1);
        _charAmount--;
    }
    
    public void ClearDisplay()
    {
        if (_charAmount == 0)
            return;
        
        displayText.text = "";
        _charAmount = 0;
    }

    public void CallNumber()
    {
        
    }
    
    public void HangCall()
    {
        
    }
}
