using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class Remark:IListPage
    {
        public Guid Id { get; set; }

        public virtual User Receiver { get; set; }

        public DateTime ActionTime { get; set; }

        public string RemarkContent { get; set; }

        public int RemarkRate { get; set; }

        public DateTime Time { get; set; }

        public Remark()
        {
            Id = Guid.NewGuid();
            //此处需要重构构造函数@龚齐翔。
            //Receiver = Extensions.GetCurrentUser(db);
            ActionTime = DateTime.Now;
            throw new NotImplementedException();
        }
    }

    public class Operation : IListPage
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public User Creator { get; set; }

        public DateTime Time { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Content { get; set; }

        public int State { get; set; }
    }
}