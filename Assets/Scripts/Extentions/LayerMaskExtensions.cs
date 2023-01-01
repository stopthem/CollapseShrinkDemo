using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class LayerMaskExtensions
    {
        public static LayerMask AddLayerToLayerMask(this LayerMask layerMask, int layer)
        {
            return layerMask | 1 << layer;
        }

        public static LayerMask RemoveLayer(this LayerMask layerMask, int layer)
        {
            return layerMask & ~(1 << layer);
        }
    }
}