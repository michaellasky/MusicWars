using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// This is a super dumb pooler, use with caution

public static class SimplePool 
{
    public static Dictionary<string, GameObject[]>      pools;
    public static int                                   poolSize        = 1000;
    public static int                                   poolIncreasSize = 100;
    public static int                                   poolMargin      = 50;
    public static Dictionary<string, int>               poolCounts;
    public static Dictionary<string, HashSet<int>>      activeObjs;

    public static GameObject Catch (string name, Vector3 pos, Quaternion rot)
    {
        return Catch(pools[name][0], pos, rot);
    }

    public static GameObject Catch (GameObject go, Transform t)
    {
        return Catch(go, t.position, t.rotation);
    }

    public static GameObject Catch (GameObject go, Vector3 pos, Quaternion rot)
    {
        if (!PoolExists(go.name)) { CreatePool(go);       }
        if (PoolIsFull(go.name))  { IncreasePoolSize(go); }

        int i = 0;
        for (; i < pools[go.name].Length; i++)
        {
            int hc  = pools[go.name][i].GetHashCode();   
            
            if (activeObjs[go.name].Contains(hc)) { continue; }

            activeObjs[go.name].Add(hc);
            Transform  pT  = pools[go.name][i].GetComponent<Transform>();

            pT.position = pos;
            pT.rotation = rot;

            pools[go.name][i].SetActive(true);

            break;
        }
        // i should never be out of range here because we increase size 
        // and we don't need to worry about being thread safe
        return pools[go.name][i];
    }

    public static void Release (GameObject go)
    {
        activeObjs[go.name].Remove(go.GetHashCode());
        go.SetActive(false);
    }

    public static void ReleaseAll ()
    {
        foreach(string name in pools.Keys) { ReleasePool(name); }
    }

    public static void ReleasePool (string name)
    {
        foreach(GameObject go in pools[name])
        {
            if (activeObjs[name].Contains(go.GetHashCode())) { Release(go); }
        }
    }

    public static bool PoolExists (string pName)
    {
        return pools != null && pools.ContainsKey(pName);
    }

    public static bool PoolIsFull (string pName)
    {
        return activeObjs[pName].Count > pools[pName].Length - poolMargin;
    }

    public static void CreatePool (GameObject go, int num)
    {
        if (pools == null) 
        { 
            pools      = new Dictionary<string, GameObject[]>();
            activeObjs = new Dictionary<string, HashSet<int>>();
        }

        pools[go.name] = CreateObjs(go, num);
        activeObjs[go.name] = new HashSet<int>();
    }

    public static void CreatePool (GameObject go)
    {
        CreatePool(go, poolSize);
    }

    public static void IncreasePoolSize (GameObject go)
    {
        int          currLen = pools[go.name].Length;
        GameObject[] nPool   = new GameObject[currLen + poolIncreasSize];
        GameObject[] newObjs = CreateObjs(go, poolIncreasSize);
        
        Array.Copy(pools[go.name], 0, nPool, 0, currLen);
        Array.Copy(newObjs, 0, nPool, currLen, newObjs.Length);

        pools[go.name] = nPool;
    }

    public static GameObject[] CreateObjs (GameObject go, int num)
    {
        GameObject[] gos = new GameObject[num];

        for (int i = 0; i < num; i++)
        {
            GameObject goInst = (GameObject) GameObject.Instantiate(   
                go, 
                Vector3.zero, 
                Quaternion.identity);

            goInst.name = go.name;
            goInst.SetActive(false);
            gos[i] = goInst;
        }
        return gos;
    }

    public static IEnumerator ReleaseAfter(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);

        Release(go);
    }
}