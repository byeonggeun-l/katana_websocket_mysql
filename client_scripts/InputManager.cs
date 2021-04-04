using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<InputManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static InputManager m_instance; // 싱글톤이 할당될 static 변수
    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    public void AlreadyJoinIDExists()
    {
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().color = Color.red;
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().fontSize = 10;
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().text = "이미 존재하는 아이디입니다.";
        GameObject.FindGameObjectWithTag("JoinID").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("JoinPW").GetComponent<InputField>().text = "";
    }

    public void SuccessJoin()
    {
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().text = "회원가입 ID";
        GameObject.FindGameObjectWithTag("JoinID").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("JoinPW").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("JoinBtn").GetComponentInChildren<Text>().text = "회원가입 완료";
    }

    public void FailedJoin()
    {
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().text = "회원가입 ID";
        GameObject.FindGameObjectWithTag("JoinID").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("JoinPW").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("JoinBtn").GetComponentInChildren<Text>().text = "회원가입 실패";
    }

    public void SuccessLogin()
    {
        GameObject.FindGameObjectWithTag("JoinCanvas").SetActive(false);
        Debug.Log("SuccessLogin");
    }

    public void FailedLogin()
    {
        GameObject.FindGameObjectWithTag("LoginIDPlaceholder").GetComponent<Text>().color = Color.red;
        GameObject.FindGameObjectWithTag("LoginIDPlaceholder").GetComponent<Text>().fontSize = 10;
        GameObject.FindGameObjectWithTag("LoginIDPlaceholder").GetComponent<Text>().text = "로그인에 실패했습니다.";
        GameObject.FindGameObjectWithTag("LoginID").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("LoginPW").GetComponent<InputField>().text = "";
    }

    public void ClickJoinID()
    {
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().text = "회원가입 ID";
        GameObject.FindGameObjectWithTag("JoinID").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().color = Color.black;
        GameObject.FindGameObjectWithTag("JoinIDPlaceholder").GetComponent<Text>().fontSize = 14;
    }

    public void ClickLoinID()
    {
        GameObject.FindGameObjectWithTag("LoginIDPlaceholder").GetComponent<Text>().text = "로그인 ID";
        GameObject.FindGameObjectWithTag("LoginID").GetComponent<InputField>().text = "";
        GameObject.FindGameObjectWithTag("LoginIDPlaceholder").GetComponent<Text>().color = Color.black;
        GameObject.FindGameObjectWithTag("LoginIDPlaceholder").GetComponent<Text>().fontSize = 14;
    }
}
