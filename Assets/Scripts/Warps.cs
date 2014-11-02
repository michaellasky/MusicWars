using UnityEngine;
using System.Collections;

public class Warps : MonoBehaviour 
{
    private        Transform  t;
    private static float      sWidth;
    private static float      sHeight;
    private static Vector3    llPos     = Vector3.zero;
    private static Vector3    urPos     = Vector3.zero;

    
    void Awake () 
    {
	    t = gameObject.GetComponent<Transform>();
        SetSize();
	}

    void Update ()
    {
        if (Screen.width != sWidth || Screen.height != sHeight) { SetSize(); }

        if (t.position.z < llPos.z)
        {
            t.position = new Vector3(t.position.x, 0, urPos.z);
        }

        if (t.position.z > urPos.z)
        {
            t.position = new Vector3(t.position.x, 0, llPos.z);
        }

        if (t.position.x < llPos.x)
        {
            t.position = new Vector3(urPos.x, 0, t.position.z);
        }

        if (t.position.x > urPos.x)
        {   
            t.position = new Vector3(llPos.x, 0, t.position.z);
        }
    }

    void SetSize ()
    {
                    sWidth    = Screen.width;
                    sHeight   = Screen.height;
        Camera      cam       = Camera.main;
        LayerMask   layer     = LayerMask.NameToLayer("InteractionPlane");
        Vector3     urScrnPos = new Vector3(sWidth, sHeight, 0);
        Vector3     llSrnPos  = Vector3.zero;
        Ray         llRay     = cam.ScreenPointToRay(llSrnPos);
        Ray         urRay     = cam.ScreenPointToRay(urScrnPos);
        RaycastHit  hit;

        if (Physics.Raycast(llRay, out hit, layer)) { llPos = hit.point; }
        if (Physics.Raycast(urRay, out hit, layer)) { urPos = hit.point; }
    }
}
