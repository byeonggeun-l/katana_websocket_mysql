using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    Transform tr;

    public Vector2 MousePos;
    public float angle;
    public Vector3 dir;

    public GameObject player;

    Vector3 dirNo;

    float Speed = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        tr = player.transform;
        Vector3 Pos = new Vector3(MousePos.x, MousePos.y, 0);
        dir = Pos - tr.position; // 마우스 - 플레이어 포지션을 빼면 마우스를 바라보는 벡터가 나온다.

        // 바라보는 각도 구하기.
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // normalized 단위벡터
        dirNo = new Vector3(dir.x, dir.y, 0).normalized;

        Invoke("Des", 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // 회전 적용
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 이동
        transform.position += dirNo * Speed * Time.deltaTime;
    }

    private void Des()
    {
        Destroy(gameObject);
    }
}
