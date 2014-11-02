using UnityEngine;
using System.Collections;

public class RangePowerUp : MonoBehaviour 
{
    void OnTriggerEnter (Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            SoundController.Play("powerup");
            
            Player.Instance.weaponRange *= 1.2f;
            SimplePool.Release(gameObject);
        }
    }
}
