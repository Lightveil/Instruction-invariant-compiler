using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Printer
    {
        public string Print(GrammarNode gnode)
        {
            switch (gnode)
            {
                case ArrayNode anode: return Print(anode);
                case ArrayAssign assign: return Print(assign);
                case ArrayTypeNode av: return Print(av);
                case BinaryOperationNode bnode: return Print(bnode);
                case BoolNode bnode: return Print(bnode);
                case ByteNode bnode: return Print(bnode);
                case CastNode cnode: return Print(cnode);
                case DeclarationNode dnode: return Print(dnode);
                case FixedTypeNode fv: return Print(fv);
                case FuncNode fnode: return Print(fnode);
                case Initialization init: return Print(init);
                case IntegerNode inode: return Print(inode);
                case LoopNode lnode: return Print(lnode);
                case MethodNode mnode: return Print(mnode);
                case ReturnExpression rexpr: return Print(rexpr);
                case PrimitiveTypeNode p: return Print(p);
                case SimpleAssignement sa: return Print(sa);
                case VariableNode vnode: return Print(vnode);
                case VoidNode vnode: return Print(vnode);
                default: throw new Exception("Printer not defined for " + gnode);
            }
        }

        public string Print(IntegerNode inode)
        {
            return inode.t + "";
        }

        public string Print(BoolNode bnode)
        {
            return "" + bnode.t;
        }

        public string Print(ByteNode bnode)
        {
            return "" + bnode.t;
        }

        public string Print(VariableNode vnode)
        {
            return "" + vnode.t;
        }

        public string Print(CastNode cnode)
        {
            return "(" + Print(cnode.primitive) + ")" + " " + Print(cnode.tnode);
        }

        public string Print(BinaryOperationNode bnode)
        {
            return "(" + Print(bnode.left) + ")"
                 + bnode.operation + "(" + Print(bnode.right) + ")";
        }

        public string Print(ArrayNode anode)
        {
            return anode.identifier + "[" + Print(anode.index) + "]";
        }

        public String Print(FuncNode fnode)
        {
            if (fnode.arguments.Count == 0)
                return fnode.identifier + "(" + ")";

            string str = Print(fnode.arguments[0]);

            for (int i = 1; i < fnode.arguments.Count; i++)
            {
                str += "," + Print(fnode.arguments[i]);
            }

            return fnode.identifier + "(" + str + ")";
        }

        public String Print(MethodNode mnode)
        {
            string declarations = (mnode.inputs.Count) == 0 ? ""
                                : Print(mnode.inputs[0]);
            string bodyString = "";

            mnode.body.ForEach(x => bodyString += "\t" + Print(x) + "\n");


            for (int i = 1; i < mnode.inputs.Count; i++)
            {
                declarations += "," + Print(mnode.inputs[i]);
            }

            return "Secure " + Print(mnode.t) + " " + mnode.name
                 + "(" + declarations + ")"
                 + "\n { \n" + bodyString + "\n } \n";
        }

        public string Print(PrimitiveTypeNode p)
        {
            return p.type;
        }

        public string Print(VoidNode v)
        {
            return "void";
        }

        public string Print(ArrayTypeNode av)
        {
            return Print(av.type) + "[]";
        }

        public string Print(FixedTypeNode ftn)
        {
            return "fixed " + Print(ftn.simpleTypeNode);
        }

        public String Print(DeclarationNode dnode)
        {
            return Print(dnode.t) + " " + dnode.name;
        }

        public String Print(LoopNode lnode)
        {
            string str = "loop(int " + lnode.name + "="
                       + lnode.init + "," + lnode.times 
                       + ")" + "\n" + "{";

            foreach (Expression exp in lnode.lexpr)
            {
                str += "\n \t" + Print(exp);
            }

            return str + "\n" + "}" + "\n";
        }

        public string Print(ReturnExpression rexpr)
        {
            return " return " + Print(rexpr.node) + " ; ";
        }

        public String Print(SimpleAssignement sassign)
        {
            return sassign.name + "=" + Print(sassign.tree);
        }

        public String Print(Initialization init)
        {
            return Print(init.type) + " " + init.name 
                 + "=" + Print(init.tree);
        }

        public string Print(ArrayAssign aa)
        {
            return aa.name + "[" + Print(aa.index) 
                 + "]" + " = " + Print(aa.tree);
        }
    }
}
