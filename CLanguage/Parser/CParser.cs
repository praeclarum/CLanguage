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
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class identifier_or_typename ':' base_specifier_list class_body",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class class_body",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class identifier_or_typename",
//t    "base_specifier_list : base_specifier",
//t    "base_specifier_list : base_specifier_list ',' base_specifier",
//t    "base_specifier : identifier_or_typename",
//t    "base_specifier : PUBLIC identifier_or_typename",
//t    "base_specifier : PRIVATE identifier_or_typename",
//t    "base_specifier : PROTECTED identifier_or_typename",
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
//t    "class_block_item : VIRTUAL declaration",
//t    "class_block_item : declaration_specifiers init_declarator_list OVERRIDE ';'",
//t    "class_block_item : VIRTUAL declaration_specifiers init_declarator_list OVERRIDE ';'",
//t    "class_block_item : VIRTUAL pure_virtual_declaration",
//t    "pure_virtual_declaration : declaration_specifiers declarator '=' CONSTANT ';'",
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
//t    "selection_statement : SWITCH '(' expression ')' '{' switch_cases '}'",
//t    "selection_statement : SWITCH '(' expression ')' '{' '}'",
//t    "switch_cases : switch_case",
//t    "switch_cases : switch_cases switch_case",
//t    "switch_case : CASE constant_expression ':' block_item_list",
//t    "switch_case : DEFAULT ':' block_item_list",
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
    "VIRTUAL","OVERRIDE","TYPEDEF","EXTERN","STATIC","AUTO","REGISTER",
    "INLINE","RESTRICT","CHAR","SHORT","INT","LONG","SIGNED","UNSIGNED",
    "FLOAT","DOUBLE","CONST","VOLATILE","VOID","BOOL","COMPLEX",
    "IMAGINARY","TRUE","FALSE","STRUCT","CLASS","UNION","ENUM","ELLIPSIS",
    "CASE","DEFAULT","IF","ELSE","SWITCH","WHILE","DO","FOR","GOTO",
    "CONTINUE","BREAK","RETURN","EOL",
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
#line 47 "CParser.jay"
  { var t = lexer.CurrentToken; yyVal = new VariableExpression((yyVals[0+yyTop]).ToString(), t.Location, t.EndLocation); }
  break;
case 2:
#line 48 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 49 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 50 "CParser.jay"
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 51 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 52 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 59 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 63 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 67 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 71 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 75 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 79 "CParser.jay"
  {
		yyVal = new MemberFromPointerExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 13:
#line 83 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 87 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 91 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 95 "CParser.jay"
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
#line 117 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 121 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 125 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 129 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 133 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 137 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 141 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 145 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 149 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 150 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 151 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 152 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 159 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 163 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 170 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 174 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 178 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 182 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 190 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 194 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 202 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 206 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 214 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 218 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 222 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 226 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 234 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 238 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 246 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 254 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 262 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 270 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 278 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 285 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 289 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 314 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 315 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 316 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 317 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 318 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 319 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 320 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 321 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 322 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 323 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 324 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 325 "CParser.jay"
  { yyVal = LogicOp.And; }
  break;
case 77:
#line 326 "CParser.jay"
  { yyVal = LogicOp.Or; }
  break;
case 78:
#line 333 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 79:
#line 337 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 81:
#line 348 "CParser.jay"
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
#line 427 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[0+yyTop], null);
	}
  break;
case 94:
#line 431 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[-2+yyTop], (Initializer)yyVals[0+yyTop]);
	}
  break;
case 95:
#line 435 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 96:
#line 436 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 97:
#line 437 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 98:
#line 438 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 99:
#line 439 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 100:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 101:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 102:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 103:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 104:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 105:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 106:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 107:
#line 450 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 108:
#line 451 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 109:
#line 452 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 110:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 111:
#line 454 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 114:
#line 457 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 117:
#line 466 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 118:
  case_118();
  break;
case 119:
#line 473 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 120:
#line 474 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 121:
#line 481 "CParser.jay"
  {
		yyVal = new List<BaseSpecifier> { (BaseSpecifier)yyVals[0+yyTop] };
	}
  break;
case 122:
  case_122();
  break;
case 123:
#line 490 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString()); }
  break;
case 124:
#line 491 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Public); }
  break;
case 125:
#line 492 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Private); }
  break;
case 126:
#line 493 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Protected); }
  break;
case 127:
#line 497 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 128:
#line 498 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 129:
#line 499 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
  break;
case 130:
  case_130();
  break;
case 131:
  case_131();
  break;
case 132:
  case_132();
  break;
case 133:
  case_133();
  break;
case 134:
#line 528 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 135:
#line 529 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 136:
#line 530 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 137:
#line 531 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 138:
#line 532 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 139:
  case_139();
  break;
case 140:
  case_140();
  break;
case 141:
#line 554 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 142:
#line 558 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 143:
#line 562 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 144:
#line 569 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 145:
#line 570 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 146:
#line 578 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 147:
#line 582 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("~" + (yyVals[-1+yyTop]).ToString());
	}
  break;
case 148:
#line 584 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 149:
#line 586 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[-1+yyTop]).ToString()); }
  break;
case 151:
  case_151();
  break;
case 152:
#line 606 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 153:
#line 610 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 154:
#line 614 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 155:
#line 618 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 156:
#line 622 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 157:
#line 626 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 158:
#line 630 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 159:
#line 634 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 160:
#line 638 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 161:
  case_161();
  break;
case 162:
#line 650 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 163:
#line 654 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 164:
#line 655 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 165:
#line 656 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 166:
#line 657 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 167:
#line 661 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 168:
#line 665 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 169:
#line 669 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 170:
#line 670 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 171:
#line 671 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 172:
#line 678 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 173:
  case_173();
  break;
case 174:
  case_174();
  break;
case 175:
  case_175();
  break;
case 176:
#line 706 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 177:
#line 710 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 178:
#line 714 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 179:
#line 718 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 180:
#line 725 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 181:
#line 729 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 182:
#line 736 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 184:
#line 741 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 185:
  case_185();
  break;
case 186:
#line 760 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 187:
#line 764 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 188:
#line 768 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 189:
#line 772 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 190:
#line 776 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 191:
#line 780 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 192:
#line 784 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 193:
#line 788 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 194:
#line 792 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 195:
#line 796 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 196:
#line 803 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 197:
#line 807 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 198:
#line 811 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 199:
  case_199();
  break;
case 200:
  case_200();
  break;
case 201:
  case_201();
  break;
case 202:
  case_202();
  break;
case 203:
#line 851 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 215:
#line 881 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 216:
#line 885 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 217:
#line 889 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 218:
#line 890 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 221:
#line 902 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 222:
#line 906 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 223:
#line 910 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 224:
#line 911 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 228:
#line 921 "CParser.jay"
  {
		yyVal = new VirtualDeclarationStatement((Statement)yyVals[0+yyTop]) { IsVirtual = true };
	}
  break;
case 229:
  case_229();
  break;
case 230:
  case_230();
  break;
case 231:
#line 935 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 232:
  case_232();
  break;
case 233:
#line 949 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 234:
#line 950 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 235:
#line 951 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 236:
#line 958 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 237:
#line 962 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 238:
  case_238();
  break;
case 239:
#line 980 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 240:
#line 984 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 241:
#line 991 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 242:
#line 995 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 243:
#line 999 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-4+yyTop], (List<SwitchCase>)yyVals[-1+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 244:
#line 1003 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-3+yyTop], new List<SwitchCase>(), GetLocation(yyVals[-5+yyTop]));
	}
  break;
