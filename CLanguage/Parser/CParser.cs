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
//t    "primary_expression : IDENTIFIER COLONCOLON IDENTIFIER",
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
#line 54 "CParser.jay"
  { yyVal = new ScopeResolutionExpression((yyVals[-2+yyTop]).ToString(), (yyVals[0+yyTop]).ToString()); }
  break;
case 8:
#line 61 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 9:
#line 65 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 10:
#line 69 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 11:
#line 73 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 12:
#line 77 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 13:
#line 81 "CParser.jay"
  {
		yyVal = new MemberFromPointerExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 14:
#line 85 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 89 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 16:
#line 93 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 17:
#line 97 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list ',' '}'");
	}
  break;
case 18:
  case_18();
  break;
case 19:
  case_19();
  break;
case 20:
#line 119 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 21:
#line 123 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 127 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 131 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 135 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 139 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 143 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 27:
#line 147 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 28:
#line 151 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 29:
#line 152 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 30:
#line 153 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 31:
#line 154 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 32:
#line 161 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 33:
#line 165 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 34:
#line 172 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 35:
#line 176 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 180 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 37:
#line 184 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 192 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 40:
#line 196 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 204 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 43:
#line 208 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 216 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 220 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 224 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 48:
#line 228 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 236 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 51:
#line 240 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 53:
#line 248 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 55:
#line 256 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 57:
#line 264 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 59:
#line 272 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 280 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 62:
#line 287 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 63:
#line 291 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 65:
  case_65();
  break;
case 66:
#line 316 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 67:
#line 317 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 68:
#line 318 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 69:
#line 319 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 70:
#line 320 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 71:
#line 321 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 72:
#line 322 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 73:
#line 323 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 74:
#line 324 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 75:
#line 325 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 76:
#line 326 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 77:
#line 327 "CParser.jay"
  { yyVal = LogicOp.And; }
  break;
case 78:
#line 328 "CParser.jay"
  { yyVal = LogicOp.Or; }
  break;
case 79:
#line 335 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 80:
#line 339 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 82:
#line 350 "CParser.jay"
  {
		yyVal = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-1+yyTop], null);
	}
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
  case_93();
  break;
case 94:
#line 429 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[0+yyTop], null);
	}
  break;
case 95:
#line 433 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[-2+yyTop], (Initializer)yyVals[0+yyTop]);
	}
  break;
case 96:
#line 437 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 97:
#line 438 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 98:
#line 439 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 99:
#line 440 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 100:
#line 441 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 101:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 102:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 103:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 104:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 105:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 106:
#line 450 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 107:
#line 451 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 108:
#line 452 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 109:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 110:
#line 454 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 111:
#line 455 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 112:
#line 456 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 115:
#line 459 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 118:
#line 468 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 119:
  case_119();
  break;
case 120:
#line 475 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 121:
#line 476 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 122:
#line 483 "CParser.jay"
  {
		yyVal = new List<BaseSpecifier> { (BaseSpecifier)yyVals[0+yyTop] };
	}
  break;
case 123:
  case_123();
  break;
case 124:
#line 492 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString()); }
  break;
case 125:
#line 493 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Public); }
  break;
case 126:
#line 494 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Private); }
  break;
case 127:
#line 495 "CParser.jay"
  { yyVal = new BaseSpecifier((yyVals[0+yyTop]).ToString(), DeclarationsVisibility.Protected); }
  break;
case 128:
#line 499 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 129:
#line 500 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 130:
#line 501 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
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
  case_134();
  break;
case 135:
#line 530 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 136:
#line 531 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 137:
#line 532 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 138:
#line 533 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 139:
#line 534 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 140:
  case_140();
  break;
case 141:
  case_141();
  break;
case 142:
#line 556 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 143:
#line 560 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 144:
#line 564 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 145:
#line 571 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 146:
#line 575 "CParser.jay"
  {
		yyVal = new ReferenceDeclarator((Declarator)yyVals[0+yyTop]);
	}
  break;
case 147:
#line 579 "CParser.jay"
  {
		yyVal = new ReferenceDeclarator((Declarator)yyVals[0+yyTop], (TypeQualifiers)yyVals[-1+yyTop]);
	}
  break;
case 148:
#line 580 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 149:
#line 584 "CParser.jay"
  { yyVal = "+"; }
  break;
case 150:
#line 585 "CParser.jay"
  { yyVal = "-"; }
  break;
case 151:
#line 586 "CParser.jay"
  { yyVal = "*"; }
  break;
case 152:
#line 587 "CParser.jay"
  { yyVal = "/"; }
  break;
case 153:
#line 588 "CParser.jay"
  { yyVal = "%"; }
  break;
case 154:
#line 589 "CParser.jay"
  { yyVal = "=="; }
  break;
case 155:
#line 590 "CParser.jay"
  { yyVal = "!="; }
  break;
case 156:
#line 591 "CParser.jay"
  { yyVal = "<"; }
  break;
case 157:
#line 592 "CParser.jay"
  { yyVal = ">"; }
  break;
case 158:
#line 593 "CParser.jay"
  { yyVal = "<="; }
  break;
case 159:
#line 594 "CParser.jay"
  { yyVal = ">="; }
  break;
case 160:
#line 595 "CParser.jay"
  { yyVal = "<<"; }
  break;
case 161:
#line 596 "CParser.jay"
  { yyVal = ">>"; }
  break;
case 162:
#line 597 "CParser.jay"
  { yyVal = "&"; }
  break;
case 163:
#line 598 "CParser.jay"
  { yyVal = "|"; }
  break;
case 164:
#line 599 "CParser.jay"
  { yyVal = "^"; }
  break;
case 165:
#line 600 "CParser.jay"
  { yyVal = "!"; }
  break;
case 166:
#line 601 "CParser.jay"
  { yyVal = "~"; }
  break;
case 167:
#line 602 "CParser.jay"
  { yyVal = "&&"; }
  break;
case 168:
#line 603 "CParser.jay"
  { yyVal = "||"; }
  break;
case 169:
#line 604 "CParser.jay"
  { yyVal = "++"; }
  break;
case 170:
#line 605 "CParser.jay"
  { yyVal = "--"; }
  break;
case 171:
#line 606 "CParser.jay"
  { yyVal = "="; }
  break;
case 172:
#line 607 "CParser.jay"
  { yyVal = "+="; }
  break;
case 173:
#line 608 "CParser.jay"
  { yyVal = "-="; }
  break;
case 174:
#line 609 "CParser.jay"
  { yyVal = "*="; }
  break;
case 175:
#line 610 "CParser.jay"
  { yyVal = "/="; }
  break;
case 176:
#line 611 "CParser.jay"
  { yyVal = "%="; }
  break;
case 177:
#line 612 "CParser.jay"
  { yyVal = "()"; }
  break;
case 178:
#line 613 "CParser.jay"
  { yyVal = "[]"; }
  break;
case 179:
#line 620 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 180:
#line 624 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("~" + (yyVals[-1+yyTop]).ToString());
	}
  break;
case 181:
#line 628 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("operator" + (string)yyVals[0+yyTop]);
	}
  break;
case 182:
#line 630 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 183:
#line 632 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[-1+yyTop]).ToString()); }
  break;
case 184:
#line 634 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("operator" + (string)yyVals[0+yyTop]); }
  break;
case 186:
  case_186();
  break;
case 187:
#line 654 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 188:
#line 658 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 189:
#line 662 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 190:
#line 666 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 191:
#line 670 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 192:
#line 674 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 193:
#line 678 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 194:
#line 682 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 195:
#line 686 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 196:
  case_196();
  break;
case 197:
#line 698 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 198:
#line 702 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 199:
#line 703 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 200:
#line 704 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 201:
#line 705 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 202:
#line 709 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 203:
#line 713 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 204:
#line 717 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 205:
#line 718 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 206:
#line 719 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 207:
#line 726 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 208:
  case_208();
  break;
case 209:
  case_209();
  break;
case 210:
  case_210();
  break;
case 211:
#line 754 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 212:
#line 758 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 213:
#line 762 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 214:
#line 766 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 215:
#line 773 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 216:
#line 777 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 217:
#line 784 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 218:
#line 788 "CParser.jay"
  {
		yyVal = new ReferenceDeclarator(null);
	}
  break;
case 220:
#line 793 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 221:
  case_221();
  break;
case 222:
#line 812 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 223:
#line 816 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 224:
#line 820 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 225:
#line 824 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 226:
#line 828 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 227:
#line 832 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 228:
#line 836 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 229:
#line 840 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 230:
#line 844 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 231:
#line 848 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 232:
#line 855 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 233:
#line 859 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 234:
#line 863 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
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
case 239:
#line 903 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 251:
#line 933 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 252:
#line 937 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 253:
#line 941 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 254:
#line 942 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 257:
#line 954 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 258:
#line 958 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 259:
#line 962 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 260:
#line 963 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 265:
  case_265();
  break;
