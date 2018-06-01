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
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER '{' struct_declaration_list '}'",
//t    "struct_or_union_specifier : struct_or_union '{' struct_declaration_list '}'",
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER",
//t    "struct_or_union : STRUCT",
//t    "struct_or_union : CLASS",
//t    "struct_or_union : UNION",
//t    "struct_declaration_list : struct_declaration",
//t    "struct_declaration_list : struct_declaration_list struct_declaration",
//t    "struct_declaration : specifier_qualifier_list struct_declarator_list ';'",
//t    "specifier_qualifier_list : type_specifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_specifier",
//t    "specifier_qualifier_list : type_qualifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_qualifier",
//t    "struct_declarator_list : struct_declarator",
//t    "struct_declarator_list : struct_declarator_list ',' struct_declarator",
//t    "struct_declarator : declarator",
//t    "struct_declarator : ':' constant_expression",
//t    "struct_declarator : declarator ':' constant_expression",
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
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-4+yyTop], (yyVals[-3+yyTop]).ToString()); }
  break;
case 112:
#line 452 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-3+yyTop], ""); }
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
case 138:
#line 508 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 139:
#line 515 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 140:
#line 516 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 141:
#line 521 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 142:
  case_142();
  break;
case 143:
#line 537 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 144:
#line 541 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 145:
#line 545 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 146:
#line 549 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 147:
#line 553 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 148:
#line 557 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 149:
#line 561 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 150:
#line 565 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 151:
  case_151();
  break;
case 152:
  case_152();
  break;
case 153:
  case_153();
  break;
case 154:
#line 593 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 155:
#line 594 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 156:
#line 595 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 157:
#line 596 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 158:
#line 600 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 159:
#line 604 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 160:
#line 608 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 161:
#line 609 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 162:
#line 610 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 163:
#line 617 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 164:
  case_164();
  break;
case 165:
  case_165();
  break;
case 166:
  case_166();
  break;
case 167:
  case_167();
  break;
case 168:
  case_168();
  break;
case 169:
  case_169();
  break;
case 170:
  case_170();
  break;
case 171:
  case_171();
  break;
case 177:
  case_177();
  break;
case 178:
#line 701 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 179:
#line 705 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 180:
#line 709 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 181:
#line 713 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 182:
#line 717 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 183:
#line 721 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
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
  case_187();
  break;
case 188:
#line 755 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 189:
#line 759 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 190:
#line 763 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 191:
  case_191();
  break;
case 192:
  case_192();
  break;
case 193:
  case_193();
  break;
case 194:
  case_194();
  break;
case 195:
#line 803 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 209:
#line 835 "CParser.jay"
  {
		yyVal = new Block (GetLocation(yyVals[-1+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 210:
#line 839 "CParser.jay"
  {
        yyVal = new Block (GetLocation(yyVals[-2+yyTop]), (List<Statement>)yyVals[-1+yyTop], GetLocation(yyVals[0+yyTop]));
	}
  break;
case 211:
#line 843 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 212:
#line 844 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Insert (0, (Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 215:
#line 856 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 216:
#line 860 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 217:
#line 867 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 218:
#line 871 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 220:
#line 879 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 221:
#line 883 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], (Statement)yyVals[-5+yyTop]);
	}
  break;
case 222:
#line 887 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop], null, GetLocation(yyVals[-5+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 223:
#line 891 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], null, GetLocation(yyVals[-6+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 224:
#line 895 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop]);
	}
  break;
case 225:
#line 899 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 229:
#line 909 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 230:
#line 913 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 231:
  case_231();
  break;
case 232:
  case_232();
  break;
case 235:
  case_235();
  break;
case 236:
  case_236();
  break;
case 237:
  case_237();
  break;
case 238:
  case_238();
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

void case_142()
#line 523 "CParser.jay"
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

void case_151()
#line 567 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDecl>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_152()
#line 574 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = new List<ParameterDecl>();
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDecl(n));
		}
		yyVal = d;
	}

void case_153()
#line 584 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDecl>();
		yyVal = d;
	}

