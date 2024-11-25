using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.Models
{
    public class UnifiedPreference : PreferenceModel
    {
        public string NewTerminoLinguistico { get; set; }
        public Linguistic2Tuple NewTuple { get; set; }
    }
}
