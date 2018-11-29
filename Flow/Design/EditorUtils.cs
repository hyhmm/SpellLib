using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using UnityObject = UnityEngine.Object;
namespace XFlow
{
    public class EditorUtils
    {
        ///Show an automatic editor GUI inspector for target object, taking into account drawer attributes
        public static void ReflectedObjectInspector(object target)
        {

            if (target == null)
            {
                return;
            }
    
            foreach (var field in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                //hide type altogether?
                if (field.FieldType.IsDefined(typeof(HideInInspector), true))
                {
                    continue;
                }

                //inspect only public fields or private fields with the [ExposeField] attribute
                if (field.IsPublic)
                {
                    var attributes = field.GetCustomAttributes(true);
                    //Hide field?
                    if (attributes.Any(a => a is HideInInspector))
                    {
                        continue;
                    }
                    field.SetValue(target, ReflectedFieldInspector(field.Name, field.GetValue(target), field.FieldType, field, target, attributes));
                }
            }
        }

        public static object ReflectedFieldInspector(string name, object value, Type t, FieldInfo field = null, object context = null, object[] attributes = null)
        {
            var content = new GUIContent(name);
            if (attributes != null)
            {
                ///Create proper GUIContent
                var nameAtt = attributes.FirstOrDefault(a => a is NameAttribute) as NameAttribute;
                if (nameAtt != null) { content.text = nameAtt.name; }

                var tooltipAtt = attributes.FirstOrDefault(a => a is TooltipAttribute) as TooltipAttribute;
                if (tooltipAtt != null) { content.tooltip = tooltipAtt.tooltip; }
            }

            return ReflectedFieldInspector(content, value, t, field, context, attributes);
        }

        ///Show an arbitrary field type editor. Passing a FieldInfo will also check for attributes.
		public static object ReflectedFieldInspector(GUIContent content, object value, Type t, FieldInfo field = null, object context = null, object[] attributes = null)
        {

            if (t == null)
            {
                GUILayout.Label("NO TYPE PROVIDED!");
                return value;
            }

            return DrawEditorFieldDirect(content, value, t, field, context, attributes);
        }


        ///Draws an Editor field for object of type directly
        public static object DrawEditorFieldDirect(GUIContent content, object value, Type t, FieldInfo field = null, object context = null, object[] attributes = null)
        {

            if (typeof(UnityObject).IsAssignableFrom(t) == false && t != typeof(Type))
            {
                //Check abstract
                if ((value != null && value.GetType().IsAbstract) || (value == null && t.IsAbstract))
                {
                    EditorGUILayout.LabelField(content, new GUIContent(string.Format("Abstract ({0})", t.ToString())));
                    return value;
                }

                //Auto create instance for some types
                if (value == null && t != typeof(object) && !t.IsAbstract && !t.IsInterface)
                {
                    if (t.GetConstructor(Type.EmptyTypes) != null || t.IsArray)
                    {
                        if (t.IsArray) { value = Array.CreateInstance(t.GetElementType(), 0); }
                        else { value = Activator.CreateInstance(t); }
                    }
                }
            }

            if (t == typeof(string))
            {
                return EditorGUILayout.TextField(content, (string)value);
            }
            return value;
        }

        static public string GetVariableType(Variable v)
        {
            if (v is IntVariable)
                return VariableType.Int.ToString();
            if (v is FloatVariable)
                return VariableType.Float.ToString();
            if (v is ListIntVariable)
                return VariableType.ListInt.ToString();
            if (v is ListFloatVariable)
                return VariableType.ListFloat.ToString();
            if (v is StringVariable)
                return VariableType.String.ToString();

            return v.GetType().ToString();
        }
     }
}