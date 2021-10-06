using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Bullet : MonoBehaviour
{
    public int dmg; //총알이 갖고 있는 데미지의 양
    void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
        }
    }
}
