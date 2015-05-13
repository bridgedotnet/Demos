using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Bridge.Translator;

namespace LiveBridge
{
    public class LiveTranslator 
    {
        const string HEADER = 
            @"using Bridge;
            using Bridge.Bootstrap3;
            using Bridge.Html5;
            using Bridge.jQuery2;
            using Bridge.Linq;
            using Bridge.WebGL;
            using System.Linq;
            using System.Collections.Generic;

            namespace BridgeStub
            {";

        const string FOOTER = "}";

        private Bridge.Translator.Translator translator = null;

        public bool Rebuild { get; set;  }
        public bool ChangeCase { get; set; }
        public string Config { get; set; }
        public string BridgeLocation { get; set; }
        public string Source { get; private set; }

        public LiveTranslator(string folder, string source, bool recursive, string lib, HttpContext context)
        {            
            lib = Path.Combine(folder, lib);
        
            this.BuildSourceFile(folder, source, context);
            
            this.translator = new Bridge.Translator.Translator(folder, this.Source, recursive, lib);
            
            this.Rebuild = false;
            this.ChangeCase = true;
            this.Config = null;
        }

        public string Translate()
        {            
            string bridgeLocation = !string.IsNullOrEmpty(this.BridgeLocation) ? this.BridgeLocation : Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Bridge.dll");

            this.translator.BridgeLocation = bridgeLocation;
            this.translator.Rebuild = this.Rebuild;
            this.translator.ChangeCase = this.ChangeCase;
            this.translator.Configuration = this.Config;
            this.translator.Translate();

            return translator.Outputs[translator.Outputs.Keys.First()];
        }

        private void BuildSourceFile(string folder, string source, HttpContext context)
        {
            string filename = context.Session.SessionID + ".cs",
                   path = folder + filename;

            using (TextWriter writer = File.CreateText(path))
            {
                writer.Write(HEADER);
                writer.Write(source);
                writer.Write(FOOTER);
            }

            this.Source = filename;
        }
    }
}