using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.IO;

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
        public static User GetCurrentUser()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                return db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            }
        }
    }

    /// <summary>
    /// 用于分页的分页List,继承自List
    /// </summary>
    /// <typeparam name="T">泛型参数T,必须实现IListPage接口</typeparam>
    public class ListPage<T> : List<T> where T :IListPage
    {
        private List<CourseOperation> course;
        private int page;
        private List<ActivityOperation> activity;
        private List<RoomOperation> room;

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

        public ListPage(List<CourseOperation> course, int page, int pageSize)
        {
            this.course = course;
            this.page = page;
            PageSize = pageSize;
        }

        public ListPage(List<ActivityOperation> activity, int page, int pageSize)
        {
            this.activity = activity;
            this.page = page;
            PageSize = pageSize;
        }

        public ListPage(List<RoomOperation> room, int page, int pageSize)
        {
            this.room = room;
            this.page = page;
            PageSize = pageSize;
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
}
