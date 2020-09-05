using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting{
    const string bufferName = "Lighting";

    CommandBuffer buffer = new CommandBuffer { name = bufferName };

    const int maxDirLightCount = 4;

    static int dirLitghCountId = Shader.PropertyToID("_DirectionalLightCount"),
               dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
               dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirecitons");


    static Vector4[] dirLightColors = new Vector4[maxDirLightCount],
                     dirLightDirections = new Vector4[maxDirLightCount];

    CullingResults cullingResults;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        //SetUpDirectionalLight(); //传递光照信息到GPU
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights; //返回当前的所有可见光
        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            if(visibleLight.lightType == LightType.Directional)
            {
                SetUpDirectionalLight(dirLightCount++, ref visibleLight);
                if (dirLightCount >= maxDirLightCount) //现在最多只支持四个定向灯
                {
                    break;
                }
            }
        }

        buffer.SetGlobalInt(dirLitghCountId, dirLightCount);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);

    }

    //初始化几个方向光的属性
    void SetUpDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        //    Light light = RenderSettings.sun; //获得当前场景中的主光源
        //    buffer.SetGlobalVector(dirLightColorId, light.color.linear * light.intensity); //用线性空间的颜色乘上光源的亮度
        //    buffer.SetGlobalVector(dirLightDirectionId, -light.transform.forward); //注意方向要取反，还有就是这个forward，这里代表取的是Z轴的方向
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2); //矩阵的第三列，表示forward方向
    }
}
