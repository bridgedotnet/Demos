
namespace DTO
{
    public class TextFormField : FormField
    {
        public bool Required { get; set; }

        public TextFormField()
        {
            this.Kind = FormFieldType.Text;
            this.Required = false;
        }
    }
}
