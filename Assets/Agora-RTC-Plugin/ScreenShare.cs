using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using Agora.Rtc;
using TMPro;
using Unity.VisualScripting;

public class ScreenShare : MonoBehaviour
{
    //public  static tokenVal token = new tokenVal();  
    // Fill in your app ID.
    [SerializeField] public string _appID;
    // Fill in your channel name.
    [SerializeField] public string _channelName;
    // Fill in the temporary token you obtained from Agora Console.
    [SerializeField] public string _token; //token.TempToken;
    // A variable to save the remote user uid.

    private string clientRole = "";
    // A variable to save the remote user uid.
    private uint remoteUid;
    private Toggle toggle1;
    private Toggle toggle2;
    internal VideoSurface LocalView;
    private VideoSurface ScreenView;
    internal VideoSurface RemoteView;
    internal IRtcEngine RtcEngine;
    // Volume Control
    private Slider volumeSlider;
    // Screen sharing
    private bool sharingScreen = false;
    private TMP_Text shareScreenBtnText;
    [SerializeField] private int source;
    private APIreq apiReq;



    private ArrayList permissionList = new ArrayList() { Permission.Camera, Permission.Microphone };

    private void CheckPermissions()
    {

        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }


    }
    private void SetupUI()
    {
        GameObject go = GameObject.Find("LocalView");
        LocalView = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, 180.0f);
        go = GameObject.Find("RemoteView");
        RemoteView = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, 180.0f);
        go = GameObject.Find("ScreenView");
        RemoteView = go.AddComponent<VideoSurface>();
        
        go = GameObject.Find("Leave");
        go.GetComponent<Button>().onClick.AddListener(Leave);
        go = GameObject.Find("Join");
        go.GetComponent<Button>().onClick.AddListener(Join);
        GameObject Obj1 = GameObject.Find("Broadcaster");
        toggle1 = Obj1.GetComponent<Toggle>();
        toggle1.isOn = false;
        toggle1.onValueChanged.AddListener((value) =>
        {
            Func1(value);
        });
        GameObject Obj2 = GameObject.Find("Audience");
        toggle2 = Obj2.GetComponent<Toggle>();
        toggle2.isOn = false;
        toggle2.onValueChanged.AddListener((value) =>
        {
            Func2(value);

        });
        // Access the button from the UI.
        go = GameObject.Find("shareScreen");
        // Add a listener to the button and invokes shareScreen when the button is pressed.
        go.GetComponent<Button>().onClick.AddListener(shareScreen);
        // Access the text sub-item of screen sharing button to change the button text.
        shareScreenBtnText = go.GetComponentInChildren<TextMeshProUGUI>(true);
        shareScreenBtnText.text = "Share Screen";
        shareScreenBtnText.fontSize = 14;
        // Access the slider from the UI
        go = GameObject.Find("mediaProgressBar");
        volumeSlider = go.GetComponent<Slider>();
        // Specify a maximum value for slider.
        go.GetComponent<Slider>().maxValue = 100;
        // Add a listener to the slider and invokes changeVolume when the value changes.
        go.GetComponent<Slider>().onValueChanged.AddListener(delegate { changeVolume((int)volumeSlider.value); });
        // Access the Mute toggle from the UI.
        go = GameObject.Find("Mute");
        Toggle muteToggle = go.GetComponent<Toggle>();
        muteToggle.isOn = false;
        // Invoke muteRemoteAudio when the user taps the toggle button.
        muteToggle.onValueChanged.AddListener((value) =>
        {
            muteRemoteAudio(value);
        });

    }
    private void SetupVideoSDKEngine()
    {
        // Create an instance of the video SDK.
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        // Specify the context configuration to initialize the created instance.
        RtcEngineContext context = new RtcEngineContext(_appID, 0,
        CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION,
        AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT, AREA_CODE.AREA_CODE_GLOB, null);
        // Initialize the instance.
        RtcEngine.Initialize(context);
    }
    private void InitEventHandler()
    {
        // Creates a UserEventHandler instance.
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngine.InitEventHandler(handler);
    }

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly ScreenShare _videoSample;

        internal UserEventHandler(ScreenShare videoSample)
        {
            _videoSample = videoSample;
        }
        // This callback is triggered when the local user joins the channel.
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("You joined channel: " + connection.channelId);
        }
        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            // Setup remote view.
            _videoSample.RemoteView.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            if (_videoSample.clientRole == "Audience")
            {
                // Start rendering remote video.
                _videoSample.RemoteView.SetEnable(true);
            }
            _videoSample.remoteUid = uid;


        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            if (newRole == CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER)
            {

                _videoSample.LocalView.SetEnable(true);
                _videoSample.RemoteView.SetEnable(false);
                GameObject go = GameObject.Find("RemoteView");
                go.GetComponent<RawImage>().enabled = false;
                go = GameObject.Find("LocalView");
                go.GetComponent<RawImage>().enabled = true;


                Debug.Log("Role changed to Broadcaster");
            }
            else
            {
                _videoSample.LocalView.SetEnable(false);
                _videoSample.RemoteView.SetEnable(true);
                GameObject go = GameObject.Find("RemoteView");
                go.GetComponent<RawImage>().enabled = true;
                go = GameObject.Find("LocalView");
                go.GetComponent<RawImage>().enabled = false;
                Debug.Log("Role changed to Audience");
            }
        }


        // This callback is triggered when a remote user leaves the channel or drops offline.
        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            _videoSample.RemoteView.SetEnable(false);
        }
    }
    public void Join()
    {
        if (toggle1.isOn == false && toggle2.isOn == false)
        {
            Debug.Log("Select a role first");
        }
        else
        {
            Debug.Log("JOIN");
            // Enable the video module.
            RtcEngine.EnableVideo();
            // Set the local video view.
            LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA);
            // Join a channel.
            RtcEngine.JoinChannel(_token, _channelName);
        }
    }

    // This callback is triggered when a remote user leaves the channel or drops offline.

    public void Leave()
    {
        // Leaves the channel.
        RtcEngine.LeaveChannel();
        // Disable the video modules.
        RtcEngine.DisableVideo();
        // Stops rendering the remote video.
        RemoteView.SetEnable(false);
        // Stops rendering the local video.
        LocalView.SetEnable(false);
    }

    private void Start()
    {
        //tokenVal tokenVal = GetComponentInChildren<tokenVal>();
        // _token = tokenVal.TempToken;
        //apiReq = GetComponentInChildren<APIreq>();
        SetupVideoSDKEngine();
        InitEventHandler();
        SetupUI();
    }
    private void Update()
    {
         APIreq token = GetComponentInChildren<APIreq>();
        _token = token.video;
        _channelName = token.channelId;
        _appID = "ff3ec6bd36554f2a87c8f1b8a9ffc0b4";
        //_channelName = token.channelId;
        CheckPermissions();
    }
    public void UpdateValue() 
    {
        
    }

   
    private void OnApplicationQuit()
    {
        if (RtcEngine != null)
        {
            Leave();
            RtcEngine.Dispose();
            RtcEngine = null;
        }

    }

    void Func1(bool value)
    {
        if (value == true)
        {
            toggle2.isOn = false;
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            clientRole = "Host";
        }
    }
    void Func2(bool value)
    {
        if (value == true)
        {
            toggle1.isOn = false;
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
            clientRole = "Audience";
        }
    }
    private void changeVolume(int volume)
    {
        // Adjust the recorded signal volume.
        RtcEngine.AdjustRecordingSignalVolume(volume);
    }
    private void muteRemoteAudio(bool value)
    {
        // Pass the uid of the remote user you want to mute.
        RtcEngine.MuteRemoteAudioStream(System.Convert.ToUInt32(remoteUid), value);
    }
    private void updateChannelPublishOptions(bool publishMediaPlayer)
    {
        ChannelMediaOptions channelOptions = new ChannelMediaOptions();
        channelOptions.publishScreenTrack.SetValue(publishMediaPlayer);
        //channelOptions.publishAudioTrack.SetValue(true);
        channelOptions.publishSecondaryScreenTrack.SetValue(publishMediaPlayer);
        channelOptions.publishCameraTrack.SetValue(!publishMediaPlayer);
        RtcEngine.UpdateChannelMediaOptions(channelOptions);
    }
    private void setupLocalVideo(bool isScreenSharing)
    {
        if (isScreenSharing)
        {
            GameObject go = GameObject.Find("ScreenView");
           // go.transform.Rotate(180.0f, 0.0f, 0);
            // Update the VideoSurface component of the local view.
            ScreenView = go.AddComponent<VideoSurface>();
            // Render the screen sharing track on the local view.
            ScreenView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN_PRIMARY);

        }
        else if (!isScreenSharing)
        {
            if (LocalView != null)
            {
                GameObject go = GameObject.Find("ScreenView");
                ScreenView = go.GetComponent<VideoSurface>();
                Destroy(ScreenView);
            }
        }
      //  else
        {
            GameObject go = GameObject.Find("LocalView");
            go.transform.Rotate(180.0f, 0.0f, 180.0f);
            // Update the VideoSurface component of the local view.
            LocalView = go.AddComponent<VideoSurface>();
            // Render the local video track on the local view.
            LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA_PRIMARY);
        }
    }
    public void shareScreen() 
{
    if (!sharingScreen)
    {
        // The target size of the screen or window thumbnail (the width and height are in pixels).
        SIZE t = new SIZE(360,240);
        // The target size of the icon corresponding to the application program (the width and height are in pixels)
        SIZE s = new SIZE(360,240);
        // Get a list of shareable screens and windows
        var info =  RtcEngine.GetScreenCaptureSources(t, s, true);

            // Get the first source id to share the whole screen.
            ulong dispId = (ulong)info[source].sourceId;
        // To share a part of the screen, specify the screen width and size using the Rectangle class.
        RtcEngine.StartScreenCaptureByWindowId(System.Convert.ToUInt32(dispId), new Rectangle(),
                default(ScreenCaptureParameters));
        // Publish the screen track and unpublish the local video track.
        updateChannelPublishOptions(true);
        // Display the screen track in the local view.
        setupLocalVideo(true);
        // Change the screen sharing button text.
        shareScreenBtnText.text = "Stop Sharing";
        // Update the screen sharing state.
        sharingScreen = true;
    }          
    else 
    { 
        // Stop sharing.
        RtcEngine.StopScreenCapture();
        // Publish the local video track when you stop sharing your screen.
        updateChannelPublishOptions(false);
        // Display the local video in the local view.
        setupLocalVideo(false);
        // Update the screen sharing state.
        sharingScreen = false;
        // Change to the default text of the button when you stop sharing your screen.
        shareScreenBtnText.text = "Share Screen";
    }
}




















}
