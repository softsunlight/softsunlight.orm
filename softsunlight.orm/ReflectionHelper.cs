using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 反射工具操作类
    /// </summary>
    public class ReflectionHelper
    {
        /// <summary>
        /// 缓存类型全名和属性
        /// </summary>
        private static ConcurrentDictionary<string, PropertyInfo[]> type2Properties = new ConcurrentDictionary<string, PropertyInfo[]>();

        private static readonly object type2PropertiesLockObj = new object();

        /// <summary>
        /// 缓存类型属性，按属性名获取PropertyInfo
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, PropertyInfo>> typePropertyName2Properties = new ConcurrentDictionary<string, ConcurrentDictionary<string, PropertyInfo>>();

        private static readonly object typePropertyName2PropertiesLockObj = new object();

        /// <summary>
        /// 获取类型属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            lock (type2PropertiesLockObj)
            {
                if (type2Properties.ContainsKey(type.FullName))
                {
                    return type2Properties[type.FullName];
                }
                PropertyInfo[] propertyInfos = type.GetProperties();
                type2Properties[type.FullName] = propertyInfos;
            }
            return type2Properties[type.FullName];
        }

        /// <summary>
        /// 获取类型指定属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            lock (typePropertyName2PropertiesLockObj)
            {
                if (!typePropertyName2Properties.ContainsKey(type.FullName))
                {
                    typePropertyName2Properties[type.FullName] = new ConcurrentDictionary<string, PropertyInfo>();
                }
                if (!typePropertyName2Properties[type.FullName].ContainsKey(propertyName))
                {
                    typePropertyName2Properties[type.FullName][propertyName] = type.GetProperty(propertyName);
                }
            }
            return typePropertyName2Properties[type.FullName][propertyName];
        }

        /// <summary>
        /// 获取类型指定属性值
        /// </summary>
        /// <param name="instanceObj"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static object? GetPropertyValue(object? instanceObj, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(instanceObj);
        }

    }
}
