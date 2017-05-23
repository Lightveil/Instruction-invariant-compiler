using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class ArrayConverter
    {

        AssemblyConverter converter;

        public ArrayConverter(AssemblyConverter converter)
        {
            this.converter = converter;
        }


        public string ConvertArrayCreationNode(ArrayCreationNode acn, Dictionary<string, int> dict)
        {
            Func<string, string, string> f = (x, y) => x + "\t ; " + y ;

            string code = converter.ConvertToCode(converter.Convert(acn.size, dict) 
                        + "\t \t ; length of array",
                                        f("mov ecx, [heapIndex]","ecx = current position in heap"),
                                        "shl eax, 2" + "\t \t ; length = length * 4",
                                        "add ecx, eax" + "\t \t ; ecx = position + length * 4 ",
                                        f("mov eax, [heapIndex]","eax = heapIndex"),
                                        f("mov [heapIndex], ecx", "heapIndex = ecx"));

            return code;
        }

        public string ConvertArrayNode(ArrayNode anode, Dictionary<string, int> dict)
        {
            string code = converter.ConvertToCode(converter.Convert(anode.index, dict) + "\t ; eax = index",
                                        "shl eax, 2" + "\t ; index = index  * 4",
                                        "mov dword ecx, " + converter.ConvertName(anode.identifier, dict),
                                        "add ecx, eax",
                                        "mov dword eax, [ecx]");

            return code;
        }

        public string ConvertArrayAssign(ArrayAssign arrayAssign, Dictionary<string, int> dict)
        {
            string code = converter.ConvertToCode("", converter.Convert(arrayAssign.tree, dict),
                         "push eax",
                         converter.Convert(arrayAssign.index, dict),
                         "shl eax, 2",
                         "mov ecx, " + converter.ConvertName(arrayAssign.name, dict),
                         "add ecx, eax",
                         "pop eax",
                         "mov [ecx], eax");
            return code;
        }
    }
}
