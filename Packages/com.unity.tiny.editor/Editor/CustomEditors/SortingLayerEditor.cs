#if NET_4_6
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.Properties;

namespace Unity.Tiny
{
    public class SortingLayerEditor : ComponentEditor
    {
        public override bool VisitComponent(UTinyObject tinyObject)
        {
            DrawEnum(tinyObject, "layer");
            DoField<int>(tinyObject, "order");
            return true;
        }
        
        private void DoField<T>(UTinyObject tinyObject, string fieldName)
        {
            var container = tinyObject.Properties;
            var prop = container.PropertyBag.FindProperty(fieldName) as IProperty<UTinyObject.PropertiesContainer, T>;
            var context = new VisitContext<T> {Index = -1, Property = prop, Value = prop.GetValue(container)};
            if (!ExcludeVisit(container, context))
            {
                Visit(container, context);
            }
        }
        
        private void DrawEnum(UTinyObject tinyObject, string fieldName)
        {
            var layers = SortingLayer.layers.ToList();
            var names = layers.Select(l => new GUIContent(l.name)).ToArray();
            var index = layers.FindIndex(l => l.id == (int)tinyObject[fieldName]);
            var label = new GUIContent(fieldName);

            if (names.Length == 0)
            {
                EditorGUILayout.Popup(label, -1, names);
                return;
            }

            EditorGUI.BeginChangeCheck();
            index = EditorGUILayout.Popup(label, index, names);
            if (EditorGUI.EndChangeCheck())
            {
                tinyObject[fieldName] = layers[index].id;
            }
        }
    }
}
#endif // NET_4_6
