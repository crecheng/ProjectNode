using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


[BurstCompile]
public class Factory : IDisposable
{
    public UnsafeList<BeltProgress> _belt;
    public UnsafeList<int> _buffer;
    private BeltTask _beltTask;
    public int BeltCount;
    
    private MachineTask _machineTask;
    private UnsafeList<MachineProgress> _machine;
    private UnsafeList<MachineRecipe> _machineRecipes;
    public Collider c;
    public Rigidbody r;
    public Animation a;
    
    public const int Count = 10_0000;
    
    public void Init()
    {
        
        InitBelt();
        InitMachine();
        
    }
    
    public void GameTick()
    {
        MachineGameTick();
        BeltGameTick();
    }

    private void InitBelt()
    {
        _belt = new UnsafeList<BeltProgress>(64, Allocator.Persistent);
        _buffer = new UnsafeList<int>(64, Allocator.Persistent);

        int index = 0;
        for (int i = 0; i < 100; i++)
        {
            float z = 0f;// index++ * 0.5f;
            CreateBelt(new Vector3(-500, 1, z), new Vector3(500, 1, z));
        }

        _beltShowPre.Init();
        _beltShowDateWrite.Init();
        QueueGo1 = new TransformAccessArray(8);

        // for (int i = 0; i < 100; i++)
        // {
        //     CreateBelt(new Vector3(Random.value * 100, 1, Random.value * 100),
        //         new Vector3(Random.value * 100, 1, Random.value * 100));
        // }
    }


    private void BeltGameTick()
    {
        _beltTask.Belt = _belt;
        _beltTask.Buffer = _buffer;
        for (int i = 0; i < BeltCount; i++)
        {
            _buffer[i * 2] = -10;
            _buffer[i * 2 + 1] = 1;
        }

        if (BeltCount <= 0)
            return;
        var handel = _beltTask.Schedule(BeltCount, 8);
        handel.Complete();


    }

    public List<LineRenderer> BeltLine = new List<LineRenderer>();

    private TransformAccessArray QueueGo1;
    private int QueueGoLen;

    private BeltShowPre _beltShowPre;
    private BeltShowDateWriteTask _beltShowDateWrite;
    private int _showLen;
    public GameObject pre;
    private GameObjectJob _showTask;

    private int _f = 0;
    
    public void ShowBelt()
    {

        _beltShowPre.Belts = _belt;
        _beltShowPre.PreShow();

        if (_beltShowDateWrite.ShowDates.m_length < _beltShowPre.AllCount)
        {
            _beltShowDateWrite.ShowDates.Capacity = _beltShowPre.AllCount;
            _beltShowDateWrite.ShowDates.m_length = _beltShowPre.AllCount;
        }
        _beltShowDateWrite.Belts = _belt;
        _beltShowDateWrite.WriteStartIndex = _beltShowPre.WriteStartIndex;
        _beltShowDateWrite.Schedule(BeltCount,8).Complete();
        
        CheckBeltItemGoEnough(_beltShowPre.AllCount);
        
        if(_f++%10==0)
            Debug.Log(_beltShowDateWrite.ShowDates.m_length);
        
        _showTask.Dates = _beltShowDateWrite.ShowDates;
        
        _showTask.Schedule(QueueGo1).Complete();
    }

    private void CheckBeltItemGoEnough(int need)
    {
        while (QueueGoLen<need)
        {
            GetBeltItem();
        }
    }

    private void GetBeltItem()
    {
        var go = Object.Instantiate(pre);
        QueueGo1.Add(go.transform);
        QueueGoLen++;
    }

    public LineRenderer CreatLine(Vector3 start, Vector3 end)
    {
        GameObject go = new GameObject();
        var line =go.AddComponent<LineRenderer>();
        line.SetPositions(new[] { start, end});
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        return line;
    }

