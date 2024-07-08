using UnityEngine;

public static class NoiseTool
{
    static Vector2[] GlobalVertex = new Vector2[4];
    const uint BIT_NOISE1 = 0x85297A4D;
    const uint BIT_NOISE2 = 0x68E31DA4;
    const uint BIT_NOISE3 = 0x1B56C4E9;
    private static Vector2 GlobalOffset = new Vector2(0, 0);
    public static int NoiseHash11(int position)
    {
        uint mangled = (uint)position;
        mangled *= BIT_NOISE1;
        mangled ^= (mangled >> 8);
        mangled += BIT_NOISE2;
        mangled ^= (mangled << 8);
        mangled *= BIT_NOISE3;
        mangled ^= (mangled >> 8);
        return (int)(mangled%1024);
    }
    
    public static int NoiseHashWithSeed(int position,int seed)
    {
        uint mangled = (uint)position ^ (uint)seed;
        mangled *= BIT_NOISE1;
        mangled ^= (mangled >> 8);
        mangled += BIT_NOISE2;
        mangled ^= (mangled << 8);
        mangled *= BIT_NOISE3;
        mangled ^= (mangled >> 8);
        return (int)(mangled%1024);
    }
    
    public static Vector2 NoiseHash22(Vector2 position)
    {
        var v = new Vector2(
            NoiseHash11(0x651A6BE3 * (int)position.x - (int)position.y) % 1024,
            NoiseHash11((int)position.x * (int)position.y + 0x218AE247) % 1024);
        return v * (1f / 1024f);
    }
    
    public static int NoiseHash21(Vector2 position)
    {
        return NoiseHash11(0x651A6BE1 * (int)position.x + (int)position.y) % 1024;
    }
    
    public static Vector3 NoiseHash33(Vector3 position)
    {
        const int n1 = 0x651A6BE3;
        const int n2 = 0x218A6147;
        const int n3 = 0x118A5191;
        var v=new Vector3(
            NoiseHash11((int)position.x ^ n1 + (int)position.y ^ n2 - (int)position.z ^ n3) % 1024,
            NoiseHash11((int)position.x ^ n3 - (int)position.y ^ n2 + (int)position.z ^ n2) % 1024,
            NoiseHash11((int)position.x ^ 0x21613122 - (int)position.y ^ n3 - (int)position.z ^ n2) % 1024);
        return v * (1f / 1024f);
    }
    
    public static int NoiseHash31(Vector3 position)
    {
        return NoiseHash11((int)((int)position.x * 0x651A6BE6 - (int)position.y * 0xCB251062 + (int)position.z));
    }

    public static int RandIntWithSeed(Vector2 position,int seed)
    {
        position *= 1024;
        return NoiseHashWithSeed(0x651A6BE1 * (int)position.x + (int)position.y, seed) % 1024;
    }

    public static float RandIntWithSeed01(Vector2 position, int seed)
    {
        return RandIntWithSeed(position, seed) / 1024f;
    }

    public static float PerlinNoise(Vector2 position)
    {
        position += GlobalOffset;
        Vector2 w = position;
        return Mathf.Clamp(
                Mathf.Lerp(
                    Mathf.Lerp(
                        Vector2.Dot(GlobalVertex[0], position),
                        Vector2.Dot(GlobalVertex[1], position - new Vector2(1.0f, 0.0f)),
                        w.x),
                    Mathf.Lerp(
                        Vector2.Dot(GlobalVertex[2], position - new Vector2(0.0f, 1.0f)),
                        Vector2.Dot(GlobalVertex[3], position - new Vector2(1.0f, 1.0f)),
                        w.x),
                    w.y)
                , -1, 1)
            ;
    }

    public static void PreHandlePerlinNoise(Vector2 position2D, int crystalSize)
    {
        Vector2 pi = new Vector2(
            Mathf.Floor(position2D.x / crystalSize),
            Mathf.Floor(position2D.y / crystalSize));
        Vector2[] vertex = new Vector2[]
        {
            new Vector2(pi.x, pi.y),
            new Vector2(pi.x + 1, pi.y),
            new Vector2(pi.x, pi.y + 1),
            new Vector2(pi.x + 1, pi.y + 1),
        };

        for (int i = 0; i < 4; ++i)
            GlobalVertex[i] = NoiseHash22(vertex[i]);

        GlobalOffset = (position2D - pi * crystalSize) / crystalSize;
    }

}
