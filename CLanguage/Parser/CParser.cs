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
//t    "primary_expression : TRUE",
//t    "primary_expression : FALSE",
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
    "COMPLEX","IMAGINARY","TRUE","FALSE","STRUCT","CLASS","UNION","ENUM",
    "ELLIPSIS","CASE","DEFAULT","IF","ELSE","SWITCH","WHILE","DO","FOR",
    "GOTO","CONTINUE","BREAK","RETURN",
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
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 44 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 45 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 52 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 56 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 60 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 64 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 68 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 72 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
	}
  break;
case 13:
#line 76 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 80 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 84 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 88 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list ',' '}'");
	}
  break;
case 17:
  case_17();
  break;
case 18:
  case_18();
  break;
case 19:
#line 110 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 114 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 118 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 122 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 126 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 130 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 134 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 138 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 142 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 143 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 144 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 145 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 152 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 156 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 33:
#line 163 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 167 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 171 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 175 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 183 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 187 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 195 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 199 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 207 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 211 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 215 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 219 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 227 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 231 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 239 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 247 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 255 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 263 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 271 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 278 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 282 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 300 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 301 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 302 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 303 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 304 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 305 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 306 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 307 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 308 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 309 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 310 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 317 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 77:
#line 321 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
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
  case_91();
  break;
case 92:
  case_92();
  break;
case 93:
#line 427 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 94:
#line 428 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 95:
#line 429 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 96:
#line 430 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 97:
#line 431 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 98:
#line 435 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 99:
#line 436 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 100:
#line 437 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 101:
#line 438 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 102:
#line 439 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 103:
#line 440 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 104:
#line 441 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 105:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 106:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 107:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 108:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 109:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 112:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 113:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 114:
#line 454 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 115:
#line 455 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 116:
#line 459 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 117:
#line 460 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 118:
#line 461 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
  break;
case 123:
#line 472 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 124:
#line 473 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString()); }
  break;
case 125:
#line 474 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 126:
#line 475 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString()); }
  break;
case 127:
#line 476 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 132:
#line 490 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 133:
#line 497 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 134:
#line 498 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 135:
#line 503 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 136:
  case_136();
  break;
case 137:
#line 519 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 138:
#line 523 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 139:
#line 527 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 140:
#line 531 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 141:
#line 535 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 142:
#line 539 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 143:
#line 543 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 144:
#line 547 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 145:
  case_145();
  break;
case 146:
  case_146();
  break;
case 147:
  case_147();
  break;
case 148:
#line 575 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 149:
#line 576 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 150:
#line 577 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 151:
#line 578 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 152:
#line 582 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 153:
#line 586 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 154:
#line 590 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 155:
#line 591 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 156:
#line 592 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 157:
#line 599 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
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
case 164:
  case_164();
  break;
case 165:
  case_165();
  break;
case 171:
  case_171();
  break;
case 172:
#line 683 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 173:
#line 687 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 174:
#line 691 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 175:
#line 695 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 176:
#line 699 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 177:
#line 703 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 178:
  case_178();
  break;
case 179:
  case_179();
  break;
case 180:
  case_180();
  break;
case 181:
  case_181();
  break;
case 182:
#line 737 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 183:
#line 741 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 184:
#line 745 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 185:
  case_185();
  break;
case 186:
  case_186();
  break;
case 187:
  case_187();
  break;
case 188:
  case_188();
  break;
case 189:
#line 785 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 203:
#line 817 "CParser.jay"
  {
		yyVal = new Block (GetLocation(yyVals[-1+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 204:
#line 821 "CParser.jay"
  {
        yyVal = new Block (GetLocation(yyVals[-2+yyTop]), (List<Statement>)yyVals[-1+yyTop], GetLocation(yyVals[0+yyTop]));
	}
  break;
case 205:
#line 825 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 206:
#line 826 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 209:
#line 838 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 210:
#line 842 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 211:
#line 849 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 212:
#line 853 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 214:
#line 861 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 215:
#line 865 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], (Statement)yyVals[-5+yyTop]);
	}
  break;
case 216:
#line 869 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop], null, GetLocation(yyVals[-5+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 217:
#line 873 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], null, GetLocation(yyVals[-6+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 218:
#line 877 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop]);
	}
  break;
case 219:
#line 881 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 223:
#line 891 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 224:
#line 895 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 225:
  case_225();
  break;
case 226:
  case_226();
  break;
case 229:
  case_229();
  break;
case 230:
  case_230();
  break;
case 231:
  case_231();
  break;
case 232:
  case_232();
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
void case_17()
#line 93 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 99 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 288 "CParser.jay"
{
		if (yyVals[-1+yyTop] is RelationalOp && ((RelationalOp)yyVals[-1+yyTop]) == RelationalOp.Equals) {
			yyVal = new AssignExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
		}
		else {
			var left = (Expression)yyVals[-2+yyTop]; 
			yyVal = new AssignExpression(left, new BinaryExpression (left, (Binop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]));
		}
	}

void case_79()
#line 330 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_80()
#line 336 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		d.InitDeclarators = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_81()
#line 346 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_82()
#line 352 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_83()
#line 358 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_84()
#line 364 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_85()
#line 370 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_86()
#line 376 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_87()
#line 382 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 388 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 397 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_90()
#line 403 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_91()
#line 412 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[0+yyTop];
		yyVal = id;
	}

void case_92()
#line 418 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[-2+yyTop];
		id.Initializer = (Initializer)yyVals[0+yyTop];
		yyVal = id;
	}

void case_136()
#line 505 "CParser.jay"
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

void case_145()
#line 549 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_146()
#line 556 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_147()
#line 566 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_158()
#line 601 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_159()
#line 610 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_160()
#line 616 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_161()
#line 625 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_162()
#line 630 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_163()
#line 635 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_164()
#line 643 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_165()
#line 649 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_171()
#line 669 "CParser.jay"
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

