using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline {

	CameraRenderer renderer = new CameraRenderer();

	bool useDynamicBatching, useGPUInstancing;

	//构造函数，在创建自定义管线的时候调用，可以传入一些自己定义的变量
	public CustomRenderPipeline (
		bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher
	) {
		this.useDynamicBatching = useDynamicBatching;
		this.useGPUInstancing = useGPUInstancing;
		GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher; //启用SRP批处理
	}

	protected override void Render (
		ScriptableRenderContext context, Camera[] cameras
	) {
		foreach (Camera camera in cameras) {
			renderer.Render(
				context, camera, useDynamicBatching, useGPUInstancing
			);
		}
	}
}