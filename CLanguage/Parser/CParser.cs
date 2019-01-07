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
  public System.IO.TextWriter ErrorOutput = System.Console.Out;

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
//t    "assignment_operator : AND_ASSIGN",
//t    "assignment_operator : XOR_ASSIGN",
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
    "AND_ASSIGN","XOR_ASSIGN","OR_ASSIGN","TYPE_NAME","TYPEDEF","EXTERN",
    "STATIC","AUTO","REGISTER","INLINE","RESTRICT","CHAR","SHORT","INT",
    "LONG","SIGNED","UNSIGNED","FLOAT","DOUBLE","CONST","VOLATILE","VOID",
    "BOOL","COMPLEX","IMAGINARY","TRUE","FALSE","STRUCT","CLASS","UNION",
    "ENUM","ELLIPSIS","CASE","DEFAULT","IF","ELSE","SWITCH","WHILE","DO",
    "FOR","GOTO","CONTINUE","BREAK","RETURN","EOL",
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
#line 43 "CParser.jay"
  { yyVal = new VariableExpression((yyVals[0+yyTop]).ToString()); }
  break;
case 2:
#line 44 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 45 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 46 "CParser.jay"
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 47 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 48 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 55 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 59 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 63 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 67 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 71 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 75 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
	}
  break;
case 13:
#line 79 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 83 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 87 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 91 "CParser.jay"
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
#line 113 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 117 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 121 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 125 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 129 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 133 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 137 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 141 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 145 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 146 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 147 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 148 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 155 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 159 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 166 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 170 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 174 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 178 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 186 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 190 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 198 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 202 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 210 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 214 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 218 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 222 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 230 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 234 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 242 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 250 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 258 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 266 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 274 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 281 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 285 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 303 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 304 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 305 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 306 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 307 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 308 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 309 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 310 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 311 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 312 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 313 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 320 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 77:
#line 324 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 79:
#line 335 "CParser.jay"
  {
		yyVal = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-1+yyTop], null);
	}
  break;
case 80:
#line 339 "CParser.jay"
  {
		yyVal = new MultiDeclaratorStatement ((DeclarationSpecifiers)yyVals[-2+yyTop], (List<InitDeclarator>)yyVals[-1+yyTop]);
	}
  break;
case 81:
  case_81();
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
#line 412 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[0+yyTop], null);
	}
  break;
case 92:
#line 416 "CParser.jay"
  {
		yyVal = new InitDeclarator((Declarator)yyVals[-2+yyTop], (Initializer)yyVals[0+yyTop]);
	}
  break;
case 93:
#line 420 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 94:
#line 421 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 95:
#line 422 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 96:
#line 423 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 97:
#line 424 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 98:
#line 428 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 99:
#line 429 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 100:
#line 430 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 101:
#line 431 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 102:
#line 432 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 103:
#line 433 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 104:
#line 434 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 105:
#line 435 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 106:
#line 436 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 107:
#line 437 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 108:
#line 438 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 109:
#line 439 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 112:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 113:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 114:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 115:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 116:
#line 452 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 117:
#line 453 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 118:
#line 454 "CParser.jay"
  { yyVal = TypeSpecifierKind.Union; }
  break;
case 119:
  case_119();
  break;
case 120:
  case_120();
  break;
case 121:
  case_121();
  break;
case 122:
  case_122();
  break;
case 123:
#line 483 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 124:
#line 484 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 125:
#line 485 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 126:
#line 486 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 127:
#line 487 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 128:
  case_128();
  break;
case 129:
  case_129();
  break;
case 130:
#line 509 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 131:
#line 513 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 132:
#line 517 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 133:
#line 524 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 134:
#line 525 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 135:
#line 530 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 136:
#line 531 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 138:
  case_138();
  break;
case 139:
#line 550 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 140:
#line 554 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 141:
#line 558 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 142:
#line 562 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 143:
#line 566 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 144:
#line 570 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 145:
#line 574 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 146:
#line 578 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 147:
#line 582 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 148:
  case_148();
  break;
case 149:
#line 594 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 150:
#line 598 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 151:
#line 599 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 152:
#line 600 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 153:
#line 601 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 154:
#line 605 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 155:
#line 609 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 156:
#line 613 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 157:
#line 614 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 158:
#line 615 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 159:
#line 622 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 160:
  case_160();
  break;
case 161:
  case_161();
  break;
case 162:
  case_162();
  break;
case 163:
#line 650 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 164:
#line 654 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-3+yyTop], (Declarator)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 165:
#line 658 "CParser.jay"
  {
        yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 166:
#line 662 "CParser.jay"
  {
		yyVal = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
	}
  break;
case 167:
  case_167();
  break;
case 168:
  case_168();
  break;
case 169:
#line 684 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 170:
#line 688 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 174:
  case_174();
  break;
case 175:
#line 713 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 176:
#line 717 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 177:
#line 721 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 178:
#line 725 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 179:
#line 729 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 180:
#line 733 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 181:
#line 737 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: new List<ParameterDeclaration>());
	}
  break;
case 182:
#line 741 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 183:
#line 745 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-2+yyTop], parameters: new List<ParameterDeclaration>());
	}
  break;
case 184:
#line 749 "CParser.jay"
  {
		yyVal = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: (List<ParameterDeclaration>)yyVals[-1+yyTop]);
	}
  break;
case 185:
#line 756 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 186:
#line 760 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 187:
#line 764 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 188:
  case_188();
  break;
case 189:
  case_189();
  break;
case 190:
  case_190();
  break;
case 191:
  case_191();
  break;
case 192:
#line 804 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 206:
#line 836 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 207:
#line 840 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 208:
#line 844 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 209:
#line 845 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 212:
#line 857 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 213:
#line 861 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 214:
#line 868 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 215:
#line 872 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 217:
#line 880 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 218:
#line 884 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 219:
#line 888 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 220:
#line 892 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 221:
#line 896 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 222:
#line 900 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 226:
#line 910 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 227:
#line 914 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 228:
  case_228();
  break;
case 229:
  case_229();
  break;
