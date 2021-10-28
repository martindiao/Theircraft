using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBirchStairs : NBTStairs
{
    public override string name { get { return "Birch Wood Stairs"; } }
    public override string id { get { return "minecraft:birch_stairs"; } }

    public override string stairsName { get { return "planks_birch"; } }

    public override float hardness => 2;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }
    
    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_birch"; }
}
