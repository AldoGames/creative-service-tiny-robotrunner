#if NET_4_6
using JetBrains.Annotations;

using UnityEditor;
using UnityEngine;

using Unity.Tiny.Extensions;

namespace Unity.Tiny
{
    [UsedImplicitly]
    public class SpriteRendererInversedBindings : InversedBindingsBase<SpriteRenderer>
    {
        #region Static
        [InitializeOnLoadMethod]
        private static void Register()
        {
            GameObjectTracker.RegisterForComponentModification<SpriteRenderer>(SyncRenderer);
        }

        public static void SyncRenderer(SpriteRenderer from, UTinyEntityView view)
        {
            var registry = view.Registry;
            var entity = view.EntityRef.Dereference(registry);

            var tinyRenderer = entity.GetComponent(registry.GetSprite2DRendererType());
            if (null != tinyRenderer)
            {
                SyncRenderer(from, tinyRenderer);
            }

            if (from.drawMode == SpriteDrawMode.Simple)
            {
                entity.RemoveComponent(registry.GetSprite2DRendererOptionsType());
            }
            else
            {
                var rendererOptions = entity.GetOrAddComponent(registry.GetSprite2DRendererOptionsType());
                if (null != rendererOptions)
                {
                    SyncRendererOptions(from, rendererOptions);
                }
            }
            TransformInversedBindings.SyncTransform(from.transform, view);
        }

        public static void SyncRenderer(SpriteRenderer from, [NotNull] UTinyObject renderer)
        {
            from.sharedMaterial = new Material(Shader.Find("UTiny/Sprite2D"));
            AssignIfDifferent(renderer, "sprite", from.sprite);
            AssignIfDifferent(renderer, "color", from.color);
        }

        public static void SyncRendererOptions(SpriteRenderer from, [NotNull] UTinyObject tiling)
        {
            AssignIfDifferent(tiling, "drawMode", Translate(from.drawMode, from.tileMode));
            AssignIfDifferent(tiling, "size", from.size);
        }

        private static DrawMode Translate(SpriteDrawMode drawMode, SpriteTileMode tileMode)
        {
            switch (drawMode)
            {
                case SpriteDrawMode.Sliced:
                    return DrawMode.Stretch;
                case SpriteDrawMode.Tiled:
                    return tileMode == SpriteTileMode.Continuous ? DrawMode.ContinuousTiling : DrawMode.AdaptiveTiling;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(drawMode));
            }
        }
        #endregion

        #region InversedBindingsBase<SpriteRenderer>
        public override void Create(UTinyEntityView view, SpriteRenderer spriteRenderer)
        {
            var sr = new UTinyObject(Registry, GetMainTinyType());
            SyncRenderer(spriteRenderer, sr);

            UTinyObject srt = null;
            if (spriteRenderer.drawMode != SpriteDrawMode.Simple)
            {
                srt = new UTinyObject(Registry, Registry.GetSprite2DRendererOptionsType());
                SyncRendererOptions(spriteRenderer, srt);
            }

            var entity = view.EntityRef.Dereference(Registry);
            var sprite2DRenderer = entity.GetOrAddComponent(GetMainTinyType());
            sprite2DRenderer.CopyFrom(sr);
            BindingsHelper.RunBindings(entity, sprite2DRenderer);

            if (null != srt)
            {
                var rendererOptions = entity.GetOrAddComponent(Registry.GetSprite2DRendererOptionsType());
                rendererOptions.CopyFrom(srt);
                BindingsHelper.RunBindings(entity, rendererOptions);
            }
        }

        public override UTinyType.Reference GetMainTinyType()
        {
            return Registry.GetSprite2DRendererType();
        }
        #endregion
    }
}
#endif // NET_4_6
