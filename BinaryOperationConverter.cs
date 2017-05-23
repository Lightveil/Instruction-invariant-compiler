using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class BinaryOperationConverter
    {
        HashSet<string> comparators = new HashSet<string>() { "==", ">", "<", ">=", "<=", "!=" };

        AssemblyConverter converter;

        public BinaryOperationConverter(AssemblyConverter converter)
        {
            this.converter = converter;
        }


        public string Comparator(BinaryOperationNode bnode, Dictionary<string, int> variableIndex)
        {
            string instructions = converter.ConvertToCode(StandardBinaryOperation(bnode, variableIndex));

            string zeroFlag  = converter.ConvertToCode("cmp eax, ecx", "lahf" , "shr eax, 14","and eax, 1");
            string signFlag  = converter.ConvertToCode("cmp eax, ecx", "lahf" , "shr eax, 15","and eax, 1");
            string comb      = zeroFlag + converter.ConvertToCode("mov edx, eax", "lahf", "shr eax, 14", "and eax, 1");

            string op = "";
            string negate = converter.ConvertToCode("not eax", "and eax, 1");

            switch (bnode.operation)
            {
                case "==": op = zeroFlag; break;
                case "<" : op = signFlag; break;
                case "<=": op = comb; break;
                case "!=": op = zeroFlag + negate;  break;
                case ">=": op = signFlag + negate;  break;
                case ">" : op = comb     + negate; break;
                default: throw new Exception("Comparator " + bnode.operation + " is not being converted.");
                
            }

            return instructions + op;
        }
    

        public string StandardBinaryOperation(BinaryOperationNode bnode, Dictionary<string, int> variableIndex)
        {
            return converter.ConvertToCode(converter.Convert(bnode.right, variableIndex), "push eax"
                                       , converter.Convert(bnode.left, variableIndex), "pop ecx");
        }

        public string BasicBinaryOperationNode(BinaryOperationNode bnode, Dictionary<string,int> variableIndex)
        {
            string op = null;

            if (comparators.Contains(bnode.operation))
                return Comparator(bnode, variableIndex);

            switch (bnode.operation)
            {
                case "+": op = "add";   break;
                case "-": op = "sub";   break;
                case "*": op = "imul";  break;
            }

            return StandardBinaryOperation(bnode,variableIndex) + converter.ConvertToCode(op + " eax, ecx");
        }
    }
}
