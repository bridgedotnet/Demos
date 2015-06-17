using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Bridge.Translator;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Live
{
    public class LiveTranslator 
    {
        const string HEADER =
            @"using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Text;
            using System.Linq;
            using System.Threading.Tasks;
            using Bridge;
            using Bridge.Bootstrap3;
            using Bridge.Html5;
            using Bridge.jQuery2;
            using Bridge.WebGL;
            using Bridge.Linq;

            namespace LiveApp
            {";

        const string FOOTER = "}";

        protected static readonly char ps = System.IO.Path.DirectorySeparatorChar;
        
        private string msbuildVersion = "v4.0.30319";

        private string sessionId;
        private string bridgeFolder;
        private string csFolder;

        private Bridge.Translator.Translator translator = null;

        public bool Rebuild { get; set;  }
        public bool ChangeCase { get; set; }
        public string Config { get; set; }
        public string BridgeLocation { get; set; }
        public string Source { get; private set; }

        public LiveTranslator(string folder, string source, bool recursive, string lib, HttpContext context)
        {
            this.sessionId = context.Session.SessionID;
            this.bridgeFolder = folder + @"\Bridge\Builder\";
            this.csFolder = folder + @"\UserCode\";
                                    
            this.BuildSourceFile(source);
                        
            lib = (this.RebuildStub(source)) ? this.BuildAssembly() : Path.Combine(folder, lib);
                        
            this.translator = new Bridge.Translator.Translator(this.csFolder, this.Source, recursive, lib);
                        
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

            return this.translator.Outputs[this.translator.Outputs.Keys.First()];
        }
        
        private bool RebuildStub(string source)
        {
            // Rebuild needed if more than one classes are defined or the only defined class is not named "App"
            foreach(var line in source.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Trim().StartsWith(@"//"))
                {
                    MatchCollection m = Regex.Matches(line, @"class ([a-zA-Z\d]*)");
                    if (m.Count > 0)
                    {
                        if (m[0].Groups[1].ToString() != "App")
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void BuildSourceFile(string source)
        {
            string filename = this.sessionId + ".cs",
                   path = this.csFolder + filename;

            using (TextWriter writer = File.CreateText(path))
            {
                writer.Write(HEADER);
                writer.Write(source);
                writer.Write(FOOTER);
            }

            this.Source = filename;
        }

        protected virtual string GetBuilderPath()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return Environment.GetEnvironmentVariable("windir") + ps + "Microsoft.NET" + ps + "Framework" + ps +
                        this.msbuildVersion + ps + "csc";
                default:
                    throw (System.Exception)Bridge.Translator.Exception.Create("Unsupported platform - {0}", Environment.OSVersion.Platform);
            }
        }

        protected virtual string GetBuilderArguments(string outFile)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:                    
                    return String.Format(@" /noconfig /nostdlib+ /warn:0 /reference:{0} /out:{1} {2}",
                        string.Format("{0};{1};{2};{3};{4}", 
                            this.bridgeFolder + "Bridge.dll",
                            this.bridgeFolder + "Bridge.Html5.dll",
                            this.bridgeFolder + "Bridge.jQuery2.dll",
                            this.bridgeFolder + "Bridge.Bootstrap3.dll",
                            this.bridgeFolder + "Bridge.WebGL.dll"),
                        outFile, 
                        this.csFolder + this.Source);
                default:
                    throw (System.Exception)Bridge.Translator.Exception.Create("Unsupported platform - {0}", Environment.OSVersion.Platform);
            }
        }

        protected virtual string BuildAssembly()
        {
            string filename = this.sessionId + ".dll",
                   path = this.bridgeFolder + filename;

            var info = new ProcessStartInfo()
            {
                FileName = this.GetBuilderPath(),
                Arguments = this.GetBuilderArguments(path)
            };
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = false;            
            using (var p = Process.Start(info))
            {
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    return path;
                }
            }

            return path;
        }
    }
}