using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface Assign : Expression
    { }

    public class SimpleAssignement : Assign
    {
        public readonly string name;
        public readonly ITreeNode tree;

        public SimpleAssignement(string name, ITreeNode tree)
        {
            this.name = name;
            this.tree = tree;
        }
    }

    public class Initialization : SimpleAssignement
    {
        public readonly TypeNode type;

        public Initialization(TypeNode type, string name, ITreeNode tree)
            : base(name, tree)
        {
            this.type = type;
        }
    }


    public class ArrayAssign : Assign
    {
        public readonly string name;
        public readonly ITreeNode tree;
        public readonly ITreeNode index;

        public ArrayAssign(string name, ITreeNode index, ITreeNode tree)
        {
            this.name = name;
            this.index = index;
            this.tree = tree;
        }
    }
}
