using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    interface type
    { }

    interface VariableType : type
    { }

    interface SimpleType : VariableType
    { }

    class ArrayType : type
    {
        public readonly VariableType variableType;

        public ArrayType(VariableType variableType)
        {
            this.variableType = variableType;
        }
    }

    class FixedType : VariableType
    {
        public readonly SimpleType simpleType ;

        public FixedType(SimpleType simpleType)
        {
            this.simpleType = simpleType;
        }
    }

    class FuncType : type
    {
        public readonly List<type> inputTypes;
        public readonly type outputType;

        public FuncType(List<type> inputTypes, type outputType)
        {
            this.inputTypes = inputTypes;
            this.outputType = outputType;
        }
    }

    class PrimitiveType : SimpleType
    {
        public readonly string name;

        public PrimitiveType(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }

    class NumericType : PrimitiveType
    {
        public NumericType(string str) : base(str)
        { }
    }

    class IntType : NumericType
    {
        public IntType() : base("int")
        { }
    }

    class ByteType : NumericType
    {
        public ByteType() : base("byte")
        { }
    }

    class BoolType : PrimitiveType
    {
        public BoolType() : base("bool")
        { }
    }

    class Void : type
    {
        public override string ToString()
        {
            return "Void";
        }
    }
}
