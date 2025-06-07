using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationApp
{
    public abstract class ItemBase
    {
        public abstract int ItemID { get; }
        public abstract string Title { get; set; }
        public abstract string Type { get; set; }
        public abstract string Genre { get; set; }

        public abstract string GetInfo();

        public override string ToString()           
        {
            return GetInfo();
        }
    }
}
