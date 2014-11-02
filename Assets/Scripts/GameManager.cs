using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance { get { return instance; } }

    public enum GameState
    {
        MENU    = 1,
        OPTIONS = 2,
        CREDITS = 4,
        PLAYING = 8, 
        SCORE_INFO = 16
    }

    public  GameObject[] enemyTypes;
    public  GameObject[] powerUps;
    
    [SerializeField]
    private         GameState    state = GameState.MENU;

    private         AudioClip[]  songs;
    private         Transform    t;
    private         float        waveTime           = 15f;
    private         float        ttNextWave         = 0;
    private         float        waveTimeElapsed    = Mathf.Infinity;
    private         bool         waveSpawning       = false;

    private static  int          wave               = 0;
    private static  GameManager  instance;
	
    void Awake () 
    {
        t           = gameObject.GetComponent<Transform>();
        instance    = this;
        
        AllocateForGC();
        SetupPools();

        songs = Resources.LoadAll<AudioClip>("Music");
	}

    void Start ()
    {
        Screen.showCursor = false;
    }

    void Update ()
    {
        if (state == GameState.PLAYING) 
        {
            waveTimeElapsed += Time.deltaTime;    
            ttNextWave = waveTime - waveTimeElapsed;

            if (ttNextWave < 0) { StartCoroutine(SendWave()); }
        }
        
    }

    IEnumerator BeginGame (int songIdx)
    {
        wave = 0;
        state = GameState.PLAYING;

        StartCoroutine(SendWave());
        
        Player.Instance.Reset();

        yield return new WaitForSeconds(3);

        BeginSong(songIdx);
    }

    void BeginSong (int songIdx)
    {
        GC.Collect();

        PlaylistController plc = PlaylistController.Instance;

        plc.songs = new AudioClip[1] { songs[songIdx] };
        plc.PlaySong();

        StartCoroutine(OnSongEnd(songs[songIdx]));
    }

    IEnumerator SendWave ()
    {
        wave++;
        waveTimeElapsed = 0;
        waveSpawning = true;

        int num = Random.Range(0, wave) + 
                 (int) Mathf.Ceil(Mathf.Log(1 << wave));

        for (int i = 0; i < num; i++)
        {
            SpawnEnemies(1, enemyTypes[Random.Range(0, enemyTypes.Length)]);
            yield return new WaitForSeconds(Random.Range(0, 0.25f));
        }

        waveSpawning = false;
    }

    IEnumerator OnSongEnd (AudioClip clip)
    {
        yield return new WaitForSeconds(clip.length);

        state = GameState.SCORE_INFO;

        if (Player.Score > PlayerPrefs.GetInt(clip.name)) 
        {
            PlayerPrefs.SetInt(clip.name, Player.Score);
        }
        
        foreach (GameObject eType in enemyTypes)
        {
            SimplePool.ReleasePool(eType.name);
        }
    }

    public void SpawnPowerUp (Vector3 pos)
    {
        SimplePool.Catch(   powerUps[Random.Range(0, powerUps.Length)], 
                            pos, 
                            Quaternion.identity);
    }

    void SpawnEnemies (int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            float x = Random.Range(-20f, 20f);
            float z = Random.Range(-20f, 20f);

            SimplePool.Catch(type, new Vector3(x, 0, z), Quaternion.identity);
        }
    }

	void FixedUpdate () 
    {
	}

    void OnGUI ()
    {
        if (state == GameState.MENU)
        {
            int w = 450;
            int h = 45;
            int x = (int) ((Screen.width * 0.5f) - (w * 0.5f));
            int y = (h + 10);

            for (int i = 0; i < songs.Length; i++)
            {  
                int    highScore = PlayerPrefs.GetInt(songs[i].name);
                string lbl  = songs[i].name + " \n High Score: " + highScore;
                
                if (GUI.Button(new Rect(x, 70 + y * i, w, h), lbl))
                {
                    StartCoroutine(BeginGame(i));
                }
            }
        }
        else if (state == GameState.PLAYING)
        {
            GUI.Label(new Rect(10, 10, 150, 30), "Score: " + Player.Score);
            GUI.Label(new Rect(10, 50, 150, 30), "Next Wave: " + (int)ttNextWave);
        }
        else if (state == GameState.SCORE_INFO)
        {
            string scrStr = "You Scored: (Damage Done) " + 
                            Player.damageDone + 
                            " * (Accuracy) " + 
                            Player.Accuracy + 
                            " - (Damage Received) " + 
                            Player.damageTaken +
                            " = " + Player.Score;

            int x = (int) (Screen.width  * 0.5f - 325);
            int y = (int) (Screen.height * 0.5f);

            GUI.Label(new Rect(x, y - 60, 650, 35), "Great Job!");
            GUI.Label(new Rect(x, y, 650, 35), scrStr);

            if (GUI.Button(new Rect(x, y + 40, 650, 35), "Restart"))
            {
                state = GameState.MENU;
            }
        }
    }

    void SetupPools ()
    {
        foreach (GameObject type in enemyTypes)
        {
            if (!SimplePool.PoolExists(type.name)) 
            { 
                SimplePool.CreatePool(type); 
            }
        }

        foreach (GameObject type in powerUps)
        {
            if (!SimplePool.PoolExists(type.name)) 
            { 
                SimplePool.CreatePool(type); 
            }
        }
    }

    void AllocateForGC ()
    {
        System.Object[] tmp = new System.Object[2048];
        for (int i = 0; i < 2048; i++) { tmp[i] = new byte[1024]; }
        tmp = null;
    }
}
