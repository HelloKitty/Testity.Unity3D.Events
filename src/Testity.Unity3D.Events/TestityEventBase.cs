using System;
using System.Reflection;
using Testity.EngineComponents.Unity3D;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

namespace Testity.Unity3D.Events
{
	/// <summary>
	///   <para>Abstract base class for UnityEvents.</para>
	/// </summary>
	[Serializable]
	public abstract class TestityEventBase : ISerializationCallbackReceiver
	{
		private TestityInvokableCallList m_Calls;

		[FormerlySerializedAs("m_PersistentListeners")]
		[SerializeField]
		private TestityPersistentCallGroup m_PersistentCalls;

		[SerializeField]
		private string m_TypeName;

		private bool m_CallsDirty = true;

		protected TestityEventBase()
		{
			this.m_Calls = new TestityInvokableCallList();
			this.m_PersistentCalls = new TestityPersistentCallGroup();
			this.m_TypeName = this.GetType().AssemblyQualifiedName;
		}

		public void AddBoolPersistentListener(TestityAction<bool> call, bool argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterBoolPersistentListener(persistentEventCount, call, argument);
		}

		public void AddCall(TestityBaseInvokableCall call)
		{
			this.m_Calls.AddListener(call);
		}

		public void AddFloatPersistentListener(TestityAction<float> call, float argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterFloatPersistentListener(persistentEventCount, call, argument);
		}

		public void AddIntPersistentListener(TestityAction<int> call, int argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterIntPersistentListener(persistentEventCount, call, argument);
		}

		protected void AddListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.AddListener(this.GetDelegate(targetObj, method));
		}

