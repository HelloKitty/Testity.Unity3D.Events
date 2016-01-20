using System;
using System.Collections.Generic;
using System.Reflection;

namespace Testity.Unity3D.Events
{
	public class TestityInvokableCallList
	{
		private readonly List<TestityBaseInvokableCall> m_PersistentCalls = new List<TestityBaseInvokableCall>();

		private readonly List<TestityBaseInvokableCall> m_RuntimeCalls = new List<TestityBaseInvokableCall>();

		private readonly List<TestityBaseInvokableCall> m_ExecutingCalls = new List<TestityBaseInvokableCall>();

		private bool m_NeedsUpdate = true;

		public int Count
		{
			get
			{
				return this.m_PersistentCalls.Count + this.m_RuntimeCalls.Count;
			}
		}

		public TestityInvokableCallList()
		{
		}

		public void AddListener(TestityBaseInvokableCall call)
		{
			this.m_RuntimeCalls.Add(call);
			this.m_NeedsUpdate = true;
		}

		public void AddPersistentInvokableCall(TestityBaseInvokableCall call)
		{
			this.m_PersistentCalls.Add(call);
			this.m_NeedsUpdate = true;
		}

		public void Clear()
		{
			this.m_RuntimeCalls.Clear();
			this.m_NeedsUpdate = true;
		}

		public void ClearPersistent()
		{
			this.m_PersistentCalls.Clear();
			this.m_NeedsUpdate = true;
		}

		public void Invoke(object[] parameters)
		{
			if (this.m_NeedsUpdate)
			{
				this.m_ExecutingCalls.Clear();
				this.m_ExecutingCalls.AddRange(this.m_PersistentCalls);
				this.m_ExecutingCalls.AddRange(this.m_RuntimeCalls);
				this.m_NeedsUpdate = false;
			}
			for (int i = 0; i < this.m_ExecutingCalls.Count; i++)
			{
				this.m_ExecutingCalls[i].Invoke(parameters);
			}
		}

		public void RemoveListener(object targetObj, MethodInfo method)
		{
			List<TestityBaseInvokableCall> baseInvokableCalls = new List<TestityBaseInvokableCall>();
			for (int i = 0; i < this.m_RuntimeCalls.Count; i++)
			{
				if (this.m_RuntimeCalls[i].Find(targetObj, method))
				{
					baseInvokableCalls.Add(this.m_RuntimeCalls[i]);
				}
			}
			List<TestityBaseInvokableCall> baseInvokableCalls1 = baseInvokableCalls;
			this.m_RuntimeCalls.RemoveAll(new Predicate<TestityBaseInvokableCall>(baseInvokableCalls1.Contains));
			this.m_NeedsUpdate = true;
		}
	}
}