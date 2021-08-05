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
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class identifier_or_typename compound_statement",
//t    "struct_or_union_or_class_specifier : struct_or_union_or_class compound_statement",
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
//t    "parameter_declaration : declaration_specifiers declarator '=' assignment_expression",
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
//t    "block_item : visibility",
//t    "visibility : PUBLIC ':'",
//t    "visibility : PRIVATE ':'",
//t    "visibility : PROTECTED ':'",
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
//t    "function_definition : declaration_specifiers declarator declaration_list compound_statement",
//t    "function_definition : declaration_specifiers declarator compound_statement",
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
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
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
		yyVal = new IdentifierDeclarator("~" + yyVals[0+yyTop]);
	}
  break;
case 141:
#line 558 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 143:
  case_143();
  break;
case 144:
#line 578 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 145:
#line 582 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 146:
#line 586 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 147:
#line 590 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 148:
#line 594 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 149:
#line 598 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 150:
#line 602 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 151:
#line 606 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 152:
#line 610 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 153:
  case_153();
  break;
case 154:
#line 622 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 155:
#line 626 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 156:
#line 627 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 157:
#line 628 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 158:
#line 629 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 159:
#line 633 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 160:
#line 637 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 161:
#line 641 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 162:
#line 642 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 163:
#line 643 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 164:
#line 650 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
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
#line 678 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 169:
#line 682 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 170:
#line 686 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 171:
#line 690 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 172:
  case_172();
  break;
case 173:
  case_173();
  break;
case 174:
#line 712 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 175:
#line 716 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 176:
#line 723 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 178:
#line 728 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 179:
  case_179();
  break;
case 180:
#line 747 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 181:
#line 751 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 182:
#line 755 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 183:
#line 759 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 184:
#line 763 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 185:
#line 767 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 186:
#line 771 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 187:
#line 775 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 188:
#line 779 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 189:
#line 783 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 190:
#line 790 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 191:
#line 794 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 192:
#line 798 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
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
  case_196();
  break;
case 197:
#line 838 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 211:
#line 870 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 212:
#line 874 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 213:
#line 878 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 214:
#line 879 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 218:
#line 889 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 219:
#line 890 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 220:
#line 891 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 221:
#line 898 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 222:
#line 902 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 223:
#line 909 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 224:
#line 913 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 226:
#line 921 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 227:
#line 925 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 228:
#line 929 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 229:
#line 933 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 230:
#line 937 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 231:
#line 941 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 233:
#line 949 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 234:
#line 953 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 235:
#line 957 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 236:
#line 961 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 237:
  case_237();
  break;
case 238:
  case_238();
  break;
case 242:
  case_242();
  break;
case 243:
  case_243();
  break;
case 244:
  case_244();
  break;
case 245:
  case_245();
  break;
case 246:
  case_246();
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

void case_143()
#line 564 "CParser.jay"
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

