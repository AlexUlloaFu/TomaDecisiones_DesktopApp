using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesis_Project.Models.MathUtils
{
    public class Linguistic2Tuple
    {
        public int IndexTermino;
        public double SimbolicTraslation;

        public Linguistic2Tuple(int indexTermino, double simbolicTraslation)
        {
            IndexTermino = indexTermino;
            SimbolicTraslation = simbolicTraslation;
        }
    }
}
