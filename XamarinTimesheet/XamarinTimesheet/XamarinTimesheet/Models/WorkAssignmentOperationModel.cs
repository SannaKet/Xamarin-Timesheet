using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinTimesheet.Models
{
    class WorkAssignmentOperationModel
    {
        public int AssignmentId { get; set; }

        public string Operation { get; set; }
        public string AssignmentTitle { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }


    }
}
