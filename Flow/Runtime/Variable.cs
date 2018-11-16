using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace XFlow
{
    public class Variable
    {
        public object Value;
    }

    public class Variable<T> : Variable
    {
        new public T Value;
    }

    public class FloatVariable : Variable<float>
    {
        public static implicit operator FloatVariable(float value) { return new FloatVariable { Value = value }; }
    }

    public class IntVariable : Variable<int>
    {
        public static implicit operator IntVariable(int value) { return new IntVariable { Value = value }; }
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
    }

    public class ListFloatVariable : Variable<List<float>>
    {
        public static implicit operator ListFloatVariable(List<float> value) { return new ListFloatVariable { Value = value }; }

        public override string ToString()
        {
            return string.Join(",", Value.ConvertAll(x => x.ToString()).ToArray());
        }
    }
}