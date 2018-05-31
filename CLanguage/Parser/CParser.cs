// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "CParser.jay"
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using CLanguage.Ast;
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
//t    "$$1 :",
//t    "compound_statement : '{' $$1 '}'",
//t    "$$2 :",
//t    "compound_statement : '{' $$2 block_item_list '}'",
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
//t    "$$3 :",
//t    "iteration_statement : FOR '(' declaration expression_statement ')' $$3 statement",
//t    "$$4 :",
//t    "iteration_statement : FOR '(' declaration expression_statement expression ')' $$4 statement",
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
#line 832 "CParser.jay"
  { StartBlock(GetLocation(yyVals[0+yyTop])); }
  break;
case 210:
#line 835 "CParser.jay"
  {
		yyVal = EndBlock(GetLocation(yyVals[0+yyTop]));
	}
  break;
case 211:
#line 836 "CParser.jay"
  { StartBlock(GetLocation(yyVals[0+yyTop])); }
  break;
case 212:
#line 839 "CParser.jay"
  {
		yyVal = EndBlock(GetLocation(yyVals[0+yyTop]));
	}
  break;
case 215:
  case_215();
  break;
case 216:
  case_216();
  break;
case 217:
#line 865 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 218:
#line 869 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 219:
#line 876 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 220:
#line 880 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 222:
#line 888 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 223:
#line 892 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], (Statement)yyVals[-5+yyTop]);
	}
  break;
