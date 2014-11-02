using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour 
{
    public AudioClip[] sounds;

	void Start () 
    {
        foreach (AudioClip c in sounds)
        {
            GameObject  go      = new GameObject(c.name);
            AudioSource aSource = go.AddComponent<AudioSource>();
            LifeTime    lt      = go.AddComponent<LifeTime>();

            lt.lifeTime         = c.length;
            aSource.clip        = c;
            aSource.playOnAwake = true;
            
            SimplePool.CreatePool(go, 100);            
        }
	}
    
    public static void Play (string clipName) 
    {
        SimplePool.Catch(clipName, Vector3.zero, Quaternion.identity);
    }
}
