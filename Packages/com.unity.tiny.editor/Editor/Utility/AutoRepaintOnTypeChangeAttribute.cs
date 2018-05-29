using System;
using UnityEngine;

namespace Unity.Tiny
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AutoRepaintOnTypeChangeAttribute : Attribute
    {
        public readonly Type TinyType;

        public AutoRepaintOnTypeChangeAttribute(Type type)
        {
            TinyType = type;
        }
    }
}
