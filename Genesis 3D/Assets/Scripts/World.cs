using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

  public Transform player;
  public Vector3 spawnPosition;

  public Material material;
  public BlockType[] blocktypes;

  Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

  List<ChunkCoord> activeChunks = new List<ChunkCoord>();
  ChunkCoord playerChunkCoord;
  ChunkCoord playerLastChunkCoord;

  private void Start()
  {
    spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 2f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
    GenerateWorld();
    playerLastChunkCoord = GetChunckCoordFromVector3(player.position);
  }

  private void Update()
  {
    playerChunkCoord = GetChunckCoordFromVector3(player.position);

    if (!playerChunkCoord.Equals(playerLastChunkCoord))
      CheckViewDistance();
  }

  void GenerateWorld ()
  {
    for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.WorldSizeInChunks; x++)
    {
      for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.WorldSizeInChunks; z++)
      {
        CreateNewChunk(x, z);
      }
    }
    player.position = spawnPosition;
  }

  ChunkCoord GetChunckCoordFromVector3 (Vector3 pos)
  {
    int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
    int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
    return new ChunkCoord(x, z);
  }
  void CheckViewDistance ()
  {
    ChunkCoord coord = GetChunckCoordFromVector3(player.position);

    List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

    for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++)
    {
      for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
      {
        if (IsChunkInWorld (new ChunkCoord (x, z)))
        {
          if (chunks[x, z] == null)
            CreateNewChunk(x, z);
          else if (!chunks[x, z].isActive)
          {
            chunks[x, z].isActive = true;
            activeChunks.Add(new ChunkCoord(x, z));
          }
        }
        for (int i = 0; i < previouslyActiveChunks.Count; i++)
        {
          if (previouslyActiveChunks[i].Equals (new ChunkCoord(x, z)))
            previouslyActiveChunks.RemoveAt(i);
        }
      }
    }

    foreach (ChunkCoord c in previouslyActiveChunks)
      chunks[c.x, c.z].isActive = false;
  }

  public byte GetVoxel (Vector3 pos)
  {
    if (!IsVoxelInWorld(pos))
      return 0;
    if (pos.y < 1)
      return 1;
    else if (pos.y == VoxelData.ChunkHeight - 1)
      return 3;
    else
      return 2;
  }

  void CreateNewChunk (int x, int z)
  {
    chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
    activeChunks.Add(new ChunkCoord(x, z));
  }

  bool IsChunkInWorld (ChunkCoord coord)
  {
    if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.WorldSizeInChunks - 1)
      return true;
    else
      return
        false;
  }

  bool IsVoxelInWorld (Vector3 pos)
  {
    if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
      return true;
    else
      return
        false;
  }

}

[System.Serializable]
public class BlockType
{

  public string blockName;
  public bool isSolid;

  [Header("Texture Values")]
  public int backFaceTexture;
  public int frontFaceTexture;
  public int topFaceTexture;
  public int bottomFaceTexture;
  public int leftFaceTexture;
  public int rightFaceTexture;

  // Back, Front, Top, Bottom, Left, Right

  public int GetTextureID (int faceIndex)
  {
    switch (faceIndex)
    {
      case 0:
        return backFaceTexture;
      case 1:
        return frontFaceTexture;
      case 2:
        return topFaceTexture;
      case 3:
        return bottomFaceTexture;
      case 4:
        return leftFaceTexture;
      case 5:
        return rightFaceTexture;
      default:
        Debug.Log("Error in GetColorID; invalid faceIndex");
        return 0;
    }
  }
}
