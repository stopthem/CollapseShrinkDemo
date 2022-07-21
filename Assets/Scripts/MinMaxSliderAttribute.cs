// https://frarees.github.io/default-gist-license

using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class MinMaxSliderAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;
    public readonly float floatFieldWidthMultiplier;
    public readonly bool dataFields = true;
    public readonly bool flexibleFields = true;
    public readonly bool bound = true;
    public readonly bool round = true;

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