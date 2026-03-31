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

  protected const int yyFinal = 29;
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
//t    "operator_name : '+'",
//t    "operator_name : '-'",
//t    "operator_name : '*'",
//t    "operator_name : '/'",
//t    "operator_name : '%'",
//t    "operator_name : EQ_OP",
//t    "operator_name : NE_OP",
//t    "operator_name : '<'",
//t    "operator_name : '>'",
//t    "operator_name : LE_OP",
//t    "operator_name : GE_OP",
//t    "operator_name : LEFT_OP",
//t    "operator_name : RIGHT_OP",
//t    "operator_name : '&'",
//t    "operator_name : '|'",
//t    "operator_name : '^'",
//t    "operator_name : '!'",
//t    "operator_name : '~'",
//t    "operator_name : AND_OP",
//t    "operator_name : OR_OP",
//t    "operator_name : INC_OP",
//t    "operator_name : DEC_OP",
//t    "operator_name : '='",
//t    "operator_name : ADD_ASSIGN",
//t    "operator_name : SUB_ASSIGN",
//t    "operator_name : MUL_ASSIGN",
//t    "operator_name : DIV_ASSIGN",
//t    "operator_name : MOD_ASSIGN",
//t    "operator_name : '(' ')'",
//t    "operator_name : '[' ']'",
//t    "direct_declarator_identifier_list : IDENTIFIER",
//t    "direct_declarator_identifier_list : '~' IDENTIFIER",
//t    "direct_declarator_identifier_list : OPERATOR operator_name",
//t    "direct_declarator_identifier_list : direct_declarator_identifier_list COLONCOLON IDENTIFIER",
//t    "direct_declarator_identifier_list : direct_declarator_identifier_list COLONCOLON '~' IDENTIFIER",
//t    "direct_declarator_identifier_list : direct_declarator_identifier_list COLONCOLON OPERATOR operator_name",
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
//t    "ctor_identifier_list : IDENTIFIER COLONCOLON TYPE_NAME",
//t    "ctor_identifier_list : IDENTIFIER COLONCOLON '~' TYPE_NAME",
//t    "ctor_identifier_list : ctor_identifier_list COLONCOLON TYPE_NAME",
//t    "ctor_identifier_list : ctor_identifier_list COLONCOLON '~' TYPE_NAME",
//t    "ctor_identifier_list : IDENTIFIER COLONCOLON OPERATOR operator_name",
//t    "ctor_identifier_list : ctor_identifier_list COLONCOLON OPERATOR operator_name",
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
    "VIRTUAL","OVERRIDE","OPERATOR","TYPEDEF","EXTERN","STATIC","AUTO",
    "REGISTER","INLINE","RESTRICT","CHAR","SHORT","INT","LONG","SIGNED",
    "UNSIGNED","FLOAT","DOUBLE","CONST","VOLATILE","VOID","BOOL",
    "COMPLEX","IMAGINARY","TRUE","FALSE","STRUCT","CLASS","UNION","ENUM",
    "ELLIPSIS","CASE","DEFAULT","IF","ELSE","SWITCH","WHILE","DO","FOR",
    "GOTO","CONTINUE","BREAK","RETURN","EOL",
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
#line 48 "CParser.jay"
  { var t = lexer.CurrentToken; yyVal = new VariableExpression((yyVals[0+yyTop]).ToString(), t.Location, t.EndLocation); }
  break;
case 2:
#line 49 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 50 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 51 "CParser.jay"
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 52 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 53 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 60 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 64 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 68 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 72 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 76 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 80 "CParser.jay"
  {
		yyVal = new MemberFromPointerExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 13:
#line 84 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 88 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 92 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 96 "CParser.jay"
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
#line 118 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 122 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 126 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 130 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 134 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 138 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 142 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 146 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 150 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 151 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 152 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 153 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 160 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 164 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 171 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 175 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 179 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 183 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 191 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 195 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 203 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 207 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 215 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 219 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 223 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 227 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 235 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 239 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 247 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 255 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 263 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 271 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 279 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 286 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 290 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 315 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 316 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 317 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 318 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 319 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 320 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 321 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 322 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 323 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 324 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 325 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 326 "CParser.jay"
  { yyVal = LogicOp.And; }
  break;
case 77:
#line 327 "CParser.jay"
  { yyVal = LogicOp.Or; }
  break;
case 78:
#line 334 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 79:
#line 338 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 81:
#line 349 "CParser.jay"
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
#line 428 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[0+yyTop], null);
	}
  break;
case 94:
#line 432 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[-2+yyTop], (Initializer)yyVals[0+yyTop]);
	}
  break;
case 95:
#line 436 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 96:
#line 437 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 97:
#line 438 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 98:
#line 439 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 99:
#line 440 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 100:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 101:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 102:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 103:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 104:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 105:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 106:
#line 450 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 107:
#line 451 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 108:
#line 452 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 109:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 110:
#line 454 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 111:
#line 455 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 114:
#line 458 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 117:
#line 467 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 118:
  case_118();
  break;
case 119:
#line 474 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 120:
#line 475 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 121:
#line 482 "CParser.jay"
  {
		yyVal = new List<BaseSpecifier> { (BaseSpecifier)yyVals[0+yyTop] };
	}
  break;
case 122:
  case_122();
  break;
case 123:
#line 491 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString()); }
  break;
case 124:
#line 492 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Public); }
  break;
case 125:
#line 493 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Private); }
  break;
case 126:
#line 494 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Protected); }
  break;
case 127:
#line 498 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 128:
#line 499 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 129:
#line 500 "CParser.jay"
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
#line 529 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 135:
#line 530 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 136:
#line 531 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 137:
#line 532 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 138:
#line 533 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 139:
  case_139();
  break;
case 140:
  case_140();
  break;
case 141:
#line 555 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 142:
#line 559 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 143:
#line 563 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 144:
#line 570 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 145:
#line 571 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 146:
#line 575 "CParser.jay"
  { yyVal = "+"; }
  break;
case 147:
#line 576 "CParser.jay"
  { yyVal = "-"; }
  break;
case 148:
#line 577 "CParser.jay"
  { yyVal = "*"; }
  break;
case 149:
#line 578 "CParser.jay"
  { yyVal = "/"; }
  break;
case 150:
#line 579 "CParser.jay"
  { yyVal = "%"; }
  break;
case 151:
#line 580 "CParser.jay"
  { yyVal = "=="; }
  break;
case 152:
#line 581 "CParser.jay"
  { yyVal = "!="; }
  break;
case 153:
#line 582 "CParser.jay"
  { yyVal = "<"; }
  break;
case 154:
#line 583 "CParser.jay"
  { yyVal = ">"; }
  break;
case 155:
#line 584 "CParser.jay"
  { yyVal = "<="; }
  break;
case 156:
#line 585 "CParser.jay"
  { yyVal = ">="; }
  break;
case 157:
#line 586 "CParser.jay"
  { yyVal = "<<"; }
  break;
case 158:
#line 587 "CParser.jay"
  { yyVal = ">>"; }
  break;
case 159:
#line 588 "CParser.jay"
  { yyVal = "&"; }
  break;
case 160:
#line 589 "CParser.jay"
  { yyVal = "|"; }
  break;
case 161:
#line 590 "CParser.jay"
  { yyVal = "^"; }
  break;
case 162:
#line 591 "CParser.jay"
  { yyVal = "!"; }
  break;
case 163:
#line 592 "CParser.jay"
  { yyVal = "~"; }
  break;
case 164:
#line 593 "CParser.jay"
  { yyVal = "&&"; }
  break;
case 165:
#line 594 "CParser.jay"
  { yyVal = "||"; }
  break;
case 166:
#line 595 "CParser.jay"
  { yyVal = "++"; }
  break;
case 167:
#line 596 "CParser.jay"
  { yyVal = "--"; }
  break;
case 168:
#line 597 "CParser.jay"
  { yyVal = "="; }
  break;
case 169:
#line 598 "CParser.jay"
  { yyVal = "+="; }
  break;
case 170:
#line 599 "CParser.jay"
  { yyVal = "-="; }
  break;
case 171:
#line 600 "CParser.jay"
  { yyVal = "*="; }
  break;
case 172:
#line 601 "CParser.jay"
  { yyVal = "/="; }
  break;
