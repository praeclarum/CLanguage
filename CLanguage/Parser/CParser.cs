// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "CParser.jay"
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using CLanguage.Syntax;
using CLanguage.Types;

#pragma warning disable 162

namespace CLanguage.Parser
{
	/// <summary>
	///    The C Parser
	///    http://www.quut.com/c/ANSI-C-grammar-y.html
	/// </summary>
	public partial class CParser
	{

		
#line default

  /** error output stream.
      It should be changeable.
    */
  public System.IO.TextWriter ErrorOutput = System.Console.Out;

  /** simplified error message.
      @see <a href="#yyerror(java.lang.String, java.lang.String[])">yyerror</a>
    */
  public void yyerror (string message) {
    yyerror(message, null);
  }

  /* An EOF token */
  public int eof_token;

  /** (syntax) error message.
      Can be overwritten to control message format.
      @param message text to be displayed.
      @param expected vector of acceptable tokens, if available.
    */
  public void yyerror (string message, string[] expected) {
    if ((yacc_verbose_flag > 0) && (expected != null) && (expected.Length  > 0)) {
      ErrorOutput.Write (message+", expecting");
      for (int n = 0; n < expected.Length; ++ n)
        ErrorOutput.Write (" "+expected[n]);
        ErrorOutput.WriteLine ();
    } else
      ErrorOutput.WriteLine (message);
  }

  /** debugging support, requires the package jay.yydebug.
      Set to null to suppress debugging messages.
    */
//t  internal yydebug.yyDebug debug;

  protected const int yyFinal = 27;
//t // Put this array into a separate class so it is only initialized if debugging is actually used
//t // Use MarshalByRefObject to disable inlining
//t class YYRules : MarshalByRefObject {
//t  public static readonly string [] yyRule = {
//t    "$accept : translation_unit",
//t    "primary_expression : IDENTIFIER",
//t    "primary_expression : CONSTANT",
//t    "primary_expression : STRING_LITERAL",
//t    "primary_expression : '(' expression ')'",
//t    "postfix_expression : primary_expression",
//t    "postfix_expression : postfix_expression '[' expression ']'",
//t    "postfix_expression : postfix_expression '(' ')'",
//t    "postfix_expression : postfix_expression '(' argument_expression_list ')'",
//t    "postfix_expression : postfix_expression '.' IDENTIFIER",
//t    "postfix_expression : postfix_expression PTR_OP IDENTIFIER",
//t    "postfix_expression : postfix_expression INC_OP",
//t    "postfix_expression : postfix_expression DEC_OP",
//t    "postfix_expression : '(' type_name ')' '{' initializer_list '}'",
//t    "postfix_expression : '(' type_name ')' '{' initializer_list ',' '}'",
//t    "argument_expression_list : assignment_expression",
//t    "argument_expression_list : argument_expression_list ',' assignment_expression",
//t    "unary_expression : postfix_expression",
//t    "unary_expression : INC_OP unary_expression",
//t    "unary_expression : DEC_OP unary_expression",
//t    "unary_expression : '&' cast_expression",
//t    "unary_expression : '*' cast_expression",
//t    "unary_expression : unary_operator cast_expression",
//t    "unary_expression : SIZEOF unary_expression",
//t    "unary_expression : SIZEOF '(' type_name ')'",
//t    "unary_operator : '+'",
//t    "unary_operator : '-'",
//t    "unary_operator : '~'",
//t    "unary_operator : '!'",
//t    "cast_expression : unary_expression",
//t    "cast_expression : '(' type_name ')' cast_expression",
//t    "multiplicative_expression : cast_expression",
//t    "multiplicative_expression : multiplicative_expression '*' cast_expression",
//t    "multiplicative_expression : multiplicative_expression '/' cast_expression",
//t    "multiplicative_expression : multiplicative_expression '%' cast_expression",
//t    "additive_expression : multiplicative_expression",
//t    "additive_expression : additive_expression '+' multiplicative_expression",
//t    "additive_expression : additive_expression '-' multiplicative_expression",
//t    "shift_expression : additive_expression",
//t    "shift_expression : shift_expression LEFT_OP additive_expression",
//t    "shift_expression : shift_expression RIGHT_OP additive_expression",
//t    "relational_expression : shift_expression",
//t    "relational_expression : relational_expression '<' shift_expression",
//t    "relational_expression : relational_expression '>' shift_expression",
//t    "relational_expression : relational_expression LE_OP shift_expression",
//t    "relational_expression : relational_expression GE_OP shift_expression",
//t    "equality_expression : relational_expression",
//t    "equality_expression : equality_expression EQ_OP relational_expression",
//t    "equality_expression : equality_expression NE_OP relational_expression",
//t    "and_expression : equality_expression",
//t    "and_expression : and_expression '&' equality_expression",
//t    "exclusive_or_expression : and_expression",
//t    "exclusive_or_expression : exclusive_or_expression '^' and_expression",
//t    "inclusive_or_expression : exclusive_or_expression",
//t    "inclusive_or_expression : inclusive_or_expression '|' exclusive_or_expression",
//t    "logical_and_expression : inclusive_or_expression",
//t    "logical_and_expression : logical_and_expression AND_OP inclusive_or_expression",
//t    "logical_or_expression : logical_and_expression",
//t    "logical_or_expression : logical_or_expression OR_OP logical_and_expression",
//t    "conditional_expression : logical_or_expression",
//t    "conditional_expression : logical_or_expression '?' expression ':' conditional_expression",
//t    "assignment_expression : conditional_expression",
//t    "assignment_expression : unary_expression assignment_operator assignment_expression",
//t    "assignment_operator : '='",
//t    "assignment_operator : MUL_ASSIGN",
//t    "assignment_operator : DIV_ASSIGN",
//t    "assignment_operator : MOD_ASSIGN",
//t    "assignment_operator : ADD_ASSIGN",
//t    "assignment_operator : SUB_ASSIGN",
//t    "assignment_operator : LEFT_ASSIGN",
//t    "assignment_operator : RIGHT_ASSIGN",
//t    "assignment_operator : AND_ASSIGN",
//t    "assignment_operator : XOR_ASSIGN",
//t    "assignment_operator : OR_ASSIGN",
//t    "expression : assignment_expression",
//t    "expression : expression ',' assignment_expression",
//t    "constant_expression : conditional_expression",
//t    "declaration : declaration_specifiers ';'",
//t    "declaration : declaration_specifiers init_declarator_list ';'",
//t    "declaration_specifiers : storage_class_specifier",
//t    "declaration_specifiers : storage_class_specifier declaration_specifiers",
//t    "declaration_specifiers : type_specifier",
//t    "declaration_specifiers : type_specifier declaration_specifiers",
//t    "declaration_specifiers : type_qualifier",
//t    "declaration_specifiers : type_qualifier declaration_specifiers",
//t    "declaration_specifiers : function_specifier",
//t    "declaration_specifiers : function_specifier declaration_specifiers",
//t    "init_declarator_list : init_declarator",
//t    "init_declarator_list : init_declarator_list ',' init_declarator",
//t    "init_declarator : declarator",
//t    "init_declarator : declarator '=' initializer",
//t    "storage_class_specifier : TYPEDEF",
//t    "storage_class_specifier : EXTERN",
//t    "storage_class_specifier : STATIC",
//t    "storage_class_specifier : AUTO",
//t    "storage_class_specifier : REGISTER",
//t    "type_specifier : VOID",
//t    "type_specifier : CHAR",
//t    "type_specifier : SHORT",
//t    "type_specifier : INT",
//t    "type_specifier : LONG",
//t    "type_specifier : FLOAT",
//t    "type_specifier : DOUBLE",
//t    "type_specifier : SIGNED",
//t    "type_specifier : UNSIGNED",
//t    "type_specifier : BOOL",
//t    "type_specifier : COMPLEX",
//t    "type_specifier : IMAGINARY",
//t    "type_specifier : struct_or_union_specifier",
//t    "type_specifier : enum_specifier",
//t    "type_specifier : TYPE_NAME",
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER compound_statement",
//t    "struct_or_union_specifier : struct_or_union compound_statement",
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER",
//t    "struct_or_union : STRUCT",
//t    "struct_or_union : CLASS",
//t    "struct_or_union : UNION",
//t    "specifier_qualifier_list : type_specifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_specifier",
//t    "specifier_qualifier_list : type_qualifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_qualifier",
//t    "enum_specifier : ENUM '{' enumerator_list '}'",
//t    "enum_specifier : ENUM IDENTIFIER '{' enumerator_list '}'",
//t    "enum_specifier : ENUM '{' enumerator_list ',' '}'",
//t    "enum_specifier : ENUM IDENTIFIER '{' enumerator_list ',' '}'",
//t    "enum_specifier : ENUM IDENTIFIER",
//t    "enumerator_list : enumerator",
//t    "enumerator_list : enumerator_list ',' enumerator",
//t    "enumerator : IDENTIFIER",
//t    "enumerator : IDENTIFIER '=' constant_expression",
//t    "function_specifier : INLINE",
//t    "declarator : pointer direct_declarator",
//t    "declarator : direct_declarator",
//t    "direct_declarator : IDENTIFIER",
//t    "direct_declarator : '(' declarator ')'",
//t    "direct_declarator : direct_declarator '[' type_qualifier_list assignment_expression ']'",
//t    "direct_declarator : direct_declarator '[' type_qualifier_list ']'",
//t    "direct_declarator : direct_declarator '[' assignment_expression ']'",
//t    "direct_declarator : direct_declarator '[' STATIC type_qualifier_list assignment_expression ']'",
//t    "direct_declarator : direct_declarator '[' type_qualifier_list STATIC assignment_expression ']'",
//t    "direct_declarator : direct_declarator '[' type_qualifier_list '*' ']'",
//t    "direct_declarator : direct_declarator '[' '*' ']'",
//t    "direct_declarator : direct_declarator '[' ']'",
//t    "direct_declarator : direct_declarator '(' parameter_type_list ')'",
//t    "direct_declarator : direct_declarator '(' identifier_list ')'",
//t    "direct_declarator : direct_declarator '(' ')'",
//t    "pointer : '*'",
//t    "pointer : '*' type_qualifier_list",
//t    "pointer : '*' pointer",
//t    "pointer : '*' type_qualifier_list pointer",
//t    "type_qualifier_list : type_qualifier",
//t    "type_qualifier_list : type_qualifier_list type_qualifier",
//t    "type_qualifier : CONST",
//t    "type_qualifier : RESTRICT",
//t    "type_qualifier : VOLATILE",
//t    "parameter_type_list : parameter_list",
//t    "parameter_type_list : parameter_list ',' ELLIPSIS",
//t    "parameter_list : parameter_declaration",
//t    "parameter_list : parameter_list ',' parameter_declaration",
//t    "parameter_declaration : declaration_specifiers declarator",
//t    "parameter_declaration : declaration_specifiers abstract_declarator",
//t    "parameter_declaration : declaration_specifiers",
//t    "identifier_list : IDENTIFIER",
//t    "identifier_list : identifier_list ',' IDENTIFIER",
//t    "type_name : specifier_qualifier_list",
//t    "type_name : specifier_qualifier_list abstract_declarator",
//t    "abstract_declarator : pointer",
//t    "abstract_declarator : direct_abstract_declarator",
//t    "abstract_declarator : pointer direct_abstract_declarator",
//t    "direct_abstract_declarator : '(' abstract_declarator ')'",
//t    "direct_abstract_declarator : '[' ']'",
//t    "direct_abstract_declarator : '[' assignment_expression ']'",
//t    "direct_abstract_declarator : direct_abstract_declarator '[' ']'",
//t    "direct_abstract_declarator : direct_abstract_declarator '[' assignment_expression ']'",
//t    "direct_abstract_declarator : '[' '*' ']'",
//t    "direct_abstract_declarator : direct_abstract_declarator '[' '*' ']'",
//t    "direct_abstract_declarator : '(' ')'",
//t    "direct_abstract_declarator : '(' parameter_type_list ')'",
//t    "direct_abstract_declarator : direct_abstract_declarator '(' ')'",
//t    "direct_abstract_declarator : direct_abstract_declarator '(' parameter_type_list ')'",
//t    "initializer : assignment_expression",
//t    "initializer : '{' initializer_list '}'",
//t    "initializer : '{' initializer_list ',' '}'",
//t    "initializer_list : initializer",
//t    "initializer_list : designation initializer",
//t    "initializer_list : initializer_list ',' initializer",
//t    "initializer_list : initializer_list ',' designation initializer",
//t    "designation : designator_list '='",
//t    "designator_list : designator",
//t    "designator_list : designator_list designator",
//t    "designator : '[' constant_expression ']'",
//t    "designator : '.' IDENTIFIER",
//t    "statement : labeled_statement",
//t    "statement : compound_statement",
//t    "statement : expression_statement",
//t    "statement : selection_statement",
//t    "statement : iteration_statement",
//t    "statement : jump_statement",
//t    "labeled_statement : IDENTIFIER ':' statement",
//t    "labeled_statement : CASE constant_expression ':' statement",
//t    "labeled_statement : DEFAULT ':' statement",
//t    "compound_statement : '{' '}'",
//t    "compound_statement : '{' block_item_list '}'",
//t    "block_item_list : block_item",
//t    "block_item_list : block_item_list block_item",
//t    "block_item : declaration",
//t    "block_item : statement",
//t    "expression_statement : ';'",
//t    "expression_statement : expression ';'",
//t    "selection_statement : IF '(' expression ')' statement",
//t    "selection_statement : IF '(' expression ')' statement ELSE statement",
//t    "selection_statement : SWITCH '(' expression ')' statement",
//t    "iteration_statement : WHILE '(' expression ')' statement",
//t    "iteration_statement : DO statement WHILE '(' expression ')' ';'",
//t    "iteration_statement : FOR '(' expression_statement expression_statement ')' statement",
//t    "iteration_statement : FOR '(' expression_statement expression_statement expression ')' statement",
//t    "iteration_statement : FOR '(' declaration expression_statement ')' statement",
//t    "iteration_statement : FOR '(' declaration expression_statement expression ')' statement",
//t    "jump_statement : GOTO IDENTIFIER ';'",
//t    "jump_statement : CONTINUE ';'",
//t    "jump_statement : BREAK ';'",
//t    "jump_statement : RETURN ';'",
//t    "jump_statement : RETURN expression ';'",
//t    "translation_unit : external_declaration",
//t    "translation_unit : translation_unit external_declaration",
//t    "external_declaration : function_definition",
//t    "external_declaration : declaration",
//t    "function_definition : declaration_specifiers declarator declaration_list compound_statement",
//t    "function_definition : declaration_specifiers declarator compound_statement",
//t    "declaration_list : declaration",
//t    "declaration_list : declaration_list declaration",
//t  };
//t public static string getRule (int index) {
//t    return yyRule [index];
//t }
//t}
  protected static readonly string [] yyNames = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"'!'",null,null,null,"'%'","'&'",
    null,"'('","')'","'*'","'+'","','","'-'","'.'","'/'",null,null,null,
    null,null,null,null,null,null,null,"':'","';'","'<'","'='","'>'",
    "'?'",null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"'['",null,"']'","'^'",null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,"'{'","'|'","'}'","'~'",null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"IDENTIFIER","CONSTANT","STRING_LITERAL","SIZEOF","PTR_OP",
    "INC_OP","DEC_OP","LEFT_OP","RIGHT_OP","LE_OP","GE_OP","EQ_OP",
    "NE_OP","AND_OP","OR_OP","MUL_ASSIGN","DIV_ASSIGN","MOD_ASSIGN",
    "ADD_ASSIGN","SUB_ASSIGN","LEFT_ASSIGN","RIGHT_ASSIGN","AND_ASSIGN",
    "XOR_ASSIGN","OR_ASSIGN","TYPE_NAME","TYPEDEF","EXTERN","STATIC",
    "AUTO","REGISTER","INLINE","RESTRICT","CHAR","SHORT","INT","LONG",
    "SIGNED","UNSIGNED","FLOAT","DOUBLE","CONST","VOLATILE","VOID","BOOL",
    "COMPLEX","IMAGINARY","STRUCT","CLASS","UNION","ENUM","ELLIPSIS",
    "CASE","DEFAULT","IF","ELSE","SWITCH","WHILE","DO","FOR","GOTO",
    "CONTINUE","BREAK","RETURN",
  };

