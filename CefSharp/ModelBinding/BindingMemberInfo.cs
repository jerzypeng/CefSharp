// Copyright © 2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CefSharp.ModelBinding
{
    /// <summary>
    /// Represents a bindable member of a type, which can be a property or a field.
    /// </summary>
    public class BindingMemberInfo
    {
        private readonly PropertyInfo propertyInfo;
        private readonly FieldInfo fieldInfo;

        /// <summary>
        /// Gets the name of the property or field represented by this BindingMemberInfo.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the data type of the property or field represented by this BindingMemberInfo.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Constructs a BindingMemberInfo instance for a property.
        /// </summary>
        /// <param name="propertyInfo">The bindable property to represent.</param>
        public BindingMemberInfo(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo ?? throw new ArgumentNullException("propertyInfo");

            Type = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
        }

        /// <summary>
        /// Constructs a BindingMemberInfo instance for a field.
        /// </summary>
        /// <param name="fieldInfo">The bindable field to represent.</param>
        public BindingMemberInfo(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo ?? throw new ArgumentNullException("fieldInfo");

            Type = fieldInfo.FieldType;
            Name = fieldInfo.Name;
        }

        /// <summary>
        /// Sets the value from a specified object associated with the property or field represented by this BindingMemberInfo.
        /// </summary>
        /// <param name="destinationObject">The object whose property or field should be assigned.</param>
        /// <param name="newValue">The value to assign in the specified object to this BindingMemberInfo's property or field.</param>
        public void SetValue(object destinationObject, object newValue)
        {
            if (propertyInfo == null)
            {
                fieldInfo.SetValue(destinationObject, newValue);
            }
            else
            {
                propertyInfo.SetValue(destinationObject, newValue, null);
            }
        }

        /// <summary>
        /// Returns an enumerable sequence of bindable properties for the specified type.
        /// </summary>
        /// <param name="type">The type to enumerate.</param>
        /// <returns>Bindable properties.</returns>
        public static IEnumerable<BindingMemberInfo> Collect(Type type)
        {
            var fromProperties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite)
                .Where(property => !property.GetIndexParameters().Any())
                .Select(property => new BindingMemberInfo(property));

            var fromFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => !f.IsInitOnly)
                .Select(field => new BindingMemberInfo(field));

            return fromProperties.Concat(fromFields);
        }
    }
}
