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
//t    "type_specifier : struct_or_union_specifier",
//t    "type_specifier : enum_specifier",
//t    "type_specifier : TYPE_NAME",
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER compound_statement",
//t    "struct_or_union_specifier : struct_or_union compound_statement",
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER",
//t    "struct_or_union : STRUCT",
//t    "struct_or_union : CLASS",
//t    "struct_or_union : UNION",
//t    "specifier_qualifier_list : type_specifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_specifier",
//t    "specifier_qualifier_list : type_qualifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_qualifier",
//t    "enum_specifier : ENUM '{' enumerator_list '}'",
//t    "enum_specifier : ENUM IDENTIFIER '{' enumerator_list '}'",
//t    "enum_specifier : ENUM '{' enumerator_list ',' '}'",
//t    "enum_specifier : ENUM IDENTIFIER '{' enumerator_list ',' '}'",
//t    "enum_specifier : ENUM IDENTIFIER",
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
#line 350 "CParser.jay"
  {
		yyVal = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-2+yyTop], (List<InitDeclarator>)yyVals[-1+yyTop]);
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
#line 423 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[0+yyTop], null);
	}
  break;
case 94:
#line 427 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[-2+yyTop], (Initializer)yyVals[0+yyTop]);
	}
  break;
case 95:
#line 431 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 96:
#line 432 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 97:
#line 433 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 98:
#line 434 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 99:
#line 435 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 100:
#line 439 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 101:
#line 440 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 102:
#line 441 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 103:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 104:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 105:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 106:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 107:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 108:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 109:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 110:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 111:
#line 450 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 114:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 115:
#line 457 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 116:
#line 458 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 117:
#line 459 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 118:
#line 463 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 119:
#line 464 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 120:
#line 465 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
  break;
case 121:
  case_121();
  break;
case 122:
  case_122();
  break;
case 123:
  case_123();
  break;
case 124:
  case_124();
  break;
case 125:
#line 494 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 126:
#line 495 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 127:
#line 496 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 128:
#line 497 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 129:
#line 498 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 130:
  case_130();
  break;
case 131:
  case_131();
  break;
case 132:
#line 520 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 133:
#line 524 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 134:
#line 528 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 135:
#line 535 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 136:
#line 536 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 137:
#line 541 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 138:
#line 542 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 140:
  case_140();
  break;
case 141:
#line 561 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 142:
#line 565 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 143:
#line 569 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 144:
#line 573 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 145:
#line 577 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 146:
#line 581 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 147:
#line 585 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 148:
#line 589 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 149:
#line 593 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 150:
  case_150();
  break;
case 151:
#line 605 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 152:
#line 609 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 153:
#line 610 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 154:
#line 611 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 155:
#line 612 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 156:
#line 616 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 157:
#line 620 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 158:
#line 624 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 159:
#line 625 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 160:
#line 626 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 161:
#line 633 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 162:
  case_162();
  break;
case 163:
  case_163();
  break;
case 164:
  case_164();
  break;
case 165:
#line 661 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 166:
#line 665 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
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
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 169:
  case_169();
  break;
case 170:
  case_170();
  break;
case 171:
#line 695 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 172:
#line 699 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 173:
#line 706 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[0+yyTop], null);
	}
  break;
case 175:
#line 711 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 176:
  case_176();
  break;
case 177:
#line 730 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 178:
#line 734 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 179:
#line 738 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 180:
#line 742 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 181:
#line 746 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 182:
#line 750 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 183:
#line 754 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 184:
#line 758 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 185:
#line 762 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 186:
#line 766 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 187:
#line 773 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 188:
#line 777 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 189:
#line 781 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 190:
  case_190();
  break;
case 191:
  case_191();
  break;
case 192:
  case_192();
  break;
case 193:
  case_193();
  break;
case 194:
#line 821 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 208:
#line 853 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 209:
#line 857 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 210:
#line 861 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 211:
#line 862 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 214:
#line 874 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 215:
#line 878 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 216:
#line 885 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 217:
#line 889 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 219:
#line 897 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 220:
#line 901 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 221:
#line 905 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 222:
#line 909 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 223:
#line 913 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 224:
#line 917 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 226:
#line 925 "CParser.jay"
  {
        yyVal = new ContinueStatement ();
    }
  break;
case 227:
#line 929 "CParser.jay"
  {
        yyVal = new BreakStatement ();
    }
  break;
case 228:
#line 933 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 229:
#line 937 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 230:
  case_230();
  break;
case 231:
  case_231();
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
  case_239();
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

void case_83()
#line 355 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_84()
#line 361 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_85()
#line 367 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_86()
#line 373 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_87()
#line 379 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 385 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 391 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_90()
#line 397 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_91()
#line 406 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_92()
#line 412 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_121()
#line 470 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_122()
#line 475 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_123()
#line 481 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_124()
#line 486 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_130()
#line 503 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_131()
#line 509 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_140()
#line 547 "CParser.jay"
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

void case_150()
#line 595 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_162()
#line 635 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_163()
#line 644 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_164()
#line 650 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_169()
#line 678 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_170()
#line 684 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_176()
#line 716 "CParser.jay"
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

