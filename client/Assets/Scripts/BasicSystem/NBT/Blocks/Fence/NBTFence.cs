using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTFence : NBTBlock
{
    public virtual string fenceName { get { return "planks_oak"; } }
    public override string name { get { return "Oak Fence"; } }
    public override string id { get { return "minecraft:fence"; } }

    public override float hardness => 2;

    MeshData[] meshes = new MeshData[16];
    public override void Init()
    {
        meshes[0] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0000").ToMeshData();
        meshes[1] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0001").ToMeshData();
        meshes[2] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0010").ToMeshData();
        meshes[3] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0011").ToMeshData();
        meshes[4] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0100").ToMeshData();
        meshes[5] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0101").ToMeshData();
        meshes[6] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0110").ToMeshData();
        meshes[7] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0111").ToMeshData();
        meshes[8] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1000").ToMeshData();
        meshes[9] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1001").ToMeshData();
        meshes[10] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1010").ToMeshData();
        meshes[11] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1011").ToMeshData();
        meshes[12] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1100").ToMeshData();
        meshes[13] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1101").ToMeshData();
        meshes[14] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1110").ToMeshData();
        meshes[15] = Resources.Load<Mesh>("Meshes/blocks/fence/wall_1111").ToMeshData();
    }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_birch"; }

    public override bool isTransparent => true;

    public override bool isFence => true;

    MeshData GetMesh(NBTChunk chunk, Vector3Int pos)
    {
        byte eastType = chunk.GetBlockByte(pos + Vector3Int.right);
        bool eastConnect = !NBTGeneratorManager.IsTransparent(eastType) || NBTGeneratorManager.IsFence(eastType);
        byte southType = chunk.GetBlockByte(pos + Vector3Int.back);
        bool southConnect = !NBTGeneratorManager.IsTransparent(southType) || NBTGeneratorManager.IsFence(southType);
        byte westType = chunk.GetBlockByte(pos + Vector3Int.left);
        bool westConnect = !NBTGeneratorManager.IsTransparent(westType) || NBTGeneratorManager.IsFence(westType);
        byte northType = chunk.GetBlockByte(pos + Vector3Int.forward);
        bool northConnect = !NBTGeneratorManager.IsTransparent(northType) || NBTGeneratorManager.IsFence(northType);

        int index = 0;
        if (westConnect) index += 8;
        if (northConnect) index += 4;
        if (eastConnect) index += 2;
        if (southConnect) index += 1;

        return meshes[index];
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        int faceIndex = TextureArrayManager.GetIndexByName(fenceName);

        MeshData mesh = GetMesh(chunk, pos);

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        NBTMesh nbtMesh = nbtGO.nbtMesh;
        ushort startIndex = nbtMesh.vertexCount;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            SetVertex(nbtMesh, mesh.vertices[i] + pos, faceIndex, mesh.uv[i], skyLight, blockLight, Color.white, mesh.normals[i]);
        }
        foreach (int index in mesh.triangles)
        {
            nbtMesh.triangleArray[nbtMesh.triangleCount++] = (ushort)(startIndex + index);
        }
    }

    public override Mesh GetItemMesh(NBTChunk chunk, byte blockData)
    {
        return meshes[0].mesh;
    }

    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>("GUI/block/" + fenceName);
            mat.mainTexture = tex;

            itemMaterialDict.Add(0, mat);
        }
        return itemMaterialDict[0];
    }
}
