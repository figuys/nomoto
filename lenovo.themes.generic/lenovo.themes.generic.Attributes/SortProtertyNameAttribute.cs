using System;

namespace lenovo.themes.generic.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class SortProtertyNameAttribute : Attribute
{
	public string SortPropertyName { get; set; }

	public string SortMemberPath { get; set; }
}
