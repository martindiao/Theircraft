using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBed : NBTBlock
{
    public override string name { get { return "Bed"; } }
    public override string id { get { return "minecraft:bed"; } }

    public override float topOffset => 0.0635f;

    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override bool isTileEntity => true;

    public override bool isTransparent => true;

    // 0 = south foot
    // 1 = west foot
    // 2 = north foot
    // 3 = east foot
    // 8 = south head
    // 9 = west head
    // 10 = north head
    // 11 = east head
    Mesh GetMesh(byte blockData)
    {
        if (!itemMeshDict.ContainsKey(blockData))
        {
            switch (blockData)
            {
                case 11:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_east");
                    break;
                case 3:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_east");
                    break;
                case 8:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_north");
                    break;
                case 0:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_north");
                    break;
                case 9:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_west");
                    break;
                case 1:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_west");
                    break;
                case 10:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_south");
                    break;
                case 2:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_south");
                    break;
            }
        }
        return itemMeshDict[blockData];
    }
    Material GetMaterial(byte blockData)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            itemMaterialDict[0] = Resources.Load<Material>("Materials/bed");
        }
        return itemMaterialDict[0];
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        Mesh mesh = GetMesh(blockData);
        GameObject bed = new GameObject("bed");
        bed.transform.parent = chunk.special.transform;
        bed.transform.localPosition = pos;
        bed.AddComponent<MeshFilter>().sharedMesh = mesh;
        bed.AddComponent<MeshRenderer>().sharedMaterial = GetMaterial(blockData);
        bed.AddComponent<MeshCollider>().sharedMesh = mesh;
        bed.layer = LayerMask.NameToLayer("Chunk");
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        return GetMesh(0);
    }

    public override Material GetItemMaterial(byte data)
    {
        return GetMaterial(data);
    }
}
