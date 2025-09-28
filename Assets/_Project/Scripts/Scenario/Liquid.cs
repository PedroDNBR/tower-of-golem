using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class Liquid : MonoBehaviour
    {
        Vector3 waveSource1 = new Vector3(2f, 0f, 2f);
        public float waveFrequency = 0.53f;
        public float waveHeight = 0.48f;
        public float waveLength = 0.71f;
        public bool edgeBlend = true;
        public bool forceFlatShading = true;

        Mesh mesh;

        private Camera cam;

        public Camera PlayerCamera
        {
            get => cam;
            set
            {
                cam = value;
                StartLiquidSimulation();
            }
        }

        Vector3[] verticies;

        private void OnValidate()
        {
            StartLiquidSimulation();
        }

        private void Start()
        {
            Invoke(nameof(FindPlayerCamera), 0.5f);

            StartLiquidSimulation();
        }

        private void Update()
        {
            CalculateWave();
            setEdgeBlend();
        }

        private void StartLiquidSimulation()
        {
            StartLowPolyLiquid();
            CalculateWave();
            setEdgeBlend();
        }


        private void FindPlayerCamera()
        {
            List<Camera> camerasAvailable = new List<Camera>(Camera.allCameras);
            PlayerCamera = camerasAvailable.Find(x => x.enabled);
        }

        private void StartLowPolyLiquid()
        {
            if (cam != null)
                cam.depthTextureMode |= DepthTextureMode.Depth;
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MakeMesh(meshFilter);
        }

        MeshFilter MakeMesh(MeshFilter mf)
        {
            mesh = mf.sharedMesh;
            Vector3[] oldVertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3[] newVerticies = new Vector3[triangles.Length];
            for (int i = 0; i < triangles.Length; i++)
            {
                newVerticies[i] = oldVertices[triangles[i]];
                triangles[i] = i;
            }
            mesh.vertices = newVerticies;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            verticies = mesh.vertices;
            return mf;
        }

        void setEdgeBlend()
        {
            if(!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
                edgeBlend = false;

            if (edgeBlend)
            {
                Shader.EnableKeyword("WATER_EDGEBLEND_ON");
                if (cam != null && cam)
                    cam.depthTextureMode |= DepthTextureMode.Depth;
            }
            else
            {
                Shader.DisableKeyword("WATER_EDGEBLEND_ON");
            }
            Shader.EnableKeyword("_EMISSION");
        }

        void CalculateWave()
        {
            for (int i = 0; i < verticies.Length; i++) {
                Vector3 v = verticies [i];
                v.y = 0.0f;
                float dist = Vector3.Distance (v, waveSource1);
                dist = (dist % waveLength) / waveLength;
                v.y = waveHeight * Mathf.Sin (Time.time * Mathf.PI * 2.0f * waveFrequency
                + (Mathf.PI * 2.0f * dist));
                verticies [i] = v;
            }
            mesh.vertices = verticies;
            mesh.RecalculateNormals (); 
            mesh.MarkDynamic ();
    
            GetComponent<MeshFilter> ().mesh = mesh;
        }
    }
}
