using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface IAssemblyConverter
    {
        string Convert(GrammarNode gnode, Dictionary<string,int> variableIndex);
    }

    class AssemblyConverter : IAssemblyConverter
    {
        public static int loopCount = 0;
        public const string skip = "\n" + "\t";

        BinaryOperationConverter boc;
        ArrayConverter arrayConverter;

        public AssemblyConverter()
        {
            boc = new BinaryOperationConverter(this);
            arrayConverter = new ArrayConverter(this);
        }

        public string Convert(GrammarNode gnode, Dictionary<string,int> variableIndex)
        {
            switch(gnode)
            {
                case ArrayCreationNode acn    : return arrayConverter.ConvertArrayCreationNode(acn, variableIndex);
                case ArrayNode anode          : return arrayConverter.ConvertArrayNode(anode, variableIndex);
                case ArrayAssign arrayAssign  : return arrayConverter.ConvertArrayAssign(arrayAssign, variableIndex);
                case IntegerNode inode        : return "mov eax, " + inode.t;
                case VariableNode vnode       : return "mov eax, " + ConvertName(vnode.t, variableIndex);
                case BinaryOperationNode bnode: return boc.BasicBinaryOperationNode(bnode, variableIndex);
                case FuncNode fnode           : return ConvertFuncNode(fnode, variableIndex);
                case LoopNode lnode           : return ConvertLoopNode(lnode,variableIndex);
                case CastNode cnode           : return Convert(cnode.tnode, variableIndex);
                case Assign assign            : return ConvertAssignement(assign,variableIndex);
                case ReturnExpression rexpr:  return Convert(rexpr.node, variableIndex);
                case DeclarationNode dnode: return ""; 

                default: throw new Exception("cannot convert " + gnode);
            }
        }

        private string ConvertAssignement(Assign assign, Dictionary<string,int> variableIndex)
        {
            if (assign is SimpleAssignement sa)
            {
                return ConvertToCode(Convert(sa.tree,variableIndex), 
                                   "mov " + ConvertName(sa.name,variableIndex) + ", eax");
            }
            
            throw new NotImplementedException();
        }

        public string ConvertToCode(params string[] str)
        {
            return str.Select(x => x + skip).Aggregate((x,y) => x + y);
        }

        public string ConvertName(string name, Dictionary<string, int> variableIndex)
        {
            if (!variableIndex.ContainsKey(name))
                throw new Exception("variable " + name + " was not found");

            int index = variableIndex[name];

            if (index >= 1)
            {
                index = 4 * index;
                return "[ebp-" +  index + "]";
            }
            else if (index <= -1)
            {
                index = 4 * index - 4;
                return "[ebp" + "+" +  -index + "]";
            }


            throw new NotImplementedException();
        }

        private string ConvertLoopNode(LoopNode lnode, Dictionary<string,int> variableIndex)
        {
            loopCount++;

            string loopName = "loop_" + loopCount;
            string init  = Convert(lnode.init, variableIndex);
            string times = Convert(lnode.times, variableIndex);
            string pos   = ConvertName(lnode.name, variableIndex);

            string start = ConvertToCode(
                           init, "mov edx, eax", "mov " + pos + ", eax",
                           times, "add edx, eax", "jmp " + loopName);
            
            

            string body = "push edx" + skip ;

            foreach(Expression expr in lnode.lexpr)
            {
                body += Convert(expr, variableIndex) + skip; 
            }

            body += ConvertToCode("pop edx", "inc dword " + pos, "cmp " + pos + ", " + "edx", "js " + loopName);

            return start + skip + loopName + ":" + skip  + body;
        }

        private string ConvertFuncNode(FuncNode fnode, Dictionary<string, int> variableIndex)
        {
            if (fnode.identifier.Equals("choice"))
                return ConvertChoice(fnode.arguments, variableIndex);

            List<ITreeNode> args = fnode.arguments;
            int size = args.Count;

            if (args.Count == 0)
            {
                return "call " + fnode.identifier;
            }

            string str = "";

            for (int i = args.Count - 1; i >= 0; i--)
            {
                str += Convert(args[i], variableIndex) + skip + "push eax" + skip;
            }

            return ConvertToCode(str, "call " + fnode.identifier, "sub esp, " + 4 * args.Count) + skip;
        }
       
        private int IndexAssignment(List<Expression> lexp, Dictionary<string,int> dict, int index)
        {
            Action<string> f = (x) => { index++; dict.Add(x,index); };

            for(int i=0;i<lexp.Count;i++)
            {
                Expression expr = lexp[i];

                switch(expr)
                {
                    case Initialization init   : f(init.name) ; break;
                    case DeclarationNode dnode : f(dnode.name); break;
                    case LoopNode lnode: f(lnode.name); index = IndexAssignment(lnode.lexpr, dict, index); break;
                    default: break;
                }
            }

            return index;
        }

        public string ConvertChoice(List<ITreeNode> ltnode, Dictionary<string,int> variableIndex)
        {
            Func<GrammarNode, string> f = (x) => Convert(x, variableIndex);
            string str = ConvertToCode(f(ltnode[2]), "push eax", f(ltnode[1]), "push eax", f(ltnode[0]), 
                                                     "pop ecx", "imul ecx, eax", "not eax", "and eax, 1", "pop edx", "imul eax, edx", 
                                                     "xor eax, ecx");


            return str;
        }

        public string ConvertMethodNodes(List<MethodNode> mnodes)
        {
            var sb = new StringBuilder();
            foreach (MethodNode mnode in mnodes)
            {
                sb.Append(ConvertMethodNode(mnode) + "\n" + "\n");
            }
            var output = sb.ToString();
            Console.WriteLine(output);
            return output;
        }


        public string ConvertMethodNode(MethodNode mnode)
        {
            var          dict = new Dictionary<string, int>();
            var inputs   = mnode.inputs;
            int ilen     = inputs.Count;

            for(int i=0; i < ilen; i++)

            {
                dict.Add(inputs[i].name, -i - 1);
            }

            int index = IndexAssignment(mnode.body, dict, 0);

            string result = ConvertToCode("_" + mnode.name + ":", "push ebp", "mov ebp, esp");

            if(index != 0)
            {
                result += "sub esp, " + index * 4
                + "\t ; variable storage" + skip + skip;
            }
                
            foreach(Expression expr in mnode.body)
            {
                result += Convert(expr, dict) + skip;
            }

            if(index != 0)
            {
                result += skip + "add esp, " + index * 4 + skip;
            }

            result += "pop ebp" + skip +"ret ";

            return result; 
        }
    }
}
