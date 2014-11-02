using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour 
{
    public int          Health { get { return currentHealth; } }
    
    public GameObject   explosionPrefab;
    public AudioClip    damageSound;
    public int          baseHealth = 100;
    
    private int         currentHealth;
    private Transform   t;

    // These give quick access to these components without querying the gameobj
    // There's probably a better pattern I'm missing 
    public static Dictionary<GameObject, Enemy> eRegistry;

    void Awake ()
    {
        t           = gameObject.GetComponent<Transform>();

        if (eRegistry == null)
        {
            eRegistry = new Dictionary<GameObject, Enemy>();
        }

        eRegistry[gameObject] = this;
    }

	void OnEnable () 
    {
	    currentHealth = baseHealth;
	}

    void Die ()
    {
        GameObject.Instantiate(explosionPrefab, t.position, t.rotation);
        SimplePool.Release(gameObject);
        SoundController.Play("explosion_1");
    }

    public void Damage (int amt)
    {
        currentHealth      -= amt;
        Player.damageDone  += amt;
        Player.shotsHit++;

        SoundController.Play("hit");
        
        if (Random.value > 0.95f) 
        { 
            GameManager.Instance.SpawnPowerUp(t.position); 
        }
        
        if (currentHealth < 0) { Die(); }
    }
}
