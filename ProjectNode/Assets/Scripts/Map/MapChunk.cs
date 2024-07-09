using UnityEngine;

public class MapChunk
{
    public Vector3 Center { get; private set; }
    
    /// <summary>
    /// 高度
    /// </summary>
    public short[,] BlockHeight { get; private set;}
    /// <summary>
    /// 温度
    /// </summary>
    public float[,] BlockTemperature { get; private set;}
    
    /// <summary>
    /// 湿度
    /// </summary>
    public float[,] BlockHumidity { get; private set;}
    /// <summary>
    /// 生态
    /// </summary>
    public short[,] BlockBiome { get; private set;}
    public const int MaxHeight = 512;
    public const int MaxBlockWidth = 16;

    public MapChunk()
    {
        BlockHeight = new short[MaxBlockWidth, MaxBlockWidth];
        BlockTemperature = new float[MaxBlockWidth, MaxBlockWidth];
        BlockHumidity = new float[MaxBlockWidth, MaxBlockWidth];
        BlockBiome = new short[MaxBlockWidth, MaxBlockWidth];
        BlockBiome = new short[MaxBlockWidth, MaxBlockWidth];
    }
}
