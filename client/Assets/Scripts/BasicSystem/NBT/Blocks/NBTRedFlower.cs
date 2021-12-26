﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedFlower : NBTPlant
{
    public override string name { get { return "Red Flower"; } }
    public override string id { get { return "minecraft:red_flower"; } }
    protected override int size => 4;
    protected override int height => 12;

    string GetNameByData(int data)
    {
        switch (data)
        {
            case 0:
                return "flower_rose";
            case 3:
                return "flower_houstonia";
            case 4:
                return "flower_tulip_red";
            case 5:
                return "flower_tulip_orange";
            case 6:
                return "flower_tulip_white";
            case 7:
                return "flower_tulip_pink";
            case 8:
                return "flower_oxeye_daisy";
        }
        throw new System.Exception("no index, data=" + data);
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName(GetNameByData(data));
    }

    public override string GetBreakEffectTexture(byte data)
    {
        return GetNameByData(data);
    }

    public override string GetIconPathByData(short data)
    {
        return GetNameByData(data);
    }

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Rose";
            case 3:
                return "Houstonia";
            case 4:
                return "Red Tulip";
            case 5:
                return "Orange Tulip";
            case 6:
                return "Houstonia";
            case 8:
                return "Oxeye Daisy";
        }
        throw new System.Exception("no name, data=" + data);
    }

    public override void RenderWireframe(byte blockData)
    {
        float top = 0.1875f;
        float bottom = -0.501f;
        float left = -0.25f;
        float right = 0.25f;
        float front = 0.25f;
        float back = -0.25f;

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}