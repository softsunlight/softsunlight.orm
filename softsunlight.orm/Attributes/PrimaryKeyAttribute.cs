using System;
using System.Collections.Generic;
using System.Text;

namespace softsunlight.orm.Attributes
{
    /// <summary>
    /// 指定实体类的主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class PrimaryKeyAttribute : Attribute
    {

    }
}
