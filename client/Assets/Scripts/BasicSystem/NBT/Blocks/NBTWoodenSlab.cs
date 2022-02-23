using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenSlab : NBTBlock
{
    public override string name => "Wooden Slab";
    public override string id => "minecraft:wooden_slab";

    public override bool isTransparent => true;

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Oak Wood Slab";
            case 1:
                return "Spruce Wood Slab";
            case 2:
                return "Birch Wood Slab";
            case 3:
                return "Jungle Wood Slab";
            case 4:
                return "Acacia Wood Slab";
            case 5:
                return "Dark Oak Wood Slab";
        }
        return "Wood Slab";
    }

    public override float hardness => 2f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override byte GetDropItemData(byte data)
    {
        if (data >= 8)
        {
            data -= 8;
        }
        return data;
    }

    string GetNameByData(int data)
    {
        switch (data)
        {
            case 0:
                return "planks_oak";
            case 1:
                return "planks_spruce";
            case 2:
                return "planks_birch";
            case 3:
                return "planks_jungle";
            case 4:
                return "planks_acacia";
            case 5:
                return "planks_big_oak";
        }
        return "planks_oak";
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetNameByData(data)); }
    public override int GetBottomIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetNameByData(data)); }
    public override int GetFrontIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetNameByData(data)); }
    public override int GetBackIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetNameByData(data)); }
    public override int GetLeftIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetNameByData(data)); }
    public override int GetRightIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetNameByData(data)); }

    public override string GetBreakEffectTexture(byte data)
    {
        return GetNameByData(data);
    }

    void FillMesh(FaceAttributes fa, CubeAttributes ca, NBTMesh nbtMesh)
    {
        if (ca.blockData >= 8)
        {
            fa.pos = frontVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = frontVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.normal = Vector3.forward;
        AddFace(nbtMesh, fa, ca);

        if (ca.blockData >= 8)
        {
            fa.pos = backVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = backVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.normal = Vector3.back;
        AddFace(nbtMesh, fa, ca);

        if (ca.blockData >= 8)
            fa.pos = topVertices_top;
        else
            fa.pos = topVertices_bottom;
        fa.uv = uv_full;
        fa.normal = Vector3.up;
        AddFace(nbtMesh, fa, ca);

        fa.color = Color.white;

        if (ca.blockData >= 8)
            fa.pos = bottomVertices_top;
        else
            fa.pos = bottomVertices_bottom;
        fa.uv = uv_full;
        fa.normal = Vector3.down;
        AddFace(nbtMesh, fa, ca);

        if (ca.blockData >= 8)
        {
            fa.pos = leftVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = leftVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.normal = Vector3.left;
        AddFace(nbtMesh, fa, ca);

        if (ca.blockData >= 8)
        {
            fa.pos = rightVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = rightVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.normal = Vector3.right;
        AddFace(nbtMesh, fa, ca);
    }

    public override Mesh GetItemMesh(byte data = 0)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.blockData = data;

        NBTMesh nbtMesh = new NBTMesh(256);

        FaceAttributes fa = new FaceAttributes();
        fa.faceIndex = TextureArrayManager.GetIndexByName(GetNameByData(data));
        fa.skyLight = skylight_default;
        fa.blockLight = blocklight_default;
        fa.color = Color.white;

        FillMesh(fa, ca, nbtMesh);

        nbtMesh.Refresh();

        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.blockData = blockData;

        NBTMesh nbtMesh = new NBTMesh(256);

        chunk.GetLights(pos.x - chunk.x * 16, pos.y, pos.z - chunk.z * 16, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.faceIndex = TextureArrayManager.GetIndexByName(GetNameByData(blockData));
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.color = Color.white;

        FillMesh(fa, ca, nbtMesh);

        nbtMesh.Refresh();

        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }

    public override Mesh GetBreakingEffectMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        return GetItemMesh(blockData);
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (blockData < 8 || !chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (blockData >= 8 || !chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }

    static Vector2 leftMid = new Vector2(0, 0.5f);
    static Vector2 rightMid = new Vector2(1, 0.5f);

    Vector2[] uv_full = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    Vector2[] uv_top = new Vector2[4] { leftMid, Vector2.up, Vector2.one, rightMid };
    Vector2[] uv_bot = new Vector2[4] { Vector2.zero, leftMid, rightMid, Vector2.right };

    protected static Vector3 nearMiddleLeft = new Vector3(-0.5f, 0, -0.5f);
    protected static Vector3 farMiddleLeft = new Vector3(-0.5f, 0, 0.5f);
    protected static Vector3 nearMiddleRight = new Vector3(0.5f, 0, -0.5f);
    protected static Vector3 farMiddleRight = new Vector3(0.5f, 0, 0.5f);

    protected static Vector3[] frontVertices_top = new Vector3[] { nearMiddleLeft, nearTopLeft, nearTopRight, nearMiddleRight };
    protected static Vector3[] frontVertices_bottom = new Vector3[] { nearBottomLeft, nearMiddleLeft, nearMiddleRight, nearBottomRight };

    protected static Vector3[] backVertices_top = new Vector3[] { farMiddleRight, farTopRight, farTopLeft, farMiddleLeft };
    protected static Vector3[] backVertices_bottom = new Vector3[] { farBottomRight, farMiddleRight, farMiddleLeft, farBottomLeft };

    protected static Vector3[] topVertices_top = new Vector3[] { farTopRight, nearTopRight, nearTopLeft, farTopLeft };
    protected static Vector3[] topVertices_bottom = new Vector3[] { farMiddleRight, nearMiddleRight, nearMiddleLeft, farMiddleLeft };

    protected static Vector3[] bottomVertices_top = new Vector3[] { nearMiddleRight, farMiddleRight, farMiddleLeft, nearMiddleLeft };
    protected static Vector3[] bottomVertices_bottom = new Vector3[] { nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft };

    protected static Vector3[] leftVertices_top = new Vector3[] { farMiddleLeft, farTopLeft, nearTopLeft, nearMiddleLeft };
    protected static Vector3[] leftVertices_bottom = new Vector3[] { farBottomLeft, farMiddleLeft, nearMiddleLeft, nearBottomLeft };

    protected static Vector3[] rightVertices_top = new Vector3[] { nearMiddleRight, nearTopRight, farTopRight, farMiddleRight };
    protected static Vector3[] rightVertices_bottom = new Vector3[] { nearBottomRight, nearMiddleRight, farMiddleRight, farBottomRight };


    public override void OnAddBlock(RaycastHit hit)
    {
        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);
        byte targetType = NBTHelper.GetBlockByte(pos);

        bool canAdd = false;
        byte type = NBTGeneratorManager.id2type[id];
        byte data = (byte)InventorySystem.items[ItemSelectPanel.curIndex].damage;
        Vector3Int finalPos = pos;

        if (targetType == 126)
        {
            canAdd = true;
            type = 125;
        }

        if (WireFrameHelper.generator is NBTWoodenSlab &&
            (WireFrameHelper.data < 8 && hit.normal == Vector3.up) ||
            (WireFrameHelper.data >= 8 && hit.normal == Vector3.down))
        {
            canAdd = true;
            type = 125;
            finalPos = WireFrameHelper.pos;
        }

        if (targetType == 0 && CanAddBlock(pos))
        {
            canAdd = true;
            if (WireFrameHelper.hitPos.y - pos.y > 0)
            {
                data += 8;
            }
        }

        if (canAdd)
        {
            PlayerController.instance.PlayHandAnimation();

            NBTHelper.SetBlockData(finalPos, type, data);

            InventorySystem.DecrementCurrent();
            ItemSelectPanel.instance.RefreshUI();
        }
    }

    public override void RenderWireframe(byte blockData)
    {
        float top, bottom, left, right, front, back;
        if (blockData >= 8) // top
        {
            top = 0.501f;
            bottom = -0.001f;
            left = -0.501f;
            right = 0.501f;
            front = 0.501f;
            back = -0.501f;
        }
        else // bottom
        {
            top = 0.001f;
            bottom = -0.501f;
            left = -0.501f;
            right = 0.501f;
            front = 0.501f;
            back = -0.501f;
        }

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }

    protected override FaceAttributes GetFrontFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z - 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = frontVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = frontVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
        fa.color = GetFrontTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.forward;

        return fa;
    }
    protected override FaceAttributes GetBackFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z + 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = backVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = backVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
        fa.color = GetBackTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.back;

        return fa;
    }
    protected override FaceAttributes GetTopFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y + 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
            fa.pos = topVertices_top;
        else
            fa.pos = topVertices_bottom;
        fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
        fa.color = GetTopTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.up;
        fa.uv = uv_full;

        return fa;
    }

    protected override FaceAttributes GetBottomFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y - 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
            fa.pos = bottomVertices_top;
        else
            fa.pos = bottomVertices_bottom;
        fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
        fa.color = GetBottomTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.down;
        fa.uv = uv_full;

        return fa;
    }
    protected override FaceAttributes GetLeftFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x - 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = leftVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = leftVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
        fa.color = GetLeftTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.left;

        return fa;
    }
    protected override FaceAttributes GetRightFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x + 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = rightVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = rightVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
        fa.color = GetRightTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.right;

        return fa;
    }
}
