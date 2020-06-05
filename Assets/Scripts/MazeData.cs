using System;
using UnityEngine;
using Rand = System.Random;
using System.Collections.Generic;

public class MazeData
{
    /// <summary>
    ///方向枚举
    /// </summary>
    enum EDir
    {
        Left,
        Right,
        Up,
        Down,
        Front,
        Back,
        Max,
    }
    /// <summary>
    /// 默认大小
    /// </summary>
    const int defaule_size = 10;

    int _x;
    int _y;
    int _z;
    int _seed;  //随机种子

    int _yz;
    int _start, _end, _size,_current;
    bool[] _data;
    int _invalidCheckIndex;             
    List<int> _used = new List<int>();
    List<int> _invalid = new List<int>();
    Rand _rand;
    List<EDir> _validDir = new List<EDir>();
  
    public List<int> Used { get { return _used; } }
    public int Size { get { return _size; } }
    public int MaxX { get { return _x; } }
    public int MaxY { get { return _y; } }
    public int MaxZ { get { return _z; } }
    public MazeData(int seed, int x = defaule_size, int y = defaule_size, int z = defaule_size)
    {
        _seed = seed;
        _x = x;
        _y = y;
        _z = z;
        _yz = _y * _z;
        _start = 0;

        _size = _x * _y * _z;
        _end = _size - 1;
        _data = new bool[_size];
        _rand = new Rand(_seed);
        Init();
    }

    void Init()
    {
        //添加起点
        AddPoint(0);       
        //设置当前点
        _current = 0;
        int loopCount = 0;
        while (_invalidCheckIndex < _size)
        {
            while (_current != _end && RandOnePoint(out int index))
            {
                _current = index;
                AddPoint(index);

            }
            GetValidPoint();
            //限制循环数，防止错误死循环，测试时有用
            loopCount++;
            if (loopCount >= _size)
                break;
        }
    }
    /// <summary>
    /// 获取一个起点
    /// </summary>
    void GetValidPoint()
    {
        for (int i = _invalidCheckIndex; i < _used.Count; i++)
        {
            if (_used[i] < 0) continue;
            CollectValidDir(_used[i]);
            if (_validDir.Count <= 0)
            {
                _invalidCheckIndex++;               
            }
            else
            {
                _current = _used[i];
                _used.Add(-1);
                _used.Add(_current);
            }
            break;
        }

        //for (int i = _invalidCheckIndex; i < _size; i++)
        //{
        //    if (!_data[i])
        //    {

        //        break;
        //    }
        //}
    }
    /// <summary>
    /// 随机下一个点
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    bool RandOnePoint(out int index)
    {
        index = 0;
        CollectValidDir(_current);
        if (_validDir.Count <= 0)
        {
            _invalid.Add(_current);
            return false;
        }
        int randValue = _rand.Next(0, _validDir.Count);
        switch (_validDir[randValue])
        {
            case EDir.Left:
                index = _current - 1 * _y * _z;
                break;
            case EDir.Right:
                index = _current + 1 * _y * _z;
                break;
            case EDir.Up:
                index = _current + 1 * _z;
                break;
            case EDir.Down:
                index = _current - 1 * _z;
                break;
            case EDir.Front:
                index = _current + 1;
                break;
            case EDir.Back:
                index = _current - 1;
                break;
        }
        return true;
    }
    /// <summary>
    /// 收集可以作为下一个点的方向
    /// </summary>
    /// <param name="target"></param>
    void CollectValidDir(int target)
    {
        _validDir.Clear();
        int index = 0;
        int x, y, z;
        for (int i = 0; i < (int)EDir.Max; i++)
        {
            switch (i)
            {
                case (int)EDir.Left:
                    x = target / _yz;
                    if (x - 1 < 0)
                        continue;

                    index = target - 1 * _y * _z;                   
                    break;
                case (int)EDir.Right:
                    x = target / _yz;
                    if (x + 1 >= _x)
                        continue;

                    index = target + 1 * _y * _z;                   
                    break;
                case (int)EDir.Up:
                    y = target % _yz / _z;
                    if (y + 1 >= _y)
                        continue;
                    index = target + 1 * _z;
                    break;
                case (int)EDir.Down:
                    y = target % _yz / _z;
                    if (y - 1 < 0)
                        continue;

                    index = target - 1 * _z;
                    break;
                case (int)EDir.Front:
                    if (target % _z + 1 >= _z) continue;

                    index = target + 1;                    
                    break;
                case (int)EDir.Back:
                    if (target % _z - 1 < 0)
                        continue;

                    index = target - 1;                    
                    break;
            }
            if (index > 0 && index < _size && !CheckUsed(index))
            {
                _validDir.Add((EDir)i);
            }
        }
    }
    /// <summary>
    /// 检查目标点是否已经使用
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    bool CheckUsed(int index)
    {
        return _used.Contains(index);
    }
    /// <summary>
    /// 添加一个路径点
    /// </summary>
    /// <param name="index"></param>
    void AddPoint(int index)
    {
        _used.Add(index);
        _data[index] = true;
    }
    /// <summary>
    /// 转换索引为xyz值
    /// </summary>
    /// <param name="index"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void TransIndex(int index,out int x, out int y, out int z)
    {
         x = index / _yz;
         y = index % _yz / _z;
         z = index % _z;
    }
   
}