void case_164()
#line 619 "CParser.jay"
{
		var l = (List<ParameterDecl>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_165()
#line 628 "CParser.jay"
{
		var l = new List<ParameterDecl>();
		l.Add((ParameterDecl)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_166()
#line 634 "CParser.jay"
{
		var l = (List<ParameterDecl>)yyVals[-2+yyTop];
		l.Add((ParameterDecl)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_167()
#line 643 "CParser.jay"
{
		var p = new ParameterDecl((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_168()
#line 648 "CParser.jay"
{
		var p = new ParameterDecl((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_169()
#line 653 "CParser.jay"
{
		var p = new ParameterDecl((DeclarationSpecifiers)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_170()
#line 661 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_171()
#line 667 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_177()
#line 687 "CParser.jay"
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

void case_184()
#line 723 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-1+yyTop];
		d.Parameters = new List<ParameterDecl>();
		yyVal = d;
	}

void case_185()
#line 730 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.Parameters = (List<ParameterDecl>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_186()
#line 736 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDecl> ();
		yyVal = d;
	}

void case_187()
#line 743 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDecl>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_191()
#line 768 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_192()
#line 775 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_193()
#line 783 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_194()
#line 790 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_231()
#line 918 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_232()
#line 923 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_235()
#line 936 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_236()
#line 945 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_237()
#line 956 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_238()
#line 962 "CParser.jay"
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
   34,   34,   34,   36,   36,   36,   37,   37,   38,   39,
   39,   39,   39,   40,   40,   41,   41,   41,   35,   35,
   35,   35,   35,   42,   42,   43,   43,   30,   32,   32,
   45,   45,   45,   45,   45,   45,   45,   45,   45,   45,
   45,   45,   45,   44,   44,   44,   44,   46,   46,   29,
   29,   29,   47,   47,   49,   49,   50,   50,   50,   48,
   48,    5,    5,   51,   51,   51,   52,   52,   52,   52,
   52,   52,   52,   52,   52,   52,   52,   33,   33,   33,
    6,    6,    6,    6,   53,   54,   54,   55,   55,   56,
   56,   56,   56,   56,   56,   57,   57,   57,   58,   58,
   63,   63,   64,   64,   59,   59,   60,   60,   60,   61,
   61,   61,   61,   61,   61,   62,   62,   62,   62,   62,
    0,    0,   65,   65,   66,   66,   67,   67,
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
    5,    4,    2,    1,    1,    1,    1,    2,    3,    2,
    1,    2,    1,    1,    3,    1,    2,    3,    4,    5,
    5,    6,    2,    1,    3,    1,    3,    1,    2,    1,
    1,    3,    5,    4,    4,    6,    6,    5,    4,    3,
    4,    4,    3,    1,    2,    2,    3,    1,    2,    1,
    1,    1,    1,    3,    1,    3,    2,    2,    1,    1,
    3,    1,    2,    1,    1,    2,    3,    2,    3,    3,
    4,    3,    4,    2,    3,    3,    4,    1,    3,    4,
    1,    2,    3,    4,    2,    1,    2,    3,    2,    1,
    1,    1,    1,    1,    1,    3,    4,    3,    2,    3,
    1,    2,    1,    1,    1,    2,    5,    7,    5,    5,
    7,    6,    7,    6,    7,    3,    2,    2,    2,    3,
    1,    2,    1,    1,    4,    3,    1,    2,
  };
   static readonly short [] yyDefRed = {            0,
  110,   91,   92,   93,   94,   95,  138,  161,   97,   98,
   99,  100,  103,  104,  101,  102,  160,  162,   96,  105,
  106,  107,  114,  115,  116,    0,    0,  234,    0,    0,
    0,    0,    0,  108,  109,    0,  231,  233,    0,    0,
  232,  141,    0,    0,   77,    0,   87,    0,    0,    0,
   80,   82,   84,   86,    0,    0,    0,    0,    0,  134,
    0,  158,  156,    0,    0,   78,    0,    0,  237,    0,
  236,    0,    0,    0,    0,    0,    0,    0,    0,  117,
    0,    0,    0,  129,    0,  142,  159,  157,   88,    0,
    0,    2,    3,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  209,    0,
    0,   25,   26,   27,   28,  215,    5,    0,    0,   74,
    0,   31,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   61,  213,  214,  200,  201,  202,  203,
  204,  205,    0,  211,    1,    0,  188,   90,  238,  235,
  170,  153,    0,    0,    0,    0,  165,    0,  150,    0,
    0,    0,    0,  120,  122,  112,  118,    0,    0,    0,
  124,  130,    0,   29,   76,  137,  131,  135,    0,    0,
   23,    0,   18,   19,    0,    0,    0,    0,    0,    0,
    0,    0,  227,  228,  229,    0,    0,    0,    0,   20,
   21,    0,  216,    0,   11,   12,    0,    0,    0,   64,
   65,   66,   67,   68,   69,   70,   71,   72,   73,   63,
    0,   22,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  210,  212,    0,    0,    0,  191,    0,    0,  196,
    0,    0,  167,    0,  168,    0,  151,  152,    0,    0,
    0,  149,  145,    0,  144,    0,    0,  111,  127,    0,
    0,  119,  132,  206,    0,    0,    0,  208,    0,    0,
    0,    0,    0,    0,  226,  230,    4,    0,    0,    0,
  173,   75,   10,    7,    0,   15,    0,    9,   62,   32,
   33,   34,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  199,
  189,    0,  192,  195,  197,  184,    0,    0,  178,    0,
    0,    0,    0,    0,  171,  164,  166,    0,    0,  148,
  143,  128,  125,    0,    0,  207,    0,    0,    0,    0,
    0,    0,    0,   30,    8,    0,    6,    0,  198,  190,
  193,    0,  185,  177,  182,  179,  186,    0,  180,    0,
    0,  146,  147,    0,  219,  220,    0,    0,    0,    0,
    0,    0,   16,   60,  194,  187,  183,  181,    0,    0,
  224,    0,  222,    0,   13,    0,  218,  221,  225,  223,
   14,
  };
  protected static readonly short [] yyDgoto  = {            27,
  117,  118,  119,  295,  198,  246,  120,  121,  122,  123,
  124,  125,  126,  127,  128,  129,  130,  131,  132,  133,
  134,  221,  176,   28,   70,   46,   30,   31,   32,   33,
   47,   61,  247,   34,   35,   36,   79,   80,   81,  170,
  171,   59,   60,   49,   50,   64,  327,  155,  156,  157,
  328,  256,  248,  249,  250,  136,  137,  138,  139,  140,
  141,  142,  143,  144,   37,   38,   72,
  };
  protected static readonly short [] yySindex = {         2544,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, -115, 2544,    0,   61, 2544,
 2544, 2544, 2544,    0,    0,  -88,    0,    0,  -96, -212,
    0,    0,   47,  -22,    0,    6,    0, 1046,  -25,   52,
    0,    0,    0,    0,  -56, 2570, -212,   46,    4,    0,
  114,    0,    0,  -22,   47,    0,   53, 2085,    0,   61,
    0, 2405,   52,  906, 1028, 2570, 2570, 2570, 2465,    0,
  -17,   15, 2352,    0,  -82,    0,    0,    0,    0,  105,
  129,    0,    0, 2369, 2419, 2419, 2352,  145,  143,  172,
  178,  486,  187,  -28,  190,  192, 2117,  414,    0, 2352,
 2352,    0,    0,    0,    0,    0,    0,  141,   59,    0,
  110,    0, 2352,  173,  168,   67,    2,   18,  199,  159,
  133,   -8,  -41,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  118,    0,    0, 2074,    0,    0,    0,    0,
    0,    0,   68,  223,  229,  228,    0,   25,    0, 1527,
  214, 1122, 2491,    0,    0,    0,    0, 2352,  259,  147,
    0,    0,  -76,    0,    0,    0,    0,    0,  486,  414,
    0,  414,    0,    0,  261,  486, 2352, 2352, 2352,   19,
  528,  268,    0,    0,    0,  148,  254,  324,   37,    0,
    0, 2352,    0,  117,    0,    0, 2161, 2352,  122,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2352,    0, 2352, 2352, 2352, 2352, 2352, 2352, 2352, 2352,
 2352, 2352, 2352, 2352, 2352, 2352, 2352, 2352, 2352, 2352,
 2352,    0,    0, 2352,  182,   16,    0, 2085,  111,    0,
  579, 2172,    0,  -35,    0,   83,    0,    0,  198, 2517,
  -24,    0,    0, 2352,    0, 2180,  333,    0,    0, 2352,
  -17,    0,    0,    0,  389,  403,  486,    0,  285,  355,
  407,  420, 2188, 2188,    0,    0,    0, 2212,  718,   95,
    0,    0,    0,    0,  409,    0,  108,    0,    0,    0,
    0,    0,  173,  173,  168,  168,   67,   67,   67,   67,
    2,    2,   18,  199,  159,  133,   -8,  161,  356,    0,
    0, 1137,    0,    0,    0,    0,  421,  423,    0, 2226,
  398,   83,  748, 2234,    0,    0,    0,  425,  427,    0,
    0,    0,    0,  387,  387,    0,  486,  486,  486, 2352,
 2252, 2267, 2074,    0,    0, 2352,    0, 2352,    0,    0,
    0, 2085,    0,    0,    0,    0,    0,  475,    0, 2323,
  428,    0,    0,  213,    0,    0,  417,  486,  473,  486,
  492,   24,    0,    0,    0,    0,    0,    0,  486,  468,
    0,  486,    0,  486,    0, 2063,    0,    0,    0,    0,
    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  471,
  535,  857, 1018,    0,    0,    0,    0,    0,  639,    0,
    0,    0,    0,  104,    0,    0,    0,  150,    0,  796,
    0,    0,    0,    0,  692,    0,    0,   29,    0,    0,
    0,    0,    0,  206,    0,    0,    0,    0,    0,    0,
    0,    0,  846,    0,    0,    0,   42, 1214,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  150,
 1179,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1371,    0,
 1462,    0,    0, 1470, 1620, 1777,  506, 1912, 1563,   22,
   13,   11, 1186,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  497,    0,    0,  491,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  155,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  507,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  498,    0,  502,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  508,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 1499, 1538, 1717, 1754, 1805, 1814, 1865, 1873,
 1902, 1910, 1970, 2475,  493, 2459,  570,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  516,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 1425,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  183,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -71,    0,    0, -136,  200,  -68,  412,  -99,    0,
   75,  169,  162,  270,  322,  326,  321,  328,  334,    0,
  -80,    0,  -46, 1109,    1,    0,    0, 1298, 2479,    0,
  513,  -23,  -66,    0,    0,    0,  505,  -50,  477,    0,
  311,  526,  -59,  -34,   -9,  -37,  -70,    0,    0,  329,
    9,  -86, -269,    0,  335,  -89,    0,  125, -152,    0,
    0,    0,    0,  445,  563,    0,    0,
  };
  protected static readonly short [] yyTable = {           147,
   29,  148,  175,  154,  251,   48,  161,   40,  115,   63,
  200,  201,  190,  110,   43,  108,  175,  111,  112,   44,
  113,  241,   43,  222,   44,  178,   57,   29,  167,   88,
   51,   52,   53,   54,   56,  196,  197,  162,  284,   73,
  168,   90,  177,  275,   58,  276,   90,   85,  273,   65,
  185,   57,  362,   55,   57,  252,   55,  169,  173,  322,
  201,  232,   53,  233,   66,   53,   76,  396,   57,   57,
   55,   55,  136,   57,  153,   55,  289,  147,   44,   53,
   53,  121,  121,  121,   53,  115,   43,  175,   44,  274,
  110,   74,  108,  267,  111,  112,  278,  113,  207,  121,
   43,  114,   44,   57,  209,   55,   83,  251,  197,   44,
  197,  116,  167,  178,   53,  279,  280,  281,  254,   45,
  261,  269,  333,  300,  301,  302,  362,  252,   84,  253,
  351,  352,  121,  292,  289,   57,  297,   55,  296,  172,
  321,   39,   75,  154,  154,   53,   53,  154,  395,  208,
  115,  202,  299,  136,   86,  110,  245,  108,  252,  111,
  112,  255,  113,  175,  290,   68,  201,  332,   55,  318,
  220,  324,   71,  334,   58,   67,  116,  109,  114,  147,
   58,  323,  187,  331,  202,  252,  179,  346,  354,  175,
  271,  202,  338,   89,  154,  339,  150,  319,  126,  203,
  357,  244,  186,  332,  202,  272,  286,  291,   89,  225,
  226,  188,  227,  126,  223,  217,  254,  189,  358,  224,
  217,   42,  217,  342,  217,  217,  191,  217,  192,  240,
  201,   42,  145,   92,   93,   94,  236,   95,   96,   42,
   67,  217,  242,  114,   73,  155,  155,  169,  193,  155,
  194,  153,  237,  147,  290,  361,  238,  374,  375,  376,
  153,  239,  368,  257,    8,  371,    8,  230,  231,  258,
  201,  260,  259,   17,   18,   17,   18,  384,  377,  379,
  381,   57,   55,   55,  147,  234,  235,  383,  391,  153,
  393,   53,   53,  147,  287,  385,  155,  202,  121,  397,
  303,  304,  399,   42,  400,  217,  263,  217,  217,   91,
   92,   93,   94,    8,   95,   96,  270,   42,  277,  204,
  205,  206,   17,   18,   42,  347,  285,  147,  202,  361,
  228,  229,  282,  153,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,   26,
  154,   97,   98,   99,  288,  100,  101,  102,  103,  104,
  105,  106,  107,  293,   91,   92,   93,   94,  298,   95,
   96,  210,  211,  212,  213,  214,  215,  216,  217,  218,
  219,  307,  308,  309,  310,  348,  305,  306,  202,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,   26,  341,   97,   98,   99,  344,
  100,  101,  102,  103,  104,  105,  106,  107,  320,  217,
  217,  217,  217,  345,  217,  217,  115,  349,  359,  355,
  202,  110,  356,  108,  335,  111,  112,  390,  113,  350,
  202,  363,  155,  364,  217,  217,  217,  217,  217,  217,
  217,  217,  217,  217,  217,  217,  217,  217,  217,  217,
  217,  217,  217,  217,  217,  217,  217,  217,  217,  217,
  366,  217,  217,  217,  174,  217,  217,  217,  217,  217,
  217,  217,  217,  311,  312,  181,  183,  184,  174,  353,
   79,   79,   79,  392,   79,  386,  202,  372,  115,  373,
  388,  174,  174,  110,  389,  108,  398,  111,  112,   79,
  113,  163,  394,   54,  174,  202,   54,  169,  174,  114,
  169,  174,  175,   46,  116,  175,   46,  172,  174,   46,
   54,   54,  382,  164,  165,   54,  176,  313,  315,  176,
  115,   79,  314,   46,   46,  110,  316,  108,   46,  111,
  112,  174,  113,  317,   81,   81,   81,   89,   81,  174,
  163,  343,   82,  325,  199,   54,  116,  243,  337,   41,
    0,    0,    0,   81,    0,    0,    0,    0,   46,   46,
    0,    0,    0,    0,    0,    0,    0,    0,   67,    0,
   58,  114,    0,   58,    0,    0,   54,   54,  251,  326,
   44,    0,    0,    0,    0,   81,    0,   58,   58,   46,
   46,    0,   58,    0,  174,  174,  174,  174,  174,  174,
  174,  174,  174,  174,  174,  174,  174,  174,  174,  174,
  174,  174,    0,  114,    0,  174,  199,    0,  199,    0,
    0,    0,   58,    0,    0,    0,    0,    0,    0,  252,
  145,   92,   93,   94,    0,   95,   96,  174,  133,  133,
  133,  174,  133,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   58,    1,  133,  133,    0,  174,
    0,    0,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
   26,    0,    0,    0,    0,    0,    0,   79,    0,  133,
    0,  113,  113,  113,    0,  113,    0,    0,    0,    0,
    0,  174,   91,   92,   93,   94,    0,   95,   96,  113,
  113,    0,    0,    0,    0,    0,    0,  289,  326,   44,
    0,    0,   54,   54,    0,    0,    0,    0,    0,  174,
    0,    0,    0,   46,   46,   46,   46,    0,    0,    0,
    0,  174,  113,    0,  145,   92,   93,   94,  367,   95,
   96,   81,    0,    0,   97,   98,   99,    0,  100,  101,
  102,  103,  104,  105,  106,  107,    0,    0,  252,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,   26,   42,  140,    0,    0,  140,
   58,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  140,  140,    0,  140,    0,    0,    0,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,   24,   25,   26,  139,    0,    0,  139,
    0,    0,    0,    0,    0,  133,   83,   83,   83,    0,
   83,    0,    0,  139,  139,    0,  139,    0,    0,    0,
    0,    0,    0,    0,    0,   83,    0,    0,  140,    0,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  152,   83,  113,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  139,    0,
    0,    0,    0,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,   26,    0,    0,    0,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,   26,    0,    0,   85,   85,   85,
  115,   85,    0,    0,    0,  110,    0,  108,    0,  160,
  112,    0,  113,    0,    0,    0,   85,  140,  140,  140,
  140,  140,  140,  140,  140,  140,  140,  140,  140,  140,
  140,  140,  140,  140,  140,  140,  140,  140,  140,  140,
  140,  140,  140,    0,    0,    0,   68,    0,   85,    0,
    0,    0,    0,   83,    0,    0,    0,    0,    0,    0,
  159,    0,    0,    0,    0,    0,    0,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  139,  114,  115,    0,   69,    0,    0,  110,
    0,  108,  151,  266,  112,    0,  113,    0,   67,  115,
    0,    0,    0,    0,  110,  135,  108,    0,  111,  112,
  149,  113,  245,    0,    0,    0,    0,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   24,   25,   26,    0,  265,    1,    1,    0,    1,    0,
    1,    1,    1,    1,    1,    1,   59,  244,    0,   59,
    0,    0,    0,    0,    0,    0,    0,    1,    1,    1,
    1,    1,    0,   59,   59,    0,    0,  114,    0,    0,
    0,  135,    0,  123,  123,  123,    0,    0,    0,  146,
    0,  360,  114,    0,    0,    0,    0,    0,    0,    1,
    0,  123,    1,    0,   85,    0,    0,    0,   59,    0,
    0,    0,    0,    0,  145,   92,   93,   94,    0,   95,
   96,    0,    0,    0,    0,    0,    0,    0,    0,  283,
    0,    0,    1,    0,  123,    0,    0,    0,    0,    0,
   59,    0,  158,    0,    0,    0,    8,    0,    0,    0,
    0,    0,    0,    0,    0,   17,   18,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   24,   25,   26,   77,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   77,   77,   77,   77,    0,  145,   92,
   93,   94,    0,   95,   96,    0,    0,    0,    0,    0,
    0,    0,    0,  145,   92,   93,   94,    0,   95,   96,
    0,    0,    0,    0,    0,   77,  264,   17,   17,    0,
    8,   17,   17,   17,   17,   17,    0,   17,    0,   17,
   18,    0,    0,    0,    0,    0,    0,    0,   17,   17,
   17,   17,   17,   17,    0,    0,    0,    0,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
   77,   24,   24,   17,   17,   24,   24,   24,   24,   24,
  123,   24,    0,    0,    0,    0,    0,   77,    0,   77,
    0,    0,   24,   24,   24,   24,   24,   24,    0,    0,
    0,    0,    0,    0,   17,   17,    0,    0,   29,   29,
    0,    0,   29,   29,   29,   29,   29,   35,   29,    0,
   35,    0,   35,   35,   35,    0,    0,   24,   24,   29,
   29,   29,    0,   29,   29,    0,    0,   35,   35,   35,
    0,   35,   35,    0,    0,    0,   36,    0,    0,   36,
    0,   36,   36,   36,    0,    0,    0,    0,   24,   24,
    0,    0,    0,    0,   29,   29,   36,   36,   36,  115,
   36,   36,   35,   35,  110,    0,  108,    0,  111,  112,
    0,  113,    0,    0,    0,   37,    0,    0,   37,    0,
   37,   37,   37,    0,    0,   29,   29,    0,    0,    0,
    0,   36,   36,   35,   35,   37,   37,   37,    0,   37,
   37,    0,    0,   51,    0,    0,   51,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  262,
   51,   51,   36,   36,    0,   51,    0,    0,    0,    0,
   37,   37,    0,    0,   17,   17,   17,   17,   17,   17,
   17,   17,   17,   17,   17,   17,   17,   17,   17,   17,
   17,   17,  114,    0,    0,   51,   51,   38,    0,    0,
   38,   37,   37,   38,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   38,   38,   38,
    0,   38,   38,    0,    0,    0,   51,   51,   24,   24,
   24,   24,   24,   24,   24,   24,   24,   24,   24,   24,
   24,   24,   24,   24,   24,   24,    0,    0,    0,    0,
    0,    0,   38,   38,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   29,   29,   29,   29,   29,
   29,   29,   29,   35,   35,   35,   35,   35,   35,   35,
   35,    0,    0,   38,   38,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   39,    0,    0,   39,    0,    0,
   39,    0,   36,   36,   36,   36,   36,   36,   36,   36,
    0,    0,    0,    0,   39,   39,   39,    0,   39,   39,
    0,    0,    0,  145,   92,   93,   94,    0,   95,   96,
    0,   40,    0,    0,   40,    0,    0,   40,    0,    0,
    0,   37,   37,   37,   37,   37,   37,   37,   37,   39,
   39,   40,   40,   40,   41,   40,   40,   41,    0,    0,
   41,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   51,   51,   41,   41,   41,    0,   41,   41,
   39,   39,   44,    0,    0,   44,   40,   40,   44,    0,
    0,   45,    0,    0,   45,    0,    0,   45,    0,    0,
    0,    0,   44,   44,   44,    0,   44,   44,    0,   41,
   41,   45,   45,   45,    0,   45,   45,   40,   40,    0,
    0,    0,    0,   38,   38,   38,   38,   38,   38,   38,
   38,    0,    0,    0,    0,    0,    0,   44,   44,    0,
   41,   41,   42,    0,    0,   42,   45,   45,   42,    0,
   43,    0,    0,   43,    0,    0,   43,    0,    0,    0,
    0,    0,   42,   42,   42,    0,   42,   42,   44,   44,
   43,   43,   43,    0,   43,   43,    0,   45,   45,   47,
    0,    0,   47,    0,    0,   47,    0,   48,    0,   49,
   48,    0,   49,   48,    0,   49,    0,   42,   42,   47,
   47,    0,    0,    0,   47,   43,   43,   48,   48,   49,
   49,    0,   48,    0,   49,    0,    0,    0,    0,    0,
   39,   39,   39,   39,   39,   39,   39,   39,   42,   42,
    0,    0,    0,    0,   47,   47,   43,   43,    0,    0,
    0,    0,   48,   48,   49,   49,    0,   50,    0,    0,
   50,    0,    0,   50,    0,    0,    0,   40,   40,   40,
   40,   40,   40,   40,   40,   47,   47,   50,   50,    0,
    0,    0,   50,   48,   48,   49,   49,    0,    0,    0,
    0,    0,   41,   41,   41,   41,   41,   41,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   50,   50,    0,    0,    0,    0,    0,    0,
   44,   44,   44,   44,   44,   44,    0,    0,    0,   45,
   45,   45,   45,   45,   45,    0,    0,    0,    0,    0,
    0,    0,    0,   50,   50,  115,    0,    0,    0,    0,
  110,    0,  108,    0,  111,  112,  115,  113,  245,    0,
    0,  110,    0,  108,    0,  111,  112,  115,  113,  245,
    0,    0,  110,    0,  108,    0,  111,  112,    0,  113,
   42,   42,   42,   42,   42,   42,    0,    0,   43,   43,
   43,   43,   43,   43,    0,    0,    0,    0,    0,  115,
    0,    0,    0,  244,  110,    0,  108,    0,  111,  112,
    0,  113,    0,    0,  244,    0,    0,    0,    0,   47,
   47,   47,   47,    0,    0,  195,    0,   48,   48,   48,
   48,   49,   49,    0,    0,  146,    0,  401,  114,    0,
    0,    0,    0,  115,    0,    0,  146,    0,  110,  114,
  108,  294,  111,  112,  115,  113,    0,  146,    0,  110,
  114,  108,  115,  330,  112,    0,  113,  110,    0,  108,
  115,  111,  112,    0,  113,  110,    0,  108,    0,  111,
  112,    0,  113,    0,    0,    0,    0,    0,    0,   50,
   50,    0,  114,    0,  115,    0,  116,    0,    0,  110,
    0,  108,    0,  111,  112,    0,  113,    0,  115,    0,
    0,    0,    0,  110,  329,  108,  115,  111,  112,    0,
  113,  110,  340,  108,    0,  370,  112,    0,  113,    0,
    0,    0,    0,    0,  115,    0,  114,    0,    0,  110,
    0,  108,  378,  111,  112,    0,  113,  114,    0,  115,
    0,    0,    0,    0,  110,  114,  108,  380,  111,  112,
    0,  113,    0,  114,    0,    0,    0,    0,  365,  145,
   92,   93,   94,    0,   95,   96,  369,    0,    0,    0,
  145,   92,   93,   94,  353,   95,   96,  114,    0,    0,
    0,  145,   92,   93,   94,    0,   95,   96,    0,    0,
    0,  114,    0,    0,    0,  115,    0,    0,    0,  114,
  110,    0,  108,    0,  111,  112,    0,  113,    0,    0,
    0,    0,    0,  145,   92,   93,   94,  114,   95,   96,
    0,    0,    0,    0,  115,    0,    0,    0,    0,  110,
    0,  108,  114,  111,  112,    0,  113,    0,    0,    0,
    0,  115,    0,    0,    0,    0,  110,    0,  180,    0,
  111,  112,    0,  113,    0,  387,    0,  145,   92,   93,
   94,    0,   95,   96,    0,    0,    0,    0,  145,   92,
   93,   94,    0,   95,   96,    0,  145,   92,   93,   94,
    0,   95,   96,    0,  145,   92,   93,   94,  114,   95,
   96,  115,    0,    0,    0,    0,  110,    0,  182,    0,
  111,  112,    0,  113,    0,    0,    0,    0,  145,   92,
   93,   94,    0,   95,   96,    0,    0,  114,    0,    0,
    0,    0,  145,   92,   93,   94,    0,   95,   96,    0,
  145,   92,   93,   94,  114,   95,   96,    0,    0,   56,
    0,    0,   56,    0,    0,    0,    0,    0,  145,   92,
   93,   94,    0,   95,   96,   52,   56,   56,   52,    0,
    0,   56,   62,  145,   92,   93,   94,   67,   95,   96,
    0,    0,   52,   52,   78,    0,    0,   52,    0,    0,
    0,    0,   87,    0,  114,    0,    0,    0,    0,    0,
    0,   56,    0,   62,   78,   78,   78,   78,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   52,   52,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  145,
   92,   93,   94,   56,   95,   96,   78,    0,    0,  166,
    0,    0,    0,    0,    0,    0,    0,    0,   52,   52,
    0,    0,    0,    0,    0,    0,    0,    0,  145,   92,
   93,   94,    0,   95,   96,  268,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  145,   92,   93,   94,    0,
   95,   96,    0,    0,    0,    0,   62,    0,    0,    0,
   87,   78,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   78,    0,
   78,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  145,   92,   93,   94,    0,
   95,   96,    0,    0,    0,    0,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,
   25,   26,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   56,   56,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   87,
    0,    0,    0,    0,   52,   52,    1,    0,    0,    0,
    0,    0,    0,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,
   25,   26,    1,    0,    0,    0,    0,    0,    0,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,   24,   25,   26,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,   24,   25,   26,  336,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
   26,    1,    0,    0,    0,    0,    0,    0,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,   24,   25,   26,
  };
  protected static readonly short [] yyCheck = {            68,
    0,   68,   83,   74,   40,   29,   75,  123,   33,   44,
  110,  111,  102,   38,   40,   40,   97,   42,   43,   42,
   45,   63,   40,  123,   42,   85,  123,   27,   79,   64,
   30,   31,   32,   33,  123,  107,  108,   75,  191,   49,
   58,   65,  125,  180,  257,  182,   70,   44,  125,   44,
   97,   41,  322,   41,   44,   91,   44,   81,   44,   44,
  160,   60,   41,   62,   59,   44,  123,   44,   58,   59,
   58,   59,   44,   63,   74,   63,   40,  146,   42,   58,
   59,   40,   41,   42,   63,   33,   40,  168,   42,  179,
   38,   40,   40,  162,   42,   43,  186,   45,   40,   58,
   40,  126,   42,   93,   46,   93,   61,   40,  180,   42,
  182,   59,  163,  173,   93,  187,  188,  189,  153,   59,
  158,  168,   40,  223,  224,  225,  396,   91,  125,  153,
  283,  284,   91,  202,   40,  125,  208,  125,  207,  125,
  125,  257,   91,   40,   41,  124,  125,   44,  125,   91,
   33,   44,  221,  125,   41,   38,   46,   40,   91,   42,
   43,  153,   45,  244,  199,   61,  266,  254,  257,  241,
   61,   61,   48,   91,  257,  123,   59,  125,  126,  248,
  257,  248,   40,  252,   44,   91,   58,  277,  288,  270,
   44,   44,  261,   44,   91,  264,   72,  244,   44,   59,
   93,   91,   58,  290,   44,   59,   59,  199,   59,   37,
   43,   40,   45,   59,   42,   33,  251,   40,   58,   47,
   38,  257,   40,  270,   42,   43,   40,   45,  257,  271,
  330,  257,  257,  258,  259,  260,   38,  262,  263,  257,
  123,   59,  125,  126,  254,   40,   41,  271,   59,   44,
   59,  251,   94,  322,  289,  322,  124,  347,  348,  349,
  260,  270,  333,   41,  289,  334,  289,  266,  267,   41,
  370,   44,   44,  298,  299,  298,  299,  358,  350,  351,
  352,  271,  270,  271,  353,  268,  269,  356,  378,  289,
  380,  270,  271,  362,   41,  362,   91,   44,  257,  389,
  226,  227,  392,  257,  394,  123,   93,  125,  126,  257,
  258,  259,  260,  289,  262,  263,   58,  257,   58,  261,
  262,  263,  298,  299,  257,   41,   59,  396,   44,  396,
  264,  265,  314,  333,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  257,  309,  310,  311,   41,  313,  314,  315,  316,  317,
  318,  319,  320,  257,  257,  258,  259,  260,  257,  262,
  263,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  230,  231,  232,  233,   41,  228,  229,   44,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,   93,  309,  310,  311,   41,
  313,  314,  315,  316,  317,  318,  319,  320,  257,  257,
  258,  259,  260,   41,  262,  263,   33,   41,   93,   41,
   44,   38,   44,   40,  257,   42,   43,   41,   45,   40,
   44,   41,  257,   41,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
   93,  309,  310,  311,   83,  313,  314,  315,  316,  317,
  318,  319,  320,  234,  235,   94,   95,   96,   97,  123,
   40,   41,   42,   41,   44,   41,   44,   93,   33,   93,
   93,  110,  111,   38,  312,   40,   59,   42,   43,   59,
   45,   41,   41,   41,  123,   44,   44,   41,   41,  126,
   44,   44,   41,   38,   59,   44,   41,   41,   41,   44,
   58,   59,  353,   77,   78,   63,   41,  236,  238,   44,
   33,   91,  237,   58,   59,   38,  239,   40,   63,   42,
   43,  160,   45,  240,   40,   41,   42,   65,   44,  168,
   76,  271,   57,  249,  108,   93,   59,  143,  260,   27,
   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   93,   94,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,
   41,  126,   -1,   44,   -1,   -1,  124,  125,   40,   41,
   42,   -1,   -1,   -1,   -1,   91,   -1,   58,   59,  124,
  125,   -1,   63,   -1,  223,  224,  225,  226,  227,  228,
  229,  230,  231,  232,  233,  234,  235,  236,  237,  238,
  239,  240,   -1,  126,   -1,  244,  180,   -1,  182,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,   91,
  257,  258,  259,  260,   -1,  262,  263,  266,   40,   41,
   42,  270,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  125,  282,   58,   59,   -1,  288,
   -1,   -1,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   91,
   -1,   40,   41,   42,   -1,   44,   -1,   -1,   -1,   -1,
   -1,  330,  257,  258,  259,  260,   -1,  262,  263,   58,
   59,   -1,   -1,   -1,   -1,   -1,   -1,   40,   41,   42,
   -1,   -1,  270,  271,   -1,   -1,   -1,   -1,   -1,  358,
   -1,   -1,   -1,  268,  269,  270,  271,   -1,   -1,   -1,
   -1,  370,   91,   -1,  257,  258,  259,  260,   41,  262,
  263,  257,   -1,   -1,  309,  310,  311,   -1,  313,  314,
  315,  316,  317,  318,  319,  320,   -1,   -1,   91,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  257,   41,   -1,   -1,   44,
  271,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   58,   59,   -1,   61,   -1,   -1,   -1,
  282,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,   41,   -1,   -1,   44,
   -1,   -1,   -1,   -1,   -1,  257,   40,   41,   42,   -1,
   44,   -1,   -1,   58,   59,   -1,   61,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   -1,   -1,  123,   -1,
  282,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,   41,   91,  257,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,
   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,   -1,   -1,   -1,   -1,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,   -1,   -1,   40,   41,   42,
   33,   44,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   59,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,   -1,   -1,   -1,   61,   -1,   91,   -1,
   -1,   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  126,   33,   -1,   48,   -1,   -1,   38,
   -1,   40,  257,   42,   43,   -1,   45,   -1,  123,   33,
   -1,   -1,   -1,   -1,   38,   67,   40,   -1,   42,   43,
   72,   45,   46,   -1,   -1,   -1,   -1,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,   -1,   93,   37,   38,   -1,   40,   -1,
   42,   43,   44,   45,   46,   47,   41,   91,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   63,   -1,   58,   59,   -1,   -1,  126,   -1,   -1,
   -1,  143,   -1,   40,   41,   42,   -1,   -1,   -1,  123,
   -1,  125,  126,   -1,   -1,   -1,   -1,   -1,   -1,   91,
   -1,   58,   94,   -1,  257,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  191,
   -1,   -1,  124,   -1,   91,   -1,   -1,   -1,   -1,   -1,
  125,   -1,  285,   -1,   -1,   -1,  289,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  298,  299,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,   56,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   76,   77,   78,   79,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,   -1,  108,  285,   37,   38,   -1,
  289,   41,   42,   43,   44,   45,   -1,   47,   -1,  298,
  299,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,
   60,   61,   62,   63,   -1,   -1,   -1,   -1,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,  270,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  163,   37,   38,   93,   94,   41,   42,   43,   44,   45,
  257,   47,   -1,   -1,   -1,   -1,   -1,  180,   -1,  182,
   -1,   -1,   58,   59,   60,   61,   62,   63,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   37,   38,
   -1,   -1,   41,   42,   43,   44,   45,   38,   47,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   93,   94,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   38,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   93,   94,   58,   59,   60,   33,
   62,   63,   93,   94,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   93,   94,  124,  125,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   58,   59,  124,  125,   -1,   63,   -1,   -1,   -1,   -1,
   93,   94,   -1,   -1,  264,  265,  266,  267,  268,  269,
  270,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  126,   -1,   -1,   93,   94,   38,   -1,   -1,
   41,  124,  125,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,  124,  125,  264,  265,
  266,  267,  268,  269,  270,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,
  269,  270,  271,  264,  265,  266,  267,  268,  269,  270,
  271,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,  264,  265,  266,  267,  268,  269,  270,  271,
   -1,   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,  270,  271,   93,
   94,   58,   59,   60,   38,   62,   63,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,   58,   59,   60,   -1,   62,   63,
  124,  125,   38,   -1,   -1,   41,   93,   94,   44,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,   93,
   94,   58,   59,   60,   -1,   62,   63,  124,  125,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,  270,
  271,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,
  124,  125,   38,   -1,   -1,   41,   93,   94,   44,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   58,   59,   60,   -1,   62,   63,  124,  125,
   58,   59,   60,   -1,   62,   63,   -1,  124,  125,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   38,   -1,   38,
   41,   -1,   41,   44,   -1,   44,   -1,   93,   94,   58,
   59,   -1,   -1,   -1,   63,   93,   94,   58,   59,   58,
   59,   -1,   63,   -1,   63,   -1,   -1,   -1,   -1,   -1,
  264,  265,  266,  267,  268,  269,  270,  271,  124,  125,
   -1,   -1,   -1,   -1,   93,   94,  124,  125,   -1,   -1,
   -1,   -1,   93,   94,   93,   94,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,  270,  271,  124,  125,   58,   59,   -1,
   -1,   -1,   63,  124,  125,  124,  125,   -1,   -1,   -1,
   -1,   -1,  266,  267,  268,  269,  270,  271,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,
  266,  267,  268,  269,  270,  271,   -1,   -1,   -1,  266,
  267,  268,  269,  270,  271,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   33,   45,   46,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   33,   45,   46,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
  266,  267,  268,  269,  270,  271,   -1,   -1,  266,  267,
  268,  269,  270,  271,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   91,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   -1,   -1,   91,   -1,   -1,   -1,   -1,  268,
  269,  270,  271,   -1,   -1,   59,   -1,  268,  269,  270,
  271,  270,  271,   -1,   -1,  123,   -1,  125,  126,   -1,
   -1,   -1,   -1,   33,   -1,   -1,  123,   -1,   38,  126,
   40,   41,   42,   43,   33,   45,   -1,  123,   -1,   38,
  126,   40,   33,   42,   43,   -1,   45,   38,   -1,   40,
   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,   -1,  126,   -1,   33,   -1,   59,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   33,   -1,
   -1,   -1,   -1,   38,   93,   40,   33,   42,   43,   -1,
   45,   38,   93,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   -1,   33,   -1,  126,   -1,   -1,   38,
   -1,   40,   41,   42,   43,   -1,   45,  126,   -1,   33,
   -1,   -1,   -1,   -1,   38,  126,   40,   41,   42,   43,
   -1,   45,   -1,  126,   -1,   -1,   -1,   -1,   93,  257,
  258,  259,  260,   -1,  262,  263,   93,   -1,   -1,   -1,
  257,  258,  259,  260,  123,  262,  263,  126,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,  126,   -1,   -1,   -1,   33,   -1,   -1,   -1,  126,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,  126,  262,  263,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,  126,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   93,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,  257,  258,  259,  260,  126,  262,
  263,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,  126,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  257,  258,  259,  260,  126,  262,  263,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   41,   58,   59,   44,   -1,
   -1,   63,   44,  257,  258,  259,  260,  123,  262,  263,
   -1,   -1,   58,   59,   56,   -1,   -1,   63,   -1,   -1,
   -1,   -1,   64,   -1,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,   75,   76,   77,   78,   79,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,  125,  262,  263,  108,   -1,   -1,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,  158,   -1,   -1,   -1,
  162,  163,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  180,   -1,
  182,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  261,
   -1,   -1,   -1,   -1,  270,  271,  282,   -1,   -1,   -1,
   -1,   -1,   -1,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  282,   -1,   -1,   -1,   -1,   -1,   -1,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  282,   -1,   -1,   -1,   -1,   -1,   -1,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,
  };

#line 972 "CParser.jay"

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
