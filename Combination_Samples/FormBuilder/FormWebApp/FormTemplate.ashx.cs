using System;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DTO;

namespace FormWebApp
{
    /// <summary>
    /// Returns form template data in JSON format
    /// </summary>
    public class FormTemplate : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(this.Template));
        }

        private Form Template
        {
            get
            {
                return new Form()
                {
                    Fields = {
                        new TextFormField()
                        {
                            Id = "name", Label = "Full Name"
                        },

                        new TextFormField()
                        {
                            Id = "email", Label="Email", Required = true
                        },

                        new RadioFormField()
                        {
                            Id="q1", Label = "Favorite Browser", Options = "IE,Firefox,Chrome,Safari".Split(',')
                        },

                        new RadioFormField()
                        {
                            Id="q2", Label = "Favorite Language", Options = "C#,JavaScript,Both".Split(',')
                        }
                    }
                };
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}