using System;
using System.Reflection;
using UnityEngine;

namespace Testity.Unity3D.Events
{
	public class CachedInvokableCall<T> : InvokableCall<T>
	{
		private readonly object[] m_Arg1;

		public CachedInvokableCall(UnityEngine.Object target, MethodInfo theFunction, T argument)
			: base(target, theFunction)
		{
			this.m_Arg1[0] = argument;
		}

		public override void Invoke(object[] args)
		{
			base.Invoke(this.m_Arg1);
		}
	}
}