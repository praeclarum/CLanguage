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
#line 549 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 140:
#line 550 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 142:
  case_142();
  break;
case 143:
#line 569 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 144:
#line 573 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 145:
#line 577 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 146:
#line 581 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 147:
#line 585 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 148:
#line 589 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 149:
#line 593 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 150:
#line 597 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 151:
#line 601 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 152:
  case_152();
  break;
case 153:
#line 613 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 154:
#line 617 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 155:
#line 618 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 156:
#line 619 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 157:
#line 620 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 158:
#line 624 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 159:
#line 628 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 160:
#line 632 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 161:
#line 633 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 162:
#line 634 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 163:
#line 641 "CParser.jay"
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
#line 669 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 168:
#line 673 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 169:
#line 677 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 170:
#line 681 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 171:
  case_171();
  break;
case 172:
  case_172();
  break;
case 173:
#line 703 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 174:
#line 707 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 175:
#line 714 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 177:
#line 719 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 178:
  case_178();
  break;
case 179:
#line 738 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 180:
#line 742 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 181:
#line 746 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 182:
#line 750 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 183:
#line 754 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 184:
#line 758 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 185:
#line 762 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 186:
#line 766 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 187:
#line 770 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 188:
#line 774 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 189:
#line 781 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 190:
#line 785 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 191:
#line 789 "CParser.jay"
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
#line 829 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 210:
#line 861 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 211:
#line 865 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 212:
#line 869 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 213:
#line 870 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 217:
#line 880 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Public); }
  break;
case 218:
#line 881 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Private); }
  break;
case 219:
#line 882 "CParser.jay"
  { yyVal = new VisibilityStatement(DeclarationsVisibility.Protected); }
  break;
case 220:
#line 889 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 221:
#line 893 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 222:
#line 900 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 223:
#line 904 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 225:
#line 912 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 226:
#line 916 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 227:
#line 920 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 228:
#line 924 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 229:
#line 928 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 230:
#line 932 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 232:
#line 940 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 233:
#line 944 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 234:
#line 948 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 235:
#line 952 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 236:
  case_236();
  break;
case 237:
  case_237();
  break;
case 241:
  case_241();
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

void case_142()
#line 555 "CParser.jay"
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

