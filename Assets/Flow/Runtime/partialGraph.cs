using UnityEngine;
using System;
using System.Collections;
using System.Linq;
partial class Node
{
    public int X;
    public int Y;

    public float Width;

    public float Height;

    public Rect Rect
    {
        get { return new Rect(X, Y, Width, Height); }
        set
        {
            X = (int)value.x;
            Y = (int)value.y;
            Width = value.width;
            Height = value.height;
        }
    }

    public int DefaultWidth { get { return 100; } }
    public int DefaultHeight
    {
        get
        {
            return (FlowCount + PortCount) * 30;
        }
    }

    public int FlowCount
    {
        get { return System.Math.Max(FlowInDict.Count, FlowOutDict.Count); }
    }

    public int PortCount
    {
        get { return System.Math.Max(PortValueInDict.Count, PortValueOutDict.Count); }
    }
}

partial class Blackboard
{
    public string GetShowValue(string key)
    {
        string ret = "";
        object value = this[key];
        Type type = value.GetType();
        if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
        {
            var v = (IEnumerable)value;
            foreach (var iv in v)
            {
                ret += iv + ",";
            }
            return ret.Remove(ret.LastIndexOf(','));
        }
        else
        {
            return value.ToString();
        }
    }

    public void SetShowValue(string key, string value)
    {
        if (value.Contains(","))
        {
            if (value.Contains("."))
            {
                DataSource[key] = Util.ConvertListItemsFromString<float>(value);
            }
            else
            {
                DataSource[key] = Util.ConvertListItemsFromString<int>(value);
            }
        }
        else
        {
            if (value.Contains("."))
                DataSource[key] = float.Parse(value);
            else
                DataSource[key] = int.Parse(value);
        }
    }
}