  /** index-checked interface to yyNames[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
//t  public static string yyname (int token) {
//t    if ((token < 0) || (token > yyNames.Length)) return "[illegal]";
//t    string name;
//t    if ((name = yyNames[token]) != null) return name;
//t    return "[unknown]";
//t  }

  int yyExpectingState;
  /** computes list of expected tokens on error by tracing the tables.
      @param state for which to compute the list.
      @return list of token names.
    */
  protected int [] yyExpectingTokens (int state){
    int token, n, len = 0;
    bool[] ok = new bool[yyNames.Length];
    if ((n = yySindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    if ((n = yyRindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    int [] result = new int [len];
    for (n = token = 0; n < len;  ++ token)
      if (ok[token]) result[n++] = token;
    return result;
  }
  protected string[] yyExpecting (int state) {
    int [] tokens = yyExpectingTokens (state);
    string [] result = new string[tokens.Length];
    for (int n = 0; n < tokens.Length;  n++)
      result[n++] = yyNames[tokens [n]];
    return result;
  }

  /** the generated parser, with debugging messages.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @param yydebug debug message writer implementing yyDebug, or null.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  internal Object yyparse (yyParser.yyInput yyLex, Object yyd)
				 {
//t    this.debug = (yydebug.yyDebug)yyd;
    return yyparse(yyLex);
  }

  /** initial size and increment of the state/value stack [default 256].
      This is not final so that it can be overwritten outside of invocations
      of yyparse().
    */
  protected int yyMax;

  /** executed at the beginning of a reduce action.
      Used as $$ = yyDefault($1), prior to the user-specified action, if any.
      Can be overwritten to provide deep copy, etc.
      @param first value for $1, or null.
      @return first.
    */
  protected Object yyDefault (Object first) {
    return first;
  }

	static int[] global_yyStates;
	static object[] global_yyVals;
	protected bool use_global_stacks;
	object[] yyVals;					// value stack
	object yyVal;						// value stack ptr
	int yyToken;						// current input
	int yyTop;

  /** the generated parser.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  internal Object yyparse (yyParser.yyInput yyLex)
  {
    if (yyMax <= 0) yyMax = 256;		// initial size
    int yyState = 0;                   // state stack ptr
    int [] yyStates;               	// state stack 
    yyVal = null;
    yyToken = -1;
    int yyErrorFlag = 0;				// #tks to shift
	if (use_global_stacks && global_yyStates != null) {
		yyVals = global_yyVals;
		yyStates = global_yyStates;
   } else {
		yyVals = new object [yyMax];
		yyStates = new int [yyMax];
		if (use_global_stacks) {
			global_yyVals = yyVals;
			global_yyStates = yyStates;
		}
	}

    /*yyLoop:*/ for (yyTop = 0;; ++ yyTop) {
      if (yyTop >= yyStates.Length) {			// dynamically increase
        global::System.Array.Resize (ref yyStates, yyStates.Length+yyMax);
        global::System.Array.Resize (ref yyVals, yyVals.Length+yyMax);
      }
      yyStates[yyTop] = yyState;
      yyVals[yyTop] = yyVal;
//t      if (debug != null) debug.push(yyState, yyVal);

      /*yyDiscarded:*/ while (true) {	// discarding a token does not change stack
        int yyN;
        if ((yyN = yyDefRed[yyState]) == 0) {	// else [default] reduce (yyN)
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
//t            if (debug != null)
//t              debug.lex(yyState, yyToken, yyname(yyToken), yyLex.value());
          }
          if ((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
              && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
//t            if (debug != null)
//t              debug.shift(yyState, yyTable[yyN], yyErrorFlag-1);
            yyState = yyTable[yyN];		// shift to yyN
            yyVal = yyLex.value();
            yyToken = -1;
            if (yyErrorFlag > 0) -- yyErrorFlag;
            goto continue_yyLoop;
          }
          if ((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
              && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
            yyN = yyTable[yyN];			// reduce (yyN)
          else
            switch (yyErrorFlag) {
  
            case 0:
              yyExpectingState = yyState;
              // yyerror(String.Format ("syntax error, got token `{0}'", yyname (yyToken)), yyExpecting(yyState));
//t              if (debug != null) debug.error("syntax error");
              if (yyToken == 0 /*eof*/ || yyToken == eof_token) throw new yyParser.yyUnexpectedEof ();
              goto case 1;
            case 1: case 2:
              yyErrorFlag = 3;
              do {
                if ((yyN = yySindex[yyStates[yyTop]]) != 0
                    && (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == Token.yyErrorCode) {
//t                  if (debug != null)
//t                    debug.shift(yyStates[yyTop], yyTable[yyN], 3);
                  yyState = yyTable[yyN];
                  yyVal = yyLex.value();
                  goto continue_yyLoop;
                }
//t                if (debug != null) debug.pop(yyStates[yyTop]);
              } while (-- yyTop >= 0);
//t              if (debug != null) debug.reject();
              throw new yyParser.yyException("irrecoverable syntax error");
  
            case 3:
              if (yyToken == 0) {
//t                if (debug != null) debug.reject();
                throw new yyParser.yyException("irrecoverable syntax error at end-of-file");
              }
//t              if (debug != null)
//t                debug.discard(yyState, yyToken, yyname(yyToken),
//t  							yyLex.value());
              yyToken = -1;
              goto continue_yyDiscarded;		// leave stack alone
            }
        }
        int yyV = yyTop + 1-yyLen[yyN];
//t        if (debug != null)
//t          debug.reduce(yyState, yyStates[yyV-1], yyN, YYRules.getRule (yyN), yyLen[yyN]);
        yyVal = yyV > yyTop ? null : yyVals[yyV]; // yyVal = yyDefault(yyV > yyTop ? null : yyVals[yyV]);
        switch (yyN) {
case 1:
#line 40 "CParser.jay"
  { yyVal = new VariableExpression((yyVals[0+yyTop]).ToString()); }
  break;
case 2:
#line 41 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 42 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 43 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 5:
#line 50 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 6:
#line 54 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 7:
#line 58 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 8:
#line 62 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 66 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 10:
#line 70 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
	}
  break;
case 11:
#line 74 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 12:
#line 78 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 13:
#line 82 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 14:
#line 86 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list ',' '}'");
	}
  break;
case 15:
  case_15();
  break;
case 16:
  case_16();
  break;
case 17:
#line 108 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 18:
#line 112 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 19:
#line 116 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 20:
#line 120 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '&' cast_expression");
	}
  break;
case 21:
#line 124 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '*' cast_expression");
	}
  break;
case 22:
#line 128 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 132 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: SIZEOF unary_expression");
	}
  break;
case 24:
#line 136 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: SIZEOF '(' type_name ')'");
	}
  break;
case 25:
#line 140 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 26:
#line 141 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 27:
#line 142 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 28:
#line 143 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 29:
#line 150 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 30:
#line 154 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 31:
#line 161 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 165 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 169 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 34:
#line 173 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 181 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 37:
#line 185 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 193 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 40:
#line 197 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 205 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 43:
#line 209 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 213 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 217 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 225 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 48:
#line 229 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 237 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 245 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 253 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 261 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 269 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 59:
#line 276 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 60:
#line 280 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 62:
  case_62();
  break;
case 63:
#line 298 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 64:
#line 299 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 65:
#line 300 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 66:
#line 301 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 67:
#line 302 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 68:
#line 303 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 69:
#line 304 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 70:
#line 305 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 71:
#line 306 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 72:
#line 307 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 73:
#line 308 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 74:
#line 315 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 75:
#line 319 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 77:
  case_77();
  break;
case 78:
  case_78();
  break;
case 79:
  case_79();
  break;
case 80:
  case_80();
  break;
case 81:
  case_81();
  break;
case 82:
  case_82();
  break;
case 83:
  case_83();
  break;
case 84:
  case_84();
  break;
case 85:
  case_85();
  break;
case 86:
  case_86();
  break;
case 87:
  case_87();
  break;
case 88:
  case_88();
  break;
case 89:
  case_89();
  break;
case 90:
  case_90();
  break;
case 91:
#line 425 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 92:
#line 426 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 93:
#line 427 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 94:
#line 428 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 95:
#line 429 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 96:
#line 433 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 97:
#line 434 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 98:
#line 435 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 99:
#line 436 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 100:
#line 437 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 101:
#line 438 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 102:
#line 439 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 103:
#line 440 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 104:
#line 441 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 105:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 106:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 107:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 110:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 111:
#line 451 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 112:
#line 452 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 113:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 114:
#line 457 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 115:
#line 458 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 116:
#line 459 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
  break;
case 121:
#line 470 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 122:
#line 471 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString()); }
  break;
case 123:
#line 472 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 124:
#line 473 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString()); }
  break;
case 125:
#line 474 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 130:
#line 488 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 131:
#line 495 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 132:
#line 496 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 133:
#line 501 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 134:
  case_134();
  break;
case 135:
#line 517 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 136:
#line 521 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 137:
#line 525 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 138:
#line 529 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 139:
#line 533 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 140:
#line 537 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 141:
#line 541 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 142:
#line 545 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 143:
  case_143();
  break;
case 144:
  case_144();
  break;
case 145:
  case_145();
  break;
case 146:
#line 573 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 147:
#line 574 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 148:
#line 575 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 149:
#line 576 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 150:
#line 580 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 151:
#line 584 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 152:
#line 588 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 153:
#line 589 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 154:
#line 590 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 155:
#line 597 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 156:
  case_156();
  break;
case 157:
  case_157();
  break;
case 158:
  case_158();
  break;
case 159:
  case_159();
  break;
case 160:
  case_160();
  break;
case 161:
  case_161();
  break;
case 162:
  case_162();
  break;
case 163:
  case_163();
  break;
case 169:
  case_169();
  break;
case 170:
#line 681 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 171:
#line 685 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 172:
#line 689 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 173:
#line 693 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 174:
#line 697 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 175:
#line 701 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 176:
  case_176();
  break;
case 177:
  case_177();
  break;
case 178:
  case_178();
  break;
case 179:
  case_179();
  break;
case 180:
#line 735 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 181:
#line 739 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 182:
#line 743 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 183:
  case_183();
  break;
case 184:
  case_184();
  break;
case 185:
  case_185();
  break;
case 186:
  case_186();
  break;
case 187:
#line 783 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 201:
#line 815 "CParser.jay"
  {
		yyVal = new Block (GetLocation(yyVals[-1+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 202:
#line 819 "CParser.jay"
  {
        yyVal = new Block (GetLocation(yyVals[-2+yyTop]), (List<Statement>)yyVals[-1+yyTop], GetLocation(yyVals[0+yyTop]));
	}
  break;
case 203:
#line 823 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 204:
#line 824 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 207:
#line 836 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 208:
#line 840 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 209:
#line 847 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 210:
#line 851 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 212:
#line 859 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 213:
#line 863 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], (Statement)yyVals[-5+yyTop]);
	}
  break;
case 214:
#line 867 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop], null, GetLocation(yyVals[-5+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 215:
#line 871 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], null, GetLocation(yyVals[-6+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 216:
#line 875 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop]);
	}
  break;
case 217:
#line 879 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 221:
#line 889 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 222:
#line 893 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 223:
  case_223();
  break;
case 224:
  case_224();
  break;
case 227:
  case_227();
  break;
case 228:
  case_228();
  break;
case 229:
  case_229();
  break;
case 230:
  case_230();
  break;
#line default
        }
        yyTop -= yyLen[yyN];
        yyState = yyStates[yyTop];
        int yyM = yyLhs[yyN];
        if (yyState == 0 && yyM == 0) {
//t          if (debug != null) debug.shift(0, yyFinal);
          yyState = yyFinal;
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
//t            if (debug != null)
//t               debug.lex(yyState, yyToken,yyname(yyToken), yyLex.value());
          }
          if (yyToken == 0) {
//t            if (debug != null) debug.accept(yyVal);
            return yyVal;
          }
          goto continue_yyLoop;
        }
        if (((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
            && (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
          yyState = yyTable[yyN];
        else
          yyState = yyDgoto[yyM];
//t        if (debug != null) debug.shift(yyStates[yyTop], yyState);
	 goto continue_yyLoop;
      continue_yyDiscarded: ;	// implements the named-loop continue: 'continue yyDiscarded'
      }
    continue_yyLoop: ;		// implements the named-loop continue: 'continue yyLoop'
    }
  }

/*
 All more than 3 lines long rules are wrapped into a method
*/
void case_15()
#line 91 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_16()
#line 97 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_62()
#line 286 "CParser.jay"
{
		if (yyVals[-1+yyTop] is RelationalOp && ((RelationalOp)yyVals[-1+yyTop]) == RelationalOp.Equals) {
			yyVal = new AssignExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
		}
		else {
			var left = (Expression)yyVals[-2+yyTop]; 
			yyVal = new AssignExpression(left, new BinaryExpression (left, (Binop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]));
		}
	}

void case_77()
#line 328 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_78()
#line 334 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		d.InitDeclarators = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_79()
#line 344 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_80()
#line 350 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_81()
#line 356 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_82()
#line 362 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_83()
#line 368 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_84()
#line 374 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_85()
#line 380 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_86()
#line 386 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_87()
#line 395 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_88()
#line 401 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_89()
#line 410 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[0+yyTop];
		yyVal = id;
	}

void case_90()
#line 416 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[-2+yyTop];
		id.Initializer = (Initializer)yyVals[0+yyTop];
		yyVal = id;
	}

void case_134()
#line 503 "CParser.jay"
{
		var d = (Declarator)yyVals[-1+yyTop];
		var f = FixPointerAndArrayPrecedence(d);
		if (f != null) {
			yyVal = f;
		}
		else {
			d.StrongBinding = true;
			yyVal = d;
		}		
	}

void case_143()
#line 547 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_144()
#line 554 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_145()
#line 564 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_156()
#line 599 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_157()
#line 608 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_158()
#line 614 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_159()
#line 623 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_160()
#line 628 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_161()
#line 633 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_162()
#line 641 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_163()
#line 647 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_169()
#line 667 "CParser.jay"
{
		var d = (Declarator)yyVals[-1+yyTop];
		var f = FixPointerAndArrayPrecedence(d);
		if (f != null) {
			yyVal = f;
		}
		else {
			d.StrongBinding = true;
			yyVal = d;
		}		
	}

void case_176()
#line 703 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-1+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_177()
#line 710 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_178()
#line 716 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration> ();
		yyVal = d;
	}

void case_179()
#line 723 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_183()
#line 748 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_184()
#line 755 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_185()
#line 763 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_186()
#line 770 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_223()
#line 898 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_224()
#line 903 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_227()
#line 916 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_228()
#line 925 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_229()
#line 936 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_230()
#line 942 "CParser.jay"
{
		var l = (List<Declaration>)yyVals[-1+yyTop];
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

#line default
   static readonly short [] yyLhs  = {              -1,
    1,    1,    1,    1,    3,    3,    3,    3,    3,    3,
    3,    3,    3,    3,    4,    4,    8,    8,    8,    8,
    8,    8,    8,    8,   10,   10,   10,   10,    9,    9,
   11,   11,   11,   11,   12,   12,   12,   13,   13,   13,
   14,   14,   14,   14,   14,   15,   15,   15,   16,   16,
   17,   17,   18,   18,   19,   19,   20,   20,   21,   21,
    7,    7,   22,   22,   22,   22,   22,   22,   22,   22,
   22,   22,   22,    2,    2,   23,   24,   24,   25,   25,
   25,   25,   25,   25,   25,   25,   26,   26,   31,   31,
   27,   27,   27,   27,   27,   28,   28,   28,   28,   28,
   28,   28,   28,   28,   28,   28,   28,   28,   28,   28,
   34,   34,   34,   36,   36,   36,   38,   38,   38,   38,
   35,   35,   35,   35,   35,   39,   39,   40,   40,   30,
   32,   32,   42,   42,   42,   42,   42,   42,   42,   42,
   42,   42,   42,   42,   42,   41,   41,   41,   41,   43,
   43,   29,   29,   29,   44,   44,   46,   46,   47,   47,
   47,   45,   45,    5,    5,   48,   48,   48,   49,   49,
   49,   49,   49,   49,   49,   49,   49,   49,   49,   33,
   33,   33,    6,    6,    6,    6,   50,   51,   51,   52,
   52,   53,   53,   53,   53,   53,   53,   54,   54,   54,
   37,   37,   59,   59,   60,   60,   55,   55,   56,   56,
   56,   57,   57,   57,   57,   57,   57,   58,   58,   58,
   58,   58,    0,    0,   61,   61,   62,   62,   63,   63,
  };
   static readonly short [] yyLen = {           2,
    1,    1,    1,    3,    1,    4,    3,    4,    3,    3,
    2,    2,    6,    7,    1,    3,    1,    2,    2,    2,
    2,    2,    2,    4,    1,    1,    1,    1,    1,    4,
    1,    3,    3,    3,    1,    3,    3,    1,    3,    3,
    1,    3,    3,    3,    3,    1,    3,    3,    1,    3,
    1,    3,    1,    3,    1,    3,    1,    3,    1,    5,
    1,    3,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    3,    1,    2,    3,    1,    2,
    1,    2,    1,    2,    1,    2,    1,    3,    1,    3,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    3,    2,    2,    1,    1,    1,    2,    1,    2,    1,
    4,    5,    5,    6,    2,    1,    3,    1,    3,    1,
    2,    1,    1,    3,    5,    4,    4,    6,    6,    5,
    4,    3,    4,    4,    3,    1,    2,    2,    3,    1,
    2,    1,    1,    1,    1,    3,    1,    3,    2,    2,
    1,    1,    3,    1,    2,    1,    1,    2,    3,    2,
    3,    3,    4,    3,    4,    2,    3,    3,    4,    1,
    3,    4,    1,    2,    3,    4,    2,    1,    2,    3,
    2,    1,    1,    1,    1,    1,    1,    3,    4,    3,
    2,    3,    1,    2,    1,    1,    1,    2,    5,    7,
    5,    5,    7,    6,    7,    6,    7,    3,    2,    2,
    2,    3,    1,    2,    1,    1,    4,    3,    1,    2,
  };
   static readonly short [] yyDefRed = {            0,
  110,   91,   92,   93,   94,   95,  130,  153,   97,   98,
   99,  100,  103,  104,  101,  102,  152,  154,   96,  105,
  106,  107,  114,  115,  116,    0,    0,  226,    0,    0,
    0,    0,    0,  108,  109,    0,  223,  225,    0,    0,
  224,  133,    0,    0,   77,    0,   87,    0,    0,    0,
   80,   82,   84,   86,    0,    0,  112,    0,    0,    0,
  126,    0,  150,  148,    0,    0,   78,    0,  229,    0,
  228,    0,    0,    0,    0,  111,    0,    2,    3,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  201,    0,    0,   25,   26,   27,
   28,  207,    5,    0,    0,   74,    0,   31,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   61,
  205,  193,  206,  192,  194,  195,  196,  197,    0,  203,
    0,    0,  121,    0,  134,  151,  149,   88,    0,    1,
    0,  180,   90,  230,  227,  162,  145,    0,    0,    0,
    0,  157,    0,  142,    0,    0,    0,    0,    0,   23,
    0,   18,   19,   29,   76,    0,    0,    0,    0,    0,
    0,    0,    0,  219,  220,  221,    0,    0,    0,    0,
    0,    0,   20,   21,    0,  208,    0,   11,   12,    0,
    0,    0,   64,   65,   66,   67,   68,   69,   70,   71,
   72,   73,   63,    0,   22,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  202,  204,  122,    0,  129,  123,
  127,    0,    0,    0,  183,    0,    0,  188,    0,    0,
  159,    0,  160,    0,  143,  144,    0,    0,    0,  141,
  137,    0,  136,    0,    0,  198,    0,    0,    0,  200,
    0,    0,    0,    0,    0,    0,  218,  222,    4,    0,
  117,  119,    0,    0,  165,   75,   10,    7,    0,   15,
    0,    9,   62,   32,   33,   34,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  124,    0,  191,  181,    0,  184,  187,  189,
  176,    0,    0,  170,    0,    0,    0,    0,    0,  163,
  156,  158,    0,    0,  140,  135,    0,    0,  199,    0,
    0,    0,    0,    0,    0,    0,   30,    8,    0,    6,
    0,  190,  182,  185,    0,  177,  169,  174,  171,  178,
    0,  172,    0,    0,  138,  139,    0,  211,  212,    0,
    0,    0,    0,    0,    0,   16,   60,  186,  179,  175,
  173,    0,    0,  216,    0,  214,    0,   13,    0,  210,
  213,  217,  215,   14,
  };
  protected static readonly short [] yyDgoto  = {            27,
  103,  104,  105,  279,  179,  234,  106,  107,  108,  109,
  110,  111,  112,  113,  114,  115,  116,  117,  118,  119,
  120,  204,  166,   28,   70,   46,   30,   31,   32,   33,
   47,   62,  235,   34,   35,   36,  122,  182,   60,   61,
   49,   50,   65,  312,  150,  151,  152,  313,  244,  236,
  237,  238,  123,  124,  125,  126,  127,  128,  129,  130,
   37,   38,   72,
  };
  protected static readonly short [] yySindex = {         2462,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -83, 2462,    0,  -24, 2462,
 2462, 2462, 2462,    0,    0,  -64,    0,    0,  -76, -196,
    0,    0,  -13,  -17,    0,   96,    0,  970,   16,    2,
    0,    0,    0,    0,  -35,   64,    0, -196,    7,    6,
    0,   53,    0,    0,  -17,  -13,    0,  -28,    0,  -24,
    0, 2357,    2,  344,    3,    0,   47,    0,    0, 2228,
 2261, 2261, 2295,   99,  165,  170,  184,  493,  202,   49,
  261,  270,  956,  433,    0, 2295, 2295,    0,    0,    0,
    0,    0,    0,  160,  137,    0,  130,    0, 2295,  164,
  242,  148,   66,  216,  255,  231,  210,  106,  -55,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  133,    0,
    9, 2295,    0, -101,    0,    0,    0,    0,  327,    0,
 2033,    0,    0,    0,    0,    0,    0,  -36,  356,   97,
  357,    0,    5,    0, 2056,  321,  621,  493,  433,    0,
  433,    0,    0,    0,    0,  383,  493, 2295, 2295, 2295,
  131,  532,  397,    0,    0,    0,  163,  110,  419, 2285,
 2285,   34,    0,    0, 2295,    0,  213,    0,    0, 2064,
 2295,  217,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2295,    0, 2295, 2295, 2295, 2295, 2295,
 2295, 2295, 2295, 2295, 2295, 2295, 2295, 2295, 2295, 2295,
 2295, 2295, 2295, 2295,    0,    0,    0,  -88,    0,    0,
    0, 2295,  220,   21,    0,  -28,  251,    0,  756, 2087,
    0,  -37,    0,   61,    0,    0,  256, 2409, 1045,    0,
    0, 2295,    0, 2098,  441,    0,  496,  501,  493,    0,
  173,  234,  239,  505, 2112, 2112,    0,    0,    0, 2134,
    0,    0,  893,  109,    0,    0,    0,    0,  266,    0,
   18,    0,    0,    0,    0,    0,  164,  164,  242,  242,
  148,  148,  148,  148,   66,   66,  216,  255,  231,  210,
  106,   27,    0,  460,    0,    0, 1791,    0,    0,    0,
    0,  513,  514,    0, 2156,  463,   61,  476, 2172,    0,
    0,    0,  464,  465,    0,    0,  437,  437,    0,  493,
  493,  493, 2295, 2185, 2199, 2033,    0,    0, 2295,    0,
 2295,    0,    0,    0,  -28,    0,    0,    0,    0,    0,
  520,    0, 2236,  469,    0,    0,  252,    0,    0,  273,
  493,  274,  493,  413,   22,    0,    0,    0,    0,    0,
    0,  493,  507,    0,  493,    0,  493,    0, 1993,    0,
    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  -21,
  488,  527,  629,    0,    0,    0,    0,    0,  657,    0,
    0,    0,    0,   39,    0,    0,    0,  232,    0,  863,
    0,    0,    0,    0,  832,    0,    0,    0,   23,    0,
    0,    0,    0,    0,   59,    0,    0,    0,    0,    0,
    0,    0,  944,    0,    0,    0, 1251,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1278,    0, 1346,    0,    0, 1356,
 1369, 1657, 1794,   19, 2344,   74, 2448,   28,  144,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  232,    0,
    0,    0,    0,    0,    0,    0,    0,  414,    0,    0,
  535,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  155,
  295,  537,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  428,    0,  438,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  539,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1414, 1441, 1603, 1629,
 1680, 1703, 1726, 1754, 1817, 1894, 1917, 2408, 2419, 2472,
  101,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  442,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1318,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  205,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  517,    0,    0,  228,  237,  -68,  760,  -45,    0,
  331,  332,  125,  333,  362,  363,  361,  365,  366,    0,
  -74,    0,  -93,   36,    1,    0,    0,  300, 2389,    0,
  518,  157,  -66,    0,    0,    0,  491,  368,  530, -104,
   70,  -27,  -26,  -61,    0,    0,  337,   31,  -62, -263,
    0,  353,  -77,    0, -146,    0,    0,    0,    0,  466,
  565,    0,    0,
  };
  protected static readonly short [] yyTable = {           142,
   29,  143,  239,  239,  101,   44,  156,  224,  165,   96,
  171,   94,  149,   97,   98,   43,   99,   44,   79,   79,
   79,   73,   79,  230,   44,  266,   43,   29,   44,  231,
   51,   52,   53,   54,   45,  101,  303,   79,  229,   40,
   96,   74,   94,  345,  155,   98,   58,   99,  157,  134,
  183,  184,  228,  240,  240,   43,   49,  165,   56,   49,
   59,  185,   49,  205,  307,  379,  128,  132,   57,   79,
  185,   57,  142,  273,  148,   44,   49,   49,  146,  146,
  256,   49,  146,   69,  341,   57,   57,   56,  255,  260,
   57,  121,   75,  135,  141,  154,  101,  100,  147,  147,
  318,   96,  147,   94,  158,   97,   98,  144,   99,  184,
  340,   49,   49,   64,   53,  345,  276,   53,  334,  335,
   57,  280,  102,  231,  240,  215,  249,  216,  100,  146,
  133,   53,   53,  227,  137,  283,   53,  246,  304,   66,
  247,   58,   49,   49,   58,  306,  378,  128,  273,  147,
  269,  319,   57,  185,   67,   59,  167,  165,   58,   58,
  284,  285,  286,   58,  121,  101,   53,  142,   59,  308,
   96,  316,   94,   39,   97,   98,  190,   99,  243,  317,
  323,  329,  192,  324,   59,   48,   56,   59,   95,  100,
  203,  102,   55,   58,  118,  118,  118,   53,   53,  240,
  208,   59,   59,  185,  168,  206,  185,  265,  184,  169,
  207,  317,  275,  330,   73,  223,  185,  242,  186,   42,
   42,  268,  139,  170,  337,   58,  139,  191,  140,   78,
   79,   80,   42,   81,   82,   79,   59,  209,  142,  148,
  344,  172,  209,   42,  209,  118,  209,  209,  148,  209,
  354,  274,  357,  358,  359,   56,  351,  225,  100,  140,
   78,   79,   80,  209,   81,   82,  367,  142,   59,  184,
  366,    8,   42,  148,  331,   89,  142,  185,  368,  332,
   17,   18,  185,  374,  209,  376,  210,  153,   49,   49,
   89,    8,  219,    8,  380,  146,  233,  382,   57,  383,
   17,   18,   17,   18,  241,  173,  338,  184,  242,  339,
  142,  309,  344,  373,  375,  147,  185,  185,  148,  174,
   77,   78,   79,   80,  220,   81,   82,  209,  175,  209,
  209,  213,  214,  221,  120,  120,  120,  291,  292,  293,
  294,  232,  274,   53,   53,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
   26,   58,   83,   84,   85,  222,   86,   87,   88,   89,
   90,   91,   92,   93,  147,  120,  257,   68,  258,   77,
   78,   79,   80,  180,   81,   82,  245,  187,  188,  189,
  248,  193,  194,  195,  196,  197,  198,  199,  200,  201,
  202,  211,  212,  251,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,   26,
  259,   83,   84,   85,  264,   86,   87,   88,   89,   90,
   91,   92,   93,  377,  161,  267,  185,  161,  180,  270,
  180,  209,  209,  209,  209,  101,  209,  209,  166,  277,
   96,  166,   94,  282,   97,   98,  305,   99,  167,  180,
  180,  167,  168,  217,  218,  168,  209,  209,  209,  209,
  209,  209,  209,  209,  209,  209,  209,  209,  209,  209,
  209,  209,  209,  209,  209,  209,  209,  209,  209,  209,
  209,  209,  320,  209,  209,  209,  350,  209,  209,  209,
  209,  209,  209,  209,  209,  101,   57,   81,   81,   81,
   96,   81,   94,  326,   97,   98,  327,   99,   71,  287,
  288,  328,  289,  290,  333,   76,   81,  271,  272,  295,
  296,  102,  342,  346,  347,  349,  355,  356,  100,  336,
  369,  371,  145,  372,  101,  381,   83,   83,   83,   96,
   83,   94,  365,   97,   98,  155,   99,  164,   81,  166,
  297,  299,  298,  138,  322,   83,  300,  131,  301,  310,
  102,   41,    0,    0,  226,    0,    0,    0,    0,    0,
  146,    0,    0,    0,    0,    0,    0,    0,    0,  177,
  178,    0,    0,    0,    0,   56,    0,   83,  100,    0,
    0,    0,    0,    0,    0,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
   26,    0,    0,  101,    0,    0,    0,  100,   96,    0,
   94,    0,  254,   98,    0,   99,    0,    0,   85,   85,
   85,    0,   85,    0,    0,  178,    0,  178,    0,    0,
    0,    0,    0,    0,  261,  262,  263,   85,    0,  140,
   78,   79,   80,    0,   81,   82,  125,  125,  125,    0,
  125,    0,    0,    0,    0,    0,    0,  281,    0,    0,
    0,    0,    0,  253,    1,  125,    0,    0,    0,   85,
    0,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,   26,
  302,    0,    0,    0,   81,    0,  100,  125,    0,   77,
   78,   79,   80,    0,   81,   82,    0,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   24,   25,   26,   83,    0,    0,    0,    0,  140,   78,
   79,   80,    0,   81,   82,  239,  311,   44,    0,    0,
    0,   83,   84,   85,    0,   86,   87,   88,   89,   90,
   91,   92,   93,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,   24,   25,   26,  160,
  162,  163,  164,    0,    0,    0,  240,    0,    0,  360,
  362,  364,    0,    0,    0,  164,  164,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  164,    0,
    0,  113,  113,  113,    0,  113,    0,  140,   78,   79,
   80,    0,   81,   82,    0,   85,    0,    0,    0,    0,
  113,  164,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  132,    0,  252,  132,    0,    0,    8,
    0,    0,    0,  125,  164,    0,    0,    0,   17,   18,
    0,  132,  113,  132,    0,    0,    0,    0,    0,    0,
    0,    0,  273,  311,   44,    0,    0,    0,  125,  125,
  125,  125,  125,  125,  125,  125,  125,  125,  125,  125,
  125,  125,  125,  125,  125,  125,  125,  125,  125,  125,
  125,  125,  125,  125,    0,  164,  164,  164,  164,  164,
  164,  164,  164,  164,  164,  164,  164,  164,  164,  164,
  164,  164,  164,  240,  131,  132,    0,  131,  101,    0,
    0,  164,    0,   96,    0,   94,    0,   97,   98,    0,
   99,    0,  131,    0,  131,    0,    0,    0,    0,    0,
    0,    0,   42,  164,  176,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  164,
   68,    0,    0,    0,    0,    0,    0,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   24,   25,   26,    0,    0,    0,  131,    0,    0,    0,
    0,    0,    0,    0,  164,    0,    0,  101,    0,    0,
    0,  100,   96,    0,   94,    0,   97,   98,  113,   99,
    0,    0,   56,    0,    0,    0,    0,    0,    0,    0,
  164,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  164,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,    0,
    0,    0,    0,    0,  132,  132,  132,  132,  132,  132,
  132,  132,  132,  132,  132,  132,  132,  132,  132,  132,
  132,  132,  132,  132,  132,  132,  132,  132,  132,  132,
  100,    0,    0,    0,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,   26,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  140,   78,   79,   80,    0,   81,   82,    0,
    0,    0,    0,    0,    0,  131,  131,  131,  131,  131,
  131,  131,  131,  131,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,  131,  131,  131,  131,  131,  131,
  131,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,   24,   25,   26,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    1,    1,    0,
    1,    0,    1,    1,    1,    1,    1,    1,    0,    0,
    0,  140,   78,   79,   80,    0,   81,   82,    0,    1,
    1,    1,    1,    1,   17,   17,    0,    0,   17,   17,
   17,   17,   17,    0,   17,    0,    0,    0,    0,    0,
    0,    0,    0,    8,    0,   17,   17,   17,   17,   17,
   17,    1,   17,   18,    1,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   24,   24,    0,    0,   24,   24,
   24,   24,   24,    0,   24,    0,    0,    0,    0,    0,
   17,   17,    0,    0,    1,   24,   24,   24,   24,   24,
   24,    0,   29,   29,    0,    0,   29,   29,   29,   29,
   29,    0,   29,   35,    0,    0,   35,    0,   35,   35,
   35,   17,   17,   29,   29,   29,   38,   29,   29,   38,
   24,   24,   38,   35,   35,   35,    0,   35,   35,    0,
    0,    0,    0,    0,    0,    0,   38,   38,   38,    0,
   38,   38,    0,    0,    0,    0,    0,    0,   29,   29,
    0,   24,   24,    0,    0,    0,    0,    0,   35,   35,
    0,   36,    0,    0,   36,    0,   36,   36,   36,    0,
    0,   38,   38,    0,    0,    0,    0,    0,    0,   29,
   29,   36,   36,   36,    0,   36,   36,    0,   37,   35,
   35,   37,    0,   37,   37,   37,    0,    0,    0,    0,
    0,    0,   38,   38,    0,    0,    0,    0,   37,   37,
   37,    0,   37,   37,    0,    0,   36,   36,    0,    0,
    0,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    0,   37,   37,    0,    0,   36,   36,    0,
    0,   17,   17,   17,   17,   17,   17,   17,   17,   17,
   17,   17,   17,   17,   17,   17,   17,   17,   17,    0,
    0,    0,    0,    0,   37,   37,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   24,   24,   24,   24,   24,   24,   24,   24,   24,
   24,   24,   24,   24,   24,   24,   24,   24,   24,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   29,
   29,   29,   29,   29,   29,   29,   29,    0,    0,   35,
   35,   35,   35,   35,   35,   35,   35,    0,    0,    0,
    0,    0,   38,   38,   38,   38,   38,   38,   38,   38,
   39,    0,    0,   39,    0,    0,   39,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   39,   39,   39,    0,   39,   39,   40,    0,    0,   40,
    0,    0,   40,    0,    0,    0,    0,   36,   36,   36,
   36,   36,   36,   36,   36,    0,   40,   40,   40,    0,
   40,   40,    0,    0,   41,   39,   39,   41,    0,    0,
   41,    0,    0,    0,   37,   37,   37,   37,   37,   37,
   37,   37,    0,    0,   41,   41,   41,   44,   41,   41,
   44,   40,   40,   44,    0,    0,   39,   39,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   44,   44,   44,
   45,   44,   44,   45,    0,    0,   45,    0,    0,   41,
   41,    0,   40,   40,    0,    0,    0,    0,    0,    0,
   45,   45,   45,   42,   45,   45,   42,    0,    0,   42,
    0,    0,   44,   44,    0,    0,    0,    0,    0,    0,
   41,   41,    0,   42,   42,   42,    0,   42,   42,    0,
    0,   43,    0,    0,   43,   45,   45,   43,    0,    0,
    0,    0,    0,   44,   44,    0,    0,    0,    0,    0,
    0,   43,   43,   43,    0,   43,   43,    0,   42,   42,
    0,    0,    0,  101,    0,    0,   45,   45,   96,    0,
   94,   46,   97,   98,   46,   99,  233,   46,    0,    0,
    0,    0,    0,    0,    0,    0,   43,   43,    0,   42,
   42,   46,   46,    0,   47,    0,   46,   47,    0,    0,
   47,    0,    0,    0,    0,    0,   39,   39,   39,   39,
   39,   39,   39,   39,   47,   47,    0,   43,   43,   47,
    0,  232,    0,    0,    0,    0,   46,   46,    0,    0,
    0,    0,   40,   40,   40,   40,   40,   40,   40,   40,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   47,
   47,    0,    0,  141,    0,  343,  100,   46,   46,    0,
    0,    0,   41,   41,   41,   41,   41,   41,    0,    0,
    0,   48,    0,    0,   48,    0,    0,   48,    0,    0,
   47,   47,    0,    0,    0,   44,   44,   44,   44,   44,
   44,   48,   48,    0,   50,    0,   48,   50,    0,    0,
   50,    0,    0,    0,    0,    0,    0,    0,   45,   45,
   45,   45,   45,   45,   50,   50,    0,    0,    0,   50,
    0,    0,    0,    0,    0,    0,   48,   48,    0,    0,
    0,   42,   42,   42,   42,   42,   42,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   50,
   50,    0,    0,    0,    0,    0,    0,   48,   48,   43,
   43,   43,   43,   43,   43,  101,    0,    0,    0,    0,
   96,    0,   94,    0,   97,   98,    0,   99,  233,    0,
   50,   50,    0,    0,    0,    0,    0,  140,   78,   79,
   80,    0,   81,   82,    0,    0,    0,    0,    0,    0,
    0,   46,   46,   46,   46,  101,    0,    0,    0,    0,
   96,    0,   94,    0,   97,   98,    0,   99,  233,    0,
    0,    0,    0,  232,   47,   47,   47,   47,  101,    0,
    0,    0,    0,   96,    0,   94,  101,   97,   98,    0,
   99,   96,    0,   94,  278,   97,   98,    0,   99,    0,
    0,    0,    0,    0,    0,  141,    0,  384,  100,  101,
    0,    0,    0,  232,   96,    0,   94,    0,  315,   98,
  101,   99,    0,    0,    0,   96,    0,   94,    0,   97,
   98,    0,   99,    0,  101,    0,    0,    0,  250,   96,
    0,   94,    0,   97,   98,  141,   99,    0,  100,    0,
    0,   48,   48,   48,   48,    0,  101,    0,    0,    0,
  102,   96,    0,   94,    0,   97,   98,    0,   99,  314,
    0,  100,    0,    0,    0,    0,   50,   50,  101,  100,
  325,    0,    0,   96,    0,   94,    0,   97,   98,    0,
   99,    0,    0,    0,  101,    0,    0,    0,    0,   96,
    0,   94,  100,  353,   98,    0,   99,  101,    0,    0,
    0,    0,   96,  100,   94,  361,   97,   98,    0,   99,
    0,  101,    0,    0,    0,    0,   96,  100,   94,  363,
   97,   98,    0,   99,    0,    0,    0,    0,  348,  140,
   78,   79,   80,    0,   81,   82,  336,    0,    0,  100,
  101,    0,    0,    0,  352,   96,    0,  159,  101,   97,
   98,    0,   99,   96,    0,   94,    0,   97,   98,    0,
   99,  100,    0,    0,    0,    0,    0,    0,    0,  140,
   78,   79,   80,  101,   81,   82,    0,  100,   96,    0,
  161,    0,   97,   98,    0,   99,    0,    0,    0,    0,
  100,    0,  140,   78,   79,   80,    0,   81,   82,    0,
  140,   78,   79,   80,  100,   81,   82,  101,  370,    0,
    0,    0,   96,    0,   94,    0,   97,   98,    0,   99,
    0,    0,    0,  140,   78,   79,   80,    0,   81,   82,
    0,    0,    0,  100,  140,   78,   79,   80,    0,   81,
   82,  100,    0,    0,    0,    0,    0,    0,  140,   78,
   79,   80,    0,   81,   82,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   51,    0,  100,   51,    0,    0,
  140,   78,   79,   80,    0,   81,   82,    0,    0,    0,
    0,   51,   51,    0,    0,    0,   51,    0,    0,    0,
    0,    0,  140,   78,   79,   80,    0,   81,   82,    0,
  100,    0,    0,    0,    0,    0,    0,    0,  140,   78,
   79,   80,   63,   81,   82,    0,   51,   51,    0,    0,
    0,  140,   78,   79,   80,    0,   81,   82,   52,    0,
    0,   52,    0,  136,    0,  140,   78,   79,   80,   54,
   81,   82,   54,   63,    0,   52,   52,   51,   51,    0,
   52,    0,    0,    0,    0,    0,   54,   54,    0,   56,
    0,   54,  181,    0,  140,   78,   79,   80,   55,   81,
   82,   55,  140,   78,   79,   80,    0,   81,   82,    0,
   52,   52,    0,    0,    0,   55,   55,    0,    0,    0,
   55,   54,   56,    0,    0,   56,    0,  140,   78,   79,
   80,    0,   81,   82,    0,    0,    0,    0,    0,   56,
   56,   52,   52,    0,   56,    0,    0,    0,    0,    0,
   55,   63,   54,   54,    0,  136,    0,  181,    0,  181,
    0,  140,   78,   79,   80,    0,   81,   82,    0,    0,
    0,    0,    0,    0,   56,    0,    1,    0,  181,  181,
    0,    0,   55,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,
   25,   26,    0,    0,    0,    0,   56,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   51,   51,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  136,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,   24,   25,   26,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   52,   52,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   54,   54,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,   24,   25,   26,  321,   55,   55,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   56,   56,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,   24,   25,   26,
  };
  protected static readonly short [] yyCheck = {            68,
    0,   68,   40,   40,   33,   42,   75,   63,   83,   38,
   88,   40,   74,   42,   43,   40,   45,   42,   40,   41,
   42,   49,   44,  125,   42,  172,   40,   27,   42,  134,
   30,   31,   32,   33,   59,   33,  125,   59,  132,  123,
   38,   40,   40,  307,   42,   43,  123,   45,   75,   44,
   96,   97,   44,   91,   91,   40,   38,  132,  123,   41,
  257,   44,   44,  109,   44,   44,   44,   61,   41,   91,
   44,   44,  141,   40,   74,   42,   58,   59,   40,   41,
  158,   63,   44,   48,   58,   58,   59,  123,  157,  167,
   63,   56,   91,   41,  123,   93,   33,  126,   40,   41,
   40,   38,   44,   40,   58,   42,   43,   72,   45,  155,
   93,   93,   94,   44,   41,  379,  185,   44,  265,  266,
   93,  190,   59,  228,   91,   60,  153,   62,  126,   91,
  125,   58,   59,  125,   65,  204,   63,   41,  232,   44,
   44,   41,  124,  125,   44,  125,  125,  125,   40,   91,
   41,   91,  125,   44,   59,  257,   58,  232,   58,   59,
  206,  207,  208,   63,  129,   33,   93,  236,  257,  236,
   38,  240,   40,  257,   42,   43,   40,   45,  148,  242,
  249,  259,   46,  252,   41,   29,  123,   44,  125,  126,
   61,   59,  257,   93,   40,   41,   42,  124,  125,   91,
   37,   58,   59,   44,   40,   42,   44,  172,  254,   40,
   47,  274,  182,   41,  242,  271,   44,  148,   59,  257,
  257,   59,   66,   40,  270,  125,   70,   91,  257,  258,
  259,  260,  257,  262,  263,  257,   93,   33,  307,  239,
  307,   40,   38,  257,   40,   91,   42,   43,  248,   45,
  319,  182,  330,  331,  332,  123,  318,  125,  126,  257,
  258,  259,  260,   59,  262,  263,  341,  336,  125,  315,
  339,  289,  257,  273,   41,   44,  345,   44,  345,   41,
  298,  299,   44,  361,   43,  363,   45,  285,  270,  271,
   59,  289,   38,  289,  372,  257,   46,  375,  271,  377,
  298,  299,  298,  299,  148,  257,   41,  353,  239,   44,
  379,   61,  379,   41,   41,  257,   44,   44,  318,   59,
  257,  258,  259,  260,   94,  262,  263,  123,   59,  125,
  126,  266,  267,  124,   40,   41,   42,  213,  214,  215,
  216,   91,  273,  270,  271,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  271,  309,  310,  311,  270,  313,  314,  315,  316,
  317,  318,  319,  320,   41,   91,  159,   61,  161,  257,
  258,  259,  260,   94,  262,  263,   41,  261,  262,  263,
   44,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  264,  265,   93,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
   58,  309,  310,  311,  314,  313,  314,  315,  316,  317,
  318,  319,  320,   41,   41,   59,   44,   44,  159,   41,
  161,  257,  258,  259,  260,   33,  262,  263,   41,  257,
   38,   44,   40,  257,   42,   43,  257,   45,   41,  180,
  181,   44,   41,  268,  269,   44,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  257,  309,  310,  311,   41,  313,  314,  315,
  316,  317,  318,  319,  320,   33,   36,   40,   41,   42,
   38,   44,   40,   93,   42,   43,   41,   45,   48,  209,
  210,   41,  211,  212,   40,   55,   59,  180,  181,  217,
  218,   59,   93,   41,   41,   93,   93,   93,  126,  123,
   41,   93,   72,  312,   33,   59,   40,   41,   42,   38,
   44,   40,  336,   42,   43,   41,   45,   41,   91,   41,
  219,  221,  220,   66,  248,   59,  222,   58,  223,  237,
   59,   27,   -1,   -1,  129,   -1,   -1,   -1,   -1,   -1,
  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   -1,   -1,  123,   -1,   91,  126,   -1,
   -1,   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,   -1,   -1,   33,   -1,   -1,   -1,  126,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   40,   41,
   42,   -1,   44,   -1,   -1,  159,   -1,  161,   -1,   -1,
   -1,   -1,   -1,   -1,  168,  169,  170,   59,   -1,  257,
  258,  259,  260,   -1,  262,  263,   40,   41,   42,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,  191,   -1,   -1,
   -1,   -1,   -1,   93,  282,   59,   -1,   -1,   -1,   91,
   -1,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  224,   -1,   -1,   -1,  257,   -1,  126,   91,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  257,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   40,   41,   42,   -1,   -1,
   -1,  309,  310,  311,   -1,  313,  314,  315,  316,  317,
  318,  319,  320,  282,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,   80,
   81,   82,   83,   -1,   -1,   -1,   91,   -1,   -1,  333,
  334,  335,   -1,   -1,   -1,   96,   97,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  109,   -1,
   -1,   40,   41,   42,   -1,   44,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,   -1,   -1,   -1,   -1,
   59,  132,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,  285,   44,   -1,   -1,  289,
   -1,   -1,   -1,  257,  155,   -1,   -1,   -1,  298,  299,
   -1,   59,   91,   61,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   40,   41,   42,   -1,   -1,   -1,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,   -1,  206,  207,  208,  209,  210,
  211,  212,  213,  214,  215,  216,  217,  218,  219,  220,
  221,  222,  223,   91,   41,  123,   -1,   44,   33,   -1,
   -1,  232,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   59,   -1,   61,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,  254,   59,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
   61,   -1,   -1,   -1,   -1,   -1,   -1,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,   -1,   -1,   -1,  123,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  315,   -1,   -1,   33,   -1,   -1,
   -1,  126,   38,   -1,   40,   -1,   42,   43,  257,   45,
   -1,   -1,  123,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  341,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  353,  282,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,   -1,
   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  126,   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  282,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   37,   38,   -1,
   40,   -1,   42,   43,   44,   45,   46,   47,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   59,
   60,   61,   62,   63,   37,   38,   -1,   -1,   41,   42,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  289,   -1,   58,   59,   60,   61,   62,
   63,   91,  298,  299,   94,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   37,   38,   -1,   -1,   41,   42,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
   93,   94,   -1,   -1,  124,   58,   59,   60,   61,   62,
   63,   -1,   37,   38,   -1,   -1,   41,   42,   43,   44,
   45,   -1,   47,   38,   -1,   -1,   41,   -1,   43,   44,
   45,  124,  125,   58,   59,   60,   38,   62,   63,   41,
   93,   94,   44,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   93,   94,
   -1,   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   58,   59,   60,   -1,   62,   63,   -1,   38,  124,
  125,   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   93,   94,   -1,   -1,
   -1,  261,  262,  263,  264,  265,  266,  267,  268,  269,
  270,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,   -1,   93,   94,   -1,   -1,  124,  125,   -1,
   -1,  264,  265,  266,  267,  268,  269,  270,  271,  272,
  273,  274,  275,  276,  277,  278,  279,  280,  281,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,  270,  271,  272,
  273,  274,  275,  276,  277,  278,  279,  280,  281,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,  270,  271,   -1,   -1,  264,
  265,  266,  267,  268,  269,  270,  271,   -1,   -1,   -1,
   -1,   -1,  264,  265,  266,  267,  268,  269,  270,  271,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   -1,   62,   63,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,  270,  271,   -1,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   38,   93,   94,   41,   -1,   -1,
   44,   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,
  270,  271,   -1,   -1,   58,   59,   60,   38,   62,   63,
   41,   93,   94,   44,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,
   38,   62,   63,   41,   -1,   -1,   44,   -1,   -1,   93,
   94,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   38,   62,   63,   41,   -1,   -1,   44,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   38,   -1,   -1,   41,   93,   94,   44,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,   93,   94,
   -1,   -1,   -1,   33,   -1,   -1,  124,  125,   38,   -1,
   40,   38,   42,   43,   41,   45,   46,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,
  125,   58,   59,   -1,   38,   -1,   63,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,
  268,  269,  270,  271,   58,   59,   -1,  124,  125,   63,
   -1,   91,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
   -1,   -1,  264,  265,  266,  267,  268,  269,  270,  271,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,  123,   -1,  125,  126,  124,  125,   -1,
   -1,   -1,  266,  267,  268,  269,  270,  271,   -1,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
  124,  125,   -1,   -1,   -1,  266,  267,  268,  269,  270,
  271,   58,   59,   -1,   38,   -1,   63,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  266,  267,
  268,  269,  270,  271,   58,   59,   -1,   -1,   -1,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
   -1,  266,  267,  268,  269,  270,  271,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,  266,
  267,  268,  269,  270,  271,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   46,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  268,  269,  270,  271,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   46,   -1,
   -1,   -1,   -1,   91,  268,  269,  270,  271,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   33,   42,   43,   -1,
   45,   38,   -1,   40,   41,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   -1,   -1,  123,   -1,  125,  126,   33,
   -1,   -1,   -1,   91,   38,   -1,   40,   -1,   42,   43,
   33,   45,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   33,   -1,   -1,   -1,   93,   38,
   -1,   40,   -1,   42,   43,  123,   45,   -1,  126,   -1,
   -1,  268,  269,  270,  271,   -1,   33,   -1,   -1,   -1,
   59,   38,   -1,   40,   -1,   42,   43,   -1,   45,   93,
   -1,  126,   -1,   -1,   -1,   -1,  270,  271,   33,  126,
   93,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,  126,   42,   43,   -1,   45,   33,   -1,   -1,
   -1,   -1,   38,  126,   40,   41,   42,   43,   -1,   45,
   -1,   33,   -1,   -1,   -1,   -1,   38,  126,   40,   41,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   93,  257,
  258,  259,  260,   -1,  262,  263,  123,   -1,   -1,  126,
   33,   -1,   -1,   -1,   93,   38,   -1,   40,   33,   42,
   43,   -1,   45,   38,   -1,   40,   -1,   42,   43,   -1,
   45,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   33,  262,  263,   -1,  126,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
  126,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  257,  258,  259,  260,  126,  262,  263,   33,   93,   -1,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,  126,  257,  258,  259,  260,   -1,  262,
  263,  126,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   41,   -1,  126,   44,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   44,  262,  263,   -1,   93,   94,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   41,   -1,
   -1,   44,   -1,   65,   -1,  257,  258,  259,  260,   41,
  262,  263,   44,   75,   -1,   58,   59,  124,  125,   -1,
   63,   -1,   -1,   -1,   -1,   -1,   58,   59,   -1,  123,
   -1,   63,   94,   -1,  257,  258,  259,  260,   41,  262,
  263,   44,  257,  258,  259,  260,   -1,  262,  263,   -1,
   93,   94,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,
   63,   93,   41,   -1,   -1,   44,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   58,
   59,  124,  125,   -1,   63,   -1,   -1,   -1,   -1,   -1,
   93,  153,  124,  125,   -1,  157,   -1,  159,   -1,  161,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,  282,   -1,  180,  181,
   -1,   -1,  125,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  270,  271,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  249,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
  282,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  270,  271,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  282,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,
  };

#line 952 "CParser.jay"

}

#line default
namespace yydebug {
        using System;
	 internal interface yyDebug {
		 void push (int state, Object value);
		 void lex (int state, int token, string name, Object value);
		 void shift (int from, int to, int errorFlag);
		 void pop (int state);
		 void discard (int state, int token, string name, Object value);
		 void reduce (int from, int to, int rule, string text, int len);
		 void shift (int from, int to);
		 void accept (Object value);
		 void error (string message);
		 void reject ();
	 }
	 
	 class yyDebugSimple : yyDebug {
		 void println (string s){
			 Console.Error.WriteLine (s);
		 }
		 
		 public void push (int state, Object value) {
			 println ("push\tstate "+state+"\tvalue "+value);
		 }
		 
		 public void lex (int state, int token, string name, Object value) {
			 println("lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		 }
		 
		 public void shift (int from, int to, int errorFlag) {
			 switch (errorFlag) {
			 default:				// normally
				 println("shift\tfrom state "+from+" to "+to);
				 break;
			 case 0: case 1: case 2:		// in error recovery
				 println("shift\tfrom state "+from+" to "+to
					     +"\t"+errorFlag+" left to recover");
				 break;
			 case 3:				// normally
				 println("shift\tfrom state "+from+" to "+to+"\ton error");
				 break;
			 }
		 }
		 
		 public void pop (int state) {
			 println("pop\tstate "+state+"\ton error");
		 }
		 
		 public void discard (int state, int token, string name, Object value) {
			 println("discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		 }
		 
		 public void reduce (int from, int to, int rule, string text, int len) {
			 println("reduce\tstate "+from+"\tuncover "+to
				     +"\trule ("+rule+") "+text);
		 }
		 
		 public void shift (int from, int to) {
			 println("goto\tfrom state "+from+" to "+to);
		 }
		 
		 public void accept (Object value) {
			 println("accept\tvalue "+value);
		 }
		 
		 public void error (string message) {
			 println("error\t"+message);
		 }
		 
		 public void reject () {
			 println("reject");
		 }
		 
	 }
}
// %token constants
 class Token {
  public const int IDENTIFIER = 257;
  public const int CONSTANT = 258;
  public const int STRING_LITERAL = 259;
  public const int SIZEOF = 260;
  public const int PTR_OP = 261;
  public const int INC_OP = 262;
  public const int DEC_OP = 263;
  public const int LEFT_OP = 264;
  public const int RIGHT_OP = 265;
  public const int LE_OP = 266;
  public const int GE_OP = 267;
  public const int EQ_OP = 268;
  public const int NE_OP = 269;
  public const int AND_OP = 270;
  public const int OR_OP = 271;
  public const int MUL_ASSIGN = 272;
  public const int DIV_ASSIGN = 273;
  public const int MOD_ASSIGN = 274;
  public const int ADD_ASSIGN = 275;
  public const int SUB_ASSIGN = 276;
  public const int LEFT_ASSIGN = 277;
  public const int RIGHT_ASSIGN = 278;
  public const int AND_ASSIGN = 279;
  public const int XOR_ASSIGN = 280;
  public const int OR_ASSIGN = 281;
  public const int TYPE_NAME = 282;
  public const int TYPEDEF = 283;
  public const int EXTERN = 284;
  public const int STATIC = 285;
  public const int AUTO = 286;
  public const int REGISTER = 287;
  public const int INLINE = 288;
  public const int RESTRICT = 289;
  public const int CHAR = 290;
  public const int SHORT = 291;
  public const int INT = 292;
  public const int LONG = 293;
  public const int SIGNED = 294;
  public const int UNSIGNED = 295;
  public const int FLOAT = 296;
  public const int DOUBLE = 297;
  public const int CONST = 298;
  public const int VOLATILE = 299;
  public const int VOID = 300;
  public const int BOOL = 301;
  public const int COMPLEX = 302;
  public const int IMAGINARY = 303;
  public const int STRUCT = 304;
  public const int CLASS = 305;
  public const int UNION = 306;
  public const int ENUM = 307;
  public const int ELLIPSIS = 308;
  public const int CASE = 309;
  public const int DEFAULT = 310;
  public const int IF = 311;
  public const int ELSE = 312;
  public const int SWITCH = 313;
  public const int WHILE = 314;
  public const int DO = 315;
  public const int FOR = 316;
  public const int GOTO = 317;
  public const int CONTINUE = 318;
  public const int BREAK = 319;
  public const int RETURN = 320;
  public const int yyErrorCode = 256;
 }
 namespace yyParser {
  using System;
  /** thrown for irrecoverable syntax errors and stack overflow.
    */
  internal class yyException : System.Exception {
    public yyException (string message) : base (message) {
    }
  }
  internal class yyUnexpectedEof : yyException {
    public yyUnexpectedEof (string message) : base (message) {
    }
    public yyUnexpectedEof () : base ("") {
    }
  }

  /** must be implemented by a scanner object to supply input to the parser.
    */
  internal interface yyInput {
    /** move on to next token.
        @return false if positioned beyond tokens.
        @throws IOException on input error.
      */
    bool advance (); // throws java.io.IOException;
    /** classifies current token.
        Should not be called if advance() returned false.
        @return current %token or single character.
      */
    int token ();
    /** associated with current token.
        Should not be called if advance() returned false.
        @return value for token().
      */
    Object value ();
  }
 }
} // close outermost namespace, that MUST HAVE BEEN opened in the prolog
