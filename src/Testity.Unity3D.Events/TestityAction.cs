using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testity.Unity3D.Events
{
	/// <summary>
	///   <para>Zero argument delegate used by UnityEvents.</para>
	/// </summary>
	public delegate void TestityAction();

	public delegate void TestityAction<T0>(T0 arg0);

	public delegate void TesityAction<T0, T1>(T0 arg0, T1 arg1);

	public delegate void TestityAction<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);

	public delegate void TestityAction<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);

}
