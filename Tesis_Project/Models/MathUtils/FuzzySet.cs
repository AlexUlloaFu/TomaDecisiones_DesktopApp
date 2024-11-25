using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesis_Project.Models.MathUtils
{
    public class FuzzySet
    {
        public string Term { get; set; }
        public List<Point> MembershipFunction { get; set; }

        public FuzzySet(string term, List<Point> membershipFunction)
        {
            Term = term;
            MembershipFunction = membershipFunction;
        }
    }
}
