using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
public class TestV3F3 : MonoBehaviour
{
    private void Update()
    {
        Test();
    }

    public int len = 300;
    public void Test()
    {
        var Vector3s =   new UnsafeList<Vector3>(len,Allocator.Temp)
        {
            Length = len
        };
        var  float3s = new UnsafeList<float3>(len,Allocator.Temp)
        {
            Length = len
        };
        for (int i = 0; i < len; i++)
        {
            Vector3s[i]=new Vector3(Random.value, Random.value, Random.value);
            float3s[i]=new float3(Random.value, Random.value, Random.value); 
        }
        Debug.Log(DateTime.Now.Ticks);
        var start = DateTime.Now;
        TestF3(float3s);

        var t = DateTime.Now;
        Debug.Log((t-start).Ticks);
        
        start = DateTime.Now;
        TestV3(Vector3s);
        t = DateTime.Now;
        Debug.Log((t-start).Ticks);
        float3s.Dispose();
        Vector3s.Dispose();
    }
    

    [BurstCompile]
    public static unsafe float TestF3(in UnsafeList<float3> float3s)
    {
        var len = float3s.Length;
        float3 n = float3s[0];
        for (int j = 0; j < len; j++)
        {
            for (int k = j; k < len; k++)
            {
                var m = math.cross(float3s.Ptr[j], float3s.Ptr[k]);
                n = math.cross(m, n);
            }
        }
        return n.x;
    }
    
    [BurstCompile]
    public static unsafe float TestV3(in UnsafeList<Vector3> Vector3s)
    {
        var len = Vector3s.Length;
        Vector3 n = Vector3s[0];
        for (int j = 0; j < len; j++)
        {
            for (int k = j; k < len; k++)
            {
                var m = Vector3.Cross(Vector3s.Ptr[j], Vector3s.Ptr[k]);
                n = Vector3.Cross(n, m);
            }
        }

        return n.x;
    }
}
