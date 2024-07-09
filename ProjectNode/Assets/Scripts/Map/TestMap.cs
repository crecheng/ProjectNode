using System;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    public int Max;
    public int Current;

    public MapTestBlock[] pre;
    private List<MapTestBlock> list = new List<MapTestBlock>();
    
    private void Start()
    {
        for (int i = 0; i < 256; i++)
        {
            for (int j = 0; j < 256; j++)
            {
                
                Vector2 pos = new Vector2(i, j);
                var z = NoiseTool.PerlinNoise(pos, 64) * 128f;
                z = Mathf.RoundToInt(z);
                MapTestBlock go = null;
                if (z > 35)
                    go = Instantiate(pre[1], transform);
                else
                    go = Instantiate(pre[0], transform);
                go.transform.position = new Vector3(i, z, j);
            }
        }
    }


}
