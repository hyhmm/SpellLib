using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace XFlow
{
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute : Attribute
    {
        readonly public string category;
        public CategoryAttribute(string category)
        {
            this.category = category;
        }
    }

    ///Use for friendly names
	[AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute
    {
        readonly public string name;
        readonly public int priority;
        public NameAttribute(string name, int priority = 0)
        {
            this.name = name;
            this.priority = priority;
        }
    }

    ///Use to give a description
	[AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        readonly public string description;
        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }
}