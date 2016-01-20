using System;
using System.Reflection;
using Testity.EngineComponents.Unity3D;
using UnityEngine;

namespace Testity.Unity3D.Events
{
	public abstract class TestityBaseInvokableCall
	{
		protected TestityBaseInvokableCall()
		{
		}

		protected TestityBaseInvokableCall(object target, MethodInfo function)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (function == null)
			{
				throw new ArgumentNullException("function");
			}


		}

		protected bool CheckIsTestityTarget(object target)
		{
			return typeof(ITestityBehaviour).IsAssignableFrom(target.GetType());
        }

		protected static bool AllowInvoke(Delegate @delegate)
		{
			object target = @delegate.Target;
			if (target == null)
			{
				return true;
			}

			UnityEngine.Object obj = target as UnityEngine.Object;
			if (object.ReferenceEquals(obj, null))
			{
				return true;
			}
			return obj != null;
		}

		public abstract bool Find(object targetObj, MethodInfo method);

		public abstract void Invoke(object[] args);

		protected static void ThrowOnInvalidArg<T>(object arg)
		{
			if (arg != null && !(arg is T))
			{
				throw new ArgumentException(String.Format("Passed argument 'args[0]' is of the wrong type. Type:{0} Expected:{1}", new object[] { arg.GetType(), typeof(T) }));
			}
		}
	}
}