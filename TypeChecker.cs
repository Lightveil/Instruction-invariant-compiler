using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Compiler
{
    class TypeChecker
    {
        public Environment GenerateGlobalEnvironment(List<MethodNode> lmnode)
        {
            Environment env = new Environment();

            foreach (MethodNode mnode in lmnode)
            {
                string name = mnode.name;
                List<type> ltype = mnode.inputs.Select(x => TypeNodeToType(x.t, env)).ToList();
                env.Add(name, new FuncType(ltype, TypeNodeToType(mnode.t, env)));
            }

            return env;
        }

        public Environment GenerateMethodEnvironment(MethodNode methodNode, Environment env)
        {
            Environment newEnv = env.Duplicate();
            methodNode.inputs.ForEach(x => newEnv.Add(x.name, TypeNodeToType(x.t, newEnv)));
            return newEnv;
        }

        public bool IntermediateReturn(List<Expression> lmn)
        {
            for(int i=0;i<lmn.Count-1;i++)
            {
                if(lmn[i] is ReturnExpression)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ValidReturnType(Expression expression, type lastType, type outputType, Environment env)
        {
            bool isVoid = outputType is Void;
            bool endsWithReturn = expression is ReturnExpression;

            if (isVoid && endsWithReturn)
                throw new Exception("Void methods must not end with a return statement.");

            if (!isVoid && !endsWithReturn)
                throw new Exception("Non-void method must end with a return statement.");

            if (!isVoid && !ExpressionMatches(outputType, lastType))
                throw new Exception("The return expression must be of the same type as the input.");

            return true;
        }

        public bool TypeCheckMethods(List<MethodNode> lmnode)
        {
            Environment env = GenerateGlobalEnvironment(lmnode);

            foreach (MethodNode mnode in lmnode)
            {
                type outputType = TypeNodeToType(mnode.t, env);
                Environment newEnv = GenerateMethodEnvironment(mnode,env);

                if(IntermediateReturn(mnode.body))
                    throw new Exception("only last Expression can be a return statement");

                Expression lastExpression = null;
                type LastType = null;   

                foreach (Expression expr in mnode.body)
                {
                    lastExpression = expr;
                    LastType       = GetType(expr, newEnv);
                }

                ValidReturnType(lastExpression, LastType, outputType, env);
            }

            return true;
        }

        public bool IsFixedNumericType(type t)
        {
            return (t is FixedType ft) && (ft.simpleType is NumericType);
        }

        public bool IsFixedInt(type t)
        {
            return t is FixedType ft && ft.simpleType is IntType;
        }

        public bool ExpressionMatches(type t1, type t2)
        {
            if (t1 is ArrayType at1 && t2 is ArrayType at2)
                return AssignementMatches(at1.variableType, at2.variableType);

            if (t1 is FixedType ft)
                return ExpressionMatches(ft.simpleType, t2);

            if (t2 is FixedType ft2)
                return ExpressionMatches(t1, ft2.simpleType);

            if(t1 is PrimitiveType pt1 && t2 is PrimitiveType pt2)
            {
                return pt1.name == pt2.name;
            }

            return false;
        }

        public bool AssignementMatches(type t1, type t2)
        {
            if (t1 is FixedType && ! (t2 is FixedType))
                throw new Exception("Cannot asign a non fixed value to a fixed variable");

            return ExpressionMatches(t1,t2);
        }

        public type GetType(VariableNode vnode, Environment env)
        {
            string name = vnode.t;
            return env.GetType(name);
        }

        public bool VerifyAssignement(SimpleAssignement assign, Environment env)
        {
            type t1 = env.GetType(assign.name);
            type t2 = GetType(assign.tree,env);
            return AssignementMatches(t1, t2);
        }

        public bool VerifyInitialization(GrammarNode gnode, Environment env)
        {
            Initialization initialization = (Initialization)gnode;
            type t1 = TypeNodeToType(initialization.type, env);
            type t2 = GetType(initialization.tree, env);

            if (AssignementMatches(t1, t2))
            {
                env.Add(initialization.name,t1);
                return true;
            }

            return false;
        }

        public type GetType(FuncNode fnode, Environment env)
        {
            FuncType funcType = (FuncType)env.GetType(fnode.identifier);

            List<ITreeNode> lexp = fnode.arguments;
            List<type> ltype = funcType.inputTypes;

            if (lexp.Count != ltype.Count)
                throw new Exception("wrong number of arguments to function" + fnode.identifier);

            for(int i=0; i<ltype.Count;i++)
            {
                if (!ExpressionMatches(GetType(lexp[i],env), ltype[i]))
                    throw new Exception(lexp[i].ToString() + "is not of type " + ltype[i]);
            }

            return funcType.outputType;
        }

        public type GetType(ArrayNode anode, Environment env)
        {
            type t1 = GetType(anode.index, env);

            if (!IsFixedNumericType(t1))
            {
                throw new Exception("Array lookup for " + anode.identifier + "is not a fixed int.");
            }

            ArrayType atype = (ArrayType)env.GetType(anode.identifier);
            return atype.variableType;
        }


        public type GetType(BinaryOperationNode bon, Environment env)
        {
            type t1 = GetType(bon.left, env);
            type t2 = GetType(bon.right, env);

            if (bon.operation == "==")
            {
                return new BoolType();
            }

            if (ExpressionMatches(t1, t2))
            {
				if (t1 is FixedType && !(t2 is FixedType))
					return t2;

				return t1;
            }

            throw new Exception("type mismatch" + bon.ToString());
        }

        public type GetType(CastNode cnode, Environment env)
        {
            type t1 = TypeNodeToType(cnode.primitive, env);
            type t2 = GetType(cnode.tnode,env);

            bool b = t1 is NumericType && t2 is NumericType;

            if (!b)
                throw new Exception("Only numeric types can be casted");

            return t1;
        }


        public type GetType(GrammarNode gnode, Environment env)
        {

			switch (gnode)
			{
				case IntegerNode inode:       return new FixedType(new IntType());

				case BoolNode bnode:          return new FixedType(new BoolType());

				case ByteNode bnode:          return new FixedType(new ByteType());

				case FuncNode fnode:          return GetType(fnode, env);

				case VariableNode vnode :     return GetType(vnode, env);

				case ArrayNode anode :        return GetType(anode, env);

				case BinaryOperationNode bon: return GetType(bon, env);

                case ReturnExpression rexp:   return GetType(rexp.node, env);

                case CastNode cnode: return GetType(cnode, env);

                case ArrayCreationNode anode: return GetType(anode, env);
      

				default: break;
			}

			bool b1 = gnode is Initialization	       && VerifyInitialization((Initialization)      gnode, env);
			bool b2 = gnode is SimpleAssignement       && VerifyAssignement((SimpleAssignement)      gnode, env);
			bool b3 = gnode is ArrayAssign		       && VerifyArrayAssignement((ArrayAssign)	   gnode, env);
			bool b4 = gnode is LoopNode lnode		   && VerifyLoopNode(lnode, env);
            bool b5 = gnode is DeclarationNode dnode   && VerifyDeclarationNode(dnode,env);

			if (b1 || b2 || b3 || b4 || b5)
				return new Void();

            throw new Exception(gnode + " does not type check.");
        }




        public bool VerifyDeclarationNode(DeclarationNode dnode, Environment env)
        {
            type t = TypeNodeToType(dnode.t,env);
            env.Add(dnode.name, t);
            return true;
        }

        public type GetType(ArrayCreationNode arrayCreationNode, Environment env)
        {
            if (!IsFixedNumericType(GetType(arrayCreationNode.size, env)))
                throw new Exception("array length must be of fixed numeric size");

            var type = TypeNodeToType(arrayCreationNode.primitiveTypeNode, env);
            return new ArrayType((VariableType)type);
        }


        private bool VerifyLoopNode(LoopNode lnode, Environment env)
        {
			Func<GrammarNode, bool> f = gnode => !IsFixedInt(GetType(gnode, env));

			string name = lnode.name;
            type t = TypeNodeToType(lnode.typeNode, env);
            List<Expression> lexp = lnode.lexpr;

			Environment newEnv = env.Duplicate();
            newEnv.Add(name, new FixedType(new IntType()));

            if (f(lnode.init) || f(lnode.times))
                return false;

            foreach (Expression exp in lexp)
            {
                GetType(exp, newEnv);
            }

            return true;
        }

        private bool VerifyArrayAssignement(ArrayAssign assign, Environment env)
        {
            type t1 = GetType(assign.index, env);

            if (!IsFixedInt(t1))
                return false;

            ArrayType atype = (ArrayType) env.GetType(assign.name);

            return AssignementMatches(atype.variableType, GetType(assign.tree,env));
        }

        public type TypeNodeToType(TypeNode itype, Environment env)
        {
            if (itype is PrimitiveTypeNode p)
            {
                switch (p.type)
                {
                    case "int":  return new IntType();
                    case "bool": return new BoolType();
                    case "byte": return new ByteType();
                }
            }

            switch (itype)
			{
                case FixedTypeNode ftn:  return new FixedType((SimpleType)TypeNodeToType(ftn.simpleTypeNode,env));
				case ArrayTypeNode atn:  return new ArrayType((VariableType)TypeNodeToType(atn.type, env));
                case VoidNode vn:       return new Void();
			}

            throw new InvalidOperationException();
        }
    }



  
    class Environment
    {
        Dictionary<string, type> dict;

        public Environment()
        {
            dict = new Dictionary<string, type>();
        }

        public bool Undefined(string var)
        {
            return !dict.ContainsKey(var);
        }

        public type GetType(string name)
        {
            if (!dict.ContainsKey(name))
               throw new Exception("variable " + name + " is undefined. ");

            return dict[name];
        }

        public Environment Duplicate()
        {
            Environment clone = new Environment();

            foreach(string str in dict.Keys)
            {
                clone.Add(str, dict[str]);
            }

            return clone;
        }

        public void Add(string str, type t)
        {
            if (dict.ContainsKey(str))
                throw new Exception("variable " + str + " is already defined.");

            dict.Add(str, t);
        }

        public void Remove(string str)
        {
            dict.Remove(str);
        }
    }
}
