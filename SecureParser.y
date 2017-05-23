%namespace Parsing
%output=SecureParser.cs 
%using System.Collections;
%using System.IO;
%using Compiler;


/* Code in the following section is included in the generated parser */
%{
	public List<MethodNode> Result
	{
		private set;
		get;
	}
%}

%union
{
    public string              tstr;
    public int                 tint;
	public byte                tbyte;
	public ITreeNode 		   tnode;
	public bool tbool;

	public TypeNode           itype;
	public VariableTypeNode   vtype;
	public SimpleTypeNode	  ttype; 

	public Expression 		  texpr;
	public Assign			  tassign;
	public FuncNode			  tfunc;
	public PrimitiveTypeNode  prim;
	
	public List<ITreeNode>		   litn;
	public List<Expression>		   lexpr;
	public MethodNode			   mnode;
	public List<DeclarationNode>   ldecl;
	public List<MethodNode>		   lmnode;
}

/* tokens with values: */
%token <tstr> IDENTIFIER
%token <tint>  INTEGER_LITERAL
%token <tbyte> BYTE_LITERAL
%token <tstr> BOP

/* Type declaration */
%type  <tstr>      Identifier
%type <tnode>      Tree
%type <tnode>      Func
%type <tbool>      Boolean

%type <itype>      Types
%type <itype>      MethodTypes
%type <vtype>      VType
%type <ttype>      SType
%type <prim>       Num

%type <texpr>      Expr
%type <tassign>    Assignement
%type <tfunc>      Func

%type <litn>       Arguments
%type <lexpr>      Body
%type <mnode>      Method
%type <ldecl>      Input
%type <lmnode>     Methods


/* Terminals (tokens returned by the scanner) */

/* Keywords */
%token Fixed, Loop, Return, Secure, New

/* types */
%token Int, Byte, Bool, Void

/* delimiters: */
%token LP, RP , LCB , RCB, LSB, RSB, Comma, SemiColon

/* Boolean */
%token Equal, True, False

%left BOP
 
%start Beginning
%%

//////////////////////////////////////////////////////////////////////////

Identifier  : IDENTIFIER { $$ = $1; };


Tree : LP Tree RP        			      { $$ = $2; }
     | Tree BOP Tree     				  { $$ = new BinaryOperationNode($2,$1,$3) ;}
	 | Boolean                            { $$ = new BoolNode($1);		 }
	 | Identifier        				  { $$ = new VariableNode($1);	 }
	 | INTEGER_LITERAL   				  { $$ = new IntegerNode($1);	 }
	 | BYTE_LITERAL   				      { $$ = new ByteNode($1);  	 }
	 | Identifier LSB Tree RSB 			  { $$ = new ArrayNode($1,$3);	 }
	 | Func 							  { $$ = $1 ;}
	 | New VType LSB Tree RSB             { $$ = new ArrayCreationNode((VariableTypeNode)$2,$4); }
	 | LP Num RP Tree                     { $$ = new CastNode((PrimitiveTypeNode)$2,$4);}
	 ;

Func : Identifier LP Arguments RP {$$ = new FuncNode($1,$3);}
	 | Identifier LP RP {$$ = new FuncNode($1,new List<ITreeNode>());}
	 ;
	 
Boolean : True  {$$ = true;}
        | False {$$ = false;}
	    ;  	 
		
////////////////////////////////////////////////////////////////// 

Arguments : Tree {List<ITreeNode> l = new List<ITreeNode>(); l.Add($1); $$ = l;}
          | Arguments Comma Tree {List<ITreeNode> l = $1; l.Add($3); $$ = l;}
		  ;
		
Types   : VType LSB RSB { $$   = new ArrayTypeNode($1);}
        | VType { $$ = $1;}
		;
	
VType   : Fixed SType {$$ = new FixedTypeNode($2);}
        | SType       {$$ = $1;}
		;		

SType  : Bool       {$$ = new PrimitiveTypeNode("bool");}
       | Num        {$$ = $1;}	   
	   ;
	 
Num   : Int        { $$    = new PrimitiveTypeNode("int"); }  
      | Byte       { $$    = new PrimitiveTypeNode("byte"); }
	  ;

//////////////////////////////////////////////////////////////////
	  
Assignement : Identifier LSB Tree RSB Equal Tree SemiColon    {$$ = new ArrayAssign($1,$3,$6);}
			| Types Identifier        Equal Tree SemiColon    {$$ = new Initialization($1,$2,$4);}
            | Identifier              Equal Tree SemiColon    {$$ = new SimpleAssignement($1,$3);}
			;   

Expr        : Assignement {$$ = $1;}
			| Types Identifier SemiColon {$$ = new DeclarationNode($1,$2);}
            | Func SemiColon {$$ = $1;} 
			| Loop LP Num Identifier Equal Tree Comma Tree  
			  RP LCB Body RCB {$$ = new LoopNode($3,$4,$6,$8,$11);}
			| Return Tree SemiColon {$$ = new ReturnExpression($2);}
			;	  

Body        :  Expr {List<Expression> l = new List<Expression>(); l.Add($1); $$ = l;} 
            |  Body Expr {List<Expression> l = $1; l.Add($2); $$ = l;}
			;

Method      : Secure MethodTypes Identifier  LP Input  RP LCB Body RCB {$$ = new MethodNode($2,$3,$5,$8);}
            | Secure MethodTypes Identifier  LP RP LCB Body RCB 
			  {$$ = new MethodNode($2,$3,new List<DeclarationNode>(),$7);}
			;

MethodTypes : Void {$$ = new VoidNode();}
            | Types {$$ = $1;}
			;

Input       : Types Identifier {List<DeclarationNode> l = new List<DeclarationNode>(); l.Add(new DeclarationNode($1,$2)); $$ = l; }
            | Input Comma Types Identifier {List<DeclarationNode> l = $1; l.Add(new DeclarationNode($3,$4)); $$ = l;   }
			;

Methods     : Method {List<MethodNode> l = new List<MethodNode>(); l.Add($1); $$ = l; }
            | Methods Method {List<MethodNode> l = $1; l.Add($2); $$ = l;}
            ;


Beginning   : Methods {Result = $1;};
//////////////////////////////////////////////////////////////////////			
%%

private Parser(ScanBase scanner) : base(scanner) { }

static public Parser CreateParser(Stream source)
{
	return  new Parser(new Scanner(source));
}