using System;
using UnityEngine;
using System.Collections.Generic;

public class Move : MonoBehaviour
{
    const float SPEED_SCALE = 0.01f;

    public Camera camera;
    public float speed = 1;
    Dictionary<int, List<int>> dict;
    Vector3 backPos, frontPos;
    int current,back,front;

    MazeData _maze;
    int _scale;

    public void BuildTree(MazeData maze,int scale)
    {
        _maze = maze;
        _scale = scale;
        dict = new Dictionary<int, List<int>>();
        for (int i = 0; i < maze.Used.Count - 1; i++)
        {
            int cur = maze.Used[i];
            int next = maze.Used[i + 1];
            if (cur < 0 || next < 0) continue;
            if (!dict.TryGetValue(cur, out List<int> list))
            {
                list = new List<int>();
                dict.Add(cur, list);
            }
            if(!list.Contains(next))
                list.Add(next);
            if (!dict.TryGetValue(next, out list))
            {
                list = new List<int>();
                dict.Add(next, list);
            }
            if(!list.Contains(cur))
                list.Add(cur);
        }

    }
    public void Init()
    {
        current = back = front = 0;
        
        int x, y, z;
        _maze.TransIndex(0, out x, out y, out z);

        transform.position = new Vector3(x,y,z);
    }
    private void Update()
    {
        if (dict == null) return;
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir = Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir = Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir = Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir = Vector3.right;
        }
        if (dir == Vector3.zero) return;
        dir = camera.transform.rotation * dir;
        StepMove(dir);
    }

    void StepMove(Vector3 dir)
    {
        int x, y, z;
        Vector3 moveDir = Vector3.zero;
        if ( back >= 0 && CheckNear(backPos))
        {
            current = back;

            if (dict.TryGetValue(current, out List<int> list))
            {
                foreach (var item in list)
                {
                    _maze.TransIndex(item, out x, out y, out z);
                    Vector3 pos = new Vector3(x, y, z) * _scale;
                    Vector3 posDir = (pos - transform.position).normalized;
                    float dot = Vector3.Dot(posDir, dir);
                    if (dot > 0.7f)
                    {
                        frontPos = backPos;
                        backPos = pos;
                        front = back;
                        back = item;                        
                        moveDir = posDir;
                        break;
                    }
                }

            }
            else
            {
                back = -1;
            }
        }
        if (front >= 0 && CheckNear(frontPos))
        {
            current = front;

            if (dict.TryGetValue(current, out List<int> list))
            {
                foreach (var item in list)
                {
                    _maze.TransIndex(item, out x, out y, out z);
                    Vector3 pos = new Vector3(x, y, z) * _scale;
                    Vector3 posDir = (pos - transform.position).normalized;
                    float dot = Vector3.Dot(posDir, dir);
                    if (dot > 0.7f)
                    {
                        backPos = frontPos;
                        frontPos = pos;
                        back = front;
                        front = item;
                        
                        moveDir = posDir;
                        break;
                    }
                }
            }
        }
        
        {
            Vector3 frontDir = frontPos - transform.position;
            Vector3 backDir = backPos - transform.position;
            frontDir.Normalize();
            backDir.Normalize();
            if (Vector3.Dot(frontDir, dir) > 0.7f)
            {
               
                transform.position += frontDir * speed * SPEED_SCALE;
            }
            else if (Vector3.Dot(backDir, dir) > 0.7f)
            {
                
                transform.position += backDir * speed * SPEED_SCALE;
            }
        }
       

    }

    bool CheckNear(Vector3 point)
    {
        Vector3 pos = transform.position;
        Vector3 delta = point - pos;
        if (delta.x < 0.1f && delta.y < 0.1f && delta.z < 0.1f)
            return true;
        return false;
    }
}

