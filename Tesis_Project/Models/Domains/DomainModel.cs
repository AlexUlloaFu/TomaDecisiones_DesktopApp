using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.Models.Domains
{
    public class DomainModel
    {
        private string _id;
        public string ID
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        private DomainType _type;
        public DomainType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

      

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public float LowerLimit;
        public float UpperLimit;
        public Collection<TriangularFunction> FuncionesPertenencia = new ObservableCollection<TriangularFunction>();

        public Collection<string> TerminosLinguisticos { get; set; }
        public string TerminosLinguisticosFormatted => string.Join(", ", TerminosLinguisticos);
    }
}


public enum DomainType  {
    Real,
    Intervalar,
    Linguistico
}