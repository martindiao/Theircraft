using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    [HideInInspector] public float tick;

    public bool elapse = true;

    public Gradient lightColor;
    public Gradient skyTopColor;
    public Gradient skyBottomColor;
    public float skyHeight = 1000;
    public float skyTransition = 1000;

    public static TimeOfDay instance;

    private void Start()
    {
        instance = this;

        TagNodeCompound levelDat = NBTHelper.GetLevelDat();
        TagNodeLong dayTimeNode = levelDat["DayTime"] as TagNodeLong;
        int dayTime = (int)dayTimeNode.Data;
        tick = dayTime;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Update()
    {
        if (elapse)
        {
            tick += Time.deltaTime * 20;

            if (tick > 24000)
                tick = 0;
        }
        float time01 = tick / 24000;

        float dayNight01 = 0;
        if (tick < 6000)
        {
            dayNight01 = Mathf.Lerp(0.5f, 0, tick / 6000);
        }
        else if (tick >= 6000 && tick < 18000)
        {
            dayNight01 = (tick - 6000) / 12000;
        }
        else if (tick >= 18000)
        {
            dayNight01 = Mathf.Lerp(1, 0.5f, (tick - 18000) / 6000);
        }

        Shader.SetGlobalFloat("_DayNight01", dayNight01);
        Shader.SetGlobalColor("_SkyLightColor", lightColor.Evaluate(time01));
        Shader.SetGlobalColor("_SkyTopColor", skyTopColor.Evaluate(time01).linear);
        Shader.SetGlobalColor("_SkyBottomColor", skyBottomColor.Evaluate(time01).linear);
        Shader.SetGlobalFloat("_SkyHeight", skyHeight);
        Shader.SetGlobalFloat("_SkyTransition", skyTransition);
    }
}
