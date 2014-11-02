using UnityEngine;
using System.Collections;

public class HasInitialPhysics : MonoBehaviour 
{
    public  float     initVelocityMultiplier = 5f;
    public  float     initAngularVelMultiplier = 5f;
    private Rigidbody rb;

    void OnEnable ()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(Random.value, 0, Random.value) * initVelocityMultiplier;
        rb.angularVelocity = new Vector3(0, Random.value, 0) * initVelocityMultiplier;
    }
}
