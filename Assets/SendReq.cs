using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class SendReq : MonoBehaviour
{
    [SerializeField] TokenGenerator Token;
    private void Update()
    {
        Token = GetComponentInChildren<TokenGenerator>();

        
    }
    public void SendRequest()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        // Create an instance of RequestData and set values
        RequestData requestData = new RequestData
        {
            video = Token.TempToken,
            text = "2",
            channelId = Token.ChannelName
        };

        // Serialize the RequestData to JSON
        string jsonData = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:8000/create", jsonData))
        {
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Request failed: " + www.error);
            }
            else
            {
                Debug.Log("Form upload complete! Response: " + www.downloadHandler.text);
            }
        }
    }
}

[System.Serializable]
public class RequestData
{
    public string video;
    public string text;
    public string channelId;
}