void case_190()
#line 786 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_191()
#line 793 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_192()
#line 801 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_193()
#line 808 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_230()
#line 942 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_231()
#line 947 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_235()
#line 961 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_236()
#line 970 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_237()
#line 982 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_238()
#line 988 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_239()
#line 994 "CParser.jay"
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
   28,   28,   28,   28,   34,   34,   34,   36,   36,   36,
   38,   38,   38,   38,   35,   35,   35,   35,   35,   39,
   39,   40,   40,   30,   32,   32,   43,   43,   42,   42,
   42,   42,   42,   42,   42,   42,   42,   42,   42,   42,
   42,   41,   41,   41,   41,   44,   44,   29,   29,   29,
   45,   45,   47,   47,   48,   48,   48,   48,   46,   46,
    5,    5,   49,   49,   49,   50,   50,   50,   50,   50,
   50,   50,   50,   50,   50,   50,   33,   33,   33,    6,
    6,    6,    6,   51,   52,   52,   53,   53,   54,   54,
   54,   54,   54,   54,   55,   55,   55,   37,   37,   60,
   60,   61,   61,   56,   56,   57,   57,   57,   58,   58,
   58,   58,   58,   58,   59,   59,   59,   59,   59,    0,
    0,   62,   62,   62,   63,   63,   64,   64,   64,   65,
   65,   65,
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
    1,    1,    1,    1,    3,    2,    2,    1,    1,    1,
    2,    1,    2,    1,    4,    5,    5,    6,    2,    1,
    3,    1,    3,    1,    2,    1,    1,    3,    1,    3,
    5,    4,    4,    6,    6,    5,    4,    3,    4,    4,
    3,    1,    2,    2,    3,    1,    2,    1,    1,    1,
    1,    3,    1,    3,    2,    4,    2,    1,    1,    3,
    1,    2,    1,    1,    2,    3,    2,    3,    3,    4,
    3,    4,    2,    3,    3,    4,    1,    3,    4,    1,
    2,    3,    4,    2,    1,    2,    3,    2,    1,    1,
    1,    1,    1,    1,    3,    4,    3,    2,    3,    1,
    2,    1,    1,    1,    2,    5,    7,    5,    5,    7,
    6,    7,    6,    7,    3,    2,    2,    2,    3,    1,
    2,    1,    1,    1,    4,    3,    1,    2,    2,    1,
    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  114,   95,   96,   97,   98,   99,  134,  159,  101,  102,
  103,  104,  107,  108,  105,  106,  158,  160,  100,  109,
  110,  111,  118,  119,  120,    0,  234,    0,  233,    0,
    0,    0,    0,    0,  112,  113,    0,  230,  232,    0,
    0,  231,  137,    0,    0,   81,    0,   91,    0,    0,
    0,    0,   84,   86,   88,   90,    0,    0,  116,    0,
    0,    0,  130,    0,  156,  154,    0,    0,   82,  240,
    0,  241,  242,  237,    0,  236,    0,    0,    0,    0,
    0,    0,  115,    0,    2,    3,    0,    0,    0,    4,
    5,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  208,    0,    0,   27,   28,   29,   30,
  214,    7,    0,    0,   78,    0,   33,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   63,  212,
  200,  213,  199,  201,  202,  203,  204,    0,  210,    0,
    0,  125,    0,  140,  157,  155,   92,    0,    1,    0,
  187,   94,  239,  235,  238,  169,  151,    0,    0,    0,
    0,  163,    0,  148,    0,    0,    0,  138,    0,    0,
   25,    0,   20,   21,   31,   80,    0,    0,    0,    0,
    0,    0,    0,    0,  226,  227,  228,    0,    0,    0,
    0,    0,    0,   22,   23,    0,  215,    0,   13,   14,
    0,    0,    0,   66,   67,   68,   69,   70,   71,   72,
   73,   74,   75,   76,   77,   65,    0,   24,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  209,  211,  126,
    0,  133,  127,  131,    0,    0,    0,  190,    0,    0,
  195,    0,    0,    0,    0,  167,    0,  149,  150,    0,
    0,    0,  147,  143,    0,  142,    0,    0,  205,    0,
    0,    0,  207,    0,    0,    0,    0,    0,    0,  225,
  229,    6,    0,  121,  123,    0,    0,  172,   79,   12,
    9,    0,   17,    0,   11,   64,   34,   35,   36,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  128,    0,  198,  188,    0,
  191,  194,  196,  183,    0,    0,  177,    0,    0,    0,
    0,    0,    0,  170,  162,  164,    0,    0,  146,  141,
    0,    0,  206,    0,    0,    0,    0,    0,    0,    0,
   32,   10,    0,    8,    0,  197,  189,  192,    0,  184,
  176,  181,  178,  166,  185,    0,  179,    0,    0,  144,
  145,    0,  218,  219,    0,    0,    0,    0,    0,    0,
   18,   62,  193,  186,  182,  180,    0,    0,  223,    0,
  221,    0,   15,    0,  217,  220,  224,  222,   16,
  };
  protected static readonly short [] yyDgoto  = {            28,
  112,  113,  114,  292,  190,  247,  115,  116,  117,  118,
  119,  120,  121,  122,  123,  124,  125,  126,  127,  128,
  129,  217,  177,   29,   75,   47,   31,   32,   33,   34,
   48,   64,  248,   35,   36,   37,  131,  193,   62,   63,
   50,   51,   52,   67,  325,  160,  161,  162,  326,  257,
  249,  250,  251,  132,  133,  134,  135,  136,  137,  138,
  139,   38,   39,   77,   78,
  };
  protected static readonly short [] yySindex = {         1834,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -86,    0, 1834,    0,  -20,
 2696, 2696, 2696, 2696,    0,    0,  -74,    0,    0, -102,
 -226,    0,    0,   19,  -19,    0,   27,    0,  469,  -32,
   13, -228,    0,    0,    0,    0,   85,   50,    0, -226,
   88,   61,    0,  172,    0,    0,  -19,   19,    0,    0,
 1117,    0,    0,    0,  -20,    0, 2618, 2696,   13, 1586,
  979,  -27,    0,  184,    0,    0, 1542, 1596, 1596,    0,
    0, 1607,  198,  221,  226,  244,  484,  257,   45,  252,
  262, 1230,  419,    0, 1607, 1607,    0,    0,    0,    0,
    0,    0,   30,  -22,    0,  113,    0, 1607,   59,  114,
  -44,   65,  203,  290,  269,  210,   96,  -56,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  120,    0,   64,
 1607,    0,  -79,    0,    0,    0,    0,  315,    0, 1061,
    0,    0,    0,    0,    0,    0,    0,   73,  359,  151,
  358,    0,  -38,    0, 1153,  344,  990,    0,  484,  419,
    0,  419,    0,    0,    0,    0,  389,  484, 1607, 1607,
 1607,  139,  919,  401,    0,    0,    0,   40,  174,  415,
  687,  687,   -4,    0,    0, 1607,    0,  206,    0,    0,
 1128, 1607,  268,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1607,    0, 1607, 1607,
 1607, 1607, 1607, 1607, 1607, 1607, 1607, 1607, 1607, 1607,
 1607, 1607, 1607, 1607, 1607, 1607, 1607,    0,    0,    0,
  -58,    0,    0,    0, 1607,  302,   84,    0, 1117,  156,
    0, 1705, 1269,  447,  -31,    0,   16,    0,    0,  305,
 2667,  582,    0,    0, 1607,    0, 1296,  426,    0,  526,
  533,  484,    0,  194,  251,  255,  536, 1304, 1304,    0,
    0,    0, 1359,    0,    0, 1751,   57,    0,    0,    0,
    0,  260,    0,   36,    0,    0,    0,    0,    0,   59,
   59,  114,  114,  -44,  -44,  -44,  -44,   65,   65,  203,
  290,  269,  210,   96,  147,    0,  485,    0,    0,   12,
    0,    0,    0,    0,  540,  544,    0, 1413,  494, 1607,
   16, 1795, 1424,    0,    0,    0,  495,  496,    0,    0,
  454,  454,    0,  484,  484,  484, 1607, 1399, 1467, 1061,
    0,    0, 1607,    0, 1607,    0,    0,    0, 1117,    0,
    0,    0,    0,    0,    0,  549,    0, 1502,  498,    0,
    0,  276,    0,    0,  274,  484,  360,  484,  429,   86,
    0,    0,    0,    0,    0,    0,  484,  535,    0,  484,
    0,  484,    0,  998,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  -29,  128,  492,  694,    0,    0,    0,    0,    0, 1529,
    0,    0,    0,    0,    3,    0,    0,    0,  137,    0,
  567,  525,    0,    0,    0,    0, 1649,    0,    0,    0,
   89,    0,    0,    0,    0,    0,   70,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  609,    0,
    0,    0,    0, 2110,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2137,    0, 2205,    0,    0, 1656, 2302,
 2490, 2672,  512,  479, 2545,    7,  340,  538,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  137,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  477,    0,    0,
  554,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   99,  102,  559,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  487,  500,    0,  513,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  560,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 2233,
 2242, 2422, 2482, 2519, 2558, 2587, 2615, 2679, 2751, 2774,
  505, 2578, 1440,  884,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  514,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 2165,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  191,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -25,    0,    0,   34,  259,  -65,  835, -101,    0,
   26,   56,  240,  309,  381,  385,  380,  388,  396,    0,
  -78,    0, -122,  397,    1,    0,    0,  449,   31,    0,
  565,   49,  -69,    0,    0,    0,  249,  356,  574, -115,
  -42,  -23,    0,  -13,  -80,    0,    0,  374, -118, -214,
 -248,    0,  392,  -87,    0, -157,    0,    0,    0,    0,
  501,  610,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           159,
   30,  152,   66,  194,  195,  151,  237,   44,  252,  182,
   83,   83,   83,  176,   83,  166,  218,  201,  242,   44,
   60,   45,   45,  203,  146,  279,   79,  244,   30,   83,
   61,   53,   54,   55,   56,  286,   41,   45,   46,  256,
  331,   82,  152,  152,  110,  243,  152,   57,   58,  105,
   57,  103,   80,  106,  107,  332,  108,  246,   44,  253,
   45,   83,  176,  195,   57,   57,  316,  167,  202,   57,
   68,  359,  331,  196,  288,   65,  188,  189,   49,  196,
  158,  269,  110,  196,  151,   69,  253,  105,  197,  103,
  273,  106,  107,  152,  108,  221,  286,  145,  281,   57,
  219,  268,  245,   81,  143,  220,  333,  241,  111,  153,
  153,   65,  252,  153,   45,  255,  148,  297,  298,  299,
  348,  349,  317,  148,  228,  244,  229,  320,  354,  394,
  289,   57,  132,  192,  150,  293,  357,  109,  122,  122,
  122,  124,  124,  124,  189,  359,  189,  253,  141,  262,
  287,  296,  110,  274,  275,  276,  222,  105,  223,  103,
  153,  106,  107,  253,  108,  195,  176,   85,   85,   85,
   40,   85,   58,  216,  104,  109,  294,   61,  111,  321,
   93,  351,   57,  151,  343,  142,   85,  329,  240,  122,
  196,  259,  124,   65,  260,   93,  337,  145,   61,  338,
  192,  246,  192,  270,  355,  271,  254,   58,  319,  255,
  393,  315,  144,  132,  282,  236,  322,  196,   85,  224,
  225,  192,  192,  216,   43,   43,  195,   83,  216,  168,
  216,   79,  216,  216,  344,  216,   43,  196,  198,  199,
  200,  169,   58,  287,  238,  109,  245,  300,  301,  216,
  358,  366,  158,    8,  151,  178,  372,  373,  374,  152,
  179,  158,   17,   18,  364,  180,  195,  369,  149,   85,
   86,   87,    8,   88,   89,   43,  382,   57,   57,  302,
  303,   17,   18,  181,  151,   59,  158,  381,  389,  383,
  391,  345,  145,  151,  196,  346,  183,   76,  196,  395,
  352,  184,  397,  353,  398,   83,   84,   85,   86,   87,
  185,   88,   89,  216,  388,  216,  216,  196,   90,   91,
  186,  375,  377,  379,  358,  154,  153,  232,  151,   43,
  226,  227,  158,  234,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   90,   91,   23,   24,
   25,   26,  233,   92,   93,   94,  235,   95,   96,   97,
   98,   99,  100,  101,  102,   71,   84,   85,   86,   87,
   59,   88,   89,   59,   85,  204,  205,  206,  207,  208,
  209,  210,  211,  212,  213,  214,  215,   59,   59,  258,
  390,  261,   59,  196,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   90,   91,   23,   24,
   25,   26,   59,   92,   93,   94,  264,   95,   96,   97,
   98,   99,  100,  101,  102,   74,  272,  216,  216,  216,
  216,  110,  216,  216,  130,  283,  105,  277,  103,  280,
  106,  107,  290,  108,   59,  304,  305,  306,  307,  392,
  230,  231,  196,  153,  155,  216,  216,  216,  216,  216,
  216,  216,  216,  216,  216,  216,  216,  216,  216,  216,
  216,  216,  216,  216,  216,  216,  216,  216,  216,  216,
  216,  216,  216,   72,  216,  216,  216,  330,  216,  216,
  216,  216,  216,  216,  216,  216,  110,  168,  340,   53,
  168,  105,   53,  103,  295,  106,  107,  165,  108,   71,
  165,   87,   87,   87,  130,   87,   53,   53,  308,  309,
  173,   53,  111,  173,  109,   54,  284,  285,   54,   51,
   87,  191,   51,  174,  175,   51,  174,  175,  318,  139,
   73,  334,   54,   54,  139,  139,  341,   54,  139,   51,
   51,   53,   53,  342,   51,  347,  350,  356,   61,  278,
  360,   61,   87,  139,  361,  139,  363,  370,  371,  384,
  386,   58,  387,  396,  161,   61,   61,   54,   54,  171,
  173,  136,   53,   53,   51,   51,   58,  136,  380,  109,
  136,   59,  310,  312,  110,  139,  139,  311,  191,  105,
  191,  103,  313,  106,  107,  136,  108,  136,   54,   54,
   61,  314,  147,  140,  336,   51,   51,   42,  239,  191,
  191,  323,    0,  135,    0,    0,    0,  139,    0,  135,
    0,    0,  135,    0,    0,    0,    0,    0,  136,    0,
    0,    0,   61,    0,    0,    0,    0,  135,    0,  135,
    0,    0,    0,    0,    0,  149,   85,   86,   87,    0,
   88,   89,    0,    0,    0,    0,    0,    0,    0,  136,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  135,    0,    0,    1,    0,    0,    0,  109,    0,    0,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   90,   91,   23,   24,   25,
   26,  135,    0,   89,   89,   89,    0,   89,    0,    0,
   84,   85,   86,   87,    0,   88,   89,    0,   87,   53,
   53,    0,   89,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   54,   54,   23,   24,   25,
   26,    0,   51,   51,   89,    0,    0,    0,    0,    0,
   90,   91,    0,    0,   70,    0,    0,   92,   93,   94,
    0,   95,   96,   97,   98,   99,  100,  101,  102,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,    0,    0,  139,  139,  139,  139,    0,  149,   85,
   86,   87,    0,   88,   89,    0,    0,    0,    0,    0,
  139,  136,  136,  136,  136,  136,  136,  136,  136,  136,
  136,  136,  136,  136,  136,  136,  136,  136,  136,  136,
  136,  136,  136,    8,    0,  136,  136,  136,  136,    0,
    0,    0,   17,   18,    0,    0,    0,    0,   90,   91,
    0,    0,  136,  135,  135,  135,  135,  135,  135,  135,
  135,  135,  135,  135,  135,  135,  135,  135,  135,  135,
  135,  135,  135,  135,  135,    0,    0,  135,  135,  135,
  135,  171,  173,  174,   60,    0,  175,   60,    0,    0,
    0,    0,    0,    0,  135,    0,    0,    0,    0,  175,
  175,   60,   60,    0,    0,    0,   60,    0,    0,    0,
   89,  110,  175,    0,    0,    0,  105,    0,  103,    0,
  106,  107,    0,  108,    0,    0,    0,    0,    0,    0,
    0,    1,    0,    0,    0,  175,   60,  111,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,  175,
    0,    0,    0,    0,    0,    0,    0,    0,   60,    0,
    0,  110,    0,    0,    0,    0,  105,    0,  103,    0,
  165,  107,  110,  108,    0,    0,    0,  105,    0,  103,
  110,  267,  107,    0,  108,  105,    0,  103,    0,  106,
  107,    0,  108,  246,  109,    0,    0,    0,    0,    0,
    0,    0,    0,  175,  175,  175,  175,  175,  175,  175,
  175,  175,  175,  175,  175,  175,  175,  175,  175,  175,
  175,  164,    0,    0,    0,    0,    0,    0,    0,  175,
    0,    0,  266,    0,    0,    0,    0,    0,  245,    0,
    0,    0,    0,  110,    0,    0,    0,    0,  105,    0,
  103,  175,  106,  107,  109,  108,  246,    0,    0,    0,
    0,    0,    0,    0,    0,  109,    0,  175,    0,    0,
  150,    0,  399,  109,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  110,
    0,  245,    0,    0,  105,   60,  103,    0,  106,  107,
  110,  108,  175,    0,    0,  105,    0,  103,  291,  106,
  107,    0,  108,    0,    0,  149,   85,   86,   87,    0,
   88,   89,    0,  150,    0,  110,  109,    0,    0,  175,
  105,    0,  103,    0,  106,  107,    0,  108,    0,    0,
    0,    0,  175,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   90,   91,   23,   24,   25,
   26,    0,    0,    0,    0,  149,   85,   86,   87,  150,
   88,   89,  109,    0,    0,  263,  149,   85,   86,   87,
    0,   88,   89,  109,  149,   85,   86,   87,    0,   88,
   89,    0,  110,    0,    0,    0,  163,  105,    0,  103,
    8,  106,  107,    0,  108,    0,    0,  265,  109,   17,
   18,    8,    0,    0,    0,   90,   91,    0,  187,    0,
   17,   18,    0,    0,    0,    0,   90,   91,    0,    0,
    0,  110,    0,    0,   90,   91,  105,    0,  103,    0,
  328,  107,    0,  108,    0,    0,    0,  149,   85,   86,
   87,    0,   88,   89,    0,    0,    0,    0,  110,    0,
    0,    0,    0,  105,    0,  103,  110,  106,  107,    0,
  108,  105,    0,  103,    0,  106,  107,    0,  108,    0,
    0,    0,    0,    0,    0,  109,    0,    0,    0,    0,
    0,  327,  111,    0,    0,    0,    0,   90,   91,    0,
    0,    0,    0,  149,   85,   86,   87,    0,   88,   89,
    0,    0,    0,    0,  149,   85,   86,   87,  339,   88,
   89,  110,    0,    0,  109,    0,  105,    0,  103,    0,
  106,  107,    0,  108,    0,    0,    0,    0,    0,  149,
   85,   86,   87,    0,   88,   89,    0,    0,    0,    0,
    0,  109,    0,   90,   91,    0,    0,    0,    0,  109,
    0,  110,    0,    0,   90,   91,  105,    0,  103,  376,
  106,  107,    0,  108,    0,  110,    0,    0,    0,    0,
  105,    0,  103,    0,  106,  107,  110,  108,    0,   90,
   91,  105,    0,  103,    0,  368,  107,    0,  108,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   58,  350,    0,   58,  109,    0,  149,   85,   86,   87,
    0,   88,   89,    0,    0,    0,    0,   58,   58,  110,
    0,    0,   58,    0,  105,  362,  103,  378,  106,  107,
    0,  108,    0,    0,    0,    0,  367,    0,    0,    0,
    0,    0,    0,    0,  109,  149,   85,   86,   87,    0,
   88,   89,   58,    0,  110,    0,   90,   91,  109,  105,
    0,  103,    0,  106,  107,    0,  108,    0,    0,  109,
    0,    0,  149,   85,   86,   87,    0,   88,   89,    0,
  149,   85,   86,   87,   58,   88,   89,    0,  129,  129,
  129,    0,  129,    0,  110,   90,   91,    0,    0,  105,
    0,  170,    0,  106,  107,    0,  108,  129,    0,    0,
    0,    0,  109,    0,  385,    0,    0,    0,    0,    0,
    0,    0,   90,   91,    0,    0,    0,    0,    0,    0,
   90,   91,    0,    0,    0,  149,   85,   86,   87,  129,
   88,   89,    0,    0,    0,    0,  157,  109,  110,    0,
    0,    0,    0,  105,    0,  172,    0,  106,  107,  110,
  108,    0,    0,    0,  105,    0,  103,    0,  106,  107,
    0,  108,    0,    0,    0,  149,   85,   86,   87,    0,
   88,   89,    0,    0,    0,   90,   91,  109,    0,  149,
   85,   86,   87,    0,   88,   89,    0,    0,    0,    0,
  149,   85,   86,   87,    0,   88,   89,    0,  117,  117,
  117,    0,  117,   37,    0,    0,   37,    0,   37,   37,
   37,    0,    0,    0,    0,   90,   91,  117,    0,    0,
   58,   58,    0,   37,   37,   37,    0,   37,   37,   90,
   91,  109,    0,  149,   85,   86,   87,    0,   88,   89,
   90,   91,  109,    0,    0,    0,    0,    0,    0,  117,
    0,    0,    0,    0,  252,  324,   45,    0,   37,   37,
    0,    0,    0,    0,    0,    0,    0,    0,  149,   85,
   86,   87,    0,   88,   89,    0,    0,    0,    0,    0,
    0,    0,    0,   90,   91,    0,    0,    0,    0,   37,
   37,    0,    0,    0,    0,  129,    0,    0,    0,    0,
  286,  324,   45,    0,    0,  253,    0,    0,  149,   85,
   86,   87,    0,   88,   89,    0,    0,    0,   90,   91,
    0,    0,    0,  129,  129,  129,  129,  129,  129,  129,
  129,  129,  129,  129,  129,  129,  129,  129,  129,  129,
  129,  129,  129,  129,  129,  365,    0,  129,  129,  129,
  129,  253,  156,    0,    0,    0,    0,    0,   90,   91,
    0,    0,  149,   85,   86,   87,    0,   88,   89,    0,
    0,    0,    0,  149,   85,   86,   87,    0,   88,   89,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   27,    0,   23,   24,   25,   26,    0,    0,
    0,    0,   90,   91,    0,  117,    0,    0,    0,    0,
    0,    0,    0,   90,   91,    0,    0,    0,    0,   37,
   37,   37,   37,   37,   37,    0,   37,   37,    0,    0,
    0,    0,    0,  117,  117,  117,  117,  117,  117,  117,
  117,  117,  117,  117,  117,  117,  117,  117,  117,  117,
  117,  117,  117,  117,  117,    0,    0,  117,  117,  117,
  117,   43,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,    0,    0,   23,
   24,   25,   26,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    1,    1,    0,    1,
    0,    1,    1,    1,    1,    1,    1,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    1,    1,
    1,    1,    1,   19,   19,    0,    0,   19,   19,   19,
   19,   19,    0,   19,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   19,   19,   19,   19,   19,   19,
    1,   26,   26,    1,    0,   26,   26,   26,   26,   26,
    0,   26,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   26,   26,   26,   26,   26,   26,    0,   19,
   19,    0,    0,    1,    0,    0,    0,    0,    0,    0,
    0,   31,   31,    0,    0,   31,   31,   31,   31,   31,
    0,   31,    0,    0,    0,    0,    0,   26,   26,    0,
   19,   19,   31,   31,   31,    0,   31,   31,    0,    0,
   38,    0,    0,   38,    0,   38,   38,   38,    0,   39,
    0,    0,   39,    0,   39,   39,   39,    0,   26,   26,
   38,   38,   38,    0,   38,   38,    0,   31,   31,   39,
   39,   39,    0,   39,   39,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   38,   38,    0,   31,   31,
    0,    0,    0,    0,   39,   39,    0,    0,    0,   40,
    0,    0,   40,    0,    0,   40,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   38,   38,    0,   40,
   40,   40,    0,   40,   40,   39,   39,    0,    0,    0,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    0,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,   40,   40,    0,    0,    0,    0,
   19,   19,   19,   19,   19,   19,    0,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,    0,    0,    0,    0,   40,   40,    0,   26,   26,
   26,   26,   26,   26,    0,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   41,
    0,    0,   41,    0,    0,   41,    0,    0,   31,   31,
   31,   31,   31,   31,    0,   31,   31,    0,    0,   41,
   41,   41,    0,   41,   41,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   38,   38,   38,   38,
   38,   38,    0,   38,   38,   39,   39,   39,   39,   39,
   39,    0,   39,   39,   41,   41,    0,    0,    0,   42,
    0,    0,   42,    0,    0,   42,    0,   43,    0,    0,
   43,    0,    0,   43,    0,    0,    0,    0,    0,   42,
   42,   42,    0,   42,   42,   41,   41,   43,   43,   43,
    0,   43,   43,    0,    0,    0,   46,    0,    0,   46,
    0,    0,   46,    0,    0,   40,   40,   40,   40,   40,
   40,    0,   40,   40,   42,   42,   46,   46,   46,    0,
   46,   46,   43,   43,    0,   55,    0,    0,   55,    0,
    0,    0,    0,    0,    0,   47,    0,    0,   47,    0,
    0,   47,   55,   55,    0,   42,   42,   55,    0,    0,
    0,   46,   46,   43,   43,   47,   47,   47,   56,   47,
   47,   56,    0,    0,   44,    0,    0,   44,    0,    0,
   44,    0,    0,    0,    0,   56,   56,   55,    0,    0,
   56,    0,   46,   46,   44,   44,   44,    0,   44,   44,
   47,   47,   45,    0,    0,   45,    0,    0,   45,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   55,   55,
   56,    0,   45,   45,   45,    0,   45,   45,    0,   44,
   44,   47,   47,    0,    0,   41,   41,   41,   41,   41,
   41,    0,   41,   41,    0,    0,    0,    0,    0,    0,
    0,   56,   56,    0,    0,    0,    0,   45,   45,   48,
   44,   44,   48,    0,    0,   48,   49,    0,    0,   49,
    0,    0,   49,    0,    0,    0,    0,    0,    0,   48,
   48,    0,    0,    0,   48,    0,   49,   49,   45,   45,
   58,   49,    0,    0,    0,   42,   42,   42,   42,   42,
   42,    0,   42,   42,    0,   43,   43,   43,   43,    0,
   43,   43,    0,    0,   48,   48,    0,    0,    0,    0,
    0,   49,   49,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   46,   46,   46,   46,   50,   46,
   46,   50,    0,    0,   50,   48,   48,    0,    0,    0,
    0,    0,   49,   49,    0,    0,    0,    0,   50,   50,
    0,   52,    0,   50,   52,   55,   55,   52,    0,    0,
    0,    0,    0,   47,   47,   47,   47,    0,   47,   47,
    0,   52,   52,    0,    0,    0,   52,    0,    0,    0,
    0,    0,    0,   50,   50,    0,    0,    0,   56,   56,
    0,    0,   44,   44,   44,   44,    0,   44,   44,    0,
    0,    0,    0,    0,    0,    0,   52,   52,    0,    0,
    0,    0,    0,    0,   50,   50,    0,    0,    0,    0,
   45,   45,   45,   45,    0,   45,   45,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   52,   52,    0,
    0,    0,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,    0,    0,   23,   24,   25,   26,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   48,
   48,    0,   48,   48,    0,    0,   49,   49,    0,   49,
   49,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,  335,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   50,   50,
    0,   50,   50,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   52,   52,
  };
  protected static readonly short [] yyCheck = {            80,
    0,   71,   45,  105,  106,   71,   63,   40,   40,   97,
   40,   41,   42,   92,   44,   81,  118,   40,  141,   40,
  123,   42,   42,   46,   67,  183,   50,  143,   28,   59,
  257,   31,   32,   33,   34,   40,  123,   42,   59,  158,
  255,  270,   40,   41,   33,  125,   44,   41,  123,   38,
   44,   40,   40,   42,   43,   40,   45,   46,   40,   91,
   42,   91,  141,  165,   58,   59,  125,   81,   91,   63,
   44,  320,  287,   44,  193,   45,  102,  103,   30,   44,
   80,  169,   33,   44,  150,   59,   91,   38,   59,   40,
  178,   42,   43,   91,   45,   37,   40,   67,   59,   93,
   42,  167,   91,   91,   44,   47,   91,   44,   59,   40,
   41,   81,   40,   44,   42,  158,   68,  219,  220,  221,
  278,  279,  245,   75,   60,  241,   62,   44,   93,   44,
  196,  125,   44,  103,  123,  201,  125,  126,   40,   41,
   42,   40,   41,   42,  170,  394,  172,   91,   61,  163,
  193,  217,   33,  179,  180,  181,   43,   38,   45,   40,
   91,   42,   43,   91,   45,  267,  245,   40,   41,   42,
  257,   44,  123,   61,  125,  126,  202,  257,   59,  249,
   44,  283,  257,  249,  272,  125,   59,  253,  125,   91,
   44,   41,   91,  163,   44,   59,  262,  167,  257,  265,
  170,   46,  172,  170,   58,  172,  158,  123,  125,  252,
  125,  237,   41,  125,   41,  272,   61,   44,   91,  264,
  265,  191,  192,   33,  257,  257,  328,  257,   38,  257,
   40,  255,   42,   43,   41,   45,  257,   44,  261,  262,
  263,   58,  123,  286,  125,  126,   91,  222,  223,   59,
  320,  332,  252,  292,  320,   58,  344,  345,  346,  257,
   40,  261,  301,  302,  330,   40,  368,  333,  257,  258,
  259,  260,  292,  262,  263,  257,  355,  271,  272,  224,
  225,  301,  302,   40,  350,   37,  286,  353,  376,  359,
  378,   41,  262,  359,   44,   41,   40,   49,   44,  387,
   41,  257,  390,   44,  392,   57,  257,  258,  259,  260,
   59,  262,  263,  123,   41,  125,  126,   44,  307,  308,
   59,  347,  348,  349,  394,   77,  257,   38,  394,  257,
  266,  267,  332,  124,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   94,  314,  315,  316,  271,  318,  319,  320,
  321,  322,  323,  324,  325,   61,  257,  258,  259,  260,
   41,  262,  263,   44,  257,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,  283,  284,   58,   59,   41,
   41,   44,   63,   44,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,  312,   93,  314,  315,  316,   93,  318,  319,  320,
  321,  322,  323,  324,  325,   49,   58,  257,  258,  259,
  260,   33,  262,  263,   58,   41,   38,  319,   40,   59,
   42,   43,  257,   45,  125,  226,  227,  228,  229,   41,
  268,  269,   44,   77,   78,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,  312,   35,  314,  315,  316,   61,  318,  319,
  320,  321,  322,  323,  324,  325,   33,   41,   93,   41,
   44,   38,   44,   40,  257,   42,   43,   41,   45,   61,
   44,   40,   41,   42,  138,   44,   58,   59,  230,  231,
   41,   63,   59,   44,  126,   41,  191,  192,   44,   38,
   59,  103,   41,   41,   41,   44,   44,   44,  257,   35,
   92,  257,   58,   59,   40,   41,   41,   63,   44,   58,
   59,   93,   94,   41,   63,   40,  123,   93,   41,  183,
   41,   44,   91,   59,   41,   61,   93,   93,   93,   41,
   93,  123,  317,   59,   41,   58,   59,   93,   94,   41,
   41,   35,  124,  125,   93,   94,  123,   41,  350,  126,
   44,  272,  232,  234,   33,   91,   92,  233,  170,   38,
  172,   40,  235,   42,   43,   59,   45,   61,  124,  125,
   93,  236,   68,   60,  261,  124,  125,   28,  138,  191,
  192,  250,   -1,   35,   -1,   -1,   -1,  123,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   92,   -1,
   -1,   -1,  125,   -1,   -1,   -1,   -1,   59,   -1,   61,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  123,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   92,   -1,   -1,  285,   -1,   -1,   -1,  126,   -1,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,  123,   -1,   40,   41,   42,   -1,   44,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  257,  271,
  272,   -1,   59,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  271,  272,  309,  310,  311,
  312,   -1,  271,  272,   91,   -1,   -1,   -1,   -1,   -1,
  307,  308,   -1,   -1,  326,   -1,   -1,  314,  315,  316,
   -1,  318,  319,  320,  321,  322,  323,  324,  325,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,   -1,   -1,  309,  310,  311,  312,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
  326,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  292,   -1,  309,  310,  311,  312,   -1,
   -1,   -1,  301,  302,   -1,   -1,   -1,   -1,  307,  308,
   -1,   -1,  326,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,   -1,   -1,  309,  310,  311,
  312,   87,   88,   89,   41,   -1,   92,   44,   -1,   -1,
   -1,   -1,   -1,   -1,  326,   -1,   -1,   -1,   -1,  105,
  106,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,
  257,   33,  118,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  285,   -1,   -1,   -1,  141,   93,   59,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,   -1,   -1,  309,  310,  311,  312,  165,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,
   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   33,   45,   -1,   -1,   -1,   38,   -1,   40,
   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   46,  126,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  219,  220,  221,  222,  223,  224,  225,
  226,  227,  228,  229,  230,  231,  232,  233,  234,  235,
  236,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  245,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   91,   -1,
   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,
   40,  267,   42,   43,  126,   45,   46,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  126,   -1,  283,   -1,   -1,
  123,   -1,  125,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   91,   -1,   -1,   38,  272,   40,   -1,   42,   43,
   33,   45,  328,   -1,   -1,   38,   -1,   40,   41,   42,
   43,   -1,   45,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,  123,   -1,   33,  126,   -1,   -1,  355,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,  368,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
  312,   -1,   -1,   -1,   -1,  257,  258,  259,  260,  123,
  262,  263,  126,   -1,   -1,   93,  257,  258,  259,  260,
   -1,  262,  263,  126,  257,  258,  259,  260,   -1,  262,
  263,   -1,   33,   -1,   -1,   -1,  288,   38,   -1,   40,
  292,   42,   43,   -1,   45,   -1,   -1,  288,  126,  301,
  302,  292,   -1,   -1,   -1,  307,  308,   -1,   59,   -1,
  301,  302,   -1,   -1,   -1,   -1,  307,  308,   -1,   -1,
   -1,   33,   -1,   -1,  307,  308,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   33,   42,   43,   -1,
   45,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,
   -1,   93,   59,   -1,   -1,   -1,   -1,  307,  308,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   93,  262,
  263,   33,   -1,   -1,  126,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   -1,  126,   -1,  307,  308,   -1,   -1,   -1,   -1,  126,
   -1,   33,   -1,   -1,  307,  308,   38,   -1,   40,   41,
   42,   43,   -1,   45,   -1,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   33,   45,   -1,  307,
  308,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,  123,   -1,   44,  126,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   58,   59,   33,
   -1,   -1,   63,   -1,   38,   93,   40,   41,   42,   43,
   -1,   45,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,  257,  258,  259,  260,   -1,
  262,  263,   93,   -1,   33,   -1,  307,  308,  126,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,  126,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
  257,  258,  259,  260,  125,  262,  263,   -1,   40,   41,
   42,   -1,   44,   -1,   33,  307,  308,   -1,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   59,   -1,   -1,
   -1,   -1,  126,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  307,  308,   -1,   -1,   -1,   -1,   -1,   -1,
  307,  308,   -1,   -1,   -1,  257,  258,  259,  260,   91,
  262,  263,   -1,   -1,   -1,   -1,   41,  126,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,   33,
   45,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,  307,  308,  126,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   40,   41,
   42,   -1,   44,   38,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   -1,   -1,   -1,  307,  308,   59,   -1,   -1,
  271,  272,   -1,   58,   59,   60,   -1,   62,   63,  307,
  308,  126,   -1,  257,  258,  259,  260,   -1,  262,  263,
  307,  308,  126,   -1,   -1,   -1,   -1,   -1,   -1,   91,
   -1,   -1,   -1,   -1,   40,   41,   42,   -1,   93,   94,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  307,  308,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,   -1,
   40,   41,   42,   -1,   -1,   91,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,  307,  308,
   -1,   -1,   -1,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,   41,   -1,  309,  310,  311,
  312,   91,  257,   -1,   -1,   -1,   -1,   -1,  307,  308,
   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,   59,   -1,  309,  310,  311,  312,   -1,   -1,
   -1,   -1,  307,  308,   -1,  257,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  307,  308,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   -1,   -1,   -1,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,   -1,   -1,  309,  310,  311,
  312,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,   -1,   -1,  309,  310,  311,  312,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,   -1,   -1,  309,
  310,  311,  312,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,   -1,   -1,  309,  310,  311,  312,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
   -1,   -1,  309,  310,  311,  312,   37,   38,   -1,   40,
   -1,   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   63,   37,   38,   -1,   -1,   41,   42,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,
   91,   37,   38,   94,   -1,   41,   42,   43,   44,   45,
   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   58,   59,   60,   61,   62,   63,   -1,   93,
   94,   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   37,   38,   -1,   -1,   41,   42,   43,   44,   45,
   -1,   47,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,
  124,  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   38,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,  124,  125,
   58,   59,   60,   -1,   62,   63,   -1,   93,   94,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,  125,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   58,
   59,   60,   -1,   62,   63,  124,  125,   -1,   -1,   -1,
  261,  262,  263,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  282,  283,  284,   93,   94,   -1,   -1,   -1,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   -1,   -1,   -1,   -1,  124,  125,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,  282,  283,  284,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   93,   94,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   58,
   59,   60,   -1,   62,   63,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   93,   94,   58,   59,   60,   -1,
   62,   63,   93,   94,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   58,   59,   -1,  124,  125,   63,   -1,   -1,
   -1,   93,   94,  124,  125,   58,   59,   60,   41,   62,
   63,   44,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   58,   59,   93,   -1,   -1,
   63,   -1,  124,  125,   58,   59,   60,   -1,   62,   63,
   93,   94,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
   93,   -1,   58,   59,   60,   -1,   62,   63,   -1,   93,
   94,  124,  125,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   93,   94,   38,
  124,  125,   41,   -1,   -1,   44,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,   58,
   59,   -1,   -1,   -1,   63,   -1,   58,   59,  124,  125,
  123,   63,   -1,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,  266,  267,  268,  269,   -1,
  271,  272,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  266,  267,  268,  269,   38,  271,
  272,   41,   -1,   -1,   44,  124,  125,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   58,   59,
   -1,   38,   -1,   63,   41,  271,  272,   44,   -1,   -1,
   -1,   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,
   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,  271,  272,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,   -1,   -1,  309,  310,  311,  312,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  268,
  269,   -1,  271,  272,   -1,   -1,  268,  269,   -1,  271,
  272,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,   -1,   -1,  309,  310,  311,  312,  313,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,   -1,   -1,  309,  310,  311,  312,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  271,  272,
  };

#line 1010 "CParser.jay"

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
