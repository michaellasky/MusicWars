using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class DeathCylinder : MonoBehaviour 
{   
    public float         maxFireSpeed               = 0.1f;
    public int           channel                    = 0;
    public Transform[]   barrelPoints;
    public GameObject    basicProjectilePrefab;
    public GameObject    littleYellowProjectilePrefab;
    public GameObject    bigBlueProjectilePrefab;

    private Transform    t;
    private AudioWatcher audioWatcher;
    private Rigidbody    projRb;
    private float        splPower;
    private float        rotationPower;
    private float        tSinceLastFire = 0;
    private float        currRotSpd     = 0;
    private float        baseRotSpd     = 5;
    private float        deltaTime      = 0;

	void Awake () 
    {
        t = gameObject.GetComponent<Transform>();
        SetupProjectilePools();
	}
	void Update ()
    {
        deltaTime = Time.deltaTime;
        splPower = GetPower(channel);
        tSinceLastFire += deltaTime;
        rotationPower   = (int) GetPower(0);
        currRotSpd = Mathf.Lerp(currRotSpd, rotationPower, Time.deltaTime);
        t.Rotate(0, baseRotSpd + currRotSpd, 0);
    }

    void SetupProjectilePools ()
    {
        if (!SimplePool.PoolExists(basicProjectilePrefab.name))
        {
            SimplePool.CreatePool(basicProjectilePrefab);    
        }

        if (!SimplePool.PoolExists(littleYellowProjectilePrefab.name))
        {
            SimplePool.CreatePool(littleYellowProjectilePrefab);    
        }

        if (!SimplePool.PoolExists(bigBlueProjectilePrefab.name))
        {
            SimplePool.CreatePool(bigBlueProjectilePrefab);    
        }
    }

    public void FireBasicProjectile(AudioBand bandData)
    {
        Fire(barrelPoints, basicProjectilePrefab, 0.02f);
    }

    public void FireLittleYellowProjectile(AudioBand bandData)
    {   
        Fire(barrelPoints, littleYellowProjectilePrefab, 0.01f);
    }

    public void FireBigBlueProjectile(AudioBand bandData)
    {
        Fire(barrelPoints, bigBlueProjectilePrefab, 0.03f);
    }

    public void Fire (Transform[] barrels, GameObject p, float pMod)
    {
        if (tSinceLastFire < maxFireSpeed || splPower < 1) { return; }

        tSinceLastFire = 0;

        foreach (Transform b in barrels)
        {
            if (deltaTime > 0.033f) { continue; }

            GameObject proj = SimplePool.Catch(p, b.position, b.rotation);
            EnemyProjectile.rbRegistry[proj].velocity = b.forward * splPower;
            EnemyProjectile.epRegistry[proj].damage   = (int) (splPower * pMod);
        }
    }

    float GetPower (int channel)
    {
        if (audioWatcher == null) { audioWatcher = AudioWatcher.Instance; }

        return Mathf.Abs((int) audioWatcher.CurrentSPL(channel) >> 2);
    }
}
