using UnityEngine;

public class MapChunk
{
    public Vector3 Center { get; private set; }
    public short[,] BlockHeight { get; private set;}
    public float[,] BlockTemperature { get; private set;}
    public float[,] BlockHumidity { get; private set;}
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
