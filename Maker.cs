using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Parsing;

namespace Compiler
{
   

    class Maker
    {
        readonly Func<int, string> GetData = heapSize => LinesOfCode("section. data","","heap times " + heapSize + " dd 0", "heapIndex dd heap");

        public static string LinesOfCode(params string[] args)
        {
            var builder = new StringBuilder();

            foreach(string str in args)
            {
                builder.Append(str);
                builder.Append("\n");
            }

            return builder.ToString();
        }

        public static void Main(string[] args)
        {
            Maker maker = new Maker();
            maker.Make("test.iics", "test.asm", 2000);
            System.Threading.Thread.Sleep(1000000);
        }

        public void Make(string file, string outputFilePath, int heapSize)
        {
            TypeChecker checker          = new TypeChecker();
            AssemblyConverter converter = new AssemblyConverter();

            using (Stream stream = File.Open(file, FileMode.Open))
            {
                Parser p = Parser.CreateParser(stream);

                if (p.Parse())
                    Console.WriteLine("Parsing sucessful.");
                else
                {
                    Console.WriteLine("Parsing unsuccessful.");
                    return;
                }

                List<MethodNode> lmnode = p.Result;

                try
                {
                    checker.TypeCheckMethods(lmnode);
                    Console.WriteLine("Type checking passed.");
                }
                catch(Exception e)
                {
                    Console.WriteLine("Type checking failed.");
                    Console.WriteLine(e.ToString());
                    return;
                }

                
                if (File.Exists(outputFilePath))
                {
                    File.Delete(outputFilePath);
                }

                using (FileStream fs = File.Create(outputFilePath))
                {
                    try
                    {
                        string compiled = converter.ConvertMethodNodes(lmnode) + "\n \n" 
                                        + GetData(heapSize);
                        
                        Byte[] text = new UTF8Encoding(true).GetBytes(compiled);
                        fs.Write(text, 0, text.Length);
                        
                        Console.WriteLine("Compilation succeeded.");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Compilation failed.");
                        Console.WriteLine(e.ToString());
                        return;
                    }
                }
            }
        }       
    }

}