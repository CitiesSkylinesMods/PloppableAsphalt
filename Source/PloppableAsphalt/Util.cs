using System.Reflection;

namespace PloppableAsphalt
{
	public static class Util
	{
		public static Q ReadPrivate<T, Q>(T o, string fieldName)
		{
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo fieldInfo = null;
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo2 in array)
			{
				if (fieldInfo2.Name == fieldName)
				{
					fieldInfo = fieldInfo2;
					break;
				}
			}
			return (Q)fieldInfo.GetValue(o);
		}

		public static void WritePrivate<T, Q>(T o, string fieldName, object value)
		{
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo fieldInfo = null;
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo2 in array)
			{
				if (fieldInfo2.Name == fieldName)
				{
					fieldInfo = fieldInfo2;
					break;
				}
			}
			fieldInfo.SetValue(o, value);
		}
	}
}
