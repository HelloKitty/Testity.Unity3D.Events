using System;
using System.Reflection;
using Testity.EngineComponents.Unity3D;

namespace Testity.Unity3D.Events
{
	public class TestityInvokableCall : TestityBaseInvokableCall
	{
		public TestityInvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			//We need to check if it's a TestityBehaviour first
			if (CheckIsTestityTarget(target))
			{
				//if it is we need to unbox the EngineScriptComponent reference from the TestityBehaviour
				target = UnboxTestityComponentFromObject(target);
			}				

			this.Delegate += (TestityAction)theFunction.CreateDelegate(Type.GetTypeFromHandle(typeof(TestityAction).TypeHandle), target);
		}

		public TestityInvokableCall(TestityAction action)
		{
			this.Delegate += action;
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return (this.Delegate.Target != targetObj ? false : this.Delegate.GetMethodInfo() == method);
		}

		public override void Invoke(object[] args)
		{
			if (TestityBaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate();
			}
		}

		private event TestityAction Delegate;
	}

    public class InvokableCall<T1> : TestityBaseInvokableCall
    {
        public InvokableCall(object target, MethodInfo theFunction) 
			: base(target, theFunction)
        {
			//We need to check if it's a TestityBehaviour first
			if (CheckIsTestityTarget(target))
			{
				//if it is we need to unbox the EngineScriptComponent reference from the TestityBehaviour
				target = UnboxTestityComponentFromObject(target);
			}

			this.Delegate += (TestityAction<T1>)theFunction.CreateDelegate(Type.GetTypeFromHandle(typeof(TestityAction<T1>).TypeHandle), target);
        }

        public InvokableCall(TestityAction<T1> action)
        {
            this.Delegate += action;
        }

        public override bool Find(object targetObj, MethodInfo method)
        {
            return (this.Delegate.Target != targetObj ? false : this.Delegate.GetMethodInfo() == method);
        }

        public override void Invoke(object[] args)
        {
            if ((int)args.Length != 1)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            TestityBaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            if (TestityBaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0]);
            }
        }

		protected event TestityAction<T1> Delegate;
    }

    public class InvokableCall<T1, T2> : TestityBaseInvokableCall
    {
        public InvokableCall(object target, MethodInfo theFunction) 
			: base(target, theFunction)
        {
			//We need to check if it's a TestityBehaviour first
			if (CheckIsTestityTarget(target))
			{
				//if it is we need to unbox the EngineScriptComponent reference from the TestityBehaviour
				target = UnboxTestityComponentFromObject(target);
			}

			this.Delegate = (TesityAction<T1, T2>)theFunction.CreateDelegate(typeof(TesityAction<T1, T2>), target);
        }

        public InvokableCall(TesityAction<T1, T2> action)
        {
            this.Delegate += action;
        }

        public override bool Find(object targetObj, MethodInfo method)
        {
            return (this.Delegate.Target != targetObj ? false : this.Delegate.GetMethodInfo() == method);
        }

        public override void Invoke(object[] args)
        {
            if ((int)args.Length != 2)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            TestityBaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            TestityBaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            if (TestityBaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0], (T2)args[1]);
            }
        }

		protected event TesityAction<T1, T2> Delegate;
    }

    public class InvokableCall<T1, T2, T3> : TestityBaseInvokableCall
    {
        public InvokableCall(object target, MethodInfo theFunction) 
			: base(target, theFunction)
        {
			//We need to check if it's a TestityBehaviour first
			if (CheckIsTestityTarget(target))
			{
				//if it is we need to unbox the EngineScriptComponent reference from the TestityBehaviour
				target = UnboxTestityComponentFromObject(target);
			}

			this.Delegate = (TestityAction<T1, T2, T3>)theFunction.CreateDelegate(typeof(TestityAction<T1, T2, T3>), target);
        }

        public InvokableCall(TestityAction<T1, T2, T3> action)
        {
            this.Delegate += action;
        }

        public override bool Find(object targetObj, MethodInfo method)
        {
            return (this.Delegate.Target != targetObj ? false : this.Delegate.GetMethodInfo() == method);
        }

        public override void Invoke(object[] args)
        {
            if ((int)args.Length != 3)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            TestityBaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            TestityBaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            TestityBaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
            if (TestityBaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0], (T2)args[1], (T3)args[2]);
            }
        }

		protected event TestityAction<T1, T2, T3> Delegate;
    }

    public class InvokableCall<T1, T2, T3, T4> : TestityBaseInvokableCall
    {

        public InvokableCall(object target, MethodInfo theFunction) 
			: base(target, theFunction)
        {
			//We need to check if it's a TestityBehaviour first
			if (CheckIsTestityTarget(target))
			{
				//if it is we need to unbox the EngineScriptComponent reference from the TestityBehaviour
				target = UnboxTestityComponentFromObject(target);
			}

			this.Delegate = (TestityAction<T1, T2, T3, T4>)theFunction.CreateDelegate(typeof(TestityAction<T1, T2, T3, T4>), target);
        }

        public InvokableCall(TestityAction<T1, T2, T3, T4> action)
        {
            this.Delegate += action;
        }

        public override bool Find(object targetObj, MethodInfo method)
        {
            return (this.Delegate.Target != targetObj ? false : this.Delegate.GetMethodInfo() == method);
        }

        public override void Invoke(object[] args)
        {
            if ((int)args.Length != 4)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            TestityBaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            TestityBaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            TestityBaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
            TestityBaseInvokableCall.ThrowOnInvalidArg<T4>(args[3]);
            if (TestityBaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
            }
        }

		protected event TestityAction<T1, T2, T3, T4> Delegate;
    }
}