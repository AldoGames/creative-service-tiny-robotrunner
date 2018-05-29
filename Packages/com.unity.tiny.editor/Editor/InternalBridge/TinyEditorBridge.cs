using System;
using UnityEditor;

namespace Unity.Tiny
{
	public static class TinyEditorBridge
	{
		// Bridge example
		public static void PreExport()
		{
			UnityEditor.U2D.SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);
		}
	}
}
