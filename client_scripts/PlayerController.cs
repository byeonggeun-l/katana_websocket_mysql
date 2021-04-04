using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isHost = false;
    public Vector3 direction;
    public string playerName = "";
    public int level;

    public GameObject applePrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Transform>().localScale *= (level + 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (isHost)
        {
            float x = Input.GetAxisRaw("Horizontal");
            if (x != 0.0f)
            {
                HorizontalMoveJSON data = new HorizontalMoveJSON("Move", playerName, x, transform.position.x);

                string _data = JsonUtility.ToJson(data);
                NetworkManager.WebSocket.Send(_data);
            }
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 vecMousePoint = GetMouseWorldPoint();

                //applePrefab.GetComponent<Apple>().player = gameObject;
                //applePrefab.GetComponent<Apple>().MousePos = GetMouseWorldPoint();
                //Instantiate(applePrefab, transform.position, Quaternion.identity);
                ThrowAppleJSON data = new ThrowAppleJSON("ThrowApple", playerName, vecMousePoint);

                string _data = JsonUtility.ToJson(data);
                NetworkManager.WebSocket.Send(_data);
            }
        }
    }

    public void ThrowApple(float x, float y)
    {
        Vector2 vecMousePoint = new Vector2(x, y);

        applePrefab.GetComponent<Apple>().player = gameObject;
        applePrefab.GetComponent<Apple>().MousePos = vecMousePoint;
        Instantiate(applePrefab, transform.position, Quaternion.identity);
        ThrowAppleJSON throwAPple = new ThrowAppleJSON("ThrowApple", playerName, vecMousePoint);
    }

    public void Move(float x)
    {
        transform.position += new Vector3(x, 0.0f, 0.0f) * 6.0f * Time.deltaTime;
        NewestPositionJSON data = new NewestPositionJSON("NewestPosition", playerName, transform.position.x, transform.position.y);
        string _data = JsonUtility.ToJson(data);
        NetworkManager.WebSocket.Send(_data);
    }

    private Vector2 GetMouseWorldPoint()
    {
        Vector2 vecMousePoint = Input.mousePosition;
        vecMousePoint = Camera.main.ScreenToWorldPoint(vecMousePoint);

        return vecMousePoint;
    }

    public void Des()
    {
        Destroy(gameObject);
    }


    public class HorizontalMoveJSON
    {
        public string Type;
        public string ID;
        public float x;
        public float currentX;

        public HorizontalMoveJSON(string _type, string _id, float _x, float _currentX)
        {
            Type = _type;
            ID = _id;
            x = _x;
            currentX = _currentX;
        }
    }

    public class ThrowAppleJSON
    {
        public string Type;
        public string ID;
        public float x;
        public float y;

        public ThrowAppleJSON(string _type, string _id, Vector2 mousePoint)
        {
            Type = _type;
            ID = _id;
            x = mousePoint.x;
            y = mousePoint.y;
        }
    }

    public class NewestPositionJSON
    {
        public string Type;
        public string ID;
        public float x;
        public float y;

        public NewestPositionJSON(string _type, string _id, float _x, float _y)
        {
            Type = _type;
            ID = _id;
            x = _x;
            y = _y;
        }
    }



}
