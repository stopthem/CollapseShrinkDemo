// https://frarees.github.io/default-gist-license

using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class MinMaxSliderAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;
    public readonly float floatFieldWidthMultiplier;
    public const bool DataFields = true;
    public const bool FlexibleFields = true;
    public const bool Bound = true;
    public const bool Round = false;

    public MinMaxSliderAttribute() : this(0, 1)
    {
    }

    public MinMaxSliderAttribute(float min, float max, float fieldWidthMultiplier = 1.75f)
    {
        this.min = min;
        this.max = max;
        floatFieldWidthMultiplier = fieldWidthMultiplier;
    }
}