using System.Collections;
using System.Collections.Generic;
using CanTemplate.Extensions;
using UnityEngine;

public class RendererHandle : MonoBehaviour
{
    private Renderer myRenderer;

    private void Awake() => myRenderer = GetComponent<Renderer>();


    private void LateUpdate()
    {
        myRenderer.enabled = myRenderer.IsVisibleFrom(Camera.main);
    }
}
