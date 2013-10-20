using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public partial class Nisan
    {
        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.Rumi))
                return "(tiada nama)";
            else
                return this.Rumi;
        }

        // TODO: Failed [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Death { get; set; }

        [DisplayName("Muslim")]
        public Nullable<System.DateTime> Deathm { get; set; }

        public User Creator
        {
            get
            {
                HLGranite.Mvc.Models.hlgraniteEntities db = new hlgraniteEntities();
                Activity activity = db.Activities.Where(a => a.WorkItemId == this.WorkItemId).FirstOrDefault();
                if (activity != null)
                    return activity.User;

                return null;
            }
        }

        public DateTime Created
        {
            get
            {
                HLGranite.Mvc.Models.hlgraniteEntities db = new hlgraniteEntities();
                Activity activity = db.Activities.Where(a => a.WorkItemId == this.WorkItemId).FirstOrDefault();
                if (activity != null)
                    return activity.Date;

                return DateTime.MinValue;
            }
        }
    }
}