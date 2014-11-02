using UnityEngine;
using System.Collections;

public class EWing : MonoBehaviour 
{

    public  GameObject   projectilePrefab;
    public  float        maxFireSpeed           = 0.1f;
    public  int          channel                = 0;
    public  float        spinRate               = 5f;
    public  Transform[]  barrels;

    private AudioWatcher audioWatcher;
    private Quaternion   rot;
    private Transform    t;
	private Vector3      target;
    private float        splPower               = 0;
    private float        deltaTime              = 0;
    private float        tSinceLastFire         = 0;

    void Awake () 
    {
	    t = gameObject.GetComponent<Transform>();   

        if (!SimplePool.PoolExists(projectilePrefab.name))
        {
            SimplePool.CreatePool(projectilePrefab);    
        }
	}

    void Update ()
    {
        deltaTime       = Time.deltaTime;
        splPower        = GetPower(channel);
        
        tSinceLastFire += deltaTime;
    }
	
	void FixedUpdate () 
    {

        target = Player.Instance.T.position + Player.Instance.Rb.velocity;
        
	    rot = Quaternion.LookRotation(target - t.position);
        
        t.rotation = Quaternion.Slerp(t.rotation, rot, Time.deltaTime * spinRate);
	}

    public void Fire(AudioBand bandData)
    {
        if (tSinceLastFire < maxFireSpeed || splPower < 1) { return; }

        tSinceLastFire = 0;

        foreach (Transform b in barrels)
        {
            if (deltaTime > 0.033f) { continue; }

            GameObject proj = SimplePool.Catch(projectilePrefab, b.position, b.rotation);
            EnemyProjectile.rbRegistry[proj].velocity = b.forward * splPower;
            EnemyProjectile.epRegistry[proj].damage   = (int) splPower;
        }
    }

    float GetPower (int channel)
    {
        if (audioWatcher == null) { audioWatcher = AudioWatcher.Instance; }

        return Mathf.Abs((int) audioWatcher.CurrentSPL(channel) >> 2);
    }
}
