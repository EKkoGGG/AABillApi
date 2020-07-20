using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AABillApi.Models
{
    public class BillResult
    {
        public object Data { get; set; }
        public Meta Meta { get; set; } = new Meta();

        public BillResult(StatusCodes statusCodes,object data = null)
        {
            this.Data = data;
            switch (statusCodes)
            {
                case StatusCodes.Code200:
                    this.Meta.Status = 200;
                    this.Meta.Msg = "获取成功！";
                    break;
                case StatusCodes.Code404:
                    this.Meta.Status = 200;
                    this.Meta.Msg = "获取成功！";
                    break;
                case StatusCodes.Code401:
                    this.Meta.Status = 401;
                    this.Meta.Msg = "没有权限！";
                    break;
                default:
                    break;
            }
        }
    }

    public enum StatusCodes
    {
        Code200,
        Code404,
        Code401
    }
    public class Meta
    {
        public int Status { get; set; }

        public string Msg { get; set; }
    }
}
