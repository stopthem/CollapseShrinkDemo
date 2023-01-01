using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutOutMaskUI : Image
{
    public override Material materialForRendering
    {
        get
        {
            var mat = new Material(base.materialForRendering);
            mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return mat;
        }
    }
}