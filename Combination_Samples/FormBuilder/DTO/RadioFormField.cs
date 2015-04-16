using System.Collections.Generic;

namespace DTO
{
    public class RadioFormField : FormField
    {
        public string[] Options { get; set; }

        public RadioFormField()
        {
            this.Kind = FormFieldType.Radio;
        }
    }
}
