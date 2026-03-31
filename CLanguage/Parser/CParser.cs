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
//t    "declarator : '&' direct_declarator",
//t    "declarator : '&' type_qualifier_list direct_declarator",
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
//t    "abstract_declarator : '&'",
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
#line 574 "CParser.jay"
  {
		yyVal = new ReferenceDeclarator((Declarator)yyVals[0+yyTop]);
	}
  break;
case 146:
#line 578 "CParser.jay"
  {
		yyVal = new ReferenceDeclarator((Declarator)yyVals[0+yyTop], (TypeQualifiers)yyVals[-1+yyTop]);
	}
  break;
case 147:
#line 579 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 148:
#line 583 "CParser.jay"
  { yyVal = "+"; }
  break;
case 149:
#line 584 "CParser.jay"
  { yyVal = "-"; }
  break;
case 150:
#line 585 "CParser.jay"
  { yyVal = "*"; }
  break;
case 151:
#line 586 "CParser.jay"
  { yyVal = "/"; }
  break;
case 152:
#line 587 "CParser.jay"
  { yyVal = "%"; }
  break;
case 153:
#line 588 "CParser.jay"
  { yyVal = "=="; }
  break;
case 154:
#line 589 "CParser.jay"
  { yyVal = "!="; }
  break;
case 155:
#line 590 "CParser.jay"
  { yyVal = "<"; }
  break;
case 156:
#line 591 "CParser.jay"
  { yyVal = ">"; }
  break;
case 157:
#line 592 "CParser.jay"
  { yyVal = "<="; }
  break;
case 158:
#line 593 "CParser.jay"
  { yyVal = ">="; }
  break;
case 159:
#line 594 "CParser.jay"
  { yyVal = "<<"; }
  break;
case 160:
#line 595 "CParser.jay"
  { yyVal = ">>"; }
  break;
case 161:
#line 596 "CParser.jay"
  { yyVal = "&"; }
  break;
case 162:
#line 597 "CParser.jay"
  { yyVal = "|"; }
  break;
case 163:
#line 598 "CParser.jay"
  { yyVal = "^"; }
  break;
case 164:
#line 599 "CParser.jay"
  { yyVal = "!"; }
  break;
case 165:
#line 600 "CParser.jay"
  { yyVal = "~"; }
  break;
case 166:
#line 601 "CParser.jay"
  { yyVal = "&&"; }
  break;
case 167:
#line 602 "CParser.jay"
  { yyVal = "||"; }
  break;
case 168:
#line 603 "CParser.jay"
  { yyVal = "++"; }
  break;
case 169:
#line 604 "CParser.jay"
  { yyVal = "--"; }
  break;
case 170:
#line 605 "CParser.jay"
  { yyVal = "="; }
  break;
case 171:
#line 606 "CParser.jay"
  { yyVal = "+="; }
  break;
case 172:
#line 607 "CParser.jay"
  { yyVal = "-="; }
  break;
case 173:
#line 608 "CParser.jay"
  { yyVal = "*="; }
  break;
case 174:
#line 609 "CParser.jay"
  { yyVal = "/="; }
  break;
case 175:
#line 610 "CParser.jay"
  { yyVal = "%="; }
  break;
case 176:
#line 611 "CParser.jay"
  { yyVal = "()"; }
  break;
case 177:
#line 612 "CParser.jay"
  { yyVal = "[]"; }
  break;
case 178:
#line 619 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 179:
#line 623 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("~" + (yyVals[-1+yyTop]).ToString());
	}
  break;
case 180:
#line 627 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("operator" + (string)yyVals[0+yyTop]);
	}
  break;
case 181:
#line 629 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 182:
#line 631 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[-1+yyTop]).ToString()); }
  break;
case 183:
#line 633 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("operator" + (string)yyVals[0+yyTop]); }
  break;
case 185:
  case_185();
  break;
case 186:
#line 653 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 187:
#line 657 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 188:
#line 661 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 189:
#line 665 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 190:
#line 669 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 191:
#line 673 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 192:
#line 677 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 193:
#line 681 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 194:
#line 685 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 195:
  case_195();
  break;
case 196:
#line 697 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 197:
#line 701 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 198:
#line 702 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 199:
#line 703 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 200:
#line 704 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 201:
#line 708 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 202:
#line 712 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 203:
#line 716 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 204:
#line 717 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 205:
#line 718 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 206:
#line 725 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 207:
  case_207();
  break;
case 208:
  case_208();
  break;
case 209:
  case_209();
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
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 212:
#line 761 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 213:
#line 765 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 214:
#line 772 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 215:
#line 776 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 216:
#line 783 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 217:
#line 787 "CParser.jay"
  {
		yyVal = new ReferenceDeclarator(null);
	}
  break;
case 219:
#line 792 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 220:
  case_220();
  break;
case 221:
#line 811 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 222:
#line 815 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 223:
#line 819 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 224:
#line 823 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 225:
#line 827 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 226:
#line 831 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 227:
#line 835 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 228:
#line 839 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 229:
#line 843 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 230:
#line 847 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 231:
#line 854 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 232:
#line 858 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 233:
#line 862 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 234:
  case_234();
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
#line 902 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 250:
#line 932 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 251:
#line 936 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 252:
#line 940 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 253:
#line 941 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 256:
#line 953 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 257:
#line 957 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 258:
#line 961 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 259:
#line 962 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 263:
#line 972 "CParser.jay"
  {
		yyVal = new VirtualDeclarationStatement((Statement)yyVals[0+yyTop]) { IsVirtual = true };
	}
  break;
case 264:
  case_264();
  break;
case 265:
  case_265();
  break;
case 266:
#line 986 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 267:
  case_267();
  break;
case 268:
#line 1000 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 269:
#line 1001 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 270:
#line 1002 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 271:
#line 1009 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 272:
#line 1013 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 273:
  case_273();
  break;
case 274:
#line 1031 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 275:
#line 1035 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 276:
#line 1042 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 277:
#line 1046 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 278:
#line 1050 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-4+yyTop], (List<SwitchCase>)yyVals[-1+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 279:
#line 1054 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-3+yyTop], new List<SwitchCase>(), GetLocation(yyVals[-5+yyTop]));
	}
  break;
case 280:
#line 1061 "CParser.jay"
  {
		yyVal = new List<SwitchCase> { (SwitchCase)yyVals[0+yyTop] };
	}
  break;
case 281:
  case_281();
  break;
case 282:
#line 1073 "CParser.jay"
  {
		yyVal = new SwitchCase((Expression)yyVals[-2+yyTop], (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 283:
#line 1077 "CParser.jay"
  {
		yyVal = new SwitchCase(null, (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 284:
#line 1084 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 285:
#line 1088 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 286:
#line 1092 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 287:
#line 1096 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 288:
#line 1100 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 289:
#line 1104 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 291:
#line 1112 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 292:
#line 1116 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 293:
#line 1120 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 294:
#line 1124 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 295:
  case_295();
  break;
case 296:
  case_296();
  break;
case 301:
  case_301();
  break;
case 302:
  case_302();
  break;
case 303:
#line 1172 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 304:
#line 1176 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-2+yyTop]).ToString()).Push((yyVals[0+yyTop]).ToString());
	}
  break;
case 305:
#line 1180 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("~" + (yyVals[0+yyTop]).ToString());
	}
  break;
case 306:
#line 1181 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 307:
#line 1182 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
  break;
case 308:
#line 1184 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("operator" + (string)yyVals[0+yyTop]); }
  break;
case 309:
#line 1186 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("operator" + (string)yyVals[0+yyTop]); }
  break;
case 310:
  case_310();
  break;
case 311:
  case_311();
  break;
case 312:
  case_312();
  break;
case 313:
  case_313();
  break;
case 314:
  case_314();
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

void case_185()
#line 639 "CParser.jay"
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

void case_195()
#line 687 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<Expression>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(ctorArgumentValue: n));
		}
		yyVal = d;
	}

void case_207()
#line 727 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_208()
#line 736 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_209()
#line 742 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_220()
#line 797 "CParser.jay"
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

void case_234()
#line 867 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_235()
#line 874 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_236()
#line 882 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_237()
#line 889 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_264()
#line 974 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsOverride = true };
	}

void case_265()
#line 979 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsOverride = true };
	}

