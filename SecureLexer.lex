%using QUT.Gppg;
%namespace Parsing
%option verbose, summary, minimize, parser

/* Helper definitions */
letter      [a-zA-Z]
digit       [0-9]
white       [ \t]
eol         (\r\n?|\n)
CPPComment  \/\*
CPPCommentEnd  \*+\/
CComment    \/\/
bstart      0x
%%

/* Tokens definitions */
/* (put here so that reserved words take precedence over identifiers) */

"bool"    { return (int)Tokens.Bool ;  }
"byte"    { return (int)Tokens.Byte ;  }
"int"     { return (int)Tokens.Int ;   }

"fixed"   { return (int)Tokens.Fixed ; }
"void"    { return (int)Tokens.Void ;  }
"new"     { return (int)Tokens.New ;   }
"secure"  { return (int)Tokens.Secure ;}
"loop"    { return (int)Tokens.Loop ;  }
"return"  { return (int)Tokens.Return; }


"false"   { return (int)Tokens.False ;}
"true"    { return (int)Tokens.True ;}


/* arithemtic operators */
"+"       { yylval.tstr = yytext; return (int)Tokens.BOP;}
"*"       { yylval.tstr = yytext; return (int)Tokens.BOP;}
"-"       { yylval.tstr = yytext; return (int)Tokens.BOP;}
">="      { yylval.tstr = yytext; return (int)Tokens.BOP;}
"=<"      { yylval.tstr = yytext; return (int)Tokens.BOP;}
"<<"      { yylval.tstr = yytext; return (int)Tokens.BOP;}
">>"      { yylval.tstr = yytext; return (int)Tokens.BOP;}
"="       { return (int)Tokens.Equal;}
">"       { yylval.tstr = yytext; return (int)Tokens.BOP;}
"<"       { yylval.tstr = yytext; return (int)Tokens.BOP;}

/* Equality operators */
"=="      { yylval.tstr = yytext; return (int)Tokens.BOP;}
"!="      { yylval.tstr = yytext; return (int)Tokens.BOP;}

/* Boolean operators */
"&&"      { yylval.tstr = yytext; return (int)Tokens.BOP;}
"||"      { yylval.tstr = yytext; return (int)Tokens.BOP;}
"^"       { yylval.tstr = yytext; return (int)Tokens.BOP;}

/* delimiters */
"(" { return (int)Tokens.LP;    }
")" { return (int)Tokens.RP;    }
"[" { return (int)Tokens.LSB;   }
"]" { return (int)Tokens.RSB;   }
"{" { return (int)Tokens.LCB;   }
"}" { return (int)Tokens.RCB;   }
"," { return (int)Tokens.Comma; }
";" { return (int)Tokens.SemiColon; }

/* identifiers */
{letter}({letter}|{digit}|_)* { yylval.tstr = yytext; return (int)Tokens.IDENTIFIER;}

/* integer */
{digit}{digit}* { yylval.tint = Int32.Parse(yytext); return (int)Tokens.INTEGER_LITERAL;}

/* byte */
{bstart}({digit}|{letter})* { yylval.tbyte = Byte.Parse(yytext.Substring(2),NumberStyles.HexNumber); return (int)Tokens.BYTE_LITERAL;}

/* whitespace */
{white}+ { /* ignore whitespace */ }

/* Comments */
{CPPComment}([^*]*|(\*+[^/*]))*{CPPCommentEnd}     { /*
("Encountered CPP COMMENT: " + yytext); */}
{CComment}.*{eol}? { /*Console.WriteLine("Encountered C-Style COMMENT: " + yytext);*/ }

/* lexical errors (put last so other matches take precedence) */
. { Console.WriteLine(
	"\nunexpected character in input: '" + yytext + "' at line " +
	(yyline+1) + " column " + (yycol+1));
}
  
%{
yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
%} 

  
%%

public override void yyerror(string format, params object[] args)
{
     Console.WriteLine("Unexpected error at line {0} while processing text '{1}' - Parser error: \r\n\t {2}\r\n", yyline, yytext, String.Format(format, args)); 
}
 