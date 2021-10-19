﻿using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;
using protocol.cs_enum;
using System.Linq;

public class ChunkManager
{
    static Dictionary<Vector2Int, Chunk> chunkDict;
    static Dictionary<Vector3Int, CSBlockOrientation> orientationDict;
    static Dictionary<Vector3Int, Vector3Int> dependenceDict;

    public static List<CSBlockAttrs> blockAttrs = new List<CSBlockAttrs>();

    public static void Init()
    {
        chunkDict = new Dictionary<Vector2Int, Chunk>();
        orientationDict = new Dictionary<Vector3Int, CSBlockOrientation>();
        dependenceDict = new Dictionary<Vector3Int, Vector3Int>();

        foreach (CSBlockAttrs attr in blockAttrs)
        {
            if (attr.orient != CSBlockOrientation.Default)
            {
                AddBlockOrientation(attr.pos.ToVector3Int(), attr.orient);
            }
            if (attr.depentPos != null)
            {
                AddBlockDependence(attr.pos.ToVector3Int(), attr.depentPos.ToVector3Int());
            }
        }
    }

    public static void Uninit()
    {
        chunkDict = null;
        orientationDict = null;
        dependenceDict = null;
        TorchMeshGenerator.Instance.Clear();
    }

    static void AddToChunkDict(Chunk chunk)
    {
        chunkDict.Add(chunk.pos, chunk);
    }

    static void RemoveFromChunkDict(Chunk chunk)
    {
        chunkDict.Remove(chunk.pos);
    }

    static Vector2Int keyVec = new Vector2Int();
    public static Chunk GetChunk(int x, int z)
    {
        keyVec.x = x;
        keyVec.y = z;
        if (chunkDict.ContainsKey(keyVec))
        {
            return chunkDict[keyVec];
        }
        return null;
    }

    public static Chunk GetChunk(Vector2Int pos)
    {
        return GetChunk(pos.x, pos.y);
    }

    public static Chunk GetChunk(Vector3Int pos)
    {
        return GetChunk(pos.x, pos.z);
    }

    // intput is global position
    public static Chunk GetChunk(int x, int y, int z)
    {
        int chunkX = Chunk.GetChunkPosByGlobalPos(x);
        int chunkZ = Chunk.GetChunkPosByGlobalPos(z);
        return GetChunk(chunkX, chunkZ);
    }

