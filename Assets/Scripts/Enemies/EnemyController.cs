using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour 
{
    public  enum      StrafeDirection
    {
        RIGHT = 1,
        LEFT  = -1
    }

    public StrafeDirection strafeDirection = StrafeDirection.RIGHT;

    public  float     range               = 5f;
    public  float     speed               = 1f;
    public  float     rangeMargin         = 3f;
    public  bool      strafesAroundTarget = true;

    private Transform target;
    private Transform t;
    private Rigidbody rb;
    private float     targetDistance = Mathf.Infinity;
    private Vector3   dir            = Vector3.zero;
    private Vector3   targetVec      = Vector3.zero; 

	void Awake () 
    {
	   t  = gameObject.GetComponent<Transform>();
       rb = gameObject.GetComponent<Rigidbody>();
       range += Random.Range(-(rangeMargin), rangeMargin);

       if (Random.value > 0.5f) { strafeDirection = StrafeDirection.LEFT; }
	}

	void FixedUpdate () 
    {
        if (target == null)
        {
            target = Player.Instance.T;
            return;
        }

        targetDistance = Vector3.Distance(t.position, target.position);
        targetVec      = (t.position - target.position).normalized;

        if (targetDistance > range + rangeMargin)
        {
            dir = targetVec;
        } 
        else if (targetDistance < range - rangeMargin)
        {
            dir = -targetVec;
        } 
        else if (strafesAroundTarget)
        {
            dir = t.right * (int) strafeDirection;
        } 

        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity - dir, Time.deltaTime * speed);
	}
}