case 266:
#line 983 "CParser.jay"
  {
		yyVal = new VirtualDeclarationStatement((Statement)yyVals[0+yyTop]) { IsVirtual = true };
	}
  break;
case 267:
  case_267();
  break;
case 268:
  case_268();
  break;
case 269:
#line 997 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 270:
  case_270();
  break;
case 271:
#line 1011 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 272:
#line 1012 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 273:
#line 1013 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 274:
#line 1020 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 275:
#line 1024 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 276:
#line 1028 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()), parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 277:
#line 1032 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: new IdentifierDeclarator((yyVals[-2+yyTop]).ToString()), parameters: new List<ParameterDeclaration>());
	}
  break;
case 278:
#line 1036 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: new IdentifierDeclarator("~" + (yyVals[-2+yyTop]).ToString()), parameters: new List<ParameterDeclaration>());
	}
  break;
case 279:
  case_279();
  break;
case 280:
#line 1054 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 281:
#line 1058 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 282:
#line 1065 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 283:
#line 1069 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 284:
#line 1073 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-4+yyTop], (List<SwitchCase>)yyVals[-1+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 285:
#line 1077 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-3+yyTop], new List<SwitchCase>(), GetLocation(yyVals[-5+yyTop]));
	}
  break;
case 286:
#line 1084 "CParser.jay"
  {
		yyVal = new List<SwitchCase> { (SwitchCase)yyVals[0+yyTop] };
	}
  break;
case 287:
  case_287();
  break;
case 288:
#line 1096 "CParser.jay"
  {
		yyVal = new SwitchCase((Expression)yyVals[-2+yyTop], (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 289:
#line 1100 "CParser.jay"
  {
		yyVal = new SwitchCase(null, (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 290:
#line 1107 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 291:
#line 1111 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 292:
#line 1115 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], (yyVals[-2+yyTop] as ExpressionStatement)?.Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 293:
#line 1119 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], (yyVals[-3+yyTop] as ExpressionStatement)?.Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 294:
#line 1123 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], (yyVals[-2+yyTop] as ExpressionStatement)?.Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 295:
#line 1127 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], (yyVals[-3+yyTop] as ExpressionStatement)?.Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 297:
#line 1135 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 298:
#line 1139 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 299:
#line 1143 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 300:
#line 1147 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 301:
  case_301();
  break;
case 302:
  case_302();
  break;
case 307:
  case_307();
  break;
case 308:
  case_308();
  break;
case 309:
#line 1195 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 310:
#line 1199 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-2+yyTop]).ToString()).Push((yyVals[0+yyTop]).ToString());
	}
  break;
case 311:
#line 1203 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("~" + (yyVals[0+yyTop]).ToString());
	}
  break;
case 312:
#line 1204 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 313:
#line 1205 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
  break;
case 314:
#line 1207 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("operator" + (string)yyVals[0+yyTop]); }
  break;
case 315:
#line 1209 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("operator" + (string)yyVals[0+yyTop]); }
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
case 320:
  case_320();
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
void case_18()
#line 102 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_19()
#line 108 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_65()
#line 297 "CParser.jay"
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

void case_83()
#line 352 "CParser.jay"
{
		DeclarationSpecifiers ds = (DeclarationSpecifiers)yyVals[-2+yyTop];
		List<InitDeclarator> decls = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_84()
#line 361 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_85()
#line 367 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];		
		yyVal = ds;
	}

void case_86()
#line 373 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_87()
#line 379 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_88()
#line 385 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_89()
#line 391 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_90()
#line 397 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_91()
#line 403 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_92()
#line 412 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_93()
#line 418 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_119()
#line 470 "CParser.jay"
{
		var ts = new TypeSpecifier((TypeSpecifierKind)yyVals[-4+yyTop], (yyVals[-3+yyTop]).ToString(), (Block)yyVals[0+yyTop]);
		ts.BaseSpecifiers = (List<BaseSpecifier>)yyVals[-1+yyTop];
		yyVal = ts;
	}

void case_123()
#line 485 "CParser.jay"
{
		((List<BaseSpecifier>)yyVals[-2+yyTop]).Add((BaseSpecifier)yyVals[0+yyTop]);
		yyVal = yyVals[-2+yyTop];
	}

void case_131()
#line 506 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_132()
#line 511 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_133()
#line 517 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_134()
#line 522 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_140()
#line 539 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_141()
#line 545 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_186()
#line 640 "CParser.jay"
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

void case_196()
#line 688 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<Expression>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(ctorArgumentValue: n));
		}
		yyVal = d;
	}

void case_208()
#line 728 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_209()
#line 737 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_210()
#line 743 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_221()
#line 798 "CParser.jay"
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

void case_235()
#line 868 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_236()
#line 875 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_237()
#line 883 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_238()
#line 890 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_265()
#line 972 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			fdecl,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_267()
#line 985 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsOverride = true };
	}

void case_268()
#line 990 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsOverride = true };
	}

void case_270()
#line 1002 "CParser.jay"
{
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator((Declarator)yyVals[-3+yyTop], null) };
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-4+yyTop], decls);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsPureVirtual = true };
	}

void case_279()
#line 1041 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		DeclarationSpecifiers ds = new DeclarationSpecifiers();
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator(fdecl, null) };
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_287()
#line 1086 "CParser.jay"
{
		((List<SwitchCase>)yyVals[-1+yyTop]).Add((SwitchCase)yyVals[0+yyTop]);
		yyVal = yyVals[-1+yyTop];
	}