void case_152()
#line 603 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_164()
#line 643 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_165()
#line 652 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_166()
#line 658 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_171()
#line 686 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_172()
#line 692 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_178()
#line 724 "CParser.jay"
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
#line 794 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_193()
#line 801 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_194()
#line 809 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_195()
#line 816 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_236()
#line 957 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_237()
#line 962 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_241()
#line 976 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_242()
#line 985 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_243()
#line 997 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_244()
#line 1003 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_245()
#line 1009 "CParser.jay"
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
   43,   43,   43,   43,   43,   43,   43,   43,   43,   43,
   43,   43,   43,   42,   42,   42,   42,   45,   45,   29,
   29,   29,   46,   46,   48,   48,   49,   49,   49,   49,
   47,   47,    5,    5,   50,   50,   50,   51,   51,   51,
   51,   51,   51,   51,   51,   51,   51,   51,   33,   33,
   33,    6,    6,    6,    6,   52,   53,   53,   54,   54,
   55,   55,   55,   55,   55,   55,   56,   56,   56,   38,
   38,   61,   61,   62,   62,   62,   63,   63,   63,   57,
   57,   58,   58,   58,   59,   59,   59,   59,   59,   59,
   60,   60,   60,   60,   60,    0,    0,   64,   64,   64,
   65,   65,   66,   66,   66,   67,   67,   67,
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
    2,    1,    3,    1,    3,    1,    2,    1,    1,    3,
    1,    3,    5,    4,    4,    6,    6,    5,    4,    3,
    4,    4,    3,    1,    2,    2,    3,    1,    2,    1,
    1,    1,    1,    3,    1,    3,    2,    4,    2,    1,
    1,    3,    1,    2,    1,    1,    2,    3,    2,    3,
    3,    4,    3,    4,    2,    3,    3,    4,    1,    3,
    4,    1,    2,    3,    4,    2,    1,    2,    3,    2,
    1,    1,    1,    1,    1,    1,    3,    4,    3,    2,
    3,    1,    2,    1,    1,    1,    2,    2,    2,    1,
    2,    5,    7,    5,    5,    7,    6,    7,    6,    7,
    3,    2,    2,    2,    3,    1,    2,    1,    1,    1,
    4,    3,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  114,   95,   96,   97,   98,   99,  136,  161,  101,  102,
  103,  104,  107,  108,  105,  106,  160,  162,  100,  109,
  110,  111,  120,  121,  122,    0,  240,    0,  239,    0,
    0,    0,    0,    0,  112,  113,    0,  236,  238,  115,
  116,    0,    0,  237,  139,    0,    0,   81,    0,   91,
    0,    0,    0,    0,   84,   86,   88,   90,    0,    0,
  118,    0,    0,  132,    0,    0,  158,  156,    0,    0,
   82,  246,    0,  247,  248,  243,    0,  242,    0,    0,
    0,    0,    0,    0,    0,    2,    3,    0,    0,    0,
    0,    0,    0,    4,    5,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  210,    0,    0,
   27,   28,   29,   30,  220,    7,    0,    0,   78,    0,
   33,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   63,  214,  202,  215,  201,  203,  204,  205,
  206,    0,  212,  216,  117,    0,  127,    0,    0,  142,
  159,  157,   92,    0,    1,    0,  189,   94,  245,  241,
  244,  171,  153,    0,    0,    0,    0,  165,    0,  150,
    0,    0,    0,  140,    0,    0,   25,    0,   20,   21,
  217,  218,  219,   31,   80,    0,    0,    0,    0,    0,
    0,    0,    0,  232,  233,  234,    0,    0,    0,    0,
    0,    0,   22,   23,    0,  221,    0,   13,   14,    0,
    0,    0,   66,   67,   68,   69,   70,   71,   72,   73,
   74,   75,   76,   77,   65,    0,   24,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  211,  213,  135,  129,
  133,  128,    0,    0,    0,    0,  192,    0,    0,  197,
    0,    0,    0,    0,  169,    0,  151,  152,    0,    0,
    0,  149,  145,    0,  144,    0,    0,  207,    0,    0,
    0,  209,    0,    0,    0,    0,    0,    0,  231,  235,
    6,    0,  123,  125,    0,    0,  174,   79,   12,    9,
    0,   17,    0,   11,   64,   34,   35,   36,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  130,    0,  200,  190,    0,  193,
  196,  198,  185,    0,    0,  179,    0,    0,    0,    0,
    0,    0,  172,  164,  166,    0,    0,  148,  143,    0,
    0,  208,    0,    0,    0,    0,    0,    0,    0,   32,
   10,    0,    8,    0,  199,  191,  194,    0,  186,  178,
  183,  180,  168,  187,    0,  181,    0,    0,  146,  147,
    0,  224,  225,    0,    0,    0,    0,    0,    0,   18,
   62,  195,  188,  184,  182,    0,    0,  229,    0,  227,
    0,   15,    0,  223,  226,  230,  228,   16,
  };
  protected static readonly short [] yyDgoto  = {            28,
  116,  117,  118,  301,  199,  256,  119,  120,  121,  122,
  123,  124,  125,  126,  127,  128,  129,  130,  131,  132,
  133,  226,  186,   29,   77,   49,   31,   32,   33,   34,
   50,   66,  257,   35,   36,   43,   37,  135,  202,   63,
   64,   52,   53,   54,   69,  334,  166,  167,  168,  335,
  266,  258,  259,  260,  136,  137,  138,  139,  140,  141,
  142,  143,  144,   38,   39,   79,   80,
  };
  protected static readonly short [] yySindex = {         1794,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -98,    0, 1794,    0,  -16,
 2724, 2724, 2724, 2724,    0,    0,  -64,    0,    0,    0,
    0, -216,  -75,    0,    0,  -25,  -38,    0,  140,    0,
  521,  -28,  -29, -209,    0,    0,    0,    0,   86,  -58,
    0,   16,   11,    0, -216,   69,    0,    0,  -38,  -25,
    0,    0,  -24,    0,    0,    0,  -16,    0, 2661, 2724,
  -29, 1759, 1044, -116,  108,    0,    0, 1496, 1536, 1536,
  120,  125,  148,    0,    0, 1552,  197,  225,  237,  267,
  536,  269,   70,  303,  311,  185,  468,    0, 1552, 1552,
    0,    0,    0,    0,    0,    0,  209,   52,    0,   77,
    0, 1552,   27,  204, -147,  102,  -26,  364,  312,  297,
  168,  -55,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  165,    0,    0,    0, 1552,    0, -115,   14,    0,
    0,    0,    0,  380,    0, 1063,    0,    0,    0,    0,
    0,    0,    0,   67,  405,  172,  441,    0,   30,    0,
  479,  401, 1055,    0,  536,  468,    0,  468,    0,    0,
    0,    0,    0,    0,    0,  447,  536, 1552, 1552, 1552,
  187,  981,  455,    0,    0,    0,  253,  277,  475,  751,
  751,  149,    0,    0, 1552,    0,  263,    0,    0, 1133,
 1552,  266,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 1552,    0, 1552, 1552, 1552,
 1552, 1552, 1552, 1552, 1552, 1552, 1552, 1552, 1552, 1552,
 1552, 1552, 1552, 1552, 1552, 1552,    0,    0,    0,    0,
    0,    0,  -89, 1552,  316,   40,    0,  -24,  154,    0,
 1697, 1291,  499,   -9,    0,  131,    0,    0,  318, 2692,
 1119,    0,    0, 1552,    0, 1361,  484,    0,  542,  546,
  536,    0,  282,  288,  323,  551, 1393, 1393,    0,    0,
    0, 1417,    0,    0, 1728,  146,    0,    0,    0,    0,
  374,    0,   13,    0,    0,    0,    0,    0,   27,   27,
  204,  204, -147, -147, -147, -147,  102,  102,  -26,  364,
  312,  297,  168,  202,    0,  504,    0,    0,  953,    0,
    0,    0,    0,  558,  559,    0, 1430,  513, 1552,  131,
 1666, 1438,    0,    0,    0,  514,  516,    0,    0,  487,
  487,    0,  536,  536,  536, 1552, 1451, 1462, 1063,    0,
    0, 1552,    0, 1552,    0,    0,    0,  -24,    0,    0,
    0,    0,    0,    0,  571,    0, 1473,  524,    0,    0,
  294,    0,    0,  375,  536,  376,  536,  385,   44,    0,
    0,    0,    0,    0,    0,  536,  563,    0,  536,    0,
  536,    0,  989,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  390,  544,  587,  653,    0,    0,    0,    0,    0,    0,
    0,    0, 1595,    0,    0,    0,   53,    0,    0,    0,
  272,    0,  622,  580,    0,    0,    0,    0,    0, 1626,
    0,   56,    0,    0,    0,    0,    0,    0,  552,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  663,    0,    0,    0, 2077,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 2104,    0, 2200,
    0,    0, 2240, 2432, 2514, 2670,   22,  567, 1136,  -21,
  -14,  138,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  272,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  392,    0,    0,  591,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   72,
  135,  592,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  394,  396,    0,  463,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  593,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 2269, 2354,
 2455, 2506, 2543, 2551, 2591, 2619, 2783, 2790,  560, 1097,
 2362,   32,   12,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  474,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 2162,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  240,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -56,    0,    0,  103,  278,  -70,  891, 1960,    0,
  -78,  107,  101,  331,  395,  400,  402,  404,  393,    0,
  -94,    0, -106,  -12,    1,    0,    0,   -6,  533,    0,
  579,   97,  -73,    0,    0,  613,    0,  268,  389,  586,
 -102,  -41,  -45,    0,    4,  -77,    0,    0,  382,   50,
 -175, -251,    0,  397,  -79,    0, -138,    0,    0,    0,
    0,  522,    0,  627,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           158,
   30,  185,  157,   47,  165,   68,   81,  246,  114,  250,
   82,   46,  172,  109,   46,  107,   47,  110,  111,   57,
  112,  191,   57,   46,   42,   47,   59,  152,   30,   59,
  261,   55,   56,   57,   58,  325,   57,   57,   76,  249,
   62,   57,   48,   59,   59,  251,  134,   65,   59,  197,
  198,  185,   60,  288,  148,   60,  205,  253,   59,   51,
   84,   83,   51,  230,   59,   51,  159,  161,  228,   60,
   60,   57,   58,  229,   60,   58,  146,  368,   59,   51,
   51,  262,  164,  329,   51,  157,  173,  403,  340,   58,
   58,  210,  154,  154,   58,  278,  154,  212,  156,  134,
  200,  113,  277,   57,   60,  363,  261,  282,   47,  150,
   59,  124,  124,  124,   51,   51,  233,  234,  114,  198,
  340,  198,  264,  109,   58,  107,   51,  110,  111,  134,
  112,  283,  284,  285,  298,  147,   60,  225,  252,  302,
  174,   62,  211,  154,  115,   51,   51,  326,  357,  358,
  251,  368,  309,  310,  303,  305,   58,  262,   40,  185,
  296,  237,  124,  238,  328,  175,  154,   62,  402,  200,
  341,  200,  271,  154,  126,  126,  126,  181,   61,  287,
  134,   61,  182,   70,  330,  295,   41,  157,  295,  324,
   47,  338,   40,  200,  200,   61,   61,  114,   71,  255,
  346,  352,  109,  347,  107,  183,  110,  111,   59,  112,
  108,  113,  268,  265,  331,  269,  245,  114,   81,  264,
   41,  342,  109,  115,  107,  126,  110,  111,   45,  112,
   61,   45,  155,   86,   87,   88,  262,   89,   90,  262,
   45,  239,  240,  196,  254,  205,  231,   45,  232,   57,
   57,  297,  205,  296,  187,  367,    8,   59,  157,  364,
  263,  164,   61,  375,  188,   17,   18,  206,  373,  391,
  164,  378,  222,  381,  382,  383,  189,  222,  279,  222,
  280,  222,  222,   60,  222,   94,   95,   59,  157,  247,
  113,  390,   51,   51,  392,  164,  205,  157,  222,  384,
  386,  388,   58,   58,   61,  398,  190,  400,  192,  154,
  113,  290,  207,  208,  209,   93,  404,  291,   78,  406,
  205,  407,  353,   45,    8,  205,  193,  145,  354,  367,
   93,  205,  157,   17,   18,  313,  314,  315,  316,  311,
  312,  164,   85,   86,   87,   88,  160,   89,   90,  213,
  214,  215,  216,  217,  218,  219,  220,  221,  222,  223,
  224,  194,  222,  355,  222,  222,  205,  235,  236,  195,
    1,   91,   92,   93,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   94,   95,   23,   24,   25,
   26,  241,   96,   97,   98,  242,   99,  100,  101,  102,
  103,  104,  105,  106,  361,  397,  399,  362,  205,  205,
  243,   85,   86,   87,   88,  401,   89,   90,  205,   83,
   83,   83,  170,   83,  167,  170,  175,  167,  244,  175,
   73,  155,   86,   87,   88,  267,   89,   90,   83,    1,
   91,   92,   93,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   94,   95,   23,   24,   25,   26,
   83,   96,   97,   98,  270,   99,  100,  101,  102,  103,
  104,  105,  106,  273,   94,   95,  222,  222,  222,  222,
  114,  222,  222,  176,  281,  109,  176,  107,  286,  110,
  111,  114,  112,  289,  177,  292,  109,  177,  107,  299,
  110,  111,  304,  112,  222,  222,  222,  222,  222,  222,
  222,  222,  222,  222,  222,  222,  222,  222,  222,  222,
  222,  222,  222,  222,  222,  222,  222,  222,  222,  222,
  222,  222,  222,  222,  222,   74,  222,  222,  222,  339,
  222,  222,  222,  222,  222,  222,  222,  222,  114,  317,
  318,  272,  327,  109,  343,  107,  349,  110,  111,   67,
  112,   73,  350,   85,   85,   85,  351,   85,  293,  294,
  356,  155,  155,  113,  115,  155,  365,   52,  369,  370,
   52,  151,   85,   52,  113,  372,  379,   53,  380,  359,
   53,  393,   75,  396,  141,   67,  395,   52,   52,  141,
  141,  405,   52,  141,   53,   53,   87,   87,   87,   53,
   87,  163,  173,  175,   85,  319,  389,  323,  141,  201,
  141,  320,  155,   59,  321,   87,   83,  322,  153,   60,
  149,  345,   52,   52,   44,  332,  138,    0,   59,   53,
   53,  113,  138,  248,    0,  138,    0,    0,    0,    0,
  141,  141,    0,    0,    0,    0,    0,   87,    0,    0,
  138,    0,  138,   52,   52,    0,    0,    0,    0,    0,
   53,   53,   89,   89,   89,    0,   89,  137,    0,    0,
    0,   67,  141,  137,    0,  151,  137,    0,  201,    0,
  201,   89,    0,  138,    0,    0,    0,    0,    0,    0,
    0,  137,    0,  137,  155,   86,   87,   88,    0,   89,
   90,    0,  201,  201,    0,  155,   86,   87,   88,    0,
   89,   90,    0,   89,  138,    0,    0,    0,    0,    0,
    0,    0,    1,    0,  137,    0,    0,    0,    0,    0,
    0,    0,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   94,   95,   23,
   24,   25,   26,    0,    0,  137,    0,    0,   94,   95,
    0,    0,   85,   86,   87,   88,    0,   89,   90,    0,
   85,    0,    0,  151,    0,    1,    0,    0,  155,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   52,   52,   23,   24,   25,   26,    0,   53,   53,    0,
    0,    0,    0,   87,    0,   94,   95,    0,    0,   72,
    0,    0,   96,   97,   98,    0,   99,  100,  101,  102,
  103,  104,  105,  106,  141,    0,    0,    0,  141,  141,
  141,  141,  141,  141,  141,  141,  141,  141,  141,  141,
  141,  141,  141,  141,  141,  141,  141,  141,  141,    0,
    0,  141,  141,  141,  141,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  138,    0,  141,   89,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,    0,    0,  138,  138,  138,  138,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  137,    0,    0,
  138,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,    0,    0,  137,  137,  137,  137,  177,  179,
  180,    0,    0,    0,    0,  114,  184,    0,    0,    0,
  109,  137,  107,    0,  110,  111,    0,  112,  255,  184,
  184,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  184,  114,    0,    0,    0,    0,  109,    0,
  107,  114,  110,  111,    0,  112,  109,    0,  107,    0,
  110,  111,    0,  112,  255,    1,  184,    0,    0,  115,
    0,    0,    0,  254,    0,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,  184,   23,   24,   25,   26,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  156,  114,  366,  113,  254,
    0,  109,    0,  107,    0,  171,  111,  114,  112,    0,
    0,    0,  109,    0,  107,  114,  276,  111,    0,  112,
  109,    0,  107,    0,  110,  111,  113,  112,  255,    0,
    0,  156,    0,  408,  113,    0,    0,    0,  184,  184,
  184,  184,  184,  184,  184,  184,  184,  184,  184,  184,
  184,  184,  184,  184,  184,  184,  170,   54,    0,    0,
   54,    0,    0,    0,  184,    0,    0,  275,    0,    0,
    0,  114,    0,  254,   54,   54,  109,    0,  107,   54,
  110,  111,    0,  112,    0,  114,  184,    0,    0,  113,
  109,    0,  107,  300,  110,  111,   55,  112,    0,   55,
  113,    0,  184,    0,    0,  156,    0,    0,  113,   54,
   54,    0,    0,   55,   55,    0,    0,    0,   55,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  155,
   86,   87,   88,    0,   89,   90,    0,    0,    0,    0,
   54,   54,    0,    0,    0,    0,    0,  184,   55,    0,
    0,    0,    0,    0,    0,    0,    0,  155,   86,   87,
   88,    0,   89,   90,  113,  155,   86,   87,   88,    0,
   89,   90,    0,    0,  184,    0,    0,    0,  113,   55,
   55,    0,   94,   95,    0,    1,    0,  184,    0,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   94,   95,   23,   24,   25,   26,    0,    0,   94,   95,
  155,   86,   87,   88,    0,   89,   90,    0,    0,    0,
    0,  155,   86,   87,   88,    0,   89,   90,    0,  155,
   86,   87,   88,  114,   89,   90,    0,    0,  109,    0,
  107,    0,  337,  111,  169,  112,    0,    0,    8,    0,
    0,    0,    0,    0,    0,  274,    0,   17,   18,    8,
    0,    0,    0,   94,   95,    0,    0,    0,   17,   18,
    0,    0,    0,    0,   94,   95,    0,   54,   54,    0,
    0,    0,   94,   95,    0,  155,   86,   87,   88,    0,
   89,   90,    0,  336,    0,    0,    0,    0,    0,  155,
   86,   87,   88,  114,   89,   90,    0,    0,  109,    0,
  107,    0,  110,  111,    0,  112,   55,   55,    0,    0,
    0,    0,    0,    8,    0,    0,  113,    0,    0,    0,
    0,    0,   17,   18,    0,  114,    0,    0,   94,   95,
  109,    0,  107,    0,  110,  111,    0,  112,    0,    0,
    0,    0,   94,   95,    0,    0,    0,    0,    0,  114,
    0,  115,    0,  348,  109,    0,  107,    0,  110,  111,
    0,  112,  114,    0,    0,    0,    0,  109,    0,  107,
  114,  110,  111,    0,  112,  109,    0,  107,    0,  377,
  111,    0,  112,  114,    0,    0,  113,    0,  109,    0,
  107,  385,  110,  111,  114,  112,    0,    0,    0,  109,
    0,  107,  387,  110,  111,  114,  112,    0,    0,    0,
  109,    0,  107,    0,  110,  111,    0,  112,  113,    0,
    0,    0,  371,    0,    0,    0,    0,    0,  114,    0,
  376,    0,    0,  109,    0,  176,    0,  110,  111,  359,
  112,    0,  113,    0,    0,    0,    0,  155,   86,   87,
   88,    0,   89,   90,    0,  113,    0,    0,    0,    0,
    0,    0,    0,  113,    0,  394,    0,    0,  114,    0,
    0,    0,    0,  109,    0,  178,  113,  110,  111,    0,
  112,    0,    0,    0,  114,    0,    0,  113,    0,  109,
    0,  107,    0,  110,  111,    0,  112,    0,  113,    0,
   94,   95,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  155,   86,   87,
   88,  113,   89,   90,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  131,  131,  131,    0,  131,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  155,
   86,   87,   88,  131,   89,   90,    0,    0,    0,    0,
    0,  113,    0,    0,    0,  119,  119,  119,    0,  119,
   94,   95,    0,  155,   86,   87,   88,  113,   89,   90,
    0,    0,    0,    0,  119,  131,  155,   86,   87,   88,
    0,   89,   90,    0,  155,   86,   87,   88,    0,   89,
   90,    0,   94,   95,    0,    0,  374,  155,   86,   87,
   88,    0,   89,   90,    0,    0,  119,    0,  155,   86,
   87,   88,    0,   89,   90,    0,   94,   95,    0,  155,
   86,   87,   88,    0,   89,   90,  261,  333,   47,   94,
   95,    0,    0,    0,    0,    0,    0,   94,   95,    0,
    0,    0,  155,   86,   87,   88,    0,   89,   90,    0,
   94,   95,    0,    0,    0,    0,    0,  295,  333,   47,
    0,   94,   95,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   94,   95,    0,    0,    0,  262,    0,    0,
    0,    0,  155,   86,   87,   88,    0,   89,   90,  163,
    0,    0,    0,    0,    0,   94,   95,    0,  155,   86,
   87,   88,    0,   89,   90,    0,    0,    0,  262,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   94,   95,    0,    0,    0,
    0,  131,   27,    0,    0,    0,    0,    0,    0,    0,
    0,   94,   95,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  131,
    0,    0,  119,  131,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,    0,    0,  131,  131,  131,  131,
  119,    0,    0,    0,  119,  119,  119,  119,  119,  119,
  119,  119,  119,  119,  119,  119,  119,  119,  119,  119,
  119,  119,  119,  119,  119,    0,    0,  119,  119,  119,
  119,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    0,    0,   45,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,    1,    0,    0,    0,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,    0,    0,   23,   24,
   25,   26,    1,    0,    0,  162,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,    0,    0,   23,
   24,   25,   26,    1,    0,    0,    0,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,  203,  204,
   23,   24,   25,   26,    0,    0,    0,    0,    1,    0,
    0,  227,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,    0,
    0,    0,    0,    1,    1,    0,    1,    0,    1,    1,
    1,    1,    1,    1,    0,    0,    0,    0,    0,    0,
  204,    0,    0,    0,    0,    1,    1,    1,    1,    1,
   19,   19,    0,    0,   19,   19,   19,   19,   19,    0,
   19,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   19,   19,   19,   19,   19,   19,    1,    0,    0,
    1,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  306,  307,  308,
    0,    0,    0,    0,    0,    0,   19,   19,   26,   26,
    1,    0,   26,   26,   26,   26,   26,    0,   26,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   26,
   26,   26,   26,   26,   26,    0,    0,   19,   19,    0,
    0,    0,    0,    0,    0,  204,   31,   31,    0,    0,
   31,   31,   31,   31,   31,    0,   31,    0,    0,    0,
    0,  360,    0,    0,   26,   26,    0,   31,   31,   31,
    0,   31,   31,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   37,    0,    0,
   37,    0,   37,   37,   37,   26,   26,    0,    0,    0,
    0,    0,   31,   31,    0,    0,  204,   37,   37,   37,
    0,   37,   37,    0,    0,    0,   38,    0,    0,   38,
    0,   38,   38,   38,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   31,   31,    0,   38,   38,   38,    0,
   38,   38,   37,   37,    0,    0,  204,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    0,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,   38,   38,   37,   37,    0,    0,   19,   19,   19,
   19,   19,   19,    0,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,    0,    0,
    0,   39,   38,   38,   39,    0,   39,   39,   39,    0,
    0,    0,   56,    0,    0,   56,    0,    0,    0,    0,
    0,   39,   39,   39,    0,   39,   39,    0,    0,   56,
   56,    0,    0,    0,   56,   26,   26,   26,   26,   26,
   26,    0,   26,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   39,   39,    0,    0,
    0,    0,    0,    0,   56,    0,    0,    0,    0,    0,
    0,    0,    0,   31,   31,   31,   31,   31,   31,   40,
   31,   31,   40,    0,    0,   40,    0,   39,   39,    0,
    0,    0,    0,    0,    0,   56,   56,    0,    0,   40,
   40,   40,   41,   40,   40,   41,    0,    0,   41,    0,
    0,    0,    0,   37,   37,   37,   37,   37,   37,    0,
   37,   37,   41,   41,   41,    0,   41,   41,    0,    0,
    0,    0,    0,    0,   40,   40,    0,    0,    0,    0,
    0,    0,   38,   38,   38,   38,   38,   38,    0,   38,
   38,    0,    0,   42,    0,    0,   42,   41,   41,   42,
    0,   43,    0,    0,   43,   40,   40,   43,    0,    0,
    0,    0,    0,   42,   42,   42,    0,   42,   42,    0,
    0,   43,   43,   43,    0,   43,   43,    0,   41,   41,
   46,    0,    0,   46,    0,    0,   46,    0,   47,    0,
    0,   47,    0,    0,   47,    0,    0,    0,   42,   42,
   46,   46,   46,    0,   46,   46,   43,   43,   47,   47,
   47,    0,   47,   47,    0,    0,    0,   39,   39,   39,
   39,   39,   39,    0,   39,   39,    0,    0,   44,   42,
   42,   44,   56,   56,   44,   46,   46,   43,   43,    0,
    0,    0,    0,   47,   47,    0,    0,    0,   44,   44,
   44,    0,   44,   44,    0,    0,   45,    0,    0,   45,
    0,    0,   45,    0,    0,    0,   46,   46,    0,    0,
    0,    0,    0,    0,   47,   47,   45,   45,   45,    0,
   45,   45,    0,   44,   44,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   40,   40,   40,   40,   40,
   40,    0,   40,   40,    0,    0,    0,   48,    0,    0,
   48,   45,   45,   48,   44,   44,    0,    0,   41,   41,
   41,   41,   41,   41,    0,   41,   41,   48,   48,    0,
    0,    0,   48,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   45,   45,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   48,   48,    0,    0,    0,    0,    0,   42,
   42,   42,   42,   42,   42,    0,   42,   42,    0,   43,
   43,   43,   43,   59,   43,   43,    0,    0,    0,    0,
    0,    0,    0,   48,   48,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   46,   46,
   46,   46,    0,   46,   46,    0,   47,   47,   47,   47,
   49,   47,   47,   49,    0,    0,   49,   50,    0,    0,
   50,    0,    0,   50,    0,    0,    0,    0,    0,    0,
   49,   49,    0,    0,    0,   49,    0,   50,   50,    0,
    0,    0,   50,    0,    0,    0,   44,   44,   44,   44,
    0,   44,   44,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   49,   49,    0,    0,    0,
    0,    0,   50,   50,   45,   45,   45,   45,    0,   45,
   45,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   49,   49,    0,    0,
    0,    0,    0,   50,   50,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   48,   48,    0,
   48,   48,    0,    0,    0,    1,    0,    0,    0,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    1,    0,    0,    0,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,  344,    1,    0,
    0,    0,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   49,   49,    0,   49,   49,    0,    0,   50,   50,    0,
   50,   50,
  };
  protected static readonly short [] yyCheck = {            73,
    0,   96,   73,   42,   82,   47,   52,   63,   33,  125,
   40,   40,   83,   38,   40,   40,   42,   42,   43,   41,
   45,  101,   44,   40,  123,   42,   41,   69,   28,   44,
   40,   31,   32,   33,   34,  125,   58,   59,   51,  146,
  257,   63,   59,   58,   59,  148,   59,  123,   63,  106,
  107,  146,   41,  192,   44,   44,   44,   44,  123,   38,
  270,   91,   41,   37,  123,   44,   79,   80,   42,   58,
   59,   93,   41,   47,   63,   44,   61,  329,   93,   58,
   59,   91,   82,   44,   63,  156,   83,   44,  264,   58,
   59,   40,   40,   41,   63,  175,   44,   46,  123,   44,
  107,  126,  173,  125,   93,   93,   40,  187,   42,   41,
  125,   40,   41,   42,   93,   94,  264,  265,   33,  176,
  296,  178,  164,   38,   93,   40,   30,   42,   43,  142,
   45,  188,  189,  190,  205,  125,  125,   61,  125,  210,
  257,  257,   91,   91,   59,  124,  125,  254,  287,  288,
  253,  403,  231,  232,  211,  226,  125,   91,  257,  254,
  202,   60,   91,   62,  125,   58,   70,  257,  125,  176,
   40,  178,  169,   77,   40,   41,   42,   58,   41,  192,
  125,   44,   58,   44,  258,   40,  285,  258,   40,  246,
   42,  262,  257,  200,  201,   58,   59,   33,   59,   46,
  271,  281,   38,  274,   40,   58,   42,   43,  123,   45,
  125,  126,   41,  164,   61,   44,  272,   33,  264,  261,
  285,   91,   38,   59,   40,   91,   42,   43,  257,   45,
   93,  257,  257,  258,  259,  260,   91,  262,  263,   91,
  257,  268,  269,   59,   91,   44,   43,  257,   45,  271,
  272,  202,   44,  295,   58,  329,  295,  272,  329,   58,
  164,  261,  125,  341,   40,  304,  305,   59,  339,  364,
  270,  342,   33,  353,  354,  355,   40,   38,  176,   40,
  178,   42,   43,  272,   45,  310,  311,  123,  359,  125,
  126,  362,  271,  272,  368,  295,   44,  368,   59,  356,
  357,  358,  271,  272,   37,  385,   40,  387,   40,  257,
  126,   59,  261,  262,  263,   44,  396,   41,   51,  399,
   44,  401,   41,  257,  295,   44,  257,   60,   41,  403,
   59,   44,  403,  304,  305,  235,  236,  237,  238,  233,
  234,  341,  257,  258,  259,  260,   79,  262,  263,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   59,  123,   41,  125,  126,   44,  266,  267,   59,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,  312,  313,  314,
  315,   38,  317,  318,  319,   94,  321,  322,  323,  324,
  325,  326,  327,  328,   41,   41,   41,   44,   44,   44,
  124,  257,  258,  259,  260,   41,  262,  263,   44,   40,
   41,   42,   41,   44,   41,   44,   41,   44,  271,   44,
   61,  257,  258,  259,  260,   41,  262,  263,   59,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,  312,  313,  314,  315,
   91,  317,  318,  319,   44,  321,  322,  323,  324,  325,
  326,  327,  328,   93,  310,  311,  257,  258,  259,  260,
   33,  262,  263,   41,   58,   38,   44,   40,  322,   42,
   43,   33,   45,   59,   41,   41,   38,   44,   40,  257,
   42,   43,  257,   45,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,  313,  314,  315,   35,  317,  318,  319,   61,
  321,  322,  323,  324,  325,  326,  327,  328,   33,  239,
  240,   93,  257,   38,  257,   40,   93,   42,   43,   47,
   45,   61,   41,   40,   41,   42,   41,   44,  200,  201,
   40,   40,   41,  126,   59,   44,   93,   38,   41,   41,
   41,   69,   59,   44,  126,   93,   93,   41,   93,  123,
   44,   41,   92,  320,   35,   83,   93,   58,   59,   40,
   41,   59,   63,   44,   58,   59,   40,   41,   42,   63,
   44,   41,   41,   41,   91,  241,  359,  245,   59,  107,
   61,  242,   91,  123,  243,   59,  257,  244,   70,   37,
   65,  270,   93,   94,   28,  259,   35,   -1,  123,   93,
   94,  126,   41,  142,   -1,   44,   -1,   -1,   -1,   -1,
   91,   92,   -1,   -1,   -1,   -1,   -1,   91,   -1,   -1,
   59,   -1,   61,  124,  125,   -1,   -1,   -1,   -1,   -1,
  124,  125,   40,   41,   42,   -1,   44,   35,   -1,   -1,
   -1,  169,  123,   41,   -1,  173,   44,   -1,  176,   -1,
  178,   59,   -1,   92,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   -1,   61,  257,  258,  259,  260,   -1,  262,
  263,   -1,  200,  201,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   91,  123,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   92,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
  313,  314,  315,   -1,   -1,  123,   -1,   -1,  310,  311,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  257,   -1,   -1,  271,   -1,  285,   -1,   -1,  257,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  271,  272,  312,  313,  314,  315,   -1,  271,  272,   -1,
   -1,   -1,   -1,  257,   -1,  310,  311,   -1,   -1,  329,
   -1,   -1,  317,  318,  319,   -1,  321,  322,  323,  324,
  325,  326,  327,  328,  285,   -1,   -1,   -1,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,   -1,
   -1,  312,  313,  314,  315,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,  329,  257,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,   -1,   -1,  312,  313,  314,  315,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,
  329,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,   -1,   -1,  312,  313,  314,  315,   88,   89,
   90,   -1,   -1,   -1,   -1,   33,   96,   -1,   -1,   -1,
   38,  329,   40,   -1,   42,   43,   -1,   45,   46,  109,
  110,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  122,   33,   -1,   -1,   -1,   -1,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   46,  285,  146,   -1,   -1,   59,
   -1,   -1,   -1,   91,   -1,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
   -1,  171,  312,  313,  314,  315,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  123,   33,  125,  126,   91,
   -1,   38,   -1,   40,   -1,   42,   43,   33,   45,   -1,
   -1,   -1,   38,   -1,   40,   33,   42,   43,   -1,   45,
   38,   -1,   40,   -1,   42,   43,  126,   45,   46,   -1,
   -1,  123,   -1,  125,  126,   -1,   -1,   -1,  228,  229,
  230,  231,  232,  233,  234,  235,  236,  237,  238,  239,
  240,  241,  242,  243,  244,  245,   93,   41,   -1,   -1,
   44,   -1,   -1,   -1,  254,   -1,   -1,   93,   -1,   -1,
   -1,   33,   -1,   91,   58,   59,   38,   -1,   40,   63,
   42,   43,   -1,   45,   -1,   33,  276,   -1,   -1,  126,
   38,   -1,   40,   41,   42,   43,   41,   45,   -1,   44,
  126,   -1,  292,   -1,   -1,  123,   -1,   -1,  126,   93,
   94,   -1,   -1,   58,   59,   -1,   -1,   -1,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,  337,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,  126,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,  364,   -1,   -1,   -1,  126,  124,
  125,   -1,  310,  311,   -1,  285,   -1,  377,   -1,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,  313,  314,  315,   -1,   -1,  310,  311,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,  257,
  258,  259,  260,   33,  262,  263,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,  291,   45,   -1,   -1,  295,   -1,
   -1,   -1,   -1,   -1,   -1,  291,   -1,  304,  305,  295,
   -1,   -1,   -1,  310,  311,   -1,   -1,   -1,  304,  305,
   -1,   -1,   -1,   -1,  310,  311,   -1,  271,  272,   -1,
   -1,   -1,  310,  311,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   93,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   33,  262,  263,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,  271,  272,   -1,   -1,
   -1,   -1,   -1,  295,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   -1,  304,  305,   -1,   33,   -1,   -1,  310,  311,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,  310,  311,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   59,   -1,   93,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   33,   -1,   -1,  126,   -1,   38,   -1,
   40,   41,   42,   43,   33,   45,   -1,   -1,   -1,   38,
   -1,   40,   41,   42,   43,   33,   45,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,  126,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   33,   -1,
   93,   -1,   -1,   38,   -1,   40,   -1,   42,   43,  123,
   45,   -1,  126,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  126,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  126,   -1,   93,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,  126,   42,   43,   -1,
   45,   -1,   -1,   -1,   33,   -1,   -1,  126,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,  126,   -1,
  310,  311,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,  126,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   59,  262,  263,   -1,   -1,   -1,   -1,
   -1,  126,   -1,   -1,   -1,   40,   41,   42,   -1,   44,
  310,  311,   -1,  257,  258,  259,  260,  126,  262,  263,
   -1,   -1,   -1,   -1,   59,   91,  257,  258,  259,  260,
   -1,  262,  263,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,  310,  311,   -1,   -1,   41,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   91,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,  310,  311,   -1,  257,
  258,  259,  260,   -1,  262,  263,   40,   41,   42,  310,
  311,   -1,   -1,   -1,   -1,   -1,   -1,  310,  311,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  310,  311,   -1,   -1,   -1,   -1,   -1,   40,   41,   42,
   -1,  310,  311,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  310,  311,   -1,   -1,   -1,   91,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   41,
   -1,   -1,   -1,   -1,   -1,  310,  311,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   91,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  310,  311,   -1,   -1,   -1,
   -1,  257,   59,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  310,  311,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,
   -1,   -1,  257,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,   -1,   -1,  312,  313,  314,  315,
  285,   -1,   -1,   -1,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,   -1,   -1,  312,  313,  314,
  315,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  285,   -1,   -1,  257,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,   -1,   -1,  312,  313,  314,
  315,  285,   -1,   -1,   -1,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,   -1,   -1,  312,  313,
  314,  315,  285,   -1,   -1,  257,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,   -1,   -1,  312,
  313,  314,  315,  285,   -1,   -1,   -1,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  109,  110,
  312,  313,  314,  315,   -1,   -1,   -1,   -1,  285,   -1,
   -1,  122,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,   -1,   -1,  312,  313,  314,  315,   -1,
   -1,   -1,   -1,   37,   38,   -1,   40,   -1,   42,   43,
   44,   45,   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,
  171,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   63,
   37,   38,   -1,   -1,   41,   42,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   61,   62,   63,   91,   -1,   -1,
   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  228,  229,  230,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   37,   38,
  124,   -1,   41,   42,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,
   59,   60,   61,   62,   63,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   -1,   -1,  276,   37,   38,   -1,   -1,
   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,  292,   -1,   -1,   93,   94,   -1,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   43,   44,   45,  124,  125,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,  337,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   38,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   58,   59,   60,   -1,
   62,   63,   93,   94,   -1,   -1,  377,  261,  262,  263,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   93,   94,  124,  125,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,  282,  283,  284,   -1,   -1,
   -1,   38,  124,  125,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,   58,
   59,   -1,   -1,   -1,   63,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,  273,  274,  275,  276,  277,  278,
  279,  280,  281,  282,  283,  284,   93,   94,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,   38,
  271,  272,   41,   -1,   -1,   44,   -1,  124,  125,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   58,
   59,   60,   38,   62,   63,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,   38,   -1,   -1,   41,   93,   94,   44,
   -1,   38,   -1,   -1,   41,  124,  125,   44,   -1,   -1,
   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,  124,  125,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   93,   94,
   58,   59,   60,   -1,   62,   63,   93,   94,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   38,  124,
  125,   41,  271,  272,   44,   93,   94,  124,  125,   -1,
   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   58,   59,   60,   -1,
   62,   63,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,   -1,   38,   -1,   -1,
   41,   93,   94,   44,  124,  125,   -1,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,   58,   59,   -1,
   -1,   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   -1,  266,
  267,  268,  269,  123,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  266,  267,
  268,  269,   -1,  271,  272,   -1,  266,  267,  268,  269,
   38,  271,  272,   41,   -1,   -1,   44,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   -1,   -1,   -1,   63,   -1,   58,   59,   -1,
   -1,   -1,   63,   -1,   -1,   -1,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,
   -1,   -1,   93,   94,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  268,  269,   -1,
  271,  272,   -1,   -1,   -1,  285,   -1,   -1,   -1,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
   -1,   -1,  312,  313,  314,  315,  285,   -1,   -1,   -1,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,   -1,   -1,  312,  313,  314,  315,  316,  285,   -1,
   -1,   -1,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,   -1,   -1,  312,  313,  314,  315,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  268,  269,   -1,  271,  272,   -1,   -1,  268,  269,   -1,
  271,  272,
  };

#line 1025 "CParser.jay"

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
