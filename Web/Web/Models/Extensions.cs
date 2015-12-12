using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.IO;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public static class Extensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> hashSet = new HashSet<TKey>();
            foreach (TSource t in source)
            {
                if (hashSet.Add(keySelector(t)))
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// 返回当前用户的User类型数据的方法.
        /// </summary>
        /// <returns>当前用户的User类型数据</returns>
        public static User GetContextUser(BaseDbContext db)
        {
            return db.Users.Find(HttpContext.Current.User.Identity.GetUserId());

        }

        public static User GetCurrentUser()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                return db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            }
        }

        public static string GetUserId()
        {
            return HttpContext.Current.User.Identity.GetUserId();
        }
    }

    /// <summary>
    /// 用于分页的分页List,继承自List
    /// </summary>
    /// <typeparam name="T">泛型参数T,必须实现IListPage接口</typeparam>
    public class ListPage<T> : List<T> where T : class, IListPage
    {
        /// <summary>
        /// 页面索引值
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 每页记录的数量
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 记录总条数
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 共有的页数和
        /// </summary>
        public int TotalPages { get; private set; }

        public ListPage(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            // 进上去取整（ 总记录条数/一面记录的条数）
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
            var temp = source.OrderBy(p => p.Time).Skip(pageIndex * pageSize);
            AddRange(source.OrderBy(p => p.Time).Skip(pageIndex * pageSize).Take(PageSize));
        }

        /// <summary>
        /// 是否存在前续页
        /// </summary>
        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        /// <summary>
        /// 是否存在后续页
        /// </summary>
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }

    /// <summary>
    /// 用于实现分页排序的接口
    /// </summary>
    public interface IListPage
    {
        DateTime Time { get; set; }
    }

    public class DayRangeAttribute : ValidationAttribute, IClientValidatable
    {
        private int _minDay;
        private int _maxDay;

        public DayRangeAttribute(int minDay, int maxDay)
        {
            if (minDay.CompareTo(maxDay) > -1)
            {
                throw new Exception("最小日期不能大于或等于最大日期");
            }
            _minDay = minDay;
            _maxDay = maxDay;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            var compareDate = value as DateTime?;
            if (compareDate.HasValue)
            {
                compareDate = compareDate.Value.Date;
                return compareDate.Value >= DateTime.Today.AddDays(_minDay).Date &&
                       compareDate.Value <= DateTime.Today.AddDays(_maxDay).Date;
            }
            return false;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "dayrange", //这里的dayrange最终会成为data-val-dayrange属性被jquery侦测到
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            //这里了的min和max将会作为jquery验证扩展方法的参数
            rule.ValidationParameters["min"] = _minDay;
            rule.ValidationParameters["max"] = _maxDay;
            yield return rule;
        }
    }
}