case 224:
#line 896 "CParser.jay"
  {
		yyVal = new ForStatement((ExpressionStatement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop], _currentBlock, GetLocation(yyVals[-5+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 225:
#line 900 "CParser.jay"
  {
		yyVal = new ForStatement((ExpressionStatement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], _currentBlock, GetLocation(yyVals[-6+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 226:
#line 901 "CParser.jay"
  { StartBlock(GetLocation(yyVals[-4+yyTop])); }
  break;
case 227:
  case_227();
  break;
case 228:
#line 907 "CParser.jay"
  { StartBlock(GetLocation(yyVals[-5+yyTop])); }
  break;
case 229:
  case_229();
  break;
case 233:
#line 922 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 234:
#line 926 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 235:
  case_235();
  break;
case 236:
  case_236();
  break;
case 239:
  case_239();
  break;
case 240:
  case_240();
  break;
case 241:
  case_241();
  break;
case 242:
  case_242();
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
		var d = new MultiDeclaration();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_78()
#line 334 "CParser.jay"
{
		var d = new MultiDeclaration();
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

void case_215()
#line 849 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop], _currentBlock);
		yyVal = null;
	}

void case_216()
#line 854 "CParser.jay"
{
		if (yyVal != null) {
			_currentBlock.AddStatement((Statement)yyVals[0+yyTop]);
		}
	}

void case_227()
#line 902 "CParser.jay"
{
		AddDeclaration(yyVals[-4+yyTop], _currentBlock);
		var b = EndBlock(GetLocation(yyVals[0+yyTop]));		
		yyVal = new ForStatement(b, ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Statement)yyVals[0+yyTop]);
	}

void case_229()
#line 908 "CParser.jay"
{
		AddDeclaration(yyVals[-5+yyTop], _currentBlock);
		var b = EndBlock(GetLocation(yyVals[-1+yyTop]));		
		yyVal = new ForStatement(b, ((ExpressionStatement)yyVals[-4+yyTop]).Expression, (Expression)yyVals[-3+yyTop], (Statement)yyVals[0+yyTop]);
	}

void case_235()
#line 931 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_236()
#line 936 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_239()
#line 949 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_240()
#line 958 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_241()
#line 969 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_242()
#line 975 "CParser.jay"
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
   56,   56,   56,   56,   56,   57,   57,   57,   63,   58,
   65,   58,   64,   64,   66,   66,   59,   59,   60,   60,
   60,   61,   61,   61,   61,   67,   61,   68,   61,   62,
   62,   62,   62,   62,    0,    0,   69,   69,   70,   70,
   71,   71,
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
    1,    1,    1,    1,    1,    3,    4,    3,    0,    3,
    0,    4,    1,    2,    1,    1,    1,    2,    5,    7,
    5,    5,    7,    6,    7,    0,    7,    0,    8,    3,
    2,    2,    2,    3,    1,    2,    1,    1,    4,    3,
    1,    2,
  };
   static readonly short [] yyDefRed = {            0,
  110,   91,   92,   93,   94,   95,  138,  161,   97,   98,
   99,  100,  103,  104,  101,  102,  160,  162,   96,  105,
  106,  107,  114,  115,  116,    0,    0,  238,    0,    0,
    0,    0,    0,  108,  109,    0,  235,  237,    0,    0,
  236,  141,    0,    0,   77,    0,   87,    0,    0,    0,
   80,   82,   84,   86,    0,    0,    0,    0,    0,  134,
    0,  158,  156,    0,    0,   78,    0,    0,  241,    0,
  240,    0,    0,    0,    0,    0,    0,    0,    0,  117,
    0,    0,    0,  129,    0,  142,  159,  157,   88,    0,
    0,    0,    1,    2,    3,    0,    0,    0,    0,    0,
    0,    0,   25,   26,   27,   28,    5,    0,  188,    0,
   31,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   61,   90,  242,  239,  170,  153,    0,    0,
    0,    0,  165,    0,  150,    0,    0,    0,    0,  120,
  122,  112,  118,    0,    0,    0,  124,  130,    0,   29,
   76,  137,  131,  135,  210,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  217,    0,   74,
  215,  216,  200,  201,  202,  203,  204,  205,    0,  213,
    0,   23,    0,   18,   19,    0,    0,    0,    0,    0,
    0,  191,    0,    0,  196,   20,   21,    0,   11,   12,
    0,    0,    0,   64,   65,   66,   67,   68,   69,   70,
   71,   72,   73,   63,    0,   22,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  167,    0,  168,
    0,  151,  152,    0,    0,    0,  149,  145,    0,  144,
    0,    0,  111,  127,    0,    0,  119,  132,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  231,  232,  233,
    0,    0,  218,  212,  214,    0,    0,    4,    0,    0,
    0,  173,    0,  199,  189,    0,  192,  195,  197,   10,
    7,    0,   15,    0,    9,   62,   32,   33,   34,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  184,    0,    0,  178,    0,
    0,    0,    0,    0,  171,  164,  166,    0,    0,  148,
  143,  128,  125,  206,    0,  208,    0,    0,    0,    0,
    0,    0,  230,  234,   75,    0,    0,    0,   30,  198,
  190,  193,    0,    8,    0,    6,    0,  185,  177,  182,
  179,  186,    0,  180,    0,    0,  146,  147,  207,    0,
    0,    0,    0,    0,    0,    0,  194,   16,   60,  187,
  183,  181,    0,  221,  222,    0,  226,    0,    0,    0,
   13,    0,    0,    0,    0,  228,  224,    0,   14,  220,
  223,  227,    0,  225,  229,
  };
  protected static readonly short [] yyDgoto  = {            27,
  107,  169,  108,  292,  187,  191,  170,  110,  111,  112,
  113,  114,  115,  116,  117,  118,  119,  120,  121,  122,
  123,  215,  152,   28,   70,   46,   30,   31,   32,   33,
   47,   61,  192,   34,   35,   36,   79,   80,   81,  146,
  147,   59,   60,   49,   50,   64,  317,  131,  132,  133,
  318,  241,  193,  194,  195,  172,  173,  174,  175,  176,
  177,  178,   91,  179,   92,  180,  395,  403,   37,   38,
   72,
  };
  protected static readonly short [] yySindex = {         2635,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -96, 2635,    0,   54, 2635,
 2635, 2635, 2635,    0,    0,  -92,    0,    0,  -51, -181,
    0,    0,  -26,  -40,    0,  131,    0, 1144,  -21,   15,
    0,    0,    0,    0,  -39, 2661, -181,   60,  -15,    0,
  110,    0,    0,  -40,  -26,    0,    0, 1367,    0,   54,
    0, 2556,   15,  944,  487, 2661, 2661, 2661, 2468,    0,
   96,    7, 2456,    0,  -84,    0,    0,    0,    0,  101,
   70,  256,    0,    0,    0, 2464, 2477, 2477,  668, 2181,
 2456, 2456,    0,    0,    0,    0,    0,  -28,    0,  111,
    0, 2456,  253,  207,  130,  -45,  -25,  167,  129,  147,
   14,  -55,    0,    0,    0,    0,    0,    0,  277,  232,
   71,  244,    0,   -7,    0, 1328,  200, 1126, 2582,    0,
    0,    0,    0, 2456,  239,  135,    0,    0,  -77,    0,
    0,    0,    0,    0,    0,  258, 2456,  313,  274,  283,
  336,  544,  341,  170,  401,  402, 2139,    0,  141,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   45,    0,
  668,    0,  668,    0,    0,  102,  422,   97, 2456,  208,
    9,    0, 1367,  156,    0,    0,    0,  209,    0,    0,
 2199, 2456,  211,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2456,    0, 2456, 2456, 2456, 2456,
 2456, 2456, 2456, 2456, 2456, 2456, 2456, 2456, 2456, 2456,
 2456, 2456, 2456, 2456, 2456,  841, 2210,    0,   -2,    0,
   27,    0,    0,  238, 2608,  589,    0,    0, 2456,    0,
 2224,  371,    0,    0, 2456,   96,    0,    0,  544,  409,
  544, 2456, 2456, 2456,  185,  617,  449,    0,    0,    0,
  142, 2456,    0,    0,    0,  469,  476,    0, 2268,  893,
   28,    0,  428,    0,    0,  996,    0,    0,    0,    0,
    0,  148,    0,   29,    0,    0,    0,    0,    0,  253,
  253,  207,  207,  130,  130,  130,  130,  -45,  -45,  -25,
  167,  129,  147,   14,  217,    0,  481,  482,    0, 2281,
  431,   27, 1013, 2289,    0,    0,    0,  433,  435,    0,
    0,    0,    0,    0,  544,    0,  155,  210,  237,  491,
 2297, 2297,    0,    0,    0,  412,  412, 2181,    0,    0,
    0,    0, 1367,    0, 2456,    0, 2456,    0,    0,    0,
    0,    0,  495,    0, 2321,  444,    0,    0,    0,  544,
  544,  544, 2456, 2338, 2379,   10,    0,    0,    0,    0,
    0,    0,  252,    0,    0,  265,    0,  325,  544,  326,
    0, 2170,  544,  509,  544,    0,    0,  544,    0,    0,
    0,    0,  544,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  -31,
  -20,   61,  100,    0,    0,    0,    0,    0,  720,    0,
    0,    0,    0,  123,    0,    0,    0,  153,    0,  987,
    0,    0,    0,    0,  775,    0,    0,   31,    0,    0,
    0,    0,    0,  418,    0,    0,  115,    0,    0,    0,
    0,    0, 1042,    0,    0,    0,  166,  583,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  153,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 1442,    0, 1510,
    0,    0,   -1, 1726, 1822, 1960, 1482,  553,  712,    6,
 2550, 1540,    0,    0,    0,    0,    0,    0,  352,    0,
    0,  538,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  169,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 1415,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  540,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  399,    0,
  407,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  542,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 1578,
 1604, 1763, 1771, 1845, 1882, 1890, 1919, 2007, 2014, 2082,
  627, 2509, 2529, 2566,    0,    0,    0,    0,    0,    0,
    0,  411,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 1470,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  187,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -53,    0,    0,   79,  240,  -68,  508,  571,    0,
  221,  231,  212,  228,  355,  359,  360,  362,  368,    0,
  -78,    0,  -62,    8,    3,    0,    0,  522, 2571,    0,
  528,   16,  -67,    0,    0,    0,  520,  -30,  434,    0,
  351,  551,  -33,  -38,  -24,   32,  -70,    0,    0,  369,
  -12,  -36, -263,    0,  424, 1207,    0,   26, -158,    0,
    0,    0,    0,    0,    0,  440,    0,    0,  599,    0,
    0,
  };
  protected static readonly short [] yyTable = {           109,
  124,   44,   29,  130,  151,   63,  137,  235,   79,   79,
   79,  201,   79,   43,  226,   44,  227,  203,   43,   81,
   81,   81,  353,   81,   73,   88,   40,   79,   85,   29,
   56,  109,   51,   52,   53,   54,   35,  236,   81,   35,
  153,   35,   35,   35,   48,  186,   55,  258,  143,   55,
  149,  154,  286,  392,   74,   69,   35,   35,   35,   79,
   35,   35,  202,   55,   55,  151,  323,  280,   55,  252,
   81,   57,  272,   71,  136,   58,  129,  106,  151,  125,
   90,  254,  101,   76,   99,   90,  102,  103,  237,  104,
  239,   35,   35,   43,  260,   44,  145,  126,   55,  171,
   83,   83,   83,  168,   83,   75,  138,  342,  143,   84,
  151,  243,   45,  271,  244,  154,  240,  324,  237,   83,
   83,  356,   35,   35,  109,  287,  283,  186,  353,  186,
   55,  148,  293,  285,  391,   43,  280,   44,   44,   85,
   85,   85,  278,   85,  238,  272,  296,  211,  294,  281,
   86,   83,  211,  144,  211,  136,  211,  211,   85,  211,
   39,   68,  154,  154,   55,  246,  154,   67,  321,  274,
  105,  214,   58,  211,   65,  282,  151,  328,  256,   58,
  329,  315,  374,  375,  272,  272,  171,  237,  354,   66,
   85,  355,  332,  257,  155,  370,   89,  239,  272,  273,
  344,  190,  322,  345,  230,  121,  121,  121,  337,  338,
  339,   89,  126,  154,   73,  234,  288,  109,  352,  219,
  224,  225,  231,  121,  219,   79,  219,  126,  219,  219,
   42,  219,  198,  199,  200,   42,   81,  211,  129,  209,
  211,  281,  228,  229,  322,  219,  189,  129,    8,  220,
  371,  221,  363,  272,   42,  366,  121,   17,   18,  276,
  272,  277,   35,   35,   35,   35,   35,   35,   35,   35,
  232,  145,  242,  341,  357,   55,   55,  372,  379,  109,
  272,    8,  129,  233,  109,  377,  378,  245,  106,  219,
   17,   18,  248,  101,  217,   99,  255,  102,  103,  218,
  104,  156,   94,   95,   96,  394,   97,   98,  272,  219,
   42,  219,  219,  262,  168,  259,  236,   83,   44,  386,
  388,  390,  263,  109,  352,  129,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,
   25,   26,   42,  157,  158,  159,   85,  160,  161,  162,
  163,  164,  165,  166,  167,  396,  398,  237,  272,  272,
  261,  211,  211,  211,  211,  264,  211,  211,   67,  154,
  266,  105,  204,  205,  206,  207,  208,  209,  210,  211,
  212,  213,  169,  222,  223,  169,  211,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  121,  211,  211,  211,  267,  211,  211,  211,
  211,  211,  211,  211,  211,  304,  305,  306,  307,  174,
  300,  301,  174,  219,  219,  219,  219,  175,  219,  219,
  175,  176,  302,  303,  176,  308,  309,  155,  155,  268,
  269,  155,  279,  331,  284,  290,  335,  295,  219,  219,
  219,  219,  219,  219,  219,  219,  219,  219,  219,  219,
  219,  219,  219,  219,  219,  219,  219,  219,  219,  219,
  219,  219,  219,  219,  325,  219,  219,  219,  340,  219,
  219,  219,  219,  219,  219,  219,  219,  343,  155,  346,
  140,  141,  156,   94,   95,   96,  347,   97,   98,  106,
  350,  358,  359,  361,  101,  367,   99,  368,  136,  103,
  373,  104,  188,   42,  348,  380,  382,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   24,   25,   26,  393,  157,  158,  159,  401,  160,  161,
  162,  163,  164,  165,  166,  167,  106,   77,  163,  135,
  172,  101,  174,   99,  310,  102,  103,  376,  104,  311,
  150,  312,   89,   51,  313,  139,   51,   77,   77,   77,
   77,  314,  168,  182,  184,  185,  333,   82,  150,  150,
   51,   51,  105,  327,  188,   51,  188,  289,  275,  150,
   77,  106,  123,  123,  123,   41,  101,    0,   99,    0,
  102,  103,    0,  104,    0,    0,    0,    0,    0,    0,
  123,    0,    0,  150,    0,   51,   51,    0,    0,  106,
    0,  150,    0,    0,  101,    0,   99,    0,  102,  103,
   77,  104,    0,    0,  150,    0,   67,   52,    0,  105,
   52,  196,  197,  123,  155,  168,   51,   51,    0,    0,
    0,    0,  216,    0,   52,   52,    0,    0,    0,   52,
    0,    0,    0,    0,    0,    0,  150,    0,    0,    0,
  106,    0,   77,    0,   77,  101,  197,   99,    0,  102,
  103,    0,  104,    0,  105,    0,    0,    0,    0,   52,
   52,    0,    0,    0,  150,  150,  150,  150,  150,  150,
  150,  150,  150,  150,  150,  150,  150,  150,  150,  150,
  150,  150,  105,   93,   94,   95,   96,    0,   97,   98,
   52,   52,   53,    0,    0,   53,    0,    0,  150,  133,
  133,  133,  150,  133,    0,    0,    0,    0,    0,   53,
   53,  134,    0,    0,   53,    8,    0,  133,  133,    0,
    0,    0,    0,    0,   17,   18,  150,  297,  298,  299,
    0,    0,    0,  105,    0,    0,    0,    0,    0,    0,
  156,   94,   95,   96,   53,   97,   98,    0,    0,    0,
  133,    0,    0,    0,  113,  113,  113,    0,  113,    0,
    0,  197,   51,   51,    0,    0,    0,  150,    0,    0,
    0,    0,  113,  113,    0,   53,   53,    0,    0,  123,
    0,    0,    0,    0,    0,   93,   94,   95,   96,  349,
   97,   98,  157,  158,  159,    0,  160,  161,  162,  163,
  164,  165,  166,  167,  150,  113,    0,    0,    0,    0,
    0,    0,  150,   93,   94,   95,   96,    8,   97,   98,
  236,  316,   44,    0,    0,    0,   17,   18,    0,    0,
  197,    0,    0,    0,    0,    0,   52,   52,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,   24,   25,   26,   93,   94,   95,   96,    0,   97,
   98,  237,  280,  316,   44,  197,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    1,
    0,    0,    0,    0,    0,    0,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,   26,    0,  133,    0,    0,    0,
    0,   53,   53,  237,  128,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  140,  106,    0,
  140,  113,    0,  101,    0,   99,    0,  102,  103,    0,
  104,  190,    0,    0,  140,  140,    0,  140,    0,    0,
    0,    0,    0,  362,    0,    0,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  139,    0,    0,  139,  189,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   42,    0,  139,
  139,    0,  139,    0,    0,    0,    0,    0,    0,  140,
    0,    0,    0,    0,    0,    0,    0,    0,  100,    0,
  351,  105,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,   24,   25,   26,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  106,    0,
    0,    0,    0,  101,  139,   99,    0,  251,  103,    0,
  104,    0,    0,    0,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,   26,
  127,    0,    0,    0,   68,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  250,    0,
    0,    0,    0,    0,    0,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
   26,  105,   93,   94,   95,   96,    0,   97,   98,    0,
    0,    0,    0,    0,    0,    0,   67,    0,  140,  140,
  140,  140,  140,  140,  140,  140,  140,  140,  140,  140,
  140,  140,  140,  140,  140,  140,  140,  140,  140,  140,
  140,  140,  140,  140,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,   26,
    0,    0,    0,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  106,    0,    0,    0,    0,  101,    0,   99,  265,  102,
  103,    0,  104,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   93,   94,   95,   96,    0,   97,   98,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  106,
    0,    0,    0,    0,  101,    0,   99,    0,  102,  103,
  249,  104,    0,    0,    8,    0,    0,    0,    0,    0,
  247,    0,    0,   17,   18,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
   26,    1,    1,  105,    1,    0,    1,    1,    1,    1,
    1,    1,    0,    0,    0,  334,    0,  336,    0,    0,
    0,    0,    0,    1,    1,    1,    1,    1,   17,   17,
    0,    0,   17,   17,   17,   17,   17,    0,   17,  100,
    0,    0,  105,    0,    0,    0,    0,    0,    0,   17,
   17,   17,   17,   17,   17,    1,   24,   24,    1,    0,
   24,   24,   24,   24,   24,    0,   24,    0,    0,   49,
    0,    0,   49,    0,    0,   49,    0,   24,   24,   24,
   24,   24,   24,    0,   17,   17,    0,    0,    1,   49,
   49,  369,    0,    0,   49,    0,   29,   29,    0,    0,
   29,   29,   29,   29,   29,    0,   29,    0,    0,    0,
    0,    0,   24,   24,    0,   17,   17,   29,   29,   29,
    0,   29,   29,    0,   49,   49,  383,  384,  385,    0,
   59,    0,    0,   59,   93,   94,   95,   96,    0,   97,
   98,    0,    0,   24,   24,  397,    0,   59,   59,  400,
    0,  402,   29,   29,  404,   49,   49,    0,    0,  405,
    0,    0,    0,    0,    0,   36,    0,    0,   36,    0,
   36,   36,   36,   93,   94,   95,   96,    0,   97,   98,
    0,    0,   59,   29,   29,   36,   36,   36,    0,   36,
   36,   37,    0,    0,   37,    0,   37,   37,   37,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   37,   37,   37,   59,   37,   37,    0,    0,    0,
   36,   36,    0,    0,    0,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,   37,   37,    0,    0,
    0,   36,   36,    0,    0,   17,   17,   17,   17,   17,
   17,   17,   17,   17,   17,   17,   17,   17,   17,   17,
   17,   17,   17,    0,    0,    0,    0,   37,   37,    0,
    0,    0,    0,   24,   24,   24,   24,   24,   24,   24,
   24,   24,   24,   24,   24,   24,   24,   24,   24,   24,
   24,   49,   49,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   38,    0,    0,   38,    0,    0,   38,
    0,    0,    0,   29,   29,   29,   29,   29,   29,   29,
   29,    0,    0,   38,   38,   38,    0,   38,   38,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   39,    0,    0,   39,    0,    0,   39,    0,   40,    0,
    0,   40,    0,    0,   40,    0,    0,    0,   38,   38,
   39,   39,   39,    0,   39,   39,    0,    0,   40,   40,
   40,    0,   40,   40,    0,    0,    0,    0,    0,    0,
    0,   36,   36,   36,   36,   36,   36,   36,   36,   38,
   38,    0,    0,    0,    0,   39,   39,    0,    0,   41,
    0,    0,   41,   40,   40,   41,    0,   37,   37,   37,
   37,   37,   37,   37,   37,    0,    0,    0,    0,   41,
   41,   41,   44,   41,   41,   44,   39,   39,   44,    0,
    0,    0,    0,    0,   40,   40,    0,    0,    0,    0,
    0,    0,   44,   44,   44,    0,   44,   44,    0,    0,
    0,    0,    0,    0,   41,   41,    0,    0,    0,   45,
    0,    0,   45,    0,    0,   45,    0,   42,    0,    0,
   42,    0,    0,   42,    0,    0,    0,   44,   44,   45,
   45,   45,    0,   45,   45,   41,   41,   42,   42,   42,
    0,   42,   42,    0,    0,    0,   43,    0,    0,   43,
    0,    0,   43,    0,    0,    0,    0,    0,   44,   44,
    0,    0,    0,    0,   45,   45,   43,   43,   43,    0,
   43,   43,   42,   42,    0,    0,    0,    0,    0,   38,
   38,   38,   38,   38,   38,   38,   38,   46,    0,    0,
   46,    0,    0,   46,    0,   45,   45,    0,    0,    0,
    0,   43,   43,   42,   42,    0,    0,   46,   46,    0,
    0,    0,   46,    0,    0,    0,   39,   39,   39,   39,
   39,   39,   39,   39,   40,   40,   40,   40,   40,   40,
   40,   40,   43,   43,   47,    0,    0,   47,    0,    0,
   47,   48,   46,   46,   48,    0,    0,   48,    0,    0,
    0,    0,    0,    0,   47,   47,    0,    0,    0,   47,
    0,   48,   48,    0,    0,    0,   48,    0,    0,    0,
    0,    0,    0,   46,   46,    0,    0,   41,   41,   41,
   41,   41,   41,    0,    0,    0,    0,    0,    0,   47,
   47,    0,    0,    0,    0,    0,   48,   48,    0,    0,
   44,   44,   44,   44,   44,   44,    0,    0,    0,   50,
    0,    0,   50,    0,    0,   50,    0,    0,    0,    0,
   47,   47,    0,    0,    0,    0,    0,   48,   48,   50,
   50,    0,    0,    0,   50,    0,    0,   45,   45,   45,
   45,   45,   45,    0,    0,   42,   42,   42,   42,   42,
   42,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  106,    0,    0,   50,   50,  101,    0,   99,    0,
  102,  103,    0,  104,   43,   43,   43,   43,   43,   43,
    0,    0,    0,    0,    0,    0,    0,  270,    0,    0,
    0,    0,  106,    0,    0,   50,   50,  101,    0,   99,
    0,  102,  103,  106,  104,  190,    0,    0,  101,    0,
   99,    0,  102,  103,    0,  104,  190,   46,   46,   46,
   46,  106,    0,    0,    0,    0,  101,    0,   99,  291,
  102,  103,  106,  104,    0,    0,    0,  101,    0,   99,
    0,  320,  103,    0,  104,    0,  106,    0,    0,    0,
  189,  101,    0,   99,  105,  102,  103,    0,  104,    0,
    0,  189,    0,    0,   47,   47,   47,   47,    0,    0,
    0,   48,   48,   48,   48,    0,    0,    0,    0,    0,
    0,    0,  100,    0,  399,  105,    0,    0,    0,    0,
  106,    0,  319,  100,    0,  101,  105,   99,    0,  102,
  103,    0,  104,  106,    0,    0,  330,    0,  101,    0,
   99,  106,  102,  103,  105,  104,  101,    0,   99,  106,
  365,  103,    0,  104,  101,  105,   99,    0,  102,  103,
    0,  104,    0,    0,    0,    0,    0,    0,    0,  105,
    0,   50,   50,  106,    0,  168,    0,    0,  101,    0,
   99,    0,  102,  103,    0,  104,    0,    0,    0,    0,
  106,    0,    0,  360,    0,  101,    0,   99,  387,  102,
  103,  364,  104,    0,    0,    0,    0,    0,    0,    0,
  348,    0,    0,  105,    0,   93,   94,   95,   96,    0,
   97,   98,    0,    0,    0,    0,  105,    0,    0,    0,
    0,  106,    0,  381,  105,    0,  101,    0,   99,  389,
  102,  103,  105,  104,    0,    0,   93,   94,   95,   96,
    0,   97,   98,    0,    0,    0,    0,   93,   94,   95,
   96,    0,   97,   98,    0,    0,  105,    0,    0,    0,
    0,    0,    0,    0,    0,   93,   94,   95,   96,    0,
   97,   98,    0,  105,    0,    0,   93,   94,   95,   96,
    0,   97,   98,    0,    0,    0,    0,    0,    0,    0,
   93,   94,   95,   96,    0,   97,   98,    0,  106,    0,
    0,    0,    0,  101,    0,   99,  106,  102,  103,    0,
  104,  101,    0,  181,  105,  102,  103,    0,  104,  106,
    0,    0,    0,    0,  101,    0,  183,    0,  102,  103,
    0,  104,    0,    0,   93,   94,   95,   96,    0,   97,
   98,    0,    0,    0,    0,    0,    0,   93,   94,   95,
   96,    0,   97,   98,    0,   93,   94,   95,   96,   54,
   97,   98,   54,   93,   94,   95,   96,    0,   97,   98,
    0,    0,    0,    0,    0,    0,   54,   54,    0,   56,
    0,   54,   56,    0,    0,    0,    0,   93,   94,   95,
   96,  105,   97,   98,    0,    0,   56,   56,    0,  105,
   57,   56,  142,   57,   93,   94,   95,   96,    0,   97,
   98,   54,  105,    0,    0,    0,   58,   57,   57,   58,
    0,    0,   57,    0,   62,    0,    0,    0,    0,    0,
    0,   56,    0,   58,   58,    0,   78,    0,   58,    0,
    0,    0,   54,   54,   87,   93,   94,   95,   96,    0,
   97,   98,   57,    0,    0,   62,   78,   78,   78,   78,
    0,    0,    0,   56,    0,    0,    0,    0,   58,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   78,
    0,    0,    0,    0,   57,    0,    0,    0,   67,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   58,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   62,    0,  253,    0,   87,   78,
    0,    0,   93,   94,   95,   96,    0,   97,   98,    0,
   93,   94,   95,   96,    0,   97,   98,    0,    0,    0,
    0,    0,    0,   93,   94,   95,   96,    0,   97,   98,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    1,
    0,   78,    0,   78,    0,    0,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,   26,    0,    0,    0,   54,   54,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   56,   56,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   87,    0,    0,    0,
   57,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   58,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   24,   25,   26,    1,    0,    0,    0,    0,    0,    0,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,   24,   25,   26,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,   26,  326,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,
   25,   26,    1,    0,    0,    0,    0,    0,    0,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,   24,   25,   26,
  };
  protected static readonly short [] yyCheck = {            68,
   68,   42,    0,   74,   83,   44,   75,   63,   40,   41,
   42,   40,   44,   40,   60,   42,   62,   46,   40,   40,
   41,   42,  286,   44,   49,   64,  123,   59,   44,   27,
  123,  100,   30,   31,   32,   33,   38,   40,   59,   41,
  125,   43,   44,   45,   29,   99,   41,  125,   79,   44,
   44,   85,   44,   44,   40,   48,   58,   59,   60,   91,
   62,   63,   91,   58,   59,  144,   40,   40,   63,  138,
   91,  123,   44,   48,   44,  257,   74,   33,  157,   72,
   65,  144,   38,  123,   40,   70,   42,   43,   91,   45,
  129,   93,   94,   40,  157,   42,   81,   72,   93,   92,
   40,   41,   42,   59,   44,   91,   75,  266,  139,  125,
  189,   41,   59,  167,   44,  149,  129,   91,   91,   59,
   61,   93,  124,  125,  193,  193,  189,  181,  392,  183,
  125,  125,  201,  125,  125,   40,   40,   42,   42,   40,
   41,   42,   41,   44,  129,   44,  215,   33,  202,  188,
   41,   91,   38,   58,   40,  125,   42,   43,   59,   45,
  257,   61,   40,   41,  257,  134,   44,  123,  237,  125,
  126,   61,  257,   59,   44,  188,  255,  246,   44,  257,
  249,  235,  341,  342,   44,   44,  179,   91,   41,   59,
   91,   44,  255,   59,  125,   41,   44,  236,   44,   59,
   59,   46,  239,  272,   38,   40,   41,   42,  262,  263,
  264,   59,   44,   91,  239,  271,   61,  286,  286,   33,
  266,  267,   94,   58,   38,  257,   40,   59,   42,   43,
  257,   45,  261,  262,  263,  257,  257,  123,  236,  125,
  126,  280,  268,  269,  281,   59,   91,  245,  289,   43,
   41,   45,  323,   44,  257,  324,   91,  298,  299,  181,
   44,  183,  264,  265,  266,  267,  268,  269,  270,  271,
  124,  256,   41,  266,   58,  270,  271,   41,  357,  348,
   44,  289,  280,  270,  353,  353,  355,   44,   33,   37,
  298,  299,   93,   38,   42,   40,   58,   42,   43,   47,
   45,  257,  258,  259,  260,   41,  262,  263,   44,  123,
  257,  125,  126,   40,   59,   58,   40,  257,   42,  373,
  374,  375,   40,  392,  392,  323,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  257,  309,  310,  311,  257,  313,  314,  315,
  316,  317,  318,  319,  320,   41,   41,   91,   44,   44,
   58,  257,  258,  259,  260,   40,  262,  263,  123,  257,
   40,  126,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,   41,  264,  265,   44,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  257,  309,  310,  311,  257,  313,  314,  315,
  316,  317,  318,  319,  320,  224,  225,  226,  227,   41,
  220,  221,   44,  257,  258,  259,  260,   41,  262,  263,
   44,   41,  222,  223,   44,  228,  229,   40,   41,   59,
   59,   44,   41,   93,  257,  257,   58,  257,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  257,  309,  310,  311,  314,  313,
  314,  315,  316,  317,  318,  319,  320,   59,   91,   41,
   77,   78,  257,  258,  259,  260,   41,  262,  263,   33,
   93,   41,   41,   93,   38,   93,   40,   93,   42,   43,
   40,   45,   99,  257,  123,   41,   93,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  312,  309,  310,  311,   59,  313,  314,
  315,  316,  317,  318,  319,  320,   33,   56,   41,   93,
   41,   38,   41,   40,  230,   42,   43,  348,   45,  231,
   83,  232,   65,   41,  233,   76,   44,   76,   77,   78,
   79,  234,   59,   96,   97,   98,  256,   57,  101,  102,
   58,   59,  126,  245,  181,   63,  183,  194,  179,  112,
   99,   33,   40,   41,   42,   27,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   -1,   -1,  136,   -1,   93,   94,   -1,   -1,   33,
   -1,  144,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
  139,   45,   -1,   -1,  157,   -1,  123,   41,   -1,  126,
   44,  101,  102,   91,  257,   59,  124,  125,   -1,   -1,
   -1,   -1,  112,   -1,   58,   59,   -1,   -1,   -1,   63,
   -1,   -1,   -1,   -1,   -1,   -1,  189,   -1,   -1,   -1,
   33,   -1,  181,   -1,  183,   38,  136,   40,   -1,   42,
   43,   -1,   45,   -1,  126,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   -1,  217,  218,  219,  220,  221,  222,
  223,  224,  225,  226,  227,  228,  229,  230,  231,  232,
  233,  234,  126,  257,  258,  259,  260,   -1,  262,  263,
  124,  125,   41,   -1,   -1,   44,   -1,   -1,  251,   40,
   41,   42,  255,   44,   -1,   -1,   -1,   -1,   -1,   58,
   59,  285,   -1,   -1,   63,  289,   -1,   58,   59,   -1,
   -1,   -1,   -1,   -1,  298,  299,  279,  217,  218,  219,
   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   93,  262,  263,   -1,   -1,   -1,
   91,   -1,   -1,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,  251,  270,  271,   -1,   -1,   -1,  320,   -1,   -1,
   -1,   -1,   58,   59,   -1,  124,  125,   -1,   -1,  257,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,  279,
  262,  263,  309,  310,  311,   -1,  313,  314,  315,  316,
  317,  318,  319,  320,  357,   91,   -1,   -1,   -1,   -1,
   -1,   -1,  365,  257,  258,  259,  260,  289,  262,  263,
   40,   41,   42,   -1,   -1,   -1,  298,  299,   -1,   -1,
  320,   -1,   -1,   -1,   -1,   -1,  270,  271,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  257,  258,  259,  260,   -1,  262,
  263,   91,   40,   41,   42,  365,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  282,
   -1,   -1,   -1,   -1,   -1,   -1,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,   -1,  257,   -1,   -1,   -1,
   -1,  270,  271,   91,   41,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  282,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,   41,   33,   -1,
   44,  257,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   46,   -1,   -1,   58,   59,   -1,   61,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   -1,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,   41,   -1,   -1,   44,   91,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   58,
   59,   -1,   61,   -1,   -1,   -1,   -1,   -1,   -1,  123,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,
  125,  126,  282,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   38,  123,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  257,   -1,   -1,   -1,   61,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  126,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,  162,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
  285,   45,   -1,   -1,  289,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,  298,  299,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,   37,   38,  126,   40,   -1,   42,   43,   44,   45,
   46,   47,   -1,   -1,   -1,  259,   -1,  261,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   63,   37,   38,
   -1,   -1,   41,   42,   43,   44,   45,   -1,   47,  123,
   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,   58,
   59,   60,   61,   62,   63,   91,   37,   38,   94,   -1,
   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   58,   59,   60,
   61,   62,   63,   -1,   93,   94,   -1,   -1,  124,   58,
   59,  335,   -1,   -1,   63,   -1,   37,   38,   -1,   -1,
   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,   93,   94,  370,  371,  372,   -1,
   41,   -1,   -1,   44,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,  124,  125,  389,   -1,   58,   59,  393,
   -1,  395,   93,   94,  398,  124,  125,   -1,   -1,  403,
   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   43,   44,   45,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   93,  124,  125,   58,   59,   60,   -1,   62,
   63,   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,  125,   62,   63,   -1,   -1,   -1,
   93,   94,   -1,   -1,   -1,  261,  262,  263,  264,  265,
  266,  267,  268,  269,  270,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,   93,   94,   -1,   -1,
   -1,  124,  125,   -1,   -1,  264,  265,  266,  267,  268,
  269,  270,  271,  272,  273,  274,  275,  276,  277,  278,
  279,  280,  281,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,  270,
  271,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  270,  271,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,  270,
  271,   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   93,   94,
   58,   59,   60,   -1,   62,   63,   -1,   -1,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,  270,  271,  124,
  125,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   38,
   -1,   -1,   41,   93,   94,   44,   -1,  264,  265,  266,
  267,  268,  269,  270,  271,   -1,   -1,   -1,   -1,   58,
   59,   60,   38,   62,   63,   41,  124,  125,   44,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,   93,   94,   58,
   59,   60,   -1,   62,   63,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   93,   94,   58,   59,   60,   -1,
   62,   63,   93,   94,   -1,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,  270,  271,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,  124,  125,   -1,   -1,   -1,
   -1,   93,   94,  124,  125,   -1,   -1,   58,   59,   -1,
   -1,   -1,   63,   -1,   -1,   -1,  264,  265,  266,  267,
  268,  269,  270,  271,  264,  265,  266,  267,  268,  269,
  270,  271,  124,  125,   38,   -1,   -1,   41,   -1,   -1,
   44,   38,   93,   94,   41,   -1,   -1,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,   63,
   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,  266,  267,  268,
  269,  270,  271,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
  266,  267,  268,  269,  270,  271,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,  124,  125,   58,
   59,   -1,   -1,   -1,   63,   -1,   -1,  266,  267,  268,
  269,  270,  271,   -1,   -1,  266,  267,  268,  269,  270,
  271,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   93,   94,   38,   -1,   40,   -1,
   42,   43,   -1,   45,  266,  267,  268,  269,  270,  271,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   -1,   -1,
   -1,   -1,   33,   -1,   -1,  124,  125,   38,   -1,   40,
   -1,   42,   43,   33,   45,   46,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   46,  268,  269,  270,
  271,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   41,
   42,   43,   33,   45,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,   33,   -1,   -1,   -1,
   91,   38,   -1,   40,  126,   42,   43,   -1,   45,   -1,
   -1,   91,   -1,   -1,  268,  269,  270,  271,   -1,   -1,
   -1,  268,  269,  270,  271,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  123,   -1,  125,  126,   -1,   -1,   -1,   -1,
   33,   -1,   93,  123,   -1,   38,  126,   40,   -1,   42,
   43,   -1,   45,   33,   -1,   -1,   93,   -1,   38,   -1,
   40,   33,   42,   43,  126,   45,   38,   -1,   40,   33,
   42,   43,   -1,   45,   38,  126,   40,   -1,   42,   43,
   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,
   -1,  270,  271,   33,   -1,   59,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   93,   -1,   38,   -1,   40,   41,   42,
   43,   93,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  123,   -1,   -1,  126,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   33,   -1,   93,  126,   -1,   38,   -1,   40,   41,
   42,   43,  126,   45,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,  126,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   33,   42,   43,   -1,
   45,   38,   -1,   40,  126,   42,   43,   -1,   45,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,   41,
  262,  263,   44,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   -1,   41,
   -1,   63,   44,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,  126,  262,  263,   -1,   -1,   58,   59,   -1,  126,
   41,   63,  125,   44,  257,  258,  259,  260,   -1,  262,
  263,   93,  126,   -1,   -1,   -1,   41,   58,   59,   44,
   -1,   -1,   63,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,   58,   59,   -1,   56,   -1,   63,   -1,
   -1,   -1,  124,  125,   64,  257,  258,  259,  260,   -1,
  262,  263,   93,   -1,   -1,   75,   76,   77,   78,   79,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   99,
   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,  123,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  134,   -1,  125,   -1,  138,  139,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  282,
   -1,  181,   -1,  183,   -1,   -1,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,   -1,   -1,   -1,  270,  271,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  246,   -1,   -1,   -1,
  271,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  271,  282,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  282,   -1,   -1,   -1,   -1,   -1,   -1,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  282,   -1,   -1,   -1,   -1,   -1,   -1,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,
  };

#line 985 "CParser.jay"

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
