using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField]
    float speed;
        
    //배경 스크롤링
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    //카메라 높이
    float viewHeghit;

    void Awake()
    {
        //현재 씬에서 사용하는 메인 카메라의 실제 View 높이
        // Camera View 높이 = Size * 2 
        viewHeghit = Camera.main.orthographicSize * 2;
    }

    void Update()
    {
        Move();
        Scrolling();

    }

    void Move()
    {
        //transform을 이용한 이동구현
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector2.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void Scrolling()
    {

        //마지막 배경이 카메라 position을 넘어 갔을 때
        // = y축이 -10 밑으로 넘어 갔을 때
        if (sprites[endIndex].position.y < viewHeghit * (-1))
        {
            //#.Sprite Reuse
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            //Transform은 월드 좌표가 기준이기 떄문에 localPosition
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;

            //이동이 완료 되면 EndIndex , StartIndex 갱신
            //#. Cursor index Change
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }

    }
}
