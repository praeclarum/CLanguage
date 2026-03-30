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
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class identifier_or_typename ':' base_specifier class_body",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class class_body",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class identifier_or_typename",
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
#line 478 "CParser.jay"
  { yyVal = (yyVals[0+yyTop]).ToString(); }
  break;
case 122:
#line 479 "CParser.jay"
  { yyVal = (yyVals[0+yyTop]).ToString(); }
  break;
case 123:
#line 480 "CParser.jay"
  { yyVal = (yyVals[0+yyTop]).ToString(); }
  break;
case 124:
#line 481 "CParser.jay"
  { yyVal = (yyVals[0+yyTop]).ToString(); }
  break;
case 125:
#line 485 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 126:
#line 486 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 127:
#line 487 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
  break;
case 128:
  case_128();
  break;
case 129:
  case_129();
  break;
case 130:
  case_130();
  break;
case 131:
  case_131();
  break;
case 132:
#line 516 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 133:
#line 517 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 134:
#line 518 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 135:
#line 519 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 136:
#line 520 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 137:
  case_137();
  break;
case 138:
  case_138();
  break;
case 139:
#line 542 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 140:
#line 546 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 141:
#line 550 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 142:
#line 557 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 143:
#line 558 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 144:
#line 566 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 145:
#line 570 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator("~" + (yyVals[-1+yyTop]).ToString());
	}
  break;
case 146:
#line 572 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 147:
#line 574 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[-1+yyTop]).ToString()); }
  break;
case 149:
  case_149();
  break;
case 150:
#line 594 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 151:
#line 598 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 152:
#line 602 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 153:
#line 606 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 154:
#line 610 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 155:
#line 614 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 156:
#line 618 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 157:
#line 622 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 158:
#line 626 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 159:
  case_159();
  break;
case 160:
#line 638 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 161:
#line 642 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 162:
#line 643 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 163:
#line 644 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 164:
#line 645 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 165:
#line 649 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 166:
#line 653 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 167:
#line 657 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 168:
#line 658 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 169:
#line 659 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 170:
#line 666 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 171:
  case_171();
  break;
case 172:
  case_172();
  break;
case 173:
  case_173();
  break;
case 174:
#line 694 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 175:
#line 698 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 176:
#line 702 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 177:
#line 706 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 178:
#line 713 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 179:
#line 717 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 180:
#line 724 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 182:
#line 729 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 183:
  case_183();
  break;
case 184:
#line 748 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 185:
#line 752 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 186:
#line 756 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 187:
#line 760 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 188:
#line 764 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 189:
#line 768 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 190:
#line 772 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 191:
#line 776 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 192:
#line 780 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 193:
#line 784 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 194:
#line 791 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 195:
#line 795 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 196:
#line 799 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 197:
  case_197();
  break;
case 198:
  case_198();
  break;
case 199:
  case_199();
  break;
case 200:
  case_200();
  break;
case 201:
#line 839 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 213:
#line 869 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 214:
#line 873 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 215:
#line 877 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 216:
#line 878 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 219:
#line 890 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 220:
#line 894 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 221:
#line 898 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 222:
#line 899 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 226:
#line 909 "CParser.jay"
  {
		yyVal = new VirtualDeclarationStatement((Statement)yyVals[0+yyTop]) { IsVirtual = true };
	}
  break;
case 227:
  case_227();
  break;
case 228:
  case_228();
  break;
case 229:
#line 923 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 230:
  case_230();
  break;
case 231:
#line 937 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 232:
#line 938 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 233:
#line 939 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 234:
#line 946 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 235:
#line 950 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 236:
  case_236();
  break;
case 237:
#line 968 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 238:
#line 972 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 239:
#line 979 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 240:
#line 983 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 241:
#line 987 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-4+yyTop], (List<SwitchCase>)yyVals[-1+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 242:
#line 991 "CParser.jay"
  {
		yyVal = new SwitchStatement((Expression)yyVals[-3+yyTop], new List<SwitchCase>(), GetLocation(yyVals[-5+yyTop]));
	}
  break;
case 243:
#line 998 "CParser.jay"
  {
		yyVal = new List<SwitchCase> { (SwitchCase)yyVals[0+yyTop] };
	}
  break;
case 244:
  case_244();
  break;
case 245:
#line 1010 "CParser.jay"
  {
		yyVal = new SwitchCase((Expression)yyVals[-2+yyTop], (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 246:
#line 1014 "CParser.jay"
  {
		yyVal = new SwitchCase(null, (List<Statement>)yyVals[0+yyTop]);
	}
  break;
case 247:
#line 1021 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 248:
#line 1025 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 249:
#line 1029 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 250:
#line 1033 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 251:
#line 1037 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 252:
#line 1041 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 254:
#line 1049 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 255:
#line 1053 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 256:
#line 1057 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 257:
#line 1061 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 258:
  case_258();
  break;
case 259:
  case_259();
  break;
case 264:
  case_264();
  break;
case 265:
  case_265();
  break;
case 266:
#line 1109 "CParser.jay"
  {
		yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString());
	}
  break;
case 267:
#line 1110 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 268:
#line 1111 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-3+yyTop])).Push ("~" + (yyVals[0+yyTop]).ToString()); }
  break;
case 269:
  case_269();
  break;
case 270:
  case_270();
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
		ts.BaseClassName = (string)yyVals[-1+yyTop];
		yyVal = ts;
	}

void case_128()
#line 492 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_129()
#line 497 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_130()
#line 503 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_131()
#line 508 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_137()
#line 525 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_138()
#line 531 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_149()
#line 580 "CParser.jay"
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

void case_159()
#line 628 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<Expression>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(ctorArgumentValue: n));
		}
		yyVal = d;
	}

void case_171()
#line 668 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_172()
#line 677 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_173()
#line 683 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_183()
#line 734 "CParser.jay"
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

void case_197()
#line 804 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_198()
#line 811 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_199()
#line 819 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_200()
#line 826 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_227()
#line 911 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsOverride = true };
	}

void case_228()
#line 916 "CParser.jay"
{
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-3+yyTop], (List<InitDeclarator>)yyVals[-2+yyTop]);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsOverride = true };
	}

void case_230()
#line 928 "CParser.jay"
{
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator((Declarator)yyVals[-3+yyTop], null) };
		var inner = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-4+yyTop], decls);
		yyVal = new VirtualDeclarationStatement(inner) { IsVirtual = true, IsPureVirtual = true };
	}

void case_236()
#line 955 "CParser.jay"
{
		var fdecl = (FunctionDeclarator)yyVals[-1+yyTop];
		DeclarationSpecifiers ds = new DeclarationSpecifiers();
		List<InitDeclarator> decls = new List<InitDeclarator> {
			new InitDeclarator(fdecl, null) };
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_244()
#line 1000 "CParser.jay"
{
		((List<SwitchCase>)yyVals[-1+yyTop]).Add((SwitchCase)yyVals[0+yyTop]);
		yyVal = yyVals[-1+yyTop];
	}

void case_258()
#line 1066 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_259()
#line 1071 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_264()
#line 1086 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_265()
#line 1095 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_269()
#line 1116 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration> ());
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_270()
#line 1125 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-4+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-2+yyTop]);
		yyVal = new FunctionDefinition(
			new DeclarationSpecifiers(),
			d,
			null,
			(Block)yyVals[0+yyTop]);
	}

