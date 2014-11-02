using UnityEngine;
using System.Collections;

public class EnginePowerUp : MonoBehaviour 
{
    void OnTriggerEnter (Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            SoundController.Play("powerup");
            
            Player.Instance.speed *= 1.2f;
            SimplePool.Release(gameObject);
        }
    }
}
