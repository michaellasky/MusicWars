using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyProjectile : MonoBehaviour 
{      
    public  Vector3   velocity;
    public  int       damage   = 1;
    public  float     lifeTime = 3f;
    private Rigidbody rigidBody;

    // These give quick access to these components without querying the gameobj
    // There's probably a better pattern I'm missing 
    public static Dictionary<GameObject, Rigidbody>       rbRegistry;
    public static Dictionary<GameObject, EnemyProjectile> epRegistry;

	void Awake ()
    {
        if (rbRegistry == null)
        {
            rbRegistry = new Dictionary<GameObject, Rigidbody>();
        }

        if (epRegistry == null)
        {
            epRegistry = new Dictionary<GameObject, EnemyProjectile>();
        }

        rbRegistry[gameObject] = gameObject.GetComponent<Rigidbody>();
        epRegistry[gameObject] = this;
    }

    void OnCollisionEnter (Collision c)
    {
        if (c.gameObject.tag == "Player")
        {
            Player.damageTaken += damage;
            gameObject.SetActive(false);
        }
    }
	
    void OnEnable () 
    {
	    StartCoroutine(ReleaseProjectile());
	}

    IEnumerator ReleaseProjectile ()
    {
        yield return null;
        yield return new WaitForSeconds(lifeTime);
        SimplePool.Release(gameObject);
    }
}
