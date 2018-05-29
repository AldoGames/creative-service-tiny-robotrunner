#if NET_4_6
using UnityEngine.Rendering;
using Unity.Tiny.Conversions;
using Unity.Tiny.Extensions;

namespace Unity.Tiny
{
    public class SortingGroupBindings : ComponentBinding
    {
        public SortingGroupBindings(UTinyType.Reference typeRef) : base(typeRef)
        {
        }

        protected override void OnAddBinding(UTinyEntity entity, UTinyObject component)
        {
            AddMissingComponent<SortingGroup>(entity);
        }

        protected override void OnRemoveComponent(UTinyEntity entity, UTinyObject component)
        {
            RemoveComponent<SortingGroup>(entity);
        }

        protected override void OnUpdateBinding(UTinyEntity entity, UTinyObject component)
        {
            OnAddBinding(entity, component);
        }
    }
}
#endif // NET_4_6
