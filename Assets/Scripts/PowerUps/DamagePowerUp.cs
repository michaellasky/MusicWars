using UnityEngine;
using System.Collections;

public class DamagePowerUp : MonoBehaviour 
{
    void OnTriggerEnter (Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            SoundController.Play("powerup");
            
            Player.Instance.projectilePower *= 1.2f;
            SimplePool.Release(gameObject);
        }
    }
}
