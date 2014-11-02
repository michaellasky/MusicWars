using UnityEngine;
using System.Collections;

public class SpeedPowerUp : MonoBehaviour 
{
    void OnTriggerEnter (Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            SoundController.Play("powerup");
            
            Player.Instance.fireRate *= 1.2f;
            SimplePool.Release(gameObject);
        }
    }
}
