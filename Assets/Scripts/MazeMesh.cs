using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMesh
{
    public Mesh mesh;

    List<Vector3> _vertex;  //顶点信息
    List<int> _triangle;    //三角形
    List<Vector2> _uv;  

    MazeData _maze;
    int _scale;             //缩放
    public void BuildMesh(MazeData maze,int scale)
    {
        _scale = scale;
        _maze = maze;
        mesh = new Mesh();
        _vertex = new List<Vector3>();
        _triangle = new List<int>();
        _uv = new List<Vector2>();
        BuildFullMesh();
        CullTriangle();
        
        mesh.SetVertices(_vertex);
        mesh.SetTriangles(_triangle, 0);
        mesh.SetUVs(0,_uv);
    }
    /// <summary>
    /// 建立立方体mesh
    /// </summary>
    void BuildFullMesh()
    {
        int x, y, z;
        Vector3 offset = -Vector3.one * 0.5f * _scale;
        int _x = _maze.MaxX + 1;
        int _y = _maze.MaxY + 1; 
        int _z = _maze.MaxZ + 1;
        int _yz = _y * _z;
        int size = _x * _y * _z;
        for (int i = 0; i < size; i++)
        {
            x = i / _yz;
            y = i % _yz / _z;
            z = i % _z;
           
            Vector3 center = new Vector3(x,y,z) * _scale;
            _vertex.Add(center + offset);
            _uv.Add(new Vector2((float)x / (_x-1),(float)z/(_z-1)));
        }

        for (int i = 0; i < _maze.Size; i++)
        {
            _maze.TransIndex(i, out x, out y, out z);
            //l:left, r:right,
            //d:down, u:up
            //b:back, f:front
            int lbd = x * _y * _z + y * _z + z;
            int rbd = lbd + 1 * _y * _z;
            int lbu = lbd + 1 * _z;
            int rbu = lbd + 1 * _y * _z + 1 * _z;
            //back face
            if (i != 0)
            {//entry
                _triangle.Add(lbd);
                _triangle.Add(lbu);
                _triangle.Add(rbd);

                _triangle.Add(rbd);
                _triangle.Add(lbu);
                _triangle.Add(rbu);
            }
            int lfd = lbd + 1;
            int lfu = lbd + 1 + _z;
            //left face
            _triangle.Add(lbd);
            _triangle.Add(lfd);
            _triangle.Add(lfu);

            _triangle.Add(lbd);
            _triangle.Add(lfu);
            _triangle.Add(lbu);

            int rfd = rbd + 1;
            //down face
            _triangle.Add(lbd);            
            _triangle.Add(rbd);
            _triangle.Add(lfd);

            _triangle.Add(rbd);           
            _triangle.Add(rfd);
            _triangle.Add(lfd);

            int rfu = rfd + _z;
            //right face
            _triangle.Add(rbd);
            _triangle.Add(rbu);
            _triangle.Add(rfu);

            _triangle.Add(rfu);
            _triangle.Add(rfd);
            _triangle.Add(rbd);
            //front face
            if (i != _maze.Size - 1)
            {//safe out 
                _triangle.Add(lfu);
                _triangle.Add(lfd);
                _triangle.Add(rfd);

                _triangle.Add(rfd);
                _triangle.Add(rfu);
                _triangle.Add(lfu);
            }
            //up face
            _triangle.Add(lbu);
            _triangle.Add(lfu);
            _triangle.Add(rfu);

            _triangle.Add(rfu);
            _triangle.Add(rbu);
            _triangle.Add(lbu);
        }
    }
    /// <summary>
    /// 剔除掉有路径的三角形
    /// </summary>
    void CullTriangle()
    {
        for (int i = _triangle.Count - 3; i >= 0; i= i-3)
        {
            Vector3 p1 = _vertex[_triangle[i]];
            Vector3 p2 = _vertex[_triangle[i+1]];
            Vector3 p3 = _vertex[_triangle[i+2]];
            int x, y, z;
            for (int j = 0; j < _maze.Used.Count - 1; j++)
            {
                int cur = _maze.Used[j];
                int next = _maze.Used[j + 1];
                if (cur < 0 || next < 0) continue;
                _maze.TransIndex(cur, out x, out y, out z);
                Vector3 start = new Vector3(x,y,z);
                _maze.TransIndex(next, out x, out y, out z);
                Vector3 end = new Vector3(x, y, z);
                Vector3 center = (start + end) * 0.5f * _scale;

                if (CheckTriangleNearby(p1,p2,p3,center))
                {
                    _triangle.RemoveAt(i + 2);
                    _triangle.RemoveAt(i + 1);
                    _triangle.RemoveAt(i);
                   //Debug.Log("remove");
                }
            }
        }
    }
    /// <summary>
    /// 判断是否是需要剔除的三角形
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    bool CheckTriangleNearby(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 point)
    {
        Vector3 halfp1p2 = (p1 + p2) * 0.5f;
        Vector3 halfp1p3 = (p1 + p3) * 0.5f;
        Vector3 halfp2p3 = (p2 + p3) * 0.5f;

        if (ApproximatelyVector(halfp1p2, point))
        {
            return true;
        }
        if (ApproximatelyVector(halfp1p3, point))
        {
            return true;
        }
        if (ApproximatelyVector(halfp2p3, point))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 判定向量是否相等
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    bool ApproximatelyVector(Vector3 a, Vector3 b)
    {
        return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
    }
}
