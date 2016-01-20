using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Testity.Unity3D.Events
{
	[Serializable]
	public class TestityPersistentCallGroup
	{
		[FormerlySerializedAs("m_Listeners")]
		[SerializeField]
		private List<TestityPersistentCall> m_Calls;

		public int Count
		{
			get
			{
				return this.m_Calls.Count;
			}
		}

		public TestityPersistentCallGroup()
		{
			this.m_Calls = new List<TestityPersistentCall>();
		}

		public void AddListener()
		{
			this.m_Calls.Add(new TestityPersistentCall());
		}

		public void AddListener(TestityPersistentCall call)
		{
			this.m_Calls.Add(call);
		}

		public void Clear()
		{
			this.m_Calls.Clear();
		}

		public TestityPersistentCall GetListener(int index)
		{
			return this.m_Calls[index];
		}

		public IEnumerable<TestityPersistentCall> GetListeners()
		{
			return this.m_Calls;
		}

		public void Initialize(TestityInvokableCallList invokableList, TestityEventBase unityEventBase)
		{
			foreach (TestityPersistentCall mCall in this.m_Calls)
			{
				if (mCall.IsValid())
				{
					TestityBaseInvokableCall runtimeCall = mCall.GetRuntimeCall(unityEventBase);
					if (runtimeCall == null)
					{
						continue;
					}
					invokableList.AddPersistentInvokableCall(runtimeCall);
				}
			}
		}

		public void RegisterBoolPersistentListener(int index, UnityEngine.Object targetObj, bool argument, string methodName)
		{
			TestityPersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = TestityPersistentListenerMode.Bool;
			listener.arguments.boolArgument = argument;
		}

		public void RegisterEventPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			TestityPersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = TestityPersistentListenerMode.EventDefined;
		}

		public void RegisterFloatPersistentListener(int index, UnityEngine.Object targetObj, float argument, string methodName)
		{
			TestityPersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = TestityPersistentListenerMode.Float;
			listener.arguments.floatArgument = argument;
		}

		public void RegisterIntPersistentListener(int index, UnityEngine.Object targetObj, int argument, string methodName)
		{
			TestityPersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = TestityPersistentListenerMode.Int;
			listener.arguments.intArgument = argument;
		}

		public void RegisterObjectPersistentListener(int index, UnityEngine.Object targetObj, UnityEngine.Object argument, string methodName)
		{
			TestityPersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = TestityPersistentListenerMode.Object;
			listener.arguments.unityObjectArgument = argument;
		}

		public void RegisterStringPersistentListener(int index, UnityEngine.Object targetObj, string argument, string methodName)
		{
			TestityPersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = TestityPersistentListenerMode.String;
			listener.arguments.stringArgument = argument;
		}

		public void RegisterVoidPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			TestityPersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = TestityPersistentListenerMode.Void;
		}

		public void RemoveListener(int index)
		{
			this.m_Calls.RemoveAt(index);
		}

		public void RemoveListeners(UnityEngine.Object target, string methodName)
		{
			List<TestityPersistentCall> persistentCalls = new List<TestityPersistentCall>();
			for (int i = 0; i < this.m_Calls.Count; i++)
			{
				if (this.m_Calls[i].target == target && this.m_Calls[i].methodName == methodName)
				{
					persistentCalls.Add(this.m_Calls[i]);
				}
			}
			List<TestityPersistentCall> persistentCalls1 = persistentCalls;
			this.m_Calls.RemoveAll(new Predicate<TestityPersistentCall>(persistentCalls1.Contains));
		}

		public void UnregisterPersistentListener(int index)
		{
			this.GetListener(index).UnregisterPersistentListener();
		}
	}
}