using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKurs.Model.Calculators;

namespace TKurs.Model
{
    public class Method
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [NotMapped]
        public ICalculator Calculator { get; set; }

        public bool Real { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
