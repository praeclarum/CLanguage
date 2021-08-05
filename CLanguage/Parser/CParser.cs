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
    "AND_ASSIGN","OR_ASSIGN","TYPE_NAME","TYPEDEF","EXTERN","STATIC",
    "AUTO","REGISTER","INLINE","RESTRICT","CHAR","SHORT","INT","LONG",
    "SIGNED","UNSIGNED","FLOAT","DOUBLE","CONST","VOLATILE","VOID","BOOL",
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
#line 45 "CParser.jay"
  { var t = lexer.CurrentToken; yyVal = new VariableExpression((yyVals[0+yyTop]).ToString(), t.Location, t.EndLocation); }
  break;
case 2:
#line 46 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 47 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 48 "CParser.jay"
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 49 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 50 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 57 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 61 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 65 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 69 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 73 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 77 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
	}
  break;
case 13:
#line 81 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 85 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 89 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 93 "CParser.jay"
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
#line 115 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 119 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 123 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 127 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 131 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 135 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 139 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 143 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 147 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 148 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 149 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 150 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 157 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 161 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 168 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 172 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 176 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 180 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 188 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 192 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 200 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 204 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 212 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 216 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 220 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 224 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 232 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 236 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 244 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 252 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 260 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 268 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 276 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 283 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 287 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 312 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 313 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 314 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 315 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 316 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 317 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 318 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 319 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 320 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 321 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 322 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 323 "CParser.jay"
  { yyVal = LogicOp.And; }
  break;
case 77:
#line 324 "CParser.jay"
  { yyVal = LogicOp.Or; }
  break;
case 78:
#line 331 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 79:
#line 335 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 81:
#line 346 "CParser.jay"
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
#line 425 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[0+yyTop], null);
	}
  break;
case 94:
#line 429 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[-2+yyTop], (Initializer)yyVals[0+yyTop]);
	}
  break;
case 95:
#line 433 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 96:
#line 434 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 97:
#line 435 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 98:
#line 436 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 99:
#line 437 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 100:
#line 441 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 101:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 102:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 103:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 104:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 105:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 106:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 107:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 108:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 109:
#line 450 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 110:
#line 451 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 111:
#line 452 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 114:
#line 455 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 117:
#line 464 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 118:
#line 465 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 119:
#line 466 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 120:
#line 470 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 121:
#line 471 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 122:
#line 472 "CParser.jay"
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
#line 501 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 128:
#line 502 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 129:
#line 503 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 130:
#line 504 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 131:
#line 505 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 132:
  case_132();
  break;
case 133:
  case_133();
  break;
case 134:
#line 527 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 135:
#line 531 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 136:
#line 535 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 137:
#line 542 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 138:
#line 543 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 139:
#line 548 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 140:
#line 549 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 142:
  case_142();
  break;
case 143:
#line 568 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 144:
#line 572 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 145:
#line 576 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 146:
#line 580 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 147:
#line 584 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 148:
#line 588 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 149:
#line 592 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 150:
#line 596 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 151:
#line 600 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 152:
  case_152();
  break;
case 153:
#line 612 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 154:
#line 616 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 155:
#line 617 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 156:
#line 618 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 157:
#line 619 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 158:
#line 623 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 159:
#line 627 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 160:
#line 631 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 161:
#line 632 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 162:
#line 633 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 163:
#line 640 "CParser.jay"
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
#line 668 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 168:
#line 672 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 169:
#line 676 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 170:
#line 680 "CParser.jay"
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
#line 702 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 174:
#line 706 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 175:
#line 713 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 177:
#line 718 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 178:
  case_178();
  break;
case 179:
#line 737 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 180:
#line 741 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 181:
#line 745 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 182:
#line 749 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 183:
#line 753 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 184:
#line 757 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 185:
#line 761 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 186:
#line 765 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 187:
#line 769 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 188:
#line 773 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 189:
#line 780 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 190:
#line 784 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 191:
#line 788 "CParser.jay"
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
#line 828 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 210:
#line 860 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 211:
#line 864 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 212:
#line 868 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 213:
#line 869 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 216:
#line 881 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 217:
#line 885 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 218:
#line 892 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 219:
#line 896 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 221:
#line 904 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 222:
#line 908 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 223:
#line 912 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 224:
#line 916 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 225:
#line 920 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 226:
#line 924 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 228:
#line 932 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 229:
#line 936 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 230:
#line 940 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 231:
#line 944 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 232:
  case_232();
  break;
case 233:
  case_233();
  break;
case 237:
  case_237();
  break;
case 238:
  case_238();
  break;
case 239:
  case_239();
  break;
case 240:
  case_240();
  break;
case 241:
  case_241();
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
#line 98 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 104 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 293 "CParser.jay"
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
#line 348 "CParser.jay"
{
		DeclarationSpecifiers ds = (DeclarationSpecifiers)yyVals[-2+yyTop];
		List<InitDeclarator> decls = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = new MultiDeclaratorStatement (ds, decls);
	}

void case_83()
#line 357 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_84()
#line 363 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];		
		yyVal = ds;
	}

