using System;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    public int Max;
    public int Current;

    public GameObject pre;
    private List<GameObject> list = new List<GameObject>();
    
    private void Start()
    {
        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 128; j++)
            {
                var go = Instantiate(pre);
                Vector2 pos = new Vector2(i, j);
                NoiseTool.PreHandlePerlinNoise(pos,8);
                var z = NoiseTool.PerlinNoise(pos);
                go.transform.position = new Vector3(i, z, j);
            }
        }
    }


}
