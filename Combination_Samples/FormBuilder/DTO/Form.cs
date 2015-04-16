using System.Collections.Generic;

namespace DTO
{
    public class Form
    {
        public List<FormField> Fields { get; set; }

        public Form()
        {
            this.Fields = new List<FormField>();
        }
    }
}
