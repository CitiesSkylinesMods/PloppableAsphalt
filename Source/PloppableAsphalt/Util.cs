namespace PloppableAsphalt
{
    using System.Reflection;

    public static class Util
	{
		public static Q ReadPrivate<T, Q>(T o, string fieldName)
		{
			var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo fieldInfo = null;
			var array = fields;
			foreach (var fieldInfo2 in array)
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
			var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo fieldInfo = null;
			var array = fields;
			foreach (var fieldInfo2 in array)
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
