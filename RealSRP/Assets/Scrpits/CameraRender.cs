using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public partial class CameraRender
{
    ScriptableRenderContext context;
    Camera camera;
    static ShaderTagId shaderTagId = new ShaderTagId("SRPUitil");
    CullingResults CullingResults;
    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer { name = bufferName };
    CullingResults cullingResults;

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context; //上下文会延迟实际渲染，除了某些专用的任务发出（绘制天空盒），其他的都需要commbuffer来间接发出
        this.camera = camera;

        PrepareForSceneWindow(); //将UI放到Scene窗口渲染
        PrepareBuffer();

        //准备渲染Scene窗口
        if (!Cull())
        {
            return;
        }

        SetUp(); //初始化参数的函数
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        DrawGizmos(); //绘制Gizmos
        Submit(); //提交渲染指令的函数

    }

    void DrawVisibleGeometry()
    {
        var sortingSettings = new SortingSettings(camera); //设置是正交还是基于深度的排序
        var drawSettings = new DrawingSettings(shaderTagId, sortingSettings); //绘制设置
        var fillterSettings = new FilteringSettings(RenderQueueRange.opaque); //滤波设置，先绘制不透明物体

        context.DrawRenderers(cullingResults, ref drawSettings, ref fillterSettings);

        context.DrawSkybox(camera); //再绘制天空盒

        //最后绘制不透明物体
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawSettings.sortingSettings = sortingSettings;
        fillterSettings.renderQueueRange = RenderQueueRange.transparent;
    }
    void SetUp()
    {
        context.SetupCameraProperties(camera); //设置矩阵和一些属性
        CameraClearFlags flags = camera.clearFlags; //CameraClearFlags是个枚举类型，分别是skybox,Color,Depth,Nothing 
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color, flags == CameraClearFlags.Color ? camera.backgroundColor : Color.clear); //清除深度，颜色，以及用于清除的颜色

        buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }

    void Submit()
    {
        buffer.EndSample(SampleName);
        ExecuteBuffer();

        context.Submit();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer); //执行commandbuffer
        buffer.Clear(); //清除commandbuffer
    }

    bool Cull()
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }


}
