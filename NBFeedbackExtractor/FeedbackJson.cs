using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace NBFeedbackExtractor
{
    
        public class Rootobject
    {
        //public Activity[] activities { get; set; }
        public List<Activity> activities { get; set; }
    }

        public class Activity
        {
            public string oneliner { get; set; }
            public string extended { get; set; }
            public Relatedsignups relatedSignups { get; set; }
            public string type { get; set; }
            public int id { get; set; }
            public bool isPrivate { get; set; }
            public DateTime timestamp { get; set; }
        }

        public class Relatedsignups
        {
            public Signup signup { get; set; }
        }

        public class Signup
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool following { get; set; }
        }


    
}
