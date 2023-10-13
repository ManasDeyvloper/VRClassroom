using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgoraIO.Media
{
    public class TokenGen : MonoBehaviour
    {
        private string token;
        public string TempToken;

        public enum Role
        {
            ROLE_PUBLISHER = 1,
            ROLE_SUBSCRIBER = 2
        }
        public string buildTokenWithUid(string appId, string appCertificate, string channelName, int uid,
                                        int tokenExpire, int joinChannelPrivilegeExpire, int pubAudioPrivilegeExpire,
                                        int pubVideoPrivilegeExpire, int pubDataStreamPrivilegeExpire)
        {
            return buildTokenWithUserAccount(appId, appCertificate, channelName, AccessToken2.getUidStr(uid),
                    tokenExpire, joinChannelPrivilegeExpire, pubAudioPrivilegeExpire, pubVideoPrivilegeExpire, pubDataStreamPrivilegeExpire);
        }
        public string buildTokenWithUserAccount(string appId, string appCertificate, string channelName, string account,
                                               int tokenExpire, int joinChannelPrivilegeExpire, int pubAudioPrivilegeExpire,
                                               int pubVideoPrivilegeExpire, int pubDataStreamPrivilegeExpire)
        {
            AccessToken2 accessToken = new AccessToken2(appId, appCertificate, tokenExpire);
            AccessToken2.Service serviceRtc = new AccessToken2.ServiceRtc(channelName, account);

            serviceRtc.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_JOIN_CHANNEL, joinChannelPrivilegeExpire);
            serviceRtc.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_PUBLISH_AUDIO_STREAM, pubAudioPrivilegeExpire);
            serviceRtc.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_PUBLISH_VIDEO_STREAM, pubVideoPrivilegeExpire);
            serviceRtc.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_PUBLISH_DATA_STREAM, pubDataStreamPrivilegeExpire);
            accessToken.addService(serviceRtc);
            token = accessToken.build();
            return token;
           

           
            
        }
        
        
        

    }
}
