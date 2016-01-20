using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Testity.Unity3D.Events
{
	[Serializable]
	public class PersistentCallGroup
	{
		[FormerlySerializedAs("m_Listeners")]
		[SerializeField]
		private List<PersistentCall> m_Calls;

		public int Count
		{
			get
			{
				return this.m_Calls.Count;
			}
		}

		public PersistentCallGroup()
		{
			this.m_Calls = new List<PersistentCall>();
		}

		public void AddListener()
		{
			this.m_Calls.Add(new PersistentCall());
		}

		public void AddListener(PersistentCall call)
		{
			this.m_Calls.Add(call);
		}

		public void Clear()
		{
			this.m_Calls.Clear();
		}

		public PersistentCall GetListener(int index)
		{
			return this.m_Calls[index];
		}

		public IEnumerable<PersistentCall> GetListeners()
		{
			return this.m_Calls;
		}

		public void Initialize(InvokableCallList invokableList, UnityEventBase unityEventBase)
		{
			foreach (PersistentCall mCall in this.m_Calls)
			{
				if (mCall.IsValid())
				{
					BaseInvokableCall runtimeCall = mCall.GetRuntimeCall(unityEventBase);
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
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Bool;
			listener.arguments.boolArgument = argument;
		}

		public void RegisterEventPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.EventDefined;
		}

		public void RegisterFloatPersistentListener(int index, UnityEngine.Object targetObj, float argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Float;
			listener.arguments.floatArgument = argument;
		}

		public void RegisterIntPersistentListener(int index, UnityEngine.Object targetObj, int argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Int;
			listener.arguments.intArgument = argument;
		}

		public void RegisterObjectPersistentListener(int index, UnityEngine.Object targetObj, UnityEngine.Object argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Object;
			listener.arguments.unityObjectArgument = argument;
		}

		public void RegisterStringPersistentListener(int index, UnityEngine.Object targetObj, string argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.String;
			listener.arguments.stringArgument = argument;
		}

		public void RegisterVoidPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Void;
		}

		public void RemoveListener(int index)
		{
			this.m_Calls.RemoveAt(index);
		}

		public void RemoveListeners(UnityEngine.Object target, string methodName)
		{
			List<PersistentCall> persistentCalls = new List<PersistentCall>();
			for (int i = 0; i < this.m_Calls.Count; i++)
			{
				if (this.m_Calls[i].target == target && this.m_Calls[i].methodName == methodName)
				{
					persistentCalls.Add(this.m_Calls[i]);
				}
			}
			List<PersistentCall> persistentCalls1 = persistentCalls;
			this.m_Calls.RemoveAll(new Predicate<PersistentCall>(persistentCalls1.Contains));
		}

		public void UnregisterPersistentListener(int index)
		{
			this.GetListener(index).UnregisterPersistentListener();
		}
	}
}