void case_178()
#line 705 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-1+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_179()
#line 712 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_180()
#line 718 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration> ();
		yyVal = d;
	}

void case_181()
#line 725 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_185()
#line 750 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_186()
#line 757 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_187()
#line 765 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_188()
#line 772 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_225()
#line 900 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_226()
#line 905 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_229()
#line 918 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_230()
#line 927 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_231()
#line 938 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_232()
#line 944 "CParser.jay"
{
		var l = (List<Declaration>)yyVals[-1+yyTop];
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

#line default
   static readonly short [] yyLhs  = {              -1,
    1,    1,    1,    1,    1,    1,    3,    3,    3,    3,
    3,    3,    3,    3,    3,    3,    4,    4,    8,    8,
    8,    8,    8,    8,    8,    8,   10,   10,   10,   10,
    9,    9,   11,   11,   11,   11,   12,   12,   12,   13,
   13,   13,   14,   14,   14,   14,   14,   15,   15,   15,
   16,   16,   17,   17,   18,   18,   19,   19,   20,   20,
   21,   21,    7,    7,   22,   22,   22,   22,   22,   22,
   22,   22,   22,   22,   22,    2,    2,   23,   24,   24,
   25,   25,   25,   25,   25,   25,   25,   25,   26,   26,
   31,   31,   27,   27,   27,   27,   27,   28,   28,   28,
   28,   28,   28,   28,   28,   28,   28,   28,   28,   28,
   28,   28,   34,   34,   34,   36,   36,   36,   38,   38,
   38,   38,   35,   35,   35,   35,   35,   39,   39,   40,
   40,   30,   32,   32,   42,   42,   42,   42,   42,   42,
   42,   42,   42,   42,   42,   42,   42,   41,   41,   41,
   41,   43,   43,   29,   29,   29,   44,   44,   46,   46,
   47,   47,   47,   45,   45,    5,    5,   48,   48,   48,
   49,   49,   49,   49,   49,   49,   49,   49,   49,   49,
   49,   33,   33,   33,    6,    6,    6,    6,   50,   51,
   51,   52,   52,   53,   53,   53,   53,   53,   53,   54,
   54,   54,   37,   37,   59,   59,   60,   60,   55,   55,
   56,   56,   56,   57,   57,   57,   57,   57,   57,   58,
   58,   58,   58,   58,    0,    0,   61,   61,   62,   62,
   63,   63,
  };
   static readonly short [] yyLen = {           2,
    1,    1,    1,    1,    1,    3,    1,    4,    3,    4,
    3,    3,    2,    2,    6,    7,    1,    3,    1,    2,
    2,    2,    2,    2,    2,    4,    1,    1,    1,    1,
    1,    4,    1,    3,    3,    3,    1,    3,    3,    1,
    3,    3,    1,    3,    3,    3,    3,    1,    3,    3,
    1,    3,    1,    3,    1,    3,    1,    3,    1,    3,
    1,    5,    1,    3,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    3,    1,    2,    3,
    1,    2,    1,    2,    1,    2,    1,    2,    1,    3,
    1,    3,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    3,    2,    2,    1,    1,    1,    2,    1,
    2,    1,    4,    5,    5,    6,    2,    1,    3,    1,
    3,    1,    2,    1,    1,    3,    5,    4,    4,    6,
    6,    5,    4,    3,    4,    4,    3,    1,    2,    2,
    3,    1,    2,    1,    1,    1,    1,    3,    1,    3,
    2,    2,    1,    1,    3,    1,    2,    1,    1,    2,
    3,    2,    3,    3,    4,    3,    4,    2,    3,    3,
    4,    1,    3,    4,    1,    2,    3,    4,    2,    1,
    2,    3,    2,    1,    1,    1,    1,    1,    1,    3,
    4,    3,    2,    3,    1,    2,    1,    1,    1,    2,
    5,    7,    5,    5,    7,    6,    7,    6,    7,    3,
    2,    2,    2,    3,    1,    2,    1,    1,    4,    3,
    1,    2,
  };
   static readonly short [] yyDefRed = {            0,
  112,   93,   94,   95,   96,   97,  132,  155,   99,  100,
  101,  102,  105,  106,  103,  104,  154,  156,   98,  107,
  108,  109,  116,  117,  118,    0,    0,  228,    0,    0,
    0,    0,    0,  110,  111,    0,  225,  227,    0,    0,
  226,  135,    0,    0,   79,    0,   89,    0,    0,    0,
   82,   84,   86,   88,    0,    0,  114,    0,    0,    0,
  128,    0,  152,  150,    0,    0,   80,    0,  231,    0,
  230,    0,    0,    0,    0,  113,    0,    2,    3,    0,
    0,    0,    4,    5,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  203,    0,    0,   27,
   28,   29,   30,  209,    7,    0,    0,   76,    0,   33,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   63,  207,  195,  208,  194,  196,  197,  198,  199,
    0,  205,    0,    0,  123,    0,  136,  153,  151,   90,
    0,    1,    0,  182,   92,  232,  229,  164,  147,    0,
    0,    0,    0,  159,    0,  144,    0,    0,    0,    0,
    0,   25,    0,   20,   21,   31,   78,    0,    0,    0,
    0,    0,    0,    0,    0,  221,  222,  223,    0,    0,
    0,    0,    0,    0,   22,   23,    0,  210,    0,   13,
   14,    0,    0,    0,   66,   67,   68,   69,   70,   71,
   72,   73,   74,   75,   65,    0,   24,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  204,  206,  124,    0,
  131,  125,  129,    0,    0,    0,  185,    0,    0,  190,
    0,    0,  161,    0,  162,    0,  145,  146,    0,    0,
    0,  143,  139,    0,  138,    0,    0,  200,    0,    0,
    0,  202,    0,    0,    0,    0,    0,    0,  220,  224,
    6,    0,  119,  121,    0,    0,  167,   77,   12,    9,
    0,   17,    0,   11,   64,   34,   35,   36,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  126,    0,  193,  183,    0,  186,
  189,  191,  178,    0,    0,  172,    0,    0,    0,    0,
    0,  165,  158,  160,    0,    0,  142,  137,    0,    0,
  201,    0,    0,    0,    0,    0,    0,    0,   32,   10,
    0,    8,    0,  192,  184,  187,    0,  179,  171,  176,
  173,  180,    0,  174,    0,    0,  140,  141,    0,  213,
  214,    0,    0,    0,    0,    0,    0,   18,   62,  188,
  181,  177,  175,    0,    0,  218,    0,  216,    0,   15,
    0,  212,  215,  219,  217,   16,
  };
  protected static readonly short [] yyDgoto  = {            27,
  105,  106,  107,  281,  181,  236,  108,  109,  110,  111,
  112,  113,  114,  115,  116,  117,  118,  119,  120,  121,
  122,  206,  168,   28,   70,   46,   30,   31,   32,   33,
   47,   62,  237,   34,   35,   36,  124,  184,   60,   61,
   49,   50,   65,  314,  152,  153,  154,  315,  246,  238,
  239,  240,  125,  126,  127,  128,  129,  130,  131,  132,
   37,   38,   72,
  };
  protected static readonly short [] yySindex = {         2633,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, -112, 2633,    0,   -3, 2633,
 2633, 2633, 2633,    0,    0, -100,    0,    0, -103, -216,
    0,    0,    2,  -15,    0,  125,    0, 1706,  -35,  -10,
    0,    0,    0,    0,  -65,   40,    0, -216,    7,   22,
    0,   65,    0,    0,  -15,    2,    0,  490,    0,   -3,
    0, 2487,  -10, 1589,  539,    0,   84,    0,    0, 1263,
 1286, 1286,    0,    0, 1302,  106,  142,  166,  188,  249,
  199,  -71,  185,  210,  911,  412,    0, 1302, 1302,    0,
    0,    0,    0,    0,    0,  130,  116,    0,  768,    0,
 1302,  248,  213,  -52,   51,  -51,  239,  194,  226,   42,
  -55,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  108,    0,   35, 1302,    0, -113,    0,    0,    0,    0,
  244,    0,  842,    0,    0,    0,    0,    0,    0,   44,
  313,   88,  320,    0,  -69,    0,  857,  281,  550,  249,
  412,    0,  412,    0,    0,    0,    0,  311,  249, 1302,
 1302, 1302,  102,  477,  363,    0,    0,    0,  146,   95,
  391, 1558, 1558,  162,    0,    0, 1302,    0,  180,    0,
    0,  931, 1302,  196,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 1302,    0, 1302, 1302, 1302,
 1302, 1302, 1302, 1302, 1302, 1302, 1302, 1302, 1302, 1302,
 1302, 1302, 1302, 1302, 1302, 1302,    0,    0,    0, -108,
    0,    0,    0, 1302,  203,   48,    0,  490,  147,    0,
 1436,  942,    0,    9,    0,   29,    0,    0,  204, 2546,
  834,    0,    0, 1302,    0,  960,  358,    0,  415,  421,
  249,    0,  114,  139,  216,  451,  980,  980,    0,    0,
    0, 1018,    0,    0, 1488,   68,    0,    0,    0,    0,
  252,    0,   50,    0,    0,    0,    0,    0,  248,  248,
  213,  213,  -52,  -52,  -52,  -52,   51,   51,  -51,  239,
  194,  226,   42,  167,    0,  402,    0,    0,    5,    0,
    0,    0,    0,  463,  464,    0, 1115,  420,   29, 1671,
 1211,    0,    0,    0,  423,  425,    0,    0,  398,  398,
    0,  249,  249,  249, 1302, 1142, 1229,  842,    0,    0,
 1302,    0, 1302,    0,    0,    0,  490,    0,    0,    0,
    0,    0,  473,    0, 1250,  432,    0,    0,  217,    0,
    0,  270,  249,  275,  249,  332,   53,    0,    0,    0,
    0,    0,    0,  249,  478,    0,  249,    0,  249,    0,
  820,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   63,
  639,  968,  975,    0,    0,    0,    0,    0, 1326,    0,
    0,    0,    0,   21,    0,    0,    0,  150,    0, 1525,
    0,    0,    0,    0, 1374,    0,    0,    0,   54,    0,
    0,    0,    0,    0,   23,    0,    0,    0,    0,    0,
    0,    0, 1643,    0,    0,    0, 1979,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2006,    0, 2074,    0,
    0, 2102, 2290, 2386,  -23,  560,  -34,  485, 1438,  483,
 1029,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  150,    0,    0,    0,    0,    0,    0,    0,    0,  343,
    0,    0,  498,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   47,  158,  499,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  344,    0,  345,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  504,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 2111, 2170,
 2349, 2358, 2409, 2446, 2454, 2483, 2542, 2602, 2643,  598,
  600, 1705,  571,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  390,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 2034,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  181,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0, 1264,    0,    0,  110,  209,  -66,  707,  -85,    0,
  224,  233,  165,  229,  328,  329,  327,  331,  333,    0,
  -82,    0, -115,   61,    1,    0,    0,   14, 2644,    0,
  486,  -13,  -68,    0,    0,    0,  155,  276,  501,  -62,
   11,  -43,  -21,  -70,    0,    0,  306,   31, -149, -263,
    0,  318,  273,    0, -152,    0,    0,    0,    0,  442,
  536,    0,    0,
  };
  protected static readonly short [] yyTable = {           145,
   29,  144,  167,  151,   43,   73,   53,  226,  158,   53,
   40,  232,  185,  186,   48,   48,  305,   48,  231,   58,
   48,  268,   56,   53,   53,  207,   44,   29,   53,   74,
   51,   52,   53,   54,   48,   48,   43,  103,   44,   48,
   59,   43,   98,   44,   96,  347,   99,  100,  241,  101,
  235,  167,  141,  159,   64,   45,  141,   56,   53,   53,
  148,  148,  149,  149,  148,  136,  149,  134,  320,   48,
   48,  186,  103,  233,  150,  139,  144,   98,  230,   96,
   75,   99,  100,  241,  101,   44,  120,  120,  120,   53,
   53,  309,  257,  187,  319,  234,  381,  130,  104,  242,
   48,   48,   81,   81,   81,  137,   81,  275,   69,  182,
  217,  148,  218,  149,  336,  337,  123,  347,  306,  321,
  278,   81,  286,  287,  288,  282,  319,  143,  248,  345,
  102,  249,  146,  251,  242,  271,  243,  120,  187,  285,
  103,  160,  342,   59,   39,   98,  135,   96,   59,   99,
  100,  167,  101,   81,  332,  192,   55,  187,  242,  229,
  244,  194,   56,  169,   97,  102,  104,  233,   66,  310,
  186,  144,  308,  187,  182,  318,  182,  380,  130,  333,
  245,  170,  187,   67,  325,  175,  339,  326,  188,  187,
   57,  123,  235,   91,  276,  182,  182,  122,  122,  122,
   73,  275,   71,   44,  270,  171,  193,  311,   91,   76,
  187,  213,  214,  211,  277,  225,  219,  220,  211,    8,
  211,   42,  211,  211,  343,  211,  147,  172,   17,   18,
   56,  186,  227,  102,  267,   53,   53,  234,  174,  211,
  346,  150,  144,  176,   48,   48,   48,   48,  122,  353,
  150,  244,  242,   42,  356,  211,  334,  212,   42,  187,
  369,  142,   78,   79,   80,   42,   81,   82,  177,  186,
  259,  144,  260,    8,  368,  150,  221,  148,  370,  149,
  144,  103,   17,   18,  210,  276,   98,  222,   96,  208,
   99,  100,  340,  101,  209,  341,   77,   78,   79,   80,
   42,   81,   82,  211,   68,  211,  211,  104,   83,   84,
  375,  224,  346,  187,  144,  377,  215,  216,  187,   81,
  150,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   83,   84,   23,   24,   25,   26,  223,
   85,   86,   87,  247,   88,   89,   90,   91,   92,   93,
   94,   95,  173,  250,   77,   78,   79,   80,  261,   81,
   82,   56,  379,  253,  102,  187,  189,  190,  191,  293,
  294,  295,  296,  163,  168,  169,  163,  168,  169,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   83,   84,   23,   24,   25,   26,  266,   85,   86,
   87,  269,   88,   89,   90,   91,   92,   93,   94,   95,
  170,  272,  258,  170,  289,  290,  279,  211,  211,  211,
  211,  262,  211,  211,  103,  291,  292,  297,  298,   98,
  328,   96,  284,   99,  100,  329,  101,  273,  274,  307,
  322,  330,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  211,
  335,  211,  211,  211,  344,  211,  211,  211,  211,  211,
  211,  211,  211,  348,  349,   77,   78,   79,   80,  103,
   81,   82,  351,  371,   98,  357,   96,  358,   99,  100,
  338,  101,  103,   59,  373,   55,   59,   98,   55,   96,
  374,   99,  100,  331,  101,  104,  383,  102,  157,  166,
   59,   59,   55,   55,  168,   59,  367,   55,  299,  301,
  300,  140,   83,   84,  302,  324,  312,  303,  133,   85,
   86,   87,   41,   88,   89,   90,   91,   92,   93,   94,
   95,  103,  228,    0,    0,   59,   98,   55,   96,    0,
  157,  100,  103,  101,    0,    0,    0,   98,    0,   96,
    0,  256,  100,    0,  101,    0,    0,   51,    0,    0,
   51,    0,  102,   51,  359,  360,  361,   59,   55,   55,
    0,   60,  143,    0,   60,  102,    0,   51,   51,    0,
    0,    0,   51,    0,    0,    0,    0,    0,   60,   60,
    0,  156,    0,   60,    0,  376,    0,  378,   54,    0,
   56,   54,  255,   56,    0,    0,  382,    0,    0,  384,
    0,  385,   51,   51,    0,   54,   54,   56,   56,    0,
   54,    0,   56,   60,  102,    0,    0,    0,  142,   78,
   79,   80,    0,   81,   82,  102,    0,    0,   83,   83,
   83,    0,   83,   51,   51,    0,    0,    0,    0,    0,
   54,   54,   56,    1,    0,   60,    0,   83,    0,    0,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   83,   84,   23,   24,   25,
   26,   54,   54,   56,   56,    0,    0,    0,    0,   83,
    0,    0,    0,  142,   78,   79,   80,    0,   81,   82,
    0,    0,    0,    0,    0,    0,  142,   78,   79,   80,
    0,   81,   82,   59,   55,   55,    0,    0,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   83,   84,   23,   24,   25,   26,  162,  164,  165,    0,
    0,  166,    0,   83,   84,  142,   78,   79,   80,    0,
   81,   82,    0,    0,  166,  166,  142,   78,   79,   80,
    0,   81,   82,    0,    0,    0,    0,  166,    0,    0,
    0,    0,    0,  155,    0,    0,    0,    8,  205,   51,
   51,    0,    0,    0,  254,    0,   17,   18,    8,    0,
  166,   60,   83,   84,    0,    0,    0,   17,   18,    0,
    0,    0,  103,   83,   84,    0,    0,   98,    0,   96,
    0,   99,  100,  166,  101,  235,  103,   54,   54,   56,
   56,   98,    0,   96,  103,   99,  100,    0,  101,   98,
    0,   96,    0,   99,  100,    0,  101,  235,    0,  103,
    0,    0,    0,    0,   98,   83,   96,    0,   99,  100,
    0,  101,    0,    0,    0,    0,    0,    0,    0,    0,
  234,    0,    0,    0,  166,  166,  166,  166,  166,  166,
  166,  166,  166,  166,  166,  166,  166,  166,  166,  166,
  166,  166,  234,    0,    0,    0,    0,    0,    0,    0,
  166,    0,  143,  103,  386,  102,    0,    0,   98,  252,
   96,    0,   99,  100,    0,  101,    0,    0,    0,  102,
    0,    0,  166,  103,  143,    0,    0,  102,   98,  178,
   96,  280,   99,  100,  103,  101,    0,    0,  166,   98,
    0,   96,  102,  317,  100,    0,  101,    0,    0,    0,
    0,    0,  103,    0,    0,    0,    0,   98,    0,   96,
    0,   99,  100,    0,  101,    0,    0,   85,   85,   85,
    0,   85,  103,    0,   87,   87,   87,   98,   87,   96,
    0,   99,  100,  166,  101,    0,   85,    0,    0,    0,
    0,    0,    0,   87,  316,    0,  102,    0,  104,  195,
  196,  197,  198,  199,  200,  201,  202,  203,  204,  166,
  103,    0,  327,    0,    0,   98,  102,   96,   85,   99,
  100,  166,  101,    0,    0,   87,    0,  102,    0,   61,
    0,    0,   61,    0,    0,    0,  142,   78,   79,   80,
    0,   81,   82,    0,    0,  102,   61,   61,    0,    0,
  142,   78,   79,   80,    0,   81,   82,    0,  142,   78,
   79,   80,    0,   81,   82,  102,    0,    0,    0,    0,
    0,    0,    0,  142,   78,   79,   80,    0,   81,   82,
    0,   61,    8,   83,   84,    0,    0,    0,    0,    0,
    0,   17,   18,    0,    0,    0,    0,   83,   84,    0,
  338,    0,    0,  102,    0,   83,   84,  103,    0,    0,
    0,    0,   98,   61,   96,    0,   99,  100,    0,  101,
   83,   84,    0,    0,    0,    0,    0,  142,   78,   79,
   80,    0,   81,   82,  103,    0,    0,    0,    0,   98,
    0,   96,  363,   99,  100,    0,  101,  142,   78,   79,
   80,    0,   81,   82,    0,    0,    0,    0,  142,   78,
   79,   80,    0,   81,   82,    0,    0,  350,    0,    0,
    0,    0,    0,    0,   83,   84,  142,   78,   79,   80,
    0,   81,   82,    0,   85,    0,    0,    0,    0,    0,
    0,   87,    0,    0,   83,   84,  142,   78,   79,   80,
  102,   81,   82,  103,    0,   83,   84,    0,   98,    0,
   96,    0,  355,  100,    0,  101,    0,    0,    0,    0,
    0,  103,    0,   83,   84,    0,   98,  102,   96,  365,
   99,  100,    0,  101,  142,   78,   79,   80,    0,   81,
   82,    0,  103,   83,   84,    0,    0,   98,    0,   96,
    0,   99,  100,    0,  101,  103,    0,    0,    0,    0,
   98,    0,  161,  354,   99,  100,    0,  101,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  103,    0,
    0,   83,   84,   98,    0,  163,    0,   99,  100,    0,
  101,    0,    0,    0,  103,    0,  102,    0,    0,   98,
    0,   96,  372,   99,  100,    0,  101,    0,    0,    0,
    0,    0,    0,    0,  102,    0,    0,    0,  179,  180,
    0,    0,    0,    0,    0,  127,  127,  127,    0,  127,
    0,  142,   78,   79,   80,  102,   81,   82,    0,    0,
    0,    0,    0,    0,  127,    0,    0,    0,  102,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  142,   78,
   79,   80,    0,   81,   82,    0,    0,    0,    0,    0,
    0,  102,    0,  115,  115,  115,  127,  115,   83,   84,
    0,    0,    0,    0,  180,    0,  180,  102,    0,    0,
    0,    0,  115,  263,  264,  265,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   83,   84,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  283,    0,    0,    0,
    0,    0,    0,    0,  115,    0,    0,  142,   78,   79,
   80,    0,   81,   82,    0,  241,  313,   44,   57,    0,
    0,   57,    0,    0,    0,  142,   78,   79,   80,  304,
   81,   82,    0,    0,    0,   57,   57,    0,    0,    0,
   57,    0,    0,    0,    0,    0,  142,   78,   79,   80,
    0,   81,   82,    0,   83,   84,    0,    0,    0,  142,
   78,   79,   80,    0,   81,   82,  242,  275,  313,   44,
   57,    0,   83,   84,    0,    0,    0,    0,    0,    0,
    0,    0,  142,   78,   79,   80,    0,   81,   82,    0,
    0,    0,    0,   83,   84,    0,    0,    0,  142,   78,
   79,   80,   57,   81,   82,  134,   83,   84,  134,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  242,    0,
    0,    0,  127,  134,    0,  134,    0,    0,    0,   83,
   84,    0,    0,    0,    0,    0,    0,    0,  362,  364,
  366,    0,    0,    0,    0,   83,   84,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  149,
  115,  127,  127,  127,  127,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  134,    0,    0,
    0,    0,    0,    0,    0,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,    0,    0,  115,
  115,  115,  115,  133,    0,    0,  133,    0,    0,    0,
    0,    0,   42,    0,    0,    0,    0,    0,    0,    0,
    0,  133,    0,  133,    0,    0,    0,   57,   57,    0,
    0,  352,    0,    0,    0,    0,    0,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,   58,    0,    0,   58,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   58,   58,    0,  133,   68,   58,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,   58,    0,    0,
    0,    0,    0,    0,    0,    0,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,   56,   58,
  134,  134,  134,  134,    0,    0,    0,    0,    0,    1,
    0,    0,    0,    0,    0,  148,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,    0,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,    0,    0,  133,  133,
  133,  133,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   58,   58,   23,   24,   25,   26,
    0,    0,    0,    0,    0,    0,    0,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,    1,    1,    0,    1,    0,
    1,    1,    1,    1,    1,    1,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    1,    1,    1,
    1,    1,   19,   19,    0,    0,   19,   19,   19,   19,
   19,    0,   19,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   19,   19,   19,   19,   19,   19,    1,
   26,   26,    1,    0,   26,   26,   26,   26,   26,    0,
   26,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   26,   26,   26,   26,   26,   26,    0,   19,   19,
    0,    0,    1,    0,    0,    0,    0,    0,    0,    0,
   31,   31,    0,    0,   31,   31,   31,   31,   31,    0,
   31,    0,    0,    0,    0,    0,   26,   26,    0,   19,
   19,   31,   31,   31,    0,   31,   31,    0,    0,   37,
    0,    0,   37,    0,   37,   37,   37,    0,   38,    0,
    0,   38,    0,   38,   38,   38,    0,   26,   26,   37,
   37,   37,    0,   37,   37,    0,   31,   31,   38,   38,
   38,    0,   38,   38,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   37,   37,    0,   31,   31,    0,
    0,    0,    0,   38,   38,    0,    0,   39,    0,    0,
   39,    0,   39,   39,   39,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   37,   37,   39,   39,   39,
    0,   39,   39,    0,   38,   38,    0,    0,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    0,    0,   39,   39,    0,    0,    0,    0,    0,   19,
   19,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,    0,    0,    0,
    0,    0,    0,   39,   39,    0,    0,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   40,    0,    0,
   40,    0,    0,   40,    0,    0,    0,   31,   31,   31,
   31,   31,   31,   31,   31,    0,    0,   40,   40,   40,
    0,   40,   40,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   37,   37,   37,   37,   37,
   37,   37,   37,    0,   38,   38,   38,   38,   38,   38,
   38,   38,   40,   40,    0,    0,   41,    0,    0,   41,
    0,    0,   41,    0,    0,   42,    0,    0,   42,    0,
    0,   42,    0,    0,    0,    0,   41,   41,   41,    0,
   41,   41,    0,   40,   40,   42,   42,   42,    0,   42,
   42,    0,    0,   43,    0,    0,   43,    0,    0,   43,
    0,    0,    0,   39,   39,   39,   39,   39,   39,   39,
   39,   41,   41,   43,   43,   43,   46,   43,   43,   46,
   42,   42,   46,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   46,   46,   46,    0,
   46,   46,   41,   41,    0,    0,    0,    0,   43,   43,
    0,   42,   42,   47,    0,    0,   47,    0,    0,   47,
    0,   44,    0,    0,   44,    0,    0,   44,    0,    0,
    0,   46,   46,   47,   47,   47,    0,   47,   47,   43,
   43,   44,   44,   44,    0,   44,   44,    0,    0,    0,
   45,    0,    0,   45,    0,    0,   45,    0,    0,    0,
    0,    0,   46,   46,    0,    0,    0,    0,   47,   47,
   45,   45,   45,    0,   45,   45,   44,   44,    0,    0,
    0,    0,    0,   40,   40,   40,   40,   40,   40,   40,
   40,    0,    0,    0,    0,    0,    0,    0,    0,   47,
   47,    0,    0,    0,    0,   45,   45,   44,   44,   49,
    0,    0,   49,    0,    0,   49,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   49,
   49,    0,    0,    0,   49,    0,   45,   45,    0,   56,
    0,    0,   41,   41,   41,   41,   41,   41,   41,   41,
    0,   42,   42,   42,   42,   42,   42,   42,   42,    0,
    0,    0,    0,    0,   49,   49,    0,    0,    0,   50,
    0,    0,   50,    0,    0,   50,    0,    0,    0,    0,
    0,   43,   43,   43,   43,   43,   43,    0,    0,   50,
   50,    0,    0,    0,   50,   49,   49,    0,    0,    0,
    0,    0,    0,    0,   46,   46,   46,   46,   46,   46,
   52,    0,    0,   52,    0,    0,   52,   63,    0,    0,
    0,    0,    0,    0,   50,   50,    0,    0,    0,    0,
   52,   52,    0,    0,    0,   52,    0,    0,  138,    0,
    0,   47,   47,   47,   47,   47,   47,    0,   63,   44,
   44,   44,   44,   44,   44,   50,   50,    0,    0,    0,
    0,    0,    0,    0,    0,   52,   52,    0,    0,  183,
    0,    0,    0,    0,    0,    0,    0,    0,   45,   45,
   45,   45,   45,   45,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   52,   52,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    0,    0,   63,    0,
    0,    0,  138,    0,  183,    0,  183,    0,    0,   49,
   49,   49,   49,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  183,  183,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,  323,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   50,
   50,   50,   50,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  138,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   52,   52,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,    0,    0,   23,   24,
   25,   26,
  };
  protected static readonly short [] yyCheck = {            68,
    0,   68,   85,   74,   40,   49,   41,   63,   75,   44,
  123,  125,   98,   99,   38,   29,  125,   41,  134,  123,
   44,  174,  123,   58,   59,  111,   42,   27,   63,   40,
   30,   31,   32,   33,   58,   59,   40,   33,   42,   63,
  257,   40,   38,   42,   40,  309,   42,   43,   40,   45,
   46,  134,   66,   75,   44,   59,   70,  123,   93,   94,
   40,   41,   40,   41,   44,   44,   44,   61,   40,   93,
   94,  157,   33,  136,   74,   65,  143,   38,   44,   40,
   91,   42,   43,   40,   45,   42,   40,   41,   42,  124,
  125,   44,  159,   44,  244,   91,   44,   44,   59,   91,
  124,  125,   40,   41,   42,   41,   44,   40,   48,   96,
   60,   91,   62,   91,  267,  268,   56,  381,  234,   91,
  187,   59,  208,  209,  210,  192,  276,  123,   41,  125,
  126,   44,   72,  155,   91,   41,  150,   91,   44,  206,
   33,   58,   93,  257,  257,   38,  125,   40,  257,   42,
   43,  234,   45,   91,   41,   40,  257,   44,   91,  125,
  150,   46,  123,   58,  125,  126,   59,  230,   44,  238,
  256,  238,  125,   44,  161,  242,  163,  125,  125,   41,
  150,   40,   44,   59,  251,  257,  272,  254,   59,   44,
   36,  131,   46,   44,  184,  182,  183,   40,   41,   42,
  244,   40,   48,   42,   59,   40,   91,   61,   59,   55,
   44,  264,  265,   33,  184,  271,  268,  269,   38,  289,
   40,  257,   42,   43,   58,   45,   72,   40,  298,  299,
  123,  317,  125,  126,  174,  270,  271,   91,   40,   59,
  309,  241,  309,   59,  268,  269,  270,  271,   91,  320,
  250,  241,   91,  257,  321,   43,   41,   45,  257,   44,
  343,  257,  258,  259,  260,  257,  262,  263,   59,  355,
  161,  338,  163,  289,  341,  275,   38,  257,  347,  257,
  347,   33,  298,  299,   37,  275,   38,   94,   40,   42,
   42,   43,   41,   45,   47,   44,  257,  258,  259,  260,
  257,  262,  263,  123,   61,  125,  126,   59,  304,  305,
   41,  270,  381,   44,  381,   41,  266,  267,   44,  257,
  320,  282,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  124,
  311,  312,  313,   41,  315,  316,  317,  318,  319,  320,
  321,  322,   90,   44,  257,  258,  259,  260,   58,  262,
  263,  123,   41,   93,  126,   44,  261,  262,  263,  215,
  216,  217,  218,   41,   41,   41,   44,   44,   44,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  316,  311,  312,
  313,   59,  315,  316,  317,  318,  319,  320,  321,  322,
   41,   41,  160,   44,  211,  212,  257,  257,  258,  259,
  260,  169,  262,  263,   33,  213,  214,  219,  220,   38,
   93,   40,  257,   42,   43,   41,   45,  182,  183,  257,
  257,   41,  282,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
   40,  311,  312,  313,   93,  315,  316,  317,  318,  319,
  320,  321,  322,   41,   41,  257,  258,  259,  260,   33,
  262,  263,   93,   41,   38,   93,   40,   93,   42,   43,
  123,   45,   33,   41,   93,   41,   44,   38,   44,   40,
  314,   42,   43,  261,   45,   59,   59,  126,   41,   41,
   58,   59,   58,   59,   41,   63,  338,   63,  221,  223,
  222,   66,  304,  305,  224,  250,  239,  225,   58,  311,
  312,  313,   27,  315,  316,  317,  318,  319,  320,  321,
  322,   33,  131,   -1,   -1,   93,   38,   93,   40,   -1,
   42,   43,   33,   45,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,   -1,   38,   -1,   -1,
   41,   -1,  126,   44,  332,  333,  334,  125,  124,  125,
   -1,   41,  123,   -1,   44,  126,   -1,   58,   59,   -1,
   -1,   -1,   63,   -1,   -1,   -1,   -1,   -1,   58,   59,
   -1,   93,   -1,   63,   -1,  363,   -1,  365,   41,   -1,
   41,   44,   93,   44,   -1,   -1,  374,   -1,   -1,  377,
   -1,  379,   93,   94,   -1,   58,   59,   58,   59,   -1,
   63,   -1,   63,   93,  126,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,  126,   -1,   -1,   40,   41,
   42,   -1,   44,  124,  125,   -1,   -1,   -1,   -1,   -1,
   93,   94,   93,  282,   -1,  125,   -1,   59,   -1,   -1,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  124,  125,  124,  125,   -1,   -1,   -1,   -1,   91,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,  271,  270,  271,   -1,   -1,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,   80,   81,   82,   -1,
   -1,   85,   -1,  304,  305,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   98,   99,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,  111,   -1,   -1,
   -1,   -1,   -1,  285,   -1,   -1,   -1,  289,   61,  270,
  271,   -1,   -1,   -1,  285,   -1,  298,  299,  289,   -1,
  134,  271,  304,  305,   -1,   -1,   -1,  298,  299,   -1,
   -1,   -1,   33,  304,  305,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,  157,   45,   46,   33,  270,  271,  270,
  271,   38,   -1,   40,   33,   42,   43,   -1,   45,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   46,   -1,   33,
   -1,   -1,   -1,   -1,   38,  257,   40,   -1,   42,   43,
   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   91,   -1,   -1,   -1,  208,  209,  210,  211,  212,  213,
  214,  215,  216,  217,  218,  219,  220,  221,  222,  223,
  224,  225,   91,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  234,   -1,  123,   33,  125,  126,   -1,   -1,   38,   93,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,  126,
   -1,   -1,  256,   33,  123,   -1,   -1,  126,   38,   59,
   40,   41,   42,   43,   33,   45,   -1,   -1,  272,   38,
   -1,   40,  126,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,   -1,   40,   41,   42,
   -1,   44,   33,   -1,   40,   41,   42,   38,   44,   40,
   -1,   42,   43,  317,   45,   -1,   59,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   93,   -1,  126,   -1,   59,  272,
  273,  274,  275,  276,  277,  278,  279,  280,  281,  343,
   33,   -1,   93,   -1,   -1,   38,  126,   40,   91,   42,
   43,  355,   45,   -1,   -1,   91,   -1,  126,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,  126,   58,   59,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  257,  258,
  259,  260,   -1,  262,  263,  126,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   93,  289,  304,  305,   -1,   -1,   -1,   -1,   -1,
   -1,  298,  299,   -1,   -1,   -1,   -1,  304,  305,   -1,
  123,   -1,   -1,  126,   -1,  304,  305,   33,   -1,   -1,
   -1,   -1,   38,  125,   40,   -1,   42,   43,   -1,   45,
  304,  305,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   41,   42,   43,   -1,   45,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,  304,  305,  257,  258,  259,  260,
   -1,  262,  263,   -1,  257,   -1,   -1,   -1,   -1,   -1,
   -1,  257,   -1,   -1,  304,  305,  257,  258,  259,  260,
  126,  262,  263,   33,   -1,  304,  305,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   33,   -1,  304,  305,   -1,   38,  126,   40,   41,
   42,   43,   -1,   45,  257,  258,  259,  260,   -1,  262,
  263,   -1,   33,  304,  305,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   93,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,
   -1,  304,  305,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   33,   -1,  126,   -1,   -1,   38,
   -1,   40,   93,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,   95,   96,
   -1,   -1,   -1,   -1,   -1,   40,   41,   42,   -1,   44,
   -1,  257,  258,  259,  260,  126,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,  126,   -1,   40,   41,   42,   91,   44,  304,  305,
   -1,   -1,   -1,   -1,  161,   -1,  163,  126,   -1,   -1,
   -1,   -1,   59,  170,  171,  172,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  304,  305,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  193,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   91,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   40,   41,   42,   41,   -1,
   -1,   44,   -1,   -1,   -1,  257,  258,  259,  260,  226,
  262,  263,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,
   63,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,  304,  305,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   91,   40,   41,   42,
   93,   -1,  304,  305,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,  304,  305,   -1,   -1,   -1,  257,  258,
  259,  260,  125,  262,  263,   41,  304,  305,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   91,   -1,
   -1,   -1,  257,   59,   -1,   61,   -1,   -1,   -1,  304,
  305,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  335,  336,
  337,   -1,   -1,   -1,   -1,  304,  305,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,   41,
  257,  306,  307,  308,  309,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,   -1,   -1,  306,
  307,  308,  309,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   -1,   61,   -1,   -1,   -1,  270,  271,   -1,
   -1,   41,   -1,   -1,   -1,   -1,   -1,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,   -1,
   -1,  306,  307,  308,  309,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   58,   59,   -1,  123,   61,   63,   -1,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,   -1,   -1,  306,  307,  308,  309,   93,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  123,  125,
  306,  307,  308,  309,   -1,   -1,   -1,   -1,   -1,  282,
   -1,   -1,   -1,   -1,   -1,  257,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,   -1,   -1,  306,  307,  308,  309,   -1,   -1,   -1,
  282,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,   -1,   -1,  306,  307,  308,  309,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,   -1,   -1,  306,  307,
  308,  309,  282,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  270,  271,  306,  307,  308,  309,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,   -1,
   -1,  306,  307,  308,  309,   37,   38,   -1,   40,   -1,
   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   63,   37,   38,   -1,   -1,   41,   42,   43,   44,
   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,   91,
   37,   38,   94,   -1,   41,   42,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   61,   62,   63,   -1,   93,   94,
   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   37,   38,   -1,   -1,   41,   42,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,
  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,   38,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   38,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,  124,  125,   58,
   59,   60,   -1,   62,   63,   -1,   93,   94,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,  124,  125,   -1,
   -1,   -1,   -1,   93,   94,   -1,   -1,   38,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,  124,  125,   -1,   -1,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,  270,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,  270,  271,  272,  273,  274,
  275,  276,  277,  278,  279,  280,  281,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,  264,  265,  266,
  267,  268,  269,  270,  271,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,  270,  271,   -1,   -1,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,
  269,  270,  271,   -1,  264,  265,  266,  267,  268,  269,
  270,  271,   93,   94,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   -1,   -1,   58,   59,   60,   -1,
   62,   63,   -1,  124,  125,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,  270,
  271,   93,   94,   58,   59,   60,   38,   62,   63,   41,
   93,   94,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   -1,
   62,   63,  124,  125,   -1,   -1,   -1,   -1,   93,   94,
   -1,  124,  125,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   -1,   93,   94,   58,   59,   60,   -1,   62,   63,  124,
  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,   94,
   58,   59,   60,   -1,   62,   63,   93,   94,   -1,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,  270,
  271,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,   93,   94,  124,  125,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,
   59,   -1,   -1,   -1,   63,   -1,  124,  125,   -1,  123,
   -1,   -1,  264,  265,  266,  267,  268,  269,  270,  271,
   -1,  264,  265,  266,  267,  268,  269,  270,  271,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,  266,  267,  268,  269,  270,  271,   -1,   -1,   58,
   59,   -1,   -1,   -1,   63,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  266,  267,  268,  269,  270,  271,
   38,   -1,   -1,   41,   -1,   -1,   44,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   65,   -1,
   -1,  266,  267,  268,  269,  270,  271,   -1,   75,  266,
  267,  268,  269,  270,  271,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   96,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  266,  267,
  268,  269,  270,  271,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
   -1,   -1,  306,  307,  308,  309,   -1,   -1,  155,   -1,
   -1,   -1,  159,   -1,  161,   -1,  163,   -1,   -1,  268,
  269,  270,  271,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  182,  183,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,   -1,
   -1,  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  268,
  269,  270,  271,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  251,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,   -1,   -1,  306,  307,
  308,  309,
  };

#line 954 "CParser.jay"

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
  public const int TRUE = 304;
  public const int FALSE = 305;
  public const int STRUCT = 306;
  public const int CLASS = 307;
  public const int UNION = 308;
  public const int ENUM = 309;
  public const int ELLIPSIS = 310;
  public const int CASE = 311;
  public const int DEFAULT = 312;
  public const int IF = 313;
  public const int ELSE = 314;
  public const int SWITCH = 315;
  public const int WHILE = 316;
  public const int DO = 317;
  public const int FOR = 318;
  public const int GOTO = 319;
  public const int CONTINUE = 320;
  public const int BREAK = 321;
  public const int RETURN = 322;
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
