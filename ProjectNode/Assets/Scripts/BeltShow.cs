using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

[BurstCompile]
public struct BeltShowDateWriteTask : IJobParallelFor
{
    public UnsafeList<BeltProgress> Belts;
    public UnsafeList<int> WriteStartIndex;
    public UnsafeList<GameObjectJobDate> ShowDates;

    public unsafe void Execute(int index)
    {
        var b = Belts.Ptr[index];
        var start = WriteStartIndex[index];
        for (int i = 0; i < b.ItemNum; i++)
        {
            var pi = (b.ItemStartIndex + i) % b.MaxCount;
            var p=b.Part.Ptr[pi];
            var pos = b.StartPos + (b.EndPos - b.StartPos) * p / BeltProgress.BeltPartEnd;
            GameObjectJobDate d = default;
            d.Position = pos;
            ShowDates.Ptr[start + i] = d;

        }
    }
    
    public void Init()
    {
        ShowDates = new UnsafeList<GameObjectJobDate>(8, Allocator.Persistent);
    }
}

public struct BeltShowPre
{
    public UnsafeList<int> WriteStartIndex;
    public int AllCount;
    public UnsafeList<BeltProgress> Belts;

    public unsafe void PreShow()
    {
        AllCount = 0;
        var len = Belts.Length;
        if (WriteStartIndex.Capacity<len)
        {
            WriteStartIndex.Capacity = len;
        }

        WriteStartIndex.m_length = len;
        for (int i = 0; i < len; i++)
        {
            var b = Belts.Ptr[i];
            WriteStartIndex[i] = AllCount;
            AllCount += b.ItemNum;
        }
    }

    public void Init()
    {
        WriteStartIndex = new UnsafeList<int>(8, Allocator.Persistent);
    }
}