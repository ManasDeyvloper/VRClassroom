using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button Server;
    [SerializeField] private Button Host;
    [SerializeField] private Button Client;
    [SerializeField] private RelayManager relayManager;
    [SerializeField] private string joincode;
    [SerializeField] Text Text;
    [SerializeField] PlayerMangager playerMangager;

    private void Awake()
    {
        Server.onClick.AddListener(() =>

        {
            
           
          
        });
        Host.onClick.AddListener(() =>
        {
            StartCoroutine(relayManager.ConfigureTransportAndStartNgoAsHost());
            playerMangager.isHost= true;
            
        });
        Client.onClick.AddListener(() =>
         
        {
             relayManager.Joincode =joincode ;
            StartCoroutine(relayManager.ConfigureTransportAndStartNgoAsConnectingPlayer());
            playerMangager.isHost = false;
            

        });
    
    }
    private void Update()
    {
       
       Text.text = relayManager.Joincode;
    }

    public void ReadInput(string input) 
    {
        joincode= input;

    }


}
