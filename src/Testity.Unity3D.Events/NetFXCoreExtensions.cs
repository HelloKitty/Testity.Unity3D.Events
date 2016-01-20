using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Testity.Unity3D.Events
{
	public static class NetFxCoreExtensions
	{
		public static Delegate CreateDelegate(this MethodInfo self, Type delegateType, object target)
		{
			return Delegate.CreateDelegate(delegateType, target, self);
		}

		public static MethodInfo GetMethodInfo(this Delegate self)
		{
			return self.Method;
		}
	}
}
