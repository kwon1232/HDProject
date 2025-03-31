using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    private ChatClient chatClient;
    private string chatChannel = "GlobalChannel";

    public void Initalize()
    {
        if(string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            PhotonNetwork.NickName = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";
        }

        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }

    private void Update()
    {
        chatClient?.Service();
    }

    public void SendMeesageToChat(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            chatClient.PublishMessage(chatChannel, message);
        }
        Debug.Log(message + " : 채팅을 보냈습니다. ");
    }

    #region ChatClient_Interface
    // Photon.Chat 클라이언트에서 발생하는 디버깅 메시지
    // 매개변수 - level(Error, Warning, Info), Message 
    public void DebugReturn(DebugLevel level, string message)
    {
        switch(level)
        {
            case DebugLevel.ERROR:
                Debug.LogError($"Photon Chat Error : {message}");
                break;
            case DebugLevel.WARNING:
                Debug.LogWarning($"Photon Chat Warning : {message}");
                break;
            default :
                Debug.Log($"Photon Chat : {message}");
                break;
        }
    }

    // Photon.Chat 클라이언트의 상태가 변경될 때 호출된다.
    // 매개변수 : state(ChatState 열거형 값 (ENUM), 클라이언트 현재 상태 (Connected, Connecting, Disconnected)
    // ex) 특정 캐릭터 머리위 말풍선 구현 시 사용
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat State Changed : {state}");
        //
        // ConnectedToNameServer : Name Server와의 연결이 완료된 상태
        // Authenticated : 인증이 완료되어 채팅 서버와 연결할 준비가 된 상태
        // Disconnected : 연결이 끊긴 상태
        // ConnectedToFrontEnd : Front-End 서버와 연결된 상태
        //
        switch(state)
        {
            case ChatState.ConnectedToNameServer:
                Debug.Log($"Connected to Name Server");
                break;
            case ChatState.Authenticated:
                Debug.Log($"Authenticated successfully.");
                break;
            case ChatState.Disconnected:
                Debug.Log($"Connected to Front End Server");
                break;
            case ChatState.ConnectedToFrontEnd:
                Debug.Log($"Connected to Front End Server");
                break;
            default:
                Debug.Log($"Unhandled Chat State : {state}");
                break;
        }
    }

    // Photon.Chat 서버와 연결이 되었을 때 호출된다.
    public void OnConnected()
    {
        Debug.Log("Photon OnConnected");

        chatClient.Subscribe(new string[] { chatChannel });
    }

    // Photon.Chat 서버와 연결이 끊어졌을 때 호출이 된다.
    public void OnDisconnected()
    {
        Debug.Log("Photon OnDisconnected");
    }

    // 특정 채널에서 메시지를 수신했을 때 호출된다.
    // 여러 개의 채팅 채널이 생겨났을 때, 각각의 채널에서 어떤 메시지가 수신되는지의 호출을 구분해준다.
    // ex) 월드, 길드 메시지 등
    // 매개 변수 : channelName : 메시지가 수신된 채널 이름, sender : 메시지가 보낸 사용자 이름 배열, messages 
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            string receivedMessage = $"{senders[i]}: {messages[i]}";
            Debug.Log($"[{channelName}] {receivedMessage}");

            ChatUIManager.instance.DisplayMessage(receivedMessage);

            Player senderPlayer = PhotonNetwork.PlayerList.FirstOrDefault(p => p.NickName == senders[i]);
            if (senderPlayer != null)
            {
                int actorNumber = senderPlayer.ActorNumber;
                string message = messages[i].ToString();

                BubbleUIManager.instance.ShowBubbleForPlayer(actorNumber, message);
            }
        }
    }

    // 다른 플레이어가 보낸 개인 메시지를 수신했을 때 호출된다.
    // 매개 변수 : sender : 메시지를 보낸 사용자 이름, message : 메시지 내용, channelName : 메시지가 속한 채널 이름
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    // 특정 사용자의 상태가 변경되었을 때 호출된다.
    // ex) 친구의 온라인 오프라인 상태 체크 
    // 매개 변수 : user : 상태가 변경된 사용자, status : 새로운 상태 코드 (온라인, 오프라인, 자리 비움, 바쁨)
    // gotMessage : 상태 변경 시 추가 메시지 여부, message : 상태 변경과 함께 전달된 메시지
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    // 채널 구독 요청이 성공적으로 처리되었을 때 호출된다.
    // channels : 구독한 채널 이름 배열, results : 각 채널의 구독 성공
    // ex) 특정 길드 가입 후 길드 채널 구독
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            if (results[i])
            {
                Debug.Log($"Subscribed to channel: {channels[i]}");
            }
            else
            {
                Debug.LogError($"Failed to subscribe to channel: {channels[i]}");
            }
        }
    }

    // 채널 구독 해제 요청이 처리되었을 때 호출된다.
    // channels : 구독 해제된 채널 이름 배열
    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    // 특정 사용자가 채널에 구독했을 때 호출된다.
    // channel : 사용자가 구독한 채널 이름, user : 구독한 사용자 이름 
    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    // 특정 사용자가 채널 구독을 해제했을 때 호출된다.
    // channel : 사용자가 구독 해제한 채널 이름, user : 구독 해제한 사용자 이름
    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    #endregion


}