case 245:
#line 1010 "CParser.jay"
  {
		yyVal = new List<SwitchCase> { (SwitchCase)yyVals[0+yyTop] };
	}
  break;
case 246:
  case_246();
  break;
case 247:
#line 1022 "CParser.jay"
  {
		yyVal = new SwitchCase((Expression)yyVals[-2+yyTop], (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 248:
#line 1026 "CParser.jay"
  {
		yyVal = new SwitchCase(null, (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 249:
#line 1033 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 250:
#line 1037 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 251:
#line 1041 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 252:
#line 1045 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 253:
#line 1049 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 254:
#line 1053 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 256:
#line 1061 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 257:
#line 1065 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 258:
#line 1069 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 259:
#line 1073 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 260:
  case_260();
  break;
case 261:
  case_261();
  break;
case 266:
  case_266();
  break;
case 267:
  case_267();
  break;
case 268:
#line 1121 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 269:
#line 1122 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 270:
#line 1123 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
  break;
case 271:
  case_271();
  break;
case 272:
  case_272();
  break;
case 273:
  case_273();
  break;
case 274:
  case_274();
  break;
case 275:
  case_275();
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
#line 100 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 106 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 295 "CParser.jay"
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
#line 350 "CParser.jay"
{
		DeclarationSpecifiers ds = (DeclarationSpecifiers)yyVals[-2+yyTop];
		List<InitDeclarator> decls = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_83()
#line 359 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_84()
#line 365 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];		
		yyVal = ds;
	}

void case_85()
#line 371 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_86()
#line 377 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_87()
#line 383 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 389 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 395 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_90()
#line 401 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_91()
#line 410 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_92()
#line 416 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_118()
#line 468 "CParser.jay"
{
		var ts = new TypeSpecifier((TypeSpecifierKind)yyVals[-4+yyTop], (yyVals[-3+yyTop]).ToString(), (Block)yyVals[0+yyTop]);
		ts.BaseSpecifiers = (List<BaseSpecifier>)yyVals[-1+yyTop];
		yyVal = ts;
	}

void case_122()
#line 483 "CParser.jay"
{
		((List<BaseSpecifier>)yyVals[-2+yyTop]).Add((BaseSpecifier)yyVals[0+yyTop]);
		yyVal = yyVals[-2+yyTop];
	}

void case_130()
#line 504 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_131()
#line 509 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_132()
#line 515 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_133()
#line 520 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_139()
#line 537 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_140()
#line 543 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_151()
#line 592 "CParser.jay"
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

void case_161()
#line 640 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<Expression>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(ctorArgumentValue: n));
		}
		yyVal = d;
	}

void case_173()
#line 680 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_174()
#line 689 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_175()
#line 695 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_185()
#line 746 "CParser.jay"
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

void case_199()
#line 816 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_200()
#line 823 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_201()
#line 831 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_202()
#line 838 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_229()
#line 923 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsOverride = true };
	}

void case_230()
#line 928 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsOverride = true };
	}

void case_232()
#line 940 "CParser.jay"
{
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator((Declarator)yyVals[-3+yyTop], null) };
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-4+yyTop], decls);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsPureVirtual = true };
	}

void case_238()
#line 967 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		DeclarationSpecifiers ds = new DeclarationSpecifiers();
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator(fdecl, null) };
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_246()
#line 1012 "CParser.jay"
{
		((List<SwitchCase>)yyVals[-1+yyTop]).Add((SwitchCase)yyVals[0+yyTop]);
		yyVal = yyVals[-1+yyTop];
	}

