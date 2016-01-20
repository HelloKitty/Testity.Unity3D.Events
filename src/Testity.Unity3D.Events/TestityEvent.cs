using System;
using System.Reflection;
using UnityEngine;

namespace Testity.Unity3D.Events
{
	/// <summary>
	///   <para>A zero argument persistent callback that can be saved with the scene.</para>
	/// </summary>
	[Serializable]
	public class TestityEvent : TestityEventBase
	{
		private readonly object[] m_InvokeArray = new object[0];

		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public TestityEvent()
		{
		}

		/// <summary>
		///   <para>Add a non persistent listener to the UnityEvent.</para>
		/// </summary>
		/// <param name="call">Callback function.</param>
		public void AddListener(TestityAction call)
		{
			base.AddCall(TestityEvent.GetDelegate(call));
		}

		public void AddPersistentListener(TestityAction call)
		{
			this.AddPersistentListener(call, TestityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(TestityAction call, TestityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return TestityEventBase.GetValidMethodInfo(targetObj, name, new Type[0]);
		}

		public override TestityBaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new TestityInvokableCall(target, theFunction);
		}

		private static TestityBaseInvokableCall GetDelegate(TestityAction action)
		{
			return new TestityInvokableCall(action);
		}

		/// <summary>
		///   <para>Invoke all registered callbacks (runtime and peristent).</para>
		/// </summary>
		public void Invoke()
		{
			base.Invoke(this.m_InvokeArray);
		}

		public void RegisterPersistentListener(int index, TestityAction call)
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
		public void RemoveListener(TestityAction call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>One argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class TestityEvent<T0> : TestityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected TestityEvent()
		{
			if (m_InvokeArray == null)
				m_InvokeArray = new object[1];
        }

		public void AddListener(TestityAction<T0> call)
		{
			base.AddCall(TestityEvent<T0>.GetDelegate(call));
		}

		public void AddPersistentListener(TestityAction<T0> call)
		{
			this.AddPersistentListener(call, TestityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(TestityAction<T0> call, TestityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return TestityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0) });
		}

		public override TestityBaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			//Debug.Log("Target Type: " + target.GetType().ToString() + " method name " + theFunction.Name);

			return new InvokableCall<T0>(target, theFunction);
		}

		private static TestityBaseInvokableCall GetDelegate(TestityAction<T0> action)
		{
			return new InvokableCall<T0>(action);
		}

		public void Invoke(T0 arg0)
		{
			try
			{
				this.m_InvokeArray[0] = arg0;
				base.Invoke(this.m_InvokeArray);
			}
			catch(NullReferenceException e)
			{
				Debug.Log(e.Message + " " + e.StackTrace);
				throw;
			}
		}

		public void RegisterPersistentListener(int index, TestityAction<T0> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(TestityAction<T0> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>Two argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class TestityEvent<T0, T1> : TestityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected TestityEvent()
		{
			if (m_InvokeArray == null)
				m_InvokeArray = new object[2];
		}

		public void AddListener(TesityAction<T0, T1> call)
		{
			base.AddCall(TestityEvent<T0, T1>.GetDelegate(call));
		}

		public void AddPersistentListener(TesityAction<T0, T1> call)
		{
			this.AddPersistentListener(call, TestityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(TesityAction<T0, T1> call, TestityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return TestityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0), typeof(T1) });
		}

		public override TestityBaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1>(target, theFunction);
		}

		private static TestityBaseInvokableCall GetDelegate(TesityAction<T0, T1> action)
		{
			return new InvokableCall<T0, T1>(action);
		}

		public void Invoke(T0 arg0, T1 arg1)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			base.Invoke(this.m_InvokeArray);
		}

		public void RegisterPersistentListener(int index, TesityAction<T0, T1> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(TesityAction<T0, T1> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>Three argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class TestityEvent<T0, T1, T2> : TestityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected TestityEvent()
		{
			if (m_InvokeArray == null)
				m_InvokeArray = new object[3];
		}

		public void AddListener(TestityAction<T0, T1, T2> call)
		{
			base.AddCall(TestityEvent<T0, T1, T2>.GetDelegate(call));
		}

		public void AddPersistentListener(TestityAction<T0, T1, T2> call)
		{
			this.AddPersistentListener(call, TestityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(TestityAction<T0, T1, T2> call, TestityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return TestityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0), typeof(T1), typeof(T2) });
		}

		public override TestityBaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2>(target, theFunction);
		}

		private static TestityBaseInvokableCall GetDelegate(TestityAction<T0, T1, T2> action)
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

		public void RegisterPersistentListener(int index, TestityAction<T0, T1, T2> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(TestityAction<T0, T1, T2> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}

	/// <summary>
	///   <para>Four argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class TestityEvent<T0, T1, T2, T3> : TestityEventBase
	{
		private readonly object[] m_InvokeArray;

		protected TestityEvent()
		{
			if (m_InvokeArray == null)
				m_InvokeArray = new object[4];
		}

		public void AddListener(TestityAction<T0, T1, T2, T3> call)
		{
			base.AddCall(TestityEvent<T0, T1, T2, T3>.GetDelegate(call));
		}

		public void AddPersistentListener(TestityAction<T0, T1, T2, T3> call)
		{
			this.AddPersistentListener(call, TestityEventCallState.RuntimeOnly);
		}

		public void AddPersistentListener(TestityAction<T0, T1, T2, T3> call, TestityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return TestityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) });
		}

		public override TestityBaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2, T3>(target, theFunction);
		}

		private static TestityBaseInvokableCall GetDelegate(TestityAction<T0, T1, T2, T3> action)
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

		public void RegisterPersistentListener(int index, TestityAction<T0, T1, T2, T3> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}

		public void RemoveListener(TestityAction<T0, T1, T2, T3> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}
	}
}