void case_267()
#line 991 "CParser.jay"
{
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator((Declarator)yyVals[-3+yyTop], null) };
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-4+yyTop], decls);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsPureVirtual = true };
	}

void case_273()
#line 1018 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		DeclarationSpecifiers ds = new DeclarationSpecifiers();
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator(fdecl, null) };
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_281()
#line 1063 "CParser.jay"
{
		((List<SwitchCase>)yyVals[-1+yyTop]).Add((SwitchCase)yyVals[0+yyTop]);
		yyVal = yyVals[-1+yyTop];
	}

void case_295()
#line 1129 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_296()
#line 1134 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_301()
#line 1149 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_302()
#line 1158 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_310()
#line 1191 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_311()
#line 1200 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_312()
#line 1212 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_313()
#line 1218 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_314()
#line 1224 "CParser.jay"
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
   43,   43,   30,   32,   32,   32,   32,   47,   47,   47,
   47,   47,   47,   47,   47,   47,   47,   47,   47,   47,
   47,   47,   47,   47,   47,   47,   47,   47,   47,   47,
   47,   47,   47,   47,   47,   47,   47,   48,   48,   48,
   48,   48,   48,   45,   45,   45,   45,   45,   45,   45,
   45,   45,   45,   45,   45,   45,   44,   44,   44,   44,
   46,   46,   29,   29,   29,   49,   49,   50,   50,   51,
   51,   51,   51,    5,    5,   52,   52,   52,   52,   53,
   53,   53,   53,   53,   53,   53,   53,   53,   53,   53,
   33,   33,   33,    6,    6,    6,    6,   54,   55,   55,
   56,   56,   57,   57,   57,   57,   57,   57,   58,   59,
   59,   64,   64,   65,   65,   38,   38,   66,   66,   67,
   67,   67,   67,   67,   67,   67,   70,   68,   68,   68,
   71,   71,   69,   60,   60,   61,   61,   61,   61,   72,
   72,   73,   73,   62,   62,   62,   62,   62,   62,   63,
   63,   63,   63,   63,    0,    0,   74,   74,   74,   74,
   75,   75,   78,   78,   78,   78,   78,   78,   78,   76,
   76,   77,   77,   77,   79,   79,   79,
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
    1,    3,    1,    2,    2,    3,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    2,    2,    1,    2,    2,
    3,    4,    4,    1,    3,    5,    4,    4,    6,    6,
    5,    4,    3,    4,    4,    3,    1,    2,    2,    3,
    1,    2,    1,    1,    1,    1,    3,    1,    3,    2,
    4,    2,    1,    1,    2,    1,    1,    1,    2,    3,
    2,    3,    3,    4,    3,    4,    2,    3,    3,    4,
    1,    3,    4,    1,    2,    3,    4,    2,    1,    2,
    3,    2,    1,    1,    1,    1,    1,    1,    3,    2,
    3,    1,    2,    1,    1,    2,    3,    1,    2,    1,
    1,    1,    2,    4,    5,    2,    5,    2,    2,    2,
    4,    3,    2,    1,    2,    5,    7,    7,    6,    1,
    2,    4,    3,    5,    7,    6,    7,    6,    7,    3,
    2,    2,    2,    3,    1,    2,    1,    1,    1,    1,
    4,    3,    1,    3,    4,    3,    4,    4,    4,    4,
    5,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,    0,   95,   96,   97,   98,   99,  143,  204,  101,
  102,  103,  104,  107,  108,  105,  106,  203,  205,  100,
  109,  110,  111,  127,  128,  129,    0,  299,    0,  298,
    0,    0,    0,    0,    0,  112,  113,    0,  295,  297,
  300,    0,    0,  115,  116,    0,    0,  296,  178,    0,
    0,    0,    0,    0,   81,    0,   91,    0,    0,    0,
    0,  114,   84,   86,   88,   90,    0,    0,  119,    0,
    0,  304,    0,    0,    0,    0,  139,    0,  168,  169,
  159,  160,  157,  158,  153,  154,  166,  167,  173,  174,
  175,  171,  172,    0,    0,  161,  150,  148,  149,  165,
  164,  151,  152,  155,  156,  163,  162,  170,  180,    0,
  201,    0,    0,  199,    0,  179,    0,   82,  315,    0,
    0,  316,  317,  312,    0,  302,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  256,  260,    0,    0,
    0,  258,  261,  262,    0,    0,  117,  306,    0,    0,
    0,    0,    0,    0,  208,  308,  305,    0,  134,    0,
    0,  176,  177,  185,  202,    0,  200,   92,    0,    0,
    2,    3,    0,    0,    0,    4,    5,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  250,    0,    0,
   27,   28,   29,   30,  274,    7,    0,    0,   78,    0,
   33,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   63,  254,  255,  243,  244,  245,  246,  247,
  248,    0,  252,    1,    0,  231,   94,  314,  301,  313,
  196,    0,   17,    0,    0,  193,    0,    0,    0,  181,
    0,    0,  268,  269,  270,  263,    0,  266,    0,    0,
  257,  259,  273,    0,    0,    0,  123,    0,  121,  309,
  307,  310,    0,    0,    0,    0,    0,  212,    0,    0,
    0,   31,   80,  142,  136,  140,  135,    0,    0,    0,
   25,    0,   20,   21,    0,    0,    0,    0,    0,    0,
  291,  292,  293,    0,    0,    0,    0,    0,    0,   22,
   23,    0,  275,    0,   13,   14,    0,    0,    0,   66,
   67,   68,   69,   70,   71,   72,   73,   74,   75,   76,
   77,   65,    0,   24,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  251,  253,    0,    0,    0,  234,    0,
    0,  239,  195,    0,  194,    0,  192,  188,    0,  187,
    0,    0,  183,  182,    0,    0,    0,  272,    0,  124,
  125,  126,    0,  118,  227,    0,    0,  221,    0,    0,
    0,    0,    0,    0,  311,  207,  209,  137,  249,    0,
    0,    0,    0,    0,    0,    0,    0,  290,  294,    6,
    0,  130,  132,    0,  217,    0,  215,   79,   12,    9,
    0,    0,   11,   64,   34,   35,   36,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  242,  232,    0,  235,  238,  240,
   18,    0,    0,  191,  186,    0,    0,  264,  271,  122,
  228,  220,  225,  222,  211,  229,    0,  223,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   32,
   10,    8,    0,  241,  233,  236,    0,  189,  190,  265,
    0,  230,  226,  224,    0,    0,  284,    0,    0,    0,
    0,    0,    0,   62,  237,  267,    0,    0,    0,  279,
    0,  280,    0,  288,    0,  286,    0,   15,    0,  277,
    0,    0,  278,  281,  285,  289,  287,   16,    0,    0,
    0,
  };
  protected static readonly short [] yyDgoto  = {            29,
  196,  197,  198,  232,  296,  348,  199,  200,  201,  202,
  203,  204,  205,  206,  207,  208,  209,  210,  211,  212,
  213,  323,  274,  214,  125,   56,   32,   33,   34,   35,
   57,  169,  227,   36,   37,  257,   38,   69,  258,  259,
  299,   76,   77,   59,   60,  113,  109,   61,  376,  154,
  155,  377,  269,  350,  351,  352,  215,  216,  217,  218,
  219,  220,  221,  222,  223,  141,  142,  143,  144,  248,
  145,  501,  502,   39,   40,   41,  127,   42,  128,
  };
  protected static readonly short [] yySindex = {         2491,
 -177,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  -79,    0, 2491,    0,
  282, 3820, 3820, 3820, 3820,    0,    0,   40,    0,    0,
    0,  -28,  -97,    0,    0, -157,    9,    0,    0, 3715,
  411,   88,  -37, -151,    0,   14,    0, 1056,   90,   12,
 -125,    0,    0,    0,    0,    0, 3708,   -2,    0,  -81,
  253,    0, 3715, -124,  115,   -5,    0, -157,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  151,  110,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  170,
    0,   12,   88,    0,  -37,    0,  411,    0,    0,  317,
 1024,    0,    0,    0,  282,    0, 3638, 3820,   12, 1135,
 1373,  -98,  188,  278,  296, 3820,    0,    0,  282,  -25,
 3764,    0,    0,    0,  194, -190,    0,    0, 3715,   82,
  238,  850,  357,  328,    0,    0,    0, 1957,    0,  -72,
   -3,    0,    0,    0,    0,   12,    0,    0,  341,  368,
    0,    0, 1979, 2015, 2015,    0,    0,  390,  401,  406,
  624,  407,  197,  393,  397,  111, 1344,    0, 1957, 1957,
    0,    0,    0,    0,    0,    0,   46,  494,    0,  137,
    0, 1957,  338,  124, -176,  -42, -198,  427,  372,  370,
  227,  -41,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  474,    0,    0, 1577,    0,    0,    0,    0,    0,
    0,   58,    0,  468,   96,    0, 1671,  418, 1431,    0,
 3715,  258,    0,    0,    0,    0,  282,    0,  -38, 2385,
    0,    0,    0, -194, -194, -194,    0,  -13,    0,    0,
    0,    0, 2218, 1716,   88,  475,   42,    0,   47,  238,
 3792,    0,    0,    0,    0,    0,    0,  -68,  624, 1344,
    0, 1344,    0,    0, 1957, 1957, 1957,  205, 1281,  482,
    0,    0,    0,  121,  195,  501, 3480, 3480,  302,    0,
    0, 1957,    0,  286,    0,    0, 1679, 1957,  287,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 1957,    0, 1957, 1957, 1957, 1957, 1957, 1957,
 1957, 1957, 1957, 1957, 1957, 1957, 1957, 1957, 1957, 1957,
 1957, 1957, 1957,    0,    0, 1957,  309,   16,    0, 1024,
  331,    0,    0, 1957,    0, 1474,    0,    0, 1957,    0,
 1751,  488,    0,    0,  -36,  506,  513,    0,  537,    0,
    0,    0, -190,    0,    0,  542,  543,    0, 1774,  493,
 1957,   47, 2435, 1782,    0,    0,    0,    0,    0,  547,
  552,  203,  308,  346,  550, 1814, 1814,    0,    0,    0,
 1841,    0,    0, 2334,    0,  183,    0,    0,    0,    0,
  387,   18,    0,    0,    0,    0,    0,  338,  338,  124,
  124, -176, -176, -176, -176,  -42,  -42, -198,  427,  372,
  370,  227,   21,  502,    0,    0,  706,    0,    0,    0,
    0,  503,  505,    0,    0,  544, 1854,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  560,    0, 1872,  511,
  484,  484,  624,  514,  624, 1957, 1880, 1912, 1577,    0,
    0,    0, 1957,    0,    0,    0, 1024,    0,    0,    0,
  546,    0,    0,    0,  283,  -99,    0,  395,  624,  469,
  624,  477,   25,    0,    0,    0,  624, 1957,  578,    0,
  -64,    0,  579,    0,  624,    0,  624,    0, 1439,    0,
  582,  549,    0,    0,    0,    0,    0,    0,  549,  549,
  549,
  };
  protected static readonly short [] yyRindex = {            0,
    0, 2294,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   75, 1419, 2139, 2845,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2106,    0,    0,    0,
    0,    0,  637,    0,    0,    0,    0,  206,    0,  778,
  615,    0,    0,    0,    0,    0,    0, 2162,    0,    0,
    0,    0,    0,    0,   37,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  899,    0,    0,  930,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  942,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  479,    0,  608,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 1000,    0,    0,  -17, 3635,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 2774,    0, 2832,
    0,    0, 2967, 3104, 3203, 3420, 3448, 3533, 2911, 2026,
 2259, 1093,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  481,  483,  485,    0,  487,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  359,  417,  610,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -17,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  491,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  611,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 3067, 3094, 3152,
 3162, 3230, 3257, 3342, 3352, 3446, 3474, 3522, 3542, 3263,
 2267, 3581,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2803,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2719,    0,    0,    0,  175,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  -11,
   10,
  };
  protected static readonly short [] yyGindex = {            0,
    0, -103,    0,  347,  109,  184, -121,  -50, -152,    0,
  -24,  -18,  129,   70,  320,  321,  323,  324,  319,    0,
 -155,    0, -299,   19,    1,  -90,    0,  147,  -39,    0,
  553,   76, -131,    0,    0,   13,    0,  -61,    0,  298,
   85,  595, -114,  -51,  -35,   27,  117,  -12,  -55,    0,
  408, -127, -235, -383,    0,  329, -170,    0,  -15,  -82,
    0,    0,    0, -453, -218,    0,  541,    0,    0,    0,
    0,    0,  185,  655,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           226,
   31,  114,  273,  345,   53,  117,  147,  117,  233,  238,
  288,   71,  111,  111,  250,  153,  112,  334,   30,  335,
  118,  343,  118,  129,  268,  500,   93,  242,   74,   31,
  373,  382,   63,   64,   65,   66,  300,  301,  160,   47,
  278,   93,  126,   46,  150,  276,  434,   30,  249,  324,
   68,  130,  275,  477,  140,  146,  388,  117,  520,  437,
  513,  302,   44,  167,  302,  521,   44,  139,  509,  336,
  337,  152,  118,  165,  234,  165,  124,  166,  473,  115,
  141,  263,  294,  295,  301,  138,  383,  330,  331,  302,
   45,  111,   43,  349,   45,  254,  255,  256,  353,   75,
  267,  354,  131,  226,  303,  116,   58,  272,  389,   67,
  472,  229,   83,  283,   83,   83,   83,  362,   83,  159,
   67,  277,  281,  283,  284,  477,  110,   51,  140,   51,
  152,   78,  264,   83,  282,  262,  247,  384,  272,  272,
  436,  139,  380,  194,  132,  228,  230,  298,  189,  508,
  187,  272,  190,  191,  246,  192,  365,  239,  240,  138,
  157,  141,   67,  276,  302,   83,  328,   54,  329,  293,
  382,  407,  415,  416,  417,  158,  295,   44,  295,  399,
  408,  392,  393,  394,   75,  233,  272,   72,   75,  156,
  273,  162,  241,   73,  369,  111,  374,  322,  511,  165,
   83,  414,  163,  148,  412,   45,  397,  276,  301,  149,
  164,  267,  276,   54,  276,   54,  276,  276,  438,  276,
  498,  499,  404,  332,  333,  111,  301,  266,  226,  112,
  342,  129,  441,  276,  442,  400,  193,  443,  302,  433,
  298,   70,  298,  463,  132,  243,  302,  406,  470,   93,
  152,  367,  253,  446,  385,  498,  499,  298,  298,  455,
    9,  356,  460,  152,   93,  260,  370,  371,  372,   18,
   19,  152,   93,  264,  272,  272,  272,  272,  272,  272,
  272,  272,  272,  272,  272,  272,  272,  272,  272,  272,
  272,  272,  485,  151,  487,  272,   44,  276,   49,  276,
  276,  345,  345,  418,  419,  476,  301,  396,  283,  283,
  272,  420,  421,  467,  468,  226,  165,  494,  504,   52,
  506,   51,  366,   53,   45,  226,  510,  457,  272,  282,
  282,   83,   50,  297,  516,  244,  517,  349,  110,  405,
   55,  404,  273,   53,   49,  495,   49,  226,  464,  194,
  272,  302,  406,  245,  189,  226,  187,  363,  190,  191,
  120,  192,  488,  490,  492,   83,  261,  224,  171,  172,
  173,  271,  174,  175,  327,  195,  347,  476,   50,  325,
   50,  402,  403,  152,  326,    9,  465,  226,  390,  302,
  391,  439,  264,    9,   18,   19,  131,  270,  131,  131,
  131,  121,   18,   19,  152,  426,  427,   54,  272,  310,
  311,  312,  313,  314,  315,  316,  317,  318,  319,  320,
  321,  346,  272,  176,  177,  279,  297,  471,  297,  285,
  354,  276,  276,  276,  276,  503,  276,  276,  302,  120,
  286,  188,  193,  297,  297,  287,  289,  272,   52,  131,
   51,  291,   53,  290,  133,  292,  133,  133,  133,  276,
  422,  423,  424,  425,  338,  339,  276,  276,  276,  276,
  276,  276,  276,  276,  276,  276,  276,  276,  276,  276,
  276,  276,  276,  276,  276,  276,  276,  276,  276,  276,
  276,  276,  276,  340,  276,  276,  276,  341,  276,  276,
  276,  276,  276,  276,  276,  276,  194,  133,  355,  505,
  358,  189,  302,  187,  364,  190,  191,  507,  192,  213,
  302,  217,  213,  210,  217,  216,  210,  218,  216,  395,
  218,  219,  195,  307,  219,  381,   54,   62,   49,  309,
  398,  401,  409,  413,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,  435,  447,   24,   25,   26,
   27,  448,   50,  170,  171,  172,  173,  449,  174,  175,
  445,  194,  451,  452,  308,  454,  189,  461,  187,  466,
  190,  191,  462,  192,  474,  478,  120,  479,  344,  193,
  482,   62,  480,  484,  496,  497,  469,  195,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,  176,
  177,   24,   25,   26,   27,  512,  486,  515,  178,  519,
  179,  180,  181,  182,  183,  184,  185,  186,  206,  184,
  214,  216,  493,  411,  184,  184,  194,  428,  184,  429,
  432,  189,  430,  187,  431,  190,  191,   49,  192,  168,
  450,  120,  161,  184,  193,  184,  197,  197,  387,  440,
  197,  252,  195,   48,    0,  514,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   50,    0,    0,    0,  184,  184,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  197,    0,    0,
  170,  171,  172,  173,    0,  174,  175,  184,  194,    0,
    0,    0,    0,  189,    0,  187,  120,  190,  191,  193,
  192,  347,    0,    0,  304,  305,  306,    0,   62,    0,
    0,    0,  197,    0,    0,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,  176,  177,   24,   25,
   26,   27,    0,    0,    0,  178,  346,  179,  180,  181,
  182,  183,  184,  185,  186,  170,  171,  172,  173,    0,
  174,  175,  147,    0,    0,    0,    0,    0,  147,    0,
    0,  147,    0,    0,    0,    0,    0,    0,  225,    0,
  475,  193,    0,   62,    0,    0,  147,    0,  147,    0,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,  176,  177,   24,   25,   26,   27,    0,    0,  147,
  178,    0,  179,  180,  181,  182,  183,  184,  185,  186,
  170,  171,  172,  173,    0,  174,  175,  265,    0,  263,
    0,   53,    0,  197,    0,    0,    0,    0,    0,  184,
  147,    0,    0,    0,  184,    0,  184,  184,  184,  184,
  184,  184,  184,  184,  184,  184,  184,  184,  184,  184,
  184,  184,  184,  184,  184,  184,  184,  197,    0,  184,
  184,  184,  184,  145,    0,    0,  176,  177,    0,  145,
  264,    0,  145,    0,    0,  178,  184,  179,  180,  181,
  182,  183,  184,  185,  186,    0,    0,  145,    0,  145,
    0,    0,  224,  171,  172,  173,    0,  174,  175,  198,
  198,    0,    0,  198,    0,   54,  144,    0,    0,    0,
    0,    0,  144,    0,    0,  144,    0,    0,    0,    0,
  145,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  144,    0,  144,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  176,  177,
  198,  145,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  144,  146,    0,    0,    0,    0,    0,
  146,    0,    0,  146,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  198,  194,    0,  146,    0,
  146,  189,  147,  187,  144,  190,  191,  147,  192,  147,
  147,  147,  147,  147,  147,  147,  147,  147,  147,  147,
  147,  147,  147,  147,  147,  147,  147,  147,  147,  147,
  122,  146,  147,  147,  147,  147,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   49,    0,    0,  147,
    0,    0,    0,    0,    0,    0,  121,    0,    0,    0,
    0,    0,  146,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   61,    0,    0,   61,    0,    0,    0,
   50,    0,    0,    0,    0,    0,  225,  123,    0,  193,
   61,   61,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  194,    0,    0,
    0,    0,  189,    0,  187,  231,  190,  191,  120,  192,
    0,    0,    0,  145,    0,   61,  198,    0,  145,    0,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,    0,    0,  145,  145,  145,  145,   61,    0,    0,
  198,    0,    0,    0,    0,    0,  144,    0,    0,    0,
  145,  144,    0,  144,  144,  144,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,  144,  144,  144,
  144,  144,  144,  144,    0,    0,  144,  144,  144,  144,
  193,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  144,    0,    0,    0,    0,    0,    0,
  224,  171,  172,  173,  146,  174,  175,    0,    0,  146,
    0,  146,  146,  146,  146,  146,  146,  146,  146,  146,
  146,  146,  146,  146,  146,  146,  146,  146,  146,  146,
  146,  146,    0,  194,  146,  146,  146,  146,  189,    0,
  187,    0,  190,  191,    0,  192,    0,    0,    0,    0,
    0,  146,    0,    0,    0,    0,  176,  177,    0,  195,
   62,    0,    0,    0,    0,    0,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,    0,    0,
   24,   25,   26,   27,    0,    0,  194,    0,    0,    0,
    0,  189,   61,  187,    0,  190,  191,  119,  192,    0,
    0,  224,  171,  172,  173,    0,  174,  175,    0,    0,
    0,    0,    0,    0,    0,  194,  193,    0,    0,    0,
  189,    0,  187,    0,  237,  191,    0,  192,    0,   62,
    0,    0,    0,    0,    0,    0,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,  176,  177,   24,
   25,   26,   27,    0,    0,    0,   85,    0,   85,   85,
   85,    0,   85,  194,    0,  236,    0,    0,  189,  193,
  187,  194,  361,  191,    0,  192,  189,   85,  187,    0,
  190,  191,    0,  192,  347,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  193,    0,
    0,    0,    0,    0,    0,    0,  194,    0,    0,   85,
    0,  189,    0,  187,    0,  190,  191,    0,  192,    0,
    0,    0,    0,  360,    0,    0,    0,    0,    0,  346,
    0,    0,    0,    0,    0,    0,    0,  224,  171,  172,
  173,    0,  174,  175,   85,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  193,    0,    0,    0,
    0,  225,    0,  518,  193,   62,    0,    0,    0,    0,
    0,    0,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,  176,  177,   24,   25,   26,   27,  193,
  224,  171,  172,  173,    0,  174,  175,    0,    0,  194,
    0,    0,    0,    0,  189,    0,  187,    0,  190,  191,
    0,  192,  347,    0,    0,    0,    0,    0,   62,  224,
  171,  172,  173,    0,  174,  175,    0,    0,    0,    0,
    0,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,  176,  177,   24,   25,
   26,   27,    0,    0,    0,    0,  235,  346,    0,    0,
    9,    0,    0,    0,    0,   85,    0,    0,    0,   18,
   19,    0,    0,    0,    0,  176,  177,  224,  171,  172,
  173,    0,  174,  175,    0,  224,  171,  172,  173,  225,
  174,  175,  193,  194,    0,    0,    0,    0,  189,   85,
  187,  194,  190,  191,    0,  192,  189,    0,  187,  410,
  190,  191,    0,  192,  359,    0,    0,    0,    9,    0,
  224,  171,  172,  173,    0,  174,  175,   18,   19,    0,
    0,    0,    0,  176,  177,    0,    0,    0,  194,    0,
    0,  176,  177,  189,    0,  187,    0,  379,  191,    0,
  192,    0,    0,  357,    0,    0,    0,    0,    0,    0,
    0,    9,    0,    0,    0,    0,    0,    0,    0,    0,
   18,   19,    0,  194,    0,    0,  176,  177,  189,    0,
  187,    0,  190,  191,    0,  192,  193,    0,    0,    0,
    0,    0,    0,    0,  193,    0,  194,    0,  378,    0,
    0,  189,    0,  187,  194,  190,  191,    0,  192,  189,
    0,  187,    0,  459,  191,    0,  192,    0,    0,    0,
    0,    0,    0,  224,  171,  172,  173,    0,  174,  175,
    0,  193,    0,  444,    0,    0,  194,    0,    0,    0,
    0,  189,    0,  187,    0,  190,  191,    0,  192,    0,
    0,    0,    0,    0,    0,    0,  453,    0,    0,    0,
    0,    0,  195,  194,  458,    0,  193,    0,  189,    0,
  187,    0,  190,  191,    0,  192,  194,    0,    0,  176,
  177,  189,    0,  187,    0,  190,  191,    0,  192,  193,
    0,    0,    0,    0,  194,    0,    0,  193,    0,  189,
    0,  187,  194,  190,  191,    0,  192,  189,    0,  187,
  489,  190,  191,    0,  192,    0,    0,  224,  171,  172,
  173,    0,  174,  175,    0,  224,  171,  172,  173,  193,
  174,  175,    0,    0,  194,    0,    0,    0,    0,  189,
    0,  187,  491,  190,  191,    0,  192,    0,    0,    0,
    0,    0,    0,  469,  483,    0,  193,    0,    0,    0,
    0,    0,  224,  171,  172,  173,  225,  174,  175,  193,
    0,    0,    0,  176,  177,    0,    0,    0,    0,  194,
    0,  176,  177,    0,  189,    0,  187,  193,  190,  191,
    0,  192,    0,    0,    0,  193,    0,  224,  171,  172,
  173,  194,  174,  175,    0,    0,  189,    0,  280,    0,
  190,  191,    0,  192,    0,    0,    0,    0,  176,  177,
  224,  171,  172,  173,    0,  174,  175,  193,  224,  171,
  172,  173,    0,  174,  175,    0,    0,  194,    0,    0,
    0,    0,  189,    0,  282,    0,  190,  191,    0,  192,
    0,    0,    0,  176,  177,    0,   57,    0,    0,   57,
  224,  171,  172,  173,    0,  174,  175,    0,    0,    0,
    0,    0,  193,   57,   57,    0,  176,  177,   57,    0,
    0,    0,    0,    0,  176,  177,    0,  224,  171,  172,
  173,    0,  174,  175,  193,    0,    0,    0,    0,    0,
  224,  481,  172,  173,    0,  174,  175,    0,   57,    0,
    0,    0,    0,    0,    0,    0,  176,  177,  224,  171,
  172,  173,    0,  174,  175,    0,  224,  171,  172,  173,
  193,  174,  175,  138,    0,  138,  138,  138,    0,  138,
   57,    0,    0,  176,  177,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  138,    0,  176,  177,  224,  171,
  172,  173,    0,  174,  175,    0,   87,    0,   87,   87,
   87,    0,   87,    0,  176,  177,    0,    0,    0,    0,
    0,    0,  176,  177,    0,    0,  138,   87,    0,  120,
    0,  120,  120,  120,    0,  120,    0,    0,    0,    0,
    0,    0,    0,  224,  171,  172,  173,    0,  174,  175,
  120,    0,    0,    0,  176,  177,    0,    0,    0,   87,
    0,  138,    0,    0,    0,  224,  171,  172,  173,    0,
  174,  175,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  120,    0,    0,  265,    0,  263,  375,   53,
    0,    0,    0,    0,   87,    0,    0,    0,    0,  176,
  177,  224,  171,  172,  173,    0,  174,  175,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  120,    0,    0,
    0,  176,  177,    0,    0,    0,   57,   57,    0,   59,
    0,    0,   59,    0,    0,    0,    0,   58,  264,    0,
   58,    0,    0,    0,    0,   57,   59,   59,    0,    0,
    0,   59,    0,    0,   58,   58,    0,  176,  177,   58,
    0,  114,    0,  114,    0,  114,    0,    0,    0,    0,
    0,    0,    0,   54,    0,    0,    0,    0,    0,    0,
    0,   59,  114,    0,    0,    0,    0,    0,    0,   58,
    0,    0,  138,    0,    0,    0,    0,    0,    0,    0,
    0,  405,    0,  404,  375,   53,    0,    0,    0,    0,
    0,    0,    0,   59,    0,    0,    0,    0,    0,    0,
  138,   58,    0,    0,    0,   87,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  120,  114,
  138,  138,  138,  138,  264,  368,    0,    0,    0,   87,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  120,    0,    0,    0,
    0,    0,  120,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,   49,  456,  120,  120,  120,  120,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   62,    0,    0,    0,    0,    0,   50,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   59,    0,   24,   25,   26,   27,    0,   58,   58,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   59,   28,
  114,    0,    0,    0,    0,    0,   58,    0,    0,    0,
    0,    0,    0,  303,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  114,    0,
    0,    0,    0,    0,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,    0,    0,  114,  114,
  114,  114,    0,    0,    0,    0,    0,    0,   62,    0,
    0,    0,    0,    0,    0,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,    0,    0,   24,   25,
   26,   27,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   62,
    0,    0,    0,    0,    0,    0,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,    0,    0,   24,
   25,   26,   27,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   62,
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
    0,    0,   31,   31,   31,   31,   31,    0,   31,    0,
    0,    0,   89,    0,   89,   89,   89,    0,   89,   31,
   31,   31,    0,   31,   31,   26,   26,   19,   19,    0,
    0,    0,    0,   89,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   31,   31,   26,   26,    0,    0,
    0,    0,    0,    0,    0,   89,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   55,    0,    0,   55,   31,   31,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   55,   55,
   89,    0,    0,   55,    0,    0,    0,    0,    0,    2,
    2,    2,    2,    2,    2,    2,    2,    2,    0,    2,
    2,    2,    2,    2,    2,    2,    2,    2,    2,    2,
    2,    2,    2,   55,   37,    0,    0,   37,    2,   37,
   37,   37,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   37,   37,   37,    0,   37,   37,
    0,    0,    0,    0,   55,   55,    0,   19,   19,   19,
   19,   19,   19,    0,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,    0,   37,
   37,    0,    0,   19,    0,    0,   26,   26,   26,   26,
   26,   26,    0,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,    0,    0,    0,
   37,   37,   26,    0,    0,   31,   31,   31,   31,   31,
   31,   89,   31,   31,   38,    0,    0,   38,    0,   38,
   38,   38,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   31,    0,    0,   38,   38,   38,    0,   38,   38,
    0,   39,    0,    0,   39,   89,   39,   39,   39,    0,
    0,   40,    0,    0,   40,    0,    0,   40,    0,    0,
    0,   39,   39,   39,    0,   39,   39,    0,    0,   38,
   38,   40,   40,   40,    0,   40,   40,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   55,   55,    0,    0,    0,   39,   39,    0,   41,
   38,   38,   41,    0,    0,   41,   40,   40,    0,   42,
   55,    0,   42,    0,    0,   42,    0,    0,    0,   41,
   41,   41,    0,   41,   41,    0,    0,   39,   39,   42,
   42,   42,    0,   42,   42,    0,    0,   40,   40,    0,
   37,   37,   37,   37,   37,   37,    0,   37,   37,    0,
   43,    0,    0,   43,   41,   41,   43,    0,    0,    0,
    0,    0,    0,    0,   42,   42,   37,    0,    0,    0,
   43,   43,   43,    0,   43,   43,    0,   46,    0,    0,
   46,    0,    0,   46,    0,   41,   41,    0,    0,    0,
    0,    0,    0,    0,    0,   42,   42,   46,   46,   46,
    0,   46,   46,    0,   47,   43,   43,   47,    0,    0,
   47,    0,    0,   56,    0,    0,   56,    0,    0,    0,
    0,    0,    0,    0,   47,   47,   47,    0,   47,   47,
   56,   56,   46,   46,    0,   56,   43,   43,    0,    0,
   38,   38,   38,   38,   38,   38,    0,   38,   38,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   47,
   47,    0,    0,   46,   46,   56,   38,   39,   39,   39,
   39,   39,   39,    0,   39,   39,    0,   40,   40,   40,
   40,   40,   40,    0,   40,   40,    0,    0,    0,   44,
   47,   47,   44,   39,    0,   44,   56,   56,    0,   45,
    0,    0,   45,   40,    0,   45,    0,    0,    0,   44,
   44,   44,    0,   44,   44,    0,    0,    0,    0,   45,
   45,   45,    0,   45,   45,   41,   41,   41,   41,   41,
   41,    0,   41,   41,    0,   42,   42,   42,   42,   42,
   42,    0,   42,   42,   44,   44,    0,    0,    0,    0,
    0,   41,    0,    0,   45,   45,    0,    0,    0,    0,
    0,   42,    0,    0,    0,    0,    0,   48,    0,    0,
   48,    0,    0,   48,    0,   44,   44,    0,   43,   43,
   43,   43,    0,   43,   43,   45,   45,   48,   48,    0,
    0,    0,   48,   49,    0,   51,   49,    0,   51,   49,
    0,   51,   43,    0,    0,   46,   46,   46,   46,    0,
   46,   46,    0,   49,   49,   51,   51,    0,   49,    0,
   51,   50,   48,   48,   50,    0,    0,   50,    0,   46,
    0,    0,   47,   47,   47,   47,    0,   47,   47,    0,
    0,   50,   50,   56,   56,    0,   50,    0,   49,   49,
   51,   51,    0,   48,   48,    0,   47,    0,    0,    0,
    0,    0,   56,    0,    0,    0,    0,    0,    0,   52,
    0,    0,   52,    0,    0,   52,   50,   50,    0,   49,
   49,   51,   51,   53,    0,    0,   53,    0,    0,   52,
   52,    0,   54,    0,   52,   54,    0,    0,    0,    0,
   53,   53,    0,    0,    0,   53,    0,   50,   50,   54,
   54,    0,    0,    0,   54,    0,    0,   44,   44,   44,
   44,    0,   44,   44,   52,   52,    0,   45,   45,   45,
   45,   60,   45,   45,   60,   53,   53,    0,    0,    0,
    0,   44,    0,    0,   54,   54,    0,    0,   60,   60,
    0,   45,    0,   60,    0,   52,   52,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   53,   53,    0,    0,
    0,    0,    0,    0,    0,   54,   54,    0,    0,    0,
    0,    1,    1,   60,    1,    0,    1,    1,    1,    1,
    1,    1,    0,    0,    0,    0,    0,   48,   48,    0,
   48,   48,    0,    1,    1,    1,    1,    1,    0,    0,
    0,    0,    0,    0,    0,   60,    0,    0,    0,   48,
    0,    0,    0,   49,   49,    0,   49,   49,   51,   51,
    0,    0,    0,    0,    0,    1,    0,    0,    1,    0,
    0,    0,    0,    0,    0,   49,    0,   51,    0,    0,
    0,   50,   50,    0,   50,   50,    0,  101,    0,    0,
    0,  103,   96,    0,   94,    0,   97,   98,    1,   99,
  120,  102,    0,   50,   62,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  104,  108,  105,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   52,   52,   24,   25,   26,   27,    0,    0,
    0,    0,    0,   53,   53,   95,    0,    0,  106,    0,
    0,   52,   54,   54,    0,    0,    0,    0,    0,    0,
    0,    0,   53,    0,    0,    0,    0,    0,    0,    0,
    0,   54,  137,   54,    0,    0,    0,    0,  107,    0,
  100,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   60,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   60,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  251,   54,
    0,    0,    0,    0,    0,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    0,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    0,
    0,    0,   62,    0,    0,    0,    0,    0,    0,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
    0,    0,   24,   25,   26,   27,    0,    0,    0,    0,
    0,    0,    0,    0,   49,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   79,   80,   81,   82,
   83,   84,   85,   86,    0,   87,   88,   89,   90,   91,
   92,   93,   62,  133,  134,  135,  136,    0,   50,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   49,    0,   24,   25,   26,   27,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   62,  133,
  134,  135,  136,    0,   50,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,   62,    0,   24,   25,
   26,   27,    0,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,   62,    0,   24,   25,   26,   27,
  386,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,    0,    0,   24,   25,   26,   27,
  };
  protected static readonly short [] yyCheck = {           121,
    0,   53,  158,  222,   42,   44,   68,   44,  130,  131,
  181,   40,   52,   53,   40,   71,   52,   60,    0,   62,
   59,   63,   59,   59,  152,  125,   44,  126,  126,   29,
   44,  267,   32,   33,   34,   35,  189,  190,   44,   27,
   44,   59,   58,  123,  126,  160,  346,   29,  139,  202,
   38,   40,  125,  437,   67,   58,  125,   44,  512,   44,
  125,   44,  257,  115,   44,  519,  257,   67,   44,  268,
  269,   71,   59,  113,  130,  115,   58,  113,   58,   53,
   44,   40,  186,  187,  237,   67,   40,  264,  265,   44,
  285,  131,  270,  225,  285,  286,  287,  288,   41,  257,
  152,   44,   91,  225,   59,  257,   31,  158,  279,  123,
   93,  127,   38,  125,   40,   41,   42,  239,   44,  125,
  123,  125,  173,  174,  175,  509,   51,   40,  141,   40,
  130,  123,   91,   59,  125,  151,  136,   91,  189,  190,
  125,  141,  264,   33,  270,  127,  128,  187,   38,  125,
   40,  202,   42,   43,  136,   45,  247,  131,  257,  141,
  285,  125,  123,  278,   44,   91,   43,  126,   45,   59,
  406,  299,  325,  326,  327,   61,  280,  257,  282,   59,
  302,  285,  286,  287,  257,  307,  237,  285,  257,   73,
  346,   41,  291,  291,  250,  235,  258,   61,  498,  239,
  126,  323,   93,  285,  308,  285,  289,   33,  361,  291,
   41,  263,   38,  126,   40,  126,   42,   43,  350,   45,
  320,  321,   40,  266,  267,  265,  379,  152,  350,  265,
  272,  267,  354,   59,  356,   41,  126,  359,   44,  343,
  280,  270,  282,   41,  270,   58,   44,  299,  401,   44,
  250,  290,   59,  290,  270,  320,  321,  297,  298,  381,
  298,  235,  384,  263,   59,  149,  254,  255,  256,  307,
  308,  271,  290,   91,  325,  326,  327,  328,  329,  330,
  331,  332,  333,  334,  335,  336,  337,  338,  339,  340,
  341,  342,  463,   41,  465,  346,  257,  123,  257,  125,
  126,  520,  521,  328,  329,  437,  459,  289,  320,  321,
  361,  330,  331,  396,  397,  437,  356,  473,  489,   38,
  491,   40,  247,   42,  285,  447,  497,  383,  379,  320,
  321,  257,  291,  187,  505,   58,  507,  469,  263,   38,
   59,   40,  498,   42,  257,  477,  257,  469,   41,   33,
  401,   44,  404,   58,   38,  477,   40,  241,   42,   43,
  123,   45,  466,  467,  468,  291,  285,  257,  258,  259,
  260,   44,  262,  263,   37,   59,   46,  509,  291,   42,
  291,  297,  298,  383,   47,  298,   41,  509,  280,   44,
  282,   61,   91,  298,  307,  308,   38,   41,   40,   41,
   42,   61,  307,  308,  404,  336,  337,  126,  459,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   91,  473,  313,  314,   58,  280,   41,  282,   40,
   44,  257,  258,  259,  260,   41,  262,  263,   44,  123,
   40,  125,  126,  297,  298,   40,   40,  498,   38,   91,
   40,   59,   42,  257,   38,   59,   40,   41,   42,  285,
  332,  333,  334,  335,   38,   94,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  313,  314,  315,
  316,  317,  318,  124,  320,  321,  322,  271,  324,  325,
  326,  327,  328,  329,  330,  331,   33,   91,   41,   41,
   93,   38,   44,   40,  257,   42,   43,   41,   45,   41,
   44,   41,   44,   41,   44,   41,   44,   41,   44,  325,
   44,   41,   59,   40,   44,   61,  126,  285,  257,   46,
   59,   41,  257,  257,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  257,   61,  315,  316,  317,
  318,   59,  291,  257,  258,  259,  260,   41,  262,  263,
   93,   33,   41,   41,   91,   93,   38,   41,   40,   40,
   42,   43,   41,   45,   93,   93,  123,   93,  125,  126,
   41,  285,   59,   93,   59,  323,  123,   59,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  313,
  314,  315,  316,  317,  318,   58,  123,   59,  322,   58,
  324,  325,  326,  327,  328,  329,  330,  331,   41,   35,
   41,   41,  469,  307,   40,   41,   33,  338,   44,  339,
  342,   38,  340,   40,  341,   42,   43,  257,   45,  117,
  373,  123,   78,   59,  126,   61,   40,   41,  271,  351,
   44,  141,   59,   29,   -1,  501,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  291,   -1,   -1,   -1,   91,   92,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   91,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,  123,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,  123,   42,   43,  126,
   45,   46,   -1,   -1,  261,  262,  263,   -1,  285,   -1,
   -1,   -1,  126,   -1,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  313,  314,  315,  316,
  317,  318,   -1,   -1,   -1,  322,   91,  324,  325,  326,
  327,  328,  329,  330,  331,  257,  258,  259,  260,   -1,
  262,  263,   35,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,
  125,  126,   -1,  285,   -1,   -1,   59,   -1,   61,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,  318,   -1,   -1,   92,
  322,   -1,  324,  325,  326,  327,  328,  329,  330,  331,
  257,  258,  259,  260,   -1,  262,  263,   38,   -1,   40,
   -1,   42,   -1,  257,   -1,   -1,   -1,   -1,   -1,  285,
  123,   -1,   -1,   -1,  290,   -1,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  291,   -1,  315,
  316,  317,  318,   35,   -1,   -1,  313,  314,   -1,   41,
   91,   -1,   44,   -1,   -1,  322,  332,  324,  325,  326,
  327,  328,  329,  330,  331,   -1,   -1,   59,   -1,   61,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   40,
   41,   -1,   -1,   44,   -1,  126,   35,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   92,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   -1,   61,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  313,  314,
   91,  123,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   92,   35,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  126,   33,   -1,   59,   -1,
   61,   38,  285,   40,  123,   42,   43,  290,   45,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
   35,   92,  315,  316,  317,  318,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,  332,
   -1,   -1,   -1,   -1,   -1,   -1,   61,   -1,   -1,   -1,
   -1,   -1,  123,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
  291,   -1,   -1,   -1,   -1,   -1,  123,   92,   -1,  126,
   58,   59,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,
   -1,   -1,   38,   -1,   40,   41,   42,   43,  123,   45,
   -1,   -1,   -1,  285,   -1,   93,  257,   -1,  290,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,   -1,   -1,  315,  316,  317,  318,  125,   -1,   -1,
  291,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,
  332,  290,   -1,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,   -1,   -1,  315,  316,  317,  318,
  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  332,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,  285,  262,  263,   -1,   -1,  290,
   -1,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   -1,   33,  315,  316,  317,  318,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,  332,   -1,   -1,   -1,   -1,  313,  314,   -1,   59,
  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,   -1,   -1,   33,   -1,   -1,   -1,
   -1,   38,  290,   40,   -1,   42,   43,  332,   45,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   33,  126,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,  285,
   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  313,  314,  315,
  316,  317,  318,   -1,   -1,   -1,   38,   -1,   40,   41,
   42,   -1,   44,   33,   -1,   93,   -1,   -1,   38,  126,
   40,   33,   42,   43,   -1,   45,   38,   59,   40,   -1,
   42,   43,   -1,   45,   46,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,   91,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   91,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,
   -1,  123,   -1,  125,  126,  285,   -1,   -1,   -1,   -1,
   -1,   -1,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  313,  314,  315,  316,  317,  318,  126,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   46,   -1,   -1,   -1,   -1,   -1,  285,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   -1,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  313,  314,  315,  316,
  317,  318,   -1,   -1,   -1,   -1,  294,   91,   -1,   -1,
  298,   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,  307,
  308,   -1,   -1,   -1,   -1,  313,  314,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,  123,
  262,  263,  126,   33,   -1,   -1,   -1,   -1,   38,  291,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   41,
   42,   43,   -1,   45,  294,   -1,   -1,   -1,  298,   -1,
  257,  258,  259,  260,   -1,  262,  263,  307,  308,   -1,
   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   33,   -1,
   -1,  313,  314,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  298,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  307,  308,   -1,   33,   -1,   -1,  313,  314,   38,   -1,
   40,   -1,   42,   43,   -1,   45,  126,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,   -1,   33,   -1,   93,   -1,
   -1,   38,   -1,   40,   33,   42,   43,   -1,   45,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,  126,   -1,   93,   -1,   -1,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,
   -1,   -1,   59,   33,   93,   -1,  126,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   33,   -1,   -1,  313,
  314,   38,   -1,   40,   -1,   42,   43,   -1,   45,  126,
   -1,   -1,   -1,   -1,   33,   -1,   -1,  126,   -1,   38,
   -1,   40,   33,   42,   43,   -1,   45,   38,   -1,   40,
   41,   42,   43,   -1,   45,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,  126,
  262,  263,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   41,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   -1,  123,   93,   -1,  126,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,  123,  262,  263,  126,
   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   -1,   33,
   -1,  313,  314,   -1,   38,   -1,   40,  126,   42,   43,
   -1,   45,   -1,   -1,   -1,  126,   -1,  257,  258,  259,
  260,   33,  262,  263,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,  313,  314,
  257,  258,  259,  260,   -1,  262,  263,  126,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   33,   -1,   -1,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   -1,   -1,   -1,  313,  314,   -1,   41,   -1,   -1,   44,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,   -1,  126,   58,   59,   -1,  313,  314,   63,   -1,
   -1,   -1,   -1,   -1,  313,  314,   -1,  257,  258,  259,
  260,   -1,  262,  263,  126,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  313,  314,  257,  258,
  259,  260,   -1,  262,  263,   -1,  257,  258,  259,  260,
  126,  262,  263,   38,   -1,   40,   41,   42,   -1,   44,
  125,   -1,   -1,  313,  314,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   -1,  313,  314,  257,  258,
  259,  260,   -1,  262,  263,   -1,   38,   -1,   40,   41,
   42,   -1,   44,   -1,  313,  314,   -1,   -1,   -1,   -1,
   -1,   -1,  313,  314,   -1,   -1,   91,   59,   -1,   38,
   -1,   40,   41,   42,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   59,   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   91,
   -1,  126,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   91,   -1,   -1,   38,   -1,   40,   41,   42,
   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,  313,
  314,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,   -1,
   -1,  313,  314,   -1,   -1,   -1,  271,  272,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   41,   91,   -1,
   44,   -1,   -1,   -1,   -1,  290,   58,   59,   -1,   -1,
   -1,   63,   -1,   -1,   58,   59,   -1,  313,  314,   63,
   -1,   38,   -1,   40,   -1,   42,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   59,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   38,   -1,   40,   41,   42,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,
  285,  125,   -1,   -1,   -1,  257,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  257,  126,
  315,  316,  317,  318,   91,   41,   -1,   -1,   -1,  291,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  257,   41,  315,  316,  317,  318,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  272,   -1,  315,  316,  317,  318,   -1,  271,  272,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,   59,
  257,   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,
   -1,   -1,   -1,  270,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,   -1,   -1,  315,  316,
  317,  318,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,   -1,   -1,  315,  316,
  317,  318,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,   -1,   -1,  315,
  316,  317,  318,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,
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
   -1,   -1,   41,   42,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   38,   -1,   40,   41,   42,   -1,   44,   58,
   59,   60,   -1,   62,   63,   93,   94,  124,  125,   -1,
   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   91,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   41,   -1,   -1,   44,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,
  126,   -1,   -1,   63,   -1,   -1,   -1,   -1,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   93,   38,   -1,   -1,   41,  290,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   -1,   -1,  124,  125,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,  282,  283,  284,   -1,   93,
   94,   -1,   -1,  290,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,   -1,   -1,   -1,
  124,  125,  290,   -1,   -1,  264,  265,  266,  267,  268,
  269,  257,  271,  272,   38,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  290,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   -1,   38,   -1,   -1,   41,  291,   43,   44,   45,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,   93,
   94,   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  271,  272,   -1,   -1,   -1,   93,   94,   -1,   38,
  124,  125,   41,   -1,   -1,   44,   93,   94,   -1,   38,
  290,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   58,
   59,   60,   -1,   62,   63,   -1,   -1,  124,  125,   58,
   59,   60,   -1,   62,   63,   -1,   -1,  124,  125,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   -1,
   38,   -1,   -1,   41,   93,   94,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,  290,   -1,   -1,   -1,
   58,   59,   60,   -1,   62,   63,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,   38,   93,   94,   41,   -1,   -1,
   44,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   58,   59,   93,   94,   -1,   63,  124,  125,   -1,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,  124,  125,   93,  290,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,   38,
  124,  125,   41,  290,   -1,   44,  124,  125,   -1,   38,
   -1,   -1,   41,  290,   -1,   44,   -1,   -1,   -1,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   58,
   59,   60,   -1,   62,   63,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   93,   94,   -1,   -1,   -1,   -1,
   -1,  290,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
   -1,  290,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,  124,  125,   -1,  266,  267,
  268,  269,   -1,  271,  272,  124,  125,   58,   59,   -1,
   -1,   -1,   63,   38,   -1,   38,   41,   -1,   41,   44,
   -1,   44,  290,   -1,   -1,  266,  267,  268,  269,   -1,
  271,  272,   -1,   58,   59,   58,   59,   -1,   63,   -1,
   63,   38,   93,   94,   41,   -1,   -1,   44,   -1,  290,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   58,   59,  271,  272,   -1,   63,   -1,   93,   94,
   93,   94,   -1,  124,  125,   -1,  290,   -1,   -1,   -1,
   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   93,   94,   -1,  124,
  125,  124,  125,   41,   -1,   -1,   44,   -1,   -1,   58,
   59,   -1,   41,   -1,   63,   44,   -1,   -1,   -1,   -1,
   58,   59,   -1,   -1,   -1,   63,   -1,  124,  125,   58,
   59,   -1,   -1,   -1,   63,   -1,   -1,  266,  267,  268,
  269,   -1,  271,  272,   93,   94,   -1,  266,  267,  268,
  269,   41,  271,  272,   44,   93,   94,   -1,   -1,   -1,
   -1,  290,   -1,   -1,   93,   94,   -1,   -1,   58,   59,
   -1,  290,   -1,   63,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   37,   38,   93,   40,   -1,   42,   43,   44,   45,
   46,   47,   -1,   -1,   -1,   -1,   -1,  268,  269,   -1,
  271,  272,   -1,   59,   60,   61,   62,   63,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,  290,
   -1,   -1,   -1,  268,  269,   -1,  271,  272,  271,  272,
   -1,   -1,   -1,   -1,   -1,   91,   -1,   -1,   94,   -1,
   -1,   -1,   -1,   -1,   -1,  290,   -1,  290,   -1,   -1,
   -1,  268,  269,   -1,  271,  272,   -1,   33,   -1,   -1,
   -1,   37,   38,   -1,   40,   -1,   42,   43,  124,   45,
  123,   47,   -1,  290,  285,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   60,   61,   62,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  271,  272,  315,  316,  317,  318,   -1,   -1,
   -1,   -1,   -1,  271,  272,   91,   -1,   -1,   94,   -1,
   -1,  290,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  290,  125,  126,   -1,   -1,   -1,   -1,  124,   -1,
  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,  126,
   -1,   -1,   -1,   -1,   -1,  261,  262,  263,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,  282,  283,  284,   -1,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
   -1,   -1,  315,  316,  317,  318,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  262,  263,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  273,  274,  275,
  276,  277,  285,  286,  287,  288,  289,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  257,   -1,  315,  316,  317,  318,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,  286,
  287,  288,  289,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  285,   -1,  315,  316,
  317,  318,   -1,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  285,   -1,  315,  316,  317,  318,
  319,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   -1,   -1,  315,  316,  317,  318,
  };

#line 1240 "CParser.jay"

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
