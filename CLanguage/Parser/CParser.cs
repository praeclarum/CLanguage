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
//t    "class_block_item : function_definition",
//t    "class_block_item : ctor_declarator compound_statement",
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
//t    "ctor_declarator : TYPE_NAME '(' parameter_type_list ')'",
//t    "ctor_declarator : TYPE_NAME '(' ')'",
//t    "ctor_declarator : '~' TYPE_NAME '(' ')'",
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
case 264:
  case_264();
  break;
case 265:
#line 982 "CParser.jay"
  {
		yyVal = new VirtualDeclarationStatement((Statement)yyVals[0+yyTop]) { IsVirtual = true };
	}
  break;
case 266:
  case_266();
  break;
case 267:
  case_267();
  break;
case 268:
#line 996 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 269:
  case_269();
  break;
case 270:
#line 1010 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 271:
#line 1011 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 272:
#line 1012 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 273:
#line 1019 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 274:
#line 1023 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 275:
#line 1027 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()), parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 276:
#line 1031 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: new IdentifierDeclarator((yyVals[-2+yyTop]).ToString()), parameters: new List<ParameterDeclaration>());
	}
  break;
case 277:
#line 1035 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: new IdentifierDeclarator("~" + (yyVals[-2+yyTop]).ToString()), parameters: new List<ParameterDeclaration>());
	}
  break;
case 278:
  case_278();
  break;
case 279:
#line 1053 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 280:
#line 1057 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 281:
#line 1064 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 282:
#line 1068 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 283:
#line 1072 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-4+yyTop], (List<SwitchCase>)yyVals[-1+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 284:
#line 1076 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-3+yyTop], new List<SwitchCase>(), GetLocation(yyVals[-5+yyTop]));
	}
  break;
case 285:
#line 1083 "CParser.jay"
  {
		yyVal = new List<SwitchCase> { (SwitchCase)yyVals[0+yyTop] };
	}
  break;
case 286:
  case_286();
  break;
case 287:
#line 1095 "CParser.jay"
  {
		yyVal = new SwitchCase((Expression)yyVals[-2+yyTop], (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 288:
#line 1099 "CParser.jay"
  {
		yyVal = new SwitchCase(null, (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 289:
#line 1106 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 290:
#line 1110 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 291:
#line 1114 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 292:
#line 1118 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 293:
#line 1122 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 294:
#line 1126 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 296:
#line 1134 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 297:
#line 1138 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 298:
#line 1142 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 299:
#line 1146 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 300:
  case_300();
  break;
case 301:
  case_301();
  break;
case 306:
  case_306();
  break;
case 307:
  case_307();
  break;
case 308:
#line 1194 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 309:
#line 1198 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-2+yyTop]).ToString()).Push((yyVals[0+yyTop]).ToString());
	}
  break;
case 310:
#line 1202 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("~" + (yyVals[0+yyTop]).ToString());
	}
  break;
case 311:
#line 1203 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 312:
#line 1204 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
  break;
case 313:
#line 1206 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("operator" + (string)yyVals[0+yyTop]); }
  break;
case 314:
#line 1208 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("operator" + (string)yyVals[0+yyTop]); }
  break;
case 315:
  case_315();
  break;
case 316:
  case_316();
  break;
case 317:
  case_317();
  break;
case 318:
  case_318();
  break;
case 319:
  case_319();
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
#line 971 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			fdecl,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_266()
#line 984 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsOverride = true };
	}

void case_267()
#line 989 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsOverride = true };
	}

void case_269()
#line 1001 "CParser.jay"
{
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator((Declarator)yyVals[-3+yyTop], null) };
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-4+yyTop], decls);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsPureVirtual = true };
	}

void case_278()
#line 1040 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		DeclarationSpecifiers ds = new DeclarationSpecifiers();
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator(fdecl, null) };
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_286()
#line 1085 "CParser.jay"
{
		((List<SwitchCase>)yyVals[-1+yyTop]).Add((SwitchCase)yyVals[0+yyTop]);
		yyVal = yyVals[-1+yyTop];
	}

