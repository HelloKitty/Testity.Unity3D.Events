
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Testity.EngineComponents.Unity3D;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace Testity.Unity3D.Events.Editor
{
	[CustomPropertyDrawer(typeof(TestityEventBase), true)]
	public class TesityEventDrawer : PropertyDrawer
	{
		protected class State
		{
			public ReorderableList m_ReorderableList;
			public int lastSelectedIndex;
		}
		private class Styles
		{
			public readonly GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus");
			public readonly GUIStyle genericFieldStyle = EditorStyles.label;
			public readonly GUIStyle removeButton = "InvisibleButton";
		}
		private struct ValidMethodMap
		{
			public UnityEngine.Object target;
			public MethodInfo methodInfo;
			public TestityPersistentListenerMode mode;
		}
		private struct UnityEventFunction
		{
			private readonly SerializedProperty m_Listener;
			private readonly UnityEngine.Object m_Target;
			private readonly MethodInfo m_Method;
			private readonly TestityPersistentListenerMode m_Mode;
			public UnityEventFunction(SerializedProperty listener, UnityEngine.Object target, MethodInfo method, TestityPersistentListenerMode mode)
			{
				this.m_Listener = listener;
				this.m_Target = target;
				this.m_Method = method;
				this.m_Mode = mode;
			}
			public void Assign()
			{
				SerializedProperty serializedProperty = this.m_Listener.FindPropertyRelative("m_Target");
				SerializedProperty serializedProperty2 = this.m_Listener.FindPropertyRelative("m_MethodName");
				SerializedProperty serializedProperty3 = this.m_Listener.FindPropertyRelative("m_Mode");
				SerializedProperty serializedProperty4 = this.m_Listener.FindPropertyRelative("m_Arguments");
				serializedProperty.objectReferenceValue = this.m_Target;
				serializedProperty2.stringValue = this.m_Method.Name;
				serializedProperty3.enumValueIndex = (int)this.m_Mode;
				if (this.m_Mode == TestityPersistentListenerMode.Object)
				{
					SerializedProperty serializedProperty5 = serializedProperty4.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
					ParameterInfo[] parameters = this.m_Method.GetParameters();
					if (parameters.Length == 1 && typeof(UnityEngine.Object).IsAssignableFrom(parameters[0].ParameterType))
					{
						serializedProperty5.stringValue = parameters[0].ParameterType.AssemblyQualifiedName;
					}
					else
					{
						serializedProperty5.stringValue = typeof(UnityEngine.Object).AssemblyQualifiedName;
					}
				}

				this.ValidateObjectParamater(serializedProperty4, this.m_Mode);

				this.m_Listener.serializedObject.ApplyModifiedProperties();
			}
			private void ValidateObjectParamater(SerializedProperty arguments, TestityPersistentListenerMode mode)
			{
				SerializedProperty serializedProperty = arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
				SerializedProperty serializedProperty2 = arguments.FindPropertyRelative("m_ObjectArgument");
				UnityEngine.Object objectReferenceValue = serializedProperty2.objectReferenceValue;
				if (mode != TestityPersistentListenerMode.Object)
				{
					serializedProperty.stringValue = typeof(UnityEngine.Object).AssemblyQualifiedName;
					serializedProperty2.objectReferenceValue = null;
					return;
				}
				if (objectReferenceValue == null)
				{
					return;
				}
				Type type = Type.GetType(serializedProperty.stringValue, false);
				if (!typeof(UnityEngine.Object).IsAssignableFrom(type) || !type.IsInstanceOfType(objectReferenceValue))
				{
					serializedProperty2.objectReferenceValue = null;
				}
			}
			public void Clear()
			{
				SerializedProperty serializedProperty = this.m_Listener.FindPropertyRelative("m_MethodName");
				serializedProperty.stringValue = null;
				SerializedProperty serializedProperty2 = this.m_Listener.FindPropertyRelative("m_Mode");
				serializedProperty2.enumValueIndex = 1;
				this.m_Listener.serializedObject.ApplyModifiedProperties();
			}
		}
		private const string kNoFunctionString = "No Function";
		private const string kInstancePath = "m_Target";
		private const string kCallStatePath = "m_CallState";
		private const string kArgumentsPath = "m_Arguments";
		private const string kModePath = "m_Mode";
		private const string kMethodNamePath = "m_MethodName";
		private const string kFloatArgument = "m_FloatArgument";
		private const string kIntArgument = "m_IntArgument";
		private const string kObjectArgument = "m_ObjectArgument";
		private const string kStringArgument = "m_StringArgument";
		private const string kBoolArgument = "m_BoolArgument";
		private const string kObjectArgumentAssemblyTypeName = "m_ObjectArgumentAssemblyTypeName";
		private const int kExtraSpacing = 9;
		private TesityEventDrawer.Styles m_Styles;
		private string m_Text;
		private TestityEventBase m_DummyEvent;
		private SerializedProperty m_Prop;
		private SerializedProperty m_ListenersArray;
		private ReorderableList m_ReorderableList;
		private int m_LastSelectedIndex;
		private Dictionary<string, TesityEventDrawer.State> m_States = new Dictionary<string, TesityEventDrawer.State>();
		private static string GetEventParams(TestityEventBase evt)
		{
			MethodInfo methodInfo = evt.FindMethod("Invoke", evt, TestityPersistentListenerMode.EventDefined, null);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" (");
			Type[] array = (
				from x in methodInfo.GetParameters()
				select x.ParameterType).ToArray<Type>();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].Name);
				if (i < array.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}
		private TesityEventDrawer.State GetState(SerializedProperty prop)
		{
			string propertyPath = prop.propertyPath;
			TesityEventDrawer.State state;
			this.m_States.TryGetValue(propertyPath, out state);
			if (state == null)
			{
				state = new TesityEventDrawer.State();
				SerializedProperty elements = prop.FindPropertyRelative("m_PersistentCalls.m_Calls");
				state.m_ReorderableList = new ReorderableList(prop.serializedObject, elements, false, true, true, true);
				state.m_ReorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawEventHeader);
				state.m_ReorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawEventListener);
				state.m_ReorderableList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectEventListener);
				state.m_ReorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild);
				state.m_ReorderableList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddEventListener);
				state.m_ReorderableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveButton);
				state.m_ReorderableList.elementHeight = 43f;
				this.m_States[propertyPath] = state;
			}
			return state;
		}
		private TesityEventDrawer.State RestoreState(SerializedProperty property)
		{
			TesityEventDrawer.State state = this.GetState(property);
			this.m_ListenersArray = state.m_ReorderableList.serializedProperty;
			this.m_ReorderableList = state.m_ReorderableList;
			this.m_LastSelectedIndex = state.lastSelectedIndex;
			return state;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.m_Prop = property;
			this.m_Text = label.text;
			TesityEventDrawer.State state = this.RestoreState(property);
			this.OnGUI(position);
			state.lastSelectedIndex = this.m_LastSelectedIndex;
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			this.RestoreState(property);
			float result = 0f;
			if (this.m_ReorderableList != null)
			{
				result = this.m_ReorderableList.GetHeight();
			}
			return result;
		}
		public void OnGUI(Rect position)
		{
			if (this.m_ListenersArray == null || !this.m_ListenersArray.isArray)
			{
				return;
			}
			this.m_DummyEvent = TesityEventDrawer.GetDummyEvent(this.m_Prop);
			if (this.m_DummyEvent == null)
			{
				return;
			}
			if (this.m_Styles == null)
			{
				this.m_Styles = new TesityEventDrawer.Styles();
			}
			if (this.m_ReorderableList != null)
			{
				int indentLevel = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
				this.m_ReorderableList.DoList(position);
				EditorGUI.indentLevel = indentLevel;
			}
		}
		protected virtual void DrawEventHeader(Rect headerRect)
		{
			headerRect.height = 16f;
			string text = ((!string.IsNullOrEmpty(this.m_Text)) ? this.m_Text : "Event") + TesityEventDrawer.GetEventParams(this.m_DummyEvent);
			GUI.Label(headerRect, text);
		}
		private static TestityPersistentListenerMode GetMode(SerializedProperty mode)
		{
			return (TestityPersistentListenerMode)mode.enumValueIndex;
		}
		private void DrawEventListener(Rect rect, int index, bool isactive, bool isfocused)
		{
			SerializedProperty arrayElementAtIndex = this.m_ListenersArray.GetArrayElementAtIndex(index);
			rect.y += 1f;
			Rect[] rowRects = this.GetRowRects(rect);
			Rect position = rowRects[0];
			Rect position2 = rowRects[1];
			Rect rect2 = rowRects[2];
			Rect position3 = rowRects[3];
			SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("m_CallState");
			SerializedProperty mode = arrayElementAtIndex.FindPropertyRelative("m_Mode");
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
			SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("m_Target");
			SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.white;
			EditorGUI.PropertyField(position, property, GUIContent.none);
			EditorGUI.BeginChangeCheck();
			GUI.Box(position2, GUIContent.none);
			EditorGUI.PropertyField(position2, serializedProperty2, GUIContent.none);
			if (EditorGUI.EndChangeCheck())
			{
				serializedProperty3.stringValue = null;
			}
			TestityPersistentListenerMode mode2 = TesityEventDrawer.GetMode(mode);
			SerializedProperty serializedProperty4;
			switch (mode2)
			{
				case TestityPersistentListenerMode.Object:
					serializedProperty4 = serializedProperty.FindPropertyRelative("m_ObjectArgument");
					break;
				case TestityPersistentListenerMode.Int:
					serializedProperty4 = serializedProperty.FindPropertyRelative("m_IntArgument");
					break;
				case TestityPersistentListenerMode.Float:
					serializedProperty4 = serializedProperty.FindPropertyRelative("m_FloatArgument");
					break;
				case TestityPersistentListenerMode.String:
					serializedProperty4 = serializedProperty.FindPropertyRelative("m_StringArgument");
					break;
				case TestityPersistentListenerMode.Bool:
					serializedProperty4 = serializedProperty.FindPropertyRelative("m_BoolArgument");
					break;
				default:
					serializedProperty4 = serializedProperty.FindPropertyRelative("m_IntArgument");
					break;
			}
			string stringValue = serializedProperty.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
			Type type = typeof(UnityEngine.Object);
			if (!string.IsNullOrEmpty(stringValue))
			{
				type = (Type.GetType(stringValue, false) ?? typeof(UnityEngine.Object));
			}
			if (mode2 == TestityPersistentListenerMode.Object)
			{
				EditorGUI.BeginChangeCheck();
				UnityEngine.Object objectReferenceValue = EditorGUI.ObjectField(position3, GUIContent.none, serializedProperty4.objectReferenceValue, type, true);
				if (EditorGUI.EndChangeCheck())
				{
					serializedProperty4.objectReferenceValue = objectReferenceValue;
				}
			}
			else
			{
				if (mode2 != TestityPersistentListenerMode.Void && mode2 != TestityPersistentListenerMode.EventDefined)
				{
					EditorGUI.PropertyField(position3, serializedProperty4, GUIContent.none);
				}
			}
			EditorGUI.BeginDisabledGroup(serializedProperty2.objectReferenceValue == null);
			EditorGUI.BeginProperty(rect2, GUIContent.none, serializedProperty3);
			GUIContent content;
			if (EditorGUI.showMixedValue)
			{
				content = new GUIContent("Temp");//MODIFIED: EditorGUI.mixedValueContent;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (serializedProperty2.objectReferenceValue == null || string.IsNullOrEmpty(serializedProperty3.stringValue))
				{
					stringBuilder.Append("No Function");
				}
				else
				{
					if (!TesityEventDrawer.IsPersistantListenerValid(this.m_DummyEvent, serializedProperty3.stringValue, serializedProperty2.objectReferenceValue, TesityEventDrawer.GetMode(mode), type))
					{
						string arg = "UnknownComponent";
						UnityEngine.Object objectReferenceValue2 = serializedProperty2.objectReferenceValue;
						if (objectReferenceValue2 != null)
						{
							arg = objectReferenceValue2.GetType().Name;
						}
						stringBuilder.Append(string.Format("<Missing {0}.{1}>", arg, serializedProperty3.stringValue));
					}
					else
					{
						stringBuilder.Append(serializedProperty2.objectReferenceValue.GetType().Name);
						if (!string.IsNullOrEmpty(serializedProperty3.stringValue))
						{
							stringBuilder.Append(".");
							if (serializedProperty3.stringValue.StartsWith("set_"))
							{
								stringBuilder.Append(serializedProperty3.stringValue.Substring(4));
							}
							else
							{
								stringBuilder.Append(serializedProperty3.stringValue);
							}
						}
					}
				}

				content = new GUIContent("Temp");//MODIFIED: GUIContent.Temp(stringBuilder.ToString());
			}
			if (GUI.Button(rect2, content, EditorStyles.popup))
			{
				TesityEventDrawer.BuildPopupList(serializedProperty2.objectReferenceValue, this.m_DummyEvent, arrayElementAtIndex).DropDown(rect2);
			}
			EditorGUI.EndProperty();
			EditorGUI.EndDisabledGroup();
			GUI.backgroundColor = backgroundColor;
		}
		private Rect[] GetRowRects(Rect rect)
		{
			Rect[] array = new Rect[4];
			rect.height = 16f;
			rect.y += 2f;
			Rect rect2 = rect;
			rect2.width *= 0.3f;
			Rect rect3 = rect2;
			rect3.y += EditorGUIUtility.singleLineHeight + 2f;
			Rect rect4 = rect;
			rect4.xMin = rect3.xMax + 5f;
			Rect rect5 = rect4;
			rect5.y += EditorGUIUtility.singleLineHeight + 2f;
			array[0] = rect2;
			array[1] = rect3;
			array[2] = rect4;
			array[3] = rect5;
			return array;
		}
		private void RemoveButton(ReorderableList list)
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
			this.m_LastSelectedIndex = list.index;
		}
		private void AddEventListener(ReorderableList list)
		{
			if (this.m_ListenersArray.hasMultipleDifferentValues)
			{
				UnityEngine.Object[] targetObjects = this.m_ListenersArray.serializedObject.targetObjects;
				for (int i = 0; i < targetObjects.Length; i++)
				{
					UnityEngine.Object obj = targetObjects[i];
					SerializedObject serializedObject = new SerializedObject(obj);
					SerializedProperty serializedProperty = serializedObject.FindProperty(this.m_ListenersArray.propertyPath);
					serializedProperty.arraySize++;
					serializedObject.ApplyModifiedProperties();
				}
				this.m_ListenersArray.serializedObject.SetIsDifferentCacheDirty();
				this.m_ListenersArray.serializedObject.Update();
				list.index = list.serializedProperty.arraySize - 1;
			}
			else
			{
				ReorderableList.defaultBehaviours.DoAddButton(list);
			}
			this.m_LastSelectedIndex = list.index;
			SerializedProperty arrayElementAtIndex = this.m_ListenersArray.GetArrayElementAtIndex(list.index);
			SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("m_CallState");
			SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("m_Target");
			SerializedProperty serializedProperty4 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
			SerializedProperty serializedProperty5 = arrayElementAtIndex.FindPropertyRelative("m_Mode");
			SerializedProperty serializedProperty6 = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
			serializedProperty2.enumValueIndex = 2;
			serializedProperty3.objectReferenceValue = null;
			serializedProperty4.stringValue = null;
			serializedProperty5.enumValueIndex = 1;
			serializedProperty6.FindPropertyRelative("m_FloatArgument").floatValue = 0f;
			serializedProperty6.FindPropertyRelative("m_IntArgument").intValue = 0;
			serializedProperty6.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = null;
			serializedProperty6.FindPropertyRelative("m_StringArgument").stringValue = null;
			serializedProperty6.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = null;
		}
		private void SelectEventListener(ReorderableList list)
		{
			this.m_LastSelectedIndex = list.index;
		}
		private void EndDragChild(ReorderableList list)
		{
			this.m_LastSelectedIndex = list.index;
		}
		private static TestityEventBase GetDummyEvent(SerializedProperty prop)
		{
			string stringValue = prop.FindPropertyRelative("m_TypeName").stringValue;
			Type type = Type.GetType(stringValue, false);
			if (type == null)
			{
				return new TestityEvent();
			}
			return Activator.CreateInstance(type) as TestityEventBase;
		}
		private static IEnumerable<TesityEventDrawer.ValidMethodMap> CalculateMethodMap(UnityEngine.Object target, Type[] t, bool allowSubclasses)
		{
			List<TesityEventDrawer.ValidMethodMap> list = new List<TesityEventDrawer.ValidMethodMap>();
			if (target == null || t == null)
			{
				return list;
			}

			//MODIFIED: This is a test to see if this is where I need to change stuff
			Type type = target.GetType();

			//If it's an ITestityBehaviour type it needs to be handled differently
			if (typeof(ITestityBehaviour).IsAssignableFrom(type))
			{
				//Debug.Log("Targeting Testity Behaviour.");

				type = type.BaseType.GetGenericArguments().First(); //this will grab the EngineScriptComponent child Type used as a generic arg in TestityBehaviour<T>
			}

			List<MethodInfo> list2 = (
				from x in type.GetMethods()
				where !x.IsSpecialName
				select x).ToList<MethodInfo>();
			IEnumerable<PropertyInfo> source = type.GetProperties().AsEnumerable<PropertyInfo>();
			source =
				from x in source
				where x.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && x.GetSetMethod() != null
				select x;
			list2.AddRange(
				from x in source
				select x.GetSetMethod());
			foreach (MethodInfo current in list2)
			{
				ParameterInfo[] parameters = current.GetParameters();
				if (parameters.Length == t.Length)
				{
					if (current.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length <= 0)
					{
						if (current.ReturnType == typeof(void))
						{
							bool flag = true;
							for (int i = 0; i < t.Length; i++)
							{
								if (!parameters[i].ParameterType.IsAssignableFrom(t[i]))
								{
									flag = false;
								}
								if (allowSubclasses && t[i].IsAssignableFrom(parameters[i].ParameterType))
								{
									flag = true;
								}
							}
							if (flag)
							{
								list.Add(new TesityEventDrawer.ValidMethodMap
								{
									target = target,
									methodInfo = current
								});
							}
						}
					}
				}
			}
			return list;
		}
		public static bool IsPersistantListenerValid(TestityEventBase dummyEvent, string methodName, UnityEngine.Object uObject, TestityPersistentListenerMode modeEnum, Type argumentType)
		{
			return !(uObject == null) && !string.IsNullOrEmpty(methodName) && dummyEvent.FindMethod(methodName, uObject, modeEnum, argumentType) != null;
		}
		private static GenericMenu BuildPopupList(UnityEngine.Object target, TestityEventBase dummyEvent, SerializedProperty listener)
		{
			UnityEngine.Object @object = target;
			if (@object is Component)
			{
				@object = (target as Component).gameObject;
			}
			SerializedProperty serializedProperty = listener.FindPropertyRelative("m_MethodName");
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("No Function"), string.IsNullOrEmpty(serializedProperty.stringValue), new GenericMenu.MenuFunction2(TesityEventDrawer.ClearEventFunction), new TesityEventDrawer.UnityEventFunction(listener, null, null, TestityPersistentListenerMode.EventDefined));
			if (@object == null)
			{
				return genericMenu;
			}
			genericMenu.AddSeparator(string.Empty);
			Type type = dummyEvent.GetType();
			MethodInfo method = type.GetMethod("Invoke");
			Type[] delegateArgumentsTypes = (
				from x in method.GetParameters()
				select x.ParameterType).ToArray<Type>();
			TesityEventDrawer.GeneratePopUpForType(genericMenu, @object, false, listener, delegateArgumentsTypes);
			if (@object is GameObject)
			{
				Component[] components = (@object as GameObject).GetComponents<Component>();
				List<string> list = (
					from c in components
					where c != null
					select c.GetType().Name into x
					group x by x into g
					where g.Count<string>() > 1
					select g.Key).ToList<string>();
				Component[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					Component component = array[i];
					if (!(component == null))
					{
						TesityEventDrawer.GeneratePopUpForType(genericMenu, component, list.Contains(component.GetType().Name), listener, delegateArgumentsTypes);
					}
				}
			}
			return genericMenu;
		}
		private static void GeneratePopUpForType(GenericMenu menu, UnityEngine.Object target, bool useFullTargetName, SerializedProperty listener, Type[] delegateArgumentsTypes)
		{
			List<TesityEventDrawer.ValidMethodMap> list = new List<TesityEventDrawer.ValidMethodMap>();
			string text = (!useFullTargetName) ? target.GetType().Name : target.GetType().FullName;
			bool flag = false;
			if (delegateArgumentsTypes.Length != 0)
			{
				TesityEventDrawer.GetMethodsForTargetAndMode(target, delegateArgumentsTypes, list, TestityPersistentListenerMode.EventDefined);
				if (list.Count > 0)
				{
					menu.AddDisabledItem(new GUIContent(text + "/Dynamic " + string.Join(", ", (
						from e in delegateArgumentsTypes
						select TesityEventDrawer.GetTypeName(e)).ToArray<string>())));
					TesityEventDrawer.AddMethodsToMenu(menu, listener, list, text);
					flag = true;
				}
			}
			list.Clear();
			TesityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
		{
			typeof(float)
		}, list, TestityPersistentListenerMode.Float);
			TesityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
		{
			typeof(int)
		}, list, TestityPersistentListenerMode.Int);
			TesityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
		{
			typeof(string)
		}, list, TestityPersistentListenerMode.String);
			TesityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
		{
			typeof(bool)
		}, list, TestityPersistentListenerMode.Bool);
			TesityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
		{
			typeof(UnityEngine.Object)
		}, list, TestityPersistentListenerMode.Object);
			TesityEventDrawer.GetMethodsForTargetAndMode(target, new Type[0], list, TestityPersistentListenerMode.Void);
			if (list.Count > 0)
			{
				if (flag)
				{
					menu.AddItem(new GUIContent(text + "/ "), false, null);
				}
				if (delegateArgumentsTypes.Length != 0)
				{
					menu.AddDisabledItem(new GUIContent(text + "/Static Parameters"));
				}
				TesityEventDrawer.AddMethodsToMenu(menu, listener, list, text);
			}
		}
		private static void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<TesityEventDrawer.ValidMethodMap> methods, string targetName)
		{
			IEnumerable<TesityEventDrawer.ValidMethodMap> enumerable =
				from e in methods
				orderby (!e.methodInfo.Name.StartsWith("set_")) ? 1 : 0, e.methodInfo.Name
				select e;
			foreach (TesityEventDrawer.ValidMethodMap current in enumerable)
			{
				TesityEventDrawer.AddFunctionsForScript(menu, listener, current, targetName);
			}
		}
		private static void GetMethodsForTargetAndMode(UnityEngine.Object target, Type[] delegateArgumentsTypes, List<TesityEventDrawer.ValidMethodMap> methods, TestityPersistentListenerMode mode)
		{
			IEnumerable<TesityEventDrawer.ValidMethodMap> enumerable = TesityEventDrawer.CalculateMethodMap(target, delegateArgumentsTypes, mode == TestityPersistentListenerMode.Object);
			foreach (TesityEventDrawer.ValidMethodMap current in enumerable)
			{
				TesityEventDrawer.ValidMethodMap item = current;
				item.mode = mode;
				methods.Add(item);
			}
		}
		private static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, TesityEventDrawer.ValidMethodMap method, string targetName)
		{
			TestityPersistentListenerMode mode = method.mode;
			UnityEngine.Object objectReferenceValue = listener.FindPropertyRelative("m_Target").objectReferenceValue;
			string stringValue = listener.FindPropertyRelative("m_MethodName").stringValue;
			TestityPersistentListenerMode mode2 = TesityEventDrawer.GetMode(listener.FindPropertyRelative("m_Mode"));
			SerializedProperty serializedProperty = listener.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
			StringBuilder stringBuilder = new StringBuilder();
			int num = method.methodInfo.GetParameters().Length;
			for (int i = 0; i < num; i++)
			{
				ParameterInfo parameterInfo = method.methodInfo.GetParameters()[i];
				stringBuilder.Append(string.Format("{0}", TesityEventDrawer.GetTypeName(parameterInfo.ParameterType)));
				if (i < num - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			bool flag = objectReferenceValue == method.target && stringValue == method.methodInfo.Name && mode == mode2;
			if (flag && mode == TestityPersistentListenerMode.Object && method.methodInfo.GetParameters().Length == 1)
			{
				flag &= (method.methodInfo.GetParameters()[0].ParameterType.AssemblyQualifiedName == serializedProperty.stringValue);
			}
			string formattedMethodName = TesityEventDrawer.GetFormattedMethodName(targetName, method.methodInfo.Name, stringBuilder.ToString(), mode == TestityPersistentListenerMode.EventDefined);
			menu.AddItem(new GUIContent(formattedMethodName), flag, new GenericMenu.MenuFunction2(TesityEventDrawer.SetEventFunction), new TesityEventDrawer.UnityEventFunction(listener, method.target, method.methodInfo, mode));
		}
		private static string GetTypeName(Type t)
		{
			if (t == typeof(int))
			{
				return "int";
			}
			if (t == typeof(float))
			{
				return "float";
			}
			if (t == typeof(string))
			{
				return "string";
			}
			if (t == typeof(bool))
			{
				return "bool";
			}
			return t.Name;
		}
		private static string GetFormattedMethodName(string targetName, string methodName, string args, bool dynamic)
		{
			if (dynamic)
			{
				if (methodName.StartsWith("set_"))
				{
					return string.Format("{0}/{1}", targetName, methodName.Substring(4));
				}
				return string.Format("{0}/{1}", targetName, methodName);
			}
			else
			{
				if (methodName.StartsWith("set_"))
				{
					return string.Format("{0}/{2} {1}", targetName, methodName.Substring(4), args);
				}
				return string.Format("{0}/{1} ({2})", targetName, methodName, args);
			}
		}
		private static void SetEventFunction(object source)
		{
			((TesityEventDrawer.UnityEventFunction)source).Assign();
		}
		private static void ClearEventFunction(object source)
		{
			((TesityEventDrawer.UnityEventFunction)source).Clear();
		}
	}
}
