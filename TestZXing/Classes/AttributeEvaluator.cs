using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TestZXing.Classes
{
    public class AttributeEvaluator
    {
        public object Instance { get; set; }

        public AttributeEvaluator(object instance)
        {
            Instance = instance;
        }

        //public bool Evaluate(object instance, string propertyName)
        //{
        //    AttributeCollection attributes = TypeDescriptor.GetProperties(instance)[propertyName].Attributes;
        //    foreach (var a in attributes)
        //    {
        //        if (a.GetType() == typeof(MergableAttribute))
        //        {
        //            return ((MergableAttribute)a).Mergable; //<-- how to cast a to appopriate type?
        //        }
        //    }
        //    return false;
        //}

        public bool Evaluate(string propertyName, Type attributeType, string attributeName)
        {
            AttributeCollection attributes = TypeDescriptor.GetProperties(Instance)[propertyName].Attributes;
            foreach (var a in attributes)
            {
                if (a.GetType() == attributeType)
                {
                    return (bool)a.GetType().GetProperty(attributeName).GetValue(a);
                }
            }
            return false;
        }

        public List<string> PropertiesByValueBool(bool matchValue, Type attributeType, string attributeName)
        {
            List<string> values = new List<string>();

            foreach (var property in Instance.GetType().GetProperties())
            {
                bool x = Evaluate(property.Name, attributeType, attributeName);
                if (x == matchValue)
                {
                    values.Add(property.Name);
                }
            }

            return values;

        }

    }
}
