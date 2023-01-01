using System.Linq;
using CanTemplate.Camera;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class RendererExtensions
    {
        public static bool IsVisibleFrom(this Bounds bounds, UnityEngine.Camera camera = null)
        {
            var cam = camera ? camera : CinemachineManager.MainCam;

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }

        public static int GetBlendShapeIndexByName(this SkinnedMeshRenderer skinnedMeshRenderer, string name)
        {
            var m = skinnedMeshRenderer.sharedMesh;
            return m.GetBlendShapeIndex(name);
        }

        ///<summary>Copies given skinned mesh renderer to target.</summary>
        public static void CopySkinnedMesh(this SkinnedMeshRenderer thisMesh, SkinnedMeshRenderer otherMesh, bool copyBlendShapes = true, bool copyRootBone = true)
        {
            thisMesh.sharedMesh = otherMesh.sharedMesh;
            if (copyRootBone) thisMesh.rootBone = otherMesh.rootBone;
            thisMesh.localBounds = new Bounds(otherMesh.bounds.center, otherMesh.bounds.size);
            thisMesh.bones = otherMesh.bones;

            if (!copyBlendShapes) return;

            for (int i = 0; i < otherMesh.sharedMesh.blendShapeCount; i++)
            {
                if (i == thisMesh.sharedMesh.blendShapeCount) break;
                thisMesh.SetBlendShapeWeight(i, otherMesh.GetBlendShapeWeight(i));
            }
        }

        public static void SetBlendShapeWeight(this SkinnedMeshRenderer renderer, string keyName, float value)
        {
            var index = renderer.GetBlendShapeIndexByName(keyName);
            if (index != -1) renderer.SetBlendShapeWeight(index, value);
        }

        public static float GetBlendShapeWeight(this SkinnedMeshRenderer renderer, string keyName)
            => renderer.GetBlendShapeWeight(renderer.GetBlendShapeIndexByName(keyName));
    }
}