#if NET_4_6
using UnityEngine;
using Unity.Tiny.Conversions;

namespace Unity.Tiny
{
    public class Camera2DAxisSortBindings : ComponentBinding
    {
        public Camera2DAxisSortBindings(UTinyType.Reference typeRef)
            : base(typeRef)
        {
            RequireComponentType<Camera>();
        }

        protected override void OnRemoveBinding(UTinyEntity entity, UTinyObject component)
        {
            var camera = GetComponent<Camera>(entity);
            if (camera && camera != null)
            {
                camera.transparencySortMode = TransparencySortMode.Default;
            }
        }

        protected override void OnUpdateBinding(UTinyEntity entity, UTinyObject component)
        {
            var camera = GetComponent<Camera>(entity);
            if (camera && camera != null)
            {
                camera.transparencySortMode = TransparencySortMode.CustomAxis;
                camera.transparencySortAxis = component.GetProperty<Vector3>("axis");
            }
        }
    }
}
#endif // NET_4_6
