using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.Rendering;

public class APIreq : MonoBehaviour
{
    public string ID;
    public string video;
    public string text;
    public string channelId;
    public string Password;
    public Message message;

    public void SendReq()
    {
        StartCoroutine(GetRequest("http://localhost:8000/getallids"));
    }
    private void Update()
    {
        video = message.video;
        channelId= message.channelId;

    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string responseText = webRequest.downloadHandler.text;

                    // Parse the JSON data
                    var jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(responseText);
                    Debug.Log(webRequest.downloadHandler.text);
                    // Access individual fields
                    if (jsonResponse.message != null && jsonResponse.message.Length > 0)
                    {
                        message = jsonResponse.message[0]; // Assuming there's only one message

                        //message.id = ID;
                        
                        Debug.Log("id: " + message.id);
                        Debug.Log("video: " + message.video);
                        Debug.Log("text: " + message.text);
                        Debug.Log("channelId: " + message.channelId);
                        Debug.Log("password: " + message.password);
                    }
                    else
                    {
                        Debug.LogError("No message found in the response.");
                    }   
                    break;

            }
        }
    }

    [System.Serializable]
    public class Message
    {
        public string id;
        public string video;
        public string text;
        public string channelId;
        public string password;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public Message[] message;
    }

}
