using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class DeclarationNode : Expression
    {
        public readonly TypeNode t;
        public readonly string name;

        public DeclarationNode(TypeNode t, string name)
        {
            this.t = t;
            this.name = name;
        }
    }


    public class LoopNode : Expression
    {
        public readonly TypeNode typeNode;
        public readonly string name;
        public readonly ITreeNode init;
        public readonly ITreeNode times;
        public List<Expression> lexpr;

        public LoopNode(TypeNode typeNode, string name, ITreeNode init, ITreeNode times, List<Expression> lexpr)
        {
            this.typeNode = typeNode;
            this.name = name;
            this.lexpr = lexpr;
            this.init = init;
            this.times = times;
        }
    }


    public class ReturnExpression : Expression
    {
        public readonly ITreeNode node;

        public ReturnExpression(ITreeNode node)
        {
            this.node = node;
        }
    }
}
