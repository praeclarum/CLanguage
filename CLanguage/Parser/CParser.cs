// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "CParser.jay"
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using CLanguage.Syntax;
using CLanguage.Types;

#pragma warning disable 162
#nullable disable

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
  public System.IO.TextWriter ErrorOutput = new System.IO.StringWriter ();

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

  protected const int yyFinal = 28;
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
//t    "assignment_operator : BINARY_AND_ASSIGN",
//t    "assignment_operator : BINARY_XOR_ASSIGN",
//t    "assignment_operator : BINARY_OR_ASSIGN",
//t    "assignment_operator : AND_ASSIGN",
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
//t    "type_specifier : struct_or_union_or_class_specifier",
//t    "type_specifier : enum_specifier",
//t    "type_specifier : TYPE_NAME",
//t    "identifier_or_typename : IDENTIFIER",
//t    "identifier_or_typename : TYPE_NAME",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class identifier_or_typename class_body",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class class_body",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class identifier_or_typename",
//t    "struct_or_union_or_class : STRUCT",
//t    "struct_or_union_or_class : CLASS",
//t    "struct_or_union_or_class : UNION",
//t    "specifier_qualifier_list : type_specifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_specifier",
//t    "specifier_qualifier_list : type_qualifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_qualifier",
//t    "enum_specifier : ENUM '{' enumerator_list '}'",
//t    "enum_specifier : ENUM identifier_or_typename '{' enumerator_list '}'",
//t    "enum_specifier : ENUM '{' enumerator_list ',' '}'",
//t    "enum_specifier : ENUM identifier_or_typename '{' enumerator_list ',' '}'",
//t    "enum_specifier : ENUM identifier_or_typename",
//t    "enumerator_list : enumerator",
//t    "enumerator_list : enumerator_list ',' enumerator",
//t    "enumerator : IDENTIFIER",
//t    "enumerator : IDENTIFIER '=' constant_expression",
//t    "function_specifier : INLINE",
//t    "declarator : pointer direct_declarator",
//t    "declarator : direct_declarator",
//t    "direct_declarator_identifier_list : IDENTIFIER",
//t    "direct_declarator_identifier_list : '~' IDENTIFIER",
//t    "direct_declarator_identifier_list : direct_declarator_identifier_list COLONCOLON IDENTIFIER",
//t    "direct_declarator_identifier_list : direct_declarator_identifier_list COLONCOLON '~' IDENTIFIER",
//t    "direct_declarator : direct_declarator_identifier_list",
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
//t    "direct_declarator : direct_declarator '(' argument_expression_list ')'",
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
//t    "parameter_declaration : declaration_specifiers declarator '=' assignment_expression",
//t    "parameter_declaration : declaration_specifiers abstract_declarator",
//t    "parameter_declaration : declaration_specifiers",
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
//t    "class_body : '{' '}'",
//t    "class_body : '{' class_block_item_list '}'",
//t    "class_block_item_list : class_block_item",
//t    "class_block_item_list : class_block_item_list class_block_item",
//t    "class_block_item : declaration",
//t    "class_block_item : visibility",
//t    "class_block_item : ctor_declaration",
//t    "visibility : PUBLIC ':'",
//t    "visibility : PRIVATE ':'",
//t    "visibility : PROTECTED ':'",
//t    "ctor_declarator : direct_declarator_identifier_list '(' parameter_type_list ')'",
//t    "ctor_declarator : direct_declarator_identifier_list '(' ')'",
//t    "ctor_declaration : ctor_declarator ';'",
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
//t    "external_declaration : ';'",
//t    "external_declaration : ctor_definition",
//t    "function_definition : declaration_specifiers declarator declaration_list compound_statement",
//t    "function_definition : declaration_specifiers declarator compound_statement",
//t    "ctor_identifier_list : TYPE_NAME",
//t    "ctor_identifier_list : ctor_identifier_list COLONCOLON TYPE_NAME",
//t    "ctor_identifier_list : ctor_identifier_list COLONCOLON '~' TYPE_NAME",
//t    "ctor_definition : ctor_identifier_list '(' ')' compound_statement",
//t    "ctor_definition : ctor_identifier_list '(' parameter_type_list ')' compound_statement",
//t    "declaration_list : declaration",
//t    "declaration_list : preproc declaration",
//t    "declaration_list : declaration_list declaration",
//t    "preproc : EOL",
//t    "preproc : '#'",
//t    "preproc : '\\\\'",
//t  };
//t public static string getRule (int index) {
//t    return yyRule [index];
//t }
//t}
  protected static readonly string [] yyNames = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"'!'",null,"'#'",null,"'%'","'&'",
    null,"'('","')'","'*'","'+'","','","'-'","'.'","'/'",null,null,null,
    null,null,null,null,null,null,null,"':'","';'","'<'","'='","'>'",
    "'?'",null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"'['","'\\\\'","']'","'^'",null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"'{'","'|'","'}'","'~'",null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,"IDENTIFIER","CONSTANT","STRING_LITERAL","SIZEOF","PTR_OP",
    "INC_OP","DEC_OP","LEFT_OP","RIGHT_OP","LE_OP","GE_OP","EQ_OP",
    "NE_OP","COLONCOLON","AND_OP","OR_OP","MUL_ASSIGN","DIV_ASSIGN",
    "MOD_ASSIGN","ADD_ASSIGN","SUB_ASSIGN","LEFT_ASSIGN","RIGHT_ASSIGN",
    "BINARY_AND_ASSIGN","BINARY_XOR_ASSIGN","BINARY_OR_ASSIGN",
    "AND_ASSIGN","OR_ASSIGN","TYPE_NAME","PUBLIC","PRIVATE","PROTECTED",
    "TYPEDEF","EXTERN","STATIC","AUTO","REGISTER","INLINE","RESTRICT",
    "CHAR","SHORT","INT","LONG","SIGNED","UNSIGNED","FLOAT","DOUBLE",
    "CONST","VOLATILE","VOID","BOOL","COMPLEX","IMAGINARY","TRUE","FALSE",
    "STRUCT","CLASS","UNION","ENUM","ELLIPSIS","CASE","DEFAULT","IF",
    "ELSE","SWITCH","WHILE","DO","FOR","GOTO","CONTINUE","BREAK","RETURN",
    "EOL",
  };

  /** index-checked interface to yyNames[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
  public static string yyname (int token) {
    if ((token < 0) || (token > yyNames.Length)) return "[illegal]";
    string name;
    if ((name = yyNames[token]) != null) return name;
    return "[unknown]";
  }

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
                    && (yyN += TokenKind.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == TokenKind.yyErrorCode) {
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
#line 46 "CParser.jay"
  { var t = lexer.CurrentToken; yyVal = new VariableExpression((yyVals[0+yyTop]).ToString(), t.Location, t.EndLocation); }
  break;
case 2:
#line 47 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 48 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 49 "CParser.jay"
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 50 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 51 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 58 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 62 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 66 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 70 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 74 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 78 "CParser.jay"
  {
		yyVal = new MemberFromPointerExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 13:
#line 82 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 86 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 90 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 94 "CParser.jay"
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
#line 116 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 120 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 124 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 128 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 132 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 136 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 140 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 144 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 148 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 149 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 150 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 151 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 158 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 162 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 169 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 173 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 177 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 181 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 189 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 193 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 201 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 205 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 213 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 217 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 221 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 225 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 233 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 237 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 245 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 253 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 261 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 269 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 277 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 284 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 288 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 313 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 314 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 315 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 316 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 317 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 318 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 319 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 320 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 321 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 322 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 323 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 324 "CParser.jay"
  { yyVal = LogicOp.And; }
  break;
case 77:
#line 325 "CParser.jay"
  { yyVal = LogicOp.Or; }
  break;
case 78:
#line 332 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 79:
#line 336 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 81:
#line 347 "CParser.jay"
  {
		yyVal = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-1+yyTop], null);
	}
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
#line 426 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[0+yyTop], null);
	}
  break;
case 94:
#line 430 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[-2+yyTop], (Initializer)yyVals[0+yyTop]);
	}
  break;
case 95:
#line 434 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 96:
#line 435 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 97:
#line 436 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 98:
#line 437 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 99:
#line 438 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 100:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 101:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 102:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 103:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 104:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 105:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 106:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 107:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 108:
#line 450 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 109:
#line 451 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 110:
#line 452 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 111:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 114:
#line 456 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 117:
#line 465 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 118:
#line 466 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 119:
#line 467 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 120:
#line 471 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 121:
#line 472 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 122:
#line 473 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
  break;
case 123:
  case_123();
  break;
case 124:
  case_124();
  break;
case 125:
  case_125();
  break;
case 126:
  case_126();
  break;
case 127:
#line 502 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 128:
#line 503 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 129:
#line 504 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 130:
#line 505 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 131:
#line 506 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 132:
  case_132();
  break;
case 133:
  case_133();
  break;
case 134:
#line 528 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 135:
#line 532 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 136:
#line 536 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 137:
#line 543 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 138:
#line 544 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 139:
#line 552 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 140:
#line 556 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("~" + (yyVals[-1+yyTop]).ToString());
	}
  break;
case 141:
#line 558 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 142:
#line 560 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[-1+yyTop]).ToString()); }
  break;
case 144:
  case_144();
  break;
case 145:
#line 580 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 146:
#line 584 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 147:
#line 588 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 148:
#line 592 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 149:
#line 596 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 150:
#line 600 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 151:
#line 604 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 152:
#line 608 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 153:
#line 612 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 154:
  case_154();
  break;
case 155:
#line 624 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 156:
#line 628 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 157:
#line 629 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 158:
#line 630 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 159:
#line 631 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 160:
#line 635 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 161:
#line 639 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 162:
#line 643 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 163:
#line 644 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 164:
#line 645 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 165:
#line 652 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
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
#line 680 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 170:
#line 684 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 171:
#line 688 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 172:
#line 692 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 173:
#line 699 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 174:
#line 703 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 175:
#line 710 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 177:
#line 715 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 178:
  case_178();
  break;
case 179:
#line 734 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 180:
#line 738 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 181:
#line 742 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 182:
#line 746 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 183:
#line 750 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 184:
#line 754 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 185:
#line 758 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 186:
#line 762 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 187:
#line 766 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 188:
#line 770 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 189:
#line 777 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 190:
#line 781 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 191:
#line 785 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
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
  case_195();
  break;
case 196:
#line 825 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 210:
#line 857 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 211:
#line 861 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 212:
#line 865 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 213:
#line 866 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 216:
#line 878 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 217:
#line 882 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 218:
#line 886 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 219:
#line 887 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 223:
#line 897 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 224:
#line 898 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 225:
#line 899 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 226:
#line 906 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 227:
#line 910 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 228:
  case_228();
  break;
case 229:
#line 928 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 230:
#line 932 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 231:
#line 939 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 232:
#line 943 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 234:
#line 951 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 235:
#line 955 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 236:
#line 959 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 237:
#line 963 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 238:
#line 967 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 239:
#line 971 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 241:
#line 979 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 242:
#line 983 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 243:
#line 987 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 244:
#line 991 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 245:
  case_245();
  break;
case 246:
  case_246();
  break;
case 251:
  case_251();
  break;
case 252:
  case_252();
  break;
case 253:
#line 1039 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 254:
#line 1040 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 255:
#line 1041 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
  break;
case 256:
  case_256();
  break;
case 257:
  case_257();
  break;
case 258:
  case_258();
  break;
case 259:
  case_259();
  break;
case 260:
  case_260();
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
#line 99 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 105 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 294 "CParser.jay"
{
		if (yyVals[-1+yyTop] is RelationalOp r && r == RelationalOp.Equals) {
			yyVal = new AssignExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
		}
		else if (yyVals[-1+yyTop] is Binop b) {
			var left = (Expression)yyVals[-2+yyTop];
			yyVal = new AssignExpression(left, new BinaryExpression (left, b, (Expression)yyVals[0+yyTop]));
		}
        else if (yyVals[-1+yyTop] is LogicOp l) {
            var left = (Expression)yyVals[-2+yyTop];
            yyVal = new AssignExpression(left, new LogicExpression (left, l, (Expression)yyVals[0+yyTop]));
        }
        else {
            throw new NotSupportedException (String.Format ("'{0}' not supported", yyVals[-1+yyTop]));
        }
	}

void case_82()
#line 349 "CParser.jay"
{
		DeclarationSpecifiers ds = (DeclarationSpecifiers)yyVals[-2+yyTop];
		List<InitDeclarator> decls = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_83()
#line 358 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_84()
#line 364 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];		
		yyVal = ds;
	}

void case_85()
#line 370 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_86()
#line 376 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_87()
#line 382 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 388 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 394 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_90()
#line 400 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_91()
#line 409 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_92()
#line 415 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_123()
#line 478 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_124()
#line 483 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_125()
#line 489 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_126()
#line 494 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_132()
#line 511 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_133()
#line 517 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_144()
#line 566 "CParser.jay"
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

void case_154()
#line 614 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<Expression>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(ctorArgumentValue: n));
		}
		yyVal = d;
	}

void case_166()
#line 654 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_167()
#line 663 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_168()
#line 669 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_178()
#line 720 "CParser.jay"
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

void case_192()
#line 790 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_193()
#line 797 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_194()
#line 805 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_195()
#line 812 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_228()
#line 915 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		DeclarationSpecifiers ds = new DeclarationSpecifiers();
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator(fdecl, null) };
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_245()
#line 996 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_246()
#line 1001 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_251()
#line 1016 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_252()
#line 1025 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_256()
#line 1046 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_257()
#line 1055 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_258()
#line 1067 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_259()
#line 1073 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_260()
#line 1079 "CParser.jay"
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
   22,   22,   22,   22,   22,   22,   22,    2,    2,   23,
   24,   24,   25,   25,   25,   25,   25,   25,   25,   25,
   26,   26,   31,   31,   27,   27,   27,   27,   27,   28,
   28,   28,   28,   28,   28,   28,   28,   28,   28,   28,
   28,   28,   28,   28,   36,   36,   34,   34,   34,   37,
   37,   37,   39,   39,   39,   39,   35,   35,   35,   35,
   35,   40,   40,   41,   41,   30,   32,   32,   44,   44,
   44,   44,   43,   43,   43,   43,   43,   43,   43,   43,
   43,   43,   43,   43,   43,   42,   42,   42,   42,   45,
   45,   29,   29,   29,   46,   46,   47,   47,   48,   48,
   48,   48,    5,    5,   49,   49,   49,   50,   50,   50,
   50,   50,   50,   50,   50,   50,   50,   50,   33,   33,
   33,    6,    6,    6,    6,   51,   52,   52,   53,   53,
   54,   54,   54,   54,   54,   54,   55,   55,   55,   56,
   56,   61,   61,   62,   62,   38,   38,   63,   63,   64,
   64,   64,   65,   65,   65,   67,   67,   66,   57,   57,
   58,   58,   58,   59,   59,   59,   59,   59,   59,   60,
   60,   60,   60,   60,    0,    0,   68,   68,   68,   68,
   69,   69,   72,   72,   72,   70,   70,   71,   71,   71,
   73,   73,   73,
  };
   static readonly short [] yyLen = {           2,
    1,    1,    1,    1,    1,    3,    1,    4,    3,    4,
    3,    3,    2,    2,    6,    7,    1,    3,    1,    2,
    2,    2,    2,    2,    2,    4,    1,    1,    1,    1,
    1,    4,    1,    3,    3,    3,    1,    3,    3,    1,
    3,    3,    1,    3,    3,    3,    3,    1,    3,    3,
    1,    3,    1,    3,    1,    3,    1,    3,    1,    3,
    1,    5,    1,    3,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    3,    1,
    2,    3,    1,    2,    1,    2,    1,    2,    1,    2,
    1,    3,    1,    3,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    3,    2,    2,    1,
    1,    1,    2,    1,    2,    1,    4,    5,    5,    6,
    2,    1,    3,    1,    3,    1,    2,    1,    1,    2,
    3,    4,    1,    3,    5,    4,    4,    6,    6,    5,
    4,    3,    4,    4,    3,    1,    2,    2,    3,    1,
    2,    1,    1,    1,    1,    3,    1,    3,    2,    4,
    2,    1,    1,    2,    1,    1,    2,    3,    2,    3,
    3,    4,    3,    4,    2,    3,    3,    4,    1,    3,
    4,    1,    2,    3,    4,    2,    1,    2,    3,    2,
    1,    1,    1,    1,    1,    1,    3,    4,    3,    2,
    3,    1,    2,    1,    1,    2,    3,    1,    2,    1,
    1,    1,    2,    2,    2,    4,    3,    2,    1,    2,
    5,    7,    5,    5,    7,    6,    7,    6,    7,    3,
    2,    2,    2,    3,    1,    2,    1,    1,    1,    1,
    4,    3,    1,    3,    4,    4,    5,    1,    2,    2,
    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,   95,   96,   97,   98,   99,  136,  163,  101,  102,
  103,  104,  107,  108,  105,  106,  162,  164,  100,  109,
  110,  111,  120,  121,  122,    0,  249,    0,  248,    0,
    0,    0,    0,    0,  112,  113,    0,  245,  247,  250,
    0,  115,  116,    0,    0,  246,  139,    0,    0,    0,
   81,    0,   91,    0,    0,    0,    0,  114,   84,   86,
   88,   90,    0,    0,  118,    0,    0,    0,    0,  132,
    0,    0,  160,  158,    0,  140,    0,   82,  261,    0,
    0,  262,  263,  258,    0,  252,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  216,  220,    0,    0,  218,
  221,  222,    0,  117,  254,    0,    0,    0,    0,    0,
  167,    0,  127,    0,    0,  144,  161,  159,   92,    0,
    0,    2,    3,    0,    0,    0,    4,    5,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  210,    0,    0,   27,   28,   29,   30,  229,    7,    0,
    0,   78,    0,   33,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   63,  214,  215,  201,  202,
  203,  204,  205,  206,    0,  212,    1,    0,  189,   94,
  260,  251,  259,  155,    0,   17,    0,    0,  152,    0,
    0,    0,  141,    0,  223,  224,  225,    0,  217,  219,
  228,  255,  256,    0,    0,    0,    0,  171,    0,    0,
    0,   31,   80,  135,  129,  133,  128,    0,    0,    0,
   25,    0,   20,   21,    0,    0,    0,    0,    0,    0,
    0,    0,  241,  242,  243,    0,    0,    0,    0,    0,
    0,   22,   23,    0,  230,    0,   13,   14,    0,    0,
    0,   66,   67,   68,   69,   70,   71,   72,   73,   74,
   75,   76,   77,   65,    0,   24,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  211,  213,    0,    0,    0,
  192,    0,    0,  197,  154,    0,  153,    0,  151,  147,
    0,  146,    0,    0,  142,  227,    0,  185,    0,    0,
  179,    0,    0,    0,    0,    0,    0,  257,  166,  168,
  130,  207,    0,    0,    0,  209,    0,    0,    0,    0,
    0,    0,  240,  244,    6,    0,  123,  125,    0,    0,
  174,   79,   12,    9,    0,    0,   11,   64,   34,   35,
   36,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  200,  190,
    0,  193,  196,  198,   18,    0,    0,  150,  145,  226,
  186,  178,  183,  180,  170,  187,    0,  181,    0,    0,
    0,    0,  208,    0,    0,    0,    0,    0,    0,    0,
   32,   10,    8,    0,  199,  191,  194,    0,  148,  149,
  188,  184,  182,    0,  233,  234,    0,    0,    0,    0,
    0,    0,   62,  195,    0,    0,  238,    0,  236,    0,
   15,    0,  232,  235,  239,  237,   16,
  };
  protected static readonly short [] yyDgoto  = {            28,
  149,  150,  151,  185,  238,  290,  152,  153,  154,  155,
  156,  157,  158,  159,  160,  161,  162,  163,  164,  165,
  166,  265,  214,   29,   85,   52,   31,   32,   33,   34,
   53,   72,  291,   35,   36,   45,   37,   65,  241,   69,
   70,   55,   56,   57,   75,  309,  110,  111,  310,  209,
  292,  293,  294,  168,  169,  170,  171,  172,  173,  174,
  175,  176,   99,  100,  101,  102,  103,   38,   39,   40,
   87,   41,   88,
  };
  protected static readonly short [] yySindex = {         1989,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -84,    0, 1989,    0,   90,
 2954, 2954, 2954, 2954,    0,    0,  -61,    0,    0,    0,
  -35,    0,    0, -184,  -46,    0,    0,   43,    3, -166,
    0,   35,    0,  651,   26,   -9,  -94,    0,    0,    0,
    0,    0, 2825,  -10,    0, -103, 1891,  120,  -16,    0,
 -184,  148,    0,    0,    3,    0,   43,    0,    0,   72,
  448,    0,    0,    0,   90,    0, 2772, 2954,   -9,  752,
  844, -107,  135,  156,  180,    0,    0,  -33, 2923,    0,
    0,    0,  146,    0,    0,  -39,  125,  613,  220,  214,
    0, 1563,    0,  -96,   -2,    0,    0,    0,    0,  206,
  237,    0,    0, 1595, 1620, 1620,    0,    0, 1563,  251,
  229,  274,  276,  373,  293,   81,  291,  333,  -18,  960,
    0, 1563, 1563,    0,    0,    0,    0,    0,    0,  166,
  -32,    0,  487,    0, 1563,  372,   77, -215,  -44,  -41,
  374,  316,  307,  162,  -51,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  145,    0,    0, 1158,    0,    0,
    0,    0,    0,    0,   10,    0,  376,   18,    0,  521,
  368, 1150,    0,  208,    0,    0,    0, 1922,    0,    0,
    0,    0,    0, 1772, 1275,  422,   53,    0,   16,  125,
 2857,    0,    0,    0,    0,    0,    0,  -87,  373,  960,
    0,  960,    0,    0,  431,  373, 1563, 1563, 1563,  183,
 1036,  474,    0,    0,    0,  175,  241,  496, 1738, 1738,
  246,    0,    0, 1563,    0,  290,    0,    0, 1126, 1563,
  312,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1563,    0, 1563, 1563, 1563, 1563,
 1563, 1563, 1563, 1563, 1563, 1563, 1563, 1563, 1563, 1563,
 1563, 1563, 1563, 1563, 1563,    0,    0, 1563,  320,   20,
    0,  448,  538,    0,    0, 1563,    0, 1245,    0,    0,
 1563,    0, 1321,  469,    0,    0,  532,    0,  540,  544,
    0, 1332,  486, 1563,   16, 1958, 1357,    0,    0,    0,
    0,    0,  549,  554,  373,    0,  305,  441,  443,  560,
 1389, 1389,    0,    0,    0, 1434,    0,    0, 1859,   25,
    0,    0,    0,    0,  451,   28,    0,    0,    0,    0,
    0,  372,  372,   77,   77, -215, -215, -215, -215,  -44,
  -44,  -41,  374,  316,  307,  162,   31,  515,    0,    0,
   15,    0,    0,    0,    0,  517,  522,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  575,    0, 1448,  525,
  494,  494,    0,  373,  373,  373, 1563, 1456, 1476, 1158,
    0,    0,    0, 1563,    0,    0,    0,  448,    0,    0,
    0,    0,    0,  299,    0,    0,  453,  373,  459,  373,
  460,   82,    0,    0,  373,  561,    0,  373,    0,  373,
    0, 1047,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
 1825,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   60, 1352, 1485, 1639,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1682,    0,    0,    0,   27,    0,
    0,    0,    0,  188,    0,  552,  511,    0,    0,    0,
    0,    0,    0, 1713,    0,    0,    0,  118,    0,    0,
    0,    0,    0,    0,   46,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  610,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  512,    0,  582,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  188,
 2268,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2295,    0, 2363,    0,    0, 2391, 2580, 1831,  622,  563,
 2929,   40,  975,  295,  547,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  516,  524,    0,  545,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  211,  387,
  583,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  568,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  584,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2400, 2460, 2640, 2649, 2677, 2700, 2737, 2745, 2774,
 2833, 2896, 2935,   84, 1504, 1050,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2323,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  217,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -93,    0,  378,   34,  228,  -81,  849,   12,    0,
   50,   87,  147,   66,  357,  361,  356,  360,  355,    0,
 -108,    0,  -97,  495,    2,    0,    0,  358,  501,    0,
  569,  141,  -80,    0,    0,  611,    0,  585,  186,  579,
  -62,  -38,  -49,  -19,  -28,  -50,    0,  447,  -95, -181,
 -293,    0,  359, -131,    0,  -11, -157,    0,    0,    0,
    0,  484,    0,  562,    0,    0,    0,  634,    0,    0,
    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           179,
  180,   30,  230,  213,   67,   89,  198,  249,  186,  191,
   74,  285,  208,  251,  147,  276,  109,  277,  194,  142,
  213,  140,  106,  143,  144,  315,  145,  114,  215,   30,
   90,  225,   59,   60,   61,   62,  118,  321,   44,  187,
  235,  218,   86,   98,   49,  236,  237,  147,  272,  273,
  295,  216,  142,  296,  140,  316,  143,  144,  250,  145,
  289,   63,  192,  371,  339,   48,  156,  156,  108,  207,
  156,  244,   68,  332,  244,  182,   71,  408,   77,   98,
   55,   91,   48,   55,   49,  157,  157,  322,  404,  157,
   76,  108,  204,   78,  326,  203,  179,   55,   55,   83,
   83,   83,   55,   83,  147,  288,  317,  146,  113,  142,
  304,  140,   63,  143,  144,  205,  145,  156,   83,  270,
  403,  271,  217,  313,   56,  432,  237,   56,  237,   48,
  148,   49,   55,  327,  328,  329,  157,  178,  408,  406,
  146,   56,   56,  205,  370,  341,   56,  307,   51,  193,
   83,   50,  156,  242,  243,  216,  346,   89,  315,  298,
   68,  134,  342,   55,   55,  207,  266,  186,   50,   68,
   54,  157,   42,  398,  399,   92,   56,  147,   50,  213,
  112,  105,  142,  348,  140,   83,  143,  144,  116,  145,
  368,  367,  195,  393,   80,   42,  141,  146,  318,  108,
   43,  243,  340,  148,  201,  108,  431,   56,   56,  244,
  179,  372,  108,  196,  375,   50,  376,  120,  244,  377,
  284,  274,  275,   43,  245,  120,  278,  279,  246,  247,
  248,   93,  385,  334,   66,  390,   92,  197,  177,  122,
  123,  124,  134,  125,  126,  202,   93,   80,  206,  231,
  124,  124,  124,  323,  231,  324,  231,  211,  231,  231,
  210,  231,  414,  415,  416,  387,   81,   80,  227,  286,
  146,  177,  122,  123,  124,  231,  125,  126,  349,  350,
  351,  335,   47,  156,  244,  339,  427,   49,  429,  179,
  407,  127,  128,  433,  219,  423,  435,    8,  436,   47,
  340,  124,  157,  417,  419,  421,   17,   18,  226,   47,
   55,   55,    8,  228,  243,  229,   83,  108,  179,  352,
  353,   17,   18,  243,  127,  128,  179,  424,  121,  122,
  123,  124,  231,  125,  126,   59,  205,  232,   59,  231,
  108,  231,  231,  360,  361,  394,   47,  401,  244,  233,
  179,  407,   59,   59,   56,   56,   58,   59,  354,  355,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,  127,  128,   23,   24,   25,   26,   59,  129,  130,
  131,  234,  132,  133,  134,  135,  136,  137,  138,  139,
  243,  121,  122,  123,  124,  147,  125,  126,  269,  281,
  142,  280,  140,  267,  143,  144,  297,  145,  268,   59,
  356,  357,  358,  359,  337,  338,  126,  126,  126,   58,
  282,  148,  283,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,  127,  128,   23,   24,   25,   26,
  300,  129,  130,  131,  305,  132,  133,  134,  135,  136,
  137,  138,  139,  231,  231,  231,  231,  126,  231,  231,
  147,  395,  314,  396,  244,  142,  244,  140,  325,  143,
  144,  402,  145,  426,  296,   80,  244,  239,  146,  428,
  430,  231,  244,  244,  330,  231,  231,  231,  231,  231,
  231,  231,  231,  231,  231,  231,  231,  231,  231,  231,
  231,  231,  231,  231,  231,  231,  231,  231,  231,  231,
  231,  231,  333,  231,  231,  231,  336,  231,  231,  231,
  231,  231,  231,  231,  231,  143,  343,  264,   84,   73,
  143,  143,  172,  147,  143,  172,  169,   97,  142,  169,
  140,  379,  143,  144,  175,  145,   59,  175,  347,  143,
  178,  143,  380,  146,  167,  117,  369,  239,  384,  239,
  381,  181,  183,  289,  382,  176,  138,   61,  176,  391,
   61,   73,  138,   97,  392,  138,  239,  239,  373,  397,
   51,  143,  143,   51,   61,   61,   51,  405,  177,  409,
  138,  177,  138,  299,  410,  411,  400,  413,  425,  434,
   51,   51,  165,  173,  175,   51,  345,  422,  288,  121,
  122,  123,  124,  143,  125,  126,  362,  364,  366,   61,
  240,  363,  365,  138,  137,  119,  146,   64,  104,  115,
  137,  374,  204,  137,   49,   51,   51,  320,  287,   48,
  200,   46,   48,    0,    0,   48,    0,    0,  137,  167,
  137,   61,    0,    0,  138,    0,    0,    0,    0,   48,
   48,    0,  127,  128,   48,   82,   51,   51,   73,  129,
  130,  131,  117,  132,  133,  134,  135,  136,  137,  138,
  139,  137,    0,  205,  177,  122,  123,  124,    0,  125,
  126,   81,    0,    0,   48,   48,    0,    0,    0,    0,
  240,    0,  240,    0,    0,  331,    0,    0,    0,    0,
    0,    0,  137,    0,    0,    0,    0,    0,   50,  240,
  240,    0,   83,    0,    0,   48,   48,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  127,  128,  252,
  253,  254,  255,  256,  257,  258,  259,  260,  261,  262,
  263,    0,    0,   80,    0,    0,    0,  177,  122,  123,
  124,    0,  125,  126,  147,    0,    0,    0,    0,  142,
    0,  140,  184,  143,  144,  143,  145,    0,  117,  143,
  143,  143,  143,  143,  143,  143,  143,  143,  143,  143,
  143,  143,  143,  143,  143,  143,  143,  143,  143,  143,
    0,    0,  143,  143,  143,  143,    0,    0,    0,    0,
  127,  128,    0,   51,   51,    0,  138,    0,    0,  143,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,    0,    0,  138,  138,  138,  138,    0,    0,   47,
    0,    0,    0,    0,    0,    0,  147,  146,    0,    0,
  138,  142,    0,  140,    0,  190,  144,    0,  145,   48,
   48,    0,   48,   48,  137,    0,    0,    0,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,    0,
    0,  137,  137,  137,  137,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   58,  189,    0,  137,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
  212,    0,   23,   24,   25,   26,    0,    0,    0,  146,
    0,    0,  221,  223,  224,    0,    0,  212,    0,   79,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  212,  212,  147,    0,    0,    0,    0,  142,    0,  140,
    0,  143,  144,  212,  145,    0,    0,    0,  177,  122,
  123,  124,    0,  125,  126,   57,    0,    0,   57,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   57,   57,    0,    0,   58,   57,  212,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,  127,  128,   23,   24,   25,   26,   57,  147,    0,
    0,    0,    0,  142,    0,  140,    0,  143,  144,  147,
  145,    0,    0,    0,  142,  146,  140,    0,  143,  144,
   60,  145,  289,   60,  148,    0,    0,    0,    0,   57,
  177,  122,  123,  124,    0,  125,  126,   60,   60,    0,
    0,    0,   60,    0,    0,  212,  212,  212,  212,  212,
  212,  212,  212,  212,  212,  212,  212,  212,  212,  212,
  212,  212,  212,    0,  188,    0,  212,  288,    8,    0,
    0,    0,   60,    0,    0,    0,    0,   17,   18,    0,
    0,  212,    0,  127,  128,    0,    0,    0,  147,    0,
  212,  146,    0,  142,    0,  140,  344,  143,  144,  178,
  145,  437,  146,    0,   60,    0,    0,    0,    0,    0,
    0,    0,  147,    0,  212,    0,    0,  142,    0,  140,
  147,  303,  144,    0,  145,  142,    0,  140,    0,  143,
  144,    0,  145,  289,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  177,  122,  123,  124,
    0,  125,  126,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  212,    0,    0,
    0,    0,  302,    0,   58,   57,   57,    0,  288,    0,
    0,  146,  212,    0,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,  127,
  128,   23,   24,   25,   26,  146,    0,  147,    0,    0,
  178,    0,  142,  146,  140,    0,  143,  144,    0,  145,
    0,    0,  177,  122,  123,  124,    0,  125,  126,    0,
    0,    0,    0,  177,  122,  123,  124,  147,  125,  126,
    0,    0,  142,    0,  140,    0,  312,  144,    0,  145,
   58,   60,    0,    0,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,  127,  128,   23,   24,   25,
   26,    0,    0,  147,    0,    0,  127,  128,  142,    0,
  140,    0,  143,  144,  147,  145,    0,  311,    0,  142,
  146,  140,    0,  143,  144,    0,  145,    0,    0,    0,
    0,    0,  177,  122,  123,  124,    0,  125,  126,  147,
    0,   85,   85,   85,  142,   85,  140,    0,  389,  144,
  146,  145,    0,    0,    0,    0,  177,  122,  123,  124,
   85,  125,  126,  378,  177,  122,  123,  124,    0,  125,
  126,  147,    0,    0,  383,    0,  142,    0,  140,    0,
  143,  144,    0,  145,    0,  127,  128,    0,    0,    0,
  301,    0,   85,    0,    8,    0,  146,  148,    0,  388,
    0,    0,    0,   17,   18,    0,    0,  146,    0,  127,
  128,    0,    0,    0,    0,    0,  147,  127,  128,    0,
    0,  142,    0,  140,    0,  143,  144,   85,  145,    0,
  147,    0,  146,    0,    0,  142,    0,  140,  147,  143,
  144,    0,  145,  142,    0,  140,  418,  143,  144,    0,
  145,  177,  122,  123,  124,    0,  125,  126,  147,    0,
    0,    0,    0,  142,  146,  140,  420,  143,  144,    0,
  145,    0,    0,    0,   87,   87,   87,    0,   87,    0,
    0,  177,  122,  123,  124,    0,  125,  126,    0,    8,
  412,    0,    0,   87,   58,    0,    0,   58,   17,   18,
    0,    0,    0,    0,  127,  128,  400,    0,    0,  146,
    0,   58,   58,    0,    0,    0,   58,    0,    0,    0,
    0,    0,    0,  146,    0,   87,    0,  177,  122,  123,
  124,  146,  125,  126,  127,  128,    0,    0,  177,  122,
  123,  124,    0,  125,  126,  147,   58,    0,    0,    0,
  142,  146,  140,    0,  143,  144,    0,  145,   85,    0,
   87,    0,    0,  177,  122,  123,  124,    0,  125,  126,
    0,    0,    0,    0,    0,    0,    0,  147,   58,    0,
  127,  128,  142,    0,  220,    0,  143,  144,    0,  145,
    0,  127,  128,    0,    0,  177,  122,  123,  124,    0,
  125,  126,  147,    0,    0,    0,    0,  142,    0,  222,
    0,  143,  144,    0,  145,    0,  127,  128,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   89,   89,
   89,    0,   89,    0,    0,    0,    0,    0,  146,    0,
  177,  122,  123,  124,    0,  125,  126,   89,  127,  128,
    0,    0,    0,    0,  177,  122,  123,  124,    0,  125,
  126,    0,  177,  122,  123,  124,    0,  125,  126,    0,
  146,  131,  131,  131,    0,  131,    0,    0,    0,   89,
    0,    0,  177,  122,  123,  124,    0,  125,  126,    0,
  131,   87,    0,  127,  128,  146,    0,    0,    0,    0,
    0,    0,  119,  119,  119,    0,  119,  127,  128,    0,
    0,    0,    0,    0,   89,  127,  128,    0,    0,    0,
    0,  119,  131,    0,   58,   58,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  127,  128,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  119,    0,    0,    0,  131,    0,    0,
    0,  204,  308,   49,    0,    0,    0,    0,    0,  177,
  122,  123,  124,    0,  125,  126,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  119,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  177,  122,  123,  124,    0,  125,  126,    0,    0,
    0,    0,  205,    0,  114,    0,  114,    0,   43,    0,
    0,   43,  127,  128,   43,    0,  177,  122,  123,  124,
    0,  125,  126,  114,    0,    0,    0,    0,   43,   43,
   43,    0,   43,   43,    0,   89,    0,   50,  339,  308,
   49,    0,    0,    0,  127,  128,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   43,   43,    0,    0,    0,    0,  127,
  128,  107,    0,    0,    0,    0,    0,    0,  131,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  205,
  114,    0,    0,    0,   43,   43,    0,    0,    0,    0,
    0,    0,  306,    0,    0,    0,  131,    0,    0,  119,
  131,  131,  131,  131,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,  131,  131,  131,  131,  131,  131,
  131,    0,    0,  131,  131,  131,  131,  119,  386,    0,
    0,  119,  119,  119,  119,  119,  119,  119,  119,  119,
  119,  119,  119,  119,  119,  119,  119,  119,  119,  119,
  119,  119,   58,    0,  119,  119,  119,  119,   47,    0,
    0,    0,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   27,    0,   23,
   24,   25,   26,    0,    0,    0,   58,    0,    0,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,  114,    0,   23,   24,   25,   26,    0,    0,    0,
    0,    0,    0,    0,  253,    0,   43,   43,   43,   43,
    0,   43,   43,    0,    0,    0,    0,    0,    0,  114,
    0,    0,    0,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,    0,    0,  114,  114,  114,  114,
    0,    0,    0,   58,    0,    0,    0,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,    0,   58,    0,    0,    0,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,   58,    0,    0,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,    0,
    0,    0,   58,    0,    0,    0,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,    0,    0,   23,
   24,   25,   26,    1,    0,    0,    0,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,    1,    1,    0,    1,    0,    1,
    1,    1,    1,    1,    1,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    1,    1,    1,    1,
    1,   19,   19,    0,    0,   19,   19,   19,   19,   19,
    0,   19,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   19,   19,   19,   19,   19,   19,    1,   26,
   26,    1,    0,   26,   26,   26,   26,   26,    0,   26,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   26,   26,   26,   26,   26,   26,    0,   19,   19,    0,
    0,    1,    0,    0,    0,    0,    0,    0,    0,   31,
   31,    0,    0,   31,   31,   31,   31,   31,    0,   31,
    0,    0,    0,    0,    0,   26,   26,    0,   19,   19,
   31,   31,   31,    0,   31,   31,    0,    0,   37,    0,
    0,   37,    0,   37,   37,   37,    0,   38,    0,    0,
   38,    0,   38,   38,   38,    0,   26,   26,   37,   37,
   37,    0,   37,   37,    0,   31,   31,   38,   38,   38,
    0,   38,   38,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   37,   37,    0,   31,   31,    0,    0,
    0,    0,   38,   38,    0,    0,    0,   39,    0,    0,
   39,    0,   39,   39,   39,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   37,   37,    0,   39,   39,   39,
    0,   39,   39,   38,   38,    0,    0,    0,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    0,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,   39,   39,    0,    0,    0,    0,   19,   19,
   19,   19,   19,   19,    0,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,   19,    0,
    0,    0,    0,   39,   39,    0,   26,   26,   26,   26,
   26,   26,    0,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   40,    0,    0,
   40,    0,    0,   40,    0,    0,   31,   31,   31,   31,
   31,   31,    0,   31,   31,    0,    0,   40,   40,   40,
    0,   40,   40,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   37,   37,   37,   37,   37,   37,
    0,   37,   37,   38,   38,   38,   38,   38,   38,    0,
   38,   38,   40,   40,    0,    0,    0,   41,    0,    0,
   41,    0,    0,   41,    0,    0,   42,    0,    0,   42,
    0,    0,   42,    0,    0,    0,    0,   41,   41,   41,
    0,   41,   41,   40,   40,    0,   42,   42,   42,    0,
   42,   42,    0,    0,   46,    0,    0,   46,    0,    0,
   46,    0,    0,   39,   39,   39,   39,   39,   39,    0,
   39,   39,   41,   41,   46,   46,   46,   47,   46,   46,
   47,   42,   42,   47,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   47,   47,   47,
    0,   47,   47,   41,   41,    0,    0,    0,    0,   46,
   46,    0,   42,   42,   44,    0,    0,   44,    0,    0,
   44,    0,   45,    0,    0,   45,    0,    0,   45,    0,
    0,    0,   47,   47,   44,   44,   44,    0,   44,   44,
   46,   46,   45,   45,   45,    0,   45,   45,    0,    0,
    0,   49,    0,    0,   49,    0,    0,   49,    0,    0,
    0,    0,    0,   47,   47,    0,    0,    0,    0,   44,
   44,   49,   49,    0,    0,    0,   49,   45,   45,    0,
    0,    0,    0,   40,   40,   40,   40,   40,   40,    0,
   40,   40,    0,    0,    0,    0,    0,    0,    0,    0,
   44,   44,    0,    0,    0,    0,   49,   49,   45,   45,
   50,    0,    0,   50,    0,    0,   50,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   50,   50,    0,    0,   80,   50,    0,   49,   49,    0,
    0,    0,    0,   41,   41,   41,   41,   41,   41,    0,
   41,   41,   42,   42,   42,   42,   42,   42,    0,   42,
   42,    0,    0,    0,    0,   50,   50,    0,    0,    0,
    0,    0,    0,   52,    0,    0,   52,    0,    0,   52,
    0,    0,   46,   46,   46,   46,    0,   46,   46,   96,
   50,    0,    0,   52,   52,    0,   50,   50,   52,    0,
    0,    0,    0,    0,    0,   47,   47,   47,   47,   53,
   47,   47,   53,    0,    0,   54,    0,    0,   54,    0,
    0,    0,    0,    0,    0,    0,   53,   53,   52,   52,
    0,   53,   54,   54,    0,    0,    0,   54,    0,    0,
    0,    0,   44,   44,   44,   44,    0,   44,   44,    0,
   45,   45,   45,   45,    0,   45,   45,    0,    0,   52,
   52,   53,   53,    0,    0,    0,    0,   54,   54,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   49,   49,    0,   49,   49,    0,  199,   50,    0,
    0,    0,   53,   53,    0,    0,   58,    0,   54,   54,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   47,    0,   23,   24,   25,   26,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   50,   50,    0,   50,   50,    0,    0,    0,    0,   58,
   93,   94,   95,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,    0,    0,   23,   24,   25,   26,
    0,   58,    0,    0,    0,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   52,   52,   23,   24,
   25,   26,  319,    0,    0,    0,    0,    0,    0,   47,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   53,
   53,    0,    0,    0,    0,   54,   54,   58,   93,   94,
   95,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,   58,    0,
    0,    0,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,
  };
  protected static readonly short [] yyCheck = {            81,
   81,    0,  134,  112,   40,   55,   40,   40,   90,   91,
   49,   63,  108,   46,   33,   60,   67,   62,  126,   38,
  129,   40,  126,   42,   43,  207,   45,   44,  125,   28,
   40,  129,   31,   32,   33,   34,   75,  125,  123,   90,
   59,   44,   54,   63,   42,  139,  140,   33,  264,  265,
   41,  114,   38,   44,   40,   40,   42,   43,   91,   45,
   46,  123,   91,   44,   40,   40,   40,   41,   67,  108,
   44,   44,  257,  231,   44,   87,  123,  371,   44,   99,
   41,   91,   40,   44,   42,   40,   41,  219,   58,   44,
  257,   90,   40,   59,  226,  107,  178,   58,   59,   40,
   41,   42,   63,   44,   33,   91,   91,  126,  125,   38,
  192,   40,  123,   42,   43,   91,   45,   91,   59,   43,
   93,   45,  125,  205,   41,   44,  220,   44,  222,   40,
   59,   42,   93,  227,  228,  229,   91,  123,  432,  125,
  126,   58,   59,   91,  125,  241,   63,  198,   59,  257,
   91,  126,  126,  142,  143,  218,  250,  207,  340,  188,
  257,   44,  244,  124,  125,  204,  155,  249,  126,  257,
   30,  126,  257,  331,  332,  270,   93,   33,  126,  288,
   61,  285,   38,  265,   40,  126,   42,   43,   41,   45,
  288,  285,   58,  325,  123,  257,  125,  126,  210,  198,
  285,  190,  241,   59,   59,  204,  125,  124,  125,   44,
  292,  292,  211,   58,  296,  126,  298,   77,   44,  301,
  272,  266,  267,  285,   59,   85,  268,  269,  261,  262,
  263,   44,  314,   59,  270,  317,  270,   58,  257,  258,
  259,  260,  125,  262,  263,  285,   59,  123,  108,   33,
   40,   41,   42,  220,   38,  222,   40,   44,   42,   43,
   41,   45,  394,  395,  396,  316,   61,  123,   40,  125,
  126,  257,  258,  259,  260,   59,  262,  263,  267,  268,
  269,   41,  257,  257,   44,   40,  418,   42,  420,  371,
  371,  310,  311,  425,   58,  404,  428,  295,  430,  257,
  339,   91,  257,  397,  398,  399,  304,  305,   58,  257,
  271,  272,  295,   40,  303,   40,  257,  316,  400,  270,
  271,  304,  305,  312,  310,  311,  408,  408,  257,  258,
  259,  260,   40,  262,  263,   41,   91,  257,   44,  123,
  339,  125,  126,  278,  279,   41,  257,  336,   44,   59,
  432,  432,   58,   59,  271,  272,  285,   63,  272,  273,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  313,  314,  315,   93,  317,  318,
  319,   59,  321,  322,  323,  324,  325,  326,  327,  328,
  389,  257,  258,  259,  260,   33,  262,  263,   37,   94,
   38,   38,   40,   42,   42,   43,   41,   45,   47,  125,
  274,  275,  276,  277,  239,  240,   40,   41,   42,  285,
  124,   59,  271,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  313,  314,  315,
   93,  317,  318,  319,  257,  321,  322,  323,  324,  325,
  326,  327,  328,  257,  258,  259,  260,   91,  262,  263,
   33,   41,   61,   41,   44,   38,   44,   40,   58,   42,
   43,   41,   45,   41,   44,  123,   44,  140,  126,   41,
   41,  285,   44,   44,  322,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  313,
  314,  315,   59,  317,  318,  319,   41,  321,  322,  323,
  324,  325,  326,  327,  328,   35,  257,   61,   54,   49,
   40,   41,   41,   33,   44,   44,   41,   63,   38,   44,
   40,   93,   42,   43,   41,   45,  272,   44,  257,   59,
  123,   61,   41,  126,   80,   75,  257,  220,   93,  222,
   41,   87,   88,   46,   41,   41,   35,   41,   44,   41,
   44,   91,   41,   99,   41,   44,  239,  240,   61,   40,
   38,   91,   92,   41,   58,   59,   44,   93,   41,   93,
   59,   44,   61,   93,   93,   41,  123,   93,  320,   59,
   58,   59,   41,   41,   41,   63,  249,  400,   91,  257,
  258,  259,  260,  123,  262,  263,  280,  282,  284,   93,
  140,  281,  283,   92,   35,   77,  126,   37,   64,   71,
   41,  293,   40,   44,   42,   93,   94,  211,  175,   38,
   99,   28,   41,   -1,   -1,   44,   -1,   -1,   59,  175,
   61,  125,   -1,   -1,  123,   -1,   -1,   -1,   -1,   58,
   59,   -1,  310,  311,   63,   35,  124,  125,  188,  317,
  318,  319,  192,  321,  322,  323,  324,  325,  326,  327,
  328,   92,   -1,   91,  257,  258,  259,  260,   -1,  262,
  263,   61,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
  220,   -1,  222,   -1,   -1,  231,   -1,   -1,   -1,   -1,
   -1,   -1,  123,   -1,   -1,   -1,   -1,   -1,  126,  239,
  240,   -1,   92,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  310,  311,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   -1,   -1,  123,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   41,   42,   43,  285,   45,   -1,  298,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
   -1,   -1,  312,  313,  314,  315,   -1,   -1,   -1,   -1,
  310,  311,   -1,  271,  272,   -1,  285,   -1,   -1,  329,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,   -1,   -1,  312,  313,  314,  315,   -1,   -1,  257,
   -1,   -1,   -1,   -1,   -1,   -1,   33,  126,   -1,   -1,
  329,   38,   -1,   40,   -1,   42,   43,   -1,   45,  268,
  269,   -1,  271,  272,  285,   -1,   -1,   -1,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,   -1,
   -1,  312,  313,  314,  315,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  285,   93,   -1,  329,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  112,   -1,  312,  313,  314,  315,   -1,   -1,   -1,  126,
   -1,   -1,  124,  125,  126,   -1,   -1,  129,   -1,  329,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  142,  143,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,  155,   45,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   58,   59,   -1,   -1,  285,   63,  190,   -1,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  313,  314,  315,   93,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   33,
   45,   -1,   -1,   -1,   38,  126,   40,   -1,   42,   43,
   41,   45,   46,   44,   59,   -1,   -1,   -1,   -1,  125,
  257,  258,  259,  260,   -1,  262,  263,   58,   59,   -1,
   -1,   -1,   63,   -1,   -1,  267,  268,  269,  270,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   -1,  291,   -1,  288,   91,  295,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,  304,  305,   -1,
   -1,  303,   -1,  310,  311,   -1,   -1,   -1,   33,   -1,
  312,  126,   -1,   38,   -1,   40,   41,   42,   43,  123,
   45,  125,  126,   -1,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   33,   -1,  336,   -1,   -1,   38,   -1,   40,
   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   46,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  389,   -1,   -1,
   -1,   -1,   93,   -1,  285,  271,  272,   -1,   91,   -1,
   -1,  126,  404,   -1,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  313,  314,  315,  126,   -1,   33,   -1,   -1,
  123,   -1,   38,  126,   40,   -1,   42,   43,   -1,   45,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   33,  262,  263,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
  285,  272,   -1,   -1,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,   -1,   -1,   33,   -1,   -1,  310,  311,   38,   -1,
   40,   -1,   42,   43,   33,   45,   -1,   93,   -1,   38,
  126,   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   33,
   -1,   40,   41,   42,   38,   44,   40,   -1,   42,   43,
  126,   45,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   59,  262,  263,   93,  257,  258,  259,  260,   -1,  262,
  263,   33,   -1,   -1,   93,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,  310,  311,   -1,   -1,   -1,
  291,   -1,   91,   -1,  295,   -1,  126,   59,   -1,   93,
   -1,   -1,   -1,  304,  305,   -1,   -1,  126,   -1,  310,
  311,   -1,   -1,   -1,   -1,   -1,   33,  310,  311,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,  126,   45,   -1,
   33,   -1,  126,   -1,   -1,   38,   -1,   40,   33,   42,
   43,   -1,   45,   38,   -1,   40,   41,   42,   43,   -1,
   45,  257,  258,  259,  260,   -1,  262,  263,   33,   -1,
   -1,   -1,   -1,   38,  126,   40,   41,   42,   43,   -1,
   45,   -1,   -1,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,  295,
   93,   -1,   -1,   59,   41,   -1,   -1,   44,  304,  305,
   -1,   -1,   -1,   -1,  310,  311,  123,   -1,   -1,  126,
   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,
   -1,   -1,   -1,  126,   -1,   91,   -1,  257,  258,  259,
  260,  126,  262,  263,  310,  311,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   33,   93,   -1,   -1,   -1,
   38,  126,   40,   -1,   42,   43,   -1,   45,  257,   -1,
  126,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,  125,   -1,
  310,  311,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   -1,  310,  311,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,  310,  311,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   40,   41,
   42,   -1,   44,   -1,   -1,   -1,   -1,   -1,  126,   -1,
  257,  258,  259,  260,   -1,  262,  263,   59,  310,  311,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  126,   40,   41,   42,   -1,   44,   -1,   -1,   -1,   91,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   59,  257,   -1,  310,  311,  126,   -1,   -1,   -1,   -1,
   -1,   -1,   40,   41,   42,   -1,   44,  310,  311,   -1,
   -1,   -1,   -1,   -1,  126,  310,  311,   -1,   -1,   -1,
   -1,   59,   91,   -1,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  310,  311,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   91,   -1,   -1,   -1,  126,   -1,   -1,
   -1,   40,   41,   42,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   91,   -1,   40,   -1,   42,   -1,   38,   -1,
   -1,   41,  310,  311,   44,   -1,  257,  258,  259,  260,
   -1,  262,  263,   59,   -1,   -1,   -1,   -1,   58,   59,
   60,   -1,   62,   63,   -1,  257,   -1,  126,   40,   41,
   42,   -1,   -1,   -1,  310,  311,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,  310,
  311,   41,   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   91,
  126,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   -1,   -1,  285,   -1,   -1,  257,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,   -1,   -1,  312,  313,  314,  315,  285,   41,   -1,
   -1,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  285,   -1,  312,  313,  314,  315,  257,   -1,
   -1,   -1,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,   59,   -1,  312,
  313,  314,  315,   -1,   -1,   -1,  285,   -1,   -1,   -1,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  257,   -1,  312,  313,  314,  315,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  270,   -1,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,  285,
   -1,   -1,   -1,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,   -1,   -1,  312,  313,  314,  315,
   -1,   -1,   -1,  285,   -1,   -1,   -1,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,   -1,   -1,
  312,  313,  314,  315,   -1,  285,   -1,   -1,   -1,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
   -1,   -1,  312,  313,  314,  315,  285,   -1,   -1,   -1,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,   -1,   -1,  312,  313,  314,  315,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   -1,   -1,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,   -1,   -1,  312,
  313,  314,  315,  285,   -1,   -1,   -1,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,   -1,   -1,
  312,  313,  314,  315,   37,   38,   -1,   40,   -1,   42,
   43,   44,   45,   46,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   63,   37,   38,   -1,   -1,   41,   42,   43,   44,   45,
   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   58,   59,   60,   61,   62,   63,   91,   37,
   38,   94,   -1,   41,   42,   43,   44,   45,   -1,   47,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   61,   62,   63,   -1,   93,   94,   -1,
   -1,  124,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   37,
   38,   -1,   -1,   41,   42,   43,   44,   45,   -1,   47,
   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,  125,
   58,   59,   60,   -1,   62,   63,   -1,   -1,   38,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   38,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,  124,  125,   58,   59,
   60,   -1,   62,   63,   -1,   93,   94,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   94,   -1,  124,  125,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   58,   59,   60,
   -1,   62,   63,  124,  125,   -1,   -1,   -1,  261,  262,
  263,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
  273,  274,  275,  276,  277,  278,  279,  280,  281,  282,
  283,  284,   93,   94,   -1,   -1,   -1,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,  282,  283,  284,   -1,
   -1,   -1,   -1,  124,  125,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,   93,   94,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   58,   59,   60,
   -1,   62,   63,  124,  125,   -1,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,   93,   94,   58,   59,   60,   38,   62,   63,
   41,   93,   94,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,
   -1,   62,   63,  124,  125,   -1,   -1,   -1,   -1,   93,
   94,   -1,  124,  125,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   93,   94,   58,   59,   60,   -1,   62,   63,
  124,  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,
   94,   58,   59,   -1,   -1,   -1,   63,   93,   94,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   93,   94,  124,  125,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   -1,   -1,  123,   63,   -1,  124,  125,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,
   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,  125,
  126,   -1,   -1,   58,   59,   -1,  124,  125,   63,   -1,
   -1,   -1,   -1,   -1,   -1,  266,  267,  268,  269,   41,
  271,  272,   44,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   93,   94,
   -1,   63,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,  124,
  125,   93,   94,   -1,   -1,   -1,   -1,   93,   94,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  268,  269,   -1,  271,  272,   -1,  125,  126,   -1,
   -1,   -1,  124,  125,   -1,   -1,  285,   -1,  124,  125,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  257,   -1,  312,  313,  314,  315,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,   -1,   -1,  312,  313,  314,  315,
   -1,  285,   -1,   -1,   -1,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  271,  272,  312,  313,
  314,  315,  316,   -1,   -1,   -1,   -1,   -1,   -1,  257,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  271,
  272,   -1,   -1,   -1,   -1,  271,  272,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,   -1,   -1,  312,  313,  314,  315,  285,   -1,
   -1,   -1,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,   -1,   -1,  312,  313,  314,  315,
  };

#line 1095 "CParser.jay"

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
			 //Console.Error.WriteLine (s);
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
public class TokenKind {
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
  public const int COLONCOLON = 270;
  public const int AND_OP = 271;
  public const int OR_OP = 272;
  public const int MUL_ASSIGN = 273;
  public const int DIV_ASSIGN = 274;
  public const int MOD_ASSIGN = 275;
  public const int ADD_ASSIGN = 276;
  public const int SUB_ASSIGN = 277;
  public const int LEFT_ASSIGN = 278;
  public const int RIGHT_ASSIGN = 279;
  public const int BINARY_AND_ASSIGN = 280;
  public const int BINARY_XOR_ASSIGN = 281;
  public const int BINARY_OR_ASSIGN = 282;
  public const int AND_ASSIGN = 283;
  public const int OR_ASSIGN = 284;
  public const int TYPE_NAME = 285;
  public const int PUBLIC = 286;
  public const int PRIVATE = 287;
  public const int PROTECTED = 288;
  public const int TYPEDEF = 289;
  public const int EXTERN = 290;
  public const int STATIC = 291;
  public const int AUTO = 292;
  public const int REGISTER = 293;
  public const int INLINE = 294;
  public const int RESTRICT = 295;
  public const int CHAR = 296;
  public const int SHORT = 297;
  public const int INT = 298;
  public const int LONG = 299;
  public const int SIGNED = 300;
  public const int UNSIGNED = 301;
  public const int FLOAT = 302;
  public const int DOUBLE = 303;
  public const int CONST = 304;
  public const int VOLATILE = 305;
  public const int VOID = 306;
  public const int BOOL = 307;
  public const int COMPLEX = 308;
  public const int IMAGINARY = 309;
  public const int TRUE = 310;
  public const int FALSE = 311;
  public const int STRUCT = 312;
  public const int CLASS = 313;
  public const int UNION = 314;
  public const int ENUM = 315;
  public const int ELLIPSIS = 316;
  public const int CASE = 317;
  public const int DEFAULT = 318;
  public const int IF = 319;
  public const int ELSE = 320;
  public const int SWITCH = 321;
  public const int WHILE = 322;
  public const int DO = 323;
  public const int FOR = 324;
  public const int GOTO = 325;
  public const int CONTINUE = 326;
  public const int BREAK = 327;
  public const int RETURN = 328;
  public const int EOL = 329;
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