    public void CreateBelt(Vector3 start, Vector3 end)
    {
        BeltProgress beltProgress = default;
        beltProgress.StartPos = start;
        beltProgress.EndPos = end;
        var len = start - end;
        beltProgress.MaxCount = (int)(len.magnitude / 0.2f);
        beltProgress.InId = BeltCount * 2;
        beltProgress.OutId = BeltCount * 2 + 1;
        _buffer.Add(-10);
        _buffer.Add(0);
        beltProgress.ItemId =
            new UnsafeList<int>(beltProgress.MaxCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        beltProgress.ItemId.m_length = beltProgress.MaxCount;
        beltProgress.Part =
            new UnsafeList<int>(beltProgress.MaxCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        beltProgress.Part.m_length = beltProgress.MaxCount;
        BeltCount++;
        beltProgress.Speed = BeltProgress.BeltPartEnd / beltProgress.MaxCount / 1;
        _belt.Add(beltProgress);
    }

    


    #region Machine
    
    private void InitMachine()
    {
        _machine = new UnsafeList<MachineProgress>(Count,Allocator.Persistent);
        _machineRecipes = new UnsafeList<MachineRecipe>(100, Allocator.Persistent);
        for (int i = 0; i < 100; i++)
        {
            MachineRecipe recipe = default;
            recipe.CostId1 = (short)(Random.value * 10 + 1);
            recipe.CostId2 = (short)(Random.value * 10 + 1);
            recipe.CostId3 = (short)(Random.value * 10 + 1);
            recipe.CostCount1 = (short)(Random.value * 10 + 1);
            recipe.CostCount2 = (short)(Random.value * 10 + 1);
            recipe.CostCount3 = (short)(Random.value * 10 + 1);
            recipe.ProduceId1 = (short)(Random.value * 10 + 1);
            recipe.ProduceId2 = (short)(Random.value * 10 + 1);
            recipe.ProduceId3 = (short)(Random.value * 10 + 1);
            recipe.ProduceCount1 = (short)(Random.value * 10 + 1);
            recipe.ProduceCount2 = (short)(Random.value * 10 + 1);
            recipe.ProduceCount3 = (short)(Random.value * 10 + 1);
            _machineRecipes.Add(recipe);
        }
        for (var i = 0; i < Count; i++)
        {
            MachineProgress d = default;
            d.PowerSpeed = Random.value;
            d.Speed = 1;
            d.RecipeId = (int)(Random.value * 99 + 1);
            d.ProgressOnce = (short)(Random.value * 100 + 10);
            d.CostCount1=(int)(Random.value * 1_0000 + 1_0000);
            d.CostCount2=(int)(Random.value * 1_0000 + 1_0000);
            d.CostCount3=(int)(Random.value * 1_0000 + 1_0000);
            _machine.Add(d);
        }
    }

    private void MachineGameTick()
    {
        _machineTask._machine = _machine;
        _machineTask._machineRecipes = _machineRecipes;
        
        var handle= _machineTask.Schedule(Count, 64);
        handle.Complete();
        
        // Execute(ref _machine);
    }
    
    
    [BurstCompile]
    public static unsafe float Execute(
        ref UnsafeList<MachineProgress> _machine)
    {
        float res = 0;
        for (int i = 0; i < _machine.Length; i++)
        {
            ref var d = ref _machine.Ptr[i];
            // ref var m = ref _machine.ElementAt(i);
            // m.Progress += d.PowerSpeed * d.Speed;
            //_machine.Ptr[i].Progress += d.PowerSpeed * d.Speed;

            res += d.PowerSpeed;
        }

        return res;
    }

    public unsafe MachineProgress CopyShow(int index)
    {
        ref var m = ref _machine.Ptr[index];
        return m;
    }
    
    public unsafe MachineRecipe CopyShowRecipe(int index)
    {
        ref var m = ref _machineRecipes.Ptr[index];
        return m;
    }
    
    #endregion

    public void Dispose()
    {
        _belt.Dispose();
        _buffer.Dispose();
        _machine.Dispose();
        _machineRecipes.Dispose();
        QueueGo1.Dispose();
    }
}



