using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheMoviesHub.Class
{
    public class PagingInfo
    {
        public int TotalItem {  get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalItem / ItemPerPage);
            }
        }
    }
}