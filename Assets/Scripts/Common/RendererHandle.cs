using System.Collections;
using System.Collections.Generic;
using CanTemplate.Camera;
using CanTemplate.Extensions;
using UnityEngine;

public class RendererHandle : MonoBehaviour
{
    [SerializeField] private bool checkCollider;

    private Renderer _renderer;
    private Bounds _bounds;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();

        _bounds = checkCollider ? GetComponent<Collider>().bounds : _renderer.bounds;
    }

    private void LateUpdate()
    {
        _renderer.enabled = _bounds.IsVisibleFrom();
    }
}