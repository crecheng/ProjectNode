using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public struct GameObjectJob : IJobParallelForTransform
{
    public UnsafeList<GameObjectJobDate> Dates;
    public void Execute(int index, TransformAccess transform)
    {
        if (Dates.m_length <= index)
        {
            return;
        }
        var d = Dates[index];
        bool flag = false;
        flag = d.isScale && SetScale(transform, d._Scale);
        flag = d.isLocalPosition && SetLocalPosition(transform, d._LocalPosition);
        flag = d.isPosition && SetPosition(transform, d._Position);
    }


    
    private bool SetPosition(TransformAccess transform, Vector3 position)
    {
        transform.position = position;
        return true;
    }
    
    private bool SetScale(TransformAccess transform, Vector3 scale)
    {
        transform.localScale = scale;
        return true;
    }
    
    private bool SetLocalPosition(TransformAccess transform, Vector3 position)
    {
        transform.localPosition = position;
        return true;
    }
}


public struct GameObjectJobDate
{


    public bool isPosition;
    public Vector3 _Position;
    public Vector3 Position{
        set
        {
            isPosition = true;
            _Position = value;
        }
    }
    
    public bool isScale;
    public Vector3 _Scale;
    public Vector3 Scale{
        set
        {
            isScale = true;
            _Scale = value;
        }
    }
    
    public bool isLocalPosition;
    public Vector3 _LocalPosition;
    public Vector3 LocalPosition{
        set
        {
            isLocalPosition = true;
            _LocalPosition = value;
        }
    }
}