case 173:
#line 602 "CParser.jay"
  { yyVal = "%="; }
  break;
case 174:
#line 603 "CParser.jay"
  { yyVal = "()"; }
  break;
case 175:
#line 604 "CParser.jay"
  { yyVal = "[]"; }
  break;
case 176:
#line 611 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 177:
#line 615 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("~" + (yyVals[-1+yyTop]).ToString());
	}
  break;
case 178:
#line 619 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("operator" + (string)yyVals[0+yyTop]);
	}
  break;
case 179:
#line 621 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 180:
#line 623 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[-1+yyTop]).ToString()); }
  break;
case 181:
#line 625 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("operator" + (string)yyVals[0+yyTop]); }
  break;
case 183:
  case_183();
  break;
case 184:
#line 645 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 185:
#line 649 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 186:
#line 653 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 187:
#line 657 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 188:
#line 661 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 189:
#line 665 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 190:
#line 669 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 191:
#line 673 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 192:
#line 677 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 193:
  case_193();
  break;
case 194:
#line 689 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 195:
#line 693 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 196:
#line 694 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 197:
#line 695 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 198:
#line 696 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 199:
#line 700 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 200:
#line 704 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 201:
#line 708 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 202:
#line 709 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 203:
#line 710 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 204:
#line 717 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 205:
  case_205();
  break;
case 206:
  case_206();
  break;
case 207:
  case_207();
  break;
case 208:
#line 745 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 209:
#line 749 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 210:
#line 753 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 211:
#line 757 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 212:
#line 764 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 213:
#line 768 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 214:
#line 775 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 216:
#line 780 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 217:
  case_217();
  break;
case 218:
#line 799 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 219:
#line 803 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 220:
#line 807 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 221:
#line 811 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 222:
#line 815 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 223:
#line 819 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 224:
#line 823 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 225:
#line 827 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 226:
#line 831 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 227:
#line 835 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 228:
#line 842 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 229:
#line 846 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 230:
#line 850 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 231:
  case_231();
  break;
case 232:
  case_232();
  break;
case 233:
  case_233();
  break;
case 234:
  case_234();
  break;
case 235:
#line 890 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 247:
#line 920 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 248:
#line 924 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 249:
#line 928 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 250:
#line 929 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 253:
#line 941 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 254:
#line 945 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 255:
#line 949 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 256:
#line 950 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 260:
#line 960 "CParser.jay"
  {
		yyVal = new VirtualDeclarationStatement((Statement)yyVals[0+yyTop]) { IsVirtual = true };
	}
  break;
case 261:
  case_261();
  break;
case 262:
  case_262();
  break;
case 263:
#line 974 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 264:
  case_264();
  break;
case 265:
#line 988 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 266:
#line 989 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 267:
#line 990 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 268:
#line 997 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 269:
#line 1001 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 270:
  case_270();
  break;
case 271:
#line 1019 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 272:
#line 1023 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 273:
#line 1030 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 274:
#line 1034 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 275:
#line 1038 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-4+yyTop], (List<SwitchCase>)yyVals[-1+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 276:
#line 1042 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-3+yyTop], new List<SwitchCase>(), GetLocation(yyVals[-5+yyTop]));
	}
  break;
case 277:
#line 1049 "CParser.jay"
  {
		yyVal = new List<SwitchCase> { (SwitchCase)yyVals[0+yyTop] };
	}
  break;
case 278:
  case_278();
  break;
case 279:
#line 1061 "CParser.jay"
  {
		yyVal = new SwitchCase((Expression)yyVals[-2+yyTop], (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 280:
#line 1065 "CParser.jay"
  {
		yyVal = new SwitchCase(null, (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 281:
#line 1072 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 282:
#line 1076 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 283:
#line 1080 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 284:
#line 1084 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 285:
#line 1088 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 286:
#line 1092 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 288:
#line 1100 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 289:
#line 1104 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 290:
#line 1108 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 291:
#line 1112 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 292:
  case_292();
  break;
case 293:
  case_293();
  break;
case 298:
  case_298();
  break;
case 299:
  case_299();
  break;
case 300:
#line 1160 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 301:
#line 1164 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-2+yyTop]).ToString()).Push((yyVals[0+yyTop]).ToString());
	}
  break;
case 302:
#line 1168 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("~" + (yyVals[0+yyTop]).ToString());
	}
  break;
case 303:
#line 1169 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 304:
#line 1170 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
  break;
case 305:
#line 1172 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("operator" + (string)yyVals[0+yyTop]); }
  break;
case 306:
#line 1174 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("operator" + (string)yyVals[0+yyTop]); }
  break;
case 307:
  case_307();
  break;
case 308:
  case_308();
  break;
case 309:
  case_309();
  break;
case 310:
  case_310();
  break;
case 311:
  case_311();
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
#line 101 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 107 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 296 "CParser.jay"
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
#line 351 "CParser.jay"
{
		DeclarationSpecifiers ds = (DeclarationSpecifiers)yyVals[-2+yyTop];
		List<InitDeclarator> decls = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_83()
#line 360 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_84()
#line 366 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];		
		yyVal = ds;
	}

void case_85()
#line 372 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_86()
#line 378 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_87()
#line 384 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 390 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 396 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_90()
#line 402 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_91()
#line 411 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_92()
#line 417 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_118()
#line 469 "CParser.jay"
{
		var ts = new TypeSpecifier((TypeSpecifierKind)yyVals[-4+yyTop], (yyVals[-3+yyTop]).ToString(), (Block)yyVals[0+yyTop]);
		ts.BaseSpecifiers = (List<BaseSpecifier>)yyVals[-1+yyTop];
		yyVal = ts;
	}

void case_122()
#line 484 "CParser.jay"
{
		((List<BaseSpecifier>)yyVals[-2+yyTop]).Add((BaseSpecifier)yyVals[0+yyTop]);
		yyVal = yyVals[-2+yyTop];
	}

void case_130()
#line 505 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_131()
#line 510 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_132()
#line 516 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_133()
#line 521 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_139()
#line 538 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_140()
#line 544 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_183()
#line 631 "CParser.jay"
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

void case_193()
#line 679 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<Expression>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(ctorArgumentValue: n));
		}
		yyVal = d;
	}

void case_205()
#line 719 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_206()
#line 728 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_207()
#line 734 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_217()
#line 785 "CParser.jay"
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

void case_231()
#line 855 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_232()
#line 862 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_233()
#line 870 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_234()
#line 877 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_261()
#line 962 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsOverride = true };
	}

void case_262()
#line 967 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsOverride = true };
	}

void case_264()
#line 979 "CParser.jay"
{
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator((Declarator)yyVals[-3+yyTop], null) };
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-4+yyTop], decls);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsPureVirtual = true };
	}

void case_270()
#line 1006 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		DeclarationSpecifiers ds = new DeclarationSpecifiers();
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator(fdecl, null) };
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_278()
#line 1051 "CParser.jay"
{
		((List<SwitchCase>)yyVals[-1+yyTop]).Add((SwitchCase)yyVals[0+yyTop]);
		yyVal = yyVals[-1+yyTop];
	}

