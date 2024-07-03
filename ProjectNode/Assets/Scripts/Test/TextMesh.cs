using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
public class TextMesh : MonoBehaviour
{
    public Mesh _mesh;
    public MeshFilter _Filter;
    public float vertDis = 1;
    public float radio = 2;

    private void Awake()
    {
        DrawMesh();
    }

    public void DrawMesh()
    {
        _mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        
        var hand = new Vector3(0, 0, radio);
        var first = hand;
        var len = Mathf.RoundToInt(0.5f * MathF.PI * radio / vertDis);
        var dPre = Quaternion.Euler(new Vector3(0, 90f / len, 0));
        
        vertices.Add(hand);
        List<int> rowBound = new List<int>();
        rowBound.Add(1);
        for (int i = 1; i < len*2; i++)
        {
            hand = dPre * hand;
            vertices.Add(hand);
            var d = Vector3.Dot(first, hand);
            var r = Mathf.Sqrt(radio * radio - d * d);
            var l=Mathf.RoundToInt(2f * MathF.PI * r / vertDis);
            var rPre = Quaternion.Euler(new Vector3(0, 0, 360f / l));
            var dHand = hand;
            for (int j = 0; j < l; j++)
            {
                dHand = rPre * dHand;
                vertices.Add(dHand);
            }

            rowBound.Add(vertices.Count);
        }

        vertices.Add(new Vector3(0, 0, -radio));
        rowBound.Add(vertices.Count);
        _mesh.vertices = vertices.ToArray();

        var triangles = new List<int>();
        int rowStart = 0;
        for (int i = 1; i < rowBound.Count; i++)
        {
            int firstRowStart = rowStart;
            int firstRowCount = rowBound[i - 1] - rowStart;
            rowStart = rowBound[i - 1];
            int secondRowStart = rowStart;
            int secondRowCount = rowBound[i] - secondRowStart;

            if (firstRowCount == 1)
            {
                var tempRow = 0;
                for (int j = 0; j < secondRowCount; j++)
                {
                    int a= firstRowStart;
                    int b = secondRowStart + tempRow;
                    tempRow++;
                    tempRow %= secondRowCount;
                    int c = secondRowStart + tempRow;
                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);
                }
            } 
            else if (secondRowCount == 1)
            {
                var tempRow = 0;
                for (int j = 0; j < firstRowCount; j++)
                {
                    int a = secondRowStart;
                    int b = firstRowStart + tempRow;
                    tempRow++;
                    tempRow %= firstRowCount;
                    int c = firstRowStart + tempRow;

                    triangles.Add(b);
                    triangles.Add(a);
                    triangles.Add(c);
                }
            }
            else
            {
                
            }
        }

        _mesh.triangles = triangles.ToArray();
        _Filter.mesh = _mesh;
    }

    public void AddTriangles(List<int> triangles, int a, int b, int c)
    {
        if (a > b) (a, b) = (b, a);
        if (b < c) (b, c) = (c, b);
        if (a < b) (a, b) = (b, a);
        
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
        
    }

    private void Update()
    {
        if (!_mesh || _mesh.vertices == null) 
            return;
    }


    private void OnDrawGizmos()
    {
        if (!_mesh || _mesh.vertices == null) 
            return;
        for (var i = 0; i < _mesh.vertices.Length; i++)
        {
            Gizmos.DrawSphere(gameObject.transform.localToWorldMatrix.MultiplyPoint(_mesh.vertices[i])  , 0.01f+0.0001f*i);
        }
    }
}