void case_301()
#line 1152 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_302()
#line 1157 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_307()
#line 1172 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_308()
#line 1181 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_316()
#line 1214 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_317()
#line 1223 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_318()
#line 1235 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_319()
#line 1241 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_320()
#line 1247 "CParser.jay"
{
		var l = (List<Declaration>)yyVals[-1+yyTop];
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

#line default
   static readonly short [] yyLhs  = {              -1,
    1,    1,    1,    1,    1,    1,    1,    3,    3,    3,
    3,    3,    3,    3,    3,    3,    3,    4,    4,    8,
    8,    8,    8,    8,    8,    8,    8,   10,   10,   10,
   10,    9,    9,   11,   11,   11,   11,   12,   12,   12,
   13,   13,   13,   14,   14,   14,   14,   14,   15,   15,
   15,   16,   16,   17,   17,   18,   18,   19,   19,   20,
   20,   21,   21,    7,    7,   22,   22,   22,   22,   22,
   22,   22,   22,   22,   22,   22,   22,   22,    2,    2,
   23,   24,   24,   25,   25,   25,   25,   25,   25,   25,
   25,   26,   26,   31,   31,   27,   27,   27,   27,   27,
   28,   28,   28,   28,   28,   28,   28,   28,   28,   28,
   28,   28,   28,   28,   28,   36,   36,   34,   34,   34,
   34,   39,   39,   40,   40,   40,   40,   37,   37,   37,
   41,   41,   41,   41,   35,   35,   35,   35,   35,   42,
   42,   43,   43,   30,   32,   32,   32,   32,   47,   47,
   47,   47,   47,   47,   47,   47,   47,   47,   47,   47,
   47,   47,   47,   47,   47,   47,   47,   47,   47,   47,
   47,   47,   47,   47,   47,   47,   47,   47,   48,   48,
   48,   48,   48,   48,   45,   45,   45,   45,   45,   45,
   45,   45,   45,   45,   45,   45,   45,   44,   44,   44,
   44,   46,   46,   29,   29,   29,   49,   49,   50,   50,
   51,   51,   51,   51,    5,    5,   52,   52,   52,   52,
   53,   53,   53,   53,   53,   53,   53,   53,   53,   53,
   53,   33,   33,   33,    6,    6,    6,    6,   54,   55,
   55,   56,   56,   57,   57,   57,   57,   57,   57,   58,
   59,   59,   64,   64,   65,   65,   38,   38,   66,   66,
   67,   67,   67,   67,   67,   67,   67,   67,   67,   72,
   68,   68,   68,   71,   71,   71,   71,   71,   69,   60,
   60,   61,   61,   61,   61,   73,   73,   74,   74,   62,
   62,   62,   62,   62,   62,   63,   63,   63,   63,   63,
    0,    0,   75,   75,   75,   75,   70,   70,   78,   78,
   78,   78,   78,   78,   78,   76,   76,   77,   77,   77,
   79,   79,   79,
  };
   static readonly short [] yyLen = {           2,
    1,    1,    1,    1,    1,    3,    3,    1,    4,    3,
    4,    3,    3,    2,    2,    6,    7,    1,    3,    1,
    2,    2,    2,    2,    2,    2,    4,    1,    1,    1,
    1,    1,    4,    1,    3,    3,    3,    1,    3,    3,
    1,    3,    3,    1,    3,    3,    3,    3,    1,    3,
    3,    1,    3,    1,    3,    1,    3,    1,    3,    1,
    3,    1,    5,    1,    3,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    3,
    1,    2,    3,    1,    2,    1,    2,    1,    2,    1,
    2,    1,    3,    1,    3,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    3,    5,    2,
    2,    1,    3,    1,    2,    2,    2,    1,    1,    1,
    2,    1,    2,    1,    4,    5,    5,    6,    2,    1,
    3,    1,    3,    1,    2,    2,    3,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    2,    2,    1,    2,
    2,    3,    4,    4,    1,    3,    5,    4,    4,    6,
    6,    5,    4,    3,    4,    4,    3,    1,    2,    2,
    3,    1,    2,    1,    1,    1,    1,    3,    1,    3,
    2,    4,    2,    1,    1,    2,    1,    1,    1,    2,
    3,    2,    3,    3,    4,    3,    4,    2,    3,    3,
    4,    1,    3,    4,    1,    2,    3,    4,    2,    1,
    2,    3,    2,    1,    1,    1,    1,    1,    1,    3,
    2,    3,    1,    2,    1,    1,    2,    3,    1,    2,
    1,    1,    1,    1,    2,    2,    4,    5,    2,    5,
    2,    2,    2,    4,    3,    4,    3,    4,    2,    1,
    2,    5,    7,    7,    6,    1,    2,    4,    3,    5,
    7,    6,    7,    6,    7,    3,    2,    2,    2,    3,
    1,    2,    1,    1,    1,    1,    4,    3,    1,    3,
    4,    3,    4,    4,    4,    4,    5,    1,    2,    2,
    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,    0,   96,   97,   98,   99,  100,  144,  205,  102,
  103,  104,  105,  108,  109,  106,  107,  204,  206,  101,
  110,  111,  112,  128,  129,  130,    0,  305,    0,  304,
    0,    0,    0,    0,    0,  113,  114,    0,  303,  301,
  306,    0,    0,  116,  117,    0,    0,  302,  179,    0,
    0,    0,    0,    0,   82,    0,   92,    0,    0,    0,
    0,  115,   85,   87,   89,   91,    0,    0,  120,    0,
    0,  310,    0,    0,    0,    0,  140,    0,  169,  170,
  160,  161,  158,  159,  154,  155,  167,  168,  174,  175,
  176,  172,  173,    0,    0,  162,  151,  149,  150,  166,
  165,  152,  153,  156,  157,  164,  163,  171,  181,    0,
  202,    0,    0,  200,    0,  180,    0,   83,  321,    0,
    0,  322,  323,  318,    0,  308,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  257,    0,  261,
    0,    0,    0,  259,  262,  263,  264,    0,    0,  118,
  312,    0,    0,    0,    0,    0,    0,  209,  314,  311,
    0,  135,    0,    0,  177,  178,  186,  203,    0,  201,
   93,    0,    0,    2,    3,    0,    0,    0,    4,    5,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  251,    0,    0,   28,   29,   30,   31,  280,    8,    0,
    0,   79,    0,   34,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   64,  255,  256,  244,  245,
  246,  247,  248,  249,    0,  253,    0,    0,  232,   95,
  320,  307,  319,  197,    0,   18,    0,    0,  194,    0,
    0,    0,  182,    0,    0,    0,  271,  272,  273,  266,
    0,  269,    0,    0,    0,  258,  260,  279,  265,    0,
    0,    0,  124,    0,  122,  315,  313,  316,    0,    0,
    0,    0,    0,  213,    0,    0,    0,   32,   81,  143,
  137,  141,  136,    0,    0,    0,    0,   26,    0,   21,
   22,    0,    0,    0,    0,    0,    0,  297,  298,  299,
    0,    0,    0,    0,    0,    0,   23,   24,    0,  281,
    0,   14,   15,    0,    0,    0,   67,   68,   69,   70,
   71,   72,   73,   74,   75,   76,   77,   78,   66,    0,
   25,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  252,  254,    0,    0,    0,  235,    0,    0,  240,  196,
    0,  195,    0,  193,  189,    0,  188,    0,    0,  184,
  183,  277,    0,    0,    0,    0,    0,  275,    0,  125,
  126,  127,    0,  119,  228,    0,    0,  222,    0,    0,
    0,    0,    0,    0,  317,  208,  210,  138,    7,  250,
    0,    0,    0,    0,    0,    0,    0,    0,  296,  300,
    6,    0,  131,  133,    0,  218,    0,  216,   80,   13,
   10,    0,    0,   12,   65,   35,   36,   37,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  243,  233,    0,  236,  239,
  241,   19,    0,    0,  192,  187,  276,    0,    0,  278,
  267,  274,  123,  229,  221,  226,  223,  212,  230,    0,
  224,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   33,   11,    9,    0,  242,  234,  237,    0,
  190,  191,  268,    0,  231,  227,  225,    0,    0,  290,
    0,    0,    0,    0,    0,    0,   63,  238,  270,    0,
    0,    0,  285,    0,  286,    0,  294,    0,  292,    0,
   16,    0,  283,    0,    0,  284,  287,  291,  295,  293,
   17,    0,    0,    0,
  };
  protected static readonly short [] yyDgoto  = {            29,
  199,  200,  201,  235,  303,  355,  202,  203,  204,  205,
  206,  207,  208,  209,  210,  211,  212,  213,  214,  215,
  216,  330,  280,  217,  125,   56,   32,   33,   34,   35,
   57,   58,  230,   36,   37,  263,   38,   69,  264,  265,
  306,   76,   77,   59,   60,  113,  109,   61,  386,  157,
  158,  387,  275,  357,  358,  359,  218,  219,  220,  221,
  222,  223,  224,  225,  226,  143,  144,  145,  146,   39,
  148,  252,  514,  515,   40,   41,  127,   42,  128,
  };
  protected static readonly short [] yySindex = {         1086,
 -197,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  -83,    0, 1086,    0,
   83, 3827, 3827, 3827, 3827,    0,    0,   85,    0,    0,
    0,  -29,  110,    0,    0, -172,  -19,    0,    0, 3778,
   88,  -33,  -26,  -64,    0,   30,    0, 1243,  124,   46,
   -1,    0,    0,    0,    0,    0, 3666,  -16,    0,  112,
 2267,    0, 3778, -187,  166,   -5,    0, -172,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  230,  228,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  298,
    0,   46,  -33,    0,  -26,    0,   88,    0,    0,  316,
  722,    0,    0,    0,   83,    0, 2324, 3827,   46, 1153,
  291,  -80,  310,  299,  305,  313, 3827,    0, -203,    0,
   83,  -21, 3771,    0,    0,    0,    0,  -18,  254,    0,
    0, 3778,   93,  257,   75,  346,  356,    0,    0,    0,
 1939,    0,  -68,   11,    0,    0,    0,    0,   46,    0,
    0,  344,  -11,    0,    0, 1979, 2056, 2056,    0,    0,
  379,  395,  408,  617,  416,  174,  398,  402, 1694, 1381,
    0, 1939, 1939,    0,    0,    0,    0,    0,    0,   99,
    5,    0,  251,    0, 1939,  391,   32, -200,   21, -160,
  442,  421,  412,  266,  -49,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  420,    0,  268, 1466,    0,    0,
    0,    0,    0,    0,  278,    0,  521,  104,    0, 1543,
  473,  771,    0, 3778,  314, 2352,    0,    0,    0,    0,
   83,    0,  537,  -38, 2454,    0,    0,    0,    0, -186,
 -186, -186,    0,    9,    0,    0,    0,    0, 2176, 1702,
  -33,  519,  116,    0,   63,  257, 3799,    0,    0,    0,
    0,    0,    0,  -65,  325,  617, 1381,    0, 1381,    0,
    0, 1939, 1939, 1939,  256, 1309,  524,    0,    0,    0,
  102,  303,  545, 3855, 3855,  392,    0,    0, 1939,    0,
  330,    0,    0, 1622, 1939,  331,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 1939,
    0, 1939, 1939, 1939, 1939, 1939, 1939, 1939, 1939, 1939,
 1939, 1939, 1939, 1939, 1939, 1939, 1939, 1939, 1939, 1939,
    0,    0, 1939,  333,   14,    0,  722,    6,    0,    0,
 1939,    0, 1453,    0,    0, 1939,    0, 1732,  498,    0,
    0,    0,  551,  -36,  532,  553,  536,    0,  555,    0,
    0,    0,  254,    0,    0,  556,  559,    0, 1743,  510,
 1939,   63, 2504, 1771,    0,    0,    0,    0,    0,    0,
  565,  566,  324,  352,  396,  595, 1779, 1779,    0,    0,
    0, 1801,    0,    0, 2399,    0,   87,    0,    0,    0,
    0,  426,   36,    0,    0,    0,    0,    0,  391,  391,
   32,   32, -200, -200, -200, -200,   21,   21, -160,  442,
  421,  412,  266,  171,  544,    0,    0,  981,    0,    0,
    0,    0,  546,  560,    0,    0,    0,  590, 1850,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  611,
    0, 1870,  561,  533,  533,  617,  535,  617, 1939, 1878,
 1899, 1466,    0,    0,    0, 1939,    0,    0,    0,  722,
    0,    0,    0,  602,    0,    0,    0,  340, -100,    0,
  428,  617,  432,  617,  434,   15,    0,    0,    0,  617,
 1939,  606,    0,  -13,    0,  607,    0,  617,    0,  617,
    0, 1394,    0,  609,  525,    0,    0,    0,    0,    0,
    0,  525,  525,  525,
  };
  protected static readonly short [] yyRindex = {            0,
    0, 2232,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  131,  896, 1014, 2205,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2080,    0,    0,    0,
    0,    0,  345,    0,    0,    0,    0,  -35,    0,  867,
  826,    0,    0,    0,    0,    0,    0, 2109,    0,    0,
    0,    0,    0,    0,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  916,    0,    0,  663,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  957,    0,
    0,    0, 2296,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  503,    0,  624,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1016,    0,
    0,  -35, 3743,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2858,    0, 2949,    0,    0, 3143, 3221, 3296, 2921, 3587,
 2163, 2732, 1244, 2009,  912,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2786,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  511,  515,  516,    0,  520,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  368,  383,  627,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  -35,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  528,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  628,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 3184, 3211,
 3258, 3269, 3306, 3343, 3456, 3541, 3564, 3580, 3638, 3627,
 3650, 1431, 3403,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2895,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2821,    0,    0,    0,  192,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   56,   62,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -89,    0,  357,  -56,  188, -118,  -43, -149,    0,
 -168,   61,  105,   70,  327,  328,  326,  336,  332,    0,
 -159,    0, -335,   20,    1, -110,    0,  177,  -25,    0,
  558,  -41, -162,    0,    0,   23,    0,  -46,    0,  302,
  122,  608, -131,  -53,  147,   39,  -58,    2,  -67,    0,
  410, -126, -235, -411,    0,  334, -167,    0,  -32, -248,
    0,    0,    0, -177, -220,    0,  547,    0,    0,   12,
    0,    0,    0,  175,  659,    0,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           114,
   31,  279,  229,  156,  352,  117,   51,  117,   94,  110,
   71,  236,  241,  350,  159,   53,  295,  445,  255,   30,
  118,  150,  118,   94,  513,  126,  111,  111,  274,   31,
  254,  282,   63,   64,   65,   66,  490,  392,  163,   46,
  258,  149,  307,  308,  314,  245,  286,  408,   30,   47,
  316,  354,  383,  116,  284,  331,  281,  448,  522,  398,
   68,  170,  237,  337,  338,  356,  450,  141,  142,  142,
   44,  155,   43,  117,  335,  172,  336,  124,  147,  309,
  341,  253,  342,  172,   75,  130,  140,  168,  118,  168,
  308,  115,   54,  266,  232,  315,  353,  160,   45,  301,
  302,  273,  393,   78,  120,  111,   67,  343,  344,  229,
  490,  526,  271,  272,  269,  259,   53,  278,  400,  162,
   52,  268,   51,  369,   53,   52,  415,   51,  485,   53,
  155,   67,  288,  290,  291,  283,  131,  251,  447,  521,
  374,   55,  309,  141,  142,  309,  231,  233,  278,  278,
  142,  390,  282,  394,  147,  269,  250,  310,  480,  481,
  410,  278,  140,   51,  305,  270,  429,  430,   84,  242,
   84,   84,   84,   44,   84,  524,  243,  270,  373,  418,
  289,  392,  426,  427,  428,  370,  288,  379,   75,   84,
  419,   75,  116,  279,  449,  236,  278,  302,  112,  302,
   54,   45,  403,  404,  405,  129,  270,   67,   54,  375,
  244,  425,  111,   54,  309,  273,  168,  384,  308,  511,
  512,   84,  349,   49,  282,  423,  161,  110,  486,  282,
  401,  282,  402,  282,  282,   74,  282,  153,  229,  308,
   70,   54,  452,  395,  453,  111,  155,  454,  132,   54,
  282,  377,  417,  458,   94,  155,   84,   50,  285,  169,
  444,  305,  483,  305,    9,  311,  312,  313,  132,  155,
  165,    9,  468,   18,   19,  473,  363,  155,  305,  305,
   18,   19,  380,  381,  382,  489,  339,  340,  278,  278,
  278,  278,  278,  278,  278,  278,  278,  278,  278,  278,
  278,  278,  278,  278,  278,  278,  511,  512,  498,  278,
  500,  329,  352,  352,  282,  407,  282,  282,  360,  356,
  166,  361,  308,  197,  278,  470,  507,  508,  192,  229,
  190,   49,  240,  194,  517,  195,  519,  168,  167,   49,
  229,   44,  523,  411,   49,  278,  309,  533,  197,  246,
  529,  279,  530,  192,  534,  190,  247,  193,  194,  489,
  195,  417,  248,  229,  476,   50,  304,  309,  278,   45,
  249,  229,   49,   50,  198,  289,  289,  267,   50,  120,
   49,  288,  288,  239,  198,  198,  276,   84,  198,  501,
  503,  505,  477,  155,   72,  309,  151,  431,  432,  277,
   73,    9,  152,  229,  121,  132,   50,  132,  132,  132,
   18,   19,  437,  438,   50,  155,  196,  112,  292,  129,
  134,   84,  134,  134,  134,  413,  414,  334,  278,  416,
  297,  415,  332,   53,  293,  198,  478,  333,  120,  309,
  191,  196,  278,  433,  434,  435,  436,  294,  282,  282,
  282,  282,  197,  282,  282,  296,  298,  192,  132,  190,
  299,  193,  194,  304,  195,  304,  484,  278,  516,  361,
  198,  309,  518,  134,  520,  309,  282,  309,  198,  345,
  304,  304,  270,  282,  282,  282,  282,  282,  282,  282,
  282,  282,  282,  282,  282,  282,  282,  282,  282,  282,
  282,  282,  282,  282,  282,  282,  282,  282,  282,  282,
   44,  282,  282,  282,  346,  282,  282,  282,  282,  282,
  282,  282,  282,  317,  318,  319,  320,  321,  322,  323,
  324,  325,  326,  327,  328,  347,  348,  285,   45,  260,
  261,  262,  120,  214,  351,  196,  214,  227,  174,  175,
  176,  218,  177,  178,  218,  211,  217,  197,  211,  217,
  219,  362,  192,  219,  190,  365,  193,  194,  220,  195,
  371,  220,  173,  174,  175,  176,  376,  177,  178,  391,
  406,  399,  409,  198,  238,  412,  420,  424,    9,  446,
  456,  457,  459,  460,  461,  462,  464,   18,   19,  465,
   62,  198,  467,  179,  180,  474,  475,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,  179,  180,
   24,   25,   26,   27,  479,  198,  487,  181,  491,  182,
  183,  184,  185,  186,  187,  188,  189,  120,  493,  197,
  196,  495,  492,  497,  192,  482,  190,  499,  193,  194,
  509,  195,  510,  525,  207,  528,  532,  215,  217,  506,
  422,  439,  441,  440,  171,  198,  173,  174,  175,  176,
  443,  177,  178,  442,  463,  164,  397,   48,  527,  257,
    0,  451,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  199,  199,   62,    0,  199,    0,    0,    0,
    0,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,  179,  180,   24,   25,   26,   27,    0,  120,
    0,  181,  196,  182,  183,  184,  185,  186,  187,  188,
  189,    0,    0,  199,  197,    0,    0,    0,    0,  192,
    0,  190,    0,  193,  194,    0,  195,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  173,  174,  175,  176,    0,  177,  178,  199,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  197,    0,    0,    0,    0,  192,   62,
  190,    0,  368,  194,    0,  195,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,  179,  180,   24,
   25,   26,   27,    0,  228,    0,  181,  196,  182,  183,
  184,  185,  186,  187,  188,  189,    0,    0,    0,    0,
  185,    0,    0,  367,    0,  185,  185,    0,    0,  185,
    0,    0,    0,  173,  174,  175,  176,    0,  177,  178,
    0,    0,    0,    0,  185,    0,  185,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  196,    0,    0,    0,
    0,  148,    0,    0,    0,    0,    0,  148,    0,    0,
  148,    0,    0,    0,    0,    0,  185,  185,    0,  199,
    0,    0,    0,    0,    0,  148,    0,  148,    0,  179,
  180,    0,    0,   86,    0,   86,   86,   86,  181,   86,
  182,  183,  184,  185,  186,  187,  188,  189,  185,    0,
  146,    0,   62,  199,   86,   62,  146,    0,  148,  146,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   62,
   62,    0,    0,    0,  146,    0,  146,    0,  227,  174,
  175,  176,    0,  177,  178,    0,   86,    0,    0,  148,
    0,  145,    0,    0,    0,    0,    0,  145,    0,    0,
  145,    0,    0,    0,   62,    0,    0,  146,    0,    0,
    0,    0,    0,  197,    0,  145,    0,  145,  192,    0,
  190,   86,  193,  194,    0,  195,  354,  227,  174,  175,
  176,    0,  177,  178,  179,  180,   62,    0,  146,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  145,    0,
  147,   88,    0,   88,   88,   88,  147,   88,    0,  147,
    0,    0,    0,    0,  366,    0,    0,    0,    9,    0,
    0,  353,   88,    0,  147,    0,  147,   18,   19,  145,
    0,    0,    0,  179,  180,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  228,   88,  488,  196,  147,    0,    0,
  185,    0,    0,    0,    0,  185,    0,  185,  185,  185,
  185,  185,  185,  185,  185,  185,  185,  185,  185,  185,
  185,  185,  185,  185,  185,  185,  185,  185,  147,   88,
  185,  185,  185,  185,   28,    0,    0,    0,    0,    0,
    0,  148,   86,    0,    0,    0,  148,  185,  148,  148,
  148,  148,  148,  148,  148,  148,  148,  148,  148,  148,
  148,  148,  148,  148,  148,  148,  148,  148,  148,    0,
    0,  148,  148,  148,  148,  197,   86,    0,    0,    0,
  192,    0,  190,  234,  193,  194,    0,  195,  148,    0,
  146,   62,    0,    0,    0,  146,    0,  146,  146,  146,
  146,  146,  146,  146,  146,  146,  146,  146,  146,  146,
  146,  146,  146,  146,  146,  146,  146,  146,    0,    0,
  146,  146,  146,  146,    0,    0,    0,  227,  174,  175,
  176,  145,  177,  178,    0,    0,  145,  146,  145,  145,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  145,  145,  145,  145,  145,    0,
   88,  145,  145,  145,  145,    0,    0,  122,  196,    0,
    0,    0,    0,    0,   58,    0,    0,   58,  145,    0,
    0,    0,    0,  179,  180,    0,    0,    0,    0,    0,
  147,   58,   58,  121,   88,  147,   58,  147,  147,  147,
  147,  147,  147,  147,  147,  147,  147,  147,  147,  147,
  147,  147,  147,  147,  147,  147,  147,  147,    0,    0,
  147,  147,  147,  147,  123,    0,   58,    0,    0,    0,
    0,  197,    1,    0,    0,    0,  192,  147,  190,    0,
  193,  194,    0,  195,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  120,    0,  198,   58,    0,
    2,    0,    0,    0,    0,    0,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,    0,    0,
   24,   25,   26,   27,    0,    0,    0,    0,    0,  227,
  174,  175,  176,  197,  177,  178,    0,    0,  192,    0,
  190,    0,  193,  194,    0,  195,  197,    0,    0,    0,
    0,  192,    0,  190,  196,  193,  194,   62,  195,  354,
    0,    0,    0,    0,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,  179,  180,   24,   25,   26,
   27,   59,    0,    0,   59,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  353,  197,    0,    0,   59,   59,
  192,    0,  190,   59,  193,  194,    0,  195,  197,    0,
    0,    0,    0,  192,    0,  190,  196,  193,  194,    0,
  195,  354,    0,    0,   58,   58,  228,    0,  531,  196,
    0,    0,    0,   59,    0,    0,    0,   62,    0,    0,
    0,    0,    0,   58,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,   59,  353,   24,   25,   26,
   27,    0,    0,    0,    0,  227,  174,  175,  176,    0,
  177,  178,    0,    0,  119,  197,    0,    0,  196,    0,
  192,    0,  190,    0,  193,  194,    0,  195,  228,    0,
    0,  196,    0,   62,    0,    0,    0,    0,    0,    0,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,  179,  180,   24,   25,   26,   27,    0,    0,    0,
    0,    0,    0,    0,    0,  364,    0,  227,  174,  175,
  176,    0,  177,  178,    0,    0,    0,    0,    0,    0,
  227,  174,  175,  176,  197,  177,  178,    0,    0,  192,
    0,  190,  421,  193,  194,   62,  195,    0,  196,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,  179,  180,   24,   25,   26,   27,    0,
    0,   59,   59,    0,    0,    0,  179,  180,    0,  227,
  174,  175,  176,    0,  177,  178,    0,    0,    0,    0,
   59,    0,  227,  174,  175,  176,  197,  177,  178,    0,
    0,  192,    0,  190,  197,  193,  194,    0,  195,  192,
    0,  190,    0,  389,  194,    0,  195,  196,    0,    0,
    9,    0,  300,    0,    0,    0,    0,    0,    0,   18,
   19,    0,    0,    0,  197,  179,  180,    0,    0,  192,
    0,  190,    0,  193,  194,  197,  195,    0,  179,  180,
  192,    0,  190,    0,  193,  194,    0,  195,    0,    0,
    0,    0,    0,    0,  388,    0,    0,    0,    0,  227,
  174,  175,  176,  197,  177,  178,    0,    0,  192,    0,
  190,  197,  472,  194,    0,  195,  192,    0,  190,  196,
  193,  194,    0,  195,  455,    0,    0,  196,    0,    0,
    0,    0,    0,  197,    0,  466,    0,  198,  192,    0,
  190,    0,  193,  194,    0,  195,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  179,  180,  196,    0,    0,
    0,    0,    0,  471,    0,    0,    0,    0,  196,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  227,  174,
  175,  176,  197,  177,  178,    0,    0,  192,    0,  190,
    0,  193,  194,    0,  195,    0,  196,    0,    0,    0,
    0,    0,  197,    0,  196,    0,    0,  192,    0,  190,
  197,  193,  194,    0,  195,  192,    0,  190,  502,  193,
  194,    0,  195,  482,    0,    0,  196,    0,    0,    0,
    0,  197,    0,    0,  179,  180,  192,    0,  190,  504,
  193,  194,    0,  195,    0,    0,    0,    0,    0,    0,
  227,  174,  175,  176,    0,  177,  178,    0,  227,  174,
  175,  176,  496,  177,  178,    0,    0,    0,    0,    0,
    0,  197,  228,    0,    0,  196,  192,    0,  190,    0,
  193,  194,    0,  195,    0,    0,    0,    0,  227,  174,
  175,  176,    0,  177,  178,  196,    0,    0,    0,  227,
  174,  175,  176,  196,  177,  178,  179,  180,    0,    0,
    0,  197,    0,    0,  179,  180,  192,    0,  287,    0,
  193,  194,    0,  195,  196,    0,    0,  227,  174,  175,
  176,    0,  177,  178,    0,  227,  174,  175,  176,    0,
  177,  178,    0,    0,  179,  180,    0,    0,    0,   60,
    0,    0,   60,    0,    0,  179,  180,  227,  174,  175,
  176,    0,  177,  178,  196,    0,   60,   60,    0,    0,
    0,   60,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  179,  180,    0,    0,    0,  197,    0,
    0,  179,  180,  192,    0,  289,    0,  193,  194,    0,
  195,   60,    0,    0,  196,    0,  227,  494,  175,  176,
    0,  177,  178,  179,  180,    0,    0,  139,    0,  139,
  139,  139,    0,  139,    0,    0,  227,  174,  175,  176,
    0,  177,  178,   60,  227,  174,  175,  176,  139,  177,
  178,    0,    0,    0,    0,    0,  121,    0,  121,  121,
  121,    0,  121,    0,    0,  227,  174,  175,  176,    0,
  177,  178,  179,  180,    0,    0,    0,  121,    0,    0,
  139,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  196,  179,  180,    0,    0,    0,    0,    0,    0,
  179,  180,    0,    0,    0,  227,  174,  175,  176,  121,
  177,  178,    0,   54,    0,  139,   54,    0,    0,    0,
    0,  179,  180,  271,    0,  269,  385,   53,    0,    0,
   54,   54,    0,    0,    0,   54,    0,    0,    0,    0,
    0,    0,    0,    0,  121,  227,  174,  175,  176,    0,
  177,  178,   90,    0,   90,   90,   90,    0,   90,    0,
    0,  179,  180,    0,    0,   54,   54,    0,    0,    0,
    0,    0,    0,   90,    0,    0,  270,    0,    0,  115,
    0,  115,    0,  115,    0,    0,    0,    0,    0,    0,
   60,    0,    0,    0,    0,    0,   54,   54,    0,    0,
  115,  179,  180,    0,    0,   90,    0,    0,   60,    0,
    0,   54,    0,    0,    0,    0,    0,  154,    0,    0,
    0,    0,  227,  174,  175,  176,    0,  177,  178,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   90,    0,    0,  115,    0,    0,  139,  115,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  115,    0,    0,  115,    0,    0,
    0,    0,    0,    0,  139,  121,    0,    0,  179,  180,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  372,  121,  139,  139,  139,  139,    0,  121,
  121,  121,  121,  121,  121,  121,  121,  121,  121,  121,
  121,  121,  121,  121,  121,  121,  121,  121,  121,  121,
  121,  115,    0,  121,  121,  121,  121,    0,    0,    0,
    0,    0,   49,   54,   54,    0,  416,    0,  415,  385,
   53,    0,    0,    0,    0,    0,  120,    0,    0,    0,
    0,    0,   54,    0,    0,    0,    0,    0,    0,    0,
   62,   90,    0,    0,    0,    0,   50,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,  115,  270,
   24,   25,   26,   27,  378,   90,    0,    0,    0,    0,
    0,  309,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  115,    0,    0,    0,
    0,    0,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  469,    0,  115,  115,  115,  115,
    0,   62,  115,    0,    0,    0,    0,    0,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,    0,
  115,   24,   25,   26,   27,    0,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,   62,    0,
  115,  115,  115,  115,    0,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,   62,    0,   24,   25,
   26,   27,    0,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,    0,    0,   24,   25,   26,   27,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   62,    0,    0,    0,    0,    0,    0,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,    0,    0,   24,   25,   26,   27,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   62,    0,
    0,    0,    0,    0,    0,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,    0,    0,   24,   25,
   26,   27,   56,    0,    0,   56,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   62,   56,
   56,    0,    0,    0,   56,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,    0,    0,   24,   25,
   26,   27,    1,    1,   56,    1,    1,    1,    1,    1,
    1,    1,    1,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    1,    1,    1,    1,    1,    0,
    0,    0,    0,    0,    0,   56,   56,    2,    2,    0,
    2,    0,    2,    2,    2,    2,    2,    2,    0,    0,
    0,    0,    0,    0,    0,    0,    1,    0,    1,    1,
    2,    2,    2,    2,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   20,   20,    0,    0,   20,   20,
   20,   20,   20,    0,   20,    0,    0,    0,    0,    1,
    1,    2,    0,    0,    2,   20,   20,   20,   20,   20,
   20,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   27,   27,    0,    0,   27,   27,   27,   27,   27,
    0,   27,    0,    0,    2,    0,    0,    0,    0,    0,
   20,   20,   27,   27,   27,   27,   27,   27,   49,    0,
    0,   49,    0,    0,   49,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   49,   49,
    0,   20,   20,   49,    0,   32,   32,   27,   27,   32,
   32,   32,   32,   32,    0,   32,    0,    0,    0,    0,
    0,    0,   56,   56,    0,    0,   32,   32,   32,    0,
   32,   32,    0,   49,   49,    0,    0,    0,   27,   27,
    0,   56,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   32,   32,    0,   49,   49,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    0,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    0,    0,   32,   32,    0,    1,    0,    0,    0,    0,
    0,    2,    2,    2,    2,    2,    2,    2,    2,    2,
    0,    2,    2,    2,    2,    2,    2,    2,    2,    2,
    2,    2,    2,    2,    2,    0,    0,    0,    0,    0,
    2,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   20,   20,   20,   20,   20,   20,    0,   20,   20,
   20,   20,   20,   20,   20,   20,   20,   20,   20,   20,
   20,   20,    0,    0,    0,    0,    0,   20,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   27,   27,
   27,   27,   27,   27,    0,   27,   27,   27,   27,   27,
   27,   27,   27,   27,   27,   27,   27,   27,   27,    0,
   38,    0,    0,   38,   27,   38,   38,   38,   49,   49,
    0,   49,   49,    0,    0,    0,    0,    0,    0,    0,
   38,   38,   38,    0,   38,   38,    0,    0,    0,    0,
   49,    0,   32,   32,   32,   32,   32,   32,    0,   32,
   32,   39,    0,    0,   39,    0,   39,   39,   39,    0,
    0,    0,    0,    0,    0,   38,   38,    0,   32,    0,
    0,   39,   39,   39,    0,   39,   39,    0,   40,    0,
    0,   40,    0,   40,   40,   40,    0,    0,   41,    0,
    0,   41,    0,    0,   41,    0,   38,   38,   40,   40,
   40,    0,   40,   40,    0,    0,   39,   39,   41,   41,
   41,    0,   41,   41,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   42,    0,    0,   42,    0,
    0,   42,    0,   40,   40,    0,   43,   39,   39,   43,
    0,    0,   43,   41,   41,   42,   42,   42,    0,   42,
   42,    0,    0,    0,    0,    0,   43,   43,   43,    0,
   43,   43,    0,   44,   40,   40,   44,    0,    0,   44,
    0,    0,    0,   47,   41,   41,   47,    0,    0,   47,
   42,   42,    0,   44,   44,   44,    0,   44,   44,    0,
    0,   43,   43,   47,   47,   47,    0,   47,   47,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   48,   42,   42,   48,    0,    0,   48,    0,   44,   44,
    0,    0,   43,   43,    0,    0,    0,    0,   47,   47,
   48,   48,   48,    0,   48,   48,   38,   38,   38,   38,
   38,   38,    0,   38,   38,    0,    0,    0,    0,   44,
   44,    0,    0,    0,    0,    0,    0,    0,    0,   47,
   47,    0,   38,    0,    0,   48,   48,    0,    0,    0,
    0,    0,    0,   61,    0,    0,   61,   39,   39,   39,
   39,   39,   39,    0,   39,   39,    0,    0,    0,    0,
   61,   61,    0,    0,    0,   61,   48,   48,    0,    0,
    0,    0,    0,   39,   40,   40,   40,   40,   40,   40,
    0,   40,   40,    0,   41,   41,   41,   41,   41,   41,
    0,   41,   41,   45,    0,   61,   45,    0,    0,   45,
   40,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   41,    0,    0,   45,   45,   45,    0,   45,   45,    0,
    0,   42,   42,   42,   42,   42,   42,   61,   42,   42,
    0,    0,   43,   43,   43,   43,   43,   43,    0,   43,
   43,    0,    0,    0,    0,    0,    0,   42,   45,   45,
    0,    0,    0,    0,    0,    0,    0,    0,   43,    0,
    0,   44,   44,   44,   44,    0,   44,   44,    0,    0,
    0,   47,   47,   47,   47,    0,   47,   47,   46,   45,
   45,   46,    0,    0,   46,   44,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   47,    0,    0,   46,   46,
   46,   50,   46,   46,   50,    0,    0,   50,   48,   48,
   48,   48,    0,   48,   48,    0,    0,   51,    0,    0,
   51,   50,   50,   51,   52,    0,   50,   52,    0,    0,
   52,    0,   48,   46,   46,    0,    0,   51,   51,    0,
    0,    0,   51,    0,   52,   52,    0,    0,    0,   52,
    0,    0,    0,    0,    0,    0,   50,   50,    0,    0,
    0,    0,    0,    0,   46,   46,    0,   55,    0,    0,
   55,    0,   51,   51,   61,   53,    0,    0,   53,   52,
   52,   53,    0,    0,   55,   55,    0,   50,   50,   55,
   57,    0,   61,   57,    0,   53,   53,    0,    0,    0,
   53,    0,    0,   51,   51,    0,    0,   57,   57,    0,
   52,   52,   57,    0,    0,    0,    0,    0,    0,   55,
   55,   45,   45,   45,   45,    0,   45,   45,    0,    0,
   53,   53,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   57,    0,    0,   45,    0,    0,    0,    0,
   55,   55,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   53,   53,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   57,   57,    0,    0,    0,    0,    1,
    1,    0,    1,    0,    1,    1,    1,    1,    1,    1,
  138,  139,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    1,    1,    1,    1,    1,   46,   46,   46,   46,
  101,   46,   46,    0,  103,   96,    0,   94,    0,   97,
   98,    0,   99,    0,  102,    0,    0,    0,    0,    0,
   46,   50,   50,    1,   50,   50,    1,  104,  108,  105,
    0,    0,    0,    0,    0,    0,    0,   51,   51,    0,
   51,   51,    0,   50,    0,    0,    0,   52,   52,    0,
    0,    0,    0,    0,    0,    0,    1,    0,   95,   51,
    0,  106,    0,    0,    0,    0,   52,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  256,  139,   55,   55,    0,
    0,  107,    0,  100,    0,    0,    0,    0,   53,   53,
    0,    0,    0,    0,    0,    0,   55,    0,    0,    0,
   57,   57,   49,    0,    0,    0,    0,   53,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   57,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  133,  134,  135,  136,  137,    0,   50,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,    0,    0,
   24,   25,   26,   27,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    0,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,   49,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   79,
   80,   81,   82,   83,   84,   85,   86,    0,   87,   88,
   89,   90,   91,   92,   93,  133,  134,  135,  136,  137,
    0,   50,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,   62,    0,   24,   25,   26,   27,    0,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,   62,    0,   24,   25,   26,   27,  396,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   62,
    0,   24,   25,   26,   27,    0,    0,    0,    0,    0,
    0,    0,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,    0,    0,   24,
   25,   26,   27,
  };
  protected static readonly short [] yyCheck = {            53,
    0,  161,  121,   71,  225,   44,   40,   44,   44,   51,
   40,  130,  131,   63,   73,   42,  184,  353,   40,    0,
   59,   68,   59,   59,  125,   58,   52,   53,  155,   29,
  141,  163,   32,   33,   34,   35,  448,  273,   44,  123,
   59,   58,  192,  193,   40,  126,   58,  296,   29,   27,
   46,   46,   44,  257,   44,  205,  125,   44,   44,  125,
   38,  115,  130,  264,  265,  228,   61,   67,   67,   44,
  257,   71,  270,   44,   43,  117,   45,   58,   67,   44,
   60,  285,   62,  125,  257,   40,   67,  113,   59,  115,
  240,   53,  126,  152,  127,   91,   91,  285,  285,  189,
  190,  155,   40,  123,  123,  131,  123,  268,  269,  228,
  522,  125,   38,  155,   40,  148,   42,  161,  286,  125,
   38,  154,   40,  242,   42,   38,   40,   40,   93,   42,
  130,  123,  176,  177,  178,  125,   91,  137,  125,  125,
  251,   59,   44,  143,  143,   44,  127,  128,  192,  193,
  125,  270,  284,   91,  143,   40,  137,   59,  407,  408,
   59,  205,  143,   40,  190,   91,  335,  336,   38,  131,
   40,   41,   42,  257,   44,  511,  257,   91,  246,  306,
  125,  417,  332,  333,  334,  244,  125,  255,  257,   59,
  309,  257,  257,  353,  357,  314,  240,  287,   52,  289,
  126,  285,  292,  293,  294,   59,   91,  123,  126,  251,
  291,  330,  238,  126,   44,  269,  242,  264,  368,  320,
  321,   91,  272,  257,   33,  315,   61,  269,   58,   38,
  287,   40,  289,   42,   43,  126,   45,  126,  357,  389,
  270,  126,  361,  276,  363,  271,  246,  366,  270,  126,
   59,  290,  306,  290,  290,  255,  126,  291,  270,  113,
  350,  287,  412,  289,  298,  261,  262,  263,  270,  269,
   41,  298,  391,  307,  308,  394,  238,  277,  304,  305,
  307,  308,  260,  261,  262,  448,  266,  267,  332,  333,
  334,  335,  336,  337,  338,  339,  340,  341,  342,  343,
  344,  345,  346,  347,  348,  349,  320,  321,  476,  353,
  478,   61,  533,  534,  123,  296,  125,  126,   41,  482,
   93,   44,  472,   33,  368,  393,  486,  490,   38,  448,
   40,  257,   42,   43,  502,   45,  504,  363,   41,  257,
  459,  257,  510,   41,  257,  389,   44,  525,   33,   40,
  518,  511,  520,   38,  532,   40,   58,   42,   43,  522,
   45,  415,   58,  482,   41,  291,  190,   44,  412,  285,
   58,  490,  257,  291,   59,  320,  321,  285,  291,  123,
  257,  320,  321,   93,   40,   41,   41,  257,   44,  479,
  480,  481,   41,  393,  285,   44,  285,  337,  338,   44,
  291,  298,  291,  522,   61,   38,  291,   40,   41,   42,
  307,  308,  343,  344,  291,  415,  126,  271,   40,  273,
   38,  291,   40,   41,   42,  304,  305,   37,  472,   38,
  257,   40,   42,   42,   40,   91,   41,   47,  123,   44,
  125,  126,  486,  339,  340,  341,  342,   40,  257,  258,
  259,  260,   33,  262,  263,   40,   59,   38,   91,   40,
   59,   42,   43,  287,   45,  289,   41,  511,   41,   44,
  126,   44,   41,   91,   41,   44,  285,   44,   59,   38,
  304,  305,   91,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  313,  314,  315,  316,  317,  318,
  257,  320,  321,  322,   94,  324,  325,  326,  327,  328,
  329,  330,  331,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  282,  283,  284,  124,  271,  270,  285,  286,
  287,  288,  123,   41,  125,  126,   44,  257,  258,  259,
  260,   41,  262,  263,   44,   41,   41,   33,   44,   44,
   41,   41,   38,   44,   40,   93,   42,   43,   41,   45,
  257,   44,  257,  258,  259,  260,   40,  262,  263,   61,
  325,  257,   59,   59,  294,   41,  257,  257,  298,  257,
   93,   41,   61,   41,   59,   41,   41,  307,  308,   41,
  285,  257,   93,  313,  314,   41,   41,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,  316,  317,  318,   40,  291,   93,  322,   93,  324,
  325,  326,  327,  328,  329,  330,  331,  123,   59,   33,
  126,   41,   93,   93,   38,  123,   40,  123,   42,   43,
   59,   45,  323,   58,   41,   59,   58,   41,   41,  482,
  314,  345,  347,  346,  117,   59,  257,  258,  259,  260,
  349,  262,  263,  348,  383,   78,  277,   29,  514,  143,
   -1,  358,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   40,   41,  285,   -1,   44,   -1,   -1,   -1,
   -1,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  313,  314,  315,  316,  317,  318,   -1,  123,
   -1,  322,  126,  324,  325,  326,  327,  328,  329,  330,
  331,   -1,   -1,   91,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,  285,
   40,   -1,   42,   43,   -1,   45,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  313,  314,  315,
  316,  317,  318,   -1,  123,   -1,  322,  126,  324,  325,
  326,  327,  328,  329,  330,  331,   -1,   -1,   -1,   -1,
   35,   -1,   -1,   93,   -1,   40,   41,   -1,   -1,   44,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,   59,   -1,   61,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   35,   -1,   -1,   -1,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   91,   92,   -1,  257,
   -1,   -1,   -1,   -1,   -1,   59,   -1,   61,   -1,  313,
  314,   -1,   -1,   38,   -1,   40,   41,   42,  322,   44,
  324,  325,  326,  327,  328,  329,  330,  331,  123,   -1,
   35,   -1,   41,  291,   59,   44,   41,   -1,   92,   44,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,
   59,   -1,   -1,   -1,   59,   -1,   61,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   91,   -1,   -1,  123,
   -1,   35,   -1,   -1,   -1,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   93,   -1,   -1,   92,   -1,   -1,
   -1,   -1,   -1,   33,   -1,   59,   -1,   61,   38,   -1,
   40,  126,   42,   43,   -1,   45,   46,  257,  258,  259,
  260,   -1,  262,  263,  313,  314,  125,   -1,  123,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   92,   -1,
   35,   38,   -1,   40,   41,   42,   41,   44,   -1,   44,
   -1,   -1,   -1,   -1,  294,   -1,   -1,   -1,  298,   -1,
   -1,   91,   59,   -1,   59,   -1,   61,  307,  308,  123,
   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  123,   91,  125,  126,   92,   -1,   -1,
  285,   -1,   -1,   -1,   -1,  290,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  123,  126,
  315,  316,  317,  318,   59,   -1,   -1,   -1,   -1,   -1,
   -1,  285,  257,   -1,   -1,   -1,  290,  332,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,   -1,
   -1,  315,  316,  317,  318,   33,  291,   -1,   -1,   -1,
   38,   -1,   40,   41,   42,   43,   -1,   45,  332,   -1,
  285,  290,   -1,   -1,   -1,  290,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,   -1,   -1,   -1,  257,  258,  259,
  260,  285,  262,  263,   -1,   -1,  290,  332,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,   -1,
  257,  315,  316,  317,  318,   -1,   -1,   35,  126,   -1,
   -1,   -1,   -1,   -1,   41,   -1,   -1,   44,  332,   -1,
   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   -1,   -1,
  285,   58,   59,   61,  291,  290,   63,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,   92,   -1,   93,   -1,   -1,   -1,
   -1,   33,  257,   -1,   -1,   -1,   38,  332,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  123,   -1,   59,  125,   -1,
  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   33,  262,  263,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,  126,   42,   43,  285,   45,   46,
   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  313,  314,  315,  316,  317,
  318,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   91,   33,   -1,   -1,   58,   59,
   38,   -1,   40,   63,   42,   43,   -1,   45,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,  126,   42,   43,   -1,
   45,   46,   -1,   -1,  271,  272,  123,   -1,  125,  126,
   -1,   -1,   -1,   93,   -1,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  290,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  125,   91,  315,  316,  317,
  318,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,  332,   33,   -1,   -1,  126,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,  123,   -1,
   -1,  126,   -1,  285,   -1,   -1,   -1,   -1,   -1,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,  318,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   33,  262,  263,   -1,   -1,   38,
   -1,   40,   41,   42,   43,  285,   45,   -1,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  313,  314,  315,  316,  317,  318,   -1,
   -1,  271,  272,   -1,   -1,   -1,  313,  314,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
  290,   -1,  257,  258,  259,  260,   33,  262,  263,   -1,
   -1,   38,   -1,   40,   33,   42,   43,   -1,   45,   38,
   -1,   40,   -1,   42,   43,   -1,   45,  126,   -1,   -1,
  298,   -1,   59,   -1,   -1,   -1,   -1,   -1,   -1,  307,
  308,   -1,   -1,   -1,   33,  313,  314,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   33,   45,   -1,  313,  314,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   33,  262,  263,   -1,   -1,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,  126,
   42,   43,   -1,   45,   93,   -1,   -1,  126,   -1,   -1,
   -1,   -1,   -1,   33,   -1,   93,   -1,   59,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  313,  314,  126,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   33,  262,  263,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,  126,   -1,   -1,   -1,
   -1,   -1,   33,   -1,  126,   -1,   -1,   38,   -1,   40,
   33,   42,   43,   -1,   45,   38,   -1,   40,   41,   42,
   43,   -1,   45,  123,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   33,   -1,   -1,  313,  314,   38,   -1,   40,   41,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  257,  258,
  259,  260,   93,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,   33,  123,   -1,   -1,  126,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,  126,   -1,   -1,   -1,  257,
  258,  259,  260,  126,  262,  263,  313,  314,   -1,   -1,
   -1,   33,   -1,   -1,  313,  314,   38,   -1,   40,   -1,
   42,   43,   -1,   45,  126,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,  313,  314,   -1,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,  313,  314,  257,  258,  259,
  260,   -1,  262,  263,  126,   -1,   58,   59,   -1,   -1,
   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  313,  314,   -1,   -1,   -1,   33,   -1,
   -1,  313,  314,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   93,   -1,   -1,  126,   -1,  257,  258,  259,  260,
   -1,  262,  263,  313,  314,   -1,   -1,   38,   -1,   40,
   41,   42,   -1,   44,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,  125,  257,  258,  259,  260,   59,  262,
  263,   -1,   -1,   -1,   -1,   -1,   38,   -1,   40,   41,
   42,   -1,   44,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,  313,  314,   -1,   -1,   -1,   59,   -1,   -1,
   91,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  126,  313,  314,   -1,   -1,   -1,   -1,   -1,   -1,
  313,  314,   -1,   -1,   -1,  257,  258,  259,  260,   91,
  262,  263,   -1,   41,   -1,  126,   44,   -1,   -1,   -1,
   -1,  313,  314,   38,   -1,   40,   41,   42,   -1,   -1,
   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,  257,  258,  259,  260,   -1,
  262,  263,   38,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,  313,  314,   -1,   -1,   93,   94,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   -1,   -1,   91,   -1,   -1,   38,
   -1,   40,   -1,   42,   -1,   -1,   -1,   -1,   -1,   -1,
  272,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   59,  313,  314,   -1,   -1,   91,   -1,   -1,  290,   -1,
   -1,  126,   -1,   -1,   -1,   -1,   -1,   41,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  126,   -1,   -1,   38,   -1,   -1,  257,   42,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   -1,   -1,  126,   -1,   -1,
   -1,   -1,   -1,   -1,  285,  257,   -1,   -1,  313,  314,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   41,  285,  315,  316,  317,  318,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  126,   -1,  315,  316,  317,  318,   -1,   -1,   -1,
   -1,   -1,  257,  271,  272,   -1,   38,   -1,   40,   41,
   42,   -1,   -1,   -1,   -1,   -1,  123,   -1,   -1,   -1,
   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,  257,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  257,   91,
  315,  316,  317,  318,   41,  291,   -1,   -1,   -1,   -1,
   -1,  270,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,   41,   -1,  315,  316,  317,  318,
   -1,  285,  257,   -1,   -1,   -1,   -1,   -1,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,   -1,
  285,  315,  316,  317,  318,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  285,   -1,
  315,  316,  317,  318,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  285,   -1,  315,  316,
  317,  318,   -1,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,   -1,   -1,  315,  316,  317,  318,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,   -1,   -1,  315,  316,  317,  318,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,   -1,   -1,  315,  316,
  317,  318,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   58,
   59,   -1,   -1,   -1,   63,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,   -1,   -1,  315,  316,
  317,  318,   37,   38,   93,   40,   41,   42,   43,   44,
   45,   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   37,   38,   -1,
   40,   -1,   42,   43,   44,   45,   46,   47,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   91,   -1,   93,   94,
   60,   61,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   37,   38,   -1,   -1,   41,   42,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,  124,
  125,   91,   -1,   -1,   94,   58,   59,   60,   61,   62,
   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   37,   38,   -1,   -1,   41,   42,   43,   44,   45,
   -1,   47,   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,
   93,   94,   58,   59,   60,   61,   62,   63,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,
   -1,  124,  125,   63,   -1,   37,   38,   93,   94,   41,
   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,   -1,   -1,   58,   59,   60,   -1,
   62,   63,   -1,   93,   94,   -1,   -1,   -1,  124,  125,
   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   94,   -1,  124,  125,  261,  262,  263,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,  273,  274,
  275,  276,  277,  278,  279,  280,  281,  282,  283,  284,
   -1,   -1,  124,  125,   -1,  290,   -1,   -1,   -1,   -1,
   -1,  261,  262,  263,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  282,  283,  284,   -1,   -1,   -1,   -1,   -1,
  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
  273,  274,  275,  276,  277,  278,  279,  280,  281,  282,
  283,  284,   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,  282,  283,  284,   -1,
   38,   -1,   -1,   41,  290,   43,   44,   45,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,
  290,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  290,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,   38,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,  124,  125,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   93,   94,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   93,   94,   -1,   38,  124,  125,   41,
   -1,   -1,   44,   93,   94,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   -1,
   62,   63,   -1,   38,  124,  125,   41,   -1,   -1,   44,
   -1,   -1,   -1,   38,  124,  125,   41,   -1,   -1,   44,
   93,   94,   -1,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   93,   94,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   38,  124,  125,   41,   -1,   -1,   44,   -1,   93,   94,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,   94,
   58,   59,   60,   -1,   62,   63,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,  290,   -1,   -1,   93,   94,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   -1,   44,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,
   58,   59,   -1,   -1,   -1,   63,  124,  125,   -1,   -1,
   -1,   -1,   -1,  290,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   -1,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   38,   -1,   93,   41,   -1,   -1,   44,
  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  290,   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,
   -1,  264,  265,  266,  267,  268,  269,  125,  271,  272,
   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,   -1,  290,   93,   94,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,   -1,
   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   -1,  266,  267,  268,  269,   -1,  271,  272,   38,  124,
  125,   41,   -1,   -1,   44,  290,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   58,   59,
   60,   38,   62,   63,   41,   -1,   -1,   44,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   38,   -1,   -1,
   41,   58,   59,   44,   38,   -1,   63,   41,   -1,   -1,
   44,   -1,  290,   93,   94,   -1,   -1,   58,   59,   -1,
   -1,   -1,   63,   -1,   58,   59,   -1,   -1,   -1,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   41,   -1,   -1,
   44,   -1,   93,   94,  272,   38,   -1,   -1,   41,   93,
   94,   44,   -1,   -1,   58,   59,   -1,  124,  125,   63,
   41,   -1,  290,   44,   -1,   58,   59,   -1,   -1,   -1,
   63,   -1,   -1,  124,  125,   -1,   -1,   58,   59,   -1,
  124,  125,   63,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,  290,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   37,
   38,   -1,   40,   -1,   42,   43,   44,   45,   46,   47,
  125,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   63,  266,  267,  268,  269,
   33,  271,  272,   -1,   37,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
  290,  268,  269,   91,  271,  272,   94,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  268,  269,   -1,
  271,  272,   -1,  290,   -1,   -1,   -1,  271,  272,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,   -1,   91,  290,
   -1,   94,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  125,  126,  271,  272,   -1,
   -1,  124,   -1,  126,   -1,   -1,   -1,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,
  271,  272,  257,   -1,   -1,   -1,   -1,  290,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,  286,  287,  288,  289,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,   -1,   -1,
  315,  316,  317,  318,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  261,  262,  263,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,  257,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  262,
  263,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
  273,  274,  275,  276,  277,  285,  286,  287,  288,  289,
   -1,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  285,   -1,  315,  316,  317,  318,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  285,   -1,  315,  316,  317,  318,  319,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  285,
   -1,  315,  316,  317,  318,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,   -1,   -1,  315,
  316,  317,  318,
  };

#line 1263 "CParser.jay"

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
