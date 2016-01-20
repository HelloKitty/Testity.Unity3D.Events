using System;
using System.Reflection;

namespace Testity.Unity3D.Events
{
	public class InvokableCall : BaseInvokableCall
	{
		public InvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			this.Delegate += (UnityAction)theFunction.CreateDelegate(Type.GetTypeFromHandle(typeof(UnityAction).TypeHandle), target);
		}

		public InvokableCall(UnityAction action)
		{
			this.Delegate += action;
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return (this.Delegate.Target != targetObj ? false : this.Delegate.GetMethodInfo() == method);
		}

		public override void Invoke(object[] args)
		{
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate();
			}
		}

		private event UnityAction Delegate;
	}

    public class InvokableCall<T1> : BaseInvokableCall
    {
        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate += (UnityAction<T1>)theFunction.CreateDelegate(Type.GetTypeFromHandle(typeof(UnityAction<T1>).TypeHandle), target);
        }

        public InvokableCall(UnityAction<T1> action)
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
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0]);
            }
        }

		protected event UnityAction<T1> Delegate;
    }

    public class InvokableCall<T1, T2> : BaseInvokableCall
    {
        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate = (UnityAction<T1, T2>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2>), target);
        }

        public InvokableCall(UnityAction<T1, T2> action)
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
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0], (T2)args[1]);
            }
        }

		protected event UnityAction<T1, T2> Delegate;
    }

    public class InvokableCall<T1, T2, T3> : BaseInvokableCall
    {
        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate = (UnityAction<T1, T2, T3>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3>), target);
        }

        public InvokableCall(UnityAction<T1, T2, T3> action)
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
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0], (T2)args[1], (T3)args[2]);
            }
        }

		protected event UnityAction<T1, T2, T3> Delegate;
    }

    public class InvokableCall<T1, T2, T3, T4> : BaseInvokableCall
    {

        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate = (UnityAction<T1, T2, T3, T4>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3, T4>), target);
        }

        public InvokableCall(UnityAction<T1, T2, T3, T4> action)
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
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
            BaseInvokableCall.ThrowOnInvalidArg<T4>(args[3]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
            }
        }

		protected event UnityAction<T1, T2, T3, T4> Delegate;
    }
}