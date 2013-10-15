using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public partial class StockType
    {
        public static int RENOVATION_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                StockType type = db.StockTypes.Where(t => t.Type.ToLower().Equals("renovation")).FirstOrDefault();
                if (type == null)
                    return 0;
                else
                    return type.Id;
            }
        }

        public static int TOMB_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                StockType type = db.StockTypes.Where(t => t.Type.ToLower().Equals("tomb")).FirstOrDefault();
                if (type == null)
                    return 0;
                else
                    return type.Id;
            }
        }

        public static int NISAN_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                StockType type = db.StockTypes.Where(t => t.Type.ToLower().Equals("nisan")).FirstOrDefault();
                if (type == null)
                    return 0;
                else
                    return type.Id;
            }
        }
    }
}