    // intput is global position
    public static byte GetBlockByte(int x, int y, int z)
    {
        if (y < 0 || y > 255)
        {
            return 0;
        }
        // get chunk position first
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            //calculate block position in chunk
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            //Debug.Log("GetBlockType,globalblockpos=(" + x + "," + y + "," + z + "),chunkpos=(" + chunk.x + "," + chunk.z + "),blockposinchunk=(" + xInChunk + "," + y + "," + zInChunk + ")");
            return chunk.GetBlockByte(xInChunk, y, zInChunk);
        }
        return 0;
    }

    public static int GetBlockLight(Vector3Int pos)
    {
        return GetBlockLight(pos.x, pos.y, pos.z);
    }

    public static Color GetBlockLightColor(Vector3Int pos)
    {
        int light = GetBlockLight(pos.x, pos.y, pos.z);
        float c = light / 15f;
        return new Color(c, c, c);
    }

    // intput is global position
    public static int GetBlockLight(int x, int y, int z)
    {
        if (y < 0 || y > 255)
        {
            return 0;
        }
        // get chunk position first
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            //calculate block position in chunk
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            //Debug.Log("GetBlockType,globalblockpos=(" + x + "," + y + "," + z + "),chunkpos=(" + chunk.x + "," + chunk.z + "),blockposinchunk=(" + xInChunk + "," + y + "," + zInChunk + ")");
            return chunk.GetLightAtPos(xInChunk, y, zInChunk);
        }
        return 15;
    }

    // intput is global position
    public static bool IsStairs(Vector3Int position)
    {
        CSBlockType type = GetBlockType(position);
        return ChunkMeshGenerator.IsStair(type);
    }

    // intput is global position
    public static CSBlockType GetBlockType(Vector3Int position)
    {
        return GetBlockType(position.x, position.y, position.z);
    }

    // intput is global position
    public static CSBlockType GetBlockType(int x, int y, int z)
    {
        return (CSBlockType)GetBlockByte(x, y, z);
    }

    // intput is global position
    public static bool HasNotPlantBlock(Vector3Int pos)
    {
        byte type = GetBlockByte(pos.x, pos.y, pos.z);
        return type > 0 && !ChunkMeshGenerator.type2texcoords[type].isPlant;
    }

    // intput is global position
    public static bool HasBlock(Vector3Int pos)
    {
        return HasBlock(pos.x, pos.y, pos.z);
    }

    // intput is global position
    public static bool HasBlock(int x, int y, int z)
    {
        return GetBlockType(x, y, z) != CSBlockType.None;
    }

    //input is global position
    public static bool HasOpaqueBlock(Vector3Int pos)
    {
        return HasOpaqueBlock(pos.x, pos.y, pos.z);
    }

    //input is global position
    public static bool HasOpaqueBlock(int x, int y, int z)
    {
        byte type = GetBlockByte(x, y, z);
        return type > 0 && !ChunkMeshGenerator.type2texcoords[type].isTransparent;
    }

    //input is global position
    public static bool HasCollidableBlock(int x, int y, int z)
    {
        byte type = GetBlockByte(x, y, z);
        return type > 0 && ChunkMeshGenerator.type2texcoords[type].isCollidable;
    }

    public static void AddBlock(CSBlock block)
    {
        Chunk chunk = GetChunk(block.position.x, block.position.y, block.position.z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(block.position.x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(block.position.z);
            chunk.SetBlockType(xInChunk, block.position.y, zInChunk, block.type);
            Vector3Int pos = block.position.ToVector3Int();
            AddBlockOrientation(pos, block.orient);
            if (block.depentPos != null)
            {
                AddBlockDependence(pos, block.depentPos.ToVector3Int());
            }
            chunk.RebuildMesh();

            // if this block is adjacent to other chunks, refresh nearby chunks
            //foreach (Chunk nearbyChunk in GetNearbyChunks(xInChunk, zInChunk, chunk))
            //{
            //    nearbyChunk.RebuildMesh();
            //}

            if (block.type == CSBlockType.Torch)
            {
                chunk.AddTorch(pos);
            }
        }
    }

    public static void SetChunkDirty(int x, int y)
    {
        Chunk chunk = GetChunk(x, y);
        if (chunk != null)
        {
            chunk.isDirty = true;
        }
    }

    public static void RebuildChunks()
    {
        foreach (Chunk chunk in chunkDict.Values)
        {
            if (chunk.isDirty)
            {
                chunk.RebuildMesh();
            }
        }
    }

    public static void RemoveBlock(Vector3Int pos)
    {
        RemoveBlock(pos.x, pos.y, pos.z);
    }

    public static void RemoveBlock(int x, int y, int z, bool removeBeDependBlocks = true, bool refreshChunks = true)
    {
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            CSBlockType type = chunk.GetBlockType(xInChunk, y, zInChunk);
            if (type == CSBlockType.Torch)
            {
                TorchMeshGenerator.Instance.RemoveTorchAt(x, y, z);
            }
            chunk.SetBlockType(xInChunk, y, zInChunk, CSBlockType.None);
            Vector3Int pos = new Vector3Int(x, y, z);
            RemoveBlockOrientation(pos);
            RemoveBlockDependence(pos);

            //if (removeBeDependBlocks)
            //{
            //    RemoveDependingBlocks(pos);

            //    // removes block on top if exists
            //    if (chunk.HasNotCollidableBlock(xInChunk, y + 1, zInChunk))
            //    {
            //        RemoveBlock(x, y + 1, z, false, false);
            //    }
            //}
             
            if (refreshChunks)
            {
                //chunk.RebuildMesh();

                // if this block is adjacent to other chunks, refresh nearby chunks
                //foreach (Chunk nearbyChunk in GetNearbyChunks(xInChunk, zInChunk, chunk))
                //{
                //    nearbyChunk.RebuildMesh();
                //}
            }

            //Item.CreateBlockDropItem(type, pos);
            //BreakBlockEffect.Create(type, pos);
            //SoundManager.PlayBreakSound(type, PlayerController.instance.gameObject);

            if (type == CSBlockType.Torch)
            {
                TorchMeshGenerator.Instance.RemoveTorchAt(pos);
            }
        }
    }

    public static void AddBlockOrientation(Vector3Int pos, CSBlockOrientation orientation)
    {
        if (orientation != CSBlockOrientation.Default)
        {
            orientationDict.Add(pos, orientation);
        }
    }

    public static void RemoveBlockOrientation(Vector3Int pos)
    {
        if (orientationDict.ContainsKey(pos))
        {
            orientationDict.Remove(pos);
        }
    }

    public static CSBlockOrientation GetBlockOrientation(Vector3Int pos)
    {
        if (orientationDict.ContainsKey(pos))
        {
            return orientationDict[pos];
        }
        else
        {
            return CSBlockOrientation.Default;
        }
    }

    public static void AddBlockDependence(Vector3Int pos, Vector3Int beDepentPos)
    {
        dependenceDict.Add(pos, beDepentPos);
    }

    public static void RemoveBlockDependence(Vector3Int pos)
    {
        dependenceDict.Remove(pos);
    }

    public static Vector3Int GetBlockDependence(Vector3Int pos)
    {
        return dependenceDict[pos];
    }

    public static void UnloadChunk(int x, int z)
    {
        //Debug.Log("UnloadChunk,x=" + x + ",z=" + z);
        NBTHelper.RemoveChunk(x, z);
    }

    public async static void ChunksEnterLeaveViewReq(List<Vector2Int> enterViewChunks, List<Vector2Int> leaveViewChunks = null)
    {
        List<NBTChunk> chunks = new List<NBTChunk>();
        foreach (Vector2Int chunkPos in enterViewChunks)
        {
            NBTChunk chunk = await NBTHelper.LoadChunkAsync(chunkPos.x, chunkPos.y);
            chunks.Add(chunk);
        }
        ChunkRefresher.Add(chunks);
        ChunkChecker.FinishRefresh();

        if (leaveViewChunks != null)
        {
            foreach (Vector2Int chunk in leaveViewChunks)
            {
                UnloadChunk(chunk.x, chunk.y);
            }
        }
    }

    public static void PreloadChunks(List<Vector2Int> enterViewChunks)
    {
        foreach (Vector2Int chunkPos in enterViewChunks)
        {
            NBTChunk chunk = NBTHelper.LoadChunk(chunkPos.x, chunkPos.y);
            ChunkRefresher.Add(chunk);
        }
        ChunkRefresher.ForceRefreshAll();
    }

    #region enter/leave view
    //public static void ChunksEnterLeaveViewReq(List<Vector2Int> enterViewChunks, List<Vector2Int> leaveViewChunks = null)
    //{
    //    CSChunksEnterLeaveViewReq req = new CSChunksEnterLeaveViewReq();

    //    List<CSVector2Int> enter = new List<CSVector2Int>();
    //    foreach (Vector2Int chunk in enterViewChunks)
    //    {
    //        CSVector2Int c = new CSVector2Int
    //        {
    //            x = chunk.x,
    //            y = chunk.y
    //        };
    //        enter.Add(c);
    //    }
    //    req.EnterViewChunks.AddRange(enter);

    //    if (leaveViewChunks != null)
    //    {
    //        List<CSVector2Int> leave = new List<CSVector2Int>();
    //        foreach (Vector2Int chunk in leaveViewChunks)
    //        {
    //            CSVector2Int c = new CSVector2Int
    //            {
    //                x = chunk.x,
    //                y = chunk.y
    //            };
    //            leave.Add(c);
    //        }
    //        req.LeaveViewChunks.AddRange(leave);
    //    }

    //    //Debug.Log("CS_CHUNKS_ENTER_LEVAE_VIEW_REQ," + req.EnterViewChunks.Count + "," + req.LeaveViewChunks.Count);
    //    NetworkManager.SendPkgToServer(ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ, req, ChunksEnterLeaveViewRes);
    //}

    static void ChunksEnterLeaveViewRes(object data)
    {
        CSChunksEnterLeaveViewRes rsp = NetworkManager.Deserialize<CSChunksEnterLeaveViewRes>(data);

        //Debug.Log("CSChunksEnterLeaveViewRes," + rsp.EnterViewChunks.Count + "," + rsp.LeaveViewChunks.Count);
        if (rsp.RetCode == 0)
        {
            foreach (CSVector2Int csv in rsp.LeaveViewChunks)
            {
                //UnloadChunk(csv.x, csv.y);
            }
            foreach (CSChunk cschunk in rsp.EnterViewChunks)
            {
                //LoadChunk(cschunk);
            }
            if (!PlayerController.isInitialized)
            {
                PlayerController.Init();
                //LocalNavMeshBuilder.Init();
                //ChunkRefresher.ForceRefreshAll();
            }
            ChunkChecker.FinishRefresh();
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
    #endregion
}
