using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Bullet : MonoBehaviour
{
    public int dmg; //총알이 갖고 있는 데미지의 양
    public bool isRotate;   //bool 변수를 활용하여 스스로 돌아가는 총알로 편집

    void Update()
    {
        if (isRotate)
        {
            transform.Rotate(Vector3.forward * 10);
        }
    }
    void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
        }
    }
}