void case_260()
#line 1078 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_261()
#line 1083 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_266()
#line 1098 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_267()
#line 1107 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_271()
#line 1128 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_272()
#line 1137 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_273()
#line 1149 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_274()
#line 1155 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_275()
#line 1161 "CParser.jay"
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
   28,   28,   28,   28,   36,   36,   34,   34,   34,   34,
   39,   39,   40,   40,   40,   40,   37,   37,   37,   41,
   41,   41,   41,   35,   35,   35,   35,   35,   42,   42,
   43,   43,   30,   32,   32,   46,   46,   46,   46,   45,
   45,   45,   45,   45,   45,   45,   45,   45,   45,   45,
   45,   45,   44,   44,   44,   44,   47,   47,   29,   29,
   29,   48,   48,   49,   49,   50,   50,   50,   50,    5,
    5,   51,   51,   51,   52,   52,   52,   52,   52,   52,
   52,   52,   52,   52,   52,   33,   33,   33,    6,    6,
    6,    6,   53,   54,   54,   55,   55,   56,   56,   56,
   56,   56,   56,   57,   58,   58,   63,   63,   64,   64,
   38,   38,   65,   65,   66,   66,   66,   66,   66,   66,
   66,   69,   67,   67,   67,   70,   70,   68,   59,   59,
   60,   60,   60,   60,   71,   71,   72,   72,   61,   61,
   61,   61,   61,   61,   62,   62,   62,   62,   62,    0,
    0,   73,   73,   73,   73,   74,   74,   77,   77,   77,
   75,   75,   76,   76,   76,   78,   78,   78,
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
    1,    1,    1,    1,    1,    1,    3,    5,    2,    2,
    1,    3,    1,    2,    2,    2,    1,    1,    1,    2,
    1,    2,    1,    4,    5,    5,    6,    2,    1,    3,
    1,    3,    1,    2,    1,    1,    2,    3,    4,    1,
    3,    5,    4,    4,    6,    6,    5,    4,    3,    4,
    4,    3,    1,    2,    2,    3,    1,    2,    1,    1,
    1,    1,    3,    1,    3,    2,    4,    2,    1,    1,
    2,    1,    1,    2,    3,    2,    3,    3,    4,    3,
    4,    2,    3,    3,    4,    1,    3,    4,    1,    2,
    3,    4,    2,    1,    2,    3,    2,    1,    1,    1,
    1,    1,    1,    3,    2,    3,    1,    2,    1,    1,
    2,    3,    1,    2,    1,    1,    1,    2,    4,    5,
    2,    5,    2,    2,    2,    4,    3,    2,    1,    2,
    5,    7,    7,    6,    1,    2,    4,    3,    5,    7,
    6,    7,    6,    7,    3,    2,    2,    2,    3,    1,
    2,    1,    1,    1,    1,    4,    3,    1,    3,    4,
    4,    5,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,   95,   96,   97,   98,   99,  143,  170,  101,  102,
  103,  104,  107,  108,  105,  106,  169,  171,  100,  109,
  110,  111,  127,  128,  129,    0,  264,    0,  263,    0,
    0,    0,    0,    0,  112,  113,    0,  260,  262,  265,
    0,  115,  116,    0,    0,  261,  146,    0,    0,    0,
   81,    0,   91,    0,    0,    0,    0,  114,   84,   86,
   88,   90,    0,    0,  119,    0,    0,    0,    0,  139,
    0,    0,  167,  165,    0,  147,    0,   82,  276,    0,
    0,  277,  278,  273,    0,  267,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  221,  225,    0,    0,
    0,  223,  226,  227,    0,    0,  117,  269,    0,    0,
    0,    0,    0,  174,    0,  134,    0,    0,  151,  168,
  166,   92,    0,    0,    2,    3,    0,    0,    0,    4,
    5,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  215,    0,    0,   27,   28,   29,   30,  239,    7,
    0,    0,   78,    0,   33,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   63,  219,  220,  208,
  209,  210,  211,  212,  213,    0,  217,    1,    0,  196,
   94,  275,  266,  274,  162,    0,   17,    0,    0,  159,
    0,    0,    0,  148,    0,  233,  234,  235,  228,    0,
  231,    0,    0,  222,  224,  238,    0,    0,    0,  123,
    0,  121,  270,  271,    0,    0,    0,    0,  178,    0,
    0,    0,   31,   80,  142,  136,  140,  135,    0,    0,
    0,   25,    0,   20,   21,    0,    0,    0,    0,    0,
    0,  256,  257,  258,    0,    0,    0,    0,    0,    0,
   22,   23,    0,  240,    0,   13,   14,    0,    0,    0,
   66,   67,   68,   69,   70,   71,   72,   73,   74,   75,
   76,   77,   65,    0,   24,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  216,  218,    0,    0,    0,  199,
    0,    0,  204,  161,    0,  160,    0,  158,  154,    0,
  153,    0,    0,  149,    0,    0,    0,  237,    0,  124,
  125,  126,    0,  118,  192,    0,    0,  186,    0,    0,
    0,    0,    0,    0,  272,  173,  175,  137,  214,    0,
    0,    0,    0,    0,    0,    0,    0,  255,  259,    6,
    0,  130,  132,    0,    0,  181,   79,   12,    9,    0,
    0,   11,   64,   34,   35,   36,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  207,  197,    0,  200,  203,  205,   18,
    0,    0,  157,  152,    0,    0,  229,  236,  122,  193,
  185,  190,  187,  177,  194,    0,  188,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   32,   10,
    8,    0,  206,  198,  201,    0,  155,  156,  230,    0,
  195,  191,  189,    0,    0,  249,    0,    0,    0,    0,
    0,    0,   62,  202,  232,    0,    0,    0,  244,    0,
  245,    0,  253,    0,  251,    0,   15,    0,  242,    0,
    0,  243,  246,  250,  254,  252,   16,    0,    0,    0,
  };
  protected static readonly short [] yyDgoto  = {            28,
  150,  151,  152,  186,  247,  299,  153,  154,  155,  156,
  157,  158,  159,  160,  161,  162,  163,  164,  165,  166,
  167,  274,  225,  168,   85,   52,   31,   32,   33,   34,
   53,  123,  181,   35,   36,  210,   37,   65,  211,  212,
  250,   69,   70,   55,   56,   57,   75,  326,  113,  114,
  327,  220,  301,  302,  303,  169,  170,  171,  172,  173,
  174,  175,  176,  177,  101,  102,  103,  104,  201,  105,
  450,  451,   38,   39,   40,   87,   41,   88,
  };
  protected static readonly short [] yySindex = {         2069,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, -101,    0, 2069,    0,   99,
 3198, 3198, 3198, 3198,    0,    0,  -77,    0,    0,    0,
  -32,    0,    0, -176,   -8,    0,    0,  -15,    7, -167,
    0,   35,    0,  683,   31,   36, -230,    0,    0,    0,
    0,    0, 3050,   -5,    0,  -95, 1899,   89,   13,    0,
 -176,  178,    0,    0,    7,    0,  -15,    0,    0,  249,
  571,    0,    0,    0,   99,    0, 3017, 3198,   36,  897,
  122,  -57,  185,  191,  263, 3198,    0,    0,   99,  -29,
 3111,    0,    0,    0,  267,  145,    0,    0,   53,  223,
  -26,  330,  339,    0, 1689,    0,  -66,   81,    0,    0,
    0,    0,  336,  341,    0,    0, 1729, 1737, 1737,    0,
    0,  364,  381,  387,   40,  400,  215,  385,  426, 1329,
 1038,    0, 1689, 1689,    0,    0,    0,    0,    0,    0,
  242,  -28,    0,  357,    0, 1689,   66,  333,  -62,  -45,
  -58,  460,  407,  394,  234,  -44,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  363,    0,    0,  477,    0,
    0,    0,    0,    0,    0,  137,    0,  480,  197,    0,
 1367,  444, 1191,    0,  310,    0,    0,    0,    0,   99,
    0,  -39, 1990,    0,    0,    0, -197, -197, -197,    0,
   14,    0,    0,    0, 1872, 1375,  508,   91,    0,  121,
  223, 3170,    0,    0,    0,    0,    0,    0,  -59,   40,
 1038,    0, 1038,    0,    0, 1689, 1689, 1689,  262,  963,
  542,    0,    0,    0,  295,  350,  564,  609,  609,  190,
    0,    0, 1689,    0,  349,    0,    0,  915, 1689,  351,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 1689,    0, 1689, 1689, 1689, 1689, 1689,
 1689, 1689, 1689, 1689, 1689, 1689, 1689, 1689, 1689, 1689,
 1689, 1689, 1689, 1689,    0,    0, 1689,  353,   82,    0,
  571,   26,    0,    0, 1689,    0, 1288,    0,    0, 1689,
    0, 1388,  514,    0,  -18,  551,  556,    0,  577,    0,
    0,    0,  145,    0,    0,  583,  586,    0, 1396,  535,
 1689,  121, 2031, 1434,    0,    0,    0,    0,    0,  588,
  601,  354,  366,  382,  603, 1497, 1497,    0,    0,    0,
 1526,    0,    0,  800,  187,    0,    0,    0,    0,  452,
   23,    0,    0,    0,    0,    0,   66,   66,  333,  333,
  -62,  -62,  -62,  -62,  -45,  -45,  -58,  460,  407,  394,
  234,  289,  552,    0,    0, 1199,    0,    0,    0,    0,
  554,  559,    0,    0,  585, 1572,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  605,    0, 1564,  560,  528,
  528,   40,  562,   40, 1689, 1632, 1672,  477,    0,    0,
    0, 1689,    0,    0,    0,  571,    0,    0,    0,  623,
    0,    0,    0,  361,  -83,    0,  487,   40,  488,   40,
  489,  203,    0,    0,    0,   40, 1689,  637,    0,  -80,
    0,  639,    0,   40,    0,   40,    0, 1266,    0,  638,
  457,    0,    0,    0,    0,    0,    0,  457,  457,  457,
  };
  protected static readonly short [] yyRindex = {            0,
 1963,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  779, 1450, 1540, 1878,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1760,    0,    0,    0,   79,    0,
    0,    0,    0,  329,    0,  753,  558,    0,    0,    0,
    0,    0,    0, 1816,    0,    0,    0,  268,    0,    0,
    0,    0,    0,    0,   92,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  836,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  494,    0,  658,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  -16, 3169,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2377,    0, 2445,    0,    0, 2473, 2720, 2767,   51,
 1749, 3007, 2907, 1808,  299,    3,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  495,  540,    0,  541,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  396,  401,  659,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -16,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  553,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  660,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2482, 2662, 2730, 2757,
 2815, 2825, 2852, 2920, 2879, 3005, 3014, 3100, 3102, 2183,
 2949,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 2405,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 2350,
    0,    0,    0,  154,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  -74,   70,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -85,    0,  445, -147,  284,  -81,  -22, 1697,    0,
   71,  232,  241,  300,  415,  416,  414,  417,  418,    0,
 -112,    0, -293,  941,    1,  -51,    0,  111, 1675,    0,
  630,  276, -142,    0,    0,  383,    0,  -40,    0,  389,
  347,  642,  -43,  -36,  -17,   41,  -21,  -60,    0,  486,
  -61, -182, -334,    0,  408, -129,    0,  -33, -217,    0,
    0,    0, -321, -174,    0,  620,    0,    0,    0,    0,
    0,  272,  695,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           180,
   30,  296,  224,  383,   77,  239,  112,   67,  187,  192,
  203,  258,   74,  215,  285,   49,  286,  260,  294,   78,
   86,   44,  347,  107,   48,   77,   49,   93,   30,  188,
  109,   59,   60,   61,   62,  332,  300,   89,  121,   92,
   78,  449,   93,   61,  462,   63,   61,  202,   49,  219,
  248,  426,  106,  183,  245,  246,  117,  323,  226,   42,
   61,   61,  259,   99,  216,  338,  253,  111,  195,  193,
   48,  298,  148,  227,  218,   90,  214,  143,   77,  141,
   68,  144,  145,  340,  146,  341,  388,   43,   48,   76,
  111,   48,  223,   78,   48,   61,  200,  180,  149,   50,
  339,   99,  278,  100,  232,  234,  235,  276,   48,   48,
   50,  313,  277,   48,   71,  421,  297,   63,  163,  163,
  223,  223,  163,  426,  229,  386,   91,   61,  416,  417,
  215,  164,  164,  223,  330,  164,   63,  116,   48,  469,
   49,  100,  319,   48,   48,  246,  470,  246,  315,  115,
  342,  343,  344,  460,  148,   42,   50,   51,  387,  143,
  333,  141,   80,  191,  145,  147,  146,  307,  223,  163,
  324,  357,  332,  361,   48,   48,  187,  304,  218,   42,
  305,  216,  164,   43,  224,  227,  241,  335,  356,  108,
   68,  241,  363,  241,  247,  241,  241,   68,  241,  194,
   89,  281,  282,  111,  163,  228,  385,   43,  382,  287,
  288,  334,  241,  355,  190,  111,   50,  164,  119,  180,
  283,  284,  111,  390,   50,  391,  354,  293,  392,  354,
   47,   49,  255,  256,  257,  447,  448,   66,  447,  448,
   92,   47,  196,  425,  248,  248,  458,  147,  197,  404,
  317,  248,  409,  223,  223,  223,  223,  223,  223,  223,
  223,  223,  223,  223,  223,  223,  223,  223,  223,  223,
  223,  395,  406,   93,  223,  300,  241,  216,  241,  241,
  216,  148,  434,  444,  436,  253,  143,   47,  141,  223,
  144,  145,   61,  146,  296,  296,  124,  125,  126,  127,
  254,  128,  129,    8,  180,   54,  223,  149,  453,  443,
  455,  141,   17,   18,  180,  425,  459,  355,   48,   48,
  198,   48,   48,   72,  465,  206,  466,  457,  223,  437,
  439,  441,  253,  111,  224,  163,  180,  213,  253,   59,
   48,  248,   59,  248,  180,   80,  422,   47,  164,  367,
  368,  130,  131,  349,  111,   47,   59,   59,  248,  248,
  132,   59,  133,  134,  135,  136,  137,  138,  139,  140,
  221,   80,   93,  142,  147,  279,  180,  280,  178,  125,
  126,  127,  222,  128,  129,  223,  217,   93,  247,  247,
  350,   59,  141,  253,  412,  148,   81,  253,  230,  223,
  143,   42,  141,  236,  144,  145,  413,  146,   45,  253,
  241,  241,  241,  241,  189,  241,  241,  273,    8,   64,
  237,  149,  414,   59,  223,  253,  238,   17,   18,   43,
  207,  208,  209,  130,  131,  131,  131,  131,  241,  240,
  133,  133,  133,  242,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  316,  241,  241,  241,  241,
  241,  241,  241,  241,  243,   80,  131,  295,  147,  148,
   72,  133,  420,    8,  143,  305,  141,  289,  144,  145,
  290,  146,   17,   18,  292,  124,  125,  126,  127,  148,
  128,  129,  369,  370,  143,  149,  141,  291,  144,  145,
  306,  146,  298,  371,  372,  373,  374,  452,  454,  456,
  253,  253,  253,   58,  179,  176,  309,  179,  176,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
  130,  131,   23,   24,   25,   26,  314,  297,  331,  132,
   59,  133,  134,  135,  136,  137,  138,  139,  140,   80,
  182,  183,  147,  182,  183,  345,  375,  376,   59,  320,
  321,  322,  150,  184,  352,  353,  184,  150,  150,  179,
  348,  150,  147,  148,  351,  358,  394,  362,  143,  384,
  141,  396,  144,  145,  397,  146,  150,  398,  150,  124,
  125,  126,  127,  400,  128,  129,  401,  403,  410,  261,
  262,  263,  264,  265,  266,  267,  268,  269,  270,  271,
  272,  411,  415,  429,  423,  431,  427,   58,  150,  150,
  418,  428,  433,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,  130,  131,   23,   24,   25,   26,
  150,  445,  446,  132,  435,  133,  134,  135,  136,  137,
  138,  139,  140,  179,  461,  468,  147,  464,  172,  180,
  182,  442,  360,  377,  379,  378,  122,  337,  380,  389,
  381,  399,  118,  124,  125,  126,  127,   82,  128,  129,
  205,  463,   46,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  178,  125,  126,  127,    0,  128,  129,
    0,   58,    0,   81,    0,    0,    0,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,  130,  131,
   23,   24,   25,   26,   83,    0,    0,  132,    0,  133,
  134,  135,  136,  137,  138,  139,  140,  145,  130,  131,
    0,    0,    0,  145,    0,    0,  145,    0,    0,    0,
    0,    0,    0,    0,    0,   80,    0,    0,    0,    0,
    0,  145,    0,  145,    0,    0,    0,    0,   83,   83,
   83,    0,   83,    0,    0,    0,    0,  178,  125,  126,
  127,    0,  128,  129,    0,    0,    0,   83,    0,  354,
  325,   49,  150,    0,  145,    0,    0,  150,  150,  150,
  150,  150,  150,  150,  150,  150,  150,  150,  150,  150,
  150,  150,  150,  150,  150,  150,  150,  150,  150,   83,
  144,  150,  150,  150,  150,  145,  144,    0,    0,  144,
    0,    0,  130,  131,    0,    0,    0,    0,  150,    0,
  216,    0,    0,   58,  144,    0,  144,    0,    0,    0,
    0,    0,    0,    0,   83,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    0,  144,    0,  148,
    0,    0,    0,    0,  143,    0,  141,  185,  144,  145,
   29,  146,    0,    0,    0,    0,    0,  148,    0,    0,
    0,    0,  143,    0,  141,  359,  144,  145,  144,  146,
    0,    0,    0,    0,    0,    0,    0,   58,   29,    0,
    0,    0,    0,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   84,  148,   23,   24,   25,   26,
  143,    0,  141,   98,  144,  145,    0,  146,    0,    0,
    0,    0,    0,   79,    0,    0,    0,    0,    0,    0,
    0,  149,  147,    0,    0,    0,    0,  182,  184,    0,
    0,    0,    0,    0,    0,   83,  199,  145,    0,    0,
  147,   98,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,    0,    0,  145,  145,  145,  145,
  148,    0,    0,    0,    0,  143,    0,  141,    0,  144,
  145,    0,  146,  145,   58,    0,    0,    0,  147,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,    0,
  144,    0,    0,    0,    0,  144,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,    0,    0,  144,
  144,  144,  144,  178,  125,  126,  127,    0,  128,  129,
    0,    0,    0,  147,    0,    0,  144,    0,    0,    0,
    0,  178,  125,  126,  127,    0,  128,  129,    0,    0,
  346,   58,    0,    0,    0,    0,    0,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,  130,  131,
   23,   24,   25,   26,    0,    0,    0,    0,    0,  178,
  125,  126,  127,  148,  128,  129,  130,  131,  143,    0,
  141,  148,  312,  145,    0,  146,  143,    0,  141,    0,
  144,  145,    0,  146,  298,    0,    0,   58,    0,    0,
    0,    0,    0,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,  130,  131,   23,   24,   25,   26,
    0,    0,    0,  311,    0,    0,    0,    0,    0,  297,
    0,    0,    0,    0,  178,  125,  126,  127,  148,  128,
  129,    0,    0,  143,    0,  141,    0,  144,  145,    0,
  146,  298,    0,    0,    0,    0,  147,    0,    0,    0,
  148,  179,   58,  424,  147,  143,    0,  141,    0,  144,
  145,    0,  146,    0,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,  130,
  131,   23,   24,   25,   26,    0,  297,    0,    0,    0,
    0,  148,    0,    0,    0,    0,  143,    0,  141,    0,
  144,  145,    0,  146,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  244,  179,    0,
  467,  147,    0,    0,    0,    0,    0,    0,    0,  148,
    0,    0,    0,    0,  143,    0,  141,  148,  144,  145,
    0,  146,  143,  147,  141,    0,  329,  145,    0,  146,
  148,    0,    0,    0,    0,  143,    0,  141,  148,  144,
  145,    0,  146,  143,    0,  141,    0,  144,  145,    0,
  146,    0,    0,    0,    0,    0,    0,  178,  125,  126,
  127,    0,  128,  129,  147,  178,  125,  126,  127,  308,
  128,  129,    0,    0,    0,    0,  148,  328,    0,    0,
    0,  143,    0,  141,    0,  408,  145,    0,  146,    0,
  393,    0,    0,  310,    0,    0,    0,    8,  402,   85,
   85,   85,  147,   85,    0,    0,   17,   18,    0,    0,
  147,    0,  130,  131,    0,    0,    0,    0,   85,    0,
  130,  131,    0,  147,    0,    0,    0,    0,    0,    0,
    0,  147,  178,  125,  126,  127,  407,  128,  129,  148,
    0,    0,    0,    0,  143,    0,  141,    0,  144,  145,
   85,  146,    0,    0,  178,  125,  126,  127,    0,  128,
  129,    0,    0,    0,    0,  149,    0,    0,  148,  147,
    0,    0,    0,  143,    0,  141,    0,  144,  145,    0,
  146,    0,    0,    0,    0,   85,    0,  130,  131,   87,
   87,   87,    0,   87,    8,  178,  125,  126,  127,    0,
  128,  129,    0,   17,   18,    0,  148,    0,   87,  130,
  131,  143,    0,  141,  148,  144,  145,    0,  146,  143,
    0,  141,    0,  144,  145,    0,  146,    0,    0,    0,
    0,    0,  147,  178,  125,  126,  127,    0,  128,  129,
   87,  178,  125,  126,  127,    0,  128,  129,    0,    0,
  130,  131,    0,    0,  178,  125,  126,  127,  418,  128,
  129,  147,  178,  125,  126,  127,  432,  128,  129,    0,
    0,    0,    0,    0,  148,   87,    0,    0,    0,  143,
    0,  141,  438,  144,  145,    0,  146,    0,  130,  131,
    0,    0,    0,    0,    0,    0,  130,  131,    0,  147,
  178,  125,  126,  127,  179,  128,  129,  147,    0,  130,
  131,    0,    0,    0,  148,    0,   85,  130,  131,  143,
    0,  141,  440,  144,  145,    0,  146,    0,    0,    0,
    0,  148,    0,   73,    0,    0,  143,    0,  141,    0,
  144,  145,    0,  146,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  130,  131,    0,    0,  120,
    0,    0,    0,  178,  125,  126,  127,  147,  128,  129,
    0,  148,    0,    0,    0,   73,  143,    0,  231,  148,
  144,  145,    0,  146,  143,    0,  233,    0,  144,  145,
    0,  146,  178,  125,  126,  127,   51,  128,  129,   51,
    0,    0,   51,    0,    0,    0,   87,  147,    0,  138,
  138,  138,    0,  138,    0,    0,   51,   51,  130,  131,
    0,   51,    0,    0,  147,  249,    0,    0,  138,    0,
  178,  125,  126,  127,    0,  128,  129,    0,  178,  430,
  126,  127,    0,  128,  129,    0,    0,  130,  131,  251,
  252,   51,   51,    0,    0,    0,    0,    0,   57,    0,
  138,   57,  275,    0,  147,  120,  120,  120,    0,  120,
    0,    0,  147,   73,    0,   57,   57,  120,    0,    0,
   57,    0,   51,   51,  120,  130,  131,    0,    0,    0,
    0,    0,    0,  130,  131,  138,    0,  252,  178,  125,
  126,  127,    0,  128,  129,    0,    0,    0,    0,    0,
   57,    0,    0,    0,    0,  249,  120,  249,    0,    0,
    0,  215,  325,   49,    0,    0,    0,   89,   89,   89,
    0,   89,  249,  249,    0,    0,    0,    0,  178,  125,
  126,  127,   57,  128,  129,    0,   89,    0,    0,  110,
    0,  120,    0,  130,  131,  178,  125,  126,  127,    0,
  128,  129,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  216,    0,    0,    0,    0,    0,   89,    0,
    0,    0,  364,  365,  366,    0,    0,    0,    0,    0,
    0,  120,    0,  130,  131,  178,  125,  126,  127,    0,
  128,  129,    0,  178,  125,  126,  127,   50,  128,  129,
  130,  131,  114,   89,  114,    0,    0,    0,  252,    0,
    0,    0,    0,    0,    0,    0,  138,    0,    0,   51,
   51,  114,    0,    0,    0,  252,    0,    0,    0,    0,
  318,    0,    0,    0,    0,    0,    0,    0,   51,    0,
  130,  131,    0,    0,  138,    0,    0,  419,  130,  131,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  405,  120,  138,  138,  138,  138,    0,   57,   57,
    0,    0,    0,    0,    0,    0,    0,    0,  114,    0,
    0,    0,    0,    0,    0,    0,    0,   57,    0,    0,
  120,    0,    0,    0,  252,    0,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,   27,   47,  120,
  120,  120,  120,    0,   89,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   58,    0,    0,    0,
    0,    0,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   58,    0,   23,   24,   25,   26,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    0,    0,    0,  114,
    0,    0,    0,   58,    0,    0,   58,    0,    0,    0,
    0,    0,  268,    0,    0,    0,    0,    0,    0,    0,
   58,   58,    0,    0,    0,   58,    0,  114,    0,    0,
    0,    0,    0,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,   58,   58,  114,  114,  114,  114,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,   58,    0,    0,
    0,    0,    0,    0,    0,   58,    0,    0,    0,    0,
    0,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,    0,    0,
    0,    0,    0,    1,    0,    0,    0,    0,    0,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    2,    2,    0,    2,
    0,    2,    2,    2,    2,    2,    2,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    2,
    2,    2,    2,   19,   19,    0,    0,   19,   19,   19,
   19,   19,    0,   19,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   19,   19,   19,   19,   19,   19,
    2,   26,   26,    2,    0,   26,   26,   26,   26,   26,
    0,   26,    0,   58,   58,    0,    0,    0,    0,    0,
    0,    0,   26,   26,   26,   26,   26,   26,    0,   19,
   19,    0,   58,    2,    0,    0,    0,    0,    0,    0,
    0,   31,   31,    0,    0,   31,   31,   31,   31,   31,
    0,   31,    0,    0,    0,    0,    0,   26,   26,    0,
   19,   19,   31,   31,   31,    0,   31,   31,    0,    0,
   37,    0,    0,   37,    0,   37,   37,   37,    0,   38,
    0,    0,   38,    0,   38,   38,   38,    0,   26,   26,
   37,   37,   37,    0,   37,   37,    0,   31,   31,   38,
   38,   38,    0,   38,   38,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   37,   37,    0,   31,   31,
    0,    0,    0,    0,   38,   38,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   37,   37,    0,    0,
    0,    0,    0,    0,    0,   38,   38,    0,    0,    0,
    2,    2,    2,    2,    2,    2,    2,    2,    2,    0,
    2,    2,    2,    2,    2,    2,    2,    2,    2,    2,
    2,    2,    2,    2,    0,    0,    0,    0,    0,    2,
   19,   19,   19,   19,   19,   19,    0,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,    0,    0,    0,    0,    0,   19,    0,   26,   26,
   26,   26,   26,   26,    0,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,    0,
    0,    0,    0,    0,   26,    0,    0,    0,    0,   39,
    0,    0,   39,    0,   39,   39,   39,    0,   31,   31,
   31,   31,   31,   31,    0,   31,   31,    0,    0,   39,
   39,   39,    0,   39,   39,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   31,    0,   37,   37,   37,   37,
   37,   37,    0,   37,   37,   38,   38,   38,   38,   38,
   38,    0,   38,   38,   39,   39,    0,   40,    0,    0,
   40,    0,   37,   40,    0,    0,    0,   41,    0,    0,
   41,   38,    0,   41,    0,    0,    0,   40,   40,   40,
    0,   40,   40,    0,    0,   39,   39,   41,   41,   41,
    0,   41,   41,    0,   42,    0,    0,   42,    0,    0,
   42,    0,    0,    0,   43,    0,    0,   43,    0,    0,
   43,    0,   40,   40,   42,   42,   42,    0,   42,   42,
    0,    0,   41,   41,   43,   43,   43,    0,   43,   43,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   40,   40,    0,    0,    0,    0,   42,
   42,    0,   46,   41,   41,   46,    0,    0,   46,   43,
   43,    0,   47,    0,    0,   47,    0,    0,   47,    0,
    0,    0,   46,   46,   46,    0,   46,   46,    0,    0,
   42,   42,   47,   47,   47,    0,   47,   47,    0,   44,
   43,   43,   44,    0,    0,   44,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   46,   46,   44,
   44,   44,    0,   44,   44,    0,   49,   47,   47,   49,
    0,    0,   49,    0,    0,   39,   39,   39,   39,   39,
   39,    0,   39,   39,    0,    0,   49,   49,   46,   46,
    0,   49,    0,    0,   44,   44,    0,   55,   47,   47,
   55,   39,    0,    0,    0,    0,    0,   45,    0,    0,
   45,    0,    0,   45,   55,   55,    0,    0,    0,   55,
    0,   49,   49,    0,    0,   44,   44,   45,   45,   45,
    0,   45,   45,   40,   40,   40,   40,   40,   40,   60,
   40,   40,   60,   41,   41,   41,   41,   41,   41,   55,
   41,   41,   49,   49,    0,    0,   60,   60,    0,   40,
    0,   60,   45,   45,    0,    0,    0,    0,    0,   41,
   42,   42,   42,   42,   42,   42,    0,   42,   42,    0,
   55,   55,   43,   43,   43,   43,    0,   43,   43,    0,
    0,   60,   50,   45,   45,   50,   42,   53,   50,    0,
   53,   52,    0,    0,   52,    0,   43,   52,    0,    0,
    0,    0,   50,   50,   53,   53,    0,   50,    0,   53,
    0,   52,   52,   60,    0,    0,   52,    0,    0,    0,
   46,   46,   46,   46,    0,   46,   46,    0,    0,    0,
   47,   47,   47,   47,    0,   47,   47,   50,   50,   53,
   53,    0,    0,    0,   46,    0,   52,   52,    0,    0,
    0,    0,    0,    0,   47,    0,    0,   44,   44,   44,
   44,    0,   44,   44,    0,    0,    0,    0,   50,   50,
   53,   53,    0,    0,    0,    0,    0,   52,   52,   80,
   54,   44,   56,   54,    0,   56,   49,   49,    0,   49,
   49,    0,    0,    0,    0,    0,    0,   54,   54,   56,
   56,    0,   54,    0,   56,    0,    0,    0,   49,    0,
    0,    0,    0,    0,   97,   50,    0,   55,   55,    0,
    0,    0,    0,    0,    0,   45,   45,   45,   45,    0,
   45,   45,   54,   54,   56,    0,   55,    0,    0,    0,
    0,    0,    0,    0,    0,    1,    1,    0,    1,   45,
    1,    1,    1,    1,    1,    1,    0,    0,    0,    0,
   60,    0,    0,   54,   54,   56,   56,    1,    1,    1,
    1,    1,    0,    0,    0,  204,   50,    0,   60,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    1,
    0,    0,    1,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   50,   50,    0,   50,   50,   53,   53,    0,
    0,    0,    0,    0,   52,   52,    0,    0,    0,    0,
    0,    0,    1,    0,   50,    0,   53,    0,    0,    0,
    0,   58,    0,   52,    0,    0,   47,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,   58,   93,   94,   95,   96,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,   47,    0,    0,
   54,   54,   56,   56,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   54,
    0,   56,    0,    0,    0,   58,   93,   94,   95,   96,
    0,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    0,   58,    0,    0,    0,    0,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,   58,   23,   24,   25,   26,  336,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,
  };
  protected static readonly short [] yyCheck = {            81,
    0,  176,  115,  297,   44,  135,   67,   40,   90,   91,
   40,   40,   49,   40,   60,   42,   62,   46,   63,   59,
   54,  123,  240,   64,   40,   44,   42,   44,   28,   90,
  126,   31,   32,   33,   34,  218,  179,   55,   75,  270,
   59,  125,   59,   41,  125,  123,   44,   99,   42,  111,
  125,  386,   58,   87,  140,  141,   44,   44,  125,  257,
   58,   59,   91,   63,   91,  125,   44,   67,  126,   91,
   40,   46,   33,  117,  111,   40,  110,   38,   44,   40,
  257,   42,   43,  231,   45,  233,   61,  285,   38,  257,
   90,   41,  115,   59,   44,   93,   96,  179,   59,  126,
  230,  101,   37,   63,  127,  128,  129,   42,   58,   59,
  126,  193,   47,   63,  123,   93,   91,  123,   40,   41,
  143,  144,   44,  458,   44,   44,   91,  125,  346,  347,
   40,   40,   41,  156,  216,   44,  123,  125,   40,  461,
   42,  101,  203,   93,   94,  231,  468,  233,  200,   61,
  236,  237,  238,  447,   33,  257,  126,   59,  301,   38,
   40,   40,  123,   42,   43,  126,   45,  189,  191,   91,
  211,  253,  355,  259,  124,  125,  258,   41,  215,  257,
   44,   91,   91,  285,  297,  229,   33,  221,  250,  285,
  257,   38,  274,   40,  125,   42,   43,  257,   45,  257,
  218,  264,  265,  203,  126,  125,  125,  285,  294,  268,
  269,   91,   59,  250,   93,  215,  126,  126,   41,  301,
  266,  267,  222,  305,  126,  307,   40,  272,  310,   40,
  257,   42,  261,  262,  263,  319,  320,  270,  319,  320,
  270,  257,   58,  386,  319,  320,   44,  126,   58,  331,
  290,  141,  334,  276,  277,  278,  279,  280,  281,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  290,  333,  290,  297,  418,  123,   91,  125,  126,
   91,   33,  412,  426,  414,   44,   38,  257,   40,  312,
   42,   43,  290,   45,  469,  470,  257,  258,  259,  260,
   59,  262,  263,  297,  386,   30,  329,   59,  438,  422,
  440,   44,  306,  307,  396,  458,  446,  354,  268,  269,
   58,  271,  272,   48,  454,   59,  456,  125,  351,  415,
  416,  417,   44,  333,  447,  257,  418,  285,   44,   41,
  290,  231,   44,  233,  426,  123,   58,  257,  257,  279,
  280,  312,  313,   59,  354,  257,   58,   59,  248,  249,
  321,   63,  323,  324,  325,  326,  327,  328,  329,  330,
   41,  123,   44,  125,  126,   43,  458,   45,  257,  258,
  259,  260,   44,  262,  263,  408,  111,   59,  319,  320,
   41,   93,  125,   44,   41,   33,   61,   44,   58,  422,
   38,  257,   40,   40,   42,   43,   41,   45,   26,   44,
  257,  258,  259,  260,  293,  262,  263,   61,  297,   37,
   40,   59,   41,  125,  447,   44,   40,  306,  307,  285,
  286,  287,  288,  312,  313,   40,   41,   42,  285,   40,
   40,   41,   42,   59,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  313,  314,  315,  316,
  317,  257,  319,  320,  321,  200,  323,  324,  325,  326,
  327,  328,  329,  330,   59,  123,   91,  125,  126,   33,
  215,   91,   41,  297,   38,   44,   40,   38,   42,   43,
   94,   45,  306,  307,  271,  257,  258,  259,  260,   33,
  262,  263,  281,  282,   38,   59,   40,  124,   42,   43,
   41,   45,   46,  283,  284,  285,  286,   41,   41,   41,
   44,   44,   44,  285,   41,   41,   93,   44,   44,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,  257,   91,   61,  321,
  272,  323,  324,  325,  326,  327,  328,  329,  330,  123,
   41,   41,  126,   44,   44,  324,  287,  288,  290,  207,
  208,  209,   35,   41,  248,  249,   44,   40,   41,  123,
   59,   44,  126,   33,   41,  257,   93,  257,   38,  257,
   40,   61,   42,   43,   59,   45,   59,   41,   61,  257,
  258,  259,  260,   41,  262,  263,   41,   93,   41,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   41,   40,   59,   93,   41,   93,  285,   91,   92,
  123,   93,   93,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  313,  314,  315,  316,  317,
  123,   59,  322,  321,  123,  323,  324,  325,  326,  327,
  328,  329,  330,  123,   58,   58,  126,   59,   41,   41,
   41,  418,  258,  289,  291,  290,   77,  222,  292,  302,
  293,  323,   71,  257,  258,  259,  260,   35,  262,  263,
  101,  450,   28,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,  285,   -1,   61,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  313,
  314,  315,  316,  317,   92,   -1,   -1,  321,   -1,  323,
  324,  325,  326,  327,  328,  329,  330,   35,  312,  313,
   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  123,   -1,   -1,   -1,   -1,
   -1,   59,   -1,   61,   -1,   -1,   -1,   -1,   40,   41,
   42,   -1,   44,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   59,   -1,   40,
   41,   42,  285,   -1,   92,   -1,   -1,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,   91,
   35,  314,  315,  316,  317,  123,   41,   -1,   -1,   44,
   -1,   -1,  312,  313,   -1,   -1,   -1,   -1,  331,   -1,
   91,   -1,   -1,  285,   59,   -1,   61,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
   -1,   -1,  314,  315,  316,  317,   -1,   92,   -1,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   41,   42,   43,
    0,   45,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,
   -1,   -1,   38,   -1,   40,   41,   42,   43,  123,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   28,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,   54,   33,  314,  315,  316,  317,
   38,   -1,   40,   63,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,  331,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,  126,   -1,   -1,   -1,   -1,   87,   88,   -1,
   -1,   -1,   -1,   -1,   -1,  257,   96,  285,   -1,   -1,
  126,  101,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,   -1,   -1,  314,  315,  316,  317,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,  331,  285,   -1,   -1,   -1,  126,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   -1,   -1,  314,  315,  316,  317,   -1,   -1,   -1,
  285,   -1,   -1,   -1,   -1,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,   -1,   -1,  314,
  315,  316,  317,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,  126,   -1,   -1,  331,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
  240,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  313,
  314,  315,  316,  317,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   33,  262,  263,  312,  313,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   46,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  313,  314,  315,  316,  317,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   91,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   33,  262,
  263,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   46,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,
   33,  123,  285,  125,  126,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  313,  314,  315,  316,  317,   -1,   91,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,  123,   -1,
  125,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   33,   42,   43,
   -1,   45,   38,  126,   40,   -1,   42,   43,   -1,   45,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   33,   42,
   43,   -1,   45,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,  126,  257,  258,  259,  260,   93,
  262,  263,   -1,   -1,   -1,   -1,   33,   93,   -1,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   93,   -1,   -1,  293,   -1,   -1,   -1,  297,   93,   40,
   41,   42,  126,   44,   -1,   -1,  306,  307,   -1,   -1,
  126,   -1,  312,  313,   -1,   -1,   -1,   -1,   59,   -1,
  312,  313,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  126,  257,  258,  259,  260,   93,  262,  263,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   91,   45,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,   59,   -1,   -1,   33,  126,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,  126,   -1,  312,  313,   40,
   41,   42,   -1,   44,  297,  257,  258,  259,  260,   -1,
  262,  263,   -1,  306,  307,   -1,   33,   -1,   59,  312,
  313,   38,   -1,   40,   33,   42,   43,   -1,   45,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,  126,  257,  258,  259,  260,   -1,  262,  263,
   91,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
  312,  313,   -1,   -1,  257,  258,  259,  260,  123,  262,
  263,  126,  257,  258,  259,  260,   93,  262,  263,   -1,
   -1,   -1,   -1,   -1,   33,  126,   -1,   -1,   -1,   38,
   -1,   40,   41,   42,   43,   -1,   45,   -1,  312,  313,
   -1,   -1,   -1,   -1,   -1,   -1,  312,  313,   -1,  126,
  257,  258,  259,  260,  123,  262,  263,  126,   -1,  312,
  313,   -1,   -1,   -1,   33,   -1,  257,  312,  313,   38,
   -1,   40,   41,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   33,   -1,   49,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  312,  313,   -1,   -1,   75,
   -1,   -1,   -1,  257,  258,  259,  260,  126,  262,  263,
   -1,   33,   -1,   -1,   -1,   91,   38,   -1,   40,   33,
   42,   43,   -1,   45,   38,   -1,   40,   -1,   42,   43,
   -1,   45,  257,  258,  259,  260,   38,  262,  263,   41,
   -1,   -1,   44,   -1,   -1,   -1,  257,  126,   -1,   40,
   41,   42,   -1,   44,   -1,   -1,   58,   59,  312,  313,
   -1,   63,   -1,   -1,  126,  141,   -1,   -1,   59,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,  312,  313,  143,
  144,   93,   94,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   91,   44,  156,   -1,  126,   40,   41,   42,   -1,   44,
   -1,   -1,  126,  189,   -1,   58,   59,  193,   -1,   -1,
   63,   -1,  124,  125,   59,  312,  313,   -1,   -1,   -1,
   -1,   -1,   -1,  312,  313,  126,   -1,  191,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,  231,   91,  233,   -1,   -1,
   -1,   40,   41,   42,   -1,   -1,   -1,   40,   41,   42,
   -1,   44,  248,  249,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,  125,  262,  263,   -1,   59,   -1,   -1,   41,
   -1,  126,   -1,  312,  313,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   91,   -1,   -1,   -1,   -1,   -1,   91,   -1,
   -1,   -1,  276,  277,  278,   -1,   -1,   -1,   -1,   -1,
   -1,  307,   -1,  312,  313,  257,  258,  259,  260,   -1,
  262,  263,   -1,  257,  258,  259,  260,  126,  262,  263,
  312,  313,   40,  126,   42,   -1,   -1,   -1,  312,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,  271,
  272,   59,   -1,   -1,   -1,  329,   -1,   -1,   -1,   -1,
   41,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,   -1,
  312,  313,   -1,   -1,  285,   -1,   -1,  351,  312,  313,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   41,  257,  314,  315,  316,  317,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,
  285,   -1,   -1,   -1,  408,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,   59,  257,  314,
  315,  316,  317,   -1,  257,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  285,   -1,  314,  315,  316,  317,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
   -1,   -1,  314,  315,  316,  317,   -1,   -1,   -1,  257,
   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,  270,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   -1,   -1,   -1,   63,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  285,   93,  314,  315,  316,  317,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   -1,   -1,  314,  315,  316,  317,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,   -1,   -1,  314,  315,  316,  317,   -1,   -1,
   -1,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
   -1,   -1,  314,  315,  316,  317,   37,   38,   -1,   40,
   -1,   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   60,
   61,   62,   63,   37,   38,   -1,   -1,   41,   42,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,
   91,   37,   38,   94,   -1,   41,   42,   43,   44,   45,
   -1,   47,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   58,   59,   60,   61,   62,   63,   -1,   93,
   94,   -1,  290,  124,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   37,   38,   -1,   -1,   41,   42,   43,   44,   45,
   -1,   47,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,
  124,  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   38,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,  124,  125,
   58,   59,   60,   -1,   62,   63,   -1,   93,   94,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,  125,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
  261,  262,  263,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  282,  283,  284,   -1,   -1,   -1,   -1,   -1,  290,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   -1,   -1,   -1,   -1,   -1,  290,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,  282,  283,  284,   -1,
   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  290,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   93,   94,   -1,   38,   -1,   -1,
   41,   -1,  290,   44,   -1,   -1,   -1,   38,   -1,   -1,
   41,  290,   -1,   44,   -1,   -1,   -1,   58,   59,   60,
   -1,   62,   63,   -1,   -1,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   93,   94,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   93,   94,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,
   94,   -1,   38,  124,  125,   41,   -1,   -1,   44,   93,
   94,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,
  124,  125,   58,   59,   60,   -1,   62,   63,   -1,   38,
  124,  125,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   58,
   59,   60,   -1,   62,   63,   -1,   38,   93,   94,   41,
   -1,   -1,   44,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,   58,   59,  124,  125,
   -1,   63,   -1,   -1,   93,   94,   -1,   41,  124,  125,
   44,  290,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   58,   59,   -1,   -1,   -1,   63,
   -1,   93,   94,   -1,   -1,  124,  125,   58,   59,   60,
   -1,   62,   63,  264,  265,  266,  267,  268,  269,   41,
  271,  272,   44,  264,  265,  266,  267,  268,  269,   93,
  271,  272,  124,  125,   -1,   -1,   58,   59,   -1,  290,
   -1,   63,   93,   94,   -1,   -1,   -1,   -1,   -1,  290,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   -1,
  124,  125,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   93,   38,  124,  125,   41,  290,   41,   44,   -1,
   44,   38,   -1,   -1,   41,   -1,  290,   44,   -1,   -1,
   -1,   -1,   58,   59,   58,   59,   -1,   63,   -1,   63,
   -1,   58,   59,  125,   -1,   -1,   63,   -1,   -1,   -1,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,
  266,  267,  268,  269,   -1,  271,  272,   93,   94,   93,
   94,   -1,   -1,   -1,  290,   -1,   93,   94,   -1,   -1,
   -1,   -1,   -1,   -1,  290,   -1,   -1,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,   -1,   -1,  124,  125,
  124,  125,   -1,   -1,   -1,   -1,   -1,  124,  125,  123,
   41,  290,   41,   44,   -1,   44,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   58,
   59,   -1,   63,   -1,   63,   -1,   -1,   -1,  290,   -1,
   -1,   -1,   -1,   -1,  125,  126,   -1,  271,  272,   -1,
   -1,   -1,   -1,   -1,   -1,  266,  267,  268,  269,   -1,
  271,  272,   93,   94,   93,   -1,  290,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   37,   38,   -1,   40,  290,
   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,   -1,
  272,   -1,   -1,  124,  125,  124,  125,   59,   60,   61,
   62,   63,   -1,   -1,   -1,  125,  126,   -1,  290,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   91,
   -1,   -1,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  268,  269,   -1,  271,  272,  271,  272,   -1,
   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,  124,   -1,  290,   -1,  290,   -1,   -1,   -1,
   -1,  285,   -1,  290,   -1,   -1,  257,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,   -1,   -1,
  314,  315,  316,  317,  285,  286,  287,  288,  289,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   -1,   -1,  314,  315,  316,  317,  257,   -1,   -1,
  271,  272,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,
   -1,  290,   -1,   -1,   -1,  285,  286,  287,  288,  289,
   -1,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,   -1,   -1,  314,  315,  316,  317,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   -1,  285,  314,  315,  316,  317,  318,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,   -1,
   -1,  314,  315,  316,  317,
  };

