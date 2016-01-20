using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Testity.Unity3D.Events
{
	[Serializable]
	public class TestityPersistentCall
	{
		[FormerlySerializedAs("instance")]
		[SerializeField]
		private UnityEngine.Object m_Target;

		[FormerlySerializedAs("methodName")]
		[SerializeField]
		private string m_MethodName;

		[FormerlySerializedAs("mode")]
		[SerializeField]
		private TestityPersistentListenerMode m_Mode;

		[FormerlySerializedAs("arguments")]
		[SerializeField]
		private TestityArgumentCache m_Arguments = new TestityArgumentCache();

		[FormerlySerializedAs("enabled")]
		[FormerlySerializedAs("m_Enabled")]
		[SerializeField]
		private TestityEventCallState m_CallState = TestityEventCallState.RuntimeOnly;

		public TestityArgumentCache arguments
		{
			get
			{
				return this.m_Arguments;
			}
		}

		public TestityEventCallState callState
		{
			get
			{
				return this.m_CallState;
			}
			set
			{
				this.m_CallState = value;
			}
		}

		public string methodName
		{
			get
			{
				return this.m_MethodName;
			}
		}

		public TestityPersistentListenerMode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				this.m_Mode = value;
			}
		}

		public UnityEngine.Object target
		{
			get
			{
				return this.m_Target;
			}
		}

		public TestityPersistentCall()
		{
		}

		private static TestityBaseInvokableCall GetObjectCall(UnityEngine.Object target, MethodInfo method, TestityArgumentCache arguments)
		{
			Type type = typeof(UnityEngine.Object);
			if (!string.IsNullOrEmpty(arguments.unityObjectArgumentAssemblyTypeName))
			{
				type = Type.GetType(arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(UnityEngine.Object);

				//Debug.Log("Calling on Type: " + type.FullName + " method name " + method.Name);
            }
			Type type1 = typeof(TestityCachedInvokableCall<>).MakeGenericType(new Type[] { type });
			ConstructorInfo constructor = type1.GetConstructor(new Type[] { typeof(UnityEngine.Object), typeof(MethodInfo), type });

			UnityEngine.Object obj = arguments.unityObjectArgument;
			if (obj != null && !type.IsAssignableFrom(obj.GetType()))
			{
				obj = null;
			}
			return constructor.Invoke(new object[] { target, method, obj }) as TestityBaseInvokableCall;
		}

		public TestityBaseInvokableCall GetRuntimeCall(TestityEventBase theEvent)
		{
			if (this.m_CallState == TestityEventCallState.RuntimeOnly && !Application.isPlaying)
			{
				return null;
			}
			if (this.m_CallState == TestityEventCallState.Off || theEvent == null)
			{
				return null;
			}

			MethodInfo methodInfo = theEvent.FindMethod(this);

			if (methodInfo == null)
			{
				return null;
			}
			switch (this.m_Mode)
			{
				case TestityPersistentListenerMode.EventDefined:
					{
						return theEvent.GetDelegate(this.target, methodInfo);
					}
				case TestityPersistentListenerMode.Void:
					{
						return new TestityInvokableCall(this.target, methodInfo);
					}
				case TestityPersistentListenerMode.Object:
					{
						return TestityPersistentCall.GetObjectCall(this.target, methodInfo, this.m_Arguments);
					}
				case TestityPersistentListenerMode.Int:
					{
						return new TestityCachedInvokableCall<int>(this.target, methodInfo, this.m_Arguments.intArgument);
					}
				case TestityPersistentListenerMode.Float:
					{
						return new TestityCachedInvokableCall<float>(this.target, methodInfo, this.m_Arguments.floatArgument);
					}
				case TestityPersistentListenerMode.String:
					{
						return new TestityCachedInvokableCall<string>(this.target, methodInfo, this.m_Arguments.stringArgument);
					}
				case TestityPersistentListenerMode.Bool:
					{
						return new TestityCachedInvokableCall<bool>(this.target, methodInfo, this.m_Arguments.boolArgument);
					}
			}
			return null;
		}

		public bool IsValid()
		{
			return (this.target == null ? false : !string.IsNullOrEmpty(this.methodName));
		}

		public void RegisterPersistentListener(UnityEngine.Object ttarget, string mmethodName)
		{
			this.m_Target = ttarget;
			this.m_MethodName = mmethodName;
		}

		public void UnregisterPersistentListener()
		{
			this.m_MethodName = string.Empty;
			this.m_Target = null;
		}
	}
}