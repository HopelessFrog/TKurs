using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model
{
    public class Task
    {
        public int Id { get; set; }
        public string Formulaa { get; set; }

        public string Descriptiom { get; set; }

        public bool Real { get; set; }

        public override string ToString()
        {
            return Formulaa;
        }
    }
}
