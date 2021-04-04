using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UnityEngine.UI;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update

    //public GameObject inputFieldJoinID;
    //public GameObject inputFieldJoinPW;
    //public GameObject inputFieldJoinBtn;

    public GameObject playerPrefab;

    // 회원가입 & 로그인 이벤트.
    public static List<string> lstMessages = new List<string>();
    // 로그인 완료 이벤트.
    public static List<SuccessLoginJSON> lstSuccessLoginMessages = new List<SuccessLoginJSON>();
    // 플레이어 무빙 이벤트.
    public static List<MoveJSON> lstMoveMessages = new List<MoveJSON>();
    // 다른 플레이어 만들기.
    public static List<SuccessLoginJSON> lstAnotherPlayer = new List<SuccessLoginJSON>();
    // 사과 던지기 이벤트.
    public static List<ThrowAppleJSON> lstThrowApple = new List<ThrowAppleJSON>();
    // 나간 플레이어 삭제 시키기 이벤트.
    public static List<DeleteQuitPlayerJSON> lstDeleteQuitPlayer = new List<DeleteQuitPlayerJSON>();



    public static List<Vector3> lstPlayerVector3 = new List<Vector3>();


    public static Dictionary<string, GameObject> dicClients = new Dictionary<string, GameObject>();
    //public static List<PlayerController> lstClients = new List<PlayerController>();

    public static string loginID = "";

    


    public static WebSocket WebSocket
    {
        get
        {
            // 싱글톤 오브젝트를 반환
            return m_WebSocket;
        }
    }
    private static WebSocket m_WebSocket; // 싱글톤이 할당될 static 변수

    void Start()
    {
        m_WebSocket = new WebSocket("ws://localhost:3000");
        m_WebSocket.Connect();

        m_WebSocket.OnMessage += GetMessage;
    }


    public void GetMessage(object sender, MessageEventArgs e)
    {
        Debug.Log($"{((WebSocket)sender).Url}에서 + 데이터 : {e.Data}가 옴.");
        // 메인스레드에서 처리할 것 이외에 이 부분에서 처리해야할 것이 있다면..
        if (e.Data.Contains("AlreadyExists") || e.Data.Contains("SuccessJoin") ||
            e.Data.Contains("SuccessJoin") || e.Data.Contains("FailedLogin")) 
        {
            DefaultTypeJSON data = JsonUtility.FromJson<DefaultTypeJSON>(e.Data);
            lstMessages.Add(data.Type);
        }
        else if (e.Data.Contains("SuccessLogin"))
        {
            SuccessLoginJSON data = JsonUtility.FromJson<SuccessLoginJSON>(e.Data);
            loginID = data.ID;
            Debug.Log("로그인 성공 : " + loginID);
            Debug.Log("로그인 성공 : " + data.x);
            Debug.Log("로그인 성공 : " + data.y);
            lstMessages.Add(data.Type);
            lstSuccessLoginMessages.Add(data);
        }
        else if (e.Data.Contains("AnotherPlayerJoin"))
        {
            SuccessLoginJSON data = JsonUtility.FromJson<SuccessLoginJSON>(e.Data);
            lstAnotherPlayer.Add(data);
        }
        else if (e.Data.Contains("PlayerMove"))
        {
            MoveJSON data = JsonUtility.FromJson<MoveJSON>(e.Data);
            lstMoveMessages.Add(data);
        }
        else if (e.Data.Contains("ThrowApple"))
        {
            ThrowAppleJSON data = JsonUtility.FromJson<ThrowAppleJSON>(e.Data);
            lstThrowApple.Add(data);
        }
        else if (e.Data.Contains("DeleteQuitPlayer"))
        {
            DeleteQuitPlayerJSON data = JsonUtility.FromJson<DeleteQuitPlayerJSON>(e.Data);
            lstDeleteQuitPlayer.Add(data);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 회원가입 & 로그인 이벤트.
        if (lstMessages.Count != 0)
        {
            string message = lstMessages[0].ToString();
            lstMessages.RemoveAt(0);

            if (message.Equals("AlreadyExists"))
            {
                InputManager.instance.AlreadyJoinIDExists();
            }
            else if (message.Equals("SuccessJoin"))
            {
                InputManager.instance.SuccessJoin();
            }
            else if (message.Equals("FailedJoin"))
            {
                InputManager.instance.FailedJoin();
            }
            else if (message.Equals("SuccessLogin"))
            {
                InputManager.instance.SuccessLogin();
                MakePlayer(true);
            }
            else if (message.Equals("FailedLogin"))
            {
                InputManager.instance.FailedLogin();
            }
        }

        // 플레이어 무빙
        if(lstMoveMessages.Count != 0)
        {
            PlayerMove();
        }

        if(lstAnotherPlayer.Count != 0)
        {
            MakeAnotherPlayer();
        }

        if(lstThrowApple.Count != 0)
        {
            ThrowApple();
        }

        if(lstDeleteQuitPlayer.Count != 0)
        {
            DeleteQuitPlayer();
        }


    }

    public void DeleteQuitPlayer()
    {
        DeleteQuitPlayerJSON data = lstDeleteQuitPlayer[0];
        lstDeleteQuitPlayer.RemoveAt(0);

        dicClients[data.ID].GetComponent<PlayerController>().Des();
        dicClients[data.ID] = null;
        dicClients.Remove(data.ID);
    }

    public void ThrowApple()
    {
        ThrowAppleJSON data = lstThrowApple[0];
        lstThrowApple.RemoveAt(0);

        Debug.Log("던지자 : " + data.ID);
        Debug.Log("던지자 2-1 : " + data.x);
        Debug.Log("던지자 2-2 : " + data.y);

        GameObject go = dicClients[data.ID];
        go.GetComponent<PlayerController>().ThrowApple(data.x, data.y);
    }

    public void PlayerMove()
    {
        MoveJSON data = lstMoveMessages[0];
        lstMoveMessages.RemoveAt(0);


        GameObject go = dicClients[data.ID];
        go.GetComponent<PlayerController>().Move(data.x);
    }


    // 일단 분리는 해보자.
    public void MakeAnotherPlayer()
    {
        SuccessLoginJSON data = lstAnotherPlayer[0];
        lstAnotherPlayer.RemoveAt(0);
        Vector3 playerVec3 = new Vector3(data.x, data.y, 0.0f);

        playerPrefab.GetComponent<PlayerController>().playerName = data.ID;
        playerPrefab.GetComponent<PlayerController>().isHost = false;
        Debug.Log(data.ID + " : 접속");
        playerPrefab.GetComponent<PlayerController>().level = data.Level;
        GameObject go = Instantiate(playerPrefab, playerVec3, Quaternion.identity);

        dicClients.Add(data.ID, go);
    }

    public void MakePlayer(bool myPlayer)
    {
        if (myPlayer)
            playerPrefab.GetComponent<PlayerController>().isHost = true;

        SuccessLoginJSON data = lstSuccessLoginMessages[0];
        lstSuccessLoginMessages.RemoveAt(0);
        Vector3 playerVec3 = new Vector3(data.x, data.y, 0.0f);

        playerPrefab.GetComponent<PlayerController>().playerName = data.ID;
        playerPrefab.GetComponent<PlayerController>().level = data.Level;
        GameObject go = Instantiate(playerPrefab, playerVec3, Quaternion.identity);

        dicClients.Add(data.ID, go);
    }    

    public void SignUp()
    {
        string ID = GameObject.FindGameObjectWithTag("JoinID").GetComponent<InputField>().text;
        string PW = GameObject.FindGameObjectWithTag("JoinPW").GetComponent<InputField>().text;
        string data = JsonUtility.ToJson(new SingUpAndLoginJSON("SingUp", ID, PW));

        m_WebSocket.Send(data);
    }

    public void Login()
    {
        string ID = GameObject.FindGameObjectWithTag("LoginID").GetComponent<InputField>().text;
        string PW = GameObject.FindGameObjectWithTag("LoginPW").GetComponent<InputField>().text;
        string data = JsonUtility.ToJson(new SingUpAndLoginJSON("Login", ID, PW));

        m_WebSocket.Send(data);
    }

    private void OnDestroy()
    {
        string data = JsonUtility.ToJson(new DestroyJSON("Destroy", loginID));
        m_WebSocket.Send(data);
    }

    public class DestroyJSON
    {
        public string Type;
        public string ID;

        public DestroyJSON(string _type, string _id)
        {
            Type = _type;
            ID = _id;
        }
    }

    public class DeleteQuitPlayerJSON
    {
        public string Type;
        public string ID;
    }

    public class ThrowAppleJSON
    {
        public string Type;
        public string ID;
        public float x;
        public float y;
    }



    public class SingUpAndLoginJSON
    {
        public string Type;
        public string ID;
        public string PW;

        public SingUpAndLoginJSON(string _type, string _id, string _pw)
        {
            Type = _type;
            ID = _id;
            PW = _pw;
        }
    }

    public class DefaultTypeJSON
    {
        public string Type;

        public DefaultTypeJSON(string _type)
        {
            Type = _type;
        }
    }

    public class SuccessLoginJSON
    {
        public string Type;
        public string ID;
        public int Level;
        public float x;
        public float y;
    }

    public class MoveJSON
    {
        public string Type;
        public string ID;
        public float x;
        public float y;
    }





}