void case_271()
#line 1137 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_272()
#line 1143 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_273()
#line 1149 "CParser.jay"
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
   39,   39,   39,   39,   37,   37,   37,   40,   40,   40,
   40,   35,   35,   35,   35,   35,   41,   41,   42,   42,
   30,   32,   32,   45,   45,   45,   45,   44,   44,   44,
   44,   44,   44,   44,   44,   44,   44,   44,   44,   44,
   43,   43,   43,   43,   46,   46,   29,   29,   29,   47,
   47,   48,   48,   49,   49,   49,   49,    5,    5,   50,
   50,   50,   51,   51,   51,   51,   51,   51,   51,   51,
   51,   51,   51,   33,   33,   33,    6,    6,    6,    6,
   52,   53,   53,   54,   54,   55,   55,   55,   55,   55,
   55,   56,   57,   57,   62,   62,   63,   63,   38,   38,
   64,   64,   65,   65,   65,   65,   65,   65,   65,   68,
   66,   66,   66,   69,   69,   67,   58,   58,   59,   59,
   59,   59,   70,   70,   71,   71,   60,   60,   60,   60,
   60,   60,   61,   61,   61,   61,   61,    0,    0,   72,
   72,   72,   72,   73,   73,   76,   76,   76,   74,   74,
   75,   75,   75,   77,   77,   77,
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
    1,    2,    2,    2,    1,    1,    1,    2,    1,    2,
    1,    4,    5,    5,    6,    2,    1,    3,    1,    3,
    1,    2,    1,    1,    2,    3,    4,    1,    3,    5,
    4,    4,    6,    6,    5,    4,    3,    4,    4,    3,
    1,    2,    2,    3,    1,    2,    1,    1,    1,    1,
    3,    1,    3,    2,    4,    2,    1,    1,    2,    1,
    1,    2,    3,    2,    3,    3,    4,    3,    4,    2,
    3,    3,    4,    1,    3,    4,    1,    2,    3,    4,
    2,    1,    2,    3,    2,    1,    1,    1,    1,    1,
    1,    3,    2,    3,    1,    2,    1,    1,    2,    3,
    1,    2,    1,    1,    1,    2,    4,    5,    2,    5,
    2,    2,    2,    4,    3,    2,    1,    2,    5,    7,
    7,    6,    1,    2,    4,    3,    5,    7,    6,    7,
    6,    7,    3,    2,    2,    2,    3,    1,    2,    1,
    1,    1,    1,    4,    3,    1,    3,    4,    4,    5,
    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
    0,   95,   96,   97,   98,   99,  141,  168,  101,  102,
  103,  104,  107,  108,  105,  106,  167,  169,  100,  109,
  110,  111,  125,  126,  127,    0,  262,    0,  261,    0,
    0,    0,    0,    0,  112,  113,    0,  258,  260,  263,
    0,  115,  116,    0,    0,  259,  144,    0,    0,    0,
   81,    0,   91,    0,    0,    0,    0,  114,   84,   86,
   88,   90,    0,    0,  119,    0,    0,    0,    0,  137,
    0,    0,  165,  163,    0,  145,    0,   82,  274,    0,
    0,  275,  276,  271,    0,  265,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  219,  223,    0,    0,
    0,  221,  224,  225,    0,    0,  117,  267,    0,    0,
    0,    0,    0,  172,    0,  132,    0,    0,  149,  166,
  164,   92,    0,    0,    2,    3,    0,    0,    0,    4,
    5,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  213,    0,    0,   27,   28,   29,   30,  237,    7,
    0,    0,   78,    0,   33,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   63,  217,  218,  206,
  207,  208,  209,  210,  211,    0,  215,    1,    0,  194,
   94,  273,  264,  272,  160,    0,   17,    0,    0,  157,
    0,    0,    0,  146,    0,  231,  232,  233,  226,    0,
  229,    0,    0,  220,  222,  236,    0,    0,    0,  121,
    0,  268,  269,    0,    0,    0,    0,  176,    0,    0,
    0,   31,   80,  140,  134,  138,  133,    0,    0,    0,
   25,    0,   20,   21,    0,    0,    0,    0,    0,    0,
  254,  255,  256,    0,    0,    0,    0,    0,    0,   22,
   23,    0,  238,    0,   13,   14,    0,    0,    0,   66,
   67,   68,   69,   70,   71,   72,   73,   74,   75,   76,
   77,   65,    0,   24,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  214,  216,    0,    0,    0,  197,    0,
    0,  202,  159,    0,  158,    0,  156,  152,    0,  151,
    0,    0,  147,    0,    0,    0,  235,    0,  122,  123,
  124,  118,  190,    0,    0,  184,    0,    0,    0,    0,
    0,    0,  270,  171,  173,  135,  212,    0,    0,    0,
    0,    0,    0,    0,    0,  253,  257,    6,    0,  128,
  130,    0,    0,  179,   79,   12,    9,    0,    0,   11,
   64,   34,   35,   36,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  205,  195,    0,  198,  201,  203,   18,    0,    0,
  155,  150,    0,    0,  227,  234,  191,  183,  188,  185,
  175,  192,    0,  186,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   32,   10,    8,    0,  204,
  196,  199,    0,  153,  154,  228,    0,  193,  189,  187,
    0,    0,  247,    0,    0,    0,    0,    0,    0,   62,
  200,  230,    0,    0,    0,  242,    0,  243,    0,  251,
    0,  249,    0,   15,    0,  240,    0,    0,  241,  244,
  248,  252,  250,   16,    0,    0,    0,
  };
  protected static readonly short [] yyDgoto  = {            28,
  150,  151,  152,  186,  246,  298,  153,  154,  155,  156,
  157,  158,  159,  160,  161,  162,  163,  164,  165,  166,
  167,  273,  224,  168,   85,   52,   31,   32,   33,   34,
   53,  123,  181,   35,   36,   45,   37,   65,  211,  249,
   69,   70,   55,   56,   57,   75,  324,  113,  114,  325,
  219,  300,  301,  302,  169,  170,  171,  172,  173,  174,
  175,  176,  177,  101,  102,  103,  104,  201,  105,  447,
  448,   38,   39,   40,   87,   41,   88,
  };
  protected static readonly short [] yySindex = {         2140,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -79,    0, 2140,    0,   54,
 3369, 3369, 3369, 3369,    0,    0,  -43,    0,    0,    0,
  -34,    0,    0, -200,  -37,    0,    0,  -27,   30, -157,
    0,   12,    0,  873,   29,   19, -146,    0,    0,    0,
    0,    0, 3281,    5,    0,  -84, 1832,  116,  -16,    0,
 -200,  108,    0,    0,   30,    0,  -27,    0,    0,  242,
 1273,    0,    0,    0,   54,    0, 3220, 3369,   19,  678,
 1160,  -39,  145,  171,  191, 3369,    0,    0,   54,  -32,
 3314,    0,    0,    0,  186,  234,    0,    0,   20,  215,
  694,  302,  311,    0, 1702,    0, -100,   -6,    0,    0,
    0,    0,  312,  335,    0,    0, 1760, 1793, 1793,    0,
    0,  360,  376,  379,  470,  385,  216,  428,  452, 1298,
 1066,    0, 1702, 1702,    0,    0,    0,    0,    0,    0,
   34,  -29,    0,  326,    0, 1702,  375,  346,  267,   23,
  172,  476,  430,  406,  291,  -45,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  359,    0,    0, 1262,    0,
    0,    0,    0,    0,    0,   14,    0,  542,   88,    0,
 1362,  491, 1201,    0,  328,    0,    0,    0,    0,   54,
    0,  -40, 2080,    0,    0,    0, -203, -203, -203,    0,
  463,    0,    0, 1971, 1457,  526,  -10,    0,  200,  215,
 3341,    0,    0,    0,    0,    0,    0,  -55,  470, 1066,
    0, 1066,    0,    0, 1702, 1702, 1702,  264,  966,  531,
    0,    0,    0,  176,   35,  550, 1905, 1905,  293,    0,
    0, 1702,    0,  340,    0,    0, 1370, 1702,  354,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 1702,    0, 1702, 1702, 1702, 1702, 1702, 1702,
 1702, 1702, 1702, 1702, 1702, 1702, 1702, 1702, 1702, 1702,
 1702, 1702, 1702,    0,    0, 1702,  355,   74,    0, 1273,
  378,    0,    0, 1702,    0, 1173,    0,    0, 1702,    0,
 1551,  499,    0,  -20,  553,  561,    0,  582,    0,    0,
    0,    0,    0,  583,  584,    0, 1559,  535, 1702,  200,
 2113, 1598,    0,    0,    0,    0,    0,  585,  586,  342,
  382,  388,  589, 1617, 1617,    0,    0,    0, 1625,    0,
    0, 2053,  288,    0,    0,    0,    0,  393,   22,    0,
    0,    0,    0,    0,  375,  375,  346,  346,  267,  267,
  267,  267,   23,   23,  172,  476,  430,  406,  291,   31,
  537,    0,    0,  118,    0,    0,    0,    0,  539,  540,
    0,    0,  572, 1655,    0,    0,    0,    0,    0,    0,
    0,    0,  593,    0, 1663,  543,  512,  512,  470,  514,
  470, 1702, 1689, 1720, 1262,    0,    0,    0, 1702,    0,
    0,    0, 1273,    0,    0,    0,  579,    0,    0,    0,
  317,  -22,    0,  394,  470,  445,  470,  453,  187,    0,
    0,    0,  470, 1702,  590,    0,   -3,    0,  581,    0,
  470,    0,  470,    0,  781,    0,  591,  450,    0,    0,
    0,    0,    0,    0,  450,  450,  450,
  };
  protected static readonly short [] yyRindex = {            0,
 2026,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  307,  840, 1350, 1733,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1865,    0,    0,    0,  -18,    0,
    0,    0,    0,  280,    0,  774,  554,    0,    0,    0,
    0,    0,    0, 1944,    0,    0,    0,  227,    0,    0,
    0,    0,    0,    0,  146,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  832,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  482,    0,  600,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    6, 3279,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2448,    0, 2516,    0,    0, 2544, 2791, 2838, 2950,
   73, 3171, 3183, 1291,   82,    2,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  484,  533,    0,  534,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  316,  329,  601,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    6,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  538,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  602,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2553, 2733, 2801, 2828, 2886, 2896,
 2923, 2991, 3076, 3085, 3144, 3173, 3212, 2020, 2983,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2476,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2421,    0,    0,    0,
  151,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   10,   21,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -93,    0,  390,  266,  263,  -81,  -23, -103,    0,
  -62,  226,   77,  274,  391,  392,  400,  401,  399,    0,
 -113,    0, -275,  125,    1,  -48,    0,  158,  647,    0,
  616,  100, -105,    0,    0,  309,    0,  -52,    0,  333,
  623,  -90,  -44,  -35,  -11,  -42,  -53,    0,  474,  -95,
 -172, -348,    0,  396, -128,    0,   75,   83,    0,    0,
    0, -381, -173,    0,  597,    0,    0,    0,    0,    0,
  252,  672,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           180,
   30,  223,  295,   77,   74,   67,  238,  203,  187,  192,
  257,  107,   48,  112,   49,  218,  259,  293,   78,   89,
  381,  161,  161,   77,  225,  161,  226,  117,   30,  214,
  121,   59,   60,   61,   62,  423,  188,  228,   78,  250,
  251,  109,   61,   44,  330,   61,  244,  245,  193,   93,
  202,  100,  274,   42,  303,   77,   68,  304,   90,   61,
   61,  258,  106,   99,   93,  252,  217,  111,   48,  336,
   78,   49,  161,  299,  252,  348,  466,  252,  252,   63,
  215,   43,  284,  467,  285,   71,  195,  251,  419,  100,
  111,  222,  253,   48,   61,   49,  200,  180,   50,   76,
  337,   99,  446,  231,  233,  234,  423,  161,  116,   91,
   51,  312,   51,   51,  418,   50,   51,  384,  227,  222,
  222,  459,   59,   92,   29,   59,   61,   63,   86,   54,
   51,   51,  222,  328,  246,   51,  245,  226,  245,   59,
   59,  340,  341,  342,   59,  245,  306,   72,  119,  318,
  148,  314,   29,  354,   50,  143,   68,  141,  322,  144,
  145,  183,  146,  297,  359,   51,   51,  222,  457,  217,
  355,  362,  363,  364,   59,  187,  115,   42,   84,   50,
  330,   89,  223,  239,  213,  162,  162,   98,  239,  162,
  239,  361,  239,  239,  385,  239,   51,   51,  383,  380,
  108,   68,  196,  111,  353,   43,   59,  251,  296,  239,
  216,  182,  184,   42,  111,  365,  366,  194,  180,  252,
  199,  111,  388,  251,  389,   98,  292,  390,  197,   47,
  455,  254,  255,  256,  347,   66,  162,   92,  161,  331,
  179,   43,  421,  147,  206,  416,   47,  401,  198,  316,
  406,  222,  222,  222,  222,  222,  222,  222,  222,  222,
  222,  222,  222,  222,  222,  222,  222,  222,  222,  393,
  139,  162,  222,  239,  148,  239,  239,  403,  422,  143,
  431,  141,  433,  144,  145,   47,  146,  222,  282,  283,
  332,   61,  295,  295,  333,   93,  444,  445,  247,  315,
  149,  251,  180,  222,  212,  440,  450,  353,  452,  299,
   47,  454,  180,   72,  456,  444,  445,  441,  434,  436,
  438,  345,  462,   93,  463,  222,    8,  352,  246,  246,
  223,  111,  352,  180,   49,   17,   18,   80,   93,  245,
  245,  180,  220,   51,   51,   64,   83,   83,   83,  422,
   83,  139,  111,   59,  221,  129,  129,  129,  369,  370,
  371,  372,   51,  344,   80,   83,  142,  147,  131,  131,
  131,   59,   81,  180,  178,  125,  126,  127,  215,  128,
  129,  222,  409,  215,    8,  252,  272,  247,  278,  247,
  279,  148,  229,   17,   18,  222,  143,   83,  141,  235,
  144,  145,  162,  146,  247,  247,  129,  239,  239,  239,
  239,  277,  239,  239,  210,  236,  275,  149,  237,  131,
  222,  276,  410,  297,  239,  252,  413,  414,  411,  130,
  131,  252,   83,  417,  449,  239,  304,  252,  386,  286,
  287,  239,  239,  239,  239,  239,  239,  239,  239,  239,
  239,  239,  239,  239,  239,  239,  239,  239,  239,  239,
  239,  239,  239,  239,  239,  239,  239,  239,  296,  239,
  239,  239,  240,  239,  239,  239,  239,  239,  239,  239,
  239,   80,  148,  294,  147,  451,  241,  143,  252,  141,
   42,  144,  145,  453,  146,  338,  252,  339,  124,  125,
  126,  127,  148,  128,  129,  367,  368,  143,  149,  141,
  242,  144,  145,  288,  146,  319,  320,  321,   43,  207,
  208,  209,  177,  289,  174,  177,   58,  174,  149,  290,
  280,  281,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,  130,  131,   23,   24,   25,   26,  373,
  374,  291,  132,   83,  133,  134,  135,  136,  137,  138,
  139,  140,   80,  180,  181,  147,  180,  181,  182,  350,
  351,  182,  305,  308,  313,   63,  329,  343,  148,  346,
  349,  392,   80,  148,  148,  147,  356,  148,  260,  261,
  262,  263,  264,  265,  266,  267,  268,  269,  270,  271,
  360,  382,  148,  394,  148,  124,  125,  126,  127,  395,
  128,  129,  396,  397,  398,  407,  408,  400,  412,  420,
  426,  424,  425,  428,  415,  430,  432,  442,  443,  461,
  170,  178,  180,   58,  148,  148,  358,  458,  465,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
  130,  131,   23,   24,   25,   26,  148,  439,  375,  132,
  376,  133,  134,  135,  136,  137,  138,  139,  140,  377,
  379,  378,  122,  118,  335,   73,  387,  205,  460,   46,
    0,    0,    0,    0,    0,    0,  124,  125,  126,  127,
  148,  128,  129,    0,    0,  143,    0,  141,  185,  144,
  145,  120,  146,    0,    0,    0,  124,  125,  126,  127,
    0,  128,  129,  214,   58,   49,    0,   73,    0,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,  130,  131,   23,   24,   25,   26,    0,    0,    0,
  132,    0,  133,  134,  135,  136,  137,  138,  139,  140,
    0,  130,  131,    0,  215,    0,    0,  248,    0,    0,
  132,    0,  133,  134,  135,  136,  137,  138,  139,  140,
    0,    0,    0,  147,    0,    0,    0,    0,  143,    0,
    0,    0,    0,  148,  143,    0,    0,  143,  143,   50,
  141,    0,  144,  145,    0,  146,  297,    0,    0,    0,
    0,    0,  143,    0,  143,   73,    0,    0,  148,  120,
    0,    0,    0,  148,  148,  148,  148,  148,  148,  148,
  148,  148,  148,  148,  148,  148,  148,  148,  148,  148,
  148,  148,  148,  148,  148,  143,  142,  148,  148,  148,
  148,  296,  142,    0,    0,  142,  248,    0,  248,   85,
   85,   85,    0,   85,  148,    0,    0,    0,    0,    0,
  142,    0,  142,  248,  248,    0,  143,    0,   85,    0,
    0,    0,    0,  179,    0,  464,  147,   82,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  142,    0,    0,    0,    0,    0,    0,
   85,    0,    0,   81,  178,  125,  126,  127,    0,  128,
  129,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   47,    0,  120,    0,  142,    0,    0,    0,    0,    0,
    0,    0,   58,    0,   83,   85,    0,    0,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,  130,
  131,   23,   24,   25,   26,   80,    0,    0,  148,    0,
    0,    0,    0,  143,    0,  141,    0,  144,  145,    0,
  146,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  149,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  178,  125,  126,
  127,    0,  128,  129,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  143,    0,
    0,    0,    0,  143,  143,  143,  143,  143,  143,  143,
  143,  143,  143,  143,  143,  143,  143,  143,  143,  143,
  143,  143,  143,  143,  143,    0,    0,  143,  143,  143,
  143,  147,  130,  131,    0,    0,   85,    0,  148,    0,
    0,    0,    0,  143,  143,  141,    0,  144,  145,    0,
  146,    0,    0,    0,    0,    0,  142,    0,    0,    0,
    0,  142,  142,  142,  142,  142,  142,  142,  142,  142,
  142,  142,  142,  142,  142,  142,  142,  142,  142,  142,
  142,  142,  142,    0,    0,  142,  142,  142,  142,    0,
    0,    0,    0,    0,    0,    0,    0,   58,    0,    0,
    0,    0,  142,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,    0,    0,   23,   24,   25,   26,
    0,  147,  148,    0,    0,    0,    0,  143,    0,  141,
    0,  191,  145,   79,  146,  148,    0,    0,    0,    0,
  143,    0,  141,    0,  144,  145,    0,  146,    0,    0,
    0,    0,  178,  125,  126,  127,    0,  128,  129,    0,
    0,    0,    0,  148,    0,    0,    0,    0,  143,    0,
  141,    0,  311,  145,    0,  146,    0,    0,    0,    0,
   58,    0,  190,    0,    0,    0,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,  130,  131,   23,
   24,   25,   26,    0,    0,  147,    0,    0,    0,    0,
    0,    0,    0,  310,  148,    0,    0,    0,  147,  143,
    0,  141,    0,  144,  145,  148,  146,  297,    0,    0,
  143,    0,  141,    0,  144,  145,    0,  146,    0,    0,
    0,    0,  178,  125,  126,  127,  147,  128,  129,    0,
  148,   57,    0,    0,   57,  143,    0,  141,    0,  144,
  145,    0,  146,    0,    0,    0,    0,    0,   57,   57,
   58,    0,  296,   57,    0,    0,  243,    0,    0,    0,
    0,    0,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,  130,  131,   23,
   24,   25,   26,   57,  179,    0,    0,  147,    0,   87,
   87,   87,    0,   87,  148,  179,    0,    0,  147,  143,
    0,  141,  148,  144,  145,    0,  146,  143,   87,  141,
  357,  144,  145,    0,  146,   57,  178,  125,  126,  127,
    0,  128,  129,  147,    0,    0,    0,    0,    0,  178,
  125,  126,  127,    0,  128,  129,    0,    0,    0,    0,
   87,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  189,    0,  307,    0,    8,  178,  125,  126,
  127,    0,  128,  129,    0,   17,   18,    0,    0,    8,
    0,  130,  131,    0,    0,   87,    0,    0,   17,   18,
    0,    0,    0,    0,  130,  131,    0,  147,    0,  148,
    0,    0,    0,  309,  143,  147,  141,    8,  327,  145,
    0,  146,    0,    0,    0,    0,   17,   18,    0,    0,
    0,    0,  130,  131,    0,    0,    0,    0,  178,  125,
  126,  127,    0,  128,  129,    0,    0,    0,    0,  178,
  125,  126,  127,    0,  128,  129,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  326,
    0,    0,    0,    0,  178,  125,  126,  127,    0,  128,
  129,   57,   57,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  130,  131,    0,    0,    0,    0,    0,
   57,    0,  147,  148,  130,  131,    0,    0,  143,    0,
  141,  148,  144,  145,    0,  146,  143,    0,  141,    0,
  144,  145,    0,  146,    0,    0,   87,    0,    0,  130,
  131,    0,    0,    0,    0,    0,    0,    0,  178,  125,
  126,  127,    0,  128,  129,    0,  178,  125,  126,  127,
  148,  128,  129,    0,    0,  143,    0,  141,    0,  405,
  145,    0,  146,  391,    0,    0,    0,    0,    0,  148,
    0,  399,    0,    0,  143,    0,  141,  148,  144,  145,
    0,  146,  143,    0,  141,    0,  144,  145,    0,  146,
    0,    0,    0,  130,  131,  149,  147,    0,    0,    0,
    0,  130,  131,    0,  147,    0,    0,  148,    0,    0,
  404,    0,  143,    0,  141,  148,  144,  145,    0,  146,
  143,    0,  141,    0,  144,  145,    0,  146,    0,    0,
    0,    0,    0,  178,  125,  126,  127,    0,  128,  129,
    0,  148,    0,  147,    0,    0,  143,    0,  141,  435,
  144,  145,    0,  146,  148,    0,    0,    0,    0,  143,
    0,  141,  147,  144,  145,    0,  146,  415,    0,    0,
  147,    0,  148,    0,    0,  429,    0,  143,    0,  141,
  437,  144,  145,    0,  146,    0,    0,    0,  130,  131,
    0,    0,   89,   89,   89,    0,   89,  179,    0,    0,
  147,    0,    0,    0,    0,    0,    0,    0,  147,    0,
    0,   89,  148,    0,    0,    0,    0,  143,    0,  230,
    0,  144,  145,    0,  146,    0,    0,  178,  125,  126,
  127,    0,  128,  129,  147,  178,  125,  126,  127,    0,
  128,  129,    0,   89,    0,  148,    0,  147,    0,    0,
  143,    0,  232,    0,  144,  145,    0,  146,    0,    0,
    0,    0,    0,    0,    0,  147,    0,    0,    0,    0,
    0,    0,    0,    0,  178,  125,  126,  127,   89,  128,
  129,    0,  130,  131,    0,    0,    0,    0,    0,    0,
  130,  131,  110,  178,  125,  126,  127,    0,  128,  129,
    0,  178,  125,  126,  127,  147,  128,  129,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  136,  136,  136,    0,  136,  130,
  131,  178,  427,  126,  127,    0,  128,  129,  147,  178,
  125,  126,  127,  136,  128,  129,    0,    0,  130,  131,
    0,    0,    0,    0,    0,    0,  130,  131,    0,    0,
    0,    0,    0,    0,    0,  178,  125,  126,  127,    0,
  128,  129,    0,    0,    0,  136,    0,    0,  178,  125,
  126,  127,    0,  128,  129,    0,  130,  131,    0,    0,
    0,    0,    0,    0,  130,  131,  178,  125,  126,  127,
    0,  128,  129,  120,  120,  120,    0,  120,    0,   89,
  136,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  130,  131,  120,    0,    0,    0,    0,    0,    0,    0,
  214,  323,   49,  130,  131,    0,  178,  125,  126,  127,
    0,  128,  129,    0,    0,    0,    0,    0,    0,    0,
    0,  130,  131,    0,  120,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  178,
  125,  126,  127,    0,  128,  129,    0,    0,    0,    0,
   58,  215,    0,   58,    0,  114,    0,  114,    0,  120,
    0,  130,  131,    0,    0,    0,    0,   58,   58,    0,
    0,    0,   58,    0,  114,    0,    0,    0,    0,    0,
    0,    0,  352,  323,   49,    0,   50,    0,    0,    0,
    0,    0,    0,    0,  130,  131,    0,    0,    0,    0,
    0,    0,   58,    0,    0,    0,   58,    0,    0,    0,
  317,  136,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,  215,   58,   23,   24,   25,   26,  136,
    0,  114,    0,  402,    0,  136,  136,  136,  136,  136,
  136,  136,  136,  136,  136,  136,  136,  136,  136,  136,
  136,  136,  136,  136,  136,  136,    0,    0,  136,  136,
  136,  136,    0,    0,    0,    0,    0,    0,    0,   58,
    0,    0,    0,    0,    0,    0,    0,    0,   27,    0,
  120,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,    0,    0,   23,   24,
   25,   26,    0,    0,    0,    0,    0,   47,  120,    0,
    0,    0,    0,    0,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,  120,  120,  120,  120,  120,
  120,  120,  120,  120,  120,   58,    0,  120,  120,  120,
  120,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,  114,    0,   23,   24,   25,   26,    0,    0,
   58,   58,    0,    0,    0,  266,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   58,
  114,    0,    0,    0,    0,    0,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,  114,  114,  114,
  114,  114,  114,  114,  114,  114,  114,   58,    0,  114,
  114,  114,  114,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   58,    0,   23,   24,   25,   26,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,   58,    0,    0,
    0,    0,    0,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,    1,    0,   23,   24,   25,   26,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    2,    2,    0,
    2,    0,    2,    2,    2,    2,    2,    2,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    2,    2,    2,    2,   19,   19,    0,    0,   19,   19,
   19,   19,   19,    0,   19,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   19,   19,   19,   19,   19,
   19,    2,   26,   26,    2,    0,   26,   26,   26,   26,
   26,    0,   26,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   26,   26,   26,   26,   26,   26,    0,
   19,   19,    0,    0,    2,    0,    0,    0,    0,    0,
    0,    0,   31,   31,    0,    0,   31,   31,   31,   31,
   31,    0,   31,    0,    0,    0,    0,    0,   26,   26,
    0,   19,   19,   31,   31,   31,    0,   31,   31,    0,
    0,   37,    0,    0,   37,    0,   37,   37,   37,    0,
   38,    0,    0,   38,    0,   38,   38,   38,    0,   26,
   26,   37,   37,   37,    0,   37,   37,    0,   31,   31,
   38,   38,   38,    0,   38,   38,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   37,   37,    0,   31,
   31,    0,    0,    0,    0,   38,   38,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   37,   37,    0,
    0,    0,    0,    0,    0,    0,   38,   38,    0,    0,
    0,    2,    2,    2,    2,    2,    2,    2,    2,    2,
    0,    2,    2,    2,    2,    2,    2,    2,    2,    2,
    2,    2,    2,    2,    2,    0,    0,    0,    0,    0,
    2,   19,   19,   19,   19,   19,   19,    0,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,   19,    0,    0,    0,    0,    0,   19,    0,   26,
   26,   26,   26,   26,   26,    0,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,   26,
    0,    0,    0,    0,    0,   26,    0,    0,    0,    0,
   39,    0,    0,   39,    0,   39,   39,   39,    0,   31,
   31,   31,   31,   31,   31,    0,   31,   31,    0,    0,
   39,   39,   39,    0,   39,   39,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   31,    0,   37,   37,   37,
   37,   37,   37,    0,   37,   37,   38,   38,   38,   38,
   38,   38,    0,   38,   38,   39,   39,    0,   40,    0,
    0,   40,    0,   37,   40,    0,    0,    0,   41,    0,
    0,   41,   38,    0,   41,    0,    0,    0,   40,   40,
   40,    0,   40,   40,    0,    0,   39,   39,   41,   41,
   41,    0,   41,   41,    0,   42,    0,    0,   42,    0,
    0,   42,    0,    0,    0,   43,    0,    0,   43,    0,
    0,   43,    0,   40,   40,   42,   42,   42,    0,   42,
   42,    0,    0,   41,   41,   43,   43,   43,    0,   43,
   43,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   40,   40,    0,    0,    0,    0,
   42,   42,    0,   46,   41,   41,   46,    0,    0,   46,
   43,   43,    0,   47,    0,    0,   47,    0,    0,   47,
    0,    0,    0,   46,   46,   46,    0,   46,   46,    0,
    0,   42,   42,   47,   47,   47,    0,   47,   47,    0,
   44,   43,   43,   44,    0,    0,   44,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   46,   46,
   44,   44,   44,    0,   44,   44,    0,   48,   47,   47,
   48,    0,    0,   48,    0,    0,   39,   39,   39,   39,
   39,   39,    0,   39,   39,    0,    0,   48,   48,   46,
   46,    0,   48,    0,    0,   44,   44,    0,    0,   47,
   47,    0,   39,   60,    0,    0,   60,    0,   45,    0,
    0,   45,    0,    0,   45,    0,    0,    0,    0,    0,
   60,   60,   48,   48,    0,   60,   44,   44,   45,   45,
   45,    0,   45,   45,   40,   40,   40,   40,   40,   40,
    0,   40,   40,    0,   41,   41,   41,   41,   41,   41,
    0,   41,   41,   48,   48,   60,    0,    0,    0,    0,
   40,    0,    0,   45,   45,    0,    0,    0,    0,    0,
   41,   42,   42,   42,   42,   42,   42,    0,   42,   42,
    0,    0,    0,   43,   43,   43,   43,   60,   43,   43,
    0,    0,    0,   49,   45,   45,   49,   42,    0,   49,
    0,    0,   50,    0,    0,   50,    0,   43,   50,    0,
    0,    0,    0,   49,   49,    0,    0,    0,   49,    0,
    0,    0,   50,   50,    0,    0,    0,   50,    0,    0,
    0,   46,   46,   46,   46,    0,   46,   46,    0,    0,
    0,   47,   47,   47,   47,    0,   47,   47,   49,   49,
    0,    0,    0,    0,    0,   46,    0,   50,   50,    0,
    0,   52,    0,    0,   52,   47,    0,   52,   44,   44,
   44,   44,    0,   44,   44,    0,    0,    0,    0,   49,
   49,   52,   52,    0,    0,    0,   52,    0,   50,   50,
    0,   53,   44,   54,   53,    0,   54,   48,   48,    0,
   48,   48,    0,   55,    0,    0,   55,    0,   53,   53,
   54,   54,    0,   53,    0,   54,   52,   52,    0,   48,
   55,   55,    0,    0,    0,   55,    0,    0,    0,    0,
    0,    0,   56,    0,   60,   56,   45,   45,   45,   45,
    0,   45,   45,   53,   53,   54,   54,   52,   52,   56,
   56,    0,   60,    0,   56,   55,    0,    0,    0,    0,
   45,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   53,   53,   54,   54,    0,    0,
    0,    0,    0,    0,   56,    0,   55,   55,    0,    0,
    0,    0,    0,    0,    0,    1,    1,    0,    1,    0,
    1,    1,    1,    1,    1,    1,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   56,   56,    1,    1,    1,
    1,    1,   80,   49,   49,    0,   49,   49,    0,    0,
    0,    0,   50,   50,    0,   50,   50,    0,    0,    0,
    0,    0,    0,    0,    0,   49,    0,    0,    0,    1,
    0,    0,    1,    0,   50,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    1,    0,    0,   97,   50,    0,    0,    0,
    0,    0,    0,    0,   52,   52,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   52,    0,    0,    0,    0,  204,   50,
    0,   53,   53,   54,   54,    0,    0,    0,    0,    0,
    0,    0,    0,   55,   55,    0,    0,    0,    0,    0,
   53,    0,   54,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   55,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   56,   56,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   56,    0,    0,   58,    0,    0,    0,    0,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,   47,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    0,    0,   58,   93,   94,   95,   96,
   47,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,   58,   93,
   94,   95,   96,    0,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   58,    0,   23,   24,   25,
   26,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,   58,   23,   24,   25,   26,  334,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,
  };
  protected static readonly short [] yyCheck = {            81,
    0,  115,  176,   44,   49,   40,  135,   40,   90,   91,
   40,   64,   40,   67,   42,  111,   46,   63,   59,   55,
  296,   40,   41,   44,  125,   44,  117,   44,   28,   40,
   75,   31,   32,   33,   34,  384,   90,   44,   59,  143,
  144,  126,   41,  123,  217,   44,  140,  141,   91,   44,
   99,   63,  156,  257,   41,   44,  257,   44,   40,   58,
   59,   91,   58,   63,   59,   44,  111,   67,   40,  125,
   59,   42,   91,  179,   44,   41,  458,   44,   44,  123,
   91,  285,   60,  465,   62,  123,  126,  191,   58,  101,
   90,  115,   59,   40,   93,   42,   96,  179,  126,  257,
  229,  101,  125,  127,  128,  129,  455,  126,  125,   91,
   38,  193,   59,   41,   93,  126,   44,   44,  125,  143,
  144,  125,   41,  270,    0,   44,  125,  123,   54,   30,
   58,   59,  156,  215,  125,   63,  230,  228,  232,   58,
   59,  235,  236,  237,   63,  125,  189,   48,   41,  203,
   33,  200,   28,  249,  126,   38,  257,   40,  211,   42,
   43,   87,   45,   46,  258,   93,   94,  191,  444,  214,
  252,  275,  276,  277,   93,  257,   61,  257,   54,  126,
  353,  217,  296,   33,  110,   40,   41,   63,   38,   44,
   40,  273,   42,   43,  300,   45,  124,  125,  125,  293,
  285,  257,   58,  203,  249,  285,  125,  311,   91,   59,
  111,   87,   88,  257,  214,  278,  279,  257,  300,   44,
   96,  221,  304,  327,  306,  101,  272,  309,   58,  257,
   44,  261,  262,  263,   59,  270,   91,  270,  257,   40,
  123,  285,  125,  126,   59,  349,  257,  329,   58,  290,
  332,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  290,
   44,  126,  296,  123,   33,  125,  126,  331,  384,   38,
  409,   40,  411,   42,   43,  257,   45,  311,  266,  267,
   91,  290,  466,  467,  220,  290,  319,  320,  141,  200,
   59,  405,  384,  327,  285,  419,  435,  352,  437,  415,
  257,  125,  394,  214,  443,  319,  320,  423,  412,  413,
  414,  239,  451,   44,  453,  349,  297,   40,  319,  320,
  444,  331,   40,  415,   42,  306,  307,  123,   59,  319,
  320,  423,   41,  271,  272,   37,   40,   41,   42,  455,
   44,  125,  352,  272,   44,   40,   41,   42,  282,  283,
  284,  285,  290,  239,  123,   59,  125,  126,   40,   41,
   42,  290,   61,  455,  257,  258,  259,  260,   91,  262,
  263,  405,   41,   91,  297,   44,   61,  230,   43,  232,
   45,   33,   58,  306,  307,  419,   38,   91,   40,   40,
   42,   43,  257,   45,  247,  248,   91,  257,  258,  259,
  260,   37,  262,  263,  106,   40,   42,   59,   40,   91,
  444,   47,   41,   46,   40,   44,  344,  345,   41,  312,
  313,   44,  126,   41,   41,  285,   44,   44,   61,  268,
  269,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  313,  314,  315,  316,  317,   91,  319,
  320,  321,  257,  323,  324,  325,  326,  327,  328,  329,
  330,  123,   33,  125,  126,   41,   59,   38,   44,   40,
  257,   42,   43,   41,   45,  230,   44,  232,  257,  258,
  259,  260,   33,  262,  263,  280,  281,   38,   59,   40,
   59,   42,   43,   38,   45,  207,  208,  209,  285,  286,
  287,  288,   41,   94,   41,   44,  285,   44,   59,  124,
  264,  265,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,  312,  313,  314,  315,  316,  317,  286,
  287,  271,  321,  257,  323,  324,  325,  326,  327,  328,
  329,  330,  123,   41,   41,  126,   44,   44,   41,  247,
  248,   44,   41,   93,  257,  123,   61,  324,   35,   59,
   41,   93,  123,   40,   41,  126,  257,   44,  273,  274,
  275,  276,  277,  278,  279,  280,  281,  282,  283,  284,
  257,  257,   59,   61,   61,  257,  258,  259,  260,   59,
  262,  263,   41,   41,   41,   41,   41,   93,   40,   93,
   59,   93,   93,   41,  123,   93,  123,   59,  322,   59,
   41,   41,   41,  285,   91,   92,  257,   58,   58,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,  316,  317,  123,  415,  288,  321,
  289,  323,  324,  325,  326,  327,  328,  329,  330,  290,
  292,  291,   77,   71,  221,   49,  301,  101,  447,   28,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   33,  262,  263,   -1,   -1,   38,   -1,   40,   41,   42,
   43,   75,   45,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   40,  285,   42,   -1,   91,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  313,  314,  315,  316,  317,   -1,   -1,   -1,
  321,   -1,  323,  324,  325,  326,  327,  328,  329,  330,
   -1,  312,  313,   -1,   91,   -1,   -1,  141,   -1,   -1,
  321,   -1,  323,  324,  325,  326,  327,  328,  329,  330,
   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,   35,   -1,
   -1,   -1,   -1,   33,   41,   -1,   -1,   44,   38,  126,
   40,   -1,   42,   43,   -1,   45,   46,   -1,   -1,   -1,
   -1,   -1,   59,   -1,   61,  189,   -1,   -1,  285,  193,
   -1,   -1,   -1,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,   92,   35,  314,  315,  316,
  317,   91,   41,   -1,   -1,   44,  230,   -1,  232,   40,
   41,   42,   -1,   44,  331,   -1,   -1,   -1,   -1,   -1,
   59,   -1,   61,  247,  248,   -1,  123,   -1,   59,   -1,
   -1,   -1,   -1,  123,   -1,  125,  126,   35,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   92,   -1,   -1,   -1,   -1,   -1,   -1,
   91,   -1,   -1,   61,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  257,   -1,  306,   -1,  123,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   92,  126,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  313,  314,  315,  316,  317,  123,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,
   -1,   -1,   -1,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,   -1,   -1,  314,  315,  316,
  317,  126,  312,  313,   -1,   -1,  257,   -1,   33,   -1,
   -1,   -1,   -1,   38,  331,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,   -1,
   -1,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,   -1,   -1,  314,  315,  316,  317,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,
   -1,   -1,  331,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,   -1,   -1,  314,  315,  316,  317,
   -1,  126,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,  331,   45,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
  285,   -1,   93,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,  316,  317,   -1,   -1,  126,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   33,   -1,   -1,   -1,  126,   38,
   -1,   40,   -1,   42,   43,   33,   45,   46,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,  126,  262,  263,   -1,
   33,   41,   -1,   -1,   44,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   58,   59,
  285,   -1,   91,   63,   -1,   -1,   59,   -1,   -1,   -1,
   -1,   -1,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,  316,  317,   93,  123,   -1,   -1,  126,   -1,   40,
   41,   42,   -1,   44,   33,  123,   -1,   -1,  126,   38,
   -1,   40,   33,   42,   43,   -1,   45,   38,   59,   40,
   41,   42,   43,   -1,   45,  125,  257,  258,  259,  260,
   -1,  262,  263,  126,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   91,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  293,   -1,   93,   -1,  297,  257,  258,  259,
  260,   -1,  262,  263,   -1,  306,  307,   -1,   -1,  297,
   -1,  312,  313,   -1,   -1,  126,   -1,   -1,  306,  307,
   -1,   -1,   -1,   -1,  312,  313,   -1,  126,   -1,   33,
   -1,   -1,   -1,  293,   38,  126,   40,  297,   42,   43,
   -1,   45,   -1,   -1,   -1,   -1,  306,  307,   -1,   -1,
   -1,   -1,  312,  313,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  312,  313,   -1,   -1,   -1,   -1,   -1,
  290,   -1,  126,   33,  312,  313,   -1,   -1,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  257,   -1,   -1,  312,
  313,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,  257,  258,  259,  260,
   33,  262,  263,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   93,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   93,   -1,   -1,   38,   -1,   40,   33,   42,   43,
   -1,   45,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   -1,   -1,   -1,  312,  313,   59,  126,   -1,   -1,   -1,
   -1,  312,  313,   -1,  126,   -1,   -1,   33,   -1,   -1,
   93,   -1,   38,   -1,   40,   33,   42,   43,   -1,   45,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   33,   -1,  126,   -1,   -1,   38,   -1,   40,   41,
   42,   43,   -1,   45,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,  126,   42,   43,   -1,   45,  123,   -1,   -1,
  126,   -1,   33,   -1,   -1,   93,   -1,   38,   -1,   40,
   41,   42,   43,   -1,   45,   -1,   -1,   -1,  312,  313,
   -1,   -1,   40,   41,   42,   -1,   44,  123,   -1,   -1,
  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,
   -1,   59,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,  126,  257,  258,  259,  260,   -1,
  262,  263,   -1,   91,   -1,   33,   -1,  126,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,  126,  262,
  263,   -1,  312,  313,   -1,   -1,   -1,   -1,   -1,   -1,
  312,  313,   41,  257,  258,  259,  260,   -1,  262,  263,
   -1,  257,  258,  259,  260,  126,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   40,   41,   42,   -1,   44,  312,
  313,  257,  258,  259,  260,   -1,  262,  263,  126,  257,
  258,  259,  260,   59,  262,  263,   -1,   -1,  312,  313,
   -1,   -1,   -1,   -1,   -1,   -1,  312,  313,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   91,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,  312,  313,   -1,   -1,
   -1,   -1,   -1,   -1,  312,  313,  257,  258,  259,  260,
   -1,  262,  263,   40,   41,   42,   -1,   44,   -1,  257,
  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  312,  313,   59,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   40,   41,   42,  312,  313,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  312,  313,   -1,   91,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   41,   91,   -1,   44,   -1,   40,   -1,   42,   -1,  126,
   -1,  312,  313,   -1,   -1,   -1,   -1,   58,   59,   -1,
   -1,   -1,   63,   -1,   59,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   40,   41,   42,   -1,  126,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  312,  313,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,  285,   -1,   -1,   -1,
   41,  257,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,   91,  125,  314,  315,  316,  317,  285,
   -1,  126,   -1,   41,   -1,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,   -1,   -1,  314,  315,
  316,  317,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   -1,
  257,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,   -1,   -1,  314,  315,
  316,  317,   -1,   -1,   -1,   -1,   -1,  257,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  285,   -1,  314,  315,  316,
  317,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  257,   -1,  314,  315,  316,  317,   -1,   -1,
  271,  272,   -1,   -1,   -1,  270,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  290,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  285,   -1,  314,
  315,  316,  317,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  285,   -1,  314,  315,  316,  317,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   -1,   -1,  314,  315,  316,  317,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  285,   -1,  314,  315,  316,  317,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   -1,   -1,  314,  315,  316,  317,   37,   38,   -1,
   40,   -1,   42,   43,   44,   45,   46,   47,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   60,   61,   62,   63,   37,   38,   -1,   -1,   41,   42,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   61,   62,
   63,   91,   37,   38,   94,   -1,   41,   42,   43,   44,
   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,   -1,
   93,   94,   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   37,   38,   -1,   -1,   41,   42,   43,   44,
   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   93,   94,
   -1,  124,  125,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,  124,
  125,   58,   59,   60,   -1,   62,   63,   -1,   93,   94,
   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,
  125,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,  261,  262,  263,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  282,  283,  284,   -1,   -1,   -1,   -1,   -1,
  290,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
  273,  274,  275,  276,  277,  278,  279,  280,  281,  282,
  283,  284,   -1,   -1,   -1,   -1,   -1,  290,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,  273,  274,
  275,  276,  277,  278,  279,  280,  281,  282,  283,  284,
   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,
   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  290,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,   93,   94,   -1,   38,   -1,
   -1,   41,   -1,  290,   44,   -1,   -1,   -1,   38,   -1,
   -1,   41,  290,   -1,   44,   -1,   -1,   -1,   58,   59,
   60,   -1,   62,   63,   -1,   -1,  124,  125,   58,   59,
   60,   -1,   62,   63,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   93,   94,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   93,   94,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
   93,   94,   -1,   38,  124,  125,   41,   -1,   -1,   44,
   93,   94,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,
   -1,  124,  125,   58,   59,   60,   -1,   62,   63,   -1,
   38,  124,  125,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,
   58,   59,   60,   -1,   62,   63,   -1,   38,   93,   94,
   41,   -1,   -1,   44,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   58,   59,  124,
  125,   -1,   63,   -1,   -1,   93,   94,   -1,   -1,  124,
  125,   -1,  290,   41,   -1,   -1,   44,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   58,   59,   93,   94,   -1,   63,  124,  125,   58,   59,
   60,   -1,   62,   63,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   -1,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,  124,  125,   93,   -1,   -1,   -1,   -1,
  290,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,
  290,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   -1,   -1,   -1,  266,  267,  268,  269,  125,  271,  272,
   -1,   -1,   -1,   38,  124,  125,   41,  290,   -1,   44,
   -1,   -1,   38,   -1,   -1,   41,   -1,  290,   44,   -1,
   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,   63,   -1,
   -1,   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,
   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   -1,  266,  267,  268,  269,   -1,  271,  272,   93,   94,
   -1,   -1,   -1,   -1,   -1,  290,   -1,   93,   94,   -1,
   -1,   38,   -1,   -1,   41,  290,   -1,   44,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,  124,
  125,   58,   59,   -1,   -1,   -1,   63,   -1,  124,  125,
   -1,   41,  290,   41,   44,   -1,   44,  268,  269,   -1,
  271,  272,   -1,   41,   -1,   -1,   44,   -1,   58,   59,
   58,   59,   -1,   63,   -1,   63,   93,   94,   -1,  290,
   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,  272,   44,  266,  267,  268,  269,
   -1,  271,  272,   93,   94,   93,   94,  124,  125,   58,
   59,   -1,  290,   -1,   63,   93,   -1,   -1,   -1,   -1,
  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   37,   38,   -1,   40,   -1,
   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   59,   60,   61,
   62,   63,  123,  268,  269,   -1,  271,  272,   -1,   -1,
   -1,   -1,  268,  269,   -1,  271,  272,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,   91,
   -1,   -1,   94,   -1,  290,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,   -1,   -1,  125,  126,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,  125,  126,
   -1,  271,  272,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
  290,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  290,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   -1,   -1,  314,  315,  316,  317,  257,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   -1,   -1,  285,  286,  287,  288,  289,
  257,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,   -1,   -1,  314,  315,  316,  317,  285,  286,
  287,  288,  289,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  285,   -1,  314,  315,  316,
  317,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,   -1,  285,  314,  315,  316,  317,  318,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
   -1,   -1,  314,  315,  316,  317,
  };

#line 1165 "CParser.jay"

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
