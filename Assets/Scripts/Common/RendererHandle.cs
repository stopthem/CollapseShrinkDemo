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
        if (myRenderer.IsVisibleFrom(Camera.main))
            myRenderer.enabled = true;
        else
            myRenderer.enabled = false;
    }
}