		public void AddObjectPersistentListener<T>(TestityAction<T> call, T argument)
		where T : UnityEngine.Object
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterObjectPersistentListener<T>(persistentEventCount, call, argument);
		}

		public void AddPersistentListener()
		{
			this.m_PersistentCalls.AddListener();
		}

		public void AddStringPersistentListener(TestityAction<string> call, string argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterStringPersistentListener(persistentEventCount, call, argument);
		}

		public void AddVoidPersistentListener(TestityAction call)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterVoidPersistentListener(persistentEventCount, call);
		}

		private void DirtyPersistentCalls()
		{
			this.m_Calls.ClearPersistent();
			this.m_CallsDirty = true;
		}

		public MethodInfo FindMethod(TestityPersistentCall call)
		{
			Type type = typeof(UnityEngine.Object);
			if (!string.IsNullOrEmpty(call.arguments.unityObjectArgumentAssemblyTypeName))
			{
				type = Type.GetType(call.arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(UnityEngine.Object);
			}
			return this.FindMethod(call.methodName, call.target, call.mode, type);
		}

		public MethodInfo FindMethod(string name, object listener, TestityPersistentListenerMode mode, Type argumentType)
		{
			switch (mode)
			{
				case TestityPersistentListenerMode.EventDefined:
					{
						return this.FindMethod_Impl(name, listener);
					}
				case TestityPersistentListenerMode.Void:
					{
						return TestityEventBase.GetValidMethodInfo(listener, name, new Type[0]);
					}
				case TestityPersistentListenerMode.Object:
					{
						return TestityEventBase.GetValidMethodInfo(listener, name, new Type[] { argumentType ?? typeof(UnityEngine.Object) });
					}
				case TestityPersistentListenerMode.Int:
					{
						return TestityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(int) });
					}
				case TestityPersistentListenerMode.Float:
					{
						return TestityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(float) });
					}
				case TestityPersistentListenerMode.String:
					{
						return TestityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(string) });
					}
				case TestityPersistentListenerMode.Bool:
					{
						return TestityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(bool) });
					}
			}
			return null;
		}

		protected abstract MethodInfo FindMethod_Impl(string name, object targetObj);

		public abstract TestityBaseInvokableCall GetDelegate(object target, MethodInfo theFunction);

		/// <summary>
		///   <para>Get the number of registered persistent listeners.</para>
		/// </summary>
		public int GetPersistentEventCount()
		{
			return this.m_PersistentCalls.Count;
		}

		/// <summary>
		///   <para>Get the target method name of the listener at index index.</para>
		/// </summary>
		/// <param name="index">Index of the listener to query.</param>
		public string GetPersistentMethodName(int index)
		{
			TestityPersistentCall listener = this.m_PersistentCalls.GetListener(index);
			return (listener == null ? string.Empty : listener.methodName);
		}

		/// <summary>
		///   <para>Get the target component of the listener at index index.</para>
		/// </summary>
		/// <param name="index">Index of the listener to query.</param>
		public UnityEngine.Object GetPersistentTarget(int index)
		{
			UnityEngine.Object obj;
			TestityPersistentCall listener = this.m_PersistentCalls.GetListener(index);
			if (listener == null)
			{
				obj = null;
			}
			else
			{
				obj = listener.target;
			}
			return obj;
		}

		/// <summary>
		///   <para>Given an object, function name, and a list of argument types; find the method that matches.</para>
		/// </summary>
		/// <param name="obj">Object to search for the method.</param>
		/// <param name="functionName">Function name to search for.</param>
		/// <param name="argumentTypes">Argument types for the function.</param>
		public static MethodInfo GetValidMethodInfo(object obj, string functionName, Type[] argumentTypes)
		{
			Type typeToParse = obj.GetType();

			//If it's an ITestityBehaviour type it needs to be handled differently
			if (typeof(ITestityBehaviour).IsAssignableFrom(typeToParse))
			{
				//We override typeToParse (imagine we route Unity down a different path) This dramatically changes the path it takes through types
				//But it'll follow up the hierharchy of the T in TestityBehaviour<T>
				typeToParse = typeToParse.BaseType.GetGenericArguments().First();//this will grab the EngineScriptComponent child Type used as a generic arg in TestityBehaviour<T>
			}

			for (Type i = typeToParse; i != typeof(object) && i != null; i = i.BaseType)
			{
				MethodInfo method = i.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
				if (method != null)
				{


					ParameterInfo[] parameters = method.GetParameters();
					bool isPrimitive = true;
					int num = 0;
					ParameterInfo[] parameterInfoArray = parameters;
					int num1 = 0;
					while (num1 < (int)parameterInfoArray.Length)
					{
						ParameterInfo parameterInfo = parameterInfoArray[num1];
						Type type = argumentTypes[num];
						Type parameterType = parameterInfo.ParameterType;
						isPrimitive = type.IsPrimitive == parameterType.IsPrimitive;
						if (isPrimitive)
						{
							num++;
							num1++;
						}
						else
						{
							break;
						}
					}
					if (isPrimitive)
					{
						return method;
					}
				}
			}
			return null;
		}

		protected void Invoke(object[] parameters)
		{
			this.RebuildPersistentCallsIfNeeded();
			this.m_Calls.Invoke(parameters);
		}

		private void RebuildPersistentCallsIfNeeded()
		{
			if (this.m_CallsDirty)
			{
				this.m_PersistentCalls.Initialize(this.m_Calls, this);
				this.m_CallsDirty = false;
			}
		}

		public void RegisterBoolPersistentListener(int index, TestityAction<bool> call, bool argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			if (!this.ValidateRegistration(call.Method, call.Target, TestityPersistentListenerMode.Bool))
			{
				return;
			}
			this.m_PersistentCalls.RegisterBoolPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
			this.DirtyPersistentCalls();
		}

		public void RegisterFloatPersistentListener(int index, TestityAction<float> call, float argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			if (!this.ValidateRegistration(call.Method, call.Target, TestityPersistentListenerMode.Float))
			{
				return;
			}
			this.m_PersistentCalls.RegisterFloatPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
			this.DirtyPersistentCalls();
		}

		public void RegisterIntPersistentListener(int index, TestityAction<int> call, int argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			if (!this.ValidateRegistration(call.Method, call.Target, TestityPersistentListenerMode.Int))
			{
				return;
			}
			this.m_PersistentCalls.RegisterIntPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
			this.DirtyPersistentCalls();
		}

		public void RegisterObjectPersistentListener<T>(int index, TestityAction<T> call, T argument)
		where T : UnityEngine.Object
		{
			if (call == null)
			{
				throw new ArgumentNullException("call", "Registering a Listener requires a non null call");
			}
			if (!this.ValidateRegistration(call.Method, call.Target, TestityPersistentListenerMode.Object, (argument != null ? argument.GetType() : typeof(UnityEngine.Object))))
			{
				return;
			}
			this.m_PersistentCalls.RegisterObjectPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
			this.DirtyPersistentCalls();
		}

		protected void RegisterPersistentListener(int index, object targetObj, MethodInfo method)
		{
			if (!this.ValidateRegistration(method, targetObj, TestityPersistentListenerMode.EventDefined))
			{
				return;
			}
			this.m_PersistentCalls.RegisterEventPersistentListener(index, targetObj as UnityEngine.Object, method.Name);
			this.DirtyPersistentCalls();
		}

		public void RegisterStringPersistentListener(int index, TestityAction<string> call, string argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			if (!this.ValidateRegistration(call.Method, call.Target, TestityPersistentListenerMode.String))
			{
				return;
			}
			this.m_PersistentCalls.RegisterStringPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
			this.DirtyPersistentCalls();
		}

		public void RegisterVoidPersistentListener(int index, TestityAction call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			if (!this.ValidateRegistration(call.Method, call.Target, TestityPersistentListenerMode.Void))
			{
				return;
			}
			this.m_PersistentCalls.RegisterVoidPersistentListener(index, call.Target as UnityEngine.Object, call.Method.Name);
			this.DirtyPersistentCalls();
		}

		/// <summary>
		///   <para>Remove all listeners from the event.</para>
		/// </summary>
		public void RemoveAllListeners()
		{
			this.m_Calls.Clear();
		}

		protected void RemoveListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.RemoveListener(targetObj, method);
		}

		public void RemovePersistentListener(UnityEngine.Object target, MethodInfo method)
		{
			if (method == null || method.IsStatic || target == null || target.GetInstanceID() == 0)
			{
				return;
			}
			this.m_PersistentCalls.RemoveListeners(target, method.Name);
			this.DirtyPersistentCalls();
		}

		public void RemovePersistentListener(int index)
		{
			this.m_PersistentCalls.RemoveListener(index);
			this.DirtyPersistentCalls();
		}

		/// <summary>
		///   <para>Modify the execution state of a persistent listener.</para>
		/// </summary>
		/// <param name="index">Index of the listener to query.</param>
		/// <param name="state">State to set.</param>
		public void SetPersistentListenerState(int index, TestityEventCallState state)
		{
			TestityPersistentCall listener = this.m_PersistentCalls.GetListener(index);
			if (listener != null)
			{
				listener.callState = state;
			}
			this.DirtyPersistentCalls();
		}

		public override string ToString()
		{
			return string.Concat(this.ToString(), " ", this.GetType().FullName);
		}

		void UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.DirtyPersistentCalls();
			this.m_TypeName = this.GetType().AssemblyQualifiedName;
		}

		void UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		public void UnregisterPersistentListener(int index)
		{
			this.m_PersistentCalls.UnregisterPersistentListener(index);
			this.DirtyPersistentCalls();
		}

		protected bool ValidateRegistration(MethodInfo method, object targetObj, TestityPersistentListenerMode mode)
		{
			return this.ValidateRegistration(method, targetObj, mode, typeof(UnityEngine.Object));
		}

		protected bool ValidateRegistration(MethodInfo method, object targetObj, TestityPersistentListenerMode mode, Type argumentType)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method", String.Format("Can not register null method on {0} for callback!", new object[] { targetObj }));
			}

			UnityEngine.Object obj = targetObj as UnityEngine.Object;
			if (obj == null || obj.GetInstanceID() == 0)
			{
				object[] name = new object[] { method.Name, targetObj, null };
				name[2] = (targetObj != null ? targetObj.GetType().ToString() : "null");
				throw new ArgumentException(String.Format("Could not register callback {0} on {1}. The class {2} does not derive from UnityEngine.Object", name));
			}
			if (method.IsStatic)
			{
				throw new ArgumentException(String.Format("Could not register listener {0} on {1} static functions are not supported.", new object[] { method, this.GetType() }));
			}
			if (this.FindMethod(method.Name, targetObj, mode, argumentType) != null)
			{
				return true;
			}
			Debug.LogWarning(String.Format("Could not register listener {0}.{1} on {2} the method could not be found.", new object[] { targetObj, method, this.GetType() }));
			return false;
		}
	}
}