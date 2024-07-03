using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateSphereMesh : MonoBehaviour
{
    public Mesh _mesh;
    public MeshFilter _Filter;
    public int radius = 1; // 纬线数量
    public int recursions = 4;   // 经线数量

    private void Update()
    {
        _Filter.mesh = CreateIcosphere(radius, recursions);
    }
    
    public static Mesh CreateIcosphere(float radius, int recursions)
    {
        // 计算初始顶点和面
        var t = (1f + Mathf.Sqrt(5f)) / 2f;
        var vertices = new Vector3[]
        {
            new Vector3(-1f, t, 0f).normalized * radius,
            new Vector3(1f, t, 0f).normalized * radius,
            new Vector3(-1f, -t, 0f).normalized * radius,
            new Vector3(1f, -t, 0f).normalized * radius,
            new Vector3(0f, -1f, t).normalized * radius,
            new Vector3(0f, 1f, t).normalized * radius,
            new Vector3(0f, -1f, -t).normalized * radius,
            new Vector3(0f, 1f, -t).normalized * radius,
            new Vector3(t, 0f, -1f).normalized * radius,
            new Vector3(t, 0f, 1f).normalized * radius,
            new Vector3(-t, 0f, -1f).normalized * radius,
            new Vector3(-t, 0f, 1f).normalized * radius
        };

        var indices = new int[]
        {
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,
            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,
            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,
            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };

        for (int i = 0; i < recursions; i++)
        {
            var newVertices = new List<Vector3>();
            var newTriangles = new List<int>();

            // 保留原始顶点
            newVertices.AddRange(vertices);

            // 遍历当前索引列表，细分每个三角形
            for (int j = 0; j < indices.Length; j += 3)
            {
                int a = indices[j];
                int b = indices[j + 1];
                int c = indices[j + 2];

                // 计算新顶点
                Vector3 ab = (vertices[a] + vertices[b]) / 2f;
                Vector3 bc = (vertices[b] + vertices[c]) / 2f;
                Vector3 ca = (vertices[c] + vertices[a]) / 2f;

                ab.Normalize();
                bc.Normalize();
                ca.Normalize();

                int abIndex = newVertices.Count;
                newVertices.Add(ab);
                int bcIndex = newVertices.Count;
                newVertices.Add(bc);
                int caIndex = newVertices.Count;
                newVertices.Add(ca);

                // 构建新三角形索引
                newTriangles.AddRange(new[] { a, abIndex, caIndex });
                newTriangles.AddRange(new[] { b, bcIndex, abIndex });
                newTriangles.AddRange(new[] { c, caIndex, bcIndex });
                newTriangles.AddRange(new[] { abIndex, bcIndex, caIndex });
            }

            // 更新顶点和索引列表
            vertices = newVertices.ToArray();
            indices = newTriangles.ToArray();
        }
        
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        return mesh;
    }
    
    public void Subdivide(Vector3[] inputVertices, int[] inputIndices, int currentDepth, int maxDepth, ref List<Vector3> outputVertices, ref List<int> outputTriangles)
    {
        if (currentDepth == maxDepth || inputIndices.Length == 0) return; // 达到最大深度或无更多索引则停止

        // 临时存储新顶点和索引
        var tempVertices = new List<Vector3>(inputVertices);
        var tempTriangles = new List<int>();

        for (int j = 0; j < inputIndices.Length; j += 3)
        {
            int a = inputIndices[j];
            int b = inputIndices[j + 1];
            int c = inputIndices[j + 2];

            Vector3 ab = (inputVertices[a] + inputVertices[b]) / 2f;
            Vector3 bc = (inputVertices[b] + inputVertices[c]) / 2f;
            Vector3 ca = (inputVertices[c] + inputVertices[a]) / 2f;

            ab.Normalize();
            bc.Normalize();
            ca.Normalize();

            int abIndex = tempVertices.Count;
            tempVertices.Add(ab);
            int bcIndex = tempVertices.Count;
            tempVertices.Add(bc);
            int caIndex = tempVertices.Count;
            tempVertices.Add(ca);

            tempTriangles.AddRange(new[] { a, abIndex, caIndex });
            tempTriangles.AddRange(new[] { b, bcIndex, abIndex });
            tempTriangles.AddRange(new[] { c, caIndex, bcIndex });
            tempTriangles.AddRange(new[] { abIndex, bcIndex, caIndex });
        }

        // 递归调用自身，基于当前细分结果进行下一次细分
        Subdivide(tempVertices.ToArray(), tempTriangles.ToArray(), currentDepth + 1, maxDepth, ref outputVertices, ref outputTriangles);

        // 如果到达最顶层（即开始的调用），则将最终结果赋值给输出变量
        if (currentDepth == 0)
        {
            outputVertices = tempVertices;
            outputTriangles = tempTriangles;
        }
    }
    
    // 辅助方法获取由索引表示的面
    private static IEnumerable<int[]> GetFacesFromIndices(int[] indices)
    {
        for (int i = 0; i < indices.Length; i += 3)
        {
            yield return new[] { indices[i], indices[i + 1], indices[i + 2] };
        }
    }
}


