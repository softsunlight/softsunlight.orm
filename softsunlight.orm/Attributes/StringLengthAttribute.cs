using System;
using System.Collections.Generic;
using System.Text;

namespace softsunlight.orm.Attributes
{
    /// <summary>
    /// 字符串长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class StringLengthAttribute : Attribute
    {
        public StringLengthAttribute(int length)
        {
            this.Length = length;
        }
        /// <summary>
        /// 字符串的长度
        /// </summary>
        public int Length { get; set; }
    }
}