void case_85()
#line 369 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_86()
#line 375 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_87()
#line 381 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 387 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 393 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_90()
#line 399 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_91()
#line 408 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_92()
#line 414 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_123()
#line 477 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_124()
#line 482 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_125()
#line 488 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_126()
#line 493 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_132()
#line 510 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_133()
#line 516 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_142()
#line 554 "CParser.jay"
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
#line 602 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_164()
#line 642 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_165()
#line 651 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_166()
#line 657 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_171()
#line 685 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_172()
#line 691 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_178()
#line 723 "CParser.jay"
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
#line 793 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_193()
#line 800 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_194()
#line 808 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_195()
#line 815 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_232()
#line 949 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_233()
#line 954 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_237()
#line 968 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_238()
#line 977 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_239()
#line 989 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_240()
#line 995 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_241()
#line 1001 "CParser.jay"
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
   38,   61,   61,   62,   62,   57,   57,   58,   58,   58,
   59,   59,   59,   59,   59,   59,   60,   60,   60,   60,
   60,    0,    0,   63,   63,   63,   64,   64,   65,   65,
   65,   66,   66,   66,
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
    3,    1,    2,    1,    1,    1,    2,    5,    7,    5,
    5,    7,    6,    7,    6,    7,    3,    2,    2,    2,
    3,    1,    2,    1,    1,    1,    4,    3,    1,    2,
    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  114,   95,   96,   97,   98,   99,  136,  161,  101,  102,
  103,  104,  107,  108,  105,  106,  160,  162,  100,  109,
  110,  111,  120,  121,  122,    0,  236,    0,  235,    0,
    0,    0,    0,    0,  112,  113,    0,  232,  234,  115,
  116,    0,    0,  233,  139,    0,    0,   81,    0,   91,
    0,    0,    0,    0,   84,   86,   88,   90,    0,    0,
  118,    0,    0,  132,    0,    0,  158,  156,    0,    0,
   82,  242,    0,  243,  244,  239,    0,  238,    0,    0,
    0,    0,    0,    0,    0,    2,    3,    0,    0,    0,
    4,    5,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  210,    0,    0,   27,   28,   29,
   30,  216,    7,    0,    0,   78,    0,   33,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   63,
  214,  202,  215,  201,  203,  204,  205,  206,    0,  212,
  117,    0,  127,    0,    0,  142,  159,  157,   92,    0,
    1,    0,  189,   94,  241,  237,  240,  171,  153,    0,
    0,    0,    0,  165,    0,  150,    0,    0,    0,  140,
    0,    0,   25,    0,   20,   21,   31,   80,    0,    0,
    0,    0,    0,    0,    0,    0,  228,  229,  230,    0,
    0,    0,    0,    0,    0,   22,   23,    0,  217,    0,
   13,   14,    0,    0,    0,   66,   67,   68,   69,   70,
   71,   72,   73,   74,   75,   76,   77,   65,    0,   24,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  211,
  213,  135,  129,  133,  128,    0,    0,    0,    0,  192,
    0,    0,  197,    0,    0,    0,    0,  169,    0,  151,
  152,    0,    0,    0,  149,  145,    0,  144,    0,    0,
  207,    0,    0,    0,  209,    0,    0,    0,    0,    0,
    0,  227,  231,    6,    0,  123,  125,    0,    0,  174,
   79,   12,    9,    0,   17,    0,   11,   64,   34,   35,
   36,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  130,    0,  200,
  190,    0,  193,  196,  198,  185,    0,    0,  179,    0,
    0,    0,    0,    0,    0,  172,  164,  166,    0,    0,
  148,  143,    0,    0,  208,    0,    0,    0,    0,    0,
    0,    0,   32,   10,    0,    8,    0,  199,  191,  194,
    0,  186,  178,  183,  180,  168,  187,    0,  181,    0,
    0,  146,  147,    0,  220,  221,    0,    0,    0,    0,
    0,    0,   18,   62,  195,  188,  184,  182,    0,    0,
  225,    0,  223,    0,   15,    0,  219,  222,  226,  224,
   16,
  };
  protected static readonly short [] yyDgoto  = {            28,
  113,  114,  115,  294,  192,  249,  116,  117,  118,  119,
  120,  121,  122,  123,  124,  125,  126,  127,  128,  129,
  130,  219,  179,   29,   77,   49,   31,   32,   33,   34,
   50,   66,  250,   35,   36,   43,   37,  132,  195,   63,
   64,   52,   53,   54,   69,  327,  162,  163,  164,  328,
  259,  251,  252,  253,  133,  134,  135,  136,  137,  138,
  139,  140,   38,   39,   79,   80,
  };
  protected static readonly short [] yySindex = {         1840,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -93,    0, 1840,    0,   26,
 2713, 2713, 2713, 2713,    0,    0,  -78,    0,    0,    0,
    0, -206,  -68,    0,    0,   30,  -29,    0,  112,    0,
  684,  -14,   53, -182,    0,    0,    0,    0,   74,  -12,
    0,   57,    4,    0, -206,   96,    0,    0,  -29,   30,
    0,    0,   44,    0,    0,    0,   26,    0, 2609, 2713,
   53, 1784,  908, -115,   91,    0,    0, 1509, 1540, 1540,
    0,    0, 1598,  101,  144,  156,  172,  373,  176,  -52,
  162,  169,  781, 1020,    0, 1598, 1598,    0,    0,    0,
    0,    0,    0,  143,  -37,    0,  554,    0, 1598,  258,
  253,   13,   48,   73,  196,  154,  130,   21,  -55,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  168,    0,
    0, 1598,    0,  -83,   20,    0,    0,    0,    0,  197,
    0,  403,    0,    0,    0,    0,    0,    0,    0,   52,
  243,  201,  266,    0,  -21,    0, 1251,  223, 1079,    0,
  373, 1020,    0, 1020,    0,    0,    0,    0,  272,  373,
 1598, 1598, 1598,   29,  964,  332,    0,    0,    0,  188,
  305,  382, 2631, 2631,   60,    0,    0, 1598,    0,  157,
    0,    0,  479, 1598,  187,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1598,    0,
 1598, 1598, 1598, 1598, 1598, 1598, 1598, 1598, 1598, 1598,
 1598, 1598, 1598, 1598, 1598, 1598, 1598, 1598, 1598,    0,
    0,    0,    0,    0,    0,  -76, 1598,  190,   32,    0,
   44,  104,    0, 1699, 1259,  372,  -28,    0,   95,    0,
    0,  194, 2673, 1099,    0,    0, 1598,    0, 1350,  347,
    0,  440,  444,  373,    0,  306,  312,  313,  455, 1159,
 1159,    0,    0,    0, 1370,    0,    0, 1733,  113,    0,
    0,    0,    0,  314,    0,   80,    0,    0,    0,    0,
    0,  258,  258,  253,  253,   13,   13,   13,   13,   48,
   48,   73,  196,  154,  130,   21,  165,    0,  412,    0,
    0,  -22,    0,    0,    0,    0,  465,  466,    0, 1402,
  415, 1598,   95, 1812, 1413,    0,    0,    0,  418,  421,
    0,    0,  392,  392,    0,  373,  373,  373, 1598, 1430,
 1444,  403,    0,    0, 1598,    0, 1598,    0,    0,    0,
   44,    0,    0,    0,    0,    0,    0,  475,    0, 1498,
  425,    0,    0,  206,    0,    0,  359,  373,  368,  373,
  376,   37,    0,    0,    0,    0,    0,    0,  373,  468,
    0,  373,    0,  373,    0, 1116,    0,    0,    0,    0,
    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  761, 1562, 1655, 1800,    0,    0,    0,    0,    0,    0,
    0,    0, 1578,    0,    0,    0,  -13,    0,    0,    0,
  217,    0,  578,  469,    0,    0,    0,    0,    0, 1639,
    0,   47,    0,    0,    0,    0,    0,    0,   18,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  623,    0,    0,    0, 2116,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2143,    0, 2211,    0,    0,  582,
 2308, 2496, 1690,  973,  -19, 1471,  690,    2,  615,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  217,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  378,
    0,    0,  484,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  148,  178,  490,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  380,  393,    0,  394,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  491,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2239, 2248, 2428, 2488, 2525, 2564, 2593, 2621, 2678,
 2685, 1305,  627, 2551, 2584, 2725,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  398,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 2171,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  279,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,   -6,    0,    0,  278,  181,  -73,  849, -100,    0,
  183,  271,   98,  268,  300,  308,  299,  303,  307,    0,
  -89,    0,  -95,   75,    1,    0,    0,  146,  -33,    0,
  474,   43,  -71,    0,    0,  509,    0,  350,  309,  482,
 -106,  -32,  -11,    0,  -27,  -77,    0,    0,  285,  -80,
  -74, -265,    0,  297,  -81,    0, -133,    0,    0,    0,
    0,  411,  523,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           153,
   30,  154,  203,  178,  161,  196,  197,  239,  205,  168,
  111,  254,   47,   67,   68,  106,  184,  104,  220,  107,
  108,   53,  109,  248,   53,   46,  154,  154,   30,   42,
  154,   55,   56,   57,   58,  147,  148,  244,   53,   53,
   81,  243,   59,   53,   59,   59,  242,  144,  318,   67,
   62,  281,  178,  204,   65,  169,  361,  155,  155,   59,
   59,  155,  255,  246,   59,   46,  197,   47,  247,   46,
  194,   47,   51,   53,   53,  322,  111,  154,  153,  258,
  396,  106,  160,  104,   48,  107,  108,   84,  109,  271,
  134,  254,   82,   47,   59,  270,  190,  191,  275,  288,
  152,   47,  359,  110,   53,   53,  111,  230,  155,  231,
   59,  106,  150,  104,  290,  107,  108,  142,  109,  150,
  299,  300,  301,  198,  291,   76,   59,  257,  143,  295,
  361,   67,  112,  131,  334,  147,  146,  264,  194,  244,
  194,  170,  255,   83,  245,  298,  350,  351,  171,  248,
  255,  319,  288,  155,  157,   70,  321,  178,  180,  194,
  194,  395,  289,   40,  324,  191,  152,  191,  197,  110,
   71,  134,  356,   62,  276,  277,  278,  153,   40,  323,
   62,  331,  333,  181,  353,  335,  198,  124,  124,  124,
  339,   41,  345,  340,  247,  182,   59,  296,  105,  110,
  111,  199,  256,  255,  186,  106,   41,  104,  198,  107,
  108,  183,  109,  131,  333,  185,  238,  126,  126,  126,
  187,  257,  357,  200,  201,  202,  112,  188,   45,  197,
  147,  198,  317,  234,  151,   86,   87,   88,  124,   89,
   90,  261,   45,  154,  262,   81,  283,  235,  153,  193,
  360,   53,   53,  236,  160,  289,  368,   73,  366,  280,
   93,  371,    8,  160,  374,  375,  376,  384,  126,  197,
    8,   17,   18,   59,  155,   93,  226,  227,  153,   17,
   18,  383,   45,  260,   91,   92,   45,  153,  160,  385,
   59,  237,  240,  110,  223,  224,  391,  225,  393,  221,
  151,   86,   87,   88,  222,   89,   90,  397,   45,  263,
  399,  218,  400,  228,  229,  266,  218,  193,  218,  193,
  218,  218,  153,  218,  360,  306,  307,  308,  309,  274,
   85,   86,   87,   88,  160,   89,   90,  218,  193,  193,
  232,  233,  377,  379,  381,  284,  346,  279,  198,  198,
   91,   92,  347,  348,  354,  198,  198,  355,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   91,   92,   23,   24,   25,   26,   61,   93,   94,   95,
  282,   96,   97,   98,   99,  100,  101,  102,  103,  390,
   78,  218,  198,  218,  218,  111,  302,  303,  392,  141,
  106,  198,  104,  292,  107,  108,  394,  109,  170,  198,
  167,  170,  285,  167,   85,   86,   87,   88,  156,   89,
   90,  112,  332,  175,  176,  111,  175,  176,  177,  342,
  106,  177,  104,  297,  107,  108,  320,  109,  248,  272,
  336,  273,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   91,   92,   23,   24,   25,   26,
  343,   93,   94,   95,  344,   96,   97,   98,   99,  100,
  101,  102,  103,  247,  349,   59,  304,  305,  110,  310,
  311,  286,  287,  141,  358,  362,  363,  365,  141,  141,
  372,  111,  141,  373,  352,  386,  106,  388,  104,  293,
  107,  108,  389,  109,  163,  152,  398,  141,  110,  141,
  173,  175,  382,  312,  314,  218,  218,  218,  218,  315,
  218,  218,  313,  149,  316,   60,  145,  338,  325,  241,
   44,    0,    0,    0,    0,    0,    0,    0,    0,  141,
  141,    0,    0,  218,  218,  218,  218,  218,  218,  218,
  218,  218,  218,  218,  218,  218,  218,  218,  218,  218,
  218,  218,  218,  218,  218,  218,  218,  218,  218,  218,
  218,  141,  218,  218,  218,    0,  218,  218,  218,  218,
  218,  218,  218,  218,  110,    0,    0,    0,    0,    0,
    0,    0,  138,    0,  218,    0,    0,    0,  138,   37,
    0,  138,   37,    0,   37,   37,   37,    0,    0,   85,
   86,   87,   88,    0,   89,   90,  138,    0,  138,   37,
   37,   37,    0,   37,   37,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   61,    0,  137,   61,  151,
   86,   87,   88,  137,   89,   90,  137,   54,    0,  138,
   54,    0,   61,   61,   37,   37,    0,    0,    0,   91,
   92,  137,    0,  137,   54,   54,   93,   94,   95,   54,
   96,   97,   98,   99,  100,  101,  102,  103,    0,    0,
  138,    0,    0,    0,    0,   37,   37,   61,    0,   91,
   92,    0,    0,    0,  137,    0,    0,    0,   74,   54,
   54,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   57,    0,    0,   57,    0,  151,   86,   87,   88,   61,
   89,   90,    0,    0,   73,  137,    0,   57,   57,    0,
   54,   54,   57,  141,  141,  141,  141,  141,  141,  141,
  141,  141,  141,  141,  141,  141,  141,  141,  141,  141,
  141,  141,  141,  141,  141,   75,    0,  141,  141,  141,
  141,    0,   57,    0,    0,   91,   92,    0,    0,    0,
    0,    0,    0,    0,  141,    0,    0,    0,    0,    0,
   83,   83,   83,    0,   83,    0,   59,    0,    0,    0,
    0,    0,    0,  111,   57,    0,    0,    0,  106,   83,
  104,    0,  107,  108,    0,  109,  206,  207,  208,  209,
  210,  211,  212,  213,  214,  215,  216,  217,    0,  189,
    0,    0,    0,    0,    0,   37,   37,   37,   37,   37,
   37,   83,   37,   37,    0,    0,    0,    0,    0,    0,
    0,    0,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,    0,    0,  138,  138,  138,  138,
    0,    0,    0,    0,    0,    0,    0,   54,   54,    0,
    0,    0,    0,  138,    0,    0,  110,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,    0,
    0,  137,  137,  137,  137,    0,  173,  175,  176,    0,
  111,  177,    0,    0,    0,  106,    0,  104,  137,  167,
  108,    0,  109,    0,  177,  177,    0,    0,    0,    0,
   57,   57,    0,    0,    0,    0,    0,  177,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
  177,    0,   23,   24,   25,   26,  111,    0,    0,    0,
  166,  106,    0,  104,    0,  107,  108,    0,  109,   72,
   51,    0,    0,   51,    0,  177,   51,   83,    0,    0,
    0,    0,  112,    0,    0,    0,    0,    0,    0,    0,
   51,   51,    0,  110,    0,   51,    0,  151,   86,   87,
   88,    0,   89,   90,    0,    0,    0,    0,    0,    0,
    0,    0,  111,    0,    0,    0,    0,  106,    0,  104,
    0,  107,  108,    0,  109,   51,   51,    0,    0,  177,
  177,  177,  177,  177,  177,  177,  177,  177,  177,  177,
  177,  177,  177,  177,  177,  177,  177,   91,   92,  110,
    0,    0,    0,    0,    0,  177,   51,   51,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  111,    0,    0,    0,    0,  106,  177,  104,    0,
  269,  108,    0,  109,    0,    0,    0,    0,    0,    0,
    0,  111,    0,  177,    0,    0,  106,    0,  104,    0,
  107,  108,    0,  109,    0,  110,    0,    0,  111,    0,
    0,    0,    0,  106,    0,  104,    0,  107,  108,    0,
  109,  248,    0,    0,  151,   86,   87,   88,    0,   89,
   90,  268,    0,    0,    0,    0,    0,    0,  177,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  111,    0,    0,    0,  165,  106,    0,  104,    8,
  107,  108,    0,  109,  110,  177,  247,    0,   17,   18,
    0,    0,    0,    0,   91,   92,    0,  112,  177,    0,
  151,   86,   87,   88,  110,   89,   90,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  152,    0,
  401,  110,    0,   51,   51,    0,    0,    0,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   91,   92,   23,   24,   25,   26,  151,   86,   87,   88,
    0,   89,   90,  111,  110,    0,    0,    0,  106,    0,
  104,  111,  107,  108,    0,  109,  106,    0,  104,    0,
  330,  108,    0,  109,    1,    0,    0,    0,    0,    0,
    0,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   91,   92,   23,   24,
   25,   26,    0,    0,    0,  151,   86,   87,   88,    0,
   89,   90,   52,  265,    0,   52,    0,    0,   52,    0,
    0,  329,    0,    0,    0,  151,   86,   87,   88,    0,
   89,   90,   52,   52,    0,    0,  267,   52,    0,    0,
    8,    0,  151,   86,   87,   88,  110,   89,   90,   17,
   18,    0,  111,    0,  110,   91,   92,  106,    0,  104,
    8,  107,  108,    0,  109,    0,    0,   52,   52,   17,
   18,    0,  111,    0,    0,   91,   92,  106,    0,  104,
    0,  107,  108,    0,  109,  151,   86,   87,   88,    0,
   89,   90,   91,   92,    0,    0,    0,    0,   52,   52,
    0,    0,    0,    0,  111,    0,    0,    0,    0,  106,
    0,  104,  341,  107,  108,  111,  109,    0,    0,    0,
  106,    0,  104,    0,  370,  108,    0,  109,    0,    0,
    0,    0,  111,    0,    0,   91,   92,  106,    0,  104,
  378,  107,  108,    0,  109,  110,  111,    0,    0,    0,
    0,  106,    0,  104,  380,  107,  108,    0,  109,    0,
    0,    0,  352,    0,  364,  110,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  369,    0,  151,   86,   87,
   88,   55,   89,   90,   55,  151,   86,   87,   88,    0,
   89,   90,    0,    0,    0,    0,    0,  110,   55,   55,
  111,    0,    0,   55,    0,  106,    0,  104,  110,  107,
  108,  111,  109,    0,    0,    0,  106,    0,  172,    0,
  107,  108,    0,  109,    0,  110,    0,   91,   92,    0,
    0,    0,    0,   55,    0,   91,   92,    0,    0,  110,
    0,    0,  111,    0,    0,   52,   52,  106,    0,  174,
    0,  107,  108,    0,  109,    0,    0,    0,    0,    0,
  387,    0,    0,    0,   55,   55,    0,    0,    0,    0,
    0,   85,   85,   85,    0,   85,  151,   86,   87,   88,
    0,   89,   90,    0,    0,    0,    0,  131,  131,  131,
   85,  131,    0,  110,    0,    0,  151,   86,   87,   88,
  111,   89,   90,    0,  110,  106,  131,  104,    0,  107,
  108,    0,  109,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   85,    0,    0,    0,   91,   92,  151,   86,
   87,   88,    0,   89,   90,  110,    0,    0,  131,  151,
   86,   87,   88,    0,   89,   90,   91,   92,  119,  119,
  119,    0,  119,    0,    0,    0,  151,   86,   87,   88,
    0,   89,   90,    0,   87,   87,   87,  119,   87,    0,
  151,   86,   87,   88,    0,   89,   90,    0,   91,   92,
    0,    0,    0,   87,    0,    0,    0,    0,    0,   91,
   92,    0,    0,  110,    0,    0,    0,   48,    0,  119,
   48,    0,    0,   48,    0,    0,   91,   92,  254,  326,
   47,   55,   55,    0,    0,   87,    0,   48,   48,    0,
   91,   92,   48,    0,  151,   86,   87,   88,    0,   89,
   90,    0,    0,    0,    0,  151,   86,   87,   88,    0,
   89,   90,  288,  326,   47,    0,    0,    0,    0,    0,
    0,    0,   48,   48,    0,    0,    0,    0,    0,  255,
    0,    0,    0,    0,    0,    0,  151,   86,   87,   88,
    0,   89,   90,    0,   91,   92,    0,    0,    0,    0,
    0,    0,    0,   48,   48,   91,   92,    0,   85,    0,
    0,    0,    0,  255,  159,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  131,    0,    0,    0,    0,   89,
   89,   89,    0,   89,    0,    0,   91,   92,    0,    0,
    0,    0,  367,    0,  151,   86,   87,   88,   89,   89,
   90,    0,  131,  131,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,  131,  131,  131,  131,  131,  131,
  131,  131,  131,  131,    0,    0,  131,  131,  131,  131,
   89,    0,    0,    0,    0,  119,    0,    0,   27,    0,
    0,    0,    0,    0,   91,   92,    0,    0,    0,    0,
    0,   87,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  119,  119,  119,  119,  119,  119,  119,
  119,  119,  119,  119,  119,  119,  119,  119,  119,  119,
  119,  119,  119,  119,  119,    0,    0,  119,  119,  119,
  119,    0,    0,    0,    0,   45,    0,   48,   48,    0,
   48,   48,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,    0,    0,    0,    0,    0,    0,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
  158,   23,   24,   25,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   89,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,    1,    2,    3,    4,    5,    6,
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
   31,    0,   31,   31,    0,    0,   38,    0,    0,   38,
    0,   38,   38,   38,    0,   39,    0,    0,   39,    0,
   39,   39,   39,    0,   26,   26,   38,   38,   38,    0,
   38,   38,    0,   31,   31,   39,   39,   39,    0,   39,
   39,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   38,   38,    0,   31,   31,    0,    0,    0,    0,
   39,   39,    0,    0,    0,   40,    0,    0,   40,    0,
    0,   40,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   38,   38,    0,   40,   40,   40,    0,   40,
   40,   39,   39,    0,    0,    0,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    0,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
   40,   40,    0,    0,    0,    0,   19,   19,   19,   19,
   19,   19,    0,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,    0,    0,    0,
    0,   40,   40,    0,   26,   26,   26,   26,   26,   26,
    0,   26,   26,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   41,    0,    0,   41,    0,
    0,   41,    0,    0,   31,   31,   31,   31,   31,   31,
    0,   31,   31,    0,    0,   41,   41,   41,    0,   41,
   41,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   38,   38,   38,   38,   38,   38,    0,   38,
   38,   39,   39,   39,   39,   39,   39,    0,   39,   39,
   41,   41,    0,    0,    0,   42,    0,    0,   42,    0,
    0,   42,    0,   43,    0,    0,   43,    0,    0,   43,
    0,    0,    0,    0,    0,   42,   42,   42,    0,   42,
   42,   41,   41,   43,   43,   43,    0,   43,   43,    0,
    0,    0,   46,    0,    0,   46,    0,    0,   46,    0,
    0,   40,   40,   40,   40,   40,   40,    0,   40,   40,
   42,   42,   46,   46,   46,    0,   46,   46,   43,   43,
    0,   56,    0,    0,   56,    0,    0,    0,    0,    0,
    0,   47,    0,    0,   47,    0,    0,   47,   56,   56,
    0,   42,   42,   56,    0,    0,    0,   46,   46,   43,
   43,   47,   47,   47,   58,   47,   47,   58,    0,    0,
   44,    0,    0,   44,    0,    0,   44,    0,    0,    0,
    0,   58,   58,   56,    0,    0,   58,    0,   46,   46,
   44,   44,   44,    0,   44,   44,   47,   47,   45,    0,
    0,   45,    0,    0,   45,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   56,   56,   58,    0,   45,   45,
   45,    0,   45,   45,    0,   44,   44,   47,   47,    0,
    0,   41,   41,   41,   41,   41,   41,    0,   41,   41,
    0,    0,    0,    0,    0,    0,    0,    0,   58,    0,
    0,    0,    0,   45,   45,   49,   44,   44,   49,    0,
    0,   49,   50,    0,    0,   50,    0,    0,   50,    0,
    0,   59,    0,    0,    0,   49,   49,    0,    0,    0,
   49,    0,   50,   50,   45,   45,    0,   50,    0,    0,
    0,   42,   42,   42,   42,   42,   42,    0,   42,   42,
    0,   43,   43,   43,   43,   60,   43,   43,   60,    0,
   49,   49,    0,    0,    0,    0,    0,   50,   50,    0,
    0,    0,   60,   60,    0,    0,    0,   60,    0,    0,
   46,   46,   46,   46,    0,   46,   46,    0,    0,    0,
    0,   49,   49,    0,    0,    0,    0,    0,   50,   50,
    0,    0,    0,    0,    0,    0,    0,   60,    0,    0,
    0,   56,   56,    0,    0,    0,    0,    0,    0,   47,
   47,   47,   47,    0,   47,   47,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   60,
    0,    0,    0,    0,   58,   58,    0,    0,   44,   44,
   44,   44,    0,   44,   44,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   45,   45,   45,   45,
    0,   45,   45,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    1,    0,   23,   24,   25,
   26,    0,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,    0,    0,   23,
   24,   25,   26,    0,    0,   49,   49,    0,   49,   49,
    0,    0,   50,   50,    0,   50,   50,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,  337,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   60,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,
  };
  protected static readonly short [] yyCheck = {            73,
    0,   73,   40,   93,   82,  106,  107,   63,   46,   83,
   33,   40,   42,   47,   47,   38,   98,   40,  119,   42,
   43,   41,   45,   46,   44,   40,   40,   41,   28,  123,
   44,   31,   32,   33,   34,   69,   69,  144,   58,   59,
   52,  125,   41,   63,  123,   44,  142,   44,  125,   83,
  257,  185,  142,   91,  123,   83,  322,   40,   41,   58,
   59,   44,   91,   44,   63,   40,  167,   42,   91,   40,
  104,   42,   30,   93,   94,   44,   33,   91,  152,  160,
   44,   38,   82,   40,   59,   42,   43,  270,   45,  171,
   44,   40,   40,   42,   93,  169,  103,  104,  180,   40,
  123,   42,  125,  126,  124,  125,   33,   60,   91,   62,
  123,   38,   70,   40,  195,   42,   43,   61,   45,   77,
  221,  222,  223,   44,  198,   51,  125,  160,  125,  203,
  396,  165,   59,   59,   40,  169,   41,  165,  172,  246,
  174,  257,   91,   91,  125,  219,  280,  281,   58,   46,
   91,  247,   40,   79,   80,   44,  125,  247,   58,  193,
  194,  125,  195,  257,   61,  172,  123,  174,  269,  126,
   59,  125,   93,  257,  181,  182,  183,  251,  257,  251,
  257,  255,  257,   40,  285,   91,   44,   40,   41,   42,
  264,  285,  274,  267,   91,   40,  123,  204,  125,  126,
   33,   59,  160,   91,  257,   38,  285,   40,   44,   42,
   43,   40,   45,  139,  289,   40,  272,   40,   41,   42,
   59,  254,   58,  261,  262,  263,   59,   59,  257,  330,
  264,   44,  239,   38,  257,  258,  259,  260,   91,  262,
  263,   41,  257,  257,   44,  257,   59,   94,  322,  104,
  322,  271,  272,  124,  254,  288,  334,   61,  332,  185,
   44,  335,  292,  263,  346,  347,  348,  357,   91,  370,
  292,  301,  302,  272,  257,   59,  264,  265,  352,  301,
  302,  355,  257,   41,  307,  308,  257,  361,  288,  361,
  123,  271,  125,  126,   37,   43,  378,   45,  380,   42,
  257,  258,  259,  260,   47,  262,  263,  389,  257,   44,
  392,   33,  394,  266,  267,   93,   38,  172,   40,  174,
   42,   43,  396,   45,  396,  228,  229,  230,  231,   58,
  257,  258,  259,  260,  334,  262,  263,   59,  193,  194,
  268,  269,  349,  350,  351,   41,   41,  319,   44,   44,
  307,  308,   41,   41,   41,   44,   44,   44,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,   37,  314,  315,  316,
   59,  318,  319,  320,  321,  322,  323,  324,  325,   41,
   51,  123,   44,  125,  126,   33,  224,  225,   41,   60,
   38,   44,   40,  257,   42,   43,   41,   45,   41,   44,
   41,   44,   41,   44,  257,  258,  259,  260,   79,  262,
  263,   59,   61,   41,   41,   33,   44,   44,   41,   93,
   38,   44,   40,  257,   42,   43,  257,   45,   46,  172,
  257,  174,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,  312,
   41,  314,  315,  316,   41,  318,  319,  320,  321,  322,
  323,  324,  325,   91,   40,  123,  226,  227,  126,  232,
  233,  193,  194,   35,   93,   41,   41,   93,   40,   41,
   93,   33,   44,   93,  123,   41,   38,   93,   40,   41,
   42,   43,  317,   45,   41,  123,   59,   59,  126,   61,
   41,   41,  352,  234,  236,  257,  258,  259,  260,  237,
  262,  263,  235,   70,  238,   37,   65,  263,  252,  139,
   28,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   91,
   92,   -1,   -1,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  123,  314,  315,  316,   -1,  318,  319,  320,  321,
  322,  323,  324,  325,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   35,   -1,   61,   -1,   -1,   -1,   41,   38,
   -1,   44,   41,   -1,   43,   44,   45,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   59,   -1,   61,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   35,   44,  257,
  258,  259,  260,   41,  262,  263,   44,   41,   -1,   92,
   44,   -1,   58,   59,   93,   94,   -1,   -1,   -1,  307,
  308,   59,   -1,   61,   58,   59,  314,  315,  316,   63,
  318,  319,  320,  321,  322,  323,  324,  325,   -1,   -1,
  123,   -1,   -1,   -1,   -1,  124,  125,   93,   -1,  307,
  308,   -1,   -1,   -1,   92,   -1,   -1,   -1,   35,   93,
   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   -1,   44,   -1,  257,  258,  259,  260,  125,
  262,  263,   -1,   -1,   61,  123,   -1,   58,   59,   -1,
  124,  125,   63,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,   92,   -1,  309,  310,  311,
  312,   -1,   93,   -1,   -1,  307,  308,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  326,   -1,   -1,   -1,   -1,   -1,
   40,   41,   42,   -1,   44,   -1,  123,   -1,   -1,   -1,
   -1,   -1,   -1,   33,  125,   -1,   -1,   -1,   38,   59,
   40,   -1,   42,   43,   -1,   45,  273,  274,  275,  276,
  277,  278,  279,  280,  281,  282,  283,  284,   -1,   59,
   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,
  269,   91,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,   -1,   -1,  309,  310,  311,  312,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,
   -1,   -1,   -1,  326,   -1,   -1,  126,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   -1,
   -1,  309,  310,  311,  312,   -1,   88,   89,   90,   -1,
   33,   93,   -1,   -1,   -1,   38,   -1,   40,  326,   42,
   43,   -1,   45,   -1,  106,  107,   -1,   -1,   -1,   -1,
  271,  272,   -1,   -1,   -1,   -1,   -1,  119,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  142,   -1,  309,  310,  311,  312,   33,   -1,   -1,   -1,
   93,   38,   -1,   40,   -1,   42,   43,   -1,   45,  326,
   38,   -1,   -1,   41,   -1,  167,   44,  257,   -1,   -1,
   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   -1,  126,   -1,   63,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   93,   94,   -1,   -1,  221,
  222,  223,  224,  225,  226,  227,  228,  229,  230,  231,
  232,  233,  234,  235,  236,  237,  238,  307,  308,  126,
   -1,   -1,   -1,   -1,   -1,  247,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   -1,   -1,   38,  269,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,  285,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,  126,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   46,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   93,   -1,   -1,   -1,   -1,   -1,   -1,  330,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   -1,  288,   38,   -1,   40,  292,
   42,   43,   -1,   45,  126,  357,   91,   -1,  301,  302,
   -1,   -1,   -1,   -1,  307,  308,   -1,   59,  370,   -1,
  257,  258,  259,  260,  126,  262,  263,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,   -1,
  125,  126,   -1,  271,  272,   -1,   -1,   -1,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,  312,  257,  258,  259,  260,
   -1,  262,  263,   33,  126,   -1,   -1,   -1,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,
   42,   43,   -1,   45,  285,   -1,   -1,   -1,   -1,   -1,
   -1,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   38,   93,   -1,   41,   -1,   -1,   44,   -1,
   -1,   93,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   58,   59,   -1,   -1,  288,   63,   -1,   -1,
  292,   -1,  257,  258,  259,  260,  126,  262,  263,  301,
  302,   -1,   33,   -1,  126,  307,  308,   38,   -1,   40,
  292,   42,   43,   -1,   45,   -1,   -1,   93,   94,  301,
  302,   -1,   33,   -1,   -1,  307,  308,   38,   -1,   40,
   -1,   42,   43,   -1,   45,  257,  258,  259,  260,   -1,
  262,  263,  307,  308,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   93,   42,   43,   33,   45,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   33,   -1,   -1,  307,  308,   38,   -1,   40,
   41,   42,   43,   -1,   45,  126,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,   41,   42,   43,   -1,   45,   -1,
   -1,   -1,  123,   -1,   93,  126,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,  257,  258,  259,
  260,   41,  262,  263,   44,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,  126,   58,   59,
   33,   -1,   -1,   63,   -1,   38,   -1,   40,  126,   42,
   43,   33,   45,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,  126,   -1,  307,  308,   -1,
   -1,   -1,   -1,   93,   -1,  307,  308,   -1,   -1,  126,
   -1,   -1,   33,   -1,   -1,  271,  272,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
   -1,   40,   41,   42,   -1,   44,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   40,   41,   42,
   59,   44,   -1,  126,   -1,   -1,  257,  258,  259,  260,
   33,  262,  263,   -1,  126,   38,   59,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   91,   -1,   -1,   -1,  307,  308,  257,  258,
  259,  260,   -1,  262,  263,  126,   -1,   -1,   91,  257,
  258,  259,  260,   -1,  262,  263,  307,  308,   40,   41,
   42,   -1,   44,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   40,   41,   42,   59,   44,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  307,  308,
   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,  307,
  308,   -1,   -1,  126,   -1,   -1,   -1,   38,   -1,   91,
   41,   -1,   -1,   44,   -1,   -1,  307,  308,   40,   41,
   42,  271,  272,   -1,   -1,   91,   -1,   58,   59,   -1,
  307,  308,   63,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   40,   41,   42,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   91,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,  307,  308,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,  307,  308,   -1,  257,   -1,
   -1,   -1,   -1,   91,   41,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,   -1,   40,
   41,   42,   -1,   44,   -1,   -1,  307,  308,   -1,   -1,
   -1,   -1,   41,   -1,  257,  258,  259,  260,   59,  262,
  263,   -1,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,   -1,   -1,  309,  310,  311,  312,
   91,   -1,   -1,   -1,   -1,  257,   -1,   -1,   59,   -1,
   -1,   -1,   -1,   -1,  307,  308,   -1,   -1,   -1,   -1,
   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,   -1,   -1,  309,  310,  311,
  312,   -1,   -1,   -1,   -1,  257,   -1,  268,  269,   -1,
  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,   -1,   -1,  309,  310,  311,
  312,   -1,   -1,   -1,   -1,   -1,   -1,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   -1,
  257,  309,  310,  311,  312,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
   -1,   -1,  309,  310,  311,  312,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,   -1,   -1,
  309,  310,  311,  312,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,   -1,   -1,  309,  310,
  311,  312,   37,   38,   -1,   40,   -1,   42,   43,   44,
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
   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
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
   -1,   58,   59,   93,   -1,   -1,   63,   -1,  124,  125,
   58,   59,   60,   -1,   62,   63,   93,   94,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   93,   -1,   58,   59,
   60,   -1,   62,   63,   -1,   93,   94,  124,  125,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,
   -1,   -1,   -1,   93,   94,   38,  124,  125,   41,   -1,
   -1,   44,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,  123,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,
   63,   -1,   58,   59,  124,  125,   -1,   63,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   -1,  266,  267,  268,  269,   41,  271,  272,   44,   -1,
   93,   94,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,
   -1,   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,
   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  266,  267,  268,  269,
   -1,  271,  272,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  285,   -1,  309,  310,  311,
  312,   -1,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,   -1,   -1,  309,
  310,  311,  312,   -1,   -1,  268,  269,   -1,  271,  272,
   -1,   -1,  268,  269,   -1,  271,  272,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   -1,
   -1,  309,  310,  311,  312,  313,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  272,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   -1,
   -1,  309,  310,  311,  312,
  };

