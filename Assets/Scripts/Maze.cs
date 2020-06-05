using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Maze : MonoBehaviour
{
    MazeData maze;
    MazeMesh mesh;
    Dictionary<int, List<int>> dict;
    MeshFilter meshFilter;
    public int x;
    public int y;
    public int z;
    public int seed;

    public int scale = 1;
    void Start()
    {
        maze = new MazeData(seed,x,y,z);        
    }

    private void OnValidate()
    {
        maze = new MazeData(seed, x, y, z);
    }

    private void OnDrawGizmos()
    {
        Vector3 lastPoint = Vector3.zero;
        Color defaultColor = Gizmos.color;
        for (int j = 0; j < maze.Used.Count; j++)
        {
            int index = maze.Used[j];
            if (index < 0)
            {
                lastPoint = -Vector3.one;
                continue;
            }
            

            int _x, _y, _z;
            maze.TransIndex(index, out _x, out _y, out _z);
            Vector3 center = new Vector3(_x, _y, _z) * scale;
                        
            if (index == 0)
            {
                Gizmos.color = Color.red;
            }
            else if (index == maze.Size - 1)
            {
                Gizmos.color = Color.green;
            }           
            else
            {
                Gizmos.color = defaultColor;
            }
            Gizmos.DrawSphere(center, 0.1f);
            if (lastPoint == -Vector3.one)
            {
                lastPoint = center;
                continue;
            }

            Vector3 half = (lastPoint + center) * 0.5f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(lastPoint, half);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(half, center);
            Gizmos.color = defaultColor;
            lastPoint = center;
        }

      
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Build Mesh"))
        {
            BuildMesh();
            BuildTree();
        }
    }

    void BuildMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new MazeMesh();
        mesh.BuildMesh(maze,scale);

        meshFilter.mesh = mesh.mesh;
    }

    void BuildTree()
    {
        dict = new Dictionary<int, List<int>>();
        for (int i = 0; i < maze.Used.Count - 1; i++)
        {
            int cur = maze.Used[i];
            int next = maze.Used[i + 1];
            if (cur < 0 || next < 0) continue;
            if (!dict.TryGetValue(cur, out List<int> list))
            {
                list = new List<int>();
                dict.Add(cur,list);
            }
            
            list.Add(next);
            
        }
       
    }
}
