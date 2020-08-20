using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {

	const string bufferName = "Render Camera";

	static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
	static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");

	CommandBuffer buffer = new CommandBuffer {
		name = bufferName
	};

	ScriptableRenderContext context;

	Camera camera;

	CullingResults cullingResults;

	public void Render (
		ScriptableRenderContext context, Camera camera,
		bool useDynamicBatching, bool useGPUInstancing
	) {
		this.context = context;//上下文会延迟实际渲染，除了某些专用的任务发出（绘制天空盒），其他的都需要commbuffer来间接发出
		this.camera = camera;

		PrepareBuffer();//将UI放到Scene窗口渲染
		PrepareForSceneWindow();
		//准备渲染Scene窗口
		if (!Cull()) {
			return;
		}

		Setup();//初始化参数的函数
		DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
		DrawUnsupportedShaders();
		DrawGizmos();//绘制Gizmos
		Submit();//提交渲染指令的函数
	}

	bool Cull () {
		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
			cullingResults = context.Cull(ref p);
			return true;
		}
		return false;
	}

	void Setup () {
		context.SetupCameraProperties(camera);//设置矩阵和一些属性
		CameraClearFlags flags = camera.clearFlags;//CameraClearFlags是个枚举类型，分别是skybox,Color,Depth,Nothing
		buffer.ClearRenderTarget(
			flags <= CameraClearFlags.Depth,
			flags == CameraClearFlags.Color,
			flags == CameraClearFlags.Color ?
				camera.backgroundColor.linear : Color.clear
		);//清除深度，颜色，以及用于清除的颜色
		buffer.BeginSample(SampleName);
		ExecuteBuffer();
	}

	void Submit () {
		buffer.EndSample(SampleName);
		ExecuteBuffer();
		context.Submit();
	}

	void ExecuteBuffer () {
		context.ExecuteCommandBuffer(buffer);//执行commandbuffer
		buffer.Clear();//清除commandbuffer
	}

	void DrawVisibleGeometry (bool useDynamicBatching, bool useGPUInstancing) {
		var sortingSettings = new SortingSettings(camera) {
			criteria = SortingCriteria.CommonOpaque
		};//设置是正交还是基于深度的排序
		var drawingSettings = new DrawingSettings(
			unlitShaderTagId, sortingSettings
		) {
			enableDynamicBatching = useDynamicBatching,
			enableInstancing = useGPUInstancing
		};//绘制设置
		drawingSettings.SetShaderPassName(1, litShaderTagId);
		var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);//滤波设置，先绘制不透明物体

		context.DrawRenderers(
			cullingResults, ref drawingSettings, ref filteringSettings
		);

		context.DrawSkybox(camera);//再绘制天空盒

		//最后绘制不透明物体
		sortingSettings.criteria = SortingCriteria.CommonTransparent;
		drawingSettings.sortingSettings = sortingSettings;
		filteringSettings.renderQueueRange = RenderQueueRange.transparent;

		context.DrawRenderers(
			cullingResults, ref drawingSettings, ref filteringSettings
		);
	}
}