using System;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.Hibernate
{
    public class WebServiceQueryOptions
    {
        public int PageSize = -1;
        public int PageNumber = 0;

        public int FirstResult
        {
            get
            {
                return PageSize * PageNumber;
            }
        }

        public WebServiceQueryOptions()
        {
        }

        public WebServiceQueryOptions(int pagesize, int pagenumber)
        {
            PageSize = pagesize;
            PageNumber = pagenumber;
        }
    };
}
