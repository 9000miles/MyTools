namespace Sirenix.OdinInspector
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DelayedPropertyAttribute : Attribute
	{ }
}
