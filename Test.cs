using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Parsing;

namespace Compiler
{
    class Test
    {

        public static IEnumerable<string> TestEnumerator()
        {
            yield return "secure int init_array(fixed int len)"
                         + "{"
                         + "int[] array = new int[len];"
                         + "loop(int i = 0, len)"
                         + "{"
                         + "array[i] = i;"
                         + "}"
                         + "return array;"
                         + "}";

            yield return "secure int arraySum(int array, int len)"
                       + "{"
                       + "int result = 0;"
                       +  "loop(int i=0,len)"
                       + "{"
                       +  "result = array[i] + result;" 
                       + "}"
                       + "return result;"
                       + "}";


            yield break;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