#line 1177 "CParser.jay"

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
  public const int VIRTUAL = 289;
  public const int OVERRIDE = 290;
  public const int TYPEDEF = 291;
  public const int EXTERN = 292;
  public const int STATIC = 293;
  public const int AUTO = 294;
  public const int REGISTER = 295;
  public const int INLINE = 296;
  public const int RESTRICT = 297;
  public const int CHAR = 298;
  public const int SHORT = 299;
  public const int INT = 300;
  public const int LONG = 301;
  public const int SIGNED = 302;
  public const int UNSIGNED = 303;
  public const int FLOAT = 304;
  public const int DOUBLE = 305;
  public const int CONST = 306;
  public const int VOLATILE = 307;
  public const int VOID = 308;
  public const int BOOL = 309;
  public const int COMPLEX = 310;
  public const int IMAGINARY = 311;
  public const int TRUE = 312;
  public const int FALSE = 313;
  public const int STRUCT = 314;
  public const int CLASS = 315;
  public const int UNION = 316;
  public const int ENUM = 317;
  public const int ELLIPSIS = 318;
  public const int CASE = 319;
  public const int DEFAULT = 320;
  public const int IF = 321;
  public const int ELSE = 322;
  public const int SWITCH = 323;
  public const int WHILE = 324;
  public const int DO = 325;
  public const int FOR = 326;
  public const int GOTO = 327;
  public const int CONTINUE = 328;
  public const int BREAK = 329;
  public const int RETURN = 330;
  public const int EOL = 331;
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