void case_300()
#line 1151 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_301()
#line 1156 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_306()
#line 1171 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_307()
#line 1180 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_315()
#line 1213 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_316()
#line 1222 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_317()
#line 1234 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_318()
#line 1240 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_319()
#line 1246 "CParser.jay"
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
   67,   67,   67,   67,   67,   67,   67,   67,   72,   68,
   68,   68,   71,   71,   71,   71,   71,   69,   60,   60,
   61,   61,   61,   61,   73,   73,   74,   74,   62,   62,
   62,   62,   62,   62,   63,   63,   63,   63,   63,    0,
    0,   75,   75,   75,   75,   70,   70,   78,   78,   78,
   78,   78,   78,   78,   76,   76,   77,   77,   77,   79,
   79,   79,
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
    1,    1,    1,    2,    2,    4,    5,    2,    5,    2,
    2,    2,    4,    3,    4,    3,    4,    2,    1,    2,
    5,    7,    7,    6,    1,    2,    4,    3,    5,    7,
    6,    7,    6,    7,    3,    2,    2,    2,    3,    1,
    2,    1,    1,    1,    1,    4,    3,    1,    3,    4,
    3,    4,    4,    4,    4,    5,    1,    2,    2,    1,
    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,    0,   95,   96,   97,   98,   99,  143,  204,  101,
  102,  103,  104,  107,  108,  105,  106,  203,  205,  100,
  109,  110,  111,  127,  128,  129,    0,  304,    0,  303,
    0,    0,    0,    0,    0,  112,  113,    0,  302,  300,
  305,    0,    0,  115,  116,    0,    0,  301,  178,    0,
    0,    0,    0,    0,   81,    0,   91,    0,    0,    0,
    0,  114,   84,   86,   88,   90,    0,    0,  119,    0,
    0,  309,    0,    0,    0,    0,  139,    0,  168,  169,
  159,  160,  157,  158,  153,  154,  166,  167,  173,  174,
  175,  171,  172,    0,    0,  161,  150,  148,  149,  165,
  164,  151,  152,  155,  156,  163,  162,  170,  180,    0,
  201,    0,    0,  199,    0,  179,    0,   82,  320,    0,
    0,  321,  322,  317,    0,  307,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  256,    0,  260,
    0,    0,    0,  258,  261,  262,  263,    0,    0,  117,
  311,    0,    0,    0,    0,    0,    0,  208,  313,  310,
    0,  134,    0,    0,  176,  177,  185,  202,    0,  200,
   92,    0,    0,    2,    3,    0,    0,    0,    4,    5,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  250,    0,    0,   27,   28,   29,   30,  279,    7,    0,
    0,   78,    0,   33,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   63,  254,  255,  243,  244,
  245,  246,  247,  248,    0,  252,    1,    0,  231,   94,
  319,  306,  318,  196,    0,   17,    0,    0,  193,    0,
    0,    0,  181,    0,    0,    0,  270,  271,  272,  265,
    0,  268,    0,    0,    0,  257,  259,  278,  264,    0,
    0,    0,  123,    0,  121,  314,  312,  315,    0,    0,
    0,    0,    0,  212,    0,    0,    0,   31,   80,  142,
  136,  140,  135,    0,    0,    0,   25,    0,   20,   21,
    0,    0,    0,    0,    0,    0,  296,  297,  298,    0,
    0,    0,    0,    0,    0,   22,   23,    0,  280,    0,
   13,   14,    0,    0,    0,   66,   67,   68,   69,   70,
   71,   72,   73,   74,   75,   76,   77,   65,    0,   24,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  251,
  253,    0,    0,    0,  234,    0,    0,  239,  195,    0,
  194,    0,  192,  188,    0,  187,    0,    0,  183,  182,
  276,    0,    0,    0,    0,    0,  274,    0,  124,  125,
  126,    0,  118,  227,    0,    0,  221,    0,    0,    0,
    0,    0,    0,  316,  207,  209,  137,  249,    0,    0,
    0,    0,    0,    0,    0,    0,  295,  299,    6,    0,
  130,  132,    0,  217,    0,  215,   79,   12,    9,    0,
    0,   11,   64,   34,   35,   36,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  242,  232,    0,  235,  238,  240,   18,
    0,    0,  191,  186,  275,    0,    0,  277,  266,  273,
  122,  228,  220,  225,  222,  211,  229,    0,  223,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   32,   10,    8,    0,  241,  233,  236,    0,  189,  190,
  267,    0,  230,  226,  224,    0,    0,  289,    0,    0,
    0,    0,    0,    0,   62,  237,  269,    0,    0,    0,
  284,    0,  285,    0,  293,    0,  291,    0,   15,    0,
  282,    0,    0,  283,  286,  290,  294,  292,   16,    0,
    0,    0,
  };
  protected static readonly short [] yyDgoto  = {            29,
  199,  200,  201,  235,  302,  354,  202,  203,  204,  205,
  206,  207,  208,  209,  210,  211,  212,  213,  214,  215,
  216,  329,  280,  217,  125,   56,   32,   33,   34,   35,
   57,   58,  230,   36,   37,  263,   38,   69,  264,  265,
  305,   76,   77,   59,   60,  113,  109,   61,  385,  157,
  158,  386,  275,  356,  357,  358,  218,  219,  220,  221,
  222,  223,  224,  225,  226,  143,  144,  145,  146,   39,
  148,  252,  512,  513,   40,   41,  127,   42,  128,
  };
  protected static readonly short [] yySindex = {         2520,
 -218,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  -80,    0, 2520,    0,
   63, 3681, 3681, 3681, 3681,    0,    0,  247,    0,    0,
    0,  -34,  -36,    0,    0, -196,  -45,    0,    0, 3808,
   55,  -32,   11, -187,    0,  221,    0,   80,   22,   17,
 -182,    0,    0,    0,    0,    0, 3746,    2,    0,   62,
 2376,    0, 3808, -145,  102,    7,    0, -196,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  144,  106,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  163,
    0,   17,  -32,    0,   11,    0,   55,    0,    0,  299,
  740,    0,    0,    0,   63,    0, 3653, 3681,   17, 1026,
 1358,  -84,  182,  199,  203,  219, 3681,    0, -165,    0,
   63,  -17, 3802,    0,    0,    0,    0,   30,  280,    0,
    0, 3808,   36,  192,  869,  290,  296,    0,    0,    0,
 1914,    0,  -98,   10,    0,    0,    0,    0,   17,    0,
    0,  308,  294,    0,    0, 1922, 1958, 1958,    0,    0,
  321,  373,  381,  599,  387,  148,  392,  401, 1237, 1324,
    0, 1914, 1914,    0,    0,    0,    0,    0,    0,  350,
  309,    0,  269,    0, 1914,  405,  540,    3,   57, -169,
  398,  406,  402,  260,  -38,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  390,    0,    0, 1520,    0,    0,
    0,    0,    0,    0,  327,    0,  494,  147,    0,  631,
  447, 1447,    0, 3808,  297, 2426,    0,    0,    0,    0,
   63,    0,  520,  -30, 2454,    0,    0,    0,    0,  -90,
  -90,  -90,    0,    6,    0,    0,    0,    0, 2154, 1636,
  -32,  503,   42,    0,   84,  192, 2292,    0,    0,    0,
    0,    0,    0,  -81,  599, 1324,    0, 1324,    0,    0,
 1914, 1914, 1914,  244, 1219,  521,    0,    0,    0,  372,
  409,  546, 3830, 3830,   27,    0,    0, 1914,    0,  361,
    0,    0, 1534, 1914,  363,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1914,    0,
 1914, 1914, 1914, 1914, 1914, 1914, 1914, 1914, 1914, 1914,
 1914, 1914, 1914, 1914, 1914, 1914, 1914, 1914, 1914,    0,
    0, 1914,  374,   32,    0,  740,  365,    0,    0, 1914,
    0, 1430,    0,    0, 1914,    0, 1658,  545,    0,    0,
    0,  602,  -28,  579,  604,  587,    0,  610,    0,    0,
    0,  280,    0,    0,  613,  614,    0, 1675,  563, 1914,
   84, 2491, 1762,    0,    0,    0,    0,    0,  621,  624,
  417,  421,  473,  626, 1776, 1776,    0,    0,    0, 1784,
    0,    0, 2321,    0,  118,    0,    0,    0,    0,  477,
  127,    0,    0,    0,    0,    0,  405,  405,  540,  540,
    3,    3,    3,    3,   57,   57, -169,  398,  406,  402,
  260,   52,  574,    0,    0,  925,    0,    0,    0,    0,
  575,  577,    0,    0,    0,  618, 1799,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  638,    0, 1820,
  588,  549,  549,  599,  557,  599, 1914, 1842, 1886, 1520,
    0,    0,    0, 1914,    0,    0,    0,  740,    0,    0,
    0,  650,    0,    0,    0,  388, -120,    0,  478,  599,
  479,  599,  483,   35,    0,    0,    0,  599, 1914,  655,
    0,  -93,    0,  664,    0,  599,    0,  599,    0, 1335,
    0,  668,  496,    0,    0,    0,    0,    0,    0,  496,
  496,  496,
  };
  protected static readonly short [] yyRindex = {            0,
    0, 2219,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  -20,   43,  619,  827,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2026,    0,    0,    0,
    0,    0, 1682,    0,    0,    0,    0,   15,    0,  793,
  725,    0,    0,    0,    0,    0,    0, 2098,    0,    0,
    0,    0,    0,    0,   72,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  839,    0,    0, 1946,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  915,    0,
    0,    0, 2248,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  484,    0,  686,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1137,    0,
    0,   15, 3743,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2829,    0, 2897,    0,    0, 2925, 3172, 3219, 3331, 3525,
 3552, 3593, 3364, 3646,  120,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  489,  532,  533,    0,  534,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  366,  377,  687,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   15,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  538,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  688,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2934, 3114, 3182, 3209,
 3267, 3277, 3304, 3372, 3457, 3466, 3554, 3564, 3639, 3401,
 3652,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2857,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2802,    0,    0,    0,  181,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  -88,  -78,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -76,    0,  418,  300,  250, -121,  -50, 1881,    0,
  255,   20,  131,  291,  389,  391,  386,  393,  394,    0,
 -158,    0, -326,   19,    1, -100,    0,  160,  -40,    0,
  617,  312, -172,    0,    0,  -10,    0,  -53,    0,  353,
  332,  659, -114,  -51,   93,   -8,  -15,   -4,  -64,    0,
  461, -109, -235, -366,    0,  382, -173,    0,  -18, -240,
    0,    0,    0, -337, -221,    0,  598,    0,    0,   31,
    0,    0,    0,  231,  715,    0,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           229,
   31,  114,  279,  351,  511,   71,  156,   51,  236,  241,
  294,  111,  111,  117,  150,  117,   47,   83,   30,   83,
   83,   83,  255,   83,  349,  443,  281,   68,  118,   31,
  118,  524,   63,   64,   65,   66,  288,  391,   83,  126,
  254,  245,   46,  397,  115,  274,  287,   30,  282,  382,
  163,   43,   53,  284,  406,  355,  130,  159,   93,  149,
   75,   51,  142,  170,  414,  237,  413,  141,   53,  116,
   83,  155,  168,   93,  168,  446,  124,   78,  520,  488,
   85,  269,   85,   85,   85,  140,   85,  132,  258,   74,
  111,  116,   52,   54,   51,  308,   53,  147,  342,  343,
   52,   85,   51,  273,   53,   83,  229,  131,  232,  484,
  278,  398,  300,  301,  122,  141,  340,  270,  341,  253,
  368,   55,  242,  392,   67,  287,  289,  290,   67,  259,
  155,  162,  270,   85,  283,  268,  266,  251,  142,  160,
  121,  278,  278,  141,  112,  231,  233,   54,  389,  304,
  373,  129,  120,  488,  278,  250,  445,  413,   75,  519,
   61,  140,  161,   61,  478,  479,   44,   54,   85,  282,
  308,  123,  243,  147,  393,   75,   44,   61,   61,  391,
   54,  372,  522,  447,  165,  531,  417,  153,   54,  278,
  378,  236,  532,  279,   45,  416,  141,  111,  166,  509,
  510,  168,  120,  167,   45,  169,  244,  423,  270,  301,
  383,  301,   61,  281,  401,  402,  403,  273,  281,  483,
  281,  246,  281,  281,   49,  281,  509,  510,  369,  362,
  111,  288,  288,  348,  229,   70,   83,  421,  450,  281,
  451,  287,  287,  452,   61,  304,  155,  304,   72,  379,
  380,  381,  132,  415,   73,  155,  247,  394,   50,  376,
  248,  456,  304,  304,  117,    9,  336,  337,  466,  155,
   83,  471,  442,  487,   18,   19,  249,  155,   49,  118,
  278,  278,  278,  278,  278,  278,  278,  278,  278,  278,
  278,  278,  278,  278,  278,  278,  278,  278,   49,   85,
  496,  278,  498,  281,   93,  281,  281,  355,    9,  351,
  351,   49,   50,  405,  120,  506,  278,   18,   19,   49,
  267,  168,  338,  339,  229,  505,  515,  468,  517,  328,
  276,  197,   50,   85,  521,  229,  192,  278,  190,  277,
  193,  194,  527,  195,  528,   50,  151,  487,  313,  303,
  279,  285,  152,   50,  315,  429,  430,  198,  229,  278,
  291,  415,  110,  112,   62,  129,  229,  359,  121,   67,
  360,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,  155,  308,   24,   25,   26,   27,  229,  314,
  499,  501,  503,  131,  296,  131,  131,  131,  309,   61,
  353,  119,  292,  155,  133,  308,  133,  133,  133,  278,
  293,  120,  197,  191,  196,  448,  295,  192,  172,  190,
  408,  193,  194,  278,  195,  344,  172,  281,  281,  281,
  281,  333,  281,  281,    9,  303,  331,  303,  198,  409,
  297,  332,  308,   18,   19,  352,  131,  474,  278,  298,
  308,  475,  303,  303,  308,  281,  272,  133,  431,  432,
  433,  434,  281,  281,  281,  281,  281,  281,  281,  281,
  281,  281,  281,  281,  281,  281,  281,  281,  281,  281,
  281,  281,  281,  281,  281,  281,  281,  281,  281,  345,
  281,  281,  281,   44,  281,  281,  281,  281,  281,  281,
  281,  281,  120,  476,  350,  196,  308,  482,  514,  516,
  360,  308,  308,  518,  213,  346,  308,  213,  197,  217,
  347,   45,  217,  192,  361,  190,   44,  193,  194,  364,
  195,  316,  317,  318,  319,  320,  321,  322,  323,  324,
  325,  326,  327,  370,  198,  173,  174,  175,  176,  375,
  177,  178,  374,  390,   45,  260,  261,  262,  404,  310,
  311,  312,  210,  216,  218,  210,  216,  218,  219,  407,
  110,  219,  334,   62,  335,  399,  410,  400,  427,  428,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,  179,  180,   24,   25,   26,   27,  418,  120,  422,
  181,  196,  182,  183,  184,  185,  186,  187,  188,  189,
  444,  197,  435,  436,  411,  412,  192,  454,  190,  457,
  193,  194,  455,  195,  458,  459,  173,  174,  175,  176,
  460,  177,  178,  462,  463,  465,   87,  198,   87,   87,
   87,  472,   87,  197,  473,  477,  485,  489,  192,  490,
  190,  480,  193,  194,   62,  195,  491,   87,  493,  497,
  495,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,  179,  180,   24,   25,   26,   27,  507,   87,
  508,  181,  523,  182,  183,  184,  185,  186,  187,  188,
  189,  120,  526,  363,  196,  530,  206,  214,  216,  504,
  420,  439,  437,  171,  461,  438,  164,  396,  449,  440,
  257,  441,  525,   48,   87,    0,    0,    0,    0,    0,
    0,    0,  173,  174,  175,  176,  196,  177,  178,  184,
    0,    0,    0,    0,  184,  184,    0,    0,  184,    0,
    0,    0,  197,    0,    0,    0,    0,  192,    0,  190,
   62,  193,  194,  184,  195,  184,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,  179,  180,
   24,   25,   26,   27,    0,  184,  184,  181,    0,  182,
  183,  184,  185,  186,  187,  188,  189,  147,    0,    0,
    0,    0,    0,  147,    0,    0,  147,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  184,    0,    0,
    0,  147,    0,  147,    0,  173,  174,  175,  176,    0,
  177,  178,  228,    0,   89,  196,   89,   89,   89,    0,
   89,    0,    0,  145,    0,   87,    0,    0,    0,  145,
    0,    0,  145,    0,  147,   89,    0,  227,  174,  175,
  176,    0,  177,  178,    0,    0,    0,  145,    0,  145,
    0,    0,    0,    0,    0,    0,  271,    0,  269,   87,
   53,  179,  180,    0,    0,  147,    0,   89,    0,    0,
  181,    0,  182,  183,  184,  185,  186,  187,  188,  189,
  145,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  179,  180,    0,    0,    0,    0,  144,
    0,    0,   89,    0,    0,  144,    0,  197,  144,  270,
    0,  145,  192,    0,  190,    0,  193,  194,    0,  195,
  353,    0,    0,  144,    0,  144,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   54,    0,  227,  174,  175,  176,
    0,  177,  178,    0,    0,    0,  144,    0,    0,  184,
    0,    0,    0,    0,  184,  352,  184,  184,  184,  184,
  184,  184,  184,  184,  184,  184,  184,  184,  184,  184,
  184,  184,  184,  184,  184,  184,  184,  144,    0,  184,
  184,  184,  184,    0,    0,    0,    0,  228,    0,  486,
  196,    0,  179,  180,    0,    0,  184,    0,  197,    0,
    0,    0,    0,  192,    0,  190,  234,  193,  194,    0,
  195,    0,    0,    0,    0,    0,    0,  147,    0,    0,
    0,    0,  147,   89,  147,  147,  147,  147,  147,  147,
  147,  147,  147,  147,  147,  147,  147,  147,  147,  147,
  147,  147,  147,  147,  147,    0,    0,  147,  147,  147,
  147,    0,    0,    0,    0,    0,    0,   89,    0,    0,
    0,    0,    0,  145,  147,   49,    0,    0,  145,    0,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  196,    0,  145,  145,  145,  145,    0,    0,   50,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  145,  146,    0,    0,    0,    0,    0,  146,    0,    0,
  146,  227,  174,  175,  176,    0,  177,  178,    0,    0,
    0,    0,    0,    0,    0,  146,    0,  146,    0,  144,
    0,    0,    0,    0,  144,    0,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,    0,  146,  144,
  144,  144,  144,    0,    0,    0,    0,  179,  180,    0,
    0,    0,    0,    0,    0,    0,  144,    0,    0,    0,
    0,  197,    0,    0,    0,    0,  192,    0,  190,  146,
  193,  194,    0,  195,    0,    0,    0,    0,    0,  197,
    0,    0,    0,    0,  192,    0,  190,  198,  193,  194,
    0,  195,  227,  174,  175,  176,    0,  177,  178,    0,
    0,    0,    0,    0,    0,  299,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   62,    0,    0,    0,    0,    0,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,  179,  180,
   24,   25,   26,   27,  196,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  197,    0,    0,    0,
    0,  192,  196,  190,    0,  193,  194,  197,  195,    0,
    0,    0,  192,    0,  190,    0,  193,  194,    0,  195,
  353,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  197,    0,    0,    0,    0,  192,    0,  190,    0,  240,
  194,    0,  195,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  146,    0,    0,    0,  352,  146,    0,  146,  146,
  146,  146,  146,  146,  146,  146,  146,  146,  146,  146,
  146,  146,  146,  146,  146,  146,  146,  146,  146,  196,
  239,  146,  146,  146,  146,    0,    0,  228,    0,  529,
  196,    0,  197,    0,    0,    0,    0,  192,  146,  190,
    0,  193,  194,    0,  195,  227,  174,  175,  176,  197,
  177,  178,    0,  196,  192,    0,  190,    0,  367,  194,
    0,  195,    0,  227,  174,  175,  176,    0,  177,  178,
    0,    0,    0,   62,    0,    0,    0,    0,    0,    0,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,  179,  180,   24,   25,   26,   27,    0,    0,  366,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  179,
  180,    0,  197,    0,    0,  196,    0,  192,    0,  190,
    0,  193,  194,    0,  195,  353,  197,    0,    0,    0,
    0,  192,  196,  190,  419,  193,  194,    0,  195,    0,
  227,  174,  175,  176,    0,  177,  178,    0,    0,    0,
    0,  227,  174,  175,  176,    0,  177,  178,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   62,    0,
  352,    0,    0,    0,  227,  174,  175,  176,    0,  177,
  178,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,  179,  180,   24,   25,
   26,   27,  228,    0,    0,  196,    0,  179,  180,    0,
    0,  238,    0,    0,    0,    9,    0,    0,    0,  196,
    0,    0,    0,    0,   18,   19,    0,    0,  197,    0,
  179,  180,    0,  192,    0,  190,    0,  388,  194,    0,
  195,    0,    0,    0,    0,    0,  227,  174,  175,  176,
  197,  177,  178,    0,    0,  192,    0,  190,    0,  193,
  194,    0,  195,  227,  174,  175,  176,  197,  177,  178,
    0,    0,  192,    0,  190,    0,  193,  194,    0,  195,
    0,  197,  197,    0,    0,  197,    0,    9,  387,    0,
    0,    0,    0,    0,    0,    0,   18,   19,    0,    0,
  365,    0,  179,  180,    9,    0,    0,    0,    0,    0,
  453,    0,    0,   18,   19,    0,    0,    0,    0,  179,
  180,  196,    0,    0,    0,    0,    0,  464,    0,    0,
    0,    0,  197,    0,    0,    0,  227,  174,  175,  176,
    0,  177,  178,  196,    0,    0,    0,    0,    0,    0,
  227,  174,  175,  176,  197,  177,  178,    0,    0,  192,
  196,  190,    0,  470,  194,    0,  195,  197,  197,    0,
    0,    0,    0,  192,    0,  190,  197,  193,  194,    0,
  195,  192,    0,  190,    0,  193,  194,    0,  195,    0,
    0,  197,  179,  180,  198,    0,  192,    0,  190,    0,
  193,  194,    0,  195,    0,    0,  179,  180,    0,    0,
    0,    0,  197,    0,  469,    0,    0,  192,    0,  190,
    0,  193,  194,    0,  195,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  197,    0,    0,    0,    0,  192,
    0,  190,  500,  193,  194,    0,  195,  196,    0,    0,
    0,    0,  227,  174,  175,  176,    0,  177,  178,    0,
    0,  196,    0,    0,    0,    0,  480,    0,    0,  196,
    0,    0,  494,    0,  227,  174,  175,  176,  197,  177,
  178,  228,    0,  192,  196,  190,  502,  193,  194,    0,
  195,  227,  174,  175,  176,    0,  177,  178,  197,    0,
    0,    0,    0,    0,    0,  196,  197,    0,  179,  180,
    0,  192,    0,  190,  197,  193,  194,    0,  195,  192,
    0,  286,    0,  193,  194,    0,  195,  196,    0,    0,
  179,  180,  197,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  198,  198,  179,  180,  198,
  197,    0,    0,    0,    0,  192,    0,  288,    0,  193,
  194,    0,  195,    0,    0,    0,    0,    0,    0,    0,
    0,  196,    0,    0,    0,    0,    0,    0,  227,  174,
  175,  176,    0,  177,  178,    0,    0,    0,    0,    0,
    0,    0,  227,  174,  175,  176,  198,  177,  178,  196,
  227,  174,  175,  176,    0,  177,  178,  196,    0,    0,
    0,    0,    0,    0,    0,  227,  492,  175,  176,    0,
  177,  178,    0,  138,    0,  138,  138,  138,    0,  138,
    0,  198,  306,  307,  179,  180,  227,  174,  175,  176,
    0,  177,  178,  196,  138,  330,    0,    0,  179,  180,
    0,    0,    0,    0,    0,    0,  179,  180,  227,  174,
  175,  176,    0,  177,  178,    0,    0,    0,    0,    0,
    0,  179,  180,    0,    0,    0,  138,    0,    0,    0,
  307,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  179,  180,    0,  120,    0,  120,  120,  120,
    0,  120,  227,  174,  175,  176,    0,  177,  178,    0,
    0,  138,    0,    0,  179,  180,  120,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  227,  174,  175,  176,    0,  177,  178,    0,  227,  174,
  175,  176,    0,  177,  178,    0,    0,    0,  120,    0,
    0,  271,    0,  269,  384,   53,    0,    0,  179,  180,
    0,    0,  198,    0,    0,    0,    0,    0,    0,    0,
    0,  424,  425,  426,  227,  174,  175,  176,    0,  177,
  178,    0,    0,  120,    0,    0,  179,  180,    0,    0,
    0,    0,    0,    0,  179,  180,  198,    0,    0,    0,
    0,    0,    0,    0,  270,    0,    0,  307,    0,    0,
    0,    0,    0,    0,    0,    0,  114,    0,  114,    0,
  114,    0,    0,    0,    0,    0,    0,    0,  307,    0,
  179,  180,    0,    0,    0,    0,    0,  114,    0,   54,
    0,    0,  138,    0,    0,  114,    0,    0,    0,  114,
  481,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  114,    0,    0,    0,
  138,    0,    0,    0,    0,    0,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,    0,    0,
  138,  138,  138,  138,  114,    0,    0,    0,    0,    0,
  307,    0,    0,    0,  120,    0,    0,    0,  414,    0,
  413,  384,   53,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  114,    0,    0,    0,    0,    0,    0,
    0,    0,  120,    0,    0,    0,    0,    0,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,  120,  120,
   49,  270,  120,  120,  120,  120,  154,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   62,    0,
    0,    0,    0,    0,   50,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,  371,    0,   24,   25,
   26,   27,    0,    0,    0,  114,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  308,    0,
    0,    0,    0,    0,  377,    0,    0,    0,    0,    0,
    0,    0,    0,  114,  114,    0,    0,    0,    0,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  467,  114,  114,  114,  114,  114,    0,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
    0,    0,  114,  114,  114,  114,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   62,    0,   28,    0,
    0,    0,    0,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,    0,   62,   24,   25,   26,   27,
  395,    0,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,    0,    0,   24,   25,   26,   27,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   62,    0,    0,    0,    0,    0,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,    0,    0,
   24,   25,   26,   27,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   62,    0,    0,    0,    0,    0,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   62,    0,
   24,   25,   26,   27,    0,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,    0,    0,   24,   25,
   26,   27,    0,    0,    0,   62,    1,    0,    0,    0,
    0,    0,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,    0,    2,   24,   25,   26,   27,    0,
    0,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,    0,    0,   24,   25,   26,   27,    2,    2,
    0,    2,    0,    2,    2,    2,    2,    2,    2,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    2,    2,    2,    2,   19,   19,    0,    0,   19,
   19,   19,   19,   19,    0,   19,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   19,   19,   19,   19,
   19,   19,    2,   26,   26,    2,    0,   26,   26,   26,
   26,   26,    0,   26,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   26,   26,   26,   26,   26,   26,
    0,   19,   19,    0,    0,    2,    0,    0,    0,    0,
    0,    0,    0,   31,   31,    0,    0,   31,   31,   31,
   31,   31,    0,   31,    0,    0,    0,    0,    0,   26,
   26,    0,   19,   19,   31,   31,   31,    0,   31,   31,
    0,    0,   37,    0,    0,   37,    0,   37,   37,   37,
    0,   38,    0,    0,   38,    0,   38,   38,   38,    0,
   26,   26,   37,   37,   37,    0,   37,   37,    0,   31,
   31,   38,   38,   38,    0,   38,   38,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   37,   37,    0,
   31,   31,    0,    0,    0,    0,   38,   38,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   37,   37,
    0,    0,    0,    0,    0,    0,    0,   38,   38,    0,
    0,    0,    2,    2,    2,    2,    2,    2,    2,    2,
    2,    0,    2,    2,    2,    2,    2,    2,    2,    2,
    2,    2,    2,    2,    2,    2,    0,    0,    0,    0,
    0,    2,   19,   19,   19,   19,   19,   19,    0,   19,
   19,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,    0,    0,    0,    0,    0,   19,    0,
   26,   26,   26,   26,   26,   26,    0,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,   26,
   26,    0,    0,    0,    0,    0,   26,    0,    0,    0,
    0,   39,    0,    0,   39,    0,   39,   39,   39,    0,
   31,   31,   31,   31,   31,   31,    0,   31,   31,    0,
    0,   39,   39,   39,    0,   39,   39,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   31,    0,   37,   37,
   37,   37,   37,   37,    0,   37,   37,   38,   38,   38,
   38,   38,   38,    0,   38,   38,   39,   39,    0,   40,
    0,    0,   40,    0,   37,   40,    0,    0,    0,   41,
    0,    0,   41,   38,    0,   41,    0,    0,    0,   40,
   40,   40,    0,   40,   40,    0,    0,   39,   39,   41,
   41,   41,    0,   41,   41,    0,   42,    0,    0,   42,
    0,    0,   42,    0,    0,    0,   43,    0,    0,   43,
    0,    0,   43,    0,   40,   40,   42,   42,   42,    0,
   42,   42,    0,    0,   41,   41,   43,   43,   43,    0,
   43,   43,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   40,   40,    0,    0,    0,
    0,   42,   42,    0,   46,   41,   41,   46,    0,    0,
   46,   43,   43,    0,   47,    0,    0,   47,    0,    0,
   47,    0,    0,    0,   46,   46,   46,    0,   46,   46,
    0,    0,   42,   42,   47,   47,   47,    0,   47,   47,
    0,   44,   43,   43,   44,    0,    0,   44,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   46,
   46,   44,   44,   44,    0,   44,   44,    0,   48,   47,
   47,   48,    0,    0,   48,    0,    0,   39,   39,   39,
   39,   39,   39,    0,   39,   39,    0,    0,   48,   48,
   46,   46,    0,   48,    0,    0,   44,   44,    0,    0,
   47,   47,    0,   39,   57,    0,    0,   57,    0,   45,
    0,    0,   45,    0,    0,   45,    0,    0,    0,    0,
    0,   57,   57,   48,   48,    0,   57,   44,   44,   45,
   45,   45,    0,   45,   45,   40,   40,   40,   40,   40,
   40,   58,   40,   40,   58,   41,   41,   41,   41,   41,
   41,    0,   41,   41,   48,   48,   57,    0,   58,   58,
    0,   40,    0,   58,   45,   45,    0,    0,    0,    0,
    0,   41,   42,   42,   42,   42,   42,   42,    0,   42,
   42,    0,    0,    0,   43,   43,   43,   43,   57,   43,
   43,    0,    0,   58,   49,   45,   45,   49,   42,    0,
   49,    0,    0,   50,    0,    0,   50,    0,   43,   50,
    0,    0,    0,    0,   49,   49,    0,    0,    0,   49,
    0,    0,    0,   50,   50,   58,    0,    0,   50,    0,
    0,    0,   46,   46,   46,   46,    0,   46,   46,    0,
    0,    0,   47,   47,   47,   47,    0,   47,   47,   49,
   49,    0,    0,    0,    0,    0,   46,    0,   50,   50,
    0,    0,   51,    0,    0,   51,   47,    0,   51,   44,
   44,   44,   44,    0,   44,   44,    0,    0,    0,    0,
   49,   49,   51,   51,    0,    0,    0,   51,    0,   50,
   50,   52,   53,   44,   52,   53,    0,   52,   48,   48,
    0,   48,   48,    0,   54,    0,    0,   54,    0,   53,
   53,   52,   52,    0,   53,    0,   52,   51,   51,    0,
   48,   54,   54,    0,    0,    0,   54,    0,    0,    0,
    0,    0,    0,   55,   57,   57,   55,   45,   45,   45,
   45,    0,   45,   45,   53,   53,   52,   52,   51,   51,
   55,   55,    0,   57,    0,   55,   54,   54,    0,    0,
    0,   45,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   58,   58,    0,    0,   53,   53,   52,   52,   56,
    0,    0,   56,    0,    0,   55,   59,   54,   54,   59,
   58,    0,   60,    0,    0,   60,   56,   56,    0,    0,
    0,   56,    0,   59,   59,    0,    0,    0,   59,   60,
   60,    0,    0,    0,   60,    0,   55,   55,    0,    0,
    0,    0,    0,    0,   49,   49,    0,   49,   49,    0,
    0,   56,    0,   50,   50,    0,   50,   50,   59,    0,
    0,    0,    0,    0,   60,    0,   49,    0,    0,    0,
    0,    0,    0,    0,    0,   50,    0,    0,    0,    0,
    0,    0,   56,   56,    0,    0,    0,    0,    0,    0,
   59,    0,    0,    0,    0,  120,   60,    0,    0,    1,
    1,    0,    1,    0,    1,    1,    1,    1,    1,    1,
    0,    0,    0,    0,    0,   51,   51,    0,    0,    0,
    0,    1,    1,    1,    1,    1,    0,    0,    0,    0,
    0,    0,    0,    0,   51,    0,    0,    0,    0,    0,
    0,    0,   53,   53,   52,   52,    0,    0,    0,    0,
    0,    0,    0,    1,   54,   54,    1,    0,    0,    0,
  101,   53,    0,   52,  103,   96,    0,   94,    0,   97,
   98,    0,   99,   54,  102,    0,    0,    0,    0,    0,
    0,    0,    0,   55,   55,    0,    1,  104,  108,  105,
  138,  139,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   55,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   95,    0,
    0,  106,    0,    0,    0,    0,    0,    0,    0,   56,
   56,    0,    0,    0,    0,    0,    0,   59,    0,    0,
    0,    0,    0,   60,    0,    0,  256,  139,   56,    0,
    0,  107,    0,  100,    0,   59,    0,   62,    0,    0,
    0,   60,    0,    0,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,   62,    0,   24,   25,   26,
   27,    0,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,    0,    0,   24,   25,   26,   27,    0,
    0,    0,   49,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    0,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    0,    0,    0,
  133,  134,  135,  136,  137,    0,   50,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   49,    0,
   24,   25,   26,   27,    0,    0,    0,    0,    0,   79,
   80,   81,   82,   83,   84,   85,   86,    0,   87,   88,
   89,   90,   91,   92,   93,    0,  133,  134,  135,  136,
  137,    0,   50,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,   62,    0,   24,   25,   26,   27,
    0,    0,    0,    0,    0,    0,    0,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,    0,    0,   24,   25,   26,   27,
  };
  protected static readonly short [] yyCheck = {           121,
    0,   53,  161,  225,  125,   40,   71,   40,  130,  131,
  184,   52,   53,   44,   68,   44,   27,   38,    0,   40,
   41,   42,   40,   44,   63,  352,  125,   38,   59,   29,
   59,  125,   32,   33,   34,   35,  125,  273,   59,   58,
  141,  126,  123,  125,   53,  155,  125,   29,  163,   44,
   44,  270,   42,   44,  295,  228,   40,   73,   44,   58,
  257,   40,   67,  115,   38,  130,   40,   67,   42,  257,
   91,   71,  113,   59,  115,   44,   58,  123,   44,  446,
   38,   40,   40,   41,   42,   67,   44,  270,   59,  126,
  131,  257,   38,  126,   40,   44,   42,   67,  268,  269,
   38,   59,   40,  155,   42,  126,  228,   91,  127,   58,
  161,  285,  189,  190,   35,   44,   60,   91,   62,  285,
  242,   59,  131,   40,  123,  176,  177,  178,  123,  148,
  130,  125,   91,   91,  125,  154,  152,  137,  143,  285,
   61,  192,  193,  143,   52,  127,  128,  126,  270,  190,
  251,   59,  123,  520,  205,  137,  125,   40,  257,  125,
   41,  143,   61,   44,  405,  406,  257,  126,  126,  284,
   44,   92,  257,  143,   91,  257,  257,   58,   59,  415,
  126,  246,  509,  356,   41,  523,  308,  126,  126,  240,
  255,  313,  530,  352,  285,  305,  125,  238,   93,  320,
  321,  242,  123,   41,  285,  113,  291,  329,   91,  286,
  264,  288,   93,   33,  291,  292,  293,  269,   38,   93,
   40,   40,   42,   43,  257,   45,  320,  321,  244,  238,
  271,  320,  321,  272,  356,  270,  257,  314,  360,   59,
  362,  320,  321,  365,  125,  286,  246,  288,  285,  260,
  261,  262,  270,  305,  291,  255,   58,  276,  291,  290,
   58,  290,  303,  304,   44,  298,  264,  265,  390,  269,
  291,  393,  349,  446,  307,  308,   58,  277,  257,   59,
  331,  332,  333,  334,  335,  336,  337,  338,  339,  340,
  341,  342,  343,  344,  345,  346,  347,  348,  257,  257,
  474,  352,  476,  123,  290,  125,  126,  480,  298,  531,
  532,  257,  291,  295,  123,  488,  367,  307,  308,  257,
  285,  362,  266,  267,  446,  484,  500,  392,  502,   61,
   41,   33,  291,  291,  508,  457,   38,  388,   40,   44,
   42,   43,  516,   45,  518,  291,  285,  520,   40,  190,
  509,   58,  291,  291,   46,  336,  337,   59,  480,  410,
   40,  413,   51,  271,  285,  273,  488,   41,   61,  123,
   44,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  392,   44,  315,  316,  317,  318,  520,   91,
  477,  478,  479,   38,  257,   40,   41,   42,   59,  290,
   46,  332,   40,  413,   38,   44,   40,   41,   42,  470,
   40,  123,   33,  125,  126,   61,   40,   38,  117,   40,
   59,   42,   43,  484,   45,   38,  125,  257,  258,  259,
  260,   37,  262,  263,  298,  286,   42,  288,   59,   41,
   59,   47,   44,  307,  308,   91,   91,   41,  509,   59,
   44,   41,  303,  304,   44,  285,  155,   91,  338,  339,
  340,  341,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  313,  314,  315,  316,  317,  318,   94,
  320,  321,  322,  257,  324,  325,  326,  327,  328,  329,
  330,  331,  123,   41,  125,  126,   44,   41,   41,   41,
   44,   44,   44,   41,   41,  124,   44,   44,   33,   41,
  271,  285,   44,   38,   41,   40,  257,   42,   43,   93,
   45,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,  257,   59,  257,  258,  259,  260,   40,
  262,  263,  251,   61,  285,  286,  287,  288,  325,  261,
  262,  263,   41,   41,   41,   44,   44,   44,   41,   59,
  269,   44,   43,  285,   45,  286,   41,  288,  334,  335,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,  318,  257,  123,  257,
  322,  126,  324,  325,  326,  327,  328,  329,  330,  331,
  257,   33,  342,  343,  303,  304,   38,   93,   40,   61,
   42,   43,   41,   45,   41,   59,  257,  258,  259,  260,
   41,  262,  263,   41,   41,   93,   38,   59,   40,   41,
   42,   41,   44,   33,   41,   40,   93,   93,   38,   93,
   40,  123,   42,   43,  285,   45,   59,   59,   41,  123,
   93,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  313,  314,  315,  316,  317,  318,   59,   91,
  323,  322,   58,  324,  325,  326,  327,  328,  329,  330,
  331,  123,   59,   93,  126,   58,   41,   41,   41,  480,
  313,  346,  344,  117,  382,  345,   78,  277,  357,  347,
  143,  348,  512,   29,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,  126,  262,  263,   35,
   -1,   -1,   -1,   -1,   40,   41,   -1,   -1,   44,   -1,
   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
  285,   42,   43,   59,   45,   61,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,  316,  317,  318,   -1,   91,   92,  322,   -1,  324,
  325,  326,  327,  328,  329,  330,  331,   35,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,   -1,
   -1,   59,   -1,   61,   -1,  257,  258,  259,  260,   -1,
  262,  263,  123,   -1,   38,  126,   40,   41,   42,   -1,
   44,   -1,   -1,   35,   -1,  257,   -1,   -1,   -1,   41,
   -1,   -1,   44,   -1,   92,   59,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   59,   -1,   61,
   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   40,  291,
   42,  313,  314,   -1,   -1,  123,   -1,   91,   -1,   -1,
  322,   -1,  324,  325,  326,  327,  328,  329,  330,  331,
   92,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   -1,   35,
   -1,   -1,  126,   -1,   -1,   41,   -1,   33,   44,   91,
   -1,  123,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   46,   -1,   -1,   59,   -1,   61,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   92,   -1,   -1,  285,
   -1,   -1,   -1,   -1,  290,   91,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  123,   -1,  315,
  316,  317,  318,   -1,   -1,   -1,   -1,  123,   -1,  125,
  126,   -1,  313,  314,   -1,   -1,  332,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   41,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,
   -1,   -1,  290,  257,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,   -1,   -1,  315,  316,  317,
  318,   -1,   -1,   -1,   -1,   -1,   -1,  291,   -1,   -1,
   -1,   -1,   -1,  285,  332,  257,   -1,   -1,  290,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  126,   -1,  315,  316,  317,  318,   -1,   -1,  291,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  332,   35,   -1,   -1,   -1,   -1,   -1,   41,   -1,   -1,
   44,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   -1,   61,   -1,  285,
   -1,   -1,   -1,   -1,  290,   -1,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,   -1,   92,  315,
  316,  317,  318,   -1,   -1,   -1,   -1,  313,  314,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  332,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,  123,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   59,   42,   43,
   -1,   45,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,  316,  317,  318,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,
   -1,   38,  126,   40,   -1,   42,   43,   33,   45,   -1,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   46,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  285,   -1,   -1,   -1,   91,  290,   -1,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  126,
   93,  315,  316,  317,  318,   -1,   -1,  123,   -1,  125,
  126,   -1,   33,   -1,   -1,   -1,   -1,   38,  332,   40,
   -1,   42,   43,   -1,   45,  257,  258,  259,  260,   33,
  262,  263,   -1,  126,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,  318,   -1,   -1,   93,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  313,
  314,   -1,   33,   -1,   -1,  126,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   46,   33,   -1,   -1,   -1,
   -1,   38,  126,   40,   41,   42,   43,   -1,   45,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,
   91,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  313,  314,  315,  316,
  317,  318,  123,   -1,   -1,  126,   -1,  313,  314,   -1,
   -1,  294,   -1,   -1,   -1,  298,   -1,   -1,   -1,  126,
   -1,   -1,   -1,   -1,  307,  308,   -1,   -1,   33,   -1,
  313,  314,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   33,  262,  263,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,  257,  258,  259,  260,   33,  262,  263,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   -1,   40,   41,   -1,   -1,   44,   -1,  298,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  307,  308,   -1,   -1,
  294,   -1,  313,  314,  298,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,  307,  308,   -1,   -1,   -1,   -1,  313,
  314,  126,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   91,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,  126,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   33,  262,  263,   -1,   -1,   38,
  126,   40,   -1,   42,   43,   -1,   45,  126,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   33,   42,   43,   -1,
   45,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   33,  313,  314,   59,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  313,  314,   -1,   -1,
   -1,   -1,   33,   -1,   93,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   41,   42,   43,   -1,   45,  126,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,  126,   -1,   -1,   -1,   -1,  123,   -1,   -1,  126,
   -1,   -1,   93,   -1,  257,  258,  259,  260,   33,  262,
  263,  123,   -1,   38,  126,   40,   41,   42,   43,   -1,
   45,  257,  258,  259,  260,   -1,  262,  263,  257,   -1,
   -1,   -1,   -1,   -1,   -1,  126,   33,   -1,  313,  314,
   -1,   38,   -1,   40,   33,   42,   43,   -1,   45,   38,
   -1,   40,   -1,   42,   43,   -1,   45,  126,   -1,   -1,
  313,  314,  291,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   40,   41,  313,  314,   44,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   91,  262,  263,  126,
  257,  258,  259,  260,   -1,  262,  263,  126,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   38,   -1,   40,   41,   42,   -1,   44,
   -1,  126,  192,  193,  313,  314,  257,  258,  259,  260,
   -1,  262,  263,  126,   59,  205,   -1,   -1,  313,  314,
   -1,   -1,   -1,   -1,   -1,   -1,  313,  314,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,  313,  314,   -1,   -1,   -1,   91,   -1,   -1,   -1,
  240,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  313,  314,   -1,   38,   -1,   40,   41,   42,
   -1,   44,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,  126,   -1,   -1,  313,  314,   59,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   91,   -1,
   -1,   38,   -1,   40,   41,   42,   -1,   -1,  313,  314,
   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  331,  332,  333,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,  126,   -1,   -1,  313,  314,   -1,   -1,
   -1,   -1,   -1,   -1,  313,  314,  291,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   91,   -1,   -1,  367,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  388,   -1,
  313,  314,   -1,   -1,   -1,   -1,   -1,   59,   -1,  126,
   -1,   -1,  257,   -1,   -1,   38,   -1,   -1,   -1,   42,
  410,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,  126,   -1,   -1,   -1,   -1,   -1,
  470,   -1,   -1,   -1,  257,   -1,   -1,   -1,   38,   -1,
   40,   41,   42,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  257,   91,  315,  316,  317,  318,   41,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,   41,   -1,  315,  316,
  317,  318,   -1,   -1,   -1,  257,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,   -1,
   -1,   -1,   -1,   -1,   41,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  285,  257,   -1,   -1,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,   41,  285,  315,  316,  317,  318,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
   -1,   -1,  315,  316,  317,  318,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   59,   -1,
   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,   -1,  285,  315,  316,  317,  318,
  319,   -1,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,   -1,   -1,  315,  316,  317,  318,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  285,   -1,
  315,  316,  317,  318,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,   -1,   -1,  315,  316,
  317,  318,   -1,   -1,   -1,  285,  257,   -1,   -1,   -1,
   -1,   -1,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,   -1,  285,  315,  316,  317,  318,   -1,
   -1,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   -1,   -1,  315,  316,  317,  318,   37,   38,
   -1,   40,   -1,   42,   43,   44,   45,   46,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   60,   61,   62,   63,   37,   38,   -1,   -1,   41,
   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   61,
   62,   63,   91,   37,   38,   94,   -1,   41,   42,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,
   -1,   93,   94,   -1,   -1,  124,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   37,   38,   -1,   -1,   41,   42,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,  124,  125,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   38,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
  124,  125,   58,   59,   60,   -1,   62,   63,   -1,   93,
   94,   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,
  124,  125,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,  261,  262,  263,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,  273,  274,  275,  276,  277,  278,
  279,  280,  281,  282,  283,  284,   -1,   -1,   -1,   -1,
   -1,  290,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   -1,   -1,   -1,   -1,   -1,  290,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,
   -1,   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  290,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   93,   94,   -1,   38,
   -1,   -1,   41,   -1,  290,   44,   -1,   -1,   -1,   38,
   -1,   -1,   41,  290,   -1,   44,   -1,   -1,   -1,   58,
   59,   60,   -1,   62,   63,   -1,   -1,  124,  125,   58,
   59,   60,   -1,   62,   63,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   93,   94,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   93,   94,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   93,   94,   -1,   38,  124,  125,   41,   -1,   -1,
   44,   93,   94,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   -1,   -1,  124,  125,   58,   59,   60,   -1,   62,   63,
   -1,   38,  124,  125,   41,   -1,   -1,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   58,   59,   60,   -1,   62,   63,   -1,   38,   93,
   94,   41,   -1,   -1,   44,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   58,   59,
  124,  125,   -1,   63,   -1,   -1,   93,   94,   -1,   -1,
  124,  125,   -1,  290,   41,   -1,   -1,   44,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   58,   59,   93,   94,   -1,   63,  124,  125,   58,
   59,   60,   -1,   62,   63,  264,  265,  266,  267,  268,
  269,   41,  271,  272,   44,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,  124,  125,   93,   -1,   58,   59,
   -1,  290,   -1,   63,   93,   94,   -1,   -1,   -1,   -1,
   -1,  290,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,  266,  267,  268,  269,  125,  271,
  272,   -1,   -1,   93,   38,  124,  125,   41,  290,   -1,
   44,   -1,   -1,   38,   -1,   -1,   41,   -1,  290,   44,
   -1,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,   63,
   -1,   -1,   -1,   58,   59,  125,   -1,   -1,   63,   -1,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   93,
   94,   -1,   -1,   -1,   -1,   -1,  290,   -1,   93,   94,
   -1,   -1,   38,   -1,   -1,   41,  290,   -1,   44,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,
  124,  125,   58,   59,   -1,   -1,   -1,   63,   -1,  124,
  125,   38,   41,  290,   41,   44,   -1,   44,  268,  269,
   -1,  271,  272,   -1,   41,   -1,   -1,   44,   -1,   58,
   59,   58,   59,   -1,   63,   -1,   63,   93,   94,   -1,
  290,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   41,  271,  272,   44,  266,  267,  268,
  269,   -1,  271,  272,   93,   94,   93,   94,  124,  125,
   58,   59,   -1,  290,   -1,   63,   93,   94,   -1,   -1,
   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  271,  272,   -1,   -1,  124,  125,  124,  125,   41,
   -1,   -1,   44,   -1,   -1,   93,   41,  124,  125,   44,
  290,   -1,   41,   -1,   -1,   44,   58,   59,   -1,   -1,
   -1,   63,   -1,   58,   59,   -1,   -1,   -1,   63,   58,
   59,   -1,   -1,   -1,   63,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,  268,  269,   -1,  271,  272,   -1,
   -1,   93,   -1,  268,  269,   -1,  271,  272,   93,   -1,
   -1,   -1,   -1,   -1,   93,   -1,  290,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,
  125,   -1,   -1,   -1,   -1,  123,  125,   -1,   -1,   37,
   38,   -1,   40,   -1,   42,   43,   44,   45,   46,   47,
   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   91,  271,  272,   94,   -1,   -1,   -1,
   33,  290,   -1,  290,   37,   38,   -1,   40,   -1,   42,
   43,   -1,   45,  290,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  271,  272,   -1,  124,   60,   61,   62,
  125,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   91,   -1,
   -1,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,   -1,  272,   -1,   -1,
   -1,   -1,   -1,  272,   -1,   -1,  125,  126,  290,   -1,
   -1,  124,   -1,  126,   -1,  290,   -1,  285,   -1,   -1,
   -1,  290,   -1,   -1,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  285,   -1,  315,  316,  317,
  318,   -1,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,   -1,   -1,  315,  316,  317,  318,   -1,
   -1,   -1,  257,  261,  262,  263,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,   -1,   -1,   -1,
  285,  286,  287,  288,  289,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  257,   -1,
  315,  316,  317,  318,   -1,   -1,   -1,   -1,   -1,  262,
  263,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
  273,  274,  275,  276,  277,   -1,  285,  286,  287,  288,
  289,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  285,   -1,  315,  316,  317,  318,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   -1,   -1,  315,  316,  317,  318,
  };

#line 1262 "CParser.jay"

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
