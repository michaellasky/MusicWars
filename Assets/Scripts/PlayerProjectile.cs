using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerProjectile : MonoBehaviour 
{
    public float damage = 1;

    // These give quick access to these components without querying the gameobj
    // There's probably a better pattern I'm missing 
    public static Dictionary<GameObject, Rigidbody>        rbRegistry;
    public static Dictionary<GameObject, PlayerProjectile> ppRegistry;

    void Awake ()
    {
        if (rbRegistry == null)
        {
            rbRegistry = new Dictionary<GameObject, Rigidbody>();
        }

        if (ppRegistry == null)
        {
            ppRegistry = new Dictionary<GameObject, PlayerProjectile>();
        }

        rbRegistry[gameObject] = gameObject.GetComponent<Rigidbody>();
        ppRegistry[gameObject] = this;
    }

    void OnCollisionEnter (Collision c)
    {
        Enemy e = Enemy.eRegistry[c.gameObject];
        if (e == null) { return; }

        e.Damage((int) damage);

        gameObject.SetActive(false);
    }
}
