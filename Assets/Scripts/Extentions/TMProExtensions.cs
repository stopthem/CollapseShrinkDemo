using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class TMProExtensions
    {
        public static void UpdateAll(this TMP_Text tmPro)
        {
            tmPro.UpdateMeshPadding();
            tmPro.UpdateVertexData();
            tmPro.ForceMeshUpdate();
        }
    }
}