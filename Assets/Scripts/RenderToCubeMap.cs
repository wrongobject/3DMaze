using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderToCubeMap : MonoBehaviour
{
    public Camera camera;
    public Cubemap cubemap;

    public int width;

    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (Check())
        {
            RenderGUI();
        }
       
        GUILayout.Space(10);
        GUILayout.EndHorizontal();
    }

    private bool Check()
    {
        bool result = true;
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        if (camera == null)
        {
            GUILayout.Label("null camera");
            result = false;
        }
        if (cubemap == null)
        {
            GUILayout.Label("null cubemap");
            result = false;
        }
        GUILayout.Space(10);
        GUILayout.EndVertical();
        return result;
    }

    private void RenderGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        if (GUILayout.Button("开始渲染"))
        {
            bool result = camera.RenderToCubemap(cubemap);
        }
        GUILayout.Space(10);
        GUILayout.EndVertical();
    }
}
