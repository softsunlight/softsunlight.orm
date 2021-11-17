using System;
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
        private static Dictionary<string, PropertyInfo[]> type2Properties = new Dictionary<string, PropertyInfo[]>();

        /// <summary>
        /// 缓存类型属性，按属性名获取PropertyInfo
        /// </summary>
        private static Dictionary<string, Dictionary<string, PropertyInfo>> typePropertyName2Properties = new Dictionary<string, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// 获取类型属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            if (type2Properties.ContainsKey(type.FullName))
            {
                return type2Properties[type.FullName];
            }
            PropertyInfo[] propertyInfos = type.GetProperties();
            type2Properties[type.FullName] = propertyInfos;
            return propertyInfos;
        }

        /// <summary>
        /// 获取类型指定属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(Type type, string typeName)
        {
            if (!typePropertyName2Properties.ContainsKey(type.FullName))
            {
                typePropertyName2Properties[type.FullName] = new Dictionary<string, PropertyInfo>();
            }
            if (!typePropertyName2Properties[type.FullName].ContainsKey(typeName))
            {
                typePropertyName2Properties[type.FullName][typeName] = type.GetProperty(typeName);
            }
            return typePropertyName2Properties[type.FullName][typeName];
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
