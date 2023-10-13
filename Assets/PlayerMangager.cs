using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class PlayerMangager : NetworkBehaviour
{
    public bool isHost;
    [SerializeField] private Camera HostCam; 
    [SerializeField] private Camera ClientCam;



    // Start is called before the first frame update
    void Start()
    {
        if(!IsLocalPlayer)
        {
            HostCam.enabled = false;
           // ClientCam.enabled = false;
           

        }
        else 
        {
           // HostCam.enabled = false;
            //ClientCam.enabled = true;
        }        
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(OwnerClientId);
        
    }
}
