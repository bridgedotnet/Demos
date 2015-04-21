using Bridge;

namespace DTO
{
    public partial class Form
    {
        public Form(object obj) 
            : this()
        {
            dynamic rawFields = obj["Fields"];

            foreach (var rawField in rawFields)
            {
                if (rawField["Kind"] == (int)FormFieldType.Radio)
                {
                    this.Fields.Add(new RadioFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        Options = rawField["Options"]
                    });
                }
                else
                {
                    this.Fields.Add(new TextFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        Required = rawField["Required"]
                    });
                }
            }
        } 
    }
}