#line 1017 "CParser.jay"

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
  public const int TYPEDEF = 286;
  public const int EXTERN = 287;
  public const int STATIC = 288;
  public const int AUTO = 289;
  public const int REGISTER = 290;
  public const int INLINE = 291;
  public const int RESTRICT = 292;
  public const int CHAR = 293;
  public const int SHORT = 294;
  public const int INT = 295;
  public const int LONG = 296;
  public const int SIGNED = 297;
  public const int UNSIGNED = 298;
  public const int FLOAT = 299;
  public const int DOUBLE = 300;
  public const int CONST = 301;
  public const int VOLATILE = 302;
  public const int VOID = 303;
  public const int BOOL = 304;
  public const int COMPLEX = 305;
  public const int IMAGINARY = 306;
  public const int TRUE = 307;
  public const int FALSE = 308;
  public const int STRUCT = 309;
  public const int CLASS = 310;
  public const int UNION = 311;
  public const int ENUM = 312;
  public const int ELLIPSIS = 313;
  public const int CASE = 314;
  public const int DEFAULT = 315;
  public const int IF = 316;
  public const int ELSE = 317;
  public const int SWITCH = 318;
  public const int WHILE = 319;
  public const int DO = 320;
  public const int FOR = 321;
  public const int GOTO = 322;
  public const int CONTINUE = 323;
  public const int BREAK = 324;
  public const int RETURN = 325;
  public const int EOL = 326;
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
