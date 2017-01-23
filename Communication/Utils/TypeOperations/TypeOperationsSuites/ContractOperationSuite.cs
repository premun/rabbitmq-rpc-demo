using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for specified contract type.
	/// </summary>
	/// <typeparam name="T">Type of contract</typeparam>
	public class ContractOperationSuite<T> : BaseTypeOperationsSuite<T>
	{
		/// <summary>
		/// Contract property description.
		/// Allows get or set the property.
		/// Is used to handle with properties with different signature thus the getter and setter use object type.
		/// </summary>
		public interface IProperty
		{
			object Getter(object instance);

			void Setter(object instance, object input);

			PropertyInfo PropertyInfo { get; set; }
		}

		/// <summary>
		/// Generic implementation of IProperty. Contains contract property description.
		/// Allows get or set the property. 
		/// </summary>
		/// <typeparam name="TInstance">Type of contract instance.</typeparam>
		/// <typeparam name="TValue">Type of describing property.</typeparam>
		public class Property<TInstance, TValue> : IProperty
		{
			public PropertyInfo PropertyInfo { get; set; }

			public Func<TInstance, TValue> Get { get; set; }

			public Action<TInstance, TValue> Set { get; set; }

			public object Getter(object input)
			{
				return Get((TInstance)input);
			}

			public void Setter(object instance, object input)
			{
				Set((TInstance)instance, (TValue)input);
			}
		}
			
		/// <summary>
		/// Configure contract suite through reflection. Specified properties which will be used in operations. 
		/// </summary>
		public class ContractSuiteConfig
		{
			private readonly List<IProperty> _properties = new List<IProperty>();

			public void RegisterProperty<TY>(Expression<Func<T, TY>> selector)
			{
				var newValue = Expression.Parameter(selector.Body.Type);
				var assign = Expression.Lambda<Action<T, TY>>(
					Expression.Assign(selector.Body, newValue),
					selector.Parameters[0], 
					newValue);

				var getter = selector.Compile();
				var setter = assign.Compile();

				_properties.Add(new Property<T, TY>
				{
					Set = setter,
					Get = getter,
					PropertyInfo = (PropertyInfo)((MemberExpression)selector.Body).Member
				});
			}

			public List<IProperty> SelectedProperties
			{
				get
				{
					return _properties;
				}
			}
		}

		private readonly List<IProperty> _propeties;

		public ContractOperationSuite(Action<ContractSuiteConfig> config)
		{
			var configInstance = new ContractSuiteConfig();
			config(configInstance);
			_propeties = configInstance.SelectedProperties;
		}
			
		#region Overrides of BaseTypeOperationsSuite<T>

		
		public override bool AreEqual(T first, T second)
		{
			bool areEqual = true;

			foreach (var propertyInfo in _propeties)
			{
				var val1 = propertyInfo.Getter(first);
				var val2 = propertyInfo.Getter(second);
				areEqual = areEqual && TypeOperationsService.AreEqual(val1, val2);
			}

			return areEqual;
		}

		public override T GenerateTyped()
		{
			var instance = Activator.CreateInstance<T>();
			foreach (var property in _propeties)
			{
				property.Setter(instance, TypeOperationsService.Generate(property.PropertyInfo.PropertyType));
			}

			return instance;
		}
		#endregion
	}
}
