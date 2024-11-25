using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.Models
{
    public interface IAggregationOperator
    {
        string Name { get; }
        public Linguistic2Tuple Aggregate(List<Linguistic2Tuple> tuples);
    }
}
