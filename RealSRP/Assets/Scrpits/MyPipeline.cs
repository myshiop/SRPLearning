using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MyPipeline : RenderPipeline
{
    CameraRender renderer = new CameraRender();

    protected override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        foreach(Camera camera in cameras)
        {
            renderer.Render(renderContext, camera);
        }
    }


}
