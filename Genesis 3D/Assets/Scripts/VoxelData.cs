using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class to store data that belongs to Voxels
public static class VoxelData
{
  //define chunk Size
  public static readonly int ChunkWidth = 20;
  public static readonly int ChunkHeight = 10;
  public static readonly int WorldSizeInChunks = 100;

  public static int WorldSizeInVoxels
  {
    get { return WorldSizeInChunks * ChunkWidth; }
  }

  public static readonly int ViewDistanceInChunks = 5;

  public static readonly int TextureAtlasSizeInBlocks = 3;
  public static float NormalizedBlockTextureSize
  {
    get { return 1f / (float)TextureAtlasSizeInBlocks; }
  }

  // Make a Cube defined by it's Vertices (AKA a Voxel!)
  // 8 vertices equadistant from another make a cube -- store (8) new vertices in a read only array
  public static readonly Vector3[] voxelVerts = new Vector3[8]
  {
    // 0
    new Vector3(0.0f,0.0f,0.0f),
    // 1
    new Vector3(1.0f,0.0f,0.0f),
    // 2
    new Vector3(1.0f,1.0f,0.0f),
    // 3
    new Vector3(0.0f,1.0f,0.0f),
    // 4
    new Vector3(0.0f,0.0f,1.0f),
    // 5
    new Vector3(1.0f,0.0f,1.0f),
    // 6
    new Vector3(1.0f,1.0f,1.0f),
    // 7
    new Vector3(0.0f,1.0f,1.0f),
  };
  // represents offsets telling us where to look (making sure we don't render faces that don't need to be)
  public static readonly Vector3[] faceChecks = new Vector3[6]
  {
    new Vector3(0.0f, 0.0f, -1.0f),
    new Vector3(0.0f, 0.0f, 1.0f),
    new Vector3(0.0f, 1.0f, 0.0f),
    new Vector3(0.0f, -1.0f, 0.0f),
    new Vector3(-1.0f, 0.0f, 0.0f),
    new Vector3(1.0f, 0.0f, 0.0f)
  };

  // Unity uses triangles to render *Mesh* (see:ChuckClass) -- Store voxel triangels in a read only lookup-table (size 6)
  public static readonly int[,] voxelTris = new int[6, 4]
  { // 0, 1, 2, 2, 1, 0
    {0, 3, 1, 2}, // Back Face
    {5, 6, 4, 7}, // Front Face
    {3, 7, 2, 6}, // Top Face
    {1, 5, 0, 4}, // Bottom Face
    {4, 7, 0, 3}, // Left Face
    {1, 2, 5, 6} // Right Face
  };

  // using faces of the cube to render textures (see:ChuckClass)
  public static readonly Vector2[] voxelUvs = new Vector2[4]
  {
    //need a texture for each face of
    new Vector2 (0.0f, 0.0f),
    new Vector2 (0.0f, 1.0f),
    new Vector2 (1.0f, 0.0f),
    /*new Vector2 (1.0f, 0.0f),
    new Vector2 (0.0f, 1.0f),*/
    new Vector2 (1.0f, 1.0f)
  };

}
