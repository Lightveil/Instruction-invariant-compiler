using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface TypeNode : GrammarNode
    {}

    public abstract class VariableTypeNode : TypeNode
    { }

    public abstract class SimpleTypeNode : VariableTypeNode
    { }

    public class ArrayTypeNode : TypeNode
    {
        public readonly VariableTypeNode type;

        public ArrayTypeNode(VariableTypeNode type)
        {
            this.type = type;
        }
    }

    public class FixedTypeNode : VariableTypeNode
    {
        public readonly SimpleTypeNode simpleTypeNode;

        public FixedTypeNode(SimpleTypeNode simpleTypeNode)
        {
            this.simpleTypeNode = simpleTypeNode;
        }
    }

    public class PrimitiveTypeNode : SimpleTypeNode
    {
        public readonly string type;

        public PrimitiveTypeNode(string type)
        {
            this.type = type;
        }
    }

    public class VoidNode : TypeNode
    {
        public VoidNode()
        { }
    }
}
