using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

[Serializable]
public struct MachineProgress
{
    public float Progress;
    public float ProgressOnce;
    public float PowerSpeed;
    public int Speed;
    public int RecipeId;
    public int CostCount1;
    public int CostCount2;
    public int CostCount3;
    public int ProduceCount1;
    public int ProduceCount2;
    public int ProduceCount3;
    
}

[BurstCompile]
public struct MachineTask : IJobParallelFor
{
    public UnsafeList<MachineProgress> _machine;
    public UnsafeList<MachineRecipe> _machineRecipes;

    public unsafe void Execute(int index)
    {
        
        ref var d = ref _machine.Ptr[index];
        ref var recipe = ref _machineRecipes.Ptr[d.RecipeId];
        float realSpeed = 1;
        //判断消耗品

        var costNum1 = recipe.CostCount1 == 0 ? 0 : d.CostCount1 / recipe.CostCount1;
        var costNum2 = recipe.CostCount2 == 0 ? 0 : d.CostCount2 / recipe.CostCount2;
        var costNum3 = recipe.CostCount3 == 0 ? 0 : d.CostCount3 / recipe.CostCount3;
        var maxCostCount = math.min(costNum1, math.min(costNum2, costNum3));
        realSpeed *= math.min(1, maxCostCount);
        //实际速度
        realSpeed *= d.PowerSpeed * d.Speed;

        //进度
        d.Progress += realSpeed * 60;

        //计算次数
        d.Progress = math.min(d.Progress, d.ProgressOnce * 5);
        
        if (maxCostCount==0)
            return;
        
        
        int count = (int)(d.Progress / d.ProgressOnce);
        count = math.min(count, maxCostCount);
        d.Progress -= count * d.ProgressOnce;

        d.ProduceCount1 += recipe.ProduceCount1 * count;
        d.ProduceCount2 += recipe.ProduceCount2 * count;
        d.ProduceCount3 += recipe.ProduceCount3 * count;
        d.CostCount1 -= recipe.CostCount1 * count;
        d.CostCount2 -= recipe.CostCount2 * count;
        d.CostCount3 -= recipe.CostCount3 * count;
        count++;
    }
}