case 233:
  case_233();
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
#line 96 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 102 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 291 "CParser.jay"
{
		if (yyVals[-1+yyTop] is RelationalOp && ((RelationalOp)yyVals[-1+yyTop]) == RelationalOp.Equals) {
			yyVal = new AssignExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
		}
		else {
			var left = (Expression)yyVals[-2+yyTop]; 
			yyVal = new AssignExpression(left, new BinaryExpression (left, (Binop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]));
		}
	}

void case_81()
#line 344 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_82()
#line 350 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_83()
#line 356 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_84()
#line 362 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_85()
#line 368 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_86()
#line 374 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_87()
#line 380 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 386 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 395 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_90()
#line 401 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_119()
#line 459 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_120()
#line 464 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_121()
#line 470 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_122()
#line 475 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_128()
#line 492 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_129()
#line 498 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_138()
#line 536 "CParser.jay"
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

void case_148()
#line 584 "CParser.jay"
{
		var d = new FunctionDeclarator(innerDeclarator: (Declarator)yyVals[-3+yyTop], parameters: new List<ParameterDeclaration>());
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_160()
#line 624 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_161()
#line 633 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_162()
#line 639 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_167()
#line 667 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_168()
#line 673 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_174()
#line 699 "CParser.jay"
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

void case_188()
#line 769 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_189()
#line 776 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_190()
#line 784 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_191()
#line 791 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_228()
#line 919 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_229()
#line 924 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_233()
#line 938 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_234()
#line 947 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_235()
#line 959 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_236()
#line 965 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_237()
#line 971 "CParser.jay"
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
   22,   22,   22,   22,   22,    2,    2,   23,   24,   24,
   25,   25,   25,   25,   25,   25,   25,   25,   26,   26,
   31,   31,   27,   27,   27,   27,   27,   28,   28,   28,
   28,   28,   28,   28,   28,   28,   28,   28,   28,   28,
   28,   28,   34,   34,   34,   36,   36,   36,   38,   38,
   38,   38,   35,   35,   35,   35,   35,   39,   39,   40,
   40,   30,   32,   32,   43,   43,   42,   42,   42,   42,
   42,   42,   42,   42,   42,   42,   42,   42,   42,   41,
   41,   41,   41,   44,   44,   29,   29,   29,   45,   45,
   47,   47,   48,   48,   48,   48,   46,   46,    5,    5,
   49,   49,   49,   50,   50,   50,   50,   50,   50,   50,
   50,   50,   50,   50,   33,   33,   33,    6,    6,    6,
    6,   51,   52,   52,   53,   53,   54,   54,   54,   54,
   54,   54,   55,   55,   55,   37,   37,   60,   60,   61,
   61,   56,   56,   57,   57,   57,   58,   58,   58,   58,
   58,   58,   59,   59,   59,   59,   59,    0,    0,   62,
   62,   62,   63,   63,   64,   64,   64,   65,   65,   65,
  };
   static readonly short [] yyLen = {           2,
    1,    1,    1,    1,    1,    3,    1,    4,    3,    4,
    3,    3,    2,    2,    6,    7,    1,    3,    1,    2,
    2,    2,    2,    2,    2,    4,    1,    1,    1,    1,
    1,    4,    1,    3,    3,    3,    1,    3,    3,    1,
    3,    3,    1,    3,    3,    3,    3,    1,    3,    3,
    1,    3,    1,    3,    1,    3,    1,    3,    1,    3,
    1,    5,    1,    3,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    3,    1,    2,    3,
    1,    2,    1,    2,    1,    2,    1,    2,    1,    3,
    1,    3,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    3,    2,    2,    1,    1,    1,    2,    1,
    2,    1,    4,    5,    5,    6,    2,    1,    3,    1,
    3,    1,    2,    1,    1,    3,    1,    3,    5,    4,
    4,    6,    6,    5,    4,    3,    4,    4,    3,    1,
    2,    2,    3,    1,    2,    1,    1,    1,    1,    3,
    1,    3,    2,    4,    2,    1,    1,    3,    1,    2,
    1,    1,    2,    3,    2,    3,    3,    4,    3,    4,
    2,    3,    3,    4,    1,    3,    4,    1,    2,    3,
    4,    2,    1,    2,    3,    2,    1,    1,    1,    1,
    1,    1,    3,    4,    3,    2,    3,    1,    2,    1,
    1,    1,    2,    5,    7,    5,    5,    7,    6,    7,
    6,    7,    3,    2,    2,    2,    3,    1,    2,    1,
    1,    1,    4,    3,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  112,   93,   94,   95,   96,   97,  132,  157,   99,  100,
  101,  102,  105,  106,  103,  104,  156,  158,   98,  107,
  108,  109,  116,  117,  118,    0,  232,    0,  231,    0,
    0,    0,    0,    0,  110,  111,    0,  228,  230,    0,
    0,  229,  135,    0,    0,   79,    0,   89,    0,    0,
    0,    0,   82,   84,   86,   88,    0,    0,  114,    0,
    0,    0,  128,    0,  154,  152,    0,    0,   80,  238,
    0,  239,  240,  235,    0,  234,    0,    0,    0,    0,
    0,    0,  113,    0,    2,    3,    0,    0,    0,    4,
    5,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  206,    0,    0,   27,   28,   29,   30,
  212,    7,    0,    0,   76,    0,   33,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   63,  210,
  198,  211,  197,  199,  200,  201,  202,    0,  208,    0,
    0,  123,    0,  138,  155,  153,   90,    0,    1,    0,
  185,   92,  237,  233,  236,  167,  149,    0,    0,    0,
    0,  161,    0,  146,    0,    0,    0,  136,    0,    0,
   25,    0,   20,   21,   31,   78,    0,    0,    0,    0,
    0,    0,    0,    0,  224,  225,  226,    0,    0,    0,
    0,    0,    0,   22,   23,    0,  213,    0,   13,   14,
    0,    0,    0,   66,   67,   68,   69,   70,   71,   72,
   73,   74,   75,   65,    0,   24,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  207,  209,  124,    0,  131,
  125,  129,    0,    0,    0,  188,    0,    0,  193,    0,
    0,    0,    0,  165,    0,  147,  148,    0,    0,    0,
  145,  141,    0,  140,    0,    0,  203,    0,    0,    0,
  205,    0,    0,    0,    0,    0,    0,  223,  227,    6,
    0,  119,  121,    0,    0,  170,   77,   12,    9,    0,
   17,    0,   11,   64,   34,   35,   36,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  126,    0,  196,  186,    0,  189,  192,
  194,  181,    0,    0,  175,    0,    0,    0,    0,    0,
    0,  168,  160,  162,    0,    0,  144,  139,    0,    0,
  204,    0,    0,    0,    0,    0,    0,    0,   32,   10,
    0,    8,    0,  195,  187,  190,    0,  182,  174,  179,
  176,  164,  183,    0,  177,    0,    0,  142,  143,    0,
  216,  217,    0,    0,    0,    0,    0,    0,   18,   62,
  191,  184,  180,  178,    0,    0,  221,    0,  219,    0,
   15,    0,  215,  218,  222,  220,   16,
  };
  protected static readonly short [] yyDgoto  = {            28,
  112,  113,  114,  290,  190,  245,  115,  116,  117,  118,
  119,  120,  121,  122,  123,  124,  125,  126,  127,  128,
  129,  215,  177,   29,   75,   47,   31,   32,   33,   34,
   48,   64,  246,   35,   36,   37,  131,  193,   62,   63,
   50,   51,   52,   67,  323,  160,  161,  162,  324,  255,
  247,  248,  249,  132,  133,  134,  135,  136,  137,  138,
  139,   38,   39,   77,   78,
  };
  protected static readonly short [] yySindex = {         1769,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, -111,    0, 1769,    0,   13,
 2564, 2564, 2564, 2564,    0,    0, -104,    0,    0,  -73,
 -199,    0,    0,  -27,  -31,    0,  144,    0,  467,    4,
    3, -190,    0,    0,    0,    0,  -39,   50,    0, -199,
   35,   -2,    0,   67,    0,    0,  -31,  -27,    0,    0,
 1042,    0,    0,    0,   13,    0, 2598, 2564,    3, 1707,
  973, -143,    0,   63,    0,    0, 1468, 1476, 1476,    0,
    0, 1494,   99,  150,  157,  175,  482,  187,  -17,  183,
  193, 1055,  419,    0, 1494, 1494,    0,    0,    0,    0,
    0,    0,  160,  -15,    0,  109,    0, 1494,  421,  166,
 -138,   39,   12,  224,  170,  169,   40,  -60,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  118,    0,   42,
 1494,    0,  -98,    0,    0,    0,    0,  260,    0,  992,
    0,    0,    0,    0,    0,    0,    0,  -12,  285,  103,
  317,    0,  156,    0, 1283,  239,  984,    0,  482,  419,
    0,  419,    0,    0,    0,    0,  307,  482, 1494, 1494,
 1494,   57,  915,  339,    0,    0,    0,  162,  125,  388,
 1551, 1551,  132,    0,    0, 1494,    0,  186,    0,    0,
 1108, 1494,  209,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1494,    0, 1494, 1494, 1494, 1494,
 1494, 1494, 1494, 1494, 1494, 1494, 1494, 1494, 1494, 1494,
 1494, 1494, 1494, 1494, 1494,    0,    0,    0,  -89,    0,
    0,    0, 1494,  214,   62,    0, 1042,  147,    0, 1631,
 1291,  445,   38,    0,   28,    0,    0,  264, 2626,  580,
    0,    0, 1494,    0, 1299,  424,    0,  492,  496,  482,
    0,  158,  250,  256,  483, 1313, 1313,    0,    0,    0,
 1353,    0,    0, 1659,   29,    0,    0,    0,    0,  258,
    0,   18,    0,    0,    0,    0,    0,  421,  421,  166,
  166, -138, -138, -138, -138,   39,   39,   12,  224,  170,
  169,   40,   94,    0,  447,    0,    0,   14,    0,    0,
    0,    0,  505,  516,    0, 1378,  472, 1494,   28, 1735,
 1389,    0,    0,    0,  473,  477,    0,    0,  437,  437,
    0,  482,  482,  482, 1494, 1403, 1418,  992,    0,    0,
 1494,    0, 1494,    0,    0,    0, 1042,    0,    0,    0,
    0,    0,    0,  530,    0, 1454,  479,    0,    0,  259,
    0,    0,  274,  482,  281,  482,  355,   66,    0,    0,
    0,    0,    0,    0,  482,  520,    0,  482,    0,  482,
    0,  427,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  176,  490,  536,  910,    0,    0,    0,    0,    0, 1523,
    0,    0,    0,    0,  -20,    0,    0,    0,  163,    0,
  565,  523,    0,    0,    0,    0, 1583,    0,    0,    0,
   71,    0,    0,    0,    0,    0,   33,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  607,    0,
    0,    0,    0, 2043,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2070,    0, 2138,    0,    0, 2166, 2355,
 2451,  510,  611, 2574,  921,    7,  -18,  503,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  163,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  356,    0,    0,
  534,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   93,  288,  542,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  401,  403,    0,  475,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  544,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 2175, 2234, 2414,
 2423, 2474, 2511, 2519, 2548, 2607, 2670,  883, 2703, 1103,
 2709, 1101,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  485,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 2098,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  191,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  941,    0,    0,   56,  238,  -65,  831, -101,    0,
  315,  316,  168,  314,  357,  358,  359,  360,  354,    0,
  -78,    0, -102,  -40,    1,    0,    0,    9,   22,    0,
  524,   74,  -69,    0,    0,    0,  247,  364,  537,  -94,
  -45,  -43,    0,  -35,  -72,    0,    0,  335,  -71, -123,
 -221,    0,  350,  -87,    0, -122,    0,    0,    0,    0,
  461,  573,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {            66,
   30,  152,  235,  194,  195,  151,   79,  159,   74,  182,
   45,   41,   44,  176,   45,  166,  216,  130,   58,  150,
  150,  146,   59,  150,  201,   59,  241,  250,   30,   45,
  203,   53,   54,   55,   56,  314,  153,  155,  240,   59,
   59,  143,   80,   44,   59,  167,  110,   57,  242,   60,
   57,  105,   44,  103,   45,  106,  107,   61,  108,  244,
  277,  196,  176,  195,   57,   57,   65,  330,  284,   57,
  150,   46,  151,  151,   59,  202,  151,  250,  251,   82,
  158,  267,  110,   58,  151,  239,  254,  105,  145,  103,
  271,  106,  107,   81,  108,  141,  357,  130,  226,   57,
  227,  266,   65,   49,  243,  318,   59,  144,  111,  392,
  352,  191,  253,  168,  130,  295,  296,  297,  331,  251,
  169,  286,  142,  151,  192,  222,  223,  260,  251,  329,
  287,   57,  120,  120,  120,  291,  150,  196,  355,  109,
  315,  148,  276,  257,  242,   40,  258,  285,  148,  294,
  110,  353,   57,  346,  347,  105,  178,  103,   61,  106,
  107,  329,  108,  195,  176,  280,  238,   61,  196,  214,
  357,  284,   58,   45,  104,  109,  111,  319,  191,  349,
  191,  151,  341,  120,   65,  327,  317,   68,  145,  179,
  391,  192,  244,  192,  335,  130,  180,  336,  342,  191,
  191,  196,   69,  196,  253,  196,   91,  320,  220,   79,
  221,  234,  192,  192,  181,   81,   81,   81,  197,   81,
  279,   91,  251,  214,  195,  268,  183,  269,  214,   43,
  214,  252,  214,  214,   81,  214,  150,  243,  285,  184,
   58,  185,  236,  109,   43,  198,  199,  200,  356,  214,
  158,  186,  151,   59,  370,  371,  372,  364,    8,  158,
   43,  230,  362,  231,  195,  367,   81,   17,   18,   43,
  149,   85,   86,   87,  380,   88,   89,   57,   57,  228,
  229,  145,  151,   59,  158,  379,  387,  381,  389,  151,
  343,  151,  232,  196,   43,   76,  344,  393,  350,  196,
  395,  351,  396,   83,  224,  225,   84,   85,   86,   87,
  233,   88,   89,  214,  386,  214,  214,  196,   90,   91,
   71,  388,  356,  154,  196,  256,  151,  122,  122,  122,
  158,  262,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   90,   91,   23,   24,   25,   26,
  259,   92,   93,   94,  270,   95,   96,   97,   98,   99,
  100,  101,  102,  275,   84,   85,   86,   87,  122,   88,
   89,  204,  205,  206,  207,  208,  209,  210,  211,  212,
  213,  302,  303,  304,  305,  390,  166,  278,  196,  166,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   90,   91,   23,   24,   25,   26,  281,   92,
   93,   94,   81,   95,   96,   97,   98,   99,  100,  101,
  102,  163,  288,  171,  163,    8,  171,  214,  214,  214,
  214,  110,  214,  214,   17,   18,  105,  219,  103,  110,
  106,  107,  217,  108,  105,  293,  103,  218,  106,  107,
  316,  108,  244,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,   72,  214,  214,  214,  328,  214,  214,  214,  214,
  214,  214,  214,  214,  110,  172,  338,  243,  172,  105,
  332,  103,  345,  106,  107,  173,  108,   71,  173,   83,
   83,   83,  339,   83,  298,  299,  340,  300,  301,  354,
  111,  306,  307,   61,  109,  358,   61,   48,   83,  150,
   48,  397,  109,   48,  282,  283,  359,  137,   73,  348,
   61,   61,  137,  137,  361,  368,  137,   48,   48,  369,
  382,  384,   48,  385,  159,   85,   85,   85,  394,   85,
   83,  137,  169,  137,  171,  378,  308,  312,  309,   58,
  310,  147,  311,  334,   85,   61,  140,  321,  237,  134,
   42,    0,   48,   48,   58,  134,    0,  109,  134,    0,
    0,    0,  110,  137,  137,    0,    0,  105,    0,  103,
    0,  106,  107,  134,  108,  134,   85,   61,    0,    0,
    0,    0,    0,   48,   48,    0,    0,    0,    0,    0,
    0,  133,    0,    0,    0,  137,    0,  133,   51,    0,
  133,   51,    0,    0,   51,    0,  134,    0,    0,    0,
    0,    0,    0,    0,    0,  133,    0,  133,   51,   51,
    0,    0,    0,   51,    0,  149,   85,   86,   87,    0,
   88,   89,    0,  149,   85,   86,   87,  134,   88,   89,
    0,    0,    0,    0,    0,    0,    0,    0,  133,    0,
    0,    1,    0,   51,   51,  109,    0,    0,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   90,   91,   23,   24,   25,   26,  133,
    0,   90,   91,    0,   51,   51,    0,    0,   84,   85,
   86,   87,    0,   88,   89,    0,   83,    0,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,   48,   48,    0,
   48,   48,    0,    0,    0,    0,   90,   91,    0,    0,
   70,    0,   85,   92,   93,   94,    0,   95,   96,   97,
   98,   99,  100,  101,  102,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,    0,    0,  137,
  137,  137,  137,    0,    0,    0,  149,   85,   86,   87,
    0,   88,   89,    0,    0,    0,  137,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,  134,    8,
    0,  134,  134,  134,  134,    0,    0,    0,   17,   18,
    0,   51,   51,    0,   90,   91,    0,    0,  134,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,    0,    0,  133,  133,  133,  133,  171,  173,  174,
   52,    0,  175,   52,    0,    0,   52,    0,    0,    0,
  133,    0,    0,    0,    0,  175,  175,    0,    0,    0,
   52,   52,    0,    0,    0,   52,    0,  110,  175,   87,
   87,   87,  105,   87,  103,    0,  106,  107,    0,  108,
    0,   55,    0,    0,   55,    0,    0,    0,   87,    0,
    0,  175,    0,  111,    0,   52,   52,    0,   55,   55,
    0,    0,    0,   55,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  175,    0,    0,    0,    0,
   87,    0,    0,    0,    0,  110,   52,   52,    0,    0,
  105,    0,  103,   55,  165,  107,  110,  108,    0,    0,
    0,  105,    0,  103,  110,  265,  107,    0,  108,  105,
    0,  103,    0,  106,  107,    0,  108,  244,    0,    0,
  109,    0,  188,  189,   55,   55,    0,  175,  175,  175,
  175,  175,  175,  175,  175,  175,  175,  175,  175,  175,
  175,  175,  175,  175,  175,  164,    0,    0,    0,    0,
    0,    0,    0,  175,  110,    0,  264,    0,    0,  105,
    0,  103,  243,  106,  107,    0,  108,  110,    0,    0,
    0,    0,  105,    0,  103,  175,  106,  107,  109,  108,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  109,
  189,  175,  189,  187,  150,    0,    0,  109,    0,  272,
  273,  274,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  110,   60,  292,   56,   60,  105,   56,  103,  289,  106,
  107,    0,  108,   52,   52,    0,  175,    0,   60,   60,
   56,   56,    0,   60,  150,   56,   87,  109,    0,    0,
    0,  149,   85,   86,   87,  313,   88,   89,    0,    0,
  109,    0,    0,  175,    0,    0,    0,    0,    0,    0,
    0,   55,   55,   60,    0,   56,  175,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   90,
   91,   23,   24,   25,   26,   60,   56,   56,    0,  149,
   85,   86,   87,  109,   88,   89,    0,    0,    0,    0,
  149,   85,   86,   87,    0,   88,   89,    0,  149,   85,
   86,   87,    0,   88,   89,    0,    0,    0,  163,    0,
    0,    0,    8,    0,    0,    0,    0,    0,    0,  263,
    0,   17,   18,    8,    0,    0,    0,   90,   91,    0,
    0,    0,   17,   18,    0,  373,  375,  377,   90,   91,
    0,    0,    0,    0,    0,    0,   90,   91,  149,   85,
   86,   87,    0,   88,   89,    0,    0,    0,    0,    0,
    0,  149,   85,   86,   87,  110,   88,   89,    0,    0,
  105,    0,  103,  110,  106,  107,    0,  108,  105,    0,
  103,  110,  326,  107,    0,  108,  105,    0,  103,    0,
  106,  107,    0,  108,    0,  110,   90,   91,    0,    0,
  105,    0,  103,    0,  106,  107,    0,  108,    0,   90,
   91,    0,    0,    0,  149,   85,   86,   87,    0,   88,
   89,  111,   60,   56,   56,  261,    0,    0,    0,    0,
    0,    0,    0,  325,    0,  110,    0,    0,    0,    0,
  105,  337,  103,    0,  106,  107,    0,  108,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  109,    0,
  110,    0,   90,   91,    0,  105,  109,  103,    0,  106,
  107,  110,  108,    0,  109,    0,  105,    0,  103,    0,
  366,  107,    0,  108,    0,  110,    0,    0,  109,    0,
  105,    0,  103,  374,  106,  107,    0,  108,    0,    0,
  110,    0,    0,    0,    0,  105,    0,  103,  376,  106,
  107,    0,  108,    0,    0,    0,    0,    0,    0,    0,
  360,    0,    0,    0,    0,  348,    0,    0,  109,    0,
    0,  365,    0,    0,    0,    0,  110,    0,    0,    0,
    0,  105,    0,  103,    0,  106,  107,    0,  108,    0,
  110,    0,    0,  109,    0,  105,    0,  170,  110,  106,
  107,    0,  108,  105,  109,  172,    0,  106,  107,    0,
  108,    0,    0,    0,    0,    0,  110,    0,  109,    0,
    0,  105,    0,  103,    0,  106,  107,    0,  108,  149,
   85,   86,   87,  109,   88,   89,  383,  149,   85,   86,
   87,    0,   88,   89,    0,  149,   85,   86,   87,    0,
   88,   89,  127,  127,  127,    0,  127,    0,    0,  149,
   85,   86,   87,    0,   88,   89,    0,    0,    0,  109,
    0,  127,    0,    0,    0,    0,    0,   90,   91,    0,
    0,    0,    0,  109,    0,   90,   91,    0,    0,    0,
    0,  109,    0,   90,   91,    0,    0,    0,    0,  149,
   85,   86,   87,  127,   88,   89,    0,   90,   91,  109,
    0,    0,  115,  115,  115,    0,  115,    0,    0,    0,
    0,    0,    0,    0,  149,   85,   86,   87,    0,   88,
   89,  115,    0,    0,    0,  149,   85,   86,   87,    0,
   88,   89,    0,    0,    0,    0,    0,   90,   91,  149,
   85,   86,   87,    0,   88,   89,    0,    0,    0,    0,
  250,  322,   45,  115,  149,   85,   86,   87,    0,   88,
   89,    0,   90,   91,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   90,   91,    0,    0,    0,  284,  322,
   45,    0,    0,    0,    0,    0,    0,   90,   91,    0,
  149,   85,   86,   87,    0,   88,   89,    0,    0,    0,
    0,  251,   90,   91,  149,   85,   86,   87,    0,   88,
   89,    0,  149,   85,   86,   87,    0,   88,   89,    0,
    0,    0,    0,    0,    0,    0,    0,  157,    0,  251,
  149,   85,   86,   87,    0,   88,   89,    0,   90,   91,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   90,   91,    0,  363,    0,    0,    0,  127,
   90,   91,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   90,   91,
    0,    0,    0,    0,    0,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,   27,    0,  127,
  127,  127,  127,    1,    0,    0,    0,    0,    0,  115,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,    0,    0,    0,    0,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,   43,    0,  115,
  115,  115,  115,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,  156,    0,   23,   24,   25,   26,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,    0,    0,    0,    0,    0,
    0,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,    1,
    1,    0,    1,    0,    1,    1,    1,    1,    1,    1,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    1,    1,    1,    1,    1,   19,   19,    0,    0,
   19,   19,   19,   19,   19,    0,   19,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   19,   19,   19,
   19,   19,   19,    1,   26,   26,    1,    0,   26,   26,
   26,   26,   26,    0,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   26,   26,   26,   26,   26,
   26,    0,   19,   19,    0,    0,    1,    0,    0,    0,
    0,    0,    0,    0,   31,   31,    0,    0,   31,   31,
   31,   31,   31,    0,   31,    0,    0,    0,    0,    0,
   26,   26,    0,   19,   19,   31,   31,   31,    0,   31,
   31,    0,    0,   37,    0,    0,   37,    0,   37,   37,
   37,    0,   38,    0,    0,   38,    0,   38,   38,   38,
    0,   26,   26,   37,   37,   37,    0,   37,   37,    0,
   31,   31,   38,   38,   38,    0,   38,   38,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   37,   37,
    0,   31,   31,    0,    0,    0,    0,   38,   38,    0,
    0,   39,    0,    0,   39,    0,   39,   39,   39,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   37,
   37,   39,   39,   39,    0,   39,   39,    0,   38,   38,
    0,    0,    0,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    0,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    0,   39,   39,    0,    0,
    0,    0,    0,   19,   19,   19,   19,   19,   19,    0,
   19,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,   19,    0,    0,    0,    0,    0,   39,   39,    0,
    0,   26,   26,   26,   26,   26,   26,    0,   26,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,   26,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   40,    0,    0,   40,    0,    0,   40,    0,
    0,   31,   31,   31,   31,   31,   31,    0,   31,   31,
    0,    0,   40,   40,   40,    0,   40,   40,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   37,
   37,   37,   37,   37,   37,    0,   37,   37,   38,   38,
   38,   38,   38,   38,    0,   38,   38,   40,   40,    0,
    0,   41,    0,    0,   41,    0,    0,   41,    0,    0,
   42,    0,    0,   42,    0,    0,   42,    0,    0,    0,
    0,   41,   41,   41,    0,   41,   41,    0,   40,   40,
   42,   42,   42,    0,   42,   42,    0,    0,   43,    0,
    0,   43,    0,    0,   43,    0,    0,   39,   39,   39,
   39,   39,   39,    0,   39,   39,   41,   41,   43,   43,
   43,   46,   43,   43,   46,   42,   42,   46,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   46,   46,   46,    0,   46,   46,   41,   41,    0,
    0,    0,    0,   43,   43,    0,   42,   42,   47,    0,
    0,   47,    0,    0,   47,    0,   44,    0,    0,   44,
    0,    0,   44,    0,    0,    0,   46,   46,   47,   47,
   47,    0,   47,   47,   43,   43,   44,   44,   44,    0,
   44,   44,    0,    0,    0,   45,    0,    0,   45,    0,
    0,   45,    0,    0,    0,    0,    0,   46,   46,    0,
    0,    0,    0,   47,   47,   45,   45,   45,    0,   45,
   45,   44,   44,    0,   53,    0,    0,   53,   40,   40,
   40,   40,   40,   40,    0,   40,   40,    0,    0,    0,
    0,   53,   53,    0,   47,   47,   53,    0,    0,    0,
   45,   45,   44,   44,   49,    0,    0,   49,    0,    0,
   49,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   49,   49,   53,   53,    0,   49,
    0,   45,   45,    0,    0,    0,    0,   41,   41,   41,
   41,   41,   41,    0,   41,   41,   42,   42,   42,   42,
   42,   42,    0,   42,   42,    0,    0,   53,   53,   49,
   49,    0,    0,    0,    0,    0,    0,   50,    0,    0,
   50,    0,    0,   50,    0,    0,   43,   43,   43,   43,
   58,   43,   43,    0,    0,    0,    0,   50,   50,    0,
   49,   49,   50,    0,    0,    0,    0,    0,    0,   46,
   46,   46,   46,   54,   46,   46,   54,    0,    0,   58,
    0,    0,   58,    0,    0,    0,    0,    0,    0,    0,
   54,   54,   50,   50,    0,   54,   58,   58,    0,    0,
    0,   58,    0,    0,    0,    0,   47,   47,   47,   47,
    0,   47,   47,    0,   44,   44,   44,   44,    0,   44,
   44,    0,    0,   50,   50,   54,   54,    0,    0,    0,
    0,   58,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   45,   45,   45,   45,    0,   45,   45,
    0,    0,    0,    0,    0,    0,   54,   54,    0,    0,
    0,    0,    0,   58,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   53,   53,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,   49,   49,    0,   49,   49,    0,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,  333,   50,   50,    0,
   50,   50,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   54,   54,    0,    0,    0,    0,   58,
   58,
  };
  protected static readonly short [] yyCheck = {            45,
    0,   71,   63,  105,  106,   71,   50,   80,   49,   97,
   42,  123,   40,   92,   42,   81,  118,   58,  123,   40,
   41,   67,   41,   44,   40,   44,  125,   40,   28,   42,
   46,   31,   32,   33,   34,  125,   77,   78,  141,   58,
   59,   44,   40,   40,   63,   81,   33,   41,  143,  123,
   44,   38,   40,   40,   42,   42,   43,  257,   45,   46,
  183,   44,  141,  165,   58,   59,   45,   40,   40,   63,
   91,   59,   40,   41,   93,   91,   44,   40,   91,  270,
   80,  169,   33,  123,  150,   44,  158,   38,   67,   40,
  178,   42,   43,   91,   45,   61,  318,  138,   60,   93,
   62,  167,   81,   30,   91,   44,  125,   41,   59,   44,
   93,  103,  158,  257,   44,  217,  218,  219,   91,   91,
   58,  193,  125,   91,  103,  264,  265,  163,   91,  253,
  196,  125,   40,   41,   42,  201,  123,   44,  125,  126,
  243,   68,  183,   41,  239,  257,   44,  193,   75,  215,
   33,   58,  257,  276,  277,   38,   58,   40,  257,   42,
   43,  285,   45,  265,  243,   41,  125,  257,   44,   61,
  392,   40,  123,   42,  125,  126,   59,  247,  170,  281,
  172,  247,  270,   91,  163,  251,  125,   44,  167,   40,
  125,  170,   46,  172,  260,  125,   40,  263,   41,  191,
  192,   44,   59,   44,  250,   44,   44,   61,   43,  253,
   45,  272,  191,  192,   40,   40,   41,   42,   59,   44,
   59,   59,   91,   33,  326,  170,   40,  172,   38,  257,
   40,  158,   42,   43,   59,   45,  257,   91,  284,  257,
  123,   59,  125,  126,  257,  261,  262,  263,  318,   59,
  250,   59,  318,  272,  342,  343,  344,  330,  290,  259,
  257,   38,  328,   94,  366,  331,   91,  299,  300,  257,
  257,  258,  259,  260,  353,  262,  263,  271,  272,  268,
  269,  260,  348,   37,  284,  351,  374,  357,  376,  257,
   41,  357,  124,   44,  257,   49,   41,  385,   41,   44,
  388,   44,  390,   57,  266,  267,  257,  258,  259,  260,
  271,  262,  263,  123,   41,  125,  126,   44,  305,  306,
   61,   41,  392,   77,   44,   41,  392,   40,   41,   42,
  330,   93,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   44,  312,  313,  314,   58,  316,  317,  318,  319,  320,
  321,  322,  323,  317,  257,  258,  259,  260,   91,  262,
  263,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,  224,  225,  226,  227,   41,   41,   59,   44,   44,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,   41,  312,
  313,  314,  257,  316,  317,  318,  319,  320,  321,  322,
  323,   41,  257,   41,   44,  290,   44,  257,  258,  259,
  260,   33,  262,  263,  299,  300,   38,   37,   40,   33,
   42,   43,   42,   45,   38,  257,   40,   47,   42,   43,
  257,   45,   46,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,   35,  312,  313,  314,   61,  316,  317,  318,  319,
  320,  321,  322,  323,   33,   41,   93,   91,   44,   38,
  257,   40,   40,   42,   43,   41,   45,   61,   44,   40,
   41,   42,   41,   44,  220,  221,   41,  222,  223,   93,
   59,  228,  229,   41,  126,   41,   44,   38,   59,  123,
   41,  125,  126,   44,  191,  192,   41,   35,   92,  123,
   58,   59,   40,   41,   93,   93,   44,   58,   59,   93,
   41,   93,   63,  315,   41,   40,   41,   42,   59,   44,
   91,   59,   41,   61,   41,  348,  230,  234,  231,  123,
  232,   68,  233,  259,   59,   93,   60,  248,  138,   35,
   28,   -1,   93,   94,  123,   41,   -1,  126,   44,   -1,
   -1,   -1,   33,   91,   92,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   59,   45,   61,   91,  125,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   35,   -1,   -1,   -1,  123,   -1,   41,   38,   -1,
   44,   41,   -1,   -1,   44,   -1,   92,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   -1,   61,   58,   59,
   -1,   -1,   -1,   63,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,  257,  258,  259,  260,  123,  262,  263,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   92,   -1,
   -1,  283,   -1,   93,   94,  126,   -1,   -1,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  123,
   -1,  305,  306,   -1,  124,  125,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,  257,   -1,   -1,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,  268,  269,   -1,
  271,  272,   -1,   -1,   -1,   -1,  305,  306,   -1,   -1,
  324,   -1,  257,  312,  313,  314,   -1,  316,  317,  318,
  319,  320,  321,  322,  323,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,   -1,   -1,  307,
  308,  309,  310,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,  324,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  290,
   -1,  307,  308,  309,  310,   -1,   -1,   -1,  299,  300,
   -1,  271,  272,   -1,  305,  306,   -1,   -1,  324,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,   87,   88,   89,
   38,   -1,   92,   41,   -1,   -1,   44,   -1,   -1,   -1,
  324,   -1,   -1,   -1,   -1,  105,  106,   -1,   -1,   -1,
   58,   59,   -1,   -1,   -1,   63,   -1,   33,  118,   40,
   41,   42,   38,   44,   40,   -1,   42,   43,   -1,   45,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   59,   -1,
   -1,  141,   -1,   59,   -1,   93,   94,   -1,   58,   59,
   -1,   -1,   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  165,   -1,   -1,   -1,   -1,
   91,   -1,   -1,   -1,   -1,   33,  124,  125,   -1,   -1,
   38,   -1,   40,   93,   42,   43,   33,   45,   -1,   -1,
   -1,   38,   -1,   40,   33,   42,   43,   -1,   45,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   46,   -1,   -1,
  126,   -1,  102,  103,  124,  125,   -1,  217,  218,  219,
  220,  221,  222,  223,  224,  225,  226,  227,  228,  229,
  230,  231,  232,  233,  234,   93,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  243,   33,   -1,   93,   -1,   -1,   38,
   -1,   40,   91,   42,   43,   -1,   45,   33,   -1,   -1,
   -1,   -1,   38,   -1,   40,  265,   42,   43,  126,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,
  170,  281,  172,   59,  123,   -1,   -1,  126,   -1,  179,
  180,  181,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   41,  202,   41,   44,   38,   44,   40,   41,   42,
   43,   -1,   45,  271,  272,   -1,  326,   -1,   58,   59,
   58,   59,   -1,   63,  123,   63,  257,  126,   -1,   -1,
   -1,  257,  258,  259,  260,  235,  262,  263,   -1,   -1,
  126,   -1,   -1,  353,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  271,  272,   93,   -1,   93,  366,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  125,  124,  125,   -1,  257,
  258,  259,  260,  126,  262,  263,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,  286,   -1,
   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,  286,
   -1,  299,  300,  290,   -1,   -1,   -1,  305,  306,   -1,
   -1,   -1,  299,  300,   -1,  345,  346,  347,  305,  306,
   -1,   -1,   -1,   -1,   -1,   -1,  305,  306,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   33,  262,  263,   -1,   -1,
   38,   -1,   40,   33,   42,   43,   -1,   45,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   33,  305,  306,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,  305,
  306,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   59,  272,  271,  272,   93,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   33,   -1,   -1,   -1,   -1,
   38,   93,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,
   33,   -1,  305,  306,   -1,   38,  126,   40,   -1,   42,
   43,   33,   45,   -1,  126,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   33,   -1,   -1,  126,   -1,
   38,   -1,   40,   41,   42,   43,   -1,   45,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   41,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,  123,   -1,   -1,  126,   -1,
   -1,   93,   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   33,   -1,   -1,  126,   -1,   38,   -1,   40,   33,   42,
   43,   -1,   45,   38,  126,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,   33,   -1,  126,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,  257,
  258,  259,  260,  126,  262,  263,   93,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,   -1,
  262,  263,   40,   41,   42,   -1,   44,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,  126,
   -1,   59,   -1,   -1,   -1,   -1,   -1,  305,  306,   -1,
   -1,   -1,   -1,  126,   -1,  305,  306,   -1,   -1,   -1,
   -1,  126,   -1,  305,  306,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   91,  262,  263,   -1,  305,  306,  126,
   -1,   -1,   40,   41,   42,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   59,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,  305,  306,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   40,   41,   42,   91,  257,  258,  259,  260,   -1,  262,
  263,   -1,  305,  306,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  305,  306,   -1,   -1,   -1,   40,   41,
   42,   -1,   -1,   -1,   -1,   -1,   -1,  305,  306,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,   91,  305,  306,  257,  258,  259,  260,   -1,  262,
  263,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   91,
  257,  258,  259,  260,   -1,  262,  263,   -1,  305,  306,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  305,  306,   -1,   41,   -1,   -1,   -1,  257,
  305,  306,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  305,  306,
   -1,   -1,   -1,   -1,   -1,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,   59,   -1,  307,
  308,  309,  310,  283,   -1,   -1,   -1,   -1,   -1,  257,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,   -1,   -1,  307,  308,  309,
  310,   -1,   -1,   -1,   -1,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  257,   -1,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,   -1,   -1,  307,  308,  309,
  310,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  257,   -1,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   -1,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,   -1,   -1,  307,  308,  309,  310,   37,
   38,   -1,   40,   -1,   42,   43,   44,   45,   46,   47,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   63,   37,   38,   -1,   -1,
   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,
   61,   62,   63,   91,   37,   38,   94,   -1,   41,   42,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   61,   62,
   63,   -1,   93,   94,   -1,   -1,  124,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   37,   38,   -1,   -1,   41,   42,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
   93,   94,   -1,  124,  125,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   38,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   38,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,  124,  125,   58,   59,   60,   -1,   62,   63,   -1,
   93,   94,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,
   -1,  124,  125,   -1,   -1,   -1,   -1,   93,   94,   -1,
   -1,   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   58,   59,   60,   -1,   62,   63,   -1,  124,  125,
   -1,   -1,   -1,  261,  262,  263,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,   -1,   93,   94,   -1,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  282,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
  273,  274,  275,  276,  277,  278,  279,  280,  281,  282,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,   93,   94,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,  124,  125,
   58,   59,   60,   -1,   62,   63,   -1,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   93,   94,   58,   59,
   60,   38,   62,   63,   41,   93,   94,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   -1,   62,   63,  124,  125,   -1,
   -1,   -1,   -1,   93,   94,   -1,  124,  125,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   93,   94,   58,   59,
   60,   -1,   62,   63,  124,  125,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   93,   94,   58,   59,   60,   -1,   62,
   63,   93,   94,   -1,   41,   -1,   -1,   44,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,
   -1,   58,   59,   -1,  124,  125,   63,   -1,   -1,   -1,
   93,   94,  124,  125,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   93,   94,   -1,   63,
   -1,  124,  125,   -1,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,  124,  125,   93,
   94,   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,  266,  267,  268,  269,
  123,  271,  272,   -1,   -1,   -1,   -1,   58,   59,   -1,
  124,  125,   63,   -1,   -1,   -1,   -1,   -1,   -1,  266,
  267,  268,  269,   41,  271,  272,   44,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   93,   94,   -1,   63,   58,   59,   -1,   -1,
   -1,   63,   -1,   -1,   -1,   -1,  266,  267,  268,  269,
   -1,  271,  272,   -1,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,  124,  125,   93,   94,   -1,   -1,   -1,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  271,  272,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,   -1,   -1,
  307,  308,  309,  310,  268,  269,   -1,  271,  272,   -1,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,   -1,   -1,  307,  308,  309,  310,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
   -1,   -1,  307,  308,  309,  310,  311,  268,  269,   -1,
  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,  271,
  272,
  };

#line 987 "CParser.jay"

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
			 Console.Error.WriteLine (s);
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
  public const int AND_ASSIGN = 280;
  public const int XOR_ASSIGN = 281;
  public const int OR_ASSIGN = 282;
  public const int TYPE_NAME = 283;
  public const int TYPEDEF = 284;
  public const int EXTERN = 285;
  public const int STATIC = 286;
  public const int AUTO = 287;
  public const int REGISTER = 288;
  public const int INLINE = 289;
  public const int RESTRICT = 290;
  public const int CHAR = 291;
  public const int SHORT = 292;
  public const int INT = 293;
  public const int LONG = 294;
  public const int SIGNED = 295;
  public const int UNSIGNED = 296;
  public const int FLOAT = 297;
  public const int DOUBLE = 298;
  public const int CONST = 299;
  public const int VOLATILE = 300;
  public const int VOID = 301;
  public const int BOOL = 302;
  public const int COMPLEX = 303;
  public const int IMAGINARY = 304;
  public const int TRUE = 305;
  public const int FALSE = 306;
  public const int STRUCT = 307;
  public const int CLASS = 308;
  public const int UNION = 309;
  public const int ENUM = 310;
  public const int ELLIPSIS = 311;
  public const int CASE = 312;
  public const int DEFAULT = 313;
  public const int IF = 314;
  public const int ELSE = 315;
  public const int SWITCH = 316;
  public const int WHILE = 317;
  public const int DO = 318;
  public const int FOR = 319;
  public const int GOTO = 320;
  public const int CONTINUE = 321;
  public const int BREAK = 322;
  public const int RETURN = 323;
  public const int EOL = 324;
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
