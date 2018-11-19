using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SL.Models
{
    public class PagerModel
    {
        public PagerModel(int totalCount, DataTable pagerSource, int pageIndex, int pageSize)
        {
            this.TotalCount = totalCount;
            this.PagerSource = pagerSource;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 分页 - 当前的页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页 - 每页的数据条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 分页数据源
        /// </summary>
        public DataTable PagerSource { get; set; }
    }

    public enum Sort
    {
        /// <summary>
        /// 倒序
        /// </summary>
        DESC = 1,
        /// <summary>
        /// 正序
        /// </summary>
        ASC = 2
    }

    public class TablePageListBaseModel
    {
        public TablePageListBaseModel() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsPaging">是否分页</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">每页大小</param>
        public TablePageListBaseModel(bool IsPaging, int PageIndex = 1, int PageSize = 15)
        {
            this.IsPaging = IsPaging;
            this.PageIndex = PageIndex;
            this.PageSize = PageSize;
        }
        private bool _IsPaging = true;
        /// <summary>
        /// 是否启用分页
        /// </summary>
        public bool IsPaging
        {
            get
            {
                return _IsPaging;
            }
            set
            {
                _IsPaging = value;
            }
        }
        private int _PageIndex = 1;
        public int PageIndex
        {
            get
            {
                return _PageIndex;
            }
            set
            {
                _PageIndex = value;
            }
        }
        private int _PageSize = 15;
        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = value;
            }
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortColumnName { get; set; }

        private Sort _Sort = Sort.ASC;
        /// <summary>
        /// 排序方式
        /// </summary>
        public Sort Sort
        {
            get
            {
                return _Sort;
            }
            set
            {
                _Sort = value;
            }
        }
        /// <summary>
        /// 获取排序sql
        /// </summary>
        public string sqlSort
        {
            get
            {
                return string.Format(" ORDER BY {0} {1}", this.SortColumnName, this.Sort.ToString());
            }
        }
        /// <summary>
        /// 计算总页码
        /// </summary>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        public int TotalPages(int dataCount)
        {
            if (dataCount <= PageSize)
            {
                return 1;
            }
            if (dataCount % PageSize == 0)
            {
                return dataCount / PageSize;
            }
            else
            {
                return dataCount / PageSize + 1;
            }
        }
        /// <summary>
        /// 获取开始列数
        /// </summary>
        public int RowBegin
        {
            get
            {
                return (PageIndex - 1) * PageSize + 1;
            }
        }
        /// <summary>
        /// 获取结束行
        /// </summary>
        public int RowEnd
        {
            get
            {
                return PageIndex * PageSize;
            }
        }
    }

    public class TablePageListBaseReturnModel<T> : PageBaseReturnModel<T>
    {
        public TablePageListBaseReturnModel() : base() { }
        public TablePageListBaseReturnModel(bool isSuccess, T data, int totalPages, int PageSize, int dataCount, string msg = "")
            : base(isSuccess, data, msg)
        {
            this.dataCount = dataCount;
            this.PageSize = PageSize;
            this.totalPages = totalPages;
        }
        public int totalPages { get; set; }
        public int PageSize { get; set; }
        public int dataCount { get; set; }
    }

    public class PageBaseReturnModel<T>
    {
        public PageBaseReturnModel() { }
        public PageBaseReturnModel(bool isSuccess, T data, string msg = "", string msgCode = "")
        {
            this.isSuccess = isSuccess;
            this.data = data;
            this.msg = msg;
        }
        public bool isSuccess { get; set; }
        public T data { get; set; }
        public string msg { get; set; }
        private string _msgCode;
        public string msgCode
        {
            get
            {
                if (string.IsNullOrEmpty(_msgCode) && this.isSuccess)
                {
                    return "100";
                }
                return _msgCode;
            }
            set
            {
                _msgCode = value;
            }
        }
    }
}
