using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour {

	static int baseColorId = Shader.PropertyToID("_BaseColor"),
			   cutoffId = Shader.PropertyToID("_Cutoff"),
			   metallicId = Shader.PropertyToID("_Metallic"),
			   smoothnessId = Shader.PropertyToID("_Smoothness");
	

	static MaterialPropertyBlock block; //这个是用来对材质进行复用的，可以很多物体用一个材质，但材质内的参数不一样

	[SerializeField]
	Color baseColor = Color.white;

	[SerializeField, Range(0f, 1f)]
	float alphaCutoff = 0.5f, metallic = 0f, smoothness = 0.5f;

	void Awake () {
		OnValidate();//组件数值改变的时候会执行这个函数
	}

	void OnValidate () {
		if (block == null) {
			block = new MaterialPropertyBlock();
		}
		block.SetColor(baseColorId, baseColor);
		block.SetFloat(cutoffId, alphaCutoff);
		block.SetFloat(metallicId, metallic);
		block.SetFloat(smoothnessId, smoothness);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}