void case_292()
#line 1117 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_293()
#line 1122 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_298()
#line 1137 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_299()
#line 1146 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_307()
#line 1179 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_308()
#line 1188 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_309()
#line 1200 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_310()
#line 1206 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_311()
#line 1212 "CParser.jay"
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
   43,   43,   30,   32,   32,   46,   46,   46,   46,   46,
   46,   46,   46,   46,   46,   46,   46,   46,   46,   46,
   46,   46,   46,   46,   46,   46,   46,   46,   46,   46,
   46,   46,   46,   46,   46,   47,   47,   47,   47,   47,
   47,   45,   45,   45,   45,   45,   45,   45,   45,   45,
   45,   45,   45,   45,   44,   44,   44,   44,   48,   48,
   29,   29,   29,   49,   49,   50,   50,   51,   51,   51,
   51,    5,    5,   52,   52,   52,   53,   53,   53,   53,
   53,   53,   53,   53,   53,   53,   53,   33,   33,   33,
    6,    6,    6,    6,   54,   55,   55,   56,   56,   57,
   57,   57,   57,   57,   57,   58,   59,   59,   64,   64,
   65,   65,   38,   38,   66,   66,   67,   67,   67,   67,
   67,   67,   67,   70,   68,   68,   68,   71,   71,   69,
   60,   60,   61,   61,   61,   61,   72,   72,   73,   73,
   62,   62,   62,   62,   62,   62,   63,   63,   63,   63,
   63,    0,    0,   74,   74,   74,   74,   75,   75,   78,
   78,   78,   78,   78,   78,   78,   76,   76,   77,   77,
   77,   79,   79,   79,
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
    1,    3,    1,    2,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    2,    2,    1,    2,    2,    3,    4,
    4,    1,    3,    5,    4,    4,    6,    6,    5,    4,
    3,    4,    4,    3,    1,    2,    2,    3,    1,    2,
    1,    1,    1,    1,    3,    1,    3,    2,    4,    2,
    1,    1,    2,    1,    1,    2,    3,    2,    3,    3,
    4,    3,    4,    2,    3,    3,    4,    1,    3,    4,
    1,    2,    3,    4,    2,    1,    2,    3,    2,    1,
    1,    1,    1,    1,    1,    3,    2,    3,    1,    2,
    1,    1,    2,    3,    1,    2,    1,    1,    1,    2,
    4,    5,    2,    5,    2,    2,    2,    4,    3,    2,
    1,    2,    5,    7,    7,    6,    1,    2,    4,    3,
    5,    7,    6,    7,    6,    7,    3,    2,    2,    2,
    3,    1,    2,    1,    1,    1,    1,    4,    3,    1,
    3,    4,    3,    4,    4,    4,    4,    5,    1,    2,
    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,    0,   95,   96,   97,   98,   99,  143,  202,  101,
  102,  103,  104,  107,  108,  105,  106,  201,  203,  100,
  109,  110,  111,  127,  128,  129,    0,  296,    0,  295,
    0,    0,    0,    0,    0,  112,  113,    0,  292,  294,
  297,    0,    0,  115,  116,    0,    0,  293,  176,    0,
    0,    0,    0,   81,    0,   91,    0,    0,    0,    0,
  114,   84,   86,   88,   90,    0,    0,  119,    0,    0,
  301,    0,    0,    0,    0,  139,    0,  166,  167,  157,
  158,  155,  156,  151,  152,  164,  165,  171,  172,  173,
  169,  170,    0,    0,  159,  148,  146,  147,  163,  162,
  149,  150,  153,  154,  161,  160,  168,  178,    0,  199,
  197,    0,  177,    0,   82,  312,    0,    0,  313,  314,
  309,    0,  299,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  253,  257,    0,    0,    0,  255,  258,
  259,    0,    0,  117,  303,    0,    0,    0,    0,    0,
    0,  206,  305,  302,    0,  134,    0,    0,  174,  175,
  183,  200,  198,   92,    0,    0,    2,    3,    0,    0,
    0,    4,    5,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  247,    0,    0,   27,   28,   29,   30,
  271,    7,    0,    0,   78,    0,   33,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   63,  251,
  252,  240,  241,  242,  243,  244,  245,    0,  249,    1,
    0,  228,   94,  311,  298,  310,  194,    0,   17,    0,
    0,  191,    0,    0,    0,  179,    0,    0,  265,  266,
  267,  260,    0,  263,    0,    0,  254,  256,  270,    0,
    0,    0,  123,    0,  121,  306,  304,  307,    0,    0,
    0,    0,  210,    0,    0,    0,   31,   80,  142,  136,
  140,  135,    0,    0,    0,   25,    0,   20,   21,    0,
    0,    0,    0,    0,    0,  288,  289,  290,    0,    0,
    0,    0,    0,    0,   22,   23,    0,  272,    0,   13,
   14,    0,    0,    0,   66,   67,   68,   69,   70,   71,
   72,   73,   74,   75,   76,   77,   65,    0,   24,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  248,  250,
    0,    0,    0,  231,    0,    0,  236,  193,    0,  192,
    0,  190,  186,    0,  185,    0,    0,  181,  180,    0,
    0,    0,  269,    0,  124,  125,  126,    0,  118,  224,
    0,    0,  218,    0,    0,    0,    0,    0,    0,  308,
  205,  207,  137,  246,    0,    0,    0,    0,    0,    0,
    0,    0,  287,  291,    6,    0,  130,  132,    0,    0,
  213,   79,   12,    9,    0,    0,   11,   64,   34,   35,
   36,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  239,  229,
    0,  232,  235,  237,   18,    0,    0,  189,  184,    0,
    0,  261,  268,  122,  225,  217,  222,  219,  209,  226,
    0,  220,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   32,   10,    8,    0,  238,  230,  233,
    0,  187,  188,  262,    0,  227,  223,  221,    0,    0,
  281,    0,    0,    0,    0,    0,    0,   62,  234,  264,
    0,    0,    0,  276,    0,  277,    0,  285,    0,  283,
    0,   15,    0,  274,    0,    0,  275,  278,  282,  286,
  284,   16,    0,    0,    0,
  };
  protected static readonly short [] yyDgoto  = {            29,
  192,  193,  194,  228,  291,  343,  195,  196,  197,  198,
  199,  200,  201,  202,  203,  204,  205,  206,  207,  208,
  209,  318,  269,  210,  122,   55,   32,   33,   34,   35,
   56,  165,  223,   36,   37,  253,   38,   68,  254,  255,
  294,   75,   76,   58,   59,  108,   60,  112,  371,  151,
  152,  372,  264,  345,  346,  347,  211,  212,  213,  214,
  215,  216,  217,  218,  219,  138,  139,  140,  141,  244,
  142,  495,  496,   39,   40,   41,  124,   42,  125,
  };
  protected static readonly short [] yySindex = {         2121,
 -193,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  -92,    0, 2121,    0,
  597, 3475, 3475, 3475, 3475,    0,    0,  263,    0,    0,
    0,  -22,  -84,    0,    0, -176,   -3,    0,    0, 3370,
  -29,   -1, -155,    0,   24,    0,  831,   33,  108, -162,
    0,    0,    0,    0,    0, 3363,   21,    0,   93, 1998,
    0, 3370, -116,  120,   15,    0, -176,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  156,  162,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  229,    0,
    0,   -1,    0,  -29,    0,    0,  276, 1219,    0,    0,
    0,  597,    0, 3291, 3475,  108,  902, 1141,   60,  246,
  267,  271, 3475,    0,    0,  597,  -16, 3419,    0,    0,
    0,  285,  255,    0,    0, 3370,   61,  231,  368,  349,
  348,    0,    0,    0,  721,    0,  -99,   20,    0,    0,
    0,    0,    0,    0,  337,  353,    0,    0, 1680, 1695,
 1695,    0,    0,  367,  383,  385,  581,  388,  173,  381,
  458,  913, 1112,    0,  721,  721,    0,    0,    0,    0,
    0,    0,   34,  132,    0, 1781,    0,  721,   47,   67,
  -62,  -17,  193,  404,  425,  402,  266,  -42,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  384,    0,    0,
 1314,    0,    0,    0,    0,    0,    0,  199,    0,  504,
 -216,    0, 1349,  462, 1211,    0, 3370,  303,    0,    0,
    0,    0,  597,    0,  -39, 2026,    0,    0,    0, -187,
 -187, -187,    0,   12,    0,    0,    0,    0, 1850, 1452,
  503,  265,    0,  166,  231, 3447,    0,    0,    0,    0,
    0,    0,  -74,  581, 1112,    0, 1112,    0,    0,  721,
  721,  721,  240, 1018,  507,    0,    0,    0,   36,  286,
  526, 3142, 3142,  297,    0,    0,  721,    0,  338,    0,
    0, 1418,  721,  339,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  721,    0,  721,
  721,  721,  721,  721,  721,  721,  721,  721,  721,  721,
  721,  721,  721,  721,  721,  721,  721,  721,    0,    0,
  721,  340,   52,    0, 1219,  468,    0,    0,  721,    0,
  729,    0,    0,  721,    0, 1496,  516,    0,    0,  -27,
  538,  551,    0,  571,    0,    0,    0,  255,    0,    0,
  572,  574,    0, 1510,  523,  721,  166, 2065, 1542,    0,
    0,    0,    0,    0,  576,  577,  365,  374,  408,  580,
 1557, 1557,    0,    0,    0, 1518,    0,    0, 1943,  312,
    0,    0,    0,    0,  409,   32,    0,    0,    0,    0,
    0,   47,   47,   67,   67,  -62,  -62,  -62,  -62,  -17,
  -17,  193,  404,  425,  402,  266,   46,  529,    0,    0,
  612,    0,    0,    0,    0,  534,  535,    0,    0,  573,
 1581,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  589,    0, 1600,  540,  508,  508,  581,  511,  581,  721,
 1615, 1658, 1314,    0,    0,    0,  721,    0,    0,    0,
 1219,    0,    0,    0,  579,    0,    0,    0,  313,  -96,
    0,  410,  581,  414,  581,  467,   69,    0,    0,    0,
  581,  721,  590,    0,  -79,    0,  592,    0,  581,    0,
  581,    0, 1253,    0,  591,  485,    0,    0,    0,    0,
    0,    0,  485,  485,  485,
  };
  protected static readonly short [] yyRindex = {            0,
    0, 1907,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  372, 1181, 1704, 2508,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1720,    0,    0,    0,
    0,  -25,    0,    0,    0,    0,  170,    0,  714,   65,
    0,    0,    0,    0,    0,    0, 1793,    0,    0,    0,
    0,    0,    0,   71,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  130,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  790,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  472,    0,
  594,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  -21, 3287,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2404,    0, 2462,    0,    0, 2472, 2719,
 2795, 2994, 3126, 3102, 3033, 1842, 3220,  146,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  480,  505,    0,  506,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   45,  424,  619,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  -21,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  510,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  620,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2597, 2710, 2747, 2756, 2832, 2841, 2869, 2909, 3031,
 3089, 3128, 3149, 3165, 3213, 3251,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2433,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2349,    0,    0,    0,  175,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -75,  -56,
  };
  protected static readonly short [] yyGindex = {            0,
    0, -128,    0,  351, -161,  201, -118,  -48, -158,    0,
  208,  227,  118,  226,  329,  331,  332,  330,  333,    0,
 -152,    0, -316,    8,    1,  -88,    0,  164,  -40,    0,
  554,  161, -172,    0,    0,   91,    0,  -28,    0,  304,
  270,  596, -105,  -50,  -51,  -15,  -19,  -65,  -66,    0,
  405, -127, -218, -373,    0,  328, -163,    0,    3, -231,
    0,    0,    0, -117, -212,    0,  537,    0,    0,    0,
    0,    0,  210,  687,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           222,
   31,  111,  268,  150,  114,  340,  126,   30,  229,  234,
   51,  110,   52,  283,  195,  195,  114,   70,  195,  115,
  338,  263,   93,  246,  428,  270,  295,  296,  494,   31,
   46,  115,   62,   63,   64,   65,   30,   93,  144,  319,
   52,   73,  329,  377,  330,  507,  137,  245,  344,  280,
  383,  271,  392,  289,  290,  368,  153,  471,  157,  123,
  230,  163,  235,  273,  121,  195,  136,  114,  279,   44,
  149,  162,   51,  135,  296,  297,   43,  297,  143,  297,
   74,    9,  115,  322,  131,  131,  131,  110,  320,  297,
   18,   19,  298,  321,  394,  431,   53,   45,  262,  182,
  195,  113,  222,  467,  182,  182,  267,  129,  182,  323,
  384,  324,  503,  385,  141,  386,  357,   47,  137,   77,
  276,  278,  279,  182,  466,  182,  225,  149,   67,  471,
  256,  224,  226,  243,   66,  131,  267,  267,  136,  156,
  242,  375,  293,   66,  272,  135,  290,  127,  290,  267,
  258,  387,  388,  389,  360,  182,  182,   74,   53,  461,
  462,  409,  410,  411,   44,  351,  401,  271,  154,  196,
  196,  302,  432,  196,  406,  505,  430,  304,  402,  364,
  155,  377,   74,  229,  267,  238,   61,  182,  268,   61,
  110,   57,   45,  502,  162,  141,  159,  296,  128,  408,
   71,  325,  326,   61,   61,  378,   72,  273,  262,  427,
  126,  109,  273,   93,  273,  296,  273,  273,  147,  273,
  196,  358,  303,  492,  493,  369,  222,   49,   93,  337,
  435,  195,  436,  273,  293,  437,  293,  464,   61,  348,
  492,  493,  349,  400,  280,  280,  149,   69,  327,  328,
  362,  293,  293,  129,  160,  196,  379,  449,  470,  149,
  454,   50,  440,  279,  279,  195,  149,  380,   93,  161,
   61,  267,  267,  267,  267,  267,  267,  267,  267,  267,
  267,  267,  267,  267,  267,  267,  267,  267,  267,   49,
  344,  391,  267,  479,  296,  481,    9,  273,  489,  273,
  273,  340,  340,  239,  259,   18,   19,  267,  190,  261,
  162,  451,  222,  185,  488,  183,  236,  186,  187,  498,
  188,  500,  222,   50,  240,  267,  395,  504,  241,  297,
  470,  482,  484,  486,  191,  510,  399,  511,   52,  268,
  365,  366,  367,  249,  222,  257,  292,  267,  400,  182,
  237,  399,  222,  117,  182,  260,  182,  182,  182,  182,
  182,  182,  182,  182,  182,  182,  182,  182,  182,  182,
  182,  182,  182,  182,  182,  182,  182,  145,  149,  182,
  182,  182,  182,  146,  222,   66,  196,  260,  514,  265,
   53,  266,  299,  300,  301,  515,  182,  118,  117,  149,
  184,  189,  260,  361,  267,  457,  280,  259,  297,   52,
  274,   83,   83,   83,  458,   83,  190,  297,  267,  109,
  196,  185,  281,  183,  282,  186,  187,  284,  188,  285,
   83,  273,  273,  273,  273,   61,  273,  273,  292,  286,
  292,  333,  191,  267,  416,  417,  418,  419,  459,  465,
  497,  297,  349,  297,  499,  292,  292,  297,  260,  273,
  331,  332,   83,  133,  133,  133,  273,  273,  273,  273,
  273,  273,  273,  273,  273,  273,  273,  273,  273,  273,
  273,  273,  273,  273,  273,  273,  273,  273,  273,  273,
  273,  273,  273,   53,  273,  273,  273,   83,  273,  273,
  273,  273,  273,  273,  273,  273,  117,  501,  339,  189,
  297,   44,  211,  342,  133,  211,  287,  190,  334,   44,
  208,   49,  185,  208,  183,  335,  186,  187,  433,  188,
  412,  413,  166,  167,  168,  169,  336,  170,  171,   45,
  250,  251,  252,  191,  350,  214,  215,   45,  214,  215,
  216,  414,  415,  216,  353,   50,  420,  421,  341,  359,
   61,  397,  398,  376,  390,  393,  396,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,  172,  173,
   24,   25,   26,   27,  403,  407,  429,  174,  441,  175,
  176,  177,  178,  179,  180,  181,  182,  117,  439,  442,
  189,  443,  445,  190,  446,  448,  455,  456,  185,  460,
  183,  468,  186,  187,   49,  188,  472,  473,   83,  476,
  463,  474,  478,  480,  204,  491,   51,  490,   52,  191,
  166,  167,  168,  169,  190,  170,  171,  506,  513,  185,
  509,  183,  405,  186,  187,   54,  188,  342,   50,  212,
  214,  422,   83,  487,  423,  425,  424,  164,   61,  426,
  382,  444,  158,  434,  248,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,  172,  173,   24,   25,
   26,   27,  341,  117,  508,  174,  189,  175,  176,  177,
  178,  179,  180,  181,  182,   48,    0,    0,    0,    0,
    0,    0,   53,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  221,    0,  469,  189,    0,    0,
    0,  166,  167,  168,  169,    0,  170,  171,  145,    0,
    0,    0,    0,  190,  145,    0,    0,  145,  185,    0,
  183,  190,  186,  187,    0,  188,  185,    0,  183,   61,
  186,  187,  145,  188,  145,    0,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,  172,  173,   24,
   25,   26,   27,    0,    0,  145,  174,    0,  175,  176,
  177,  178,  179,  180,  181,  182,    0,    0,    0,    0,
    0,    0,    0,    0,  144,    0,    0,    0,    0,    0,
  144,    0,    0,  144,    0,    0,  145,  166,  167,  168,
  169,    0,  170,  171,    0,    0,  189,    0,  144,    0,
  144,    0,    0,   49,  189,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  119,    0,    0,  220,  167,
  168,  169,    0,  170,  171,    0,    0,    0,    0,    0,
    0,  144,    0,    0,    0,    0,    0,   50,    0,    0,
    0,  118,    0,  172,  173,    0,    0,    0,    0,    0,
    0,    0,  174,    0,  175,  176,  177,  178,  179,  180,
  181,  182,  144,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  120,    0,  172,  173,    0,    0,    0,    0,
    0,    0,    0,    0,  190,    0,    0,    0,    0,  185,
    0,  183,  227,  186,  187,  190,  188,    0,    0,    0,
  185,    0,  183,  117,  186,  187,    0,  188,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  288,    0,    0,    0,    0,    0,  220,  167,  168,
  169,    0,  170,  171,    0,  220,  167,  168,  169,    0,
  170,  171,    0,    0,    0,    0,    0,    0,  145,    0,
    0,    0,    0,  145,    0,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  145,  145,    9,  189,  145,  145,
  145,  145,    0,  172,  173,   18,   19,    0,  189,    0,
    0,  172,  173,    0,    0,  145,    0,    0,    0,    0,
  190,    0,    0,    0,    0,  185,    0,  183,    0,  186,
  187,    0,  188,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  144,    0,  191,    0,    0,  144,
    0,  144,  144,  144,  144,  144,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,  144,  144,  144,
  144,  144,    0,    0,  144,  144,  144,  144,    0,    0,
    0,    0,    0,    0,    0,   61,    0,    0,    0,    0,
    0,  144,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,  189,  190,   24,   25,   26,   27,  185,
    0,  183,    0,  186,  187,    0,  188,    0,  220,  167,
  168,  169,  116,  170,  171,    0,    0,    0,    0,  220,
  167,  168,  169,  190,  170,  171,    0,    0,  185,    0,
  183,    0,  233,  187,    0,  188,   61,    0,    0,    0,
    0,    0,    0,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,  172,  173,   24,   25,   26,   27,
   85,   85,   85,    0,   85,  172,  173,    0,    0,    0,
    0,    0,    0,  232,    0,    0,    0,  189,    0,   85,
    0,    0,    0,  190,    0,    0,    0,    0,  185,    0,
  183,  190,  356,  187,    0,  188,  185,    0,  183,    0,
  186,  187,    0,  188,    0,    0,  189,    0,    0,    0,
    0,   85,    0,    0,  220,  167,  168,  169,    0,  170,
  171,    0,    0,    0,    0,  190,    0,    0,    0,    0,
  185,    0,  183,    0,  186,  187,    0,  188,  342,    0,
    0,    0,   61,  355,    0,    0,   85,    0,    0,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
  172,  173,   24,   25,   26,   27,  189,    0,    0,    0,
    0,  221,    0,  341,  189,    0,  190,    0,    0,    0,
    0,  185,    0,  183,    0,  186,  187,    0,  188,  342,
    0,    0,    0,    0,    0,    0,    0,    0,  220,  167,
  168,  169,    0,  170,  171,  221,    0,  512,  189,    0,
    0,  190,    0,    0,    0,    0,  185,    0,  183,    0,
  186,  187,    0,  188,    0,    0,   61,  220,  167,  168,
  169,    0,  170,  171,  341,    0,    0,    0,    0,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,  172,  173,   24,   25,   26,   27,
    0,    0,    0,    0,  231,    0,  221,   85,    9,  189,
    0,  352,    0,    0,    0,    0,    0,   18,   19,    0,
  190,    0,    0,  172,  173,  185,    0,  183,  404,  186,
  187,    0,  188,    0,    0,    0,    0,  220,  167,  168,
  169,   85,  170,  171,  189,  220,  167,  168,  169,    0,
  170,  171,    0,    0,  190,    0,    0,    0,    0,  185,
    0,  183,    0,  374,  187,    0,  188,    0,    0,    0,
    0,    0,    0,    0,  354,    0,    0,    0,    9,  220,
  167,  168,  169,    0,  170,  171,    0,   18,   19,    0,
    0,    0,    0,  172,  173,    0,    0,    0,  190,    0,
    0,  172,  173,  185,    0,  183,    0,  186,  187,    0,
  188,    0,  190,  189,  373,    0,    0,  185,    0,  183,
  190,  186,  187,    0,  188,  185,    0,  183,    0,  186,
  187,    0,  188,    0,    0,  172,  173,    0,    0,    0,
  220,  167,  168,  169,  190,  170,  171,  189,    0,  185,
    0,  183,    0,  453,  187,    0,  188,    0,  438,  190,
    0,    0,    0,    0,  185,    0,  183,    0,  186,  187,
    0,  188,  447,    0,    0,  220,  167,  168,  169,    0,
  170,  171,    0,  190,    0,  191,    0,    0,  185,    0,
  183,  189,  186,  187,    0,  188,  172,  173,    0,    0,
    0,    0,  190,    0,  452,  189,    0,  185,    0,  183,
  463,  186,  187,  189,  188,    0,    0,  190,    0,    0,
    0,    0,  185,    0,  183,  483,  186,  187,    0,  188,
    0,  172,  173,    0,    0,    0,    0,  189,    0,    0,
    0,    0,    0,    0,  220,  167,  168,  169,    0,  170,
  171,    0,  189,    0,    0,    0,    0,    0,    0,    0,
  190,    0,  477,    0,    0,  185,    0,  183,  485,  186,
  187,    0,  188,  221,    0,    0,  189,    0,  220,  167,
  168,  169,  190,  170,  171,    0,    0,  185,    0,  275,
    0,  186,  187,    0,  188,  189,    0,  190,    0,    0,
  172,  173,  185,    0,  277,    0,  186,  187,    0,  188,
  189,    0,    0,   87,   87,   87,    0,   87,    0,    0,
    0,    0,  220,  167,  168,  169,    0,  170,  171,  138,
  138,  138,   87,  138,  172,  173,  220,  167,  168,  169,
    0,  170,  171,    0,  220,  167,  168,  169,  138,  170,
  171,    0,    0,  189,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   87,    0,    0,    0,  220,  167,
  168,  169,    0,  170,  171,  189,    0,    0,  172,  173,
  138,    0,    0,  220,  167,  168,  169,    0,  170,  171,
  189,    0,  172,  173,    0,    0,    0,    0,    0,   87,
  172,  173,  120,  120,  120,    0,  120,  220,  475,  168,
  169,  317,  170,  171,    0,  138,    0,    0,    0,    0,
    0,  120,    0,    0,  172,  173,  220,  167,  168,  169,
    0,  170,  171,    0,    0,    0,    0,    0,    0,  172,
  173,  220,  167,  168,  169,    0,  170,  171,    0,    0,
    0,    0,   57,  120,    0,   57,    0,    0,    0,  259,
  370,   52,    0,  172,  173,    0,    0,    0,    0,   57,
   57,    0,    0,    0,   57,    0,    0,    0,    0,    0,
    0,    0,  172,  173,  220,  167,  168,  169,  120,  170,
  171,    0,    0,    0,    0,    0,    0,  172,  173,    0,
    0,    0,    0,    0,   57,    0,  220,  167,  168,  169,
  260,  170,  171,    0,    0,    0,  114,    0,  114,    0,
    0,  220,  167,  168,  169,    0,  170,  171,    0,    0,
   87,    0,    0,    0,    0,  114,   57,    0,    0,    0,
  172,  173,    0,    0,    0,   53,  138,    0,    0,    0,
    0,    0,  399,  370,   52,    0,    0,    0,    0,    0,
    0,    0,  172,  173,   87,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  138,    0,    0,  172,  173,    0,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  114,  260,  138,  138,  138,  138,  148,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  120,
    0,    0,    0,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,    0,  363,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  120,    0,    0,
    0,    0,    0,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  450,   49,  120,  120,  120,
  120,    0,   57,   57,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   57,    0,    0,   61,    0,    0,    0,    0,    0,
   50,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,    0,  114,   24,   25,   26,   27,    0,    0,
    0,    0,    0,    0,    0,    0,  300,    0,    0,   28,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  114,    0,    0,    0,    0,    0,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,    0,
    0,  114,  114,  114,  114,    0,    0,   61,    0,    0,
    0,    0,    0,    0,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,    0,    0,   24,   25,   26,
   27,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   61,    0,    0,    0,    0,    0,    0,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   61,    0,   24,   25,   26,   27,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,    0,    0,
   24,   25,   26,   27,    0,    0,    0,    0,    0,   61,
    0,    0,    0,    0,    0,    0,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,    1,    0,   24,
   25,   26,   27,    0,    0,    2,    2,    0,    2,    0,
    2,    2,    2,    2,    2,    2,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    2,    0,    0,    2,    2,
    2,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,    0,    0,   24,   25,   26,   27,    2,
   19,   19,    2,    0,   19,   19,   19,   19,   19,    0,
   19,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   19,   19,   19,   19,   19,   19,    0,    0,   26,
   26,    0,    2,   26,   26,   26,   26,   26,    0,   26,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   26,   26,   26,   26,   26,   26,   19,   19,   31,   31,
    0,    0,   31,   31,   31,   31,   31,    0,   31,   37,
    0,    0,   37,    0,   37,   37,   37,    0,    0,   31,
   31,   31,    0,   31,   31,   26,   26,   19,   19,   37,
   37,   37,    0,   37,   37,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   89,   89,   89,
    0,   89,    0,    0,   31,   31,   26,   26,    0,    0,
    0,    0,    0,    0,   37,   37,   89,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   31,   31,    0,    0,    0,
    0,    0,    0,    0,    0,   37,   37,    0,   89,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    2,
    2,    2,    2,    2,    2,    2,    2,    2,    0,    2,
    2,    2,    2,    2,    2,    2,    2,    2,    2,    2,
    2,    2,    2,   89,   38,    0,    0,   38,    2,   38,
   38,   38,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   38,   38,   38,    0,   38,   38,
    0,    0,    0,    0,    0,    0,    0,   19,   19,   19,
   19,   19,   19,    0,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,    0,   38,
   38,    0,    0,   19,    0,    0,   26,   26,   26,   26,
   26,   26,    0,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,    0,    0,    0,
   38,   38,   26,    0,    0,   31,   31,   31,   31,   31,
   31,    0,   31,   31,    0,   37,   37,   37,   37,   37,
   37,    0,   37,   37,    0,    0,    0,   39,    0,    0,
   39,   31,   39,   39,   39,    0,   40,    0,    0,   40,
    0,   37,   40,    0,   89,    0,    0,   39,   39,   39,
    0,   39,   39,    0,    0,    0,   40,   40,   40,    0,
   40,   40,    0,    0,   41,    0,    0,   41,    0,    0,
   41,    0,    0,   42,    0,    0,   42,    0,   89,   42,
    0,    0,   39,   39,   41,   41,   41,    0,   41,   41,
    0,   40,   40,   42,   42,   42,    0,   42,   42,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   43,   39,   39,   43,    0,    0,   43,   41,
   41,    0,   40,   40,    0,    0,    0,    0,   42,   42,
    0,    0,   43,   43,   43,    0,   43,   43,    0,    0,
   38,   38,   38,   38,   38,   38,    0,   38,   38,   46,
   41,   41,   46,    0,    0,   46,    0,    0,   47,   42,
   42,   47,    0,    0,   47,    0,   38,   43,   43,   46,
   46,   46,    0,   46,   46,    0,    0,    0,   47,   47,
   47,    0,   47,   47,    0,    0,   44,    0,    0,   44,
    0,    0,   44,    0,    0,    0,    0,    0,   43,   43,
    0,    0,    0,    0,   46,   46,   44,   44,   44,    0,
   44,   44,    0,   47,   47,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   45,    0,    0,   45,
    0,    0,   45,    0,    0,   46,   46,    0,    0,    0,
    0,   44,   44,    0,   47,   47,   45,   45,   45,    0,
   45,   45,    0,   39,   39,   39,   39,   39,   39,    0,
   39,   39,   40,   40,   40,   40,   40,   40,    0,   40,
   40,    0,   44,   44,    0,    0,    0,    0,    0,   39,
    0,   45,   45,    0,    0,    0,    0,    0,   40,    0,
   41,   41,   41,   41,   41,   41,    0,   41,   41,   42,
   42,   42,   42,   42,   42,    0,   42,   42,    0,    0,
    0,   48,   45,   45,   48,    0,   41,   48,    0,    0,
    0,    0,    0,    0,    0,   42,    0,    0,    0,    0,
    0,   48,   48,    0,    0,    0,   48,    0,    0,    0,
   43,   43,   43,   43,    0,   43,   43,    0,   49,    0,
    0,   49,    0,   55,   49,    0,   55,    0,    0,    0,
    0,    0,    0,    0,   43,    0,   48,   48,   49,   49,
   55,   55,    0,   49,    0,   55,    0,   46,   46,   46,
   46,    0,   46,   46,    0,    0,   47,   47,   47,   47,
    0,   47,   47,    0,    0,    0,    0,   48,   48,    0,
    0,   46,    0,   49,   49,   55,   50,    0,    0,   50,
   47,    0,   50,    0,   44,   44,   44,   44,    0,   44,
   44,    0,   53,    0,    0,   53,   50,   50,    0,    0,
    0,   50,    0,    0,   49,   49,   55,   55,   44,   53,
   53,    0,    0,   51,   53,   52,   51,    0,   52,   51,
    0,   52,    0,    0,   45,   45,   45,   45,    0,   45,
   45,   50,   50,   51,   51,   52,   52,    0,   51,   54,
   52,    0,   54,    0,   53,   53,    0,    0,   45,    0,
    0,    0,    0,    0,    0,   56,   54,   54,   56,    0,
    0,   54,   50,   50,    0,    0,    0,    0,   51,   51,
   52,   52,   56,   56,    0,   53,   53,   56,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   54,   54,    0,    0,    0,    0,    0,    0,   51,
   51,   52,   52,   58,    0,    0,   58,   56,    0,    0,
   59,   48,   48,   59,   48,   48,    0,    0,    0,    0,
   58,   58,   54,   54,    0,   58,    0,   59,   59,    0,
    0,    0,   59,   48,    0,    0,    0,    0,   56,   56,
    0,   60,    0,    0,   60,    0,    0,    0,   49,   49,
    0,   49,   49,   55,   55,   58,    0,    0,   60,   60,
    0,    0,   59,   60,    0,    0,    0,    0,    0,    0,
   49,    0,   55,    1,    1,    0,    1,    0,    1,    1,
    1,    1,    1,    1,    0,    0,    0,   58,    0,    0,
    0,    0,    0,   60,   59,    1,    1,    1,    1,    1,
    0,    0,    0,    0,    0,    0,   50,   50,    0,   50,
   50,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   53,   53,    0,   60,    0,    1,   50,    0,
    1,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   53,    0,    0,    0,    0,   51,   51,   52,   52,
    0,    0,  100,    0,    0,    0,  102,   95,    0,   93,
    1,   96,   97,  117,   98,   51,  101,   52,    0,   54,
   54,    0,    0,    0,    0,    0,   61,    0,    0,  103,
  107,  104,    0,    0,    0,   56,   56,    0,   54,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,   56,    0,   24,   25,   26,   27,
   94,    0,    0,  105,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   58,   58,    0,    0,  134,   53,    0,
    0,   59,    0,  106,    0,   99,    0,    0,    0,    0,
    0,    0,   58,    0,    0,    0,    0,    0,    0,   59,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   60,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   60,    0,    0,  247,   53,    0,    0,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    0,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    0,    0,    0,    0,   61,    0,    0,    0,    0,
    0,    0,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,    0,    0,   24,   25,   26,   27,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   49,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   78,   79,   80,   81,   82,   83,   84,   85,    0,
   86,   87,   88,   89,   90,   91,   92,   61,  130,  131,
  132,  133,    0,   50,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,   49,    0,   24,   25,   26,
   27,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   61,  130,  131,  132,  133,    0,   50,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,   61,    0,   24,   25,   26,   27,    0,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   61,
    0,   24,   25,   26,   27,  381,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,    0,    0,   24,
   25,   26,   27,
  };
  protected static readonly short [] yyCheck = {           118,
    0,   52,  155,   70,   44,  218,   58,    0,  127,  128,
   40,   52,   42,  177,   40,   41,   44,   40,   44,   59,
   63,  149,   44,   40,  341,  125,  185,  186,  125,   29,
  123,   59,   32,   33,   34,   35,   29,   59,   67,  198,
   42,  126,   60,  262,   62,  125,   66,  136,  221,  125,
  125,  157,  284,  182,  183,   44,   72,  431,   44,   57,
  127,  112,  128,   44,   57,   91,   66,   44,  125,  257,
   70,  112,   40,   66,  233,   44,  270,   44,   58,   44,
  257,  298,   59,   37,   40,   41,   42,  128,   42,   44,
  307,  308,   59,   47,   59,   44,  126,  285,  149,   35,
  126,  257,  221,   58,   40,   41,  155,  270,   44,   43,
  274,   45,   44,  275,   44,  277,  235,   27,  138,  123,
  169,  170,  171,   59,   93,   61,  124,  127,   38,  503,
  146,  124,  125,  133,  123,   91,  185,  186,  138,  125,
  133,  260,  183,  123,  125,  138,  275,   40,  277,  198,
  148,  280,  281,  282,  243,   91,   92,  257,  126,  391,
  392,  320,  321,  322,  257,  231,  294,  273,  285,   40,
   41,   40,  345,   44,  303,  492,  125,   46,  297,  246,
   61,  400,  257,  302,  233,  126,   41,  123,  341,   44,
  231,   31,  285,  125,  235,  125,   41,  356,   91,  318,
  285,  264,  265,   58,   59,   40,  291,   33,  259,  338,
  262,   51,   38,   44,   40,  374,   42,   43,  126,   45,
   91,  237,   91,  320,  321,  254,  345,  257,   59,  272,
  349,  257,  351,   59,  275,  354,  277,  396,   93,   41,
  320,  321,   44,  294,  320,  321,  246,  270,  266,  267,
  290,  292,  293,  270,   93,  126,   91,  376,  431,  259,
  379,  291,  290,  320,  321,  291,  266,  265,  290,   41,
  125,  320,  321,  322,  323,  324,  325,  326,  327,  328,
  329,  330,  331,  332,  333,  334,  335,  336,  337,  257,
  463,  284,  341,  457,  453,  459,  298,  123,  471,  125,
  126,  514,  515,   58,   40,  307,  308,  356,   33,  149,
  351,  378,  431,   38,  467,   40,  257,   42,   43,  483,
   45,  485,  441,  291,   58,  374,   41,  491,   58,   44,
  503,  460,  461,  462,   59,  499,   40,  501,   42,  492,
  250,  251,  252,   59,  463,  285,  183,  396,  399,  285,
  291,   40,  471,  123,  290,   91,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  285,  378,  315,
  316,  317,  318,  291,  503,  123,  257,   91,  506,   41,
  126,   44,  261,  262,  263,  513,  332,   61,  123,  399,
  125,  126,   91,  243,  453,   41,   40,   40,   44,   42,
   58,   40,   41,   42,   41,   44,   33,   44,  467,  259,
  291,   38,   40,   40,   40,   42,   43,   40,   45,  257,
   59,  257,  258,  259,  260,  290,  262,  263,  275,   59,
  277,   38,   59,  492,  327,  328,  329,  330,   41,   41,
   41,   44,   44,   44,   41,  292,  293,   44,   91,  285,
  268,  269,   91,   40,   41,   42,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  313,  314,  315,
  316,  317,  318,  126,  320,  321,  322,  126,  324,  325,
  326,  327,  328,  329,  330,  331,  123,   41,  125,  126,
   44,  257,   41,   46,   91,   44,   59,   33,   94,  257,
   41,  257,   38,   44,   40,  124,   42,   43,   61,   45,
  323,  324,  257,  258,  259,  260,  271,  262,  263,  285,
  286,  287,  288,   59,   41,   41,   41,  285,   44,   44,
   41,  325,  326,   44,   93,  291,  331,  332,   91,  257,
  285,  292,  293,   61,  325,   59,   41,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,  316,  317,  318,  257,  257,  257,  322,   61,  324,
  325,  326,  327,  328,  329,  330,  331,  123,   93,   59,
  126,   41,   41,   33,   41,   93,   41,   41,   38,   40,
   40,   93,   42,   43,  257,   45,   93,   93,  257,   41,
  123,   59,   93,  123,   41,  323,   40,   59,   42,   59,
  257,  258,  259,  260,   33,  262,  263,   58,   58,   38,
   59,   40,  302,   42,   43,   59,   45,   46,  291,   41,
   41,  333,  291,  463,  334,  336,  335,  114,  285,  337,
  266,  368,   77,  346,  138,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  313,  314,  315,  316,
  317,  318,   91,  123,  495,  322,  126,  324,  325,  326,
  327,  328,  329,  330,  331,   29,   -1,   -1,   -1,   -1,
   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  123,   -1,  125,  126,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   35,   -1,
   -1,   -1,   -1,   33,   41,   -1,   -1,   44,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,  285,
   42,   43,   59,   45,   61,   -1,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  313,  314,  315,
  316,  317,  318,   -1,   -1,   92,  322,   -1,  324,  325,
  326,  327,  328,  329,  330,  331,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   35,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,  123,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,  126,   -1,   59,   -1,
   61,   -1,   -1,  257,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   35,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,   92,   -1,   -1,   -1,   -1,   -1,  291,   -1,   -1,
   -1,   61,   -1,  313,  314,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  322,   -1,  324,  325,  326,  327,  328,  329,
  330,  331,  123,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   92,   -1,  313,  314,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   41,   42,   43,   33,   45,   -1,   -1,   -1,
   38,   -1,   40,  123,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,
   -1,   -1,   -1,  290,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  298,  126,  315,  316,
  317,  318,   -1,  313,  314,  307,  308,   -1,  126,   -1,
   -1,  313,  314,   -1,   -1,  332,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  285,   -1,   59,   -1,   -1,  290,
   -1,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   -1,   -1,  315,  316,  317,  318,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,  332,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  126,   33,  315,  316,  317,  318,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,  257,  258,
  259,  260,  332,  262,  263,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   33,  262,  263,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,  285,   -1,   -1,   -1,
   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  313,  314,  315,  316,  317,  318,
   40,   41,   42,   -1,   44,  313,  314,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,  126,   -1,   59,
   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   91,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   46,   -1,
   -1,   -1,  285,   93,   -1,   -1,  126,   -1,   -1,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  313,  314,  315,  316,  317,  318,  126,   -1,   -1,   -1,
   -1,  123,   -1,   91,  126,   -1,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   46,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,  123,   -1,  125,  126,   -1,
   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  285,  257,  258,  259,
  260,   -1,  262,  263,   91,   -1,   -1,   -1,   -1,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  313,  314,  315,  316,  317,  318,
   -1,   -1,   -1,   -1,  294,   -1,  123,  257,  298,  126,
   -1,   93,   -1,   -1,   -1,   -1,   -1,  307,  308,   -1,
   33,   -1,   -1,  313,  314,   38,   -1,   40,   41,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,  291,  262,  263,  126,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  294,   -1,   -1,   -1,  298,  257,
  258,  259,  260,   -1,  262,  263,   -1,  307,  308,   -1,
   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   33,   -1,
   -1,  313,  314,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   33,  126,   93,   -1,   -1,   38,   -1,   40,
   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,  313,  314,   -1,   -1,   -1,
  257,  258,  259,  260,   33,  262,  263,  126,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   93,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   93,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   33,   -1,   59,   -1,   -1,   38,   -1,
   40,  126,   42,   43,   -1,   45,  313,  314,   -1,   -1,
   -1,   -1,   33,   -1,   93,  126,   -1,   38,   -1,   40,
  123,   42,   43,  126,   45,   -1,   -1,   33,   -1,   -1,
   -1,   -1,   38,   -1,   40,   41,   42,   43,   -1,   45,
   -1,  313,  314,   -1,   -1,   -1,   -1,  126,   -1,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   -1,   93,   -1,   -1,   38,   -1,   40,   41,   42,
   43,   -1,   45,  123,   -1,   -1,  126,   -1,  257,  258,
  259,  260,   33,  262,  263,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,  126,   -1,   33,   -1,   -1,
  313,  314,   38,   -1,   40,   -1,   42,   43,   -1,   45,
  126,   -1,   -1,   40,   41,   42,   -1,   44,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   40,
   41,   42,   59,   44,  313,  314,  257,  258,  259,  260,
   -1,  262,  263,   -1,  257,  258,  259,  260,   59,  262,
  263,   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   91,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,  126,   -1,   -1,  313,  314,
   91,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
  126,   -1,  313,  314,   -1,   -1,   -1,   -1,   -1,  126,
  313,  314,   40,   41,   42,   -1,   44,  257,  258,  259,
  260,   61,  262,  263,   -1,  126,   -1,   -1,   -1,   -1,
   -1,   59,   -1,   -1,  313,  314,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,  313,
  314,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   41,   91,   -1,   44,   -1,   -1,   -1,   40,
   41,   42,   -1,  313,  314,   -1,   -1,   -1,   -1,   58,
   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  313,  314,  257,  258,  259,  260,  126,  262,
  263,   -1,   -1,   -1,   -1,   -1,   -1,  313,  314,   -1,
   -1,   -1,   -1,   -1,   93,   -1,  257,  258,  259,  260,
   91,  262,  263,   -1,   -1,   -1,   40,   -1,   42,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
  257,   -1,   -1,   -1,   -1,   59,  125,   -1,   -1,   -1,
  313,  314,   -1,   -1,   -1,  126,  257,   -1,   -1,   -1,
   -1,   -1,   40,   41,   42,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  313,  314,  291,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  285,   -1,   -1,  313,  314,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  126,   91,  315,  316,  317,  318,   41,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,
   -1,   -1,   -1,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  282,  283,  284,   -1,   41,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,   41,  257,  315,  316,  317,
  318,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  290,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   -1,  257,  315,  316,  317,  318,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  270,   -1,   -1,   59,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,   -1,
   -1,  315,  316,  317,  318,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,   -1,   -1,  315,  316,  317,
  318,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  285,   -1,  315,  316,  317,  318,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,   -1,   -1,   -1,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  257,   -1,  315,
  316,  317,  318,   -1,   -1,   37,   38,   -1,   40,   -1,
   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   60,   61,
   62,   63,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,   -1,   -1,  315,  316,  317,  318,   91,
   37,   38,   94,   -1,   41,   42,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   61,   62,   63,   -1,   -1,   37,
   38,   -1,  124,   41,   42,   43,   44,   45,   -1,   47,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   61,   62,   63,   93,   94,   37,   38,
   -1,   -1,   41,   42,   43,   44,   45,   -1,   47,   38,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   58,
   59,   60,   -1,   62,   63,   93,   94,  124,  125,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   40,   41,   42,
   -1,   44,   -1,   -1,   93,   94,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   59,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   91,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,  126,   38,   -1,   -1,   41,  290,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,  282,  283,  284,   -1,   93,
   94,   -1,   -1,  290,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,   -1,   -1,   -1,
  124,  125,  290,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,   -1,   38,   -1,   -1,
   41,  290,   43,   44,   45,   -1,   38,   -1,   -1,   41,
   -1,  290,   44,   -1,  257,   -1,   -1,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   38,   -1,   -1,   41,   -1,  291,   44,
   -1,   -1,   93,   94,   58,   59,   60,   -1,   62,   63,
   -1,   93,   94,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   38,  124,  125,   41,   -1,   -1,   44,   93,
   94,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,   94,
   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   38,
  124,  125,   41,   -1,   -1,   44,   -1,   -1,   38,  124,
  125,   41,   -1,   -1,   44,   -1,  290,   93,   94,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   93,   94,   58,   59,   60,   -1,
   62,   63,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   93,   94,   -1,  124,  125,   58,   59,   60,   -1,
   62,   63,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,  290,
   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,  290,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   -1,   38,  124,  125,   41,   -1,  290,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,
   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,
  266,  267,  268,  269,   -1,  271,  272,   -1,   38,   -1,
   -1,   41,   -1,   41,   44,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  290,   -1,   93,   94,   58,   59,
   58,   59,   -1,   63,   -1,   63,   -1,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,  290,   -1,   93,   94,   93,   38,   -1,   -1,   41,
  290,   -1,   44,   -1,  266,  267,  268,  269,   -1,  271,
  272,   -1,   41,   -1,   -1,   44,   58,   59,   -1,   -1,
   -1,   63,   -1,   -1,  124,  125,  124,  125,  290,   58,
   59,   -1,   -1,   38,   63,   38,   41,   -1,   41,   44,
   -1,   44,   -1,   -1,  266,  267,  268,  269,   -1,  271,
  272,   93,   94,   58,   59,   58,   59,   -1,   63,   41,
   63,   -1,   44,   -1,   93,   94,   -1,   -1,  290,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   58,   59,   44,   -1,
   -1,   63,  124,  125,   -1,   -1,   -1,   -1,   93,   94,
   93,   94,   58,   59,   -1,  124,  125,   63,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,  124,  125,   41,   -1,   -1,   44,   93,   -1,   -1,
   41,  268,  269,   44,  271,  272,   -1,   -1,   -1,   -1,
   58,   59,  124,  125,   -1,   63,   -1,   58,   59,   -1,
   -1,   -1,   63,  290,   -1,   -1,   -1,   -1,  124,  125,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,  268,  269,
   -1,  271,  272,  271,  272,   93,   -1,   -1,   58,   59,
   -1,   -1,   93,   63,   -1,   -1,   -1,   -1,   -1,   -1,
  290,   -1,  290,   37,   38,   -1,   40,   -1,   42,   43,
   44,   45,   46,   47,   -1,   -1,   -1,  125,   -1,   -1,
   -1,   -1,   -1,   93,  125,   59,   60,   61,   62,   63,
   -1,   -1,   -1,   -1,   -1,   -1,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,   -1,  125,   -1,   91,  290,   -1,
   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  290,   -1,   -1,   -1,   -1,  271,  272,  271,  272,
   -1,   -1,   33,   -1,   -1,   -1,   37,   38,   -1,   40,
  124,   42,   43,  123,   45,  290,   47,  290,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   60,
   61,   62,   -1,   -1,   -1,  271,  272,   -1,  290,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  290,   -1,  315,  316,  317,  318,
   91,   -1,   -1,   94,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  271,  272,   -1,   -1,  125,  126,   -1,
   -1,  272,   -1,  124,   -1,  126,   -1,   -1,   -1,   -1,
   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,  290,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  290,   -1,   -1,  125,  126,   -1,   -1,  261,  262,  263,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,   -1,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,   -1,   -1,  315,  316,  317,  318,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  262,  263,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  273,  274,  275,  276,  277,  285,  286,  287,
  288,  289,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  257,   -1,  315,  316,  317,
  318,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  285,  286,  287,  288,  289,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  285,   -1,  315,  316,  317,  318,   -1,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  285,
   -1,  315,  316,  317,  318,  319,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,   -1,   -1,  315,
  316,  317,  318,
  };

