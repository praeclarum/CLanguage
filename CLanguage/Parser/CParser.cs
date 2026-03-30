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
//t    "ctor_identifier_list : IDENTIFIER COLONCOLON TYPE_NAME",
//t    "ctor_identifier_list : IDENTIFIER COLONCOLON '~' TYPE_NAME",
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
#line 1125 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-2+yyTop]).ToString()).Push((yyVals[0+yyTop]).ToString());
	}
  break;
case 270:
#line 1129 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[-3+yyTop]).ToString()).Push("~" + (yyVals[0+yyTop]).ToString());
	}
  break;
case 271:
#line 1130 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 272:
#line 1131 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
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
case 276:
  case_276();
  break;
case 277:
  case_277();
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

void case_273()
#line 1136 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_274()
#line 1145 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_275()
#line 1157 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_276()
#line 1163 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_277()
#line 1169 "CParser.jay"
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
   77,   77,   75,   75,   76,   76,   76,   78,   78,   78,
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
    3,    4,    4,    5,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,    0,   95,   96,   97,   98,   99,  143,  170,  101,
  102,  103,  104,  107,  108,  105,  106,  169,  171,  100,
  109,  110,  111,  127,  128,  129,    0,  264,    0,  263,
    0,    0,    0,    0,    0,  112,  113,    0,  260,  262,
  265,    0,    0,  115,  116,    0,    0,  261,  146,    0,
    0,    0,   81,    0,   91,    0,    0,    0,    0,  114,
   84,   86,   88,   90,    0,    0,  119,    0,    0,  269,
    0,    0,    0,  139,    0,    0,  167,  165,    0,  147,
    0,   82,  278,    0,    0,  279,  280,  275,    0,  267,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  221,  225,    0,    0,    0,  223,  226,  227,    0,    0,
  117,  271,    0,    0,    0,    0,    0,  174,  270,    0,
  134,    0,    0,  151,  168,  166,   92,    0,    0,    2,
    3,    0,    0,    0,    4,    5,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  215,    0,    0,   27,
   28,   29,   30,  239,    7,    0,    0,   78,    0,   33,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   63,  219,  220,  208,  209,  210,  211,  212,  213,
    0,  217,    1,    0,  196,   94,  277,  266,  276,  162,
    0,   17,    0,    0,  159,    0,    0,    0,  148,    0,
  233,  234,  235,  228,    0,  231,    0,    0,  222,  224,
  238,    0,    0,    0,  123,    0,  121,  272,  273,    0,
    0,    0,    0,  178,    0,    0,    0,   31,   80,  142,
  136,  140,  135,    0,    0,    0,   25,    0,   20,   21,
    0,    0,    0,    0,    0,    0,  256,  257,  258,    0,
    0,    0,    0,    0,    0,   22,   23,    0,  240,    0,
   13,   14,    0,    0,    0,   66,   67,   68,   69,   70,
   71,   72,   73,   74,   75,   76,   77,   65,    0,   24,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  216,
  218,    0,    0,    0,  199,    0,    0,  204,  161,    0,
  160,    0,  158,  154,    0,  153,    0,    0,  149,    0,
    0,    0,  237,    0,  124,  125,  126,    0,  118,  192,
    0,    0,  186,    0,    0,    0,    0,    0,    0,  274,
  173,  175,  137,  214,    0,    0,    0,    0,    0,    0,
    0,    0,  255,  259,    6,    0,  130,  132,    0,    0,
  181,   79,   12,    9,    0,    0,   11,   64,   34,   35,
   36,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  207,  197,
    0,  200,  203,  205,   18,    0,    0,  157,  152,    0,
    0,  229,  236,  122,  193,  185,  190,  187,  177,  194,
    0,  188,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   32,   10,    8,    0,  206,  198,  201,
    0,  155,  156,  230,    0,  195,  191,  189,    0,    0,
  249,    0,    0,    0,    0,    0,    0,   62,  202,  232,
    0,    0,    0,  244,    0,  245,    0,  253,    0,  251,
    0,   15,    0,  242,    0,    0,  243,  246,  250,  254,
  252,   16,    0,    0,    0,
  };
  protected static readonly short [] yyDgoto  = {            29,
  155,  156,  157,  191,  252,  304,  158,  159,  160,  161,
  162,  163,  164,  165,  166,  167,  168,  169,  170,  171,
  172,  279,  230,  173,   89,   54,   32,   33,   34,   35,
   55,  128,  186,   36,   37,  215,   38,   67,  216,  217,
  255,   73,   74,   57,   58,   59,   79,  331,  117,  118,
  332,  225,  306,  307,  308,  174,  175,  176,  177,  178,
  179,  180,  181,  182,  105,  106,  107,  108,  206,  109,
  455,  456,   39,   40,   41,   91,   42,   92,
  };
  protected static readonly short [] yySindex = {          726,
 -186,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, -115,    0,  726,    0,
   36, 3303, 3303, 3303, 3303,    0,    0,  -98,    0,    0,
    0,  -27,  -97,    0,    0, -148,   85,    0,    0,   37,
   23, -111,    0,  173,    0,  887,  -21,   34,  -39,    0,
    0,    0,    0,    0, 3243,   58,    0,  -82, 1845,    0,
  -33,  196,   18,    0, -148,  236,    0,    0,   23,    0,
   37,    0,    0,  259, 1253,    0,    0,    0,   36,    0,
 3121, 3303,   34,  949, 1271,  -62,  240,  257,  281, 3303,
    0,    0,   36,  -26, 3276,    0,    0,    0,  262,  249,
    0,    0,   70,  245,   43,  322,  315,    0,    0, 1699,
    0, -109,   47,    0,    0,    0,    0,  333,  359,    0,
    0, 1713, 1721, 1721,    0,    0,  336,  389,  395,  487,
  408,  197,  423,  427,  107, 1181,    0, 1699, 1699,    0,
    0,    0,    0,    0,    0,  182,  -23,    0,  163,    0,
 1699,  346,   76,  116,   91,  165,  457,  403,  379,  242,
  -58,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  373,    0,    0, 1242,    0,    0,    0,    0,    0,    0,
  123,    0,  490,  103,    0, 1287,  415, 1279,    0,  271,
    0,    0,    0,    0,   36,    0,  -41, 2016,    0,    0,
    0,   94,   94,   94,    0,   78,    0,    0,    0, 1878,
 1344,  472,   -2,    0,   40,  245, 3162,    0,    0,    0,
    0,    0,    0,  -64,  487, 1181,    0, 1181,    0,    0,
 1699, 1699, 1699,  214, 1043,  488,    0,    0,    0,  316,
  237,  508,  340,  340,  263,    0,    0, 1699,    0,  345,
    0,    0, 1377, 1699,  347,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1699,    0,
 1699, 1699, 1699, 1699, 1699, 1699, 1699, 1699, 1699, 1699,
 1699, 1699, 1699, 1699, 1699, 1699, 1699, 1699, 1699,    0,
    0, 1699,  354,   60,    0, 1253,  310,    0,    0, 1699,
    0,   90,    0,    0, 1699,    0, 1414,  484,    0,  -32,
  518,  558,    0,  575,    0,    0,    0,  249,    0,    0,
  577,  578,    0, 1518,  528, 1699,   40, 2043, 1575,    0,
    0,    0,    0,    0,  581,  582,  351,  363,  384,  584,
 1391, 1391,    0,    0,    0, 1583,    0,    0, 1973,  146,
    0,    0,    0,    0,  460,   48,    0,    0,    0,    0,
    0,  346,  346,   76,   76,  116,  116,  116,  116,   91,
   91,  165,  457,  403,  379,  242,   59,  533,    0,    0,
  695,    0,    0,    0,    0,  535,  541,    0,    0,  593,
 1622,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  612,    0, 1642,  569,  538,  538,  487,  540,  487, 1699,
 1653, 1679, 1242,    0,    0,    0, 1699,    0,    0,    0,
 1253,    0,    0,    0,  633,    0,    0,    0,  371, -101,
    0,  470,  487,  504,  487,  537,   74,    0,    0,    0,
  487, 1699,  637,    0,  -74,    0,  645,    0,  487,    0,
  487,    0,  855,    0,  647,  467,    0,    0,    0,    0,
    0,    0,  467,  467,  467,
  };
  protected static readonly short [] yyRindex = {            0,
    0, 1946,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  282,  870,  875, 1674,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1745,    0,    0,    0,
   29,    0,    0,    0,    0,  318,    0,  763,  568,    0,
    0,    0,    0,    0,    0, 1817,    0,    0,    0,    0,
    0,  117,    0,    0,    0,    0,    0,    0,   53,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  846,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  550,    0,  665,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    1, 3230,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2351,    0, 2419,    0,
    0, 2447, 2694, 2741, 2853, 3047,  -37, 3086, 2886, 1337,
  -11,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  551,  555,    0,  556,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  295,  411,  666,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  557,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  667,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2456, 2636, 2704, 2731, 2789, 2799, 2826, 2894, 2979,
 2988, 3076, 3074, 3115, 2923, 3161,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2379,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2324,    0,    0,    0,  164,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -72,   12,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  685,    0,  446,  176,  287,  -85,  -22, -106,    0,
  114,  320,  252,  231,  417,  418,  416,  419,  420,    0,
 -119,    0, -270,  100,    2,  -45,    0,  136, 1376,    0,
  634,  400, -135,    0,    0,  130,    0,  -20,    0,  386,
  361,  642,  -50,  -40,   -3,  -24,  -36,  -54,    0,  492,
  -65, -171, -325,    0,  413, -134,    0,  -28, -139,    0,
    0,    0, -358, -161,    0,  616,    0,    0,    0,    0,
    0,  267,  694,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           185,
  229,   31,   81,   53,  299,  244,   53,   46,  192,  197,
   78,   81,   69,  208,  116,  231,  263,   82,   50,  301,
   53,   53,  265,  454,   65,   53,   82,   90,   71,   61,
   31,  388,   61,   61,   62,   63,   64,  220,  126,  193,
  104,  256,  257,  113,   93,  111,   61,   61,  305,  224,
  467,  337,  248,   93,  280,   53,   53,  207,  198,   93,
  343,  122,  188,  200,   51,  431,  103,  264,  163,  163,
  115,  232,  163,   94,  223,   50,   50,   51,   51,  338,
  104,   61,  220,   43,   51,  219,   53,   53,  221,  257,
  234,  258,  164,  164,   53,  115,  164,  228,  185,   30,
  344,  205,  258,  391,   52,  352,  103,  474,   72,  237,
  239,  240,  318,   61,  475,  110,  427,  463,  284,  163,
  285,  328,  153,   52,   95,  228,  228,  148,   30,  146,
  339,  149,  150,  221,  151,  335,  247,  431,  228,  153,
  426,   44,  121,  164,  148,   80,  146,   72,  149,  150,
  290,  151,  291,  324,  163,   88,   47,  312,   44,  320,
  141,   52,   52,  309,  102,  249,  310,   66,   52,   45,
  392,  233,  362,  228,  369,  370,  371,  192,  164,  223,
   65,  465,  229,  232,  390,  359,   45,   70,  337,  361,
  187,  189,   72,  368,  199,  329,  241,  340,  462,  204,
   65,  241,  112,  241,  102,  241,  241,   75,  241,  115,
  257,  421,  422,  298,  360,  152,   81,  452,  453,   93,
  185,  115,  241,  278,  395,  258,  396,  257,  115,  397,
   96,   82,  152,   53,   53,   49,  221,  260,  261,  262,
  259,  141,   68,   96,  452,  453,  248,  248,  322,  424,
  409,  119,   53,  414,   49,  430,  120,  400,  228,  228,
  228,  228,  228,  228,  228,  228,  228,  228,  228,  228,
  228,  228,  228,  228,  228,  228,  124,  355,   61,  228,
  258,  253,  439,  411,  441,  163,  241,  305,  241,  241,
   93,  153,   49,   49,  228,  449,  148,  201,  146,   49,
  149,  150,  359,  151,   51,  185,  257,  448,  458,  164,
  460,  228,  301,  301,  202,  185,  464,  154,  360,    9,
  211,   83,   83,   83,  470,   83,  471,  430,   18,   19,
  247,  247,  229,  228,  131,  131,  131,  185,  203,  115,
   83,  325,  326,  327,  351,  185,  183,  130,  131,  132,
   44,  133,  134,  221,  218,  303,  288,  289,  227,  258,
  115,   93,  226,  183,  130,  131,  132,   84,  133,  134,
  393,  253,   83,  253,  354,  241,   93,  185,   45,  286,
  287,   84,  283,  147,  152,  131,    9,  281,  253,  253,
  228,  417,  282,   85,  258,   18,   19,  372,  373,    9,
  302,  135,  136,  418,  228,  153,  258,   83,   18,   19,
  148,  345,  146,  346,  149,  150,  235,  151,  135,  136,
  241,  241,  241,  241,  419,  241,  241,  258,  242,  228,
   56,  154,  292,  293,  243,  266,  267,  268,  269,  270,
  271,  272,  273,  274,  275,  276,  277,  245,  241,   76,
  133,  133,  133,  246,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  241,  247,  241,  241,  241,  248,  241,  241,  241,  241,
  241,  241,  241,  241,  294,   84,  295,  300,  152,  153,
  425,  133,  296,  310,  148,   44,  146,  314,  149,  150,
  457,  151,  297,  258,  222,  129,  130,  131,  132,  153,
  133,  134,  380,  381,  148,  154,  146,  319,  149,  150,
  311,  151,  336,   45,  212,  213,  214,  350,   83,  376,
  377,  378,  379,   60,  459,  154,  353,  258,  356,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
  135,  136,   24,   25,   26,   27,  399,  461,  401,  137,
  258,  138,  139,  140,  141,  142,  143,  144,  145,   84,
  179,  176,  152,  179,  176,  182,  183,  184,  182,  183,
  184,  363,  150,  367,  321,  374,  375,  150,  150,   84,
  389,  150,  152,  357,  358,  403,  402,  405,  406,   76,
  408,  415,  416,  420,   60,  428,  150,  432,  150,  129,
  130,  131,  132,  433,  133,  134,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,  434,  436,   24,   25,   26,   27,   60,  150,  150,
  423,  438,  440,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,  135,  136,   24,   25,   26,   27,
  150,  450,  451,  137,  466,  138,  139,  140,  141,  142,
  143,  144,  145,  469,  473,  172,  180,  182,  365,  447,
  382,  384,  383,  404,  127,  385,  123,  386,  342,  394,
  210,  468,   48,  129,  130,  131,  132,  153,  133,  134,
    0,    0,  148,    0,  146,    0,  149,  150,    0,  151,
  303,    0,    0,  129,  130,  131,  132,    0,  133,  134,
    0,   60,    0,    0,    0,    0,    0,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,  135,  136,
   24,   25,   26,   27,   28,  302,    0,  137,    0,  138,
  139,  140,  141,  142,  143,  144,  145,  145,  135,  136,
    0,    0,    0,  145,    0,    0,  145,  137,    0,  138,
  139,  140,  141,  142,  143,  144,  145,  184,    0,  429,
  152,  145,    0,  145,    0,    0,    0,    0,    0,  250,
  251,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  150,    0,  145,    0,    0,  150,  150,  150,
  150,  150,  150,  150,  150,  150,  150,  150,  150,  150,
  150,  150,  150,  150,  150,  150,  150,  150,  150,    0,
  144,  150,  150,  150,  150,  145,  144,  153,    0,  144,
    0,    0,  148,    0,  146,    0,  149,  150,  150,  151,
  303,    0,    0,    0,  144,    0,  144,    0,    0,   85,
   85,   85,    0,   85,   87,   87,   87,    0,   87,    0,
  251,   86,  251,    0,    0,  347,  348,  349,   85,    0,
    0,    0,    0,   87,    0,    0,    0,  144,    0,    0,
    0,    0,    0,    0,    0,  302,    0,   85,  366,    0,
    0,  183,  130,  131,  132,    0,  133,  134,    0,    0,
   85,    0,    0,    0,    0,   87,    0,    0,  144,    0,
    0,    0,    0,    0,    0,    0,    0,  184,   87,  472,
  152,  153,    1,  387,    0,    0,  148,    0,  146,  190,
  149,  150,    0,  151,    0,   85,    0,    0,    0,    0,
   87,    0,    0,    0,    0,    0,  135,  136,    0,   84,
    2,    0,    0,    0,    0,    0,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,    0,    0,   24,
   25,   26,   27,    0,    0,    0,    0,  145,    0,    0,
    0,    0,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  145,  145,  145,  145,  145,  145,
  145,  145,  145,  145,  152,  153,  145,  145,  145,  145,
  148,    0,  146,    0,  149,  150,    0,  151,    0,    0,
    0,    0,    0,  145,    0,    0,    0,    0,    0,    0,
    0,  154,    0,    0,  442,  444,  446,    0,    0,    0,
    0,  183,  130,  131,  132,    0,  133,  134,    0,    0,
    0,    0,    0,    0,    0,    0,   85,    0,    0,    0,
  144,   87,    0,    0,    0,  144,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,  144,  144,  144,
  144,  144,  144,  144,  144,  144,  144,    0,    0,  144,
  144,  144,  144,    0,    0,    0,  135,  136,  152,    0,
    0,   60,    0,    0,    0,    0,  144,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,    0,    0,
   24,   25,   26,   27,    0,  183,  130,  131,  132,    0,
  133,  134,    0,  153,    0,    0,    0,   83,  148,    0,
  146,    0,  149,  150,    0,  151,    0,    0,    0,    0,
    0,    0,    0,   60,    0,    0,    0,    0,    0,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
  135,  136,   24,   25,   26,   27,    0,    0,    0,    0,
    0,    0,    0,    0,  153,    0,    0,    0,    0,  148,
    0,  146,    0,  149,  150,  153,  151,  303,    0,    0,
  148,    0,  146,    0,  149,  150,    0,  151,    0,  183,
  130,  131,  132,  153,  133,  134,  152,    0,  148,    0,
  146,  153,  196,  150,    0,  151,  148,    0,  146,  153,
  317,  150,    0,  151,  148,    0,  146,   60,  149,  150,
    0,  151,  302,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,  135,  136,   24,   25,   26,   27,
    0,    0,    0,  195,  184,    0,    0,  152,    0,    0,
    0,  316,    0,    0,    0,  184,  153,   59,  152,  313,
   59,  148,    0,  146,    0,  334,  150,    0,  151,    0,
    0,    0,    0,    0,   59,   59,  152,    0,    0,   59,
    0,    0,    0,    0,  152,    0,    0,    0,    0,  153,
    0,    0,  152,    0,  148,    0,  146,  364,  149,  150,
    0,  151,    0,  153,    0,    0,   77,    0,  148,   59,
  146,    0,  149,  150,    0,  151,  333,  183,  130,  131,
  132,    0,  133,  134,    0,    0,  153,    0,    0,  154,
    0,  148,    0,  146,  125,  149,  150,    0,  151,    0,
    0,   59,    0,    0,    0,   60,    0,    0,    0,  152,
   77,    0,    0,    0,    0,    0,    0,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,  135,  136,   24,   25,   26,   27,  183,  130,
  131,  132,  152,  133,  134,    0,  398,    0,    0,  183,
  130,  131,  132,    0,  133,  134,  152,    0,    0,    0,
    0,  254,    0,    0,    0,    0,    0,  183,  130,  131,
  132,    0,  133,  134,    0,  183,  130,  131,  132,  152,
  133,  134,    0,  183,  130,  131,  132,    0,  133,  134,
  153,    0,    0,  135,  136,  148,    0,  146,    0,  149,
  150,    0,  151,  194,  135,  136,    0,    9,    0,   77,
    0,  315,    0,  125,    0,    9,   18,   19,    0,    0,
    0,    0,  135,  136,   18,   19,    0,    0,    0,    0,
  135,  136,    0,    0,    0,    0,    0,    0,  135,  136,
  183,  130,  131,  132,    0,  133,  134,  153,   59,    0,
  407,  254,  148,  254,  146,  153,  413,  150,    0,  151,
  148,    0,  146,    0,  149,  150,   59,  151,  254,  254,
    0,    0,    0,  183,  130,  131,  132,    0,  133,  134,
    0,    0,    0,  152,    0,    0,    0,  183,  130,  131,
  132,    0,  133,  134,  153,  135,  136,    0,    0,  148,
    0,  146,    0,  149,  150,    0,  151,  412,    0,    0,
  183,  130,  131,  132,  153,  133,  134,    0,    0,  148,
    0,  146,    0,  149,  150,  153,  151,  125,  135,  136,
  148,    0,  146,  443,  149,  150,    0,  151,    0,    0,
  152,    0,  135,  136,    0,  423,    0,    0,  152,    0,
    0,  153,    0,   89,   89,   89,  148,   89,  146,  445,
  149,  150,    0,  151,    0,  135,  136,    0,    0,    0,
    0,  153,   89,    0,  437,    0,  148,    0,  146,    0,
  149,  150,    0,  151,  184,  153,    0,  152,    0,    0,
  148,    0,  236,  153,  149,  150,    0,  151,  148,    0,
  238,    0,  149,  150,   89,  151,    0,  152,    0,    0,
    0,    0,    0,    0,  183,  130,  131,  132,  152,  133,
  134,    0,    0,    0,  138,  138,  138,    0,  138,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   89,
    0,    0,    0,  138,  152,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  152,    0,    0,    0,    0,  135,
  136,  183,  130,  131,  132,  138,  133,  134,  152,  183,
  130,  131,  132,    0,  133,  134,  152,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  120,  120,  120,    0,
  120,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  138,    0,    0,    0,    0,  120,    0,    0,  183,  435,
  131,  132,    0,  133,  134,  114,  135,  136,    0,    0,
    0,    0,    0,    0,  135,  136,    0,    0,  183,  130,
  131,  132,    0,  133,  134,    0,    0,  120,    0,  183,
  130,  131,  132,    0,  133,  134,    0,  220,  330,   51,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   89,    0,    0,  135,  136,  183,  130,  131,  132,    0,
  133,  134,  120,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  135,  136,  183,  130,  131,  132,    0,
  133,  134,    0,    0,  135,  136,    0,    0,  221,  183,
  130,  131,  132,    0,  133,  134,    0,  183,  130,  131,
  132,    0,  133,  134,    0,  114,    0,  114,    0,    0,
  135,  136,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  138,    0,   52,  114,    0,    0,    0,    0,    0,
  135,  136,  359,  330,   51,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  135,  136,    0,    0,    0,  138,
    0,    0,  135,  136,    0,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  323,    0,  138,  138,
  138,  138,    0,  221,    0,    0,    0,    0,    0,    0,
    0,  114,    0,  120,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  410,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  120,    0,    0,    0,    0,    0,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,    0,   60,
  120,  120,  120,  120,   49,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,    0,    0,   24,   25,
   26,   27,   60,    0,    0,    0,    0,    0,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,    0,
    0,   24,   25,   26,   27,    0,    0,    0,    0,    0,
    0,    0,  114,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  268,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  114,    0,    0,    0,    0,    0,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,   60,    0,  114,
  114,  114,  114,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,    0,    0,   24,   25,   26,   27,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   60,    0,    0,    0,    0,    0,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   60,    0,   24,
   25,   26,   27,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,    0,    0,   24,   25,   26,   27,
    2,    2,    0,    2,    0,    2,    2,    2,    2,    2,
    2,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    2,    2,    2,    2,   19,   19,    0,
    0,   19,   19,   19,   19,   19,    0,   19,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   19,   19,
   19,   19,   19,   19,    2,   26,   26,    2,    0,   26,
   26,   26,   26,   26,    0,   26,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   26,   26,   26,   26,
   26,   26,    0,   19,   19,    0,    0,    2,    0,    0,
    0,    0,    0,    0,    0,   31,   31,    0,    0,   31,
   31,   31,   31,   31,    0,   31,    0,    0,    0,    0,
    0,   26,   26,    0,   19,   19,   31,   31,   31,    0,
   31,   31,    0,    0,   37,    0,    0,   37,    0,   37,
   37,   37,    0,   38,    0,    0,   38,    0,   38,   38,
   38,    0,   26,   26,   37,   37,   37,    0,   37,   37,
    0,   31,   31,   38,   38,   38,    0,   38,   38,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   37,
   37,    0,   31,   31,    0,    0,    0,    0,   38,   38,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   37,   37,    0,    0,    0,    0,    0,    0,    0,   38,
   38,    0,    0,    0,    2,    2,    2,    2,    2,    2,
    2,    2,    2,    0,    2,    2,    2,    2,    2,    2,
    2,    2,    2,    2,    2,    2,    2,    2,    0,    0,
    0,    0,    0,    2,   19,   19,   19,   19,   19,   19,
    0,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,    0,    0,    0,    0,    0,
   19,    0,   26,   26,   26,   26,   26,   26,    0,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,    0,    0,    0,    0,    0,   26,    0,
    0,    0,    0,   39,    0,    0,   39,    0,   39,   39,
   39,    0,   31,   31,   31,   31,   31,   31,    0,   31,
   31,    0,    0,   39,   39,   39,    0,   39,   39,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   31,    0,
   37,   37,   37,   37,   37,   37,    0,   37,   37,   38,
   38,   38,   38,   38,   38,    0,   38,   38,   39,   39,
    0,   40,    0,    0,   40,    0,   37,   40,    0,    0,
    0,   41,    0,    0,   41,   38,    0,   41,    0,    0,
    0,   40,   40,   40,    0,   40,   40,    0,    0,   39,
   39,   41,   41,   41,    0,   41,   41,    0,   42,    0,
    0,   42,    0,    0,   42,    0,    0,    0,   43,    0,
    0,   43,    0,    0,   43,    0,   40,   40,   42,   42,
   42,    0,   42,   42,    0,    0,   41,   41,   43,   43,
   43,    0,   43,   43,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   40,   40,    0,
    0,    0,    0,   42,   42,    0,   46,   41,   41,   46,
    0,    0,   46,   43,   43,    0,   47,    0,    0,   47,
    0,    0,   47,    0,    0,    0,   46,   46,   46,    0,
   46,   46,    0,    0,   42,   42,   47,   47,   47,    0,
   47,   47,    0,   44,   43,   43,   44,    0,    0,   44,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   46,   46,   44,   44,   44,    0,   44,   44,    0,
   48,   47,   47,   48,    0,    0,   48,    0,    0,   39,
   39,   39,   39,   39,   39,    0,   39,   39,    0,    0,
   48,   48,   46,   46,    0,   48,    0,    0,   44,   44,
    0,    0,   47,   47,    0,   39,   57,    0,    0,   57,
    0,   45,    0,    0,   45,    0,    0,   45,    0,    0,
    0,    0,    0,   57,   57,   48,   48,    0,   57,   44,
   44,   45,   45,   45,    0,   45,   45,   40,   40,   40,
   40,   40,   40,   58,   40,   40,   58,   41,   41,   41,
   41,   41,   41,    0,   41,   41,   48,   48,   57,    0,
   58,   58,    0,   40,    0,   58,   45,   45,    0,    0,
    0,    0,    0,   41,   42,   42,   42,   42,   42,   42,
    0,   42,   42,    0,    0,    0,   43,   43,   43,   43,
   57,   43,   43,    0,    0,   58,   49,   45,   45,   49,
   42,    0,   49,    0,    0,   50,    0,    0,   50,    0,
   43,   50,    0,    0,    0,    0,   49,   49,    0,    0,
    0,   49,    0,    0,    0,   50,   50,   58,    0,    0,
   50,    0,    0,    0,   46,   46,   46,   46,    0,   46,
   46,    0,    0,    0,   47,   47,   47,   47,    0,   47,
   47,   49,   49,    0,    0,    0,    0,    0,   46,    0,
   50,   50,    0,    0,   51,    0,    0,   51,   47,    0,
   51,   44,   44,   44,   44,    0,   44,   44,    0,    0,
    0,    0,   49,   49,   51,   51,    0,    0,    0,   51,
    0,   50,   50,   52,   54,   44,   52,   54,    0,   52,
   48,   48,    0,   48,   48,    0,   55,    0,    0,   55,
    0,   54,   54,   52,   52,    0,   54,    0,   52,   51,
   51,    0,   48,   55,   55,    0,    0,    0,   55,    0,
    0,    0,    0,    0,    0,   56,   57,   57,   56,   45,
   45,   45,   45,    0,   45,   45,   54,   54,   52,   52,
   51,   51,   56,   56,    0,   57,    0,   56,   55,    0,
    0,    0,    0,   45,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   58,   58,    0,    0,   54,   54,   52,
   52,   60,    0,    0,   60,    0,    0,   56,    0,   55,
   55,    0,   58,    0,    0,    0,    0,    0,   60,   60,
    0,    0,    0,   60,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   56,   56,
    0,    0,    0,   84,    0,    0,   49,   49,    0,   49,
   49,    0,    0,   60,    0,   50,   50,    0,   50,   50,
    0,    0,    0,    0,    0,    0,    1,    1,   49,    1,
    0,    1,    1,    1,    1,    1,    1,   50,    0,    0,
    0,    0,    0,    0,    0,   60,    0,    0,    1,    1,
    1,    1,    1,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   51,   51,    0,
    1,    0,    0,    1,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   51,    0,    0,    0,
    0,    0,    0,    0,   54,   54,   52,   52,    0,    0,
    0,    0,    0,    1,    0,    0,   55,   55,    0,    0,
    0,    0,    0,   54,    0,   52,    0,  101,   52,    0,
    0,    0,    0,    0,    0,   55,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   56,   56,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  209,   52,    0,    0,   56,   60,    0,    0,    0,    0,
    0,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   60,    0,   24,   25,   26,   27,    0,    0,
    0,    0,    0,    0,    0,    0,   60,    0,    0,    0,
   60,    0,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,    0,    0,   24,   25,   26,   27,  341,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    1,    1,    1,    1,    1,    1,    1,    1,   49,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   60,   97,   98,
   99,  100,   49,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,    0,    0,   24,   25,   26,   27,
   60,   97,   98,   99,  100,    0,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   60,    0,   24,
   25,   26,   27,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,    0,    0,   24,   25,   26,   27,
  };
  protected static readonly short [] yyCheck = {            85,
  120,    0,   44,   41,   63,  140,   44,  123,   94,   95,
   51,   44,   40,   40,   69,  125,   40,   59,   40,  181,
   58,   59,   46,  125,  123,   63,   59,   56,  126,   41,
   29,  302,   44,   32,   33,   34,   35,   40,   79,   94,
   65,  148,  149,  126,   44,   66,   58,   59,  184,  115,
  125,  223,  125,   57,  161,   93,   94,  103,   95,   59,
  125,   44,   91,  126,   42,  391,   65,   91,   40,   41,
   69,  122,   44,   40,  115,   40,   40,   42,   42,   40,
  105,   93,   40,  270,   42,  114,  124,  125,   91,  196,
   44,   44,   40,   41,   59,   94,   44,  120,  184,    0,
  235,  100,   44,   44,  126,  245,  105,  466,  257,  132,
  133,  134,  198,  125,  473,   58,   58,   44,   43,   91,
   45,   44,   33,  126,   91,  148,  149,   38,   29,   40,
   91,   42,   43,   91,   45,  221,  125,  463,  161,   33,
   93,  257,  125,   91,   38,  257,   40,  257,   42,   43,
   60,   45,   62,  208,  126,   56,   27,  194,  257,  205,
   44,  126,  126,   41,   65,   59,   44,   38,  126,  285,
  306,  125,  258,  196,  281,  282,  283,  263,  126,  220,
  123,  452,  302,  234,  125,   40,  285,  285,  360,  255,
   91,   92,  257,  279,  257,  216,   33,  226,  125,  100,
  123,   38,  285,   40,  105,   42,   43,  123,   45,  208,
  317,  351,  352,  272,  255,  126,   44,  319,  320,  223,
  306,  220,   59,   61,  310,   44,  312,  334,  227,  315,
  270,   59,  126,  271,  272,  257,   91,  261,  262,  263,
   59,  125,  270,  270,  319,  320,  319,  320,  290,  356,
  336,  285,  290,  339,  257,  391,   61,  290,  281,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,   41,   41,  290,  302,
   44,  146,  417,  338,  419,  257,  123,  423,  125,  126,
  290,   33,  257,  257,  317,  431,   38,   58,   40,  257,
   42,   43,   40,   45,   42,  391,  413,  427,  443,  257,
  445,  334,  474,  475,   58,  401,  451,   59,  359,  297,
   59,   40,   41,   42,  459,   44,  461,  463,  306,  307,
  319,  320,  452,  356,   40,   41,   42,  423,   58,  338,
   59,  212,  213,  214,  245,  431,  257,  258,  259,  260,
  257,  262,  263,   91,  285,   46,  266,  267,   44,   44,
  359,   44,   41,  257,  258,  259,  260,  123,  262,  263,
   61,  236,   91,  238,   59,   40,   59,  463,  285,  264,
  265,  123,   37,  125,  126,   91,  297,   42,  253,  254,
  413,   41,   47,   61,   44,  306,  307,  284,  285,  297,
   91,  312,  313,   41,  427,   33,   44,  126,  306,  307,
   38,  236,   40,  238,   42,   43,   58,   45,  312,  313,
  257,  258,  259,  260,   41,  262,  263,   44,   40,  452,
   31,   59,  268,  269,   40,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,   40,  285,   50,
   40,   41,   42,  257,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  313,  314,  315,  316,
  317,   59,  319,  320,  321,   59,  323,  324,  325,  326,
  327,  328,  329,  330,   38,  123,   94,  125,  126,   33,
   41,   91,  124,   44,   38,  257,   40,   93,   42,   43,
   41,   45,  271,   44,  115,  257,  258,  259,  260,   33,
  262,  263,  292,  293,   38,   59,   40,  257,   42,   43,
   41,   45,   61,  285,  286,  287,  288,  324,  257,  288,
  289,  290,  291,  285,   41,   59,   59,   44,   41,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,   93,   41,   61,  321,
   44,  323,  324,  325,  326,  327,  328,  329,  330,  123,
   41,   41,  126,   44,   44,   41,   41,   41,   44,   44,
   44,  257,   35,  257,  205,  286,  287,   40,   41,  123,
  257,   44,  126,  253,  254,   41,   59,   41,   41,  220,
   93,   41,   41,   40,  285,   93,   59,   93,   61,  257,
  258,  259,  260,   93,  262,  263,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   59,   41,  314,  315,  316,  317,  285,   91,   92,
  123,   93,  123,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  313,  314,  315,  316,  317,
  123,   59,  322,  321,   58,  323,  324,  325,  326,  327,
  328,  329,  330,   59,   58,   41,   41,   41,  263,  423,
  294,  296,  295,  328,   81,  297,   75,  298,  227,  307,
  105,  455,   29,  257,  258,  259,  260,   33,  262,  263,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   46,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  313,
  314,  315,  316,  317,   59,   91,   -1,  321,   -1,  323,
  324,  325,  326,  327,  328,  329,  330,   35,  312,  313,
   -1,   -1,   -1,   41,   -1,   -1,   44,  321,   -1,  323,
  324,  325,  326,  327,  328,  329,  330,  123,   -1,  125,
  126,   59,   -1,   61,   -1,   -1,   -1,   -1,   -1,  145,
  146,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   92,   -1,   -1,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,   -1,
   35,  314,  315,  316,  317,  123,   41,   33,   -1,   44,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,  331,   45,
   46,   -1,   -1,   -1,   59,   -1,   61,   -1,   -1,   40,
   41,   42,   -1,   44,   40,   41,   42,   -1,   44,   -1,
  236,   35,  238,   -1,   -1,  241,  242,  243,   59,   -1,
   -1,   -1,   -1,   59,   -1,   -1,   -1,   92,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   91,   -1,   61,  264,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   91,   -1,   -1,   -1,   -1,   91,   -1,   -1,  123,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,   92,  125,
  126,   33,  257,  299,   -1,   -1,   38,   -1,   40,   41,
   42,   43,   -1,   45,   -1,  126,   -1,   -1,   -1,   -1,
  126,   -1,   -1,   -1,   -1,   -1,  312,  313,   -1,  123,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,   -1,   -1,  314,
  315,  316,  317,   -1,   -1,   -1,   -1,  285,   -1,   -1,
   -1,   -1,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  126,   33,  314,  315,  316,  317,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,  331,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   -1,   -1,  420,  421,  422,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,
  285,  257,   -1,   -1,   -1,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,   -1,   -1,  314,
  315,  316,  317,   -1,   -1,   -1,  312,  313,  126,   -1,
   -1,  285,   -1,   -1,   -1,   -1,  331,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,   -1,   -1,
  314,  315,  316,  317,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   33,   -1,   -1,   -1,  331,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   33,   45,   46,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,  257,
  258,  259,  260,   33,  262,  263,  126,   -1,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   33,
   42,   43,   -1,   45,   38,   -1,   40,  285,   42,   43,
   -1,   45,   91,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  313,  314,  315,  316,  317,
   -1,   -1,   -1,   93,  123,   -1,   -1,  126,   -1,   -1,
   -1,   93,   -1,   -1,   -1,  123,   33,   41,  126,   93,
   44,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   -1,   58,   59,  126,   -1,   -1,   63,
   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,   33,
   -1,   -1,  126,   -1,   38,   -1,   40,   41,   42,   43,
   -1,   45,   -1,   33,   -1,   -1,   51,   -1,   38,   93,
   40,   -1,   42,   43,   -1,   45,   93,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   33,   -1,   -1,   59,
   -1,   38,   -1,   40,   79,   42,   43,   -1,   45,   -1,
   -1,  125,   -1,   -1,   -1,  285,   -1,   -1,   -1,  126,
   95,   -1,   -1,   -1,   -1,   -1,   -1,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  313,  314,  315,  316,  317,  257,  258,
  259,  260,  126,  262,  263,   -1,   93,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,  126,   -1,   -1,   -1,
   -1,  146,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,  126,
  262,  263,   -1,  257,  258,  259,  260,   -1,  262,  263,
   33,   -1,   -1,  312,  313,   38,   -1,   40,   -1,   42,
   43,   -1,   45,  293,  312,  313,   -1,  297,   -1,  194,
   -1,  293,   -1,  198,   -1,  297,  306,  307,   -1,   -1,
   -1,   -1,  312,  313,  306,  307,   -1,   -1,   -1,   -1,
  312,  313,   -1,   -1,   -1,   -1,   -1,   -1,  312,  313,
  257,  258,  259,  260,   -1,  262,  263,   33,  272,   -1,
   93,  236,   38,  238,   40,   33,   42,   43,   -1,   45,
   38,   -1,   40,   -1,   42,   43,  290,   45,  253,  254,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,  126,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   33,  312,  313,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   93,   -1,   -1,
  257,  258,  259,  260,   33,  262,  263,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   33,   45,  312,  312,  313,
   38,   -1,   40,   41,   42,   43,   -1,   45,   -1,   -1,
  126,   -1,  312,  313,   -1,  123,   -1,   -1,  126,   -1,
   -1,   33,   -1,   40,   41,   42,   38,   44,   40,   41,
   42,   43,   -1,   45,   -1,  312,  313,   -1,   -1,   -1,
   -1,   33,   59,   -1,   93,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,  123,   33,   -1,  126,   -1,   -1,
   38,   -1,   40,   33,   42,   43,   -1,   45,   38,   -1,
   40,   -1,   42,   43,   91,   45,   -1,  126,   -1,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,  126,  262,
  263,   -1,   -1,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,
   -1,   -1,   -1,   59,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,  312,
  313,  257,  258,  259,  260,   91,  262,  263,  126,  257,
  258,  259,  260,   -1,  262,  263,  126,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   40,   41,   42,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  126,   -1,   -1,   -1,   -1,   59,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   41,  312,  313,   -1,   -1,
   -1,   -1,   -1,   -1,  312,  313,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   91,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   40,   41,   42,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  257,   -1,   -1,  312,  313,  257,  258,  259,  260,   -1,
  262,  263,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  312,  313,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,  312,  313,   -1,   -1,   91,  257,
  258,  259,  260,   -1,  262,  263,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   40,   -1,   42,   -1,   -1,
  312,  313,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  257,   -1,  126,   59,   -1,   -1,   -1,   -1,   -1,
  312,  313,   40,   41,   42,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  312,  313,   -1,   -1,   -1,  285,
   -1,   -1,  312,  313,   -1,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,   41,   -1,  314,  315,
  316,  317,   -1,   91,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  126,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,   -1,  285,
  314,  315,  316,  317,  257,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,   -1,   -1,  314,  315,
  316,  317,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,   -1,
   -1,  314,  315,  316,  317,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  270,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  285,   -1,  314,
  315,  316,  317,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,   -1,   -1,  314,  315,  316,  317,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  285,   -1,  314,
  315,  316,  317,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,   -1,   -1,  314,  315,  316,  317,
   37,   38,   -1,   40,   -1,   42,   43,   44,   45,   46,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   60,   61,   62,   63,   37,   38,   -1,
   -1,   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,
   60,   61,   62,   63,   91,   37,   38,   94,   -1,   41,
   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   61,
   62,   63,   -1,   93,   94,   -1,   -1,  124,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   37,   38,   -1,   -1,   41,
   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,
   -1,   93,   94,   -1,  124,  125,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   38,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   38,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,  124,  125,   58,   59,   60,   -1,   62,   63,
   -1,   93,   94,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,   94,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,  261,  262,  263,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,  282,  283,  284,   -1,   -1,
   -1,   -1,   -1,  290,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  282,  283,  284,   -1,   -1,   -1,   -1,   -1,
  290,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   -1,   -1,   -1,   -1,   -1,  290,   -1,
   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   93,   94,
   -1,   38,   -1,   -1,   41,   -1,  290,   44,   -1,   -1,
   -1,   38,   -1,   -1,   41,  290,   -1,   44,   -1,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,  124,
  125,   58,   59,   60,   -1,   62,   63,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   93,   94,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   93,   94,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   93,   94,   -1,   38,  124,  125,   41,
   -1,   -1,   44,   93,   94,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   58,   59,   60,   -1,
   62,   63,   -1,   -1,  124,  125,   58,   59,   60,   -1,
   62,   63,   -1,   38,  124,  125,   41,   -1,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   94,   58,   59,   60,   -1,   62,   63,   -1,
   38,   93,   94,   41,   -1,   -1,   44,   -1,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   58,   59,  124,  125,   -1,   63,   -1,   -1,   93,   94,
   -1,   -1,  124,  125,   -1,  290,   41,   -1,   -1,   44,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   -1,   -1,   -1,   58,   59,   93,   94,   -1,   63,  124,
  125,   58,   59,   60,   -1,   62,   63,  264,  265,  266,
  267,  268,  269,   41,  271,  272,   44,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  124,  125,   93,   -1,
   58,   59,   -1,  290,   -1,   63,   93,   94,   -1,   -1,
   -1,   -1,   -1,  290,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,  266,  267,  268,  269,
  125,  271,  272,   -1,   -1,   93,   38,  124,  125,   41,
  290,   -1,   44,   -1,   -1,   38,   -1,   -1,   41,   -1,
  290,   44,   -1,   -1,   -1,   -1,   58,   59,   -1,   -1,
   -1,   63,   -1,   -1,   -1,   58,   59,  125,   -1,   -1,
   63,   -1,   -1,   -1,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,  266,  267,  268,  269,   -1,  271,
  272,   93,   94,   -1,   -1,   -1,   -1,   -1,  290,   -1,
   93,   94,   -1,   -1,   38,   -1,   -1,   41,  290,   -1,
   44,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   -1,   -1,  124,  125,   58,   59,   -1,   -1,   -1,   63,
   -1,  124,  125,   38,   41,  290,   41,   44,   -1,   44,
  268,  269,   -1,  271,  272,   -1,   41,   -1,   -1,   44,
   -1,   58,   59,   58,   59,   -1,   63,   -1,   63,   93,
   94,   -1,  290,   58,   59,   -1,   -1,   -1,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   41,  271,  272,   44,  266,
  267,  268,  269,   -1,  271,  272,   93,   94,   93,   94,
  124,  125,   58,   59,   -1,  290,   -1,   63,   93,   -1,
   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  271,  272,   -1,   -1,  124,  125,  124,
  125,   41,   -1,   -1,   44,   -1,   -1,   93,   -1,  124,
  125,   -1,  290,   -1,   -1,   -1,   -1,   -1,   58,   59,
   -1,   -1,   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,  123,   -1,   -1,  268,  269,   -1,  271,
  272,   -1,   -1,   93,   -1,  268,  269,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,   37,   38,  290,   40,
   -1,   42,   43,   44,   45,   46,   47,  290,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   59,   60,
   61,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,
   91,   -1,   -1,   94,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  271,  272,  271,  272,   -1,   -1,
   -1,   -1,   -1,  124,   -1,   -1,  271,  272,   -1,   -1,
   -1,   -1,   -1,  290,   -1,  290,   -1,  125,  126,   -1,
   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  125,  126,   -1,   -1,  290,  285,   -1,   -1,   -1,   -1,
   -1,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  272,   -1,  314,  315,  316,  317,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,
  290,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,   -1,   -1,  314,  315,  316,  317,  318,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  261,  262,  263,  264,  265,  266,  267,  268,  269,  257,
  271,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  282,  283,  284,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,  286,  287,
  288,  289,  257,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,   -1,   -1,  314,  315,  316,  317,
  285,  286,  287,  288,  289,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  285,   -1,  314,
  315,  316,  317,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,   -1,   -1,  314,  315,  316,  317,
  };

#line 1185 "CParser.jay"

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
