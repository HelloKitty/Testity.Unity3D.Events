using System;
using System.Reflection;
using UnityEngine;

namespace Testity.Unity3D.Events
{
	/// <summary>
	///   <para>A zero argument persistent callback that can be saved with the scene.</para>
	/// </summary>
	[Serializable]
	public class UnityEvent : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[0];

		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UnityEvent()
		{
		}

		/// <summary>
		///   <para>Add a non persistent listener to the UnityEvent.</para>
		/// </summary>
		/// <param name="call">Callback function.</param>
		public void AddListener(UnityAction call)
		{
			base.AddCall(UnityEvent.GetDelegate(call));
		}

		public void AddPersistentListener(UnityAction call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(UnityAction call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[0]);
		}

		public override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction action)
		{
			return new InvokableCall(action);
		}

		/// <summary>
		///   <para>Invoke all registered callbacks (runtime and peristent).</para>
		/// </summary>
		public void Invoke()
		{
			base.Invoke(this.m_InvokeArray);
		}

		public void RegisterPersistentListener(int index, UnityAction call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		/// <summary>
		///   <para>Remove a non persistent listener from the UnityEvent.</para>
		/// </summary>
		/// <param name="call">Callback function.</param>
		public void RemoveListener(UnityAction call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>One argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class UnityEvent<T0> : UnityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0> call)
		{
			base.AddCall(UnityEvent<T0>.GetDelegate(call));
		}

		public void AddPersistentListener(UnityAction<T0> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(UnityAction<T0> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0) });
		}

		public override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0> action)
		{
			return new InvokableCall<T0>(action);
		}

		public void Invoke(T0 arg0)
		{
			this.m_InvokeArray[0] = arg0;
			base.Invoke(this.m_InvokeArray);
		}

		public void RegisterPersistentListener(int index, UnityAction<T0> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(UnityAction<T0> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>Two argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class UnityEvent<T0, T1> : UnityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0, T1> call)
		{
			base.AddCall(UnityEvent<T0, T1>.GetDelegate(call));
		}

		public void AddPersistentListener(UnityAction<T0, T1> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(UnityAction<T0, T1> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0), typeof(T1) });
		}

		public override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1> action)
		{
			return new InvokableCall<T0, T1>(action);
		}

		public void Invoke(T0 arg0, T1 arg1)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			base.Invoke(this.m_InvokeArray);
		}

		public void RegisterPersistentListener(int index, UnityAction<T0, T1> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(UnityAction<T0, T1> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>Three argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class UnityEvent<T0, T1, T2> : UnityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0, T1, T2> call)
		{
			base.AddCall(UnityEvent<T0, T1, T2>.GetDelegate(call));
		}

		public void AddPersistentListener(UnityAction<T0, T1, T2> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(UnityAction<T0, T1, T2> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0), typeof(T1), typeof(T2) });
		}

		public override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2> action)
		{
			return new InvokableCall<T0, T1, T2>(action);
		}

		public void Invoke(T0 arg0, T1 arg1, T2 arg2)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			this.m_InvokeArray[2] = arg2;
			base.Invoke(this.m_InvokeArray);
		}

		public void RegisterPersistentListener(int index, UnityAction<T0, T1, T2> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(UnityAction<T0, T1, T2> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>Four argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class UnityEvent<T0, T1, T2, T3> : UnityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0, T1, T2, T3> call)
		{
			base.AddCall(UnityEvent<T0, T1, T2, T3>.GetDelegate(call));
		}

		public void AddPersistentListener(UnityAction<T0, T1, T2, T3> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(UnityAction<T0, T1, T2, T3> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) });
		}

		public override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2, T3>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3> action)
		{
			return new InvokableCall<T0, T1, T2, T3>(action);
		}

		public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			this.m_InvokeArray[2] = arg2;
			this.m_InvokeArray[3] = arg3;
			base.Invoke(this.m_InvokeArray);
		}

		public void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(UnityAction<T0, T1, T2, T3> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}
}