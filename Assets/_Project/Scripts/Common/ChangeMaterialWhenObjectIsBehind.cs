using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TW
{
    public class ChangeMaterialWhenObjectIsBehind : MonoBehaviour
    {
        public MeshRenderer meshRenderer;

        [SerializeField]
        List<ObjectMaterials> materials = new List<ObjectMaterials>();

        List<Material> newList = new List<Material>();

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            newList = meshRenderer.materials.ToList();
        }

        public void ChangeToNormalMaterial()
        {
            foreach (var material in materials)
            {
                newList[material.materialIndex] = material.normalMaterial;
                meshRenderer.SetMaterials(newList);
            }
        }

        public void ChangeToTransparentMaterial()
        {
            Debug.Log("Change To Transparent");
            foreach (var material in materials)
            {
                newList[material.materialIndex] = material.transparentMaterial;
                meshRenderer.SetMaterials(newList);
            }
        }
    }

    [Serializable]
    public class ObjectMaterials
    {
        public int materialIndex = 0;
        public Material normalMaterial;
        public Material transparentMaterial;
    }
}