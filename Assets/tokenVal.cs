using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tokenVal : MonoBehaviour
{
   //[SerializeField] Dropdown dropdown;
    public string TempToken="0";
    public string AppID = "0";
    public string ChannelName = "0";
    //private string Token;
    Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void OnDropdownValueChanged(int value)
    {
        // Perform an action based on the selected option
        switch (value)
        {
            case 0:
                TempToken = "007eJxTYGiTmnFy41r3HVe+ZxUEb/2R61I71ebmAbGkh8c5uNeF34lUYEhLM05NNktKMTYzNTVJM0q0ME+2SDNMski0TEtLNkgyKbsuntoQyMgwuyedhZEBAkF8ToawzJTUfOfEnBwGBgChjiNV";
                AppID= "ff3ec6bd36554f2a87c8f1b8a9ffc0b4";
                ChannelName = "VideoCall";
                Debug.Log("radhe Radhe");


                break;
            case 1: 
                TempToken= "radhe radhe";
                
                break;
            case 2:
               TempToken= "raadhe";
                
                break;
            case 3:
                
                break;
            case 4:
                
                break;
            case 5:
               
                break;

        }

    }
   
}
