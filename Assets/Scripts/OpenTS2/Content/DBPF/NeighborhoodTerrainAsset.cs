﻿using System;
using System.Collections.Generic;
using OpenTS2.Files.Formats.DBPF.Types;
using UnityEngine;

namespace OpenTS2.Content.DBPF
{
    public class NeighborhoodTerrainAsset : AbstractAsset
    {
        public int Width { get; }
        public int Height { get; }
        public float SeaLevel { get; }
        public string TerrainType { get; }
        public FloatArray2D VertexHeights { get; }

        public NeighborhoodTerrainAsset(int width, int height, float seaLevel, string terrainType,
            FloatArray2D vertexHeights) =>
            (Width, Height, SeaLevel, TerrainType, VertexHeights) =
            (width, height, seaLevel, terrainType, vertexHeights);

        public Mesh MakeMesh()
        {
            var width = VertexHeights.values.GetLength(0);
            var height = VertexHeights.values.GetLength(1);
            var terrainMesh = new Mesh();
            var terrainVertices = new List<Vector3>();

            for(var i=0;i<width;i++)
            {
                for(var j=0;j<height;j++)
                {
                    // 10 is the actual size of a cell in the neighborhood terrain grid. This translates to lot sizes such as a 10x10 in-game lot being a single cell.
                    var vertexPosition = new Vector3(i * 10f, VertexHeights.values[i, j], j * 10f);
                    terrainVertices.Add(vertexPosition);
                }
            }

            terrainMesh.SetVertices(terrainVertices);

            width--;
            height--;

            int[] terrainIndices = new int[width * height * 6];
            for (int ti = 0, vi = 0, i = 0; i < width; i++, vi++)
            {
                for (int j = 0; j <  height; j++, ti += 6, vi++)
                {
                    terrainIndices[ti] = vi + height + 2;
                    terrainIndices[ti + 3] = terrainIndices[ti + 2] = vi + 1;
                    terrainIndices[ti + 4] = terrainIndices[ti + 1] = vi + height + 1;
                    terrainIndices[ti + 5] = vi;
                }
            }

            terrainMesh.SetIndices(terrainIndices, MeshTopology.Triangles, 0);
            terrainMesh.RecalculateNormals();
            return terrainMesh;
        }

        public void ApplyToTerrain(Terrain terrain)
        {
            var heightMap = new float[VertexHeights.values.GetLength(1), VertexHeights.values.GetLength(0)];
            for (var i = 0; i < VertexHeights.values.GetLength(0); i++)
            {
                for (var j = 0; j < VertexHeights.values.GetLength(1); j++)
                {
                    // TODO: divide by the max-height here to get a proper 0 to 1 float. Picked 600 arbitrarily for now.
                    heightMap[j, i] = VertexHeights.values[i, j] / 600.0f;
                }
            }

            terrain.terrainData = new TerrainData
            {
                size = new Vector3(4096, 4096, 4096),
                heightmapResolution = Math.Max(VertexHeights.values.GetLength(0), VertexHeights.values.GetLength(1))
            };
            terrain.terrainData.SetHeights(0, 0, heightMap);
            terrain.terrainData.SyncHeightmap();
        }
    }
}