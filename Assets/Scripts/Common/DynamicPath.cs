using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate;
using CanTemplate.Extensions;
using CanTemplate.Pooling;
using CanTemplate.Utilities;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class DynamicPath : MonoBehaviour
{
    private SplineComputer _splineComputer;
    private Poolable _poolable;

    private void Awake()
    {
        _splineComputer = GetComponent<SplineComputer>();
    }

    private void Start()
    {
        _poolable = GetComponent<Poolable>();
    }

    /// <summary>
    /// Sets given obj as target of the tween for filtering operations
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="pathTransforms"></param>
    /// <param name="followingOptions"></param>
    /// <returns></returns>
    public Tween CreatePathAndFollow(Transform obj, Transform[] pathTransforms, TweenOptions followingOptions)
    {
        var startPos = new SplinePoint(obj.position);

        var t = DOTween.To(x =>
            {
                var points = new List<SplinePoint> { new(startPos) };
                points.AddRange(pathTransforms.Select(pathPoint => new SplinePoint(pathPoint.position)));

                _splineComputer.SetPoints(points.ToArray());

                var sample = new SplineSample();
                _splineComputer.Evaluate(percent: x, ref sample);

                obj.position = sample.position != Vector3.zero ? sample.position : obj.position;
                // obj.rotation = sample.rotation;
            }, 0, 1, followingOptions.duration)
            .SetEase(followingOptions.easeSelection)
            .SetSpeedBased()
            .SetTarget(obj)
            .OnComplete(() => WaitUtilities.WaitForAFrame(() => _poolable.ReturnToPool()));
        return t;
    }

    /// <summary>
    /// You MUST inherit <see cref="IVector3DynamicPathUser"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="followingOptions"></param>
    /// <returns></returns>
    public Tween CreatePathAndFollow(Transform obj, TweenOptions followingOptions)
    {
        var startPos = new SplinePoint(obj.position);

        if (!obj.TryGetComponent(out IVector3DynamicPathUser dynamicPathUser))
        {
            Debug.Log($"{obj} must inherit IDynamicPathUser");
            return null;
        }

        var t = DOTween.To(x =>
            {
                var points = new List<SplinePoint> { new(startPos) };
                points.AddRange(dynamicPathUser.GetPoints(obj).Select(pathPoint => new SplinePoint(pathPoint)));

                _splineComputer.SetPoints(points.ToArray());

                var sample = new SplineSample();
                _splineComputer.Evaluate(percent: x, ref sample);

                obj.position = sample.position != Vector3.zero ? sample.position : obj.position;
                // obj.rotation = sample.rotation;
            }, 0, 1, followingOptions.duration)
            .SetEase(followingOptions.easeSelection)
            .SetSpeedBased()
            .SetTarget(obj)
            .OnComplete(() => WaitUtilities.WaitForAFrame(() => _poolable.ReturnToPool()));
        return t;
    }

    public Tween JumpAndFollow(Transform obj, FTweenOptions followingOptions, Transform target, float jumpPointNormalized = .5f)
    {
        var startPos = new SplinePoint(obj.position);

        var t = DOTween.To(x =>
            {
                if (target.position == Vector3.zero) return;
                
                var points = new List<SplinePoint>
                {
                    new(startPos),
                    new(Vector3.Lerp(startPos.position, target.position, jumpPointNormalized).WithY(followingOptions.target, true)),
                    new(target.position)
                };

                _splineComputer.SetPoints(points.ToArray());

                var sample = new SplineSample();
                _splineComputer.Evaluate(percent: x, ref sample);

                obj.position = sample.position != Vector3.zero ? sample.position : obj.position;
                // obj.rotation = sample.rotation;
            }, 0, 1, followingOptions.duration)
            .SetEase(followingOptions.easeSelection)
            .SetUpdate(UpdateType.Late)
            .SetTarget(obj)
            .OnComplete(() => WaitUtilities.WaitForAFrame(() => _poolable.ReturnToPool()));

        return t;
    }
}

public interface IVector3DynamicPathUser
{
    public Vector3[] GetPoints(Transform target);
}