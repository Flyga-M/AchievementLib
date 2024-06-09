using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AchievementLib.Pack.PersistantData
{
    /// <summary>
    /// Utility functions for <see cref="Attribute"/>s.
    /// </summary>
    public static class AttributeUtil
    {
        /// <summary>
        /// Returns the <see cref="Attribute"/> of the given targetType <typeparamref name="T"/> on the <paramref name="object"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns>The <see cref="Attribute"/> of the given targetType <typeparamref name="T"/> on the <paramref name="object"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the <paramref name="object"/> has no 
        /// <see cref="Attribute"/> of the given targetType, or more than one.</exception>
        public static T GetAttribute<T>(object @object) where T : Attribute
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }
            
            Type type = @object.GetType();

            Attribute[] customAttributes;

            if (@object is MemberInfo memberInfo)
            {
                customAttributes = Attribute.GetCustomAttributes(memberInfo, typeof(T));
            }
            else
            {
                customAttributes = Attribute.GetCustomAttributes(type, typeof(T));
            }

            if (customAttributes == null || !customAttributes.Any())
            {
                throw new InvalidOperationException($"Given object of targetType {type} has no attribute of targetType {typeof(T)}.");
            }

            if (customAttributes.Length > 1)
            {
                throw new InvalidOperationException($"Given object of targetType {type} has more than one attribute of targetType {typeof(T)}.");
            }

            return customAttributes.First() as T;
        }

        /// <summary>
        /// Returns the <see cref="Attribute"/>s of the given targetType <typeparamref name="T"/> on the <paramref name="object"/>s 
        /// properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns>The <see cref="Attribute"/>s of the given targetType <typeparamref name="T"/> on the <paramref name="object"/>s 
        /// properties.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If one of the <paramref name="object"/>s property has more than one
        /// <see cref="Attribute"/> of the given targetType.</exception>
        public static (string Name, T Attribute, Type Type, object Value)[] GetPropertyAttributes<T> (object @object) where T : Attribute
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            Type type = @object.GetType();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            List<(string Name, T Attribute, Type Type, object Value)> result = new List<(string Name, T Attribute, Type Type, object Value)>();

            foreach(PropertyInfo property in properties)
            {
                if (!property.IsDefined(typeof(T)))
                {
                    continue;
                }

                T attribute = GetAttribute<T>(property);

                result.Add((property.Name, attribute, property.PropertyType, property.GetValue(@object)));
            }

            return result.ToArray();
        }

        /// <remarks>
        /// Should be wrapped in a general try/catch. May throw a load of different exceptions from the reflection.
        /// </remarks>
        public static void SetPropertyValue(object @object, string propertyName, Type targetType, object value)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.GetType() != targetType)
            {
                value = Convert.ChangeType(value, targetType);
            }

            Type objectType = @object.GetType();

            PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (PropertyInfo property in properties)
            {
                if (property.Name != propertyName)
                {
                    continue;
                }
                
                MethodInfo setMethod = property.GetSetMethod(nonPublic: true);

                if (setMethod == null)
                {
                    throw new InvalidOperationException($"Unable to set property value with name {propertyName} on object of type " +
                        $"{objectType} to {value}. Property has no accessible setter.");
                }

                setMethod.Invoke(@object, new object[] { value });
                return;
            }

            throw new InvalidOperationException($"Unable to set property value with name {propertyName} on object of type " +
                $"{objectType} to {value}. Object has no property with that name.");
        }
    }
}
