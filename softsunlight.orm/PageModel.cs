using System;
using System.Collections.Generic;
using System.Text;

namespace softsunlight.orm
{
    /// <summary>
    /// 分页模型
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNo { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 总条目
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总分页数
        /// </summary>
        public int TotalPages { get; set; }
    }
}
