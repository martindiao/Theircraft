﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        SettingsPanel.Init();
        ChunkChecker.Init();
        ChunkRefresher.Init();
        ChunkManager.Init();
        ChunkPool.Init();
        OtherPlayerManager.Init();
        ItemSelectPanel.Show();
        ChatPanel.ShowChatPanel();
        InventorySystem.Init();

        List<Vector2Int> preloadChunks = Utilities.GetSurroudingChunks(PlayerController.GetCurrentChunkPos());
        ChunkManager.ChunksEnterLeaveViewReq(preloadChunks);

        PlayerController.Init();
        ChunkRefresher.ForceRefreshAll();
    }

    public static Queue<Chunk> rebuildQueue = new Queue<Chunk>();
    void Update()
    {
        if (PlayerController.isInitialized)
        {
            ChunkChecker.Update();
            ChunkRefresher.Update();
        }
    }

    private void OnDestroy()
    {
        ChunkManager.Uninit();
        ChunkPool.Uninit();
        InputManager.Destroy();
        UISystem.DestroyUIRoot();
        PlayerController.Destroy();
        NBTHelper.Uninit();
    }
}
