using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public class Data
    {
        public string Name;
        public object Value;

        private string strValue = null;
        public string StrValue
        {
            get
            {
                if (strValue == null)
                    strValue = ToValueString(Value);
                return strValue;
            }
            set
            {
                if (strValue == value)
                    return;

                strValue = value;
                Value = ToValue(strValue);
            }
        }

        public string ToValueString(object value)
        {
            string ret = "";
            System.Type type = value.GetType();
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

        public object ToValue(string strValue)
        {
            object value;
            if (strValue.Contains(","))
            {
                if (strValue.Contains("."))
                {
                    value = Util.ConvertListItemsFromString<float>(strValue);
                }
                else
                {
                    value = Util.ConvertListItemsFromString<int>(strValue);
                }
            }
            else
            {
                if (strValue.Contains("."))
                    value = float.Parse(strValue);
                else
                    value = int.Parse(strValue);
            }
            return value;
        }
    }

    List<Data> showDataList;
    public List<Data> ShowDataList
    {
        get
        {
            if (showDataList == null)
            {
                showDataList = new List<Data>();
                foreach (var itr in DataSource)
                {
                    showDataList.Add(new Data()
                    {
                        Name = itr.Key,
                        Value = itr.Value
                    });
                }
            }
            return showDataList;
        }
    }

    public void AddShowData()
    {
        ShowDataList.Add(new Data()
        {
            Name = GetNewName("New"),
            Value = 0
        });
    }

    string GetNewName(string name)
    {
        foreach (var data in this.ShowDataList)
        {
            if (data.Name == name)
            {
                return GetNewName(name + "1");
            }
        }
        return name;
    }
}