#line 1228 "CParser.jay"

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
  public const int OPERATOR = 291;
  public const int TYPEDEF = 292;
  public const int EXTERN = 293;
  public const int STATIC = 294;
  public const int AUTO = 295;
  public const int REGISTER = 296;
  public const int INLINE = 297;
  public const int RESTRICT = 298;
  public const int CHAR = 299;
  public const int SHORT = 300;
  public const int INT = 301;
  public const int LONG = 302;
  public const int SIGNED = 303;
  public const int UNSIGNED = 304;
  public const int FLOAT = 305;
  public const int DOUBLE = 306;
  public const int CONST = 307;
  public const int VOLATILE = 308;
  public const int VOID = 309;
  public const int BOOL = 310;
  public const int COMPLEX = 311;
  public const int IMAGINARY = 312;
  public const int TRUE = 313;
  public const int FALSE = 314;
  public const int STRUCT = 315;
  public const int CLASS = 316;
  public const int UNION = 317;
  public const int ENUM = 318;
  public const int ELLIPSIS = 319;
  public const int CASE = 320;
  public const int DEFAULT = 321;
  public const int IF = 322;
  public const int ELSE = 323;
  public const int SWITCH = 324;
  public const int WHILE = 325;
  public const int DO = 326;
  public const int FOR = 327;
  public const int GOTO = 328;
  public const int CONTINUE = 329;
  public const int BREAK = 330;
  public const int RETURN = 331;
  public const int EOL = 332;
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
