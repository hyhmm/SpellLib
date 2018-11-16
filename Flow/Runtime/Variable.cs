using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace XFlow
{
    public class Variable
    {
        public object Value;

        public override string ToString()
        {
            return base.ToString();
        }

        public virtual void FromString(string str)
        {
            Value = str;
        }
    }

    public class Variable<T> : Variable
    {
        new public T Value;
    }

    public class BoolVariable : Variable<bool>
    {
        public static implicit operator BoolVariable(bool value) { return new BoolVariable { Value = value }; }
        public static explicit operator bool(BoolVariable v)
        {
            return v.Value;
        }
        public override void FromString(string str)
        {
            Value = bool.Parse(str);
        }
    }

    public class FloatVariable : Variable<float>
    {
        public static implicit operator FloatVariable(float value) { return new FloatVariable { Value = value }; }
        public override void FromString(string str)
        {
            Value = float.Parse(str);
        }
    }

    public class IntVariable : Variable<int>
    {
        public static implicit operator IntVariable(int value) { return new IntVariable { Value = value }; }
        public override void FromString(string str)
        {
            Value = int.Parse(str);
        }
    }

    public class StringVariable : Variable<string>
    {
        public static implicit operator StringVariable(string value) { return new StringVariable { Value = value }; }
    }

    public class ListIntVariable : Variable<List<int>>
    {
        public static implicit operator ListIntVariable(List<int> value) { return new ListIntVariable { Value = value }; }

        public override string ToString()
        {
            return string.Join(",", Value.ConvertAll(x => x.ToString()).ToArray());
        }

        public override void FromString(string str)
        {
            Value = Util.ConvertListItemsFromString<int>(str);
        }
    }

    public class ListFloatVariable : Variable<List<float>>
    {
        public static implicit operator ListFloatVariable(List<float> value) { return new ListFloatVariable { Value = value }; }

        public override string ToString()
        {
            return string.Join(",", Value.ConvertAll(x => x.ToString()).ToArray());
        }

        public override void FromString(string str)
        {
            Value = Util.ConvertListItemsFromString<float>(str);
        }
    }
}