void case_153()
#line 612 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_165()
#line 652 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_166()
#line 661 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_167()
#line 667 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_172()
#line 695 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_173()
#line 701 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_179()
#line 733 "CParser.jay"
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
#line 803 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_194()
#line 810 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_195()
#line 818 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_196()
#line 825 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_237()
#line 966 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_238()
#line 971 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_242()
#line 985 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_243()
#line 994 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_244()
#line 1006 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_245()
#line 1012 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_246()
#line 1018 "CParser.jay"
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
   44,   43,   43,   43,   43,   43,   43,   43,   43,   43,
   43,   43,   43,   43,   42,   42,   42,   42,   45,   45,
   29,   29,   29,   46,   46,   48,   48,   49,   49,   49,
   49,   47,   47,    5,    5,   50,   50,   50,   51,   51,
   51,   51,   51,   51,   51,   51,   51,   51,   51,   33,
   33,   33,    6,    6,    6,    6,   52,   53,   53,   54,
   54,   55,   55,   55,   55,   55,   55,   56,   56,   56,
   38,   38,   61,   61,   62,   62,   62,   63,   63,   63,
   57,   57,   58,   58,   58,   59,   59,   59,   59,   59,
   59,   60,   60,   60,   60,   60,    0,    0,   64,   64,
   64,   65,   65,   66,   66,   66,   67,   67,   67,
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
    3,    1,    3,    5,    4,    4,    6,    6,    5,    4,
    3,    4,    4,    3,    1,    2,    2,    3,    1,    2,
    1,    1,    1,    1,    3,    1,    3,    2,    4,    2,
    1,    1,    3,    1,    2,    1,    1,    2,    3,    2,
    3,    3,    4,    3,    4,    2,    3,    3,    4,    1,
    3,    4,    1,    2,    3,    4,    2,    1,    2,    3,
    2,    1,    1,    1,    1,    1,    1,    3,    4,    3,
    2,    3,    1,    2,    1,    1,    1,    2,    2,    2,
    1,    2,    5,    7,    5,    5,    7,    6,    7,    6,
    7,    3,    2,    2,    2,    3,    1,    2,    1,    1,
    1,    4,    3,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  114,   95,   96,   97,   98,   99,  136,  162,  101,  102,
  103,  104,  107,  108,  105,  106,  161,  163,  100,  109,
  110,  111,  120,  121,  122,    0,  241,    0,  240,    0,
    0,    0,    0,    0,  112,  113,    0,  237,  239,  115,
  116,    0,    0,  238,  139,    0,    0,    0,   81,    0,
   91,    0,    0,    0,    0,   84,   86,   88,   90,    0,
    0,  118,    0,    0,  132,    0,    0,  159,  157,    0,
  140,    0,   82,  247,    0,  248,  249,  244,    0,  243,
    0,    0,    0,    0,    0,    0,    0,    2,    3,    0,
    0,    0,    0,    0,    0,    4,    5,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  211,
    0,    0,   27,   28,   29,   30,  221,    7,    0,    0,
   78,    0,   33,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   63,  215,  203,  216,  202,  204,
  205,  206,  207,    0,  213,  217,  117,    0,  127,    0,
    0,  143,  160,  158,   92,    0,    1,    0,  190,   94,
  246,  242,  245,  172,  154,    0,    0,    0,    0,  166,
    0,  151,    0,    0,    0,  141,    0,    0,   25,    0,
   20,   21,  218,  219,  220,   31,   80,    0,    0,    0,
    0,    0,    0,    0,    0,  233,  234,  235,    0,    0,
    0,    0,    0,    0,   22,   23,    0,  222,    0,   13,
   14,    0,    0,    0,   66,   67,   68,   69,   70,   71,
   72,   73,   74,   75,   76,   77,   65,    0,   24,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  212,  214,
  135,  129,  133,  128,    0,    0,    0,    0,  193,    0,
    0,  198,    0,    0,    0,    0,  170,    0,  152,  153,
    0,    0,    0,  150,  146,    0,  145,    0,    0,  208,
    0,    0,    0,  210,    0,    0,    0,    0,    0,    0,
  232,  236,    6,    0,  123,  125,    0,    0,  175,   79,
   12,    9,    0,   17,    0,   11,   64,   34,   35,   36,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  130,    0,  201,  191,
    0,  194,  197,  199,  186,    0,    0,  180,    0,    0,
    0,    0,    0,    0,  173,  165,  167,    0,    0,  149,
  144,    0,    0,  209,    0,    0,    0,    0,    0,    0,
    0,   32,   10,    0,    8,    0,  200,  192,  195,    0,
  187,  179,  184,  181,  169,  188,    0,  182,    0,    0,
  147,  148,    0,  225,  226,    0,    0,    0,    0,    0,
    0,   18,   62,  196,  189,  185,  183,    0,    0,  230,
    0,  228,    0,   15,    0,  224,  227,  231,  229,   16,
  };
  protected static readonly short [] yyDgoto  = {            28,
  118,  119,  120,  303,  201,  258,  121,  122,  123,  124,
  125,  126,  127,  128,  129,  130,  131,  132,  133,  134,
  135,  228,  188,   29,   79,   50,   31,   32,   33,   34,
   51,   67,  259,   35,   36,   43,   37,  137,  204,   64,
   65,   53,   54,   55,   70,  336,  168,  169,  170,  337,
  268,  260,  261,  262,  138,  139,  140,  141,  142,  143,
  144,  145,  146,   38,   39,   81,   82,
  };
  protected static readonly short [] yySindex = {         1887,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, -103,    0, 1887,    0,  -33,
 2776, 2776, 2776, 2776,    0,    0,  -81,    0,    0,    0,
    0, -209,  -62,    0,    0,  -37,  -30, -189,    0,   14,
    0,  701,   26,   23, -191,    0,    0,    0,    0,   77,
  -28,    0,   40,    3,    0, -209,   72,    0,    0,  -30,
    0,  -37,    0,    0,  511,    0,    0,    0,  -33,    0,
 2659, 2776,   23, 1804,  522, -110,  105,    0,    0, 1546,
 1580, 1580,  159,  175,  185,    0,    0, 1591,  195,  223,
  247,  260,  196,  280,   67,  221,  272, 1161,  808,    0,
 1591, 1591,    0,    0,    0,    0,    0,    0,  164,   16,
    0,  167,    0, 1591,  370,  225, -212,   24,  145,  295,
  249,  217,   74,  -57,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  176,    0,    0,    0, 1591,    0, -102,
   25,    0,    0,    0,    0,  288,    0,  721,    0,    0,
    0,    0,    0,    0,    0,   81,  315,  142,  349,    0,
  -99,    0, 1169,  318, 1091,    0,  196,  808,    0,  808,
    0,    0,    0,    0,    0,    0,    0,  362,  196, 1591,
 1591, 1591,  102, 1019,  372,    0,    0,    0,  168,  210,
  396, 1662, 1662,   15,    0,    0, 1591,    0,  235,    0,
    0, 1258, 1591,  239,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 1591,    0, 1591,
 1591, 1591, 1591, 1591, 1591, 1591, 1591, 1591, 1591, 1591,
 1591, 1591, 1591, 1591, 1591, 1591, 1591, 1591,    0,    0,
    0,    0,    0,    0, -101, 1591,  255,   34,    0,  511,
  131,    0, 1726, 1347,  391,  -27,    0,   27,    0,    0,
  259, 2720,  383,    0,    0, 1591,    0, 1403,  444,    0,
  497,  498,  196,    0,  240,  244,  252,  507, 1411, 1411,
    0,    0,    0, 1336,    0,    0, 1773,  134,    0,    0,
    0,    0,  374,    0,    5,    0,    0,    0,    0,    0,
  370,  370,  225,  225, -212, -212, -212, -212,   24,   24,
  145,  295,  249,  217,   74,  182,    0,  455,    0,    0,
 1104,    0,    0,    0,    0,  509,  516,    0, 1445,  459,
 1591,   27, 1835, 1466,    0,    0,    0,  465,  466,    0,
    0,  438,  438,    0,  196,  196,  196, 1591, 1490, 1501,
  721,    0,    0, 1591,    0, 1591,    0,    0,    0,  511,
    0,    0,    0,    0,    0,    0,  525,    0, 1522,  470,
    0,    0,  279,    0,    0,  378,  196,  386,  196,  388,
  107,    0,    0,    0,    0,    0,    0,  196,  544,    0,
  196,    0,  196,    0, 1147,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  684, 1001, 1137, 1393,    0,    0,    0,    0,    0,    0,
    0,    0, 1636,    0,    0,    0,  583,    0,    0,    0,
    0,  208,    0,  618,  577,    0,    0,    0,    0,    0,
 1695,    0,  123,    0,    0,    0,    0,    0,    0, 1050,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  660,    0,    0,    0, 2166,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 2193,
    0, 2261,    0,    0, 2289,   86,  -22, 2671,  622, 1734,
   33, 2634,  -13,  572,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  208,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  416,    0,    0,  573,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  306,  319,  578,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  464,  484,    0,  485,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  579,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2298, 2358, 2478, 2538, 2546, 2575, 2614, 2643, 2728, 2735,
  664, 2601,   46, 2775, 2791,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  486,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2221,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  283,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -49,    0,    0,  149,  261,  -75,  927,  -97,    0,
  277,  296,   75,  292,  382,  384,  381,  387,  385,    0,
  -94,    0, -118,  113,    1,    0,    0,  -12,  626,    0,
  557,    9,  -73,    0,    0,  598,    0,  132,  333,  581,
 -139,  -26,  -35,    0,  -60,  -67,    0,    0,  367,  -96,
 -223, -280,    0,  389,  -95,    0, -129,    0,    0,    0,
    0,  500,    0,  621,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           159,
   30,  160,   46,  187,   47,  248,   46,  193,   47,  174,
  253,   47,  263,  205,  206,   43,  167,   83,   43,   42,
   69,   43,  252,  327,  175,   49,  229,   59,   30,  251,
   59,   56,   57,   58,   59,   43,   43,   43,   52,   43,
   43,   60,  342,  154,   59,   59,  150,   63,  207,   59,
  370,  235,  236,  187,  297,  212,   47,   72,  199,  200,
   66,  214,   84,  264,  290,   46,  343,   71,  255,  267,
   43,   43,   73,   55,  342,  206,   55,  331,   86,   59,
  156,  280,  159,  239,  166,  240,   56,  156,   48,   56,
   55,   55,   48,  284,   60,   55,  202,  365,   48,  279,
  148,   43,   43,   56,   56,  264,  213,  299,   56,  116,
  273,   59,  152,   85,  111,  253,  109,  344,  112,  113,
  263,  114,   47,   40,  370,   55,   40,  149,  200,   40,
  200,  300,  308,  309,  310,  117,  304,  328,   56,  266,
  285,  286,  287,   40,   40,   40,  176,   40,   40,  254,
  405,   48,  307,   40,   63,   63,   55,   55,  330,  359,
  360,  187,  177,  305,   78,  202,  134,  202,   62,   56,
   56,  264,  136,  297,  265,   40,  257,  298,   40,   40,
  206,   41,  270,   80,  159,  271,  332,  354,  340,  202,
  202,  333,  147,  161,  163,    8,  362,  348,  326,   60,
  349,  110,  115,   41,   17,   18,   48,  207,  116,   40,
   40,  207,  162,  111,  247,  109,  183,  112,  113,   45,
  114,  256,  208,   45,  264,  207,  292,  227,  116,   45,
   83,  404,  184,  111,  117,  109,  266,  112,  113,  366,
  114,  206,  185,   43,   43,   43,   43,  134,   43,   43,
  293,   93,  189,  207,  117,  159,  136,  369,   59,  383,
  384,  385,  190,  166,    8,  375,   93,  233,  380,  234,
  298,  393,  166,   17,   18,  377,  209,  210,  211,  196,
  355,  206,   45,  207,  356,  159,  191,  207,  392,  237,
  238,  400,  357,  402,  159,  207,  394,  166,   60,  192,
  249,  115,  406,   55,   55,  408,  289,  409,  386,  388,
  390,  315,  316,  317,  318,  223,   56,   56,   60,  194,
  223,  115,  223,  195,  223,  223,  281,  223,  282,  159,
  197,  369,  243,   87,   88,   89,   90,   45,   91,   92,
  245,  223,  244,  166,  246,  124,  124,  124,   75,   40,
   40,   40,   40,   40,   40,  269,   40,   40,  126,  126,
  126,    1,   93,   94,   95,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   96,   97,   23,   24,
   25,   26,  272,   98,   99,  100,  124,  101,  102,  103,
  104,  105,  106,  107,  108,  223,  232,  223,  223,  126,
  275,  230,  241,  242,  363,  116,  231,  364,  399,  283,
  111,  207,  109,  288,  112,  113,  401,  114,  403,  207,
  291,  207,   87,   88,   89,   90,  294,   91,   92,  215,
  216,  217,  218,  219,  220,  221,  222,  223,  224,  225,
  226,  341,   87,   88,   89,   90,  171,   91,   92,  171,
    1,   93,   94,   95,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   96,   97,   23,   24,   25,
   26,  301,   98,   99,  100,  306,  101,  102,  103,  104,
  105,  106,  107,  108,  168,   96,   97,  168,  115,  311,
  312,  329,   98,   99,  100,  345,  101,  102,  103,  104,
  105,  106,  107,  108,  176,  177,  178,  176,  177,  178,
  313,  314,  319,  320,  295,  296,  351,  352,  353,  223,
  223,  223,  223,  116,  223,  223,  358,  367,  111,  371,
  109,  374,  112,  113,  116,  114,  372,  381,  382,  111,
  361,  109,  397,  173,  113,  395,  114,  223,  223,  223,
  223,  223,  223,  223,  223,  223,  223,  223,  223,  223,
  223,  223,  223,  223,  223,  223,  223,  223,  223,  223,
  223,  223,  223,  223,  223,  223,  223,  223,  398,  223,
  223,  223,  407,  223,  223,  223,  223,  223,  223,  223,
  223,  142,   61,  164,  172,   61,  142,  142,  174,  176,
  142,  391,  155,  155,  321,  323,  155,  322,  155,   61,
   61,  325,  324,  158,   61,  142,  115,  142,  347,  157,
   88,   89,   90,  250,   91,   92,  151,  115,   44,  334,
    0,    0,  138,    0,    0,    0,    0,    0,  138,   51,
    0,  138,   51,    0,   61,   51,    0,  142,  142,    0,
    0,    0,   68,  155,    0,    0,  138,    8,  138,   51,
   51,    0,    0,    0,   51,    0,   17,   18,    0,    0,
    0,    0,   96,   97,  137,  153,   61,    0,    0,  142,
  137,   52,    0,  137,   52,    0,    0,   52,  155,  138,
   68,    0,    0,    0,   51,   51,    0,    0,  137,    0,
  137,   52,   52,   83,   83,   83,   52,   83,    0,    0,
    0,    0,    0,    0,  203,   76,    0,    0,    0,    0,
  138,    0,   83,    0,    0,   51,   51,    0,    0,    0,
    0,  137,    0,  116,    0,    0,   52,   52,  111,    0,
  109,   75,  112,  113,    0,  114,  257,  157,   88,   89,
   90,    0,   91,   92,   83,    0,    0,    0,  157,   88,
   89,   90,  137,   91,   92,    0,    0,   52,   52,    0,
    0,    0,   77,    0,    0,    0,   68,    0,    0,    0,
  153,    0,    0,  203,    0,  203,    0,    0,    0,   83,
    0,  256,  171,    0,    0,    0,    8,    0,    0,    0,
   96,   97,    0,   60,    0,   17,   18,  203,  203,    0,
    0,   96,   97,    0,    0,    0,    0,    0,    0,  155,
  116,    0,    0,  158,    0,  111,  115,  109,    0,  112,
  113,    0,  114,    0,    0,    0,    0,    0,    0,    0,
    0,  142,    0,    0,    0,  142,  142,  142,  142,  142,
  142,  142,  142,  142,  142,  142,  142,  142,  142,  142,
  142,  142,  142,  142,  142,  142,    0,    0,  142,  142,
  142,  142,   51,   51,    0,    0,    0,    0,  153,    0,
    0,    0,  138,    0,    0,  142,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,    0,    0,  138,
  138,  138,  138,  115,   52,   52,    0,    0,    0,    0,
   83,    0,    0,    0,  137,    0,  138,    0,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,    0,
    0,  137,  137,  137,  137,    0,    0,  157,   88,   89,
   90,    0,   91,   92,    0,    1,    0,    0,  137,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,  179,  181,  182,    0,
    0,    0,    0,    0,  186,    0,    0,    0,    0,   74,
   96,   97,    0,    0,    0,    0,    0,  186,  186,    0,
   85,   85,   85,    0,   85,    0,    0,    0,    0,    0,
  186,  116,    0,    0,    0,    0,  111,    0,  109,   85,
  112,  113,    0,  114,  157,   88,   89,   90,    0,   91,
   92,    0,    0,    0,  186,    0,    0,  117,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  156,
  156,   85,    1,  156,    0,    0,    0,    0,    0,  186,
    0,    0,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   96,   97,   23,
   24,   25,   26,  116,    0,    0,   85,    0,  111,    0,
  109,    0,  278,  113,    0,  114,  116,    0,    0,    0,
  156,  111,    0,  109,  115,  112,  113,    0,  114,  257,
    0,    0,    0,    0,    0,    0,  186,  186,  186,  186,
  186,  186,  186,  186,  186,  186,  186,  186,  186,  186,
  186,  186,  186,  186,    0,  156,   87,   87,   87,  116,
   87,    0,  186,  277,  111,    0,  109,    0,  112,  113,
    0,  114,  257,  116,  256,   87,    0,    0,  111,    0,
  109,  116,  112,  113,  186,  114,  111,    0,  109,    0,
  112,  113,    0,  114,    0,    0,  115,    0,    0,  198,
  186,    0,    0,    0,    0,    0,  158,   87,  368,  115,
    0,    0,    0,    0,    0,    0,    0,  256,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   85,    0,    0,
    0,  274,   87,    0,    0,  186,    0,    0,    0,  158,
    0,  410,  115,    0,    0,  157,   88,   89,   90,    0,
   91,   92,    0,    0,    0,    0,  115,    0,    0,    0,
  116,    0,  186,    0,  115,  111,    0,  109,  302,  112,
  113,    0,  114,    1,    0,  186,  156,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   96,   97,
   23,   24,   25,   26,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  157,   88,   89,
   90,    0,   91,   92,    0,    0,    0,    0,    0,    0,
  157,   88,   89,   90,    0,   91,   92,    0,  116,    0,
    0,    0,    0,  111,    0,  109,    0,  112,  113,  116,
  114,  276,    0,  115,  111,    8,  109,    0,  339,  113,
    0,  114,    0,   87,   17,   18,    0,    0,    0,    0,
   96,   97,    0,  157,   88,   89,   90,    0,   91,   92,
    0,    0,    0,   96,   97,    0,    0,  157,   88,   89,
   90,    0,   91,   92,    0,  157,   88,   89,   90,    0,
   91,   92,   89,   89,   89,  116,   89,    0,    0,  338,
  111,    0,  109,  116,  112,  113,    0,  114,  111,    0,
  109,   89,  112,  113,    0,  114,   96,   97,  361,    0,
    0,  115,    0,    0,    0,    0,    0,    0,    0,  117,
   96,   97,  115,    0,    0,    0,    0,  116,   96,   97,
    0,    0,  111,   89,  109,    0,  112,  113,    0,  114,
    0,    0,    0,    0,    0,  350,    0,    0,  116,    0,
    0,    0,    0,  111,    0,  109,    0,  379,  113,    0,
  114,    0,    0,    0,  157,   88,   89,   90,   89,   91,
   92,    0,  116,    0,    0,    0,    0,  111,  115,  109,
  387,  112,  113,  116,  114,    0,  115,  373,  111,    0,
  109,  389,  112,  113,    0,  114,    0,    0,    0,    0,
    0,    0,    0,    0,  116,    0,    0,    0,  378,  111,
    0,  109,    0,  112,  113,    0,  114,   96,   97,    0,
  115,    0,    0,    0,    0,    0,    0,    0,  116,    0,
    0,    0,    0,  111,    0,  178,    0,  112,  113,    0,
  114,  115,  157,   88,   89,   90,    0,   91,   92,    0,
    0,    0,    0,  157,   88,   89,   90,    0,   91,   92,
    0,    0,  116,    0,  396,  115,    0,  111,    0,  180,
    0,  112,  113,  116,  114,    0,  115,    0,  111,    0,
  109,    0,  112,  113,    0,  114,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   96,   97,  115,    0,   89,
    0,    0,    0,    0,    0,    0,   96,   97,    0,  157,
   88,   89,   90,    0,   91,   92,    0,  157,   88,   89,
   90,  115,   91,   92,    0,  131,  131,  131,    0,  131,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  131,    0,    0,    0,    0,    0,
    0,  157,   88,   89,   90,  115,   91,   92,    0,    0,
    0,    0,   96,   97,    0,    0,  115,    0,    0,    0,
   96,   97,  157,   88,   89,   90,  131,   91,   92,    0,
    0,    0,    0,    0,  119,  119,  119,    0,  119,    0,
    0,    0,    0,    0,    0,    0,  157,   88,   89,   90,
    0,   91,   92,  119,   96,   97,    0,  157,   88,   89,
   90,  131,   91,   92,    0,  263,  335,   47,    0,    0,
    0,    0,    0,    0,   53,   96,   97,   53,  157,   88,
   89,   90,    0,   91,   92,  119,    0,    0,    0,    0,
    0,   53,   53,    0,    0,    0,   53,    0,    0,   96,
   97,    0,  157,   88,   89,   90,    0,   91,   92,    0,
   96,   97,  297,  335,   47,    0,  264,    0,    0,    0,
  119,    0,    0,    0,    0,    0,   53,   53,    0,    0,
    0,   96,   97,    0,    0,    0,  157,   88,   89,   90,
    0,   91,   92,    0,  165,    0,    0,  157,   88,   89,
   90,   48,   91,   92,    0,   96,   97,   53,   53,    0,
    0,    0,    0,  264,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  376,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   96,
   97,    0,  131,    0,    0,    0,    0,    0,    0,    0,
   96,   97,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  131,    0,    0,    0,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,  131,   27,    1,  131,  131,  131,
  131,  119,    0,    0,    0,    0,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,  119,
    0,    0,   45,  119,  119,  119,  119,  119,  119,  119,
  119,  119,  119,  119,  119,  119,  119,  119,  119,  119,
  119,  119,  119,  119,   53,   53,  119,  119,  119,  119,
    1,    0,    0,    0,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    1,    0,    0,
  164,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,    1,    0,
    0,    0,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,    1,
    0,    0,    0,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,    0,    0,   23,   24,   25,   26,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    1,    0,    0,    0,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,    0,    0,   23,   24,
   25,   26,    1,    1,    0,    1,    0,    1,    1,    1,
    1,    1,    1,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    1,    1,    1,    1,    1,   19,
   19,    0,    0,   19,   19,   19,   19,   19,    0,   19,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   19,   19,   19,   19,   19,   19,    1,   26,   26,    1,
    0,   26,   26,   26,   26,   26,    0,   26,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   26,   26,
   26,   26,   26,   26,    0,   19,   19,    0,    0,    1,
    0,    0,    0,    0,    0,    0,    0,   31,   31,    0,
    0,   31,   31,   31,   31,   31,    0,   31,    0,    0,
    0,    0,    0,   26,   26,    0,   19,   19,   31,   31,
   31,    0,   31,   31,    0,    0,   37,    0,    0,   37,
    0,   37,   37,   37,    0,   38,    0,    0,   38,    0,
   38,   38,   38,    0,   26,   26,   37,   37,   37,    0,
   37,   37,    0,   31,   31,   38,   38,   38,    0,   38,
   38,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   37,   37,    0,   31,   31,    0,    0,    0,    0,
   38,   38,    0,    0,    0,   39,    0,    0,   39,    0,
   39,   39,   39,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   37,   37,    0,   39,   39,   39,    0,   39,
   39,   38,   38,    0,    0,    0,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    0,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
   39,   39,    0,    0,    0,    0,   19,   19,   19,   19,
   19,   19,    0,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,    0,    0,    0,
    0,   39,   39,    0,   26,   26,   26,   26,   26,   26,
    0,   26,   26,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   41,    0,    0,   41,    0,
    0,   41,    0,    0,   31,   31,   31,   31,   31,   31,
    0,   31,   31,    0,    0,   41,   41,   41,    0,   41,
   41,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   37,   37,   37,   37,   37,   37,    0,   37,
   37,   38,   38,   38,   38,   38,   38,    0,   38,   38,
   41,   41,    0,    0,    0,   42,    0,    0,   42,    0,
    0,   42,    0,   46,    0,    0,   46,    0,    0,   46,
    0,    0,    0,    0,    0,   42,   42,   42,    0,   42,
   42,   41,   41,   46,   46,   46,    0,   46,   46,    0,
    0,    0,   47,    0,    0,   47,    0,    0,   47,    0,
    0,   39,   39,   39,   39,   39,   39,    0,   39,   39,
   42,   42,   47,   47,   47,    0,   47,   47,   46,   46,
    0,   54,    0,    0,   54,    0,    0,    0,    0,    0,
    0,   44,    0,    0,   44,    0,    0,   44,   54,   54,
    0,   42,   42,   54,    0,    0,    0,   47,   47,   46,
   46,   44,   44,   44,   57,   44,   44,   57,    0,    0,
   45,    0,    0,   45,    0,    0,   45,    0,    0,    0,
    0,   57,   57,   54,   54,    0,   57,    0,   47,   47,
   45,   45,   45,    0,   45,   45,   44,   44,   48,    0,
    0,   48,    0,    0,   48,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   54,   54,   57,    0,   48,   48,
    0,    0,    0,   48,    0,   45,   45,   44,   44,    0,
    0,   41,   41,   41,   41,   41,   41,    0,   41,   41,
    0,    0,    0,    0,    0,    0,    0,    0,   57,    0,
    0,    0,    0,   48,   48,   49,   45,   45,   49,    0,
    0,   49,   50,    0,    0,   50,    0,    0,   50,    0,
    0,   60,    0,    0,    0,   49,   49,    0,    0,    0,
   49,    0,   50,   50,   48,   48,    0,   50,    0,    0,
    0,   42,   42,   42,   42,   42,   42,    0,   42,   42,
    0,   46,   46,   46,   46,   58,   46,   46,   58,    0,
   49,   49,    0,    0,    0,    0,    0,   50,   50,    0,
    0,   60,   58,   58,   60,    0,    0,   58,    0,    0,
   47,   47,   47,   47,    0,   47,   47,    0,   60,   60,
    0,   49,   49,   60,    0,    0,    0,    0,   50,   50,
    0,    0,    0,    0,    0,    0,    0,   58,    0,    0,
    0,   54,   54,    0,    0,    0,    0,    0,    0,   44,
   44,   44,   44,   60,   44,   44,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   58,
    0,    0,    0,    0,   57,   57,    0,    0,   45,   45,
   45,   45,    0,   45,   45,   60,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   48,   48,
    0,   48,   48,    1,    0,    0,    0,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   49,   49,    0,   49,   49,
    0,    0,   50,   50,    1,   50,   50,    0,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,  346,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   58,   58,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    0,   60,    0,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,
  };
  protected static readonly short [] yyCheck = {            75,
    0,   75,   40,   98,   42,   63,   40,  103,   42,   85,
  150,   42,   40,  111,  112,   38,   84,   53,   41,  123,
   47,   44,  125,  125,   85,   59,  124,   41,   28,  148,
   44,   31,   32,   33,   34,   58,   59,   60,   30,   62,
   63,  123,  266,   70,   58,   59,   44,  257,   44,   63,
  331,  264,  265,  148,   40,   40,   42,   44,  108,  109,
  123,   46,   40,   91,  194,   40,   40,  257,   44,  166,
   93,   94,   59,   41,  298,  173,   44,   44,  270,   93,
   72,  177,  158,   60,   84,   62,   41,   79,  126,   44,
   58,   59,  126,  189,  123,   63,  109,   93,  126,  175,
   61,  124,  125,   58,   59,   91,   91,  204,   63,   33,
  171,  125,   41,   91,   38,  255,   40,   91,   42,   43,
   40,   45,   42,   38,  405,   93,   41,  125,  178,   44,
  180,  207,  230,  231,  232,   59,  212,  256,   93,  166,
  190,  191,  192,   58,   59,   60,  257,   62,   63,  125,
   44,  126,  228,  257,  257,  257,  124,  125,  125,  289,
  290,  256,   58,  213,   52,  178,   44,  180,   37,  124,
  125,   91,   60,   40,  166,  257,   46,  204,   93,   94,
  278,  285,   41,   52,  260,   44,  260,  283,  264,  202,
  203,   61,   61,   81,   82,  295,  294,  273,  248,  123,
  276,  125,  126,  285,  304,  305,  126,   44,   33,  124,
  125,   44,   81,   38,  272,   40,   58,   42,   43,  257,
   45,   91,   59,  257,   91,   44,   59,   61,   33,  257,
  266,  125,   58,   38,   59,   40,  263,   42,   43,   58,
   45,  339,   58,  266,  267,  268,  269,  125,  271,  272,
   41,   44,   58,   44,   59,  331,  144,  331,  272,  355,
  356,  357,   40,  263,  295,  341,   59,   43,  344,   45,
  297,  366,  272,  304,  305,  343,  261,  262,  263,   59,
   41,  379,  257,   44,   41,  361,   40,   44,  364,  266,
  267,  387,   41,  389,  370,   44,  370,  297,  123,   40,
  125,  126,  398,  271,  272,  401,  194,  403,  358,  359,
  360,  237,  238,  239,  240,   33,  271,  272,  123,   40,
   38,  126,   40,  257,   42,   43,  178,   45,  180,  405,
   59,  405,   38,  257,  258,  259,  260,  257,  262,  263,
  124,   59,   94,  343,  271,   40,   41,   42,   61,  264,
  265,  266,  267,  268,  269,   41,  271,  272,   40,   41,
   42,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,  312,  313,
  314,  315,   44,  317,  318,  319,   91,  321,  322,  323,
  324,  325,  326,  327,  328,  123,   37,  125,  126,   91,
   93,   42,  268,  269,   41,   33,   47,   44,   41,   58,
   38,   44,   40,  322,   42,   43,   41,   45,   41,   44,
   59,   44,  257,  258,  259,  260,   41,  262,  263,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   61,  257,  258,  259,  260,   41,  262,  263,   44,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,  257,  317,  318,  319,  257,  321,  322,  323,  324,
  325,  326,  327,  328,   41,  310,  311,   44,  126,  233,
  234,  257,  317,  318,  319,  257,  321,  322,  323,  324,
  325,  326,  327,  328,   41,   41,   41,   44,   44,   44,
  235,  236,  241,  242,  202,  203,   93,   41,   41,  257,
  258,  259,  260,   33,  262,  263,   40,   93,   38,   41,
   40,   93,   42,   43,   33,   45,   41,   93,   93,   38,
  123,   40,   93,   42,   43,   41,   45,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,  311,  312,  313,  314,  315,  320,  317,
  318,  319,   59,  321,  322,  323,  324,  325,  326,  327,
  328,   35,   41,   41,   93,   44,   40,   41,   41,   41,
   44,  361,   40,   41,  243,  245,   44,  244,   72,   58,
   59,  247,  246,  123,   37,   59,  126,   61,  272,  257,
  258,  259,  260,  144,  262,  263,   66,  126,   28,  261,
   -1,   -1,   35,   -1,   -1,   -1,   -1,   -1,   41,   38,
   -1,   44,   41,   -1,   93,   44,   -1,   91,   92,   -1,
   -1,   -1,   47,   91,   -1,   -1,   59,  295,   61,   58,
   59,   -1,   -1,   -1,   63,   -1,  304,  305,   -1,   -1,
   -1,   -1,  310,  311,   35,   70,  125,   -1,   -1,  123,
   41,   38,   -1,   44,   41,   -1,   -1,   44,  126,   92,
   85,   -1,   -1,   -1,   93,   94,   -1,   -1,   59,   -1,
   61,   58,   59,   40,   41,   42,   63,   44,   -1,   -1,
   -1,   -1,   -1,   -1,  109,   35,   -1,   -1,   -1,   -1,
  123,   -1,   59,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   92,   -1,   33,   -1,   -1,   93,   94,   38,   -1,
   40,   61,   42,   43,   -1,   45,   46,  257,  258,  259,
  260,   -1,  262,  263,   91,   -1,   -1,   -1,  257,  258,
  259,  260,  123,  262,  263,   -1,   -1,  124,  125,   -1,
   -1,   -1,   92,   -1,   -1,   -1,  171,   -1,   -1,   -1,
  175,   -1,   -1,  178,   -1,  180,   -1,   -1,   -1,  126,
   -1,   91,  291,   -1,   -1,   -1,  295,   -1,   -1,   -1,
  310,  311,   -1,  123,   -1,  304,  305,  202,  203,   -1,
   -1,  310,  311,   -1,   -1,   -1,   -1,   -1,   -1,  257,
   33,   -1,   -1,  123,   -1,   38,  126,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  285,   -1,   -1,   -1,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,   -1,   -1,  312,  313,
  314,  315,  271,  272,   -1,   -1,   -1,   -1,  273,   -1,
   -1,   -1,  285,   -1,   -1,  329,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,   -1,   -1,  312,
  313,  314,  315,  126,  271,  272,   -1,   -1,   -1,   -1,
  257,   -1,   -1,   -1,  285,   -1,  329,   -1,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,   -1,
   -1,  312,  313,  314,  315,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  285,   -1,   -1,  329,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
   -1,   -1,  312,  313,  314,  315,   90,   91,   92,   -1,
   -1,   -1,   -1,   -1,   98,   -1,   -1,   -1,   -1,  329,
  310,  311,   -1,   -1,   -1,   -1,   -1,  111,  112,   -1,
   40,   41,   42,   -1,   44,   -1,   -1,   -1,   -1,   -1,
  124,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   59,
   42,   43,   -1,   45,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,  148,   -1,   -1,   59,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   40,
   41,   91,  285,   44,   -1,   -1,   -1,   -1,   -1,  173,
   -1,   -1,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  313,  314,  315,   33,   -1,   -1,  126,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   33,   -1,   -1,   -1,
   91,   38,   -1,   40,  126,   42,   43,   -1,   45,   46,
   -1,   -1,   -1,   -1,   -1,   -1,  230,  231,  232,  233,
  234,  235,  236,  237,  238,  239,  240,  241,  242,  243,
  244,  245,  246,  247,   -1,  126,   40,   41,   42,   33,
   44,   -1,  256,   93,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   46,   33,   91,   59,   -1,   -1,   38,   -1,
   40,   33,   42,   43,  278,   45,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  126,   -1,   -1,   59,
  294,   -1,   -1,   -1,   -1,   -1,  123,   91,  125,  126,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   91,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,
   -1,   93,  126,   -1,   -1,  339,   -1,   -1,   -1,  123,
   -1,  125,  126,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,
   33,   -1,  366,   -1,  126,   38,   -1,   40,   41,   42,
   43,   -1,   45,  285,   -1,  379,  257,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  313,  314,  315,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   33,
   45,  291,   -1,  126,   38,  295,   40,   -1,   42,   43,
   -1,   45,   -1,  257,  304,  305,   -1,   -1,   -1,   -1,
  310,  311,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,  310,  311,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,   -1,
  262,  263,   40,   41,   42,   33,   44,   -1,   -1,   93,
   38,   -1,   40,   33,   42,   43,   -1,   45,   38,   -1,
   40,   59,   42,   43,   -1,   45,  310,  311,  123,   -1,
   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,
  310,  311,  126,   -1,   -1,   -1,   -1,   33,  310,  311,
   -1,   -1,   38,   91,   40,   -1,   42,   43,   -1,   45,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,  257,  258,  259,  260,  126,  262,
  263,   -1,   33,   -1,   -1,   -1,   -1,   38,  126,   40,
   41,   42,   43,   33,   45,   -1,  126,   93,   38,   -1,
   40,   41,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   93,   38,
   -1,   40,   -1,   42,   43,   -1,   45,  310,  311,   -1,
  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,  126,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   33,   -1,   93,  126,   -1,   38,   -1,   40,
   -1,   42,   43,   33,   45,   -1,  126,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  310,  311,  126,   -1,  257,
   -1,   -1,   -1,   -1,   -1,   -1,  310,  311,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,  257,  258,  259,
  260,  126,  262,  263,   -1,   40,   41,   42,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,  126,  262,  263,   -1,   -1,
   -1,   -1,  310,  311,   -1,   -1,  126,   -1,   -1,   -1,
  310,  311,  257,  258,  259,  260,   91,  262,  263,   -1,
   -1,   -1,   -1,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   59,  310,  311,   -1,  257,  258,  259,
  260,  126,  262,  263,   -1,   40,   41,   42,   -1,   -1,
   -1,   -1,   -1,   -1,   41,  310,  311,   44,  257,  258,
  259,  260,   -1,  262,  263,   91,   -1,   -1,   -1,   -1,
   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,  310,
  311,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  310,  311,   40,   41,   42,   -1,   91,   -1,   -1,   -1,
  126,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
   -1,  310,  311,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   41,   -1,   -1,  257,  258,  259,
  260,  126,  262,  263,   -1,  310,  311,  124,  125,   -1,
   -1,   -1,   -1,   91,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  310,
  311,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  310,  311,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,   -1,   -1,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,   59,  285,  312,  313,  314,
  315,  257,   -1,   -1,   -1,   -1,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,   -1,   -1,  312,  313,  314,  315,   -1,   -1,  285,
   -1,   -1,  257,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  271,  272,  312,  313,  314,  315,
  285,   -1,   -1,   -1,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,   -1,   -1,  312,  313,  314,
  315,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,
  257,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,   -1,   -1,  312,  313,  314,  315,  285,   -1,
   -1,   -1,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,   -1,   -1,  312,  313,  314,  315,  285,
   -1,   -1,   -1,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,   -1,   -1,  312,  313,  314,  315,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  285,   -1,   -1,   -1,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,   -1,   -1,  312,  313,
  314,  315,   37,   38,   -1,   40,   -1,   42,   43,   44,
   45,   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   63,   37,
   38,   -1,   -1,   41,   42,   43,   44,   45,   -1,   47,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   61,   62,   63,   91,   37,   38,   94,
   -1,   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,
   60,   61,   62,   63,   -1,   93,   94,   -1,   -1,  124,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   37,   38,   -1,
   -1,   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,
   -1,   -1,   -1,   93,   94,   -1,  124,  125,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   38,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   38,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,  124,  125,   58,   59,   60,   -1,
   62,   63,   -1,   93,   94,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   94,   -1,  124,  125,   -1,   -1,   -1,   -1,
   93,   94,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   58,   59,   60,   -1,   62,
   63,  124,  125,   -1,   -1,   -1,  261,  262,  263,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,  273,  274,
  275,  276,  277,  278,  279,  280,  281,  282,  283,  284,
   93,   94,   -1,   -1,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,   -1,   -1,   -1,
   -1,  124,  125,   -1,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  282,  283,  284,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   93,   94,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   -1,   62,
   63,  124,  125,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   93,   94,   58,   59,   60,   -1,   62,   63,   93,   94,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   58,   59,
   -1,  124,  125,   63,   -1,   -1,   -1,   93,   94,  124,
  125,   58,   59,   60,   41,   62,   63,   44,   -1,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   58,   59,   93,   94,   -1,   63,   -1,  124,  125,
   58,   59,   60,   -1,   62,   63,   93,   94,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   93,   -1,   58,   59,
   -1,   -1,   -1,   63,   -1,   93,   94,  124,  125,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,
   -1,   -1,   -1,   93,   94,   38,  124,  125,   41,   -1,
   -1,   44,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,  123,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,
   63,   -1,   58,   59,  124,  125,   -1,   63,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   -1,  266,  267,  268,  269,   41,  271,  272,   44,   -1,
   93,   94,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,
   -1,   41,   58,   59,   44,   -1,   -1,   63,   -1,   -1,
  266,  267,  268,  269,   -1,  271,  272,   -1,   58,   59,
   -1,  124,  125,   63,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,  266,
  267,  268,  269,   93,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,
   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,  266,  267,
  268,  269,   -1,  271,  272,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  268,  269,
   -1,  271,  272,  285,   -1,   -1,   -1,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,   -1,   -1,
  312,  313,  314,  315,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  268,  269,   -1,  271,  272,
   -1,   -1,  268,  269,  285,  271,  272,   -1,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,   -1,
   -1,  312,  313,  314,  315,  316,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,  272,   -1,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,   -1,   -1,  312,  313,  314,
  315,
  };

#line 1034 "CParser.jay"

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
