using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents.Unity3D;
using Testity.Unity3D.Events;

//can't have a namespace because it must be referenced in generated code
public static class TestityEventToEngineTypeExtensions
{ 
	public static Action ToEngineType(this TestityEvent e)
	{
		//if the event is not null and has actual listeners
		if (e != null && e.GetPersistentEventCount() != 0)
			return new Action(() => e.Invoke());
		else
			return null; //a null action works better than invoking the slow and empty TestityEvent
	}

	public static Action<T0> ToEngineType<T0>(this TestityEvent<T0> e)
	{
		//if the event is not null and has actual listeners
		if (e != null && e.GetPersistentEventCount() != 0)
			return new Action<T0>(arg0 => e.Invoke(arg0));
		else
			return null; //a null action works better than invoking the slow and empty TestityEvent
	}

	public static Action<T0, T1> ToEngineType<T0, T1>(this TestityEvent<T0, T1> e)
	{
		//if the event is not null and has actual listeners
		if (e != null && e.GetPersistentEventCount() != 0)
			return new Action<T0, T1>((arg0, arg1) => e.Invoke(arg0, arg1));
		else
			return null; //a null action works better than invoking the slow and empty TestityEvent
	}

	public static Action<T0, T1, T2> ToEngineType<T0, T1, T2>(this TestityEvent<T0, T1, T2> e)
	{
		//if the event is not null and has actual listeners
		if (e != null && e.GetPersistentEventCount() != 0)
			return new Action<T0, T1, T2>((arg0, arg1, arg2) => e.Invoke(arg0, arg1, arg2));
		else
			return null; //a null action works better than invoking the slow and empty TestityEvent
	}

	public static Action<T0, T1, T2, T3> ToEngineType<T0, T1, T2, T3>(this TestityEvent<T0, T1, T2, T3> e)
	{
		//if the event is not null and has actual listeners
		if (e != null && e.GetPersistentEventCount() != 0)
			return new Action<T0, T1, T2, T3>((arg0, arg1, arg2, arg3) => e.Invoke(arg0, arg1, arg2, arg3));
		else
			return null; //a null action works better than invoking the slow and empty TestityEvent
	}
}

