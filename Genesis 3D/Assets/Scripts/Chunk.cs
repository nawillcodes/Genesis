using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

  public ChunkCoord coord;

  GameObject chunkObject;
  // Refrence to a mesh renderer and a mesh filter
  MeshRenderer meshRenderer;
  MeshFilter meshFilter;

  // We need to keep track of the triangles
  int vertexIndex = 0;
  // array of verticies we need to store for the triangles -- so we need 2 lists 1.vertices 2.triangles
  List<Vector3> vertices = new List<Vector3> ();
  List<int> triangles = new List<int> ();
  // for Textures
  List<Vector2> uvs = new List<Vector2> ();

  //is the block solid?
  byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData. ChunkHeight, VoxelData.ChunkWidth];

  World world;

  // Start is called before the first frame update
  public Chunk (ChunkCoord _coord, World _world)
  {
    coord = _coord;
    world = _world;
    chunkObject = new GameObject();
    meshFilter = chunkObject.AddComponent<MeshFilter>();
    meshRenderer = chunkObject.AddComponent<MeshRenderer>();

    meshRenderer.material = world.material;
    chunkObject.transform.SetParent(world.transform);

    chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
    chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

    PopulateVoxelMap ();
    CreateMeshData ();
    CreateMesh ();
  }

  void PopulateVoxelMap ()
  {
    for (int y = 0; y < VoxelData.ChunkHeight; y++)
    {
      for (int x = 0; x < VoxelData.ChunkWidth; x++)
      {
        for (int z = 0; z < VoxelData.ChunkWidth; z++)
        {
          voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
        }
      }
    }
  }

  void CreateMeshData ()
  {
    for (int y = 0; y < VoxelData.ChunkHeight; y++)
    {
      for (int x = 0; x < VoxelData.ChunkWidth; x++)
      {
        for (int z = 0; z < VoxelData.ChunkWidth; z++)
        {
          AddVoxelDataToChunk (new Vector3(x, y, z));
        }
      }
    }
  }

  public bool isActive
  {
    get { return chunkObject.activeSelf; }
    set { chunkObject.SetActive(value); }
  }

  public Vector3 position
  {
    get { return chunkObject.transform.position; }
  }

  bool IsVoxelInChunk (int x, int y, int z)
  {
    if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight -1 || z < 0 || z > VoxelData.ChunkWidth -1)
      return false;
    else
      return true;
  }

  bool CheckVoxel (Vector3 pos)
  {
    int x = Mathf.FloorToInt(pos.x);
    int y = Mathf.FloorToInt(pos.y);
    int z = Mathf.FloorToInt(pos.z);

    if (!IsVoxelInChunk(x, y, z))
      return world.blocktypes[world.GetVoxel(pos + position)].isSolid;

        return world.blocktypes [voxelMap [x, y, z]].isSolid;
  }

  void AddVoxelDataToChunk (Vector3 pos)
  {
    // loop through each integer 6 times to create the (6) faces of the voxel (see:VoxelData Class)
    for (int f = 0; f < 6; f++)
    {
      if (!CheckVoxel(pos + VoxelData.faceChecks[f]))
      {
        /*
        for (int i = 0; i < 6; i++)
        { // first call each integer from voxelTris data...
        int triangleIndex = VoxelData.voxelTris [t, i];
        // then use that integer to decide which vertex to grab from voxelVerts array...
        vertices.Add (VoxelData.voxelVerts [triangleIndex] + pos);
        // to use those Vector3 coordinates to create the triangle
        triangles.Add (vertexIndex);

        // add texture here
        uvs.Add (VoxelData.voxelUvs [i]);

        // make sure to increment to the next index in the array
        vertexIndex++;
        */
        // /*
        byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
        // add the 4 vertices of each face
        vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [f, 0]]);
        vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [f, 1]]);
        vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [f, 2]]);
        vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [f, 3]]);

        // add the uvs (color) here
        /*
        uvs.Add (VoxelData.voxelUvs [0]);
        uvs.Add (VoxelData.voxelUvs [1]);
        uvs.Add (VoxelData.voxelUvs [2]);
        uvs.Add (VoxelData.voxelUvs [3]);
        */
        AddTexture(world.blocktypes[blockID].GetTextureID(f));
        // add the triangles to each face
        triangles.Add (vertexIndex);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 2);
        triangles.Add (vertexIndex + 2);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 3);
        vertexIndex += 4;
        // */
      }
    }
  }
  //
  void CreateMesh ()
  {
    // create a mesh that we can put the above details in
    Mesh mesh = new Mesh();
    mesh.vertices = vertices.ToArray ();
    mesh.triangles = triangles.ToArray ();
    mesh.uv = uvs.ToArray ();

    mesh.RecalculateNormals ();

    meshFilter.mesh = mesh;
  }

  void AddTexture (int textureID)
  {
    float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
    float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

    x *= VoxelData.NormalizedBlockTextureSize;
    y *= VoxelData.NormalizedBlockTextureSize;

    y = 1f - y - VoxelData.NormalizedBlockTextureSize;

    uvs.Add (new Vector2(x, y));
    uvs.Add (new Vector2(x, y + VoxelData.TextureAtlasSizeInBlocks));
    uvs.Add (new Vector2(x + VoxelData.TextureAtlasSizeInBlocks, y));
    uvs.Add (new Vector2(x + VoxelData.TextureAtlasSizeInBlocks, y + VoxelData.TextureAtlasSizeInBlocks));
  }
}

public class ChunkCoord {

  public int x;
  public int z;

  public ChunkCoord (int _x, int _z)
  {
    x = _x;
    z = _z;
  }

  public bool Equals (ChunkCoord other)
  {
    if (other == null)
      return false;
    else if (other.x == x && other.z == z)
      return true;
    else
      return false;
  }
}
