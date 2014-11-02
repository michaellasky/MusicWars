using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public        Transform   T         { get { return t;                     }}
    public        Rigidbody   Rb        { get { return rb;                    }}
    public static Player      Instance  { get { return instance;              }}
    
    public static int         Score     
    { 
        get 
        { 
            return (int) ((damageDone * Accuracy) - damageTaken);
        }
    }
    
    public static float       Accuracy  
    { 
        get 
        { 
            if (shotsFired == 0) { return 0; }
            return (float) shotsHit / (float) shotsFired; 
            
        }
    }

    public         float       speed             = 0.01f;
    public         float       spinRate          = 1f;
    public         float       weaponRange       = 1f;
    public         float       projectilePower   = 1f;
    public         float       fireRate          = 10f;
    public         GameObject  playerProjectilePrefab;
    public         Transform[] barrels;
    public         Transform   playerTarget;
    public         Transform   playerShipModel;

    public static  int         damageDone        = 0;
    public static  int         damageTaken       = 0;
    public static  int         shotsHit          = 0;
    public static  int         shotsFired        = 0;
    private        float       initSpeed;
    private        float       initSpinRate;
    private        float       initWeaponRange;
    private        float       initProjectilePower;
    private        float       initFireRate;
    private        Transform   t;
    private        Rigidbody   rb;
    private        Vector3     hDelta;
    private        Vector3     vDelta;
    private        Vector3     nVelocity;
    private        RaycastHit  hit;
    private        Ray         ray;
    private        LayerMask   ipLayer;
    private        Quaternion  rot;
    private        float       fireTime;
    private        float       timeSinceLastFire = 0;
    private        Vector3     hSpeed;
    private        Vector3     vSpeed;

    private static Player     instance;

	void Awake () 
    {
        instance            = this;
        t                   = gameObject.GetComponent<Transform>();
        rb                  = gameObject.GetComponent<Rigidbody>();
	    ipLayer             = LayerMask.NameToLayer("InteractionPlane");
        hSpeed              = t.right * speed;
        vSpeed              = t.forward * speed;

        initSpeed           = speed;
        initSpinRate        = spinRate;
        initWeaponRange     = weaponRange;
        initProjectilePower = projectilePower;
        initFireRate        = fireRate;
    }

    void Update ()
    {
        fireTime = 1 / fireRate;
    
        timeSinceLastFire += Time.deltaTime;

        hDelta    = hSpeed * Input.GetAxis("Horizontal");
        vDelta    = vSpeed * Input.GetAxis("Vertical");
        nVelocity = rb.velocity + hDelta + vDelta;

        if (Physics.Raycast(ray, out hit, ipLayer))
        {
            playerTarget.position = hit.point;
        }

        if (Input.GetButton("Fire1"))
        {
            if (timeSinceLastFire > fireTime) { Fire(); }
        }
    }

	void FixedUpdate () 
    {
        rb.velocity = Vector3.Lerp(rb.velocity, nVelocity, Time.deltaTime * speed);
        ray         = Camera.main.ScreenPointToRay(Input.mousePosition);

        rot = Quaternion.LookRotation(playerTarget.position - t.position);
        
        playerShipModel.rotation = Quaternion.Slerp(playerShipModel.rotation, 
                                                    rot, 
                                                    Time.deltaTime * spinRate);
	}

    void Fire ()
    {
        shotsFired++;
        timeSinceLastFire = 0;

        SoundController.Play("shoot");
        
        foreach (Transform barrel in barrels)
        { 
            GameObject proj     = SimplePool.Catch(playerProjectilePrefab, barrel);
            Rigidbody  pRb      = PlayerProjectile.rbRegistry[proj];
            PlayerProjectile pp = PlayerProjectile.ppRegistry[proj];

            pp.damage = projectilePower;
            pRb.velocity = rb.velocity + barrel.forward * 50;

            StartCoroutine(ReleaseProjectile(proj));
        }
    }

    IEnumerator ReleaseProjectile (GameObject p)
    {
        yield return new WaitForSeconds(weaponRange);
        SimplePool.Release(p);
    }

    public void Reset ()
    {
        damageDone  = 0;
        damageTaken = 0;
        shotsFired  = 0;
        shotsHit    = 0;

        speed           = initSpeed;
        spinRate        = initSpinRate;
        weaponRange     = initWeaponRange;
        projectilePower = initProjectilePower;
        fireRate        = initFireRate;
    }
}
