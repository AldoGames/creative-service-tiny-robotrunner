#if NET_4_6
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using Unity.Tiny.Conversions;

namespace Unity.Tiny
{
    public class SortingLayerBindings : ComponentBinding
    {
        public SortingLayerBindings(UTinyType.Reference typeRef)
            : base(typeRef)
        {
        }

        protected override void OnRemoveBinding(UTinyEntity entity, UTinyObject component)
        {
            SetSortingLayerOrder(entity, 0, 0);
        }

        protected override void OnUpdateBinding(UTinyEntity entity, UTinyObject component)
        {
            SetSortingLayerOrder(entity, component.GetProperty<int>("layer"), component.GetProperty<int>("order"));
        }

        private void SetSortingLayerOrder(UTinyEntity entity, int sortingLayerID, int sortingOrder)
        {
            var group = GetComponent<SortingGroup>(entity);
            if (group && group != null)
            {
                group.sortingLayerID = sortingLayerID;
                group.sortingOrder = sortingOrder;
                EditorUtility.SetDirty(group);
            }

            var renderer = GetComponent<SpriteRenderer>(entity);
            if (renderer && renderer != null)
            {
                renderer.sortingLayerID = sortingLayerID;
                renderer.sortingOrder = sortingOrder;
                EditorUtility.SetDirty(renderer);
            }
        }
    }
}
#endif // NET_4_6
