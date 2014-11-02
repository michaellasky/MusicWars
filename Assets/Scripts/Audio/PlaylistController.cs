using UnityEngine;
using System.Collections;

public class PlaylistController : MonoBehaviour 
{
    public static PlaylistController Instance { get { return instance; } }

    public int          CurrentIdx 
    {
        get { return currentIdx;        }
        set { BeginPlaying(currentIdx); }
    }

    public AudioClip[]  songs;
    public bool         playOnAwake = true;
    public bool         loop        = false;

    private AudioSource audioSource;
    private int         currentIdx;

    private static PlaylistController instance;

    void Awake ()
    {
        instance    = this;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

	void Start () 
    {
	    if (playOnAwake) { StartCoroutine(BeginPlaying(0)); }   
	}

    public void PlaySong ()
    {
        PlaySong(0);
    }

    public void PlaySong (int idx)
    {
        StartCoroutine(BeginPlaying(idx));
    }

    IEnumerator BeginPlaying(int idx)
    {
        Debug.Log("Playing: " + idx);
        currentIdx = idx;
        audioSource.clip = songs[currentIdx];
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length + 1);
        
        if (loop) 
        { 
            StartCoroutine(BeginPlaying((currentIdx + 1) % songs.Length));
        }   
    }
}
