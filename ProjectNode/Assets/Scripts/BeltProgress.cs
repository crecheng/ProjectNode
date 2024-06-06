using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct BeltProgress
{
    public Vector3 StartPos;
    public Vector3 EndPos;
    public int InId;
    public int OutId;
    public int ItemStartIndex;
    public byte ItemNum;
    public UnsafeList<int> Part;
    public UnsafeList<int> ItemId;
    public float Split;
    public int MaxCount;
    public int Speed;
    public const int BeltPartEnd = 1000_0000;
}

[BurstCompile]
public struct BeltTask : IJobParallelFor
{
    public UnsafeList<BeltProgress> Belt;
    public UnsafeList<int> Buffer;

    public unsafe void Execute(int index)
    {
        ref var belt = ref Belt.Ptr[index];
        int p = BeltProgress.BeltPartEnd;
        var split = BeltProgress.BeltPartEnd / belt.MaxCount;
        if (belt.ItemNum > 0)
        {
            if (Buffer[belt.OutId] == 1 && belt.Part[belt.ItemStartIndex] >= split * belt.MaxCount)
            {
                Buffer[belt.OutId] = belt.ItemId[belt.ItemStartIndex];
                belt.ItemNum--;
                belt.ItemStartIndex++;
                belt.ItemStartIndex %= belt.MaxCount;
            }



            for (int i = 0; i < belt.ItemNum; i++)
            {
                var pi = (belt.ItemStartIndex + i) % belt.MaxCount;
                p = belt.Part[pi];
                p += belt.Speed;
                var end = split * (belt.MaxCount - i);
                p = math.min(end, p);
                belt.Part[pi] = p;
            }
        }

        if (p > split 
            && belt.ItemNum < belt.MaxCount
            && Buffer[belt.InId]<-1)
        {
            var pos = (belt.ItemStartIndex + belt.ItemNum) % belt.MaxCount;
            belt.ItemId[pos] = Buffer[belt.InId];
            belt.ItemNum++;
            Buffer[belt.InId] = -1;
        }
    }
}