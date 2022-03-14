using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
	//inspector
	[Header("Shader")]
	[SerializeField]
	Shader outlineShader;

	[Header("Setting")]
	[Tooltip("Number of pixels between samples that are tested for an edge. When this value is 1, tested samples are adjacent.")]
	public int outlineScale = 1;
	public Color outlineColor = Color.black;
	[Tooltip("Difference between depth values, scaled by the current depth, required to draw an edge.")]
	public float depthThreshold = 1.5f;
	[Range(0, 1), Tooltip("The value at which the dot product between the surface normal and the view direction will affect " +
		"the depth threshold. This ensures that surfaces at right angles to the camera require a larger depth threshold to draw " +
		"an edge, avoiding edges being drawn along slopes.")]
	public float depthNormalThreshold = 0.5f;
	[Tooltip("Scale the strength of how much the depthNormalThreshold affects the depth threshold.")]
	public float depthNormalThresholdScale = 7;
	[Range(0, 1), Tooltip("Larger values will require the difference between normals to be greater to draw an edge.")]
	public float normalThreshold = 0.4f;
	//--

	public Material OutlineMaterial { get; private set; }
	public Camera Camera { get; private set; }

	private void Start()
	{
		Camera = GetComponent<Camera>();
		Camera.depthTextureMode = Camera.depthTextureMode | DepthTextureMode.Depth;
		if ((OutlineMaterial = new Material(outlineShader)) != null)
			OutlineMaterial.hideFlags = HideFlags.DontSave;
	}
	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (OutlineMaterial == null)
		{
			Graphics.Blit(src, dest);
			return;
		}
		OutlineMaterial.SetFloat("_Scale", outlineScale);
		OutlineMaterial.SetColor("_Color", outlineColor);
		OutlineMaterial.SetFloat("_DepthThreshold", depthThreshold);
		OutlineMaterial.SetFloat("_DepthNormalThreshold", depthNormalThreshold);
		OutlineMaterial.SetFloat("_DepthNormalThresholdScale", depthNormalThresholdScale);
		OutlineMaterial.SetFloat("_NormalThreshold", normalThreshold);
		OutlineMaterial.SetMatrix("_ClipToView", GL.GetGPUProjectionMatrix(Camera.projectionMatrix, true).inverse);

		Graphics.Blit(src, dest, OutlineMaterial);
	}
}
