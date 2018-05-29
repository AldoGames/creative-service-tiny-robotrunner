#if NET_4_6
using UnityEngine;
using Unity.Tiny.Conversions;
using Unity.Tiny.Extensions;

namespace Unity.Tiny
{
    public enum DrawMode
    {
        ContinuousTiling,
        AdaptiveTiling,
        Stretch
    }

    public class Sprite2DRendererOptionsBindings : ComponentBinding
    {
        public Sprite2DRendererOptionsBindings(UTinyType.Reference typeRef) : base(typeRef)
        {
            RequireComponentType<SpriteRenderer>();
            UnityEditor.EditorApplication.delayCall += () =>
            {
                RegisterForEvent(UTinyEditorApplication.Registry.GetSprite2DRendererType());
                RegisterForEvent(UTinyEditorApplication.Registry.GetTransformType());
            };
        }

        protected override void OnAddBinding(UTinyEntity entity, UTinyObject component)
        {
            var renderer = GetComponent<SpriteRenderer>(entity);

            SetDrawMode(renderer, component.GetProperty<DrawMode>("drawMode"));
            if (component.GetProperty<Vector2>("size") == Vector2.zero)
            {
                component.AssignPropertyFrom("size", renderer.size);
            }
        }

        protected override void OnRemoveComponent(UTinyEntity entity, UTinyObject component)
        {
            var renderer = GetComponent<SpriteRenderer>(entity);
            renderer.drawMode = SpriteDrawMode.Simple;
        }

        protected override void OnUpdateBinding(UTinyEntity entity, UTinyObject component)
        {
            var renderer = GetComponent<SpriteRenderer>(entity);
            renderer.size = component.GetProperty<Vector2>("size");
            SetDrawMode(renderer, component.GetProperty<DrawMode>("drawMode"));
        }

        private void SetDrawMode(SpriteRenderer renderer, DrawMode mode)
        {
            switch (mode)
            {
                case DrawMode.ContinuousTiling:
                    renderer.drawMode = SpriteDrawMode.Tiled;
                    renderer.tileMode = SpriteTileMode.Continuous;
                    break;
                case DrawMode.AdaptiveTiling:
                    renderer.drawMode = SpriteDrawMode.Tiled;
                    renderer.tileMode = SpriteTileMode.Adaptive;
                    break;
                case DrawMode.Stretch:
                    renderer.drawMode = SpriteDrawMode.Sliced;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(mode));
            }
        }
    }
}
#endif // NET_4_6
