using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.Tiny
{
	internal static class TinyRepaintHelper
	{
		private static readonly HashSet<Type> s_ChangedTypes = new HashSet<Type>();
		private static readonly Dictionary<Type, List<Action>> s_RepaintMethods = new Dictionary<Type, List<Action>>();

		[InitializeOnLoadMethod]
		private static void Register()
		{
			UTinyEditorApplication.OnLoadProject += HandleProjectLoaded;
			UTinyEditorApplication.OnCloseProject += HandleProjectClosed;
			RegisterAutoRepaint();
		}

		private static void HandleProjectLoaded(UTinyProject project)
		{
			var caretaker = UTinyEditorApplication.EditorContext.Caretaker;
			caretaker.OnObjectChanged += HandleObjectChanged;
			caretaker.OnBeginUpdate   += HandleBeginUpdate;
			caretaker.OnEndUpdate     += HandleEndUpdate;

			var undo = UTinyEditorApplication.Undo;
			undo.OnUndoPerformed += RepaintAll;
			undo.OnRedoPerformed += RepaintAll;
		}

		private static void HandleProjectClosed(UTinyProject project)
		{
			var caretaker = UTinyEditorApplication.EditorContext.Caretaker;
			caretaker.OnObjectChanged -= HandleObjectChanged;
			caretaker.OnBeginUpdate   -= HandleBeginUpdate;
			caretaker.OnEndUpdate 	  -= HandleEndUpdate;

			var undo = UTinyEditorApplication.Undo;
			undo.OnUndoPerformed -= RepaintAll;
			undo.OnRedoPerformed -= RepaintAll;
		}

		private static void RegisterAutoRepaint()
		{
			foreach (var windowType in typeof(TinyRepaintHelper).Assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(EditorWindow)) &&
					        t.GetCustomAttributes(typeof(AutoRepaintOnTypeChangeAttribute)).Any()))
			{
				var repaintAllMethod = windowType.GetMethod("RepaintAll", BindingFlags.Static | BindingFlags.Public);
				if (null == repaintAllMethod || repaintAllMethod.GetParameters().Length > 0)
				{
					Debug.Log($"{UTinyConstants.ApplicationName}: To enable the AutoRepaint feature, the `{windowType.Name}` must derive from {nameof(EditorWindow)} and a public static method named RepaintAll().");
					continue;
				}

				var autoRepaints = windowType.GetCustomAttributes(typeof(AutoRepaintOnTypeChangeAttribute))
					.OfType<AutoRepaintOnTypeChangeAttribute>()
					.GroupBy(a => a.TinyType)
					.Select(group => group.First());

				foreach (var autoRepaint in autoRepaints)
				{
					if (null == autoRepaint.TinyType)
					{
						Debug.Log($"{UTinyConstants.ApplicationName}: The AutoRepaint feature will not work if no type is provided.");
						continue;
					}

					if (!autoRepaint.TinyType.IsSubclassOf(typeof(UTinyRegistryObjectBase)))
					{
						Debug.Log($"{UTinyConstants.ApplicationName}: The AutoRepaint feature will only work for subclasses of {nameof(UTinyRegistryObjectBase)}.");
						continue;
					}

					List<Action> repaintMethods;
					if (!s_RepaintMethods.TryGetValue(autoRepaint.TinyType, out repaintMethods))
					{
						s_RepaintMethods[autoRepaint.TinyType] = repaintMethods = new List<Action>();
					}

					repaintMethods.Add(() => repaintAllMethod.Invoke(null, null));
				}
			}
		}

		private static void HandleBeginUpdate()
		{
			s_ChangedTypes.Clear();
		}

		private static void HandleEndUpdate()
		{
			foreach (var target in s_ChangedTypes)
			{
				List<Action> repaintMethods;
				if (s_RepaintMethods.TryGetValue(target, out repaintMethods))
				{
					repaintMethods.ForEach(m => m());
				}
			}
		}

		private static void HandleObjectChanged(IOriginator originator, IMemento memento)
		{
			s_ChangedTypes.Add(originator.GetType());
		}

		private static void RepaintAll()
		{
			UTinyEditorUtility.RepaintAllWindows();
		}
	}
}
