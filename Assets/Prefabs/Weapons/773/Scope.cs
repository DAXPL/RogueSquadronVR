using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    [SerializeField] private Camera scopeCamera;
    [SerializeField] private Renderer scopeVisor;
    private RenderTexture drawingRenderTexture;
    private Material drawingMaterial;
    private const int RESOLUTION = 1024;

    private bool shouldDraw = false;

    void Start()
    {
        drawingMaterial = scopeVisor.material;

        drawingRenderTexture = new RenderTexture(RESOLUTION , RESOLUTION, 8, RenderTextureFormat.ARGB32);

        drawingMaterial.SetTexture("_BaseMap", drawingRenderTexture);

        scopeCamera.enabled = false;
        scopeCamera.targetTexture = drawingRenderTexture;
    }

    private void Update()
    {
        if(shouldDraw == false) return;
        scopeCamera.Render();
    }

    public void ToggleDraw(bool newState)
    {
        shouldDraw = newState;
    }
    [ContextMenu("Toggle draw flag")]
    public void ToggleDraw()
    {
        shouldDraw = !shouldDraw;
    }
}
