using System;
using System.Collections;
using System.Collections.Generic;

namespace Compiler
{
    public interface GrammarNode
    { }

    public interface ITreeNode : GrammarNode
    {}

    public interface Expression : GrammarNode
    { }

    public abstract class GenericTypeNode<T> : ITreeNode
    {
        public readonly T t;

        public GenericTypeNode(T t)
        {
            this.t = t;
        }
    }

    public class CastNode : ITreeNode
    {
        public readonly PrimitiveTypeNode primitive;
        public readonly ITreeNode tnode;

        public CastNode(PrimitiveTypeNode primitive, ITreeNode tnode)
        {
            this.primitive = primitive;
            this.tnode = tnode;
        }
    }

    public class IntegerNode : GenericTypeNode<int>
    {
        public IntegerNode(int x) : base(x)
        { }
    }

    public class BoolNode : GenericTypeNode<bool>
    {
        public BoolNode(bool b) : base(b)
        { }
    }

    public class ByteNode : GenericTypeNode<byte>
    {
        public ByteNode(byte b) : base(b)
        { }
    }

    public class VariableNode : GenericTypeNode<string>
    {
        public VariableNode(string x) : base(x)
        { }
    }

    public class BinaryOperationNode : ITreeNode
    {
        public readonly string operation;
        public readonly ITreeNode left;
        public readonly ITreeNode right;

        public BinaryOperationNode(string operation, ITreeNode left, ITreeNode right)
        {
            this.operation = operation;
            this.left = left;
            this.right = right;
        }
    }

    public class ArrayNode : ITreeNode
    {
        public readonly string identifier;
        public readonly ITreeNode index;

        public ArrayNode(string identifier, ITreeNode index)
        {
            this.identifier = identifier;
            this.index = index;
        }
    }

    public class FuncNode : ITreeNode, Expression
	{
        public readonly string identifier;
        public readonly List<ITreeNode> arguments;

        public FuncNode(string identifier, List<ITreeNode> arguments)
        {
            this.identifier = identifier;
            this.arguments = arguments;
        }
    }

    public class ArrayCreationNode : ITreeNode
    {
        public readonly VariableTypeNode primitiveTypeNode;
        public readonly ITreeNode size; 

        public ArrayCreationNode(VariableTypeNode variableTypeNode, ITreeNode size)
        {
            this.primitiveTypeNode = variableTypeNode;
            this.size = size;
        }
    }

    public class MethodNode : Expression
    {
        public readonly TypeNode t;
        public readonly string name;
        public readonly List<DeclarationNode> inputs;
        public readonly List<Expression> body;

        public MethodNode(TypeNode t, string name, List<DeclarationNode> inputs, List<Expression> body)
        {
            this.t = t;
            this.name = name;
            this.inputs = inputs;
            this.body = body;
        }
    }
}