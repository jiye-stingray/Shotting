using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField]
    float speed;
        
    //��� ��ũ�Ѹ�
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    //ī�޶� ����
    float viewHeghit;

    void Awake()
    {
        //���� ������ ����ϴ� ���� ī�޶��� ���� View ����
        // Camera View ���� = Size * 2 
        viewHeghit = Camera.main.orthographicSize * 2;
    }

    void Update()
    {
        Move();
        Scrolling();

    }

    void Move()
    {
        //transform�� �̿��� �̵�����
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector2.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void Scrolling()
    {

        //������ ����� ī�޶� position�� �Ѿ� ���� ��
        // = y���� -10 ������ �Ѿ� ���� ��
        if (sprites[endIndex].position.y < viewHeghit * (-1))
        {
            //#.Sprite Reuse
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            //Transform�� ���� ��ǥ�� �����̱� ������ localPosition
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;

            //�̵��� �Ϸ� �Ǹ� EndIndex , StartIndex ����
            //#. Cursor index Change
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }

    }
}
