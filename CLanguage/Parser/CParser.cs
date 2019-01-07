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

  protected const int yyFinal = 27;
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
case 232:
  case_232();
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

void case_232()
#line 937 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-3+yyTop],
			(Declarator)yyVals[-2+yyTop],
			(List<Declaration>)yyVals[-1+yyTop],
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_233()
#line 946 "CParser.jay"
{
		var f = new FunctionDefinition(
			(DeclarationSpecifiers)yyVals[-2+yyTop],
			(Declarator)yyVals[-1+yyTop],
			null,
			(Block)yyVals[0+yyTop]);
		yyVal = f;
	}

void case_234()
#line 958 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_235()
#line 964 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_236()
#line 970 "CParser.jay"
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
   62,   63,   63,   64,   64,   64,   65,   65,   65,
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
    1,    4,    3,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  112,   93,   94,   95,   96,   97,  132,  157,   99,  100,
  101,  102,  105,  106,  103,  104,  156,  158,   98,  107,
  108,  109,  116,  117,  118,    0,    0,  231,    0,    0,
    0,    0,    0,  110,  111,    0,  228,  230,    0,    0,
  229,  135,    0,    0,   79,    0,   89,    0,    0,    0,
    0,   82,   84,   86,   88,    0,    0,  114,    0,    0,
    0,  128,    0,  154,  152,    0,    0,   80,  237,    0,
  238,  239,  234,    0,  233,    0,    0,    0,    0,    0,
    0,  113,    0,    2,    3,    0,    0,    0,    4,    5,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  206,    0,    0,   27,   28,   29,   30,  212,
    7,    0,    0,   76,    0,   33,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   63,  210,  198,
  211,  197,  199,  200,  201,  202,    0,  208,    0,    0,
  123,    0,  138,  155,  153,   90,    0,    1,    0,  185,
   92,  236,  232,  235,  167,  149,    0,    0,    0,    0,
  161,    0,  146,    0,    0,    0,  136,    0,    0,   25,
    0,   20,   21,   31,   78,    0,    0,    0,    0,    0,
    0,    0,    0,  224,  225,  226,    0,    0,    0,    0,
    0,    0,   22,   23,    0,  213,    0,   13,   14,    0,
    0,    0,   66,   67,   68,   69,   70,   71,   72,   73,
   74,   75,   65,    0,   24,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  207,  209,  124,    0,  131,  125,
  129,    0,    0,    0,  188,    0,    0,  193,    0,    0,
    0,    0,  165,    0,  147,  148,    0,    0,    0,  145,
  141,    0,  140,    0,    0,  203,    0,    0,    0,  205,
    0,    0,    0,    0,    0,    0,  223,  227,    6,    0,
  119,  121,    0,    0,  170,   77,   12,    9,    0,   17,
    0,   11,   64,   34,   35,   36,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  126,    0,  196,  186,    0,  189,  192,  194,
  181,    0,    0,  175,    0,    0,    0,    0,    0,    0,
  168,  160,  162,    0,    0,  144,  139,    0,    0,  204,
    0,    0,    0,    0,    0,    0,    0,   32,   10,    0,
    8,    0,  195,  187,  190,    0,  182,  174,  179,  176,
  164,  183,    0,  177,    0,    0,  142,  143,    0,  216,
  217,    0,    0,    0,    0,    0,    0,   18,   62,  191,
  184,  180,  178,    0,    0,  221,    0,  219,    0,   15,
    0,  215,  218,  222,  220,   16,
  };
  protected static readonly short [] yyDgoto  = {            27,
  111,  112,  113,  289,  189,  244,  114,  115,  116,  117,
  118,  119,  120,  121,  122,  123,  124,  125,  126,  127,
  128,  214,  176,   28,   74,   46,   30,   31,   32,   33,
   47,   63,  245,   34,   35,   36,  130,  192,   61,   62,
   49,   50,   51,   66,  322,  159,  160,  161,  323,  254,
  246,  247,  248,  131,  132,  133,  134,  135,  136,  137,
  138,   37,   38,   76,   77,
  };
  protected static readonly short [] yySindex = {         2595,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, -106, 2595,    0,   63, 2595,
 2595, 2595, 2595,    0,    0,  -94,    0,    0,  -11, -151,
    0,    0,  -17,  -21,    0,   41,    0,  633,  -24,   54,
 -154,    0,    0,    0,    0,   20,   48,    0, -151,   89,
   -8,    0,  149,    0,    0,  -21,  -17,    0,    0,  501,
    0,    0,    0,   63,    0, 2637, 2595,   54, 1745,  999,
  -41,    0,  151,    0,    0, 1485, 1572, 1572,    0,    0,
 1610,  188,  242,  247,  262,  258,  264,   52,  257,  300,
 1148,  869,    0, 1610, 1610,    0,    0,    0,    0,    0,
    0,   70,  -37,    0,  458,    0, 1610,  161,  172,   64,
  -48,  185,  325,  278,  272,  160,  -52,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  116,    0,   16, 1610,
    0, -110,    0,    0,    0,    0,  366,    0, 1018,    0,
    0,    0,    0,    0,    0,    0,    7,  401,  221,  406,
    0,   87,    0, 1250,  352, 1010,    0,  258,  869,    0,
  869,    0,    0,    0,    0,  403,  258, 1610, 1610, 1610,
  146,  941,  407,    0,    0,    0,  140,  222,  428,  408,
  408,  106,    0,    0, 1610,    0,  243,    0,    0, 1175,
 1610,  267,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 1610,    0, 1610, 1610, 1610, 1610, 1610,
 1610, 1610, 1610, 1610, 1610, 1610, 1610, 1610, 1610, 1610,
 1610, 1610, 1610, 1610,    0,    0,    0,  -95,    0,    0,
    0, 1610,  268,   17,    0,  501,   36,    0, 1554, 1287,
  443,   10,    0,   91,    0,    0,  270, 2677,  564,    0,
    0, 1610,    0, 1302,  421,    0,  487,  489,  258,    0,
  248,  281,  283,  493, 1313, 1313,    0,    0,    0, 1321,
    0,    0, 1689,  100,    0,    0,    0,    0,  339,    0,
   79,    0,    0,    0,    0,    0,  161,  161,  172,  172,
   64,   64,   64,   64,  -48,  -48,  185,  325,  278,  272,
  160,  142,    0,  445,    0,    0,   13,    0,    0,    0,
    0,  495,  508,    0, 1376,  449, 1610,   91, 1596, 1403,
    0,    0,    0,  460,  463,    0,    0,  434,  434,    0,
  258,  258,  258, 1610, 1341, 1417, 1018,    0,    0, 1610,
    0, 1610,    0,    0,    0,  501,    0,    0,    0,    0,
    0,    0,  517,    0, 1428,  469,    0,    0,  251,    0,
    0,  341,  258,  347,  258,  348,   32,    0,    0,    0,
    0,    0,    0,  258,  509,    0,  258,    0,  258,    0,
  422,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   -2,
   24,   33,   69,    0,    0,    0,    0,    0, 1475,    0,
    0,    0,    0,  -22,    0,    0,    0,  199,    0,  549,
  491,    0,    0,    0,    0, 1655,    0,    0,    0,   40,
    0,    0,    0,    0,    0,    4,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  591,    0,    0,
    0,    0, 2019,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 2046,    0, 2114,    0,    0, 2142, 2331, 2427,
 2583, 2687, 2550,  496, 1662,  599,  111,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  199,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  349,    0,    0,  526,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  154,
  165,  528,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  353,  354,    0,  399,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  532,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2151, 2210, 2390, 2399,
 2450, 2487, 2495, 2524, 2646, 2685, 2744, 2742, 2782, 2784,
  910,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  400,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 2074,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  189,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  459,    0,    0,  124,  238,  -70,  858,  -97,    0,
  237,  249,  -12,  231,  357,  358,  360,  362,  359,    0,
  -78,    0, -116,   -5,    1,    0,    0,   -3,  485,    0,
  529,  156,  -68,    0,    0,    0,  145,  332,  536, -105,
  -39,  -14,    0,  -54,  -75,    0,    0,  340,   45, -182,
 -254,    0,  356,  -90,    0, -141,    0,    0,    0,    0,
  462,  573,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           150,
   29,  151,  200,  158,   65,  181,  193,  194,  202,  165,
  234,  225,  175,  226,  240,   43,   40,  150,  150,  215,
   44,  150,   43,  239,   44,  166,  145,   29,   57,  313,
   52,   53,   54,   55,   78,  142,  241,   81,   81,   81,
  276,   81,   73,  151,  151,  109,  249,  151,   44,  249,
  104,  129,  102,  201,  105,  106,   81,  107,  243,  238,
  317,  175,  356,   83,   83,   83,  194,   83,  150,  328,
  152,  154,   85,   85,   85,  391,   85,  266,  150,  157,
  109,  243,   83,  130,   67,  104,  270,  102,   81,  105,
  106,   85,  107,   79,  151,  265,  319,  250,  190,   68,
  250,  328,   43,  242,   44,   60,  110,  259,   87,   87,
   87,   59,   87,  195,   83,   81,  141,  252,  294,  295,
  296,   45,  195,   85,  286,  314,  242,   87,  196,  290,
  329,  129,  241,  345,  346,  149,  356,  354,  108,  283,
  237,  316,   57,  293,   80,  283,   60,   44,  109,  140,
   39,   61,  284,  104,   61,  102,  390,  105,  106,   87,
  107,   60,   56,  175,  130,  190,  194,  190,   61,   61,
   57,  351,  103,  108,  110,  150,  275,  318,  340,  326,
   58,  330,  348,  195,   48,  195,  190,  190,  334,  143,
  250,  335,   75,  120,  120,  120,  250,  218,  278,  352,
   82,  253,  216,   61,  122,  122,  122,  217,  168,  252,
  301,  302,  303,  304,  219,  167,  220,  223,  224,  233,
  153,  214,  147,  197,  198,  199,  214,  194,  214,  147,
  214,  214,   42,  214,  150,   61,  285,   78,   57,   42,
  235,  108,   91,  284,  120,  177,  150,  214,  355,  157,
  369,  370,  371,  363,   81,  122,  361,   91,  157,  366,
  151,  256,  279,   42,  257,  195,   42,  194,    8,  148,
   84,   85,   86,  379,   87,   88,  150,   17,   18,  378,
   83,  178,  386,  157,  388,  150,  179,  380,  341,   85,
  109,  195,  267,  392,  268,  104,  394,  102,  395,  105,
  106,  180,  107,  182,   83,   84,   85,   86,  183,   87,
   88,  214,  251,  214,  214,  184,  110,   89,   90,   42,
  150,  342,  355,  343,  195,   87,  195,  221,  222,  157,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   89,   90,   23,   24,   25,   26,  185,   91,
   92,   93,  229,   94,   95,   96,   97,   98,   99,  100,
  101,  230,   83,   84,   85,   86,    8,   87,   88,  349,
   57,  385,  350,  108,  195,   17,   18,  387,  389,  166,
  195,  195,  166,  163,  171,  231,  163,  171,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   89,   90,   23,   24,   25,   26,   70,   91,   92,   93,
  232,   94,   95,   96,   97,   98,   99,  100,  101,  172,
  173,  255,  172,  173,  261,  214,  214,  214,  214,  258,
  214,  214,  227,  228,  109,  297,  298,  305,  306,  104,
  269,  102,  274,  105,  106,  277,  107,  243,  280,  299,
  300,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,  214,  214,  214,  214,  214,  287,
  214,  214,  214,  327,  214,  214,  214,  214,  214,  214,
  214,  214,  242,  337,   83,   84,   85,   86,  213,   87,
   88,  281,  282,  292,  315,  137,  331,  338,   64,  339,
  137,  137,  344,  109,  137,  357,   55,  353,  104,   55,
  102,  360,  105,  106,  149,  107,  396,  108,  358,  137,
  144,  137,  367,   55,   55,  368,  347,  381,   55,  187,
  188,  383,   89,   90,   64,  384,  159,  393,  169,   91,
   92,   93,  171,   94,   95,   96,   97,   98,   99,  100,
  101,  137,  137,  134,  377,  307,  191,  308,   55,  134,
  309,  311,  134,  310,  139,  146,  109,  333,  236,   41,
    0,  104,  320,  102,    0,  105,  106,  134,  107,  134,
    0,    0,    0,  137,    0,    0,    0,    0,    0,   55,
   55,    0,    0,  149,    0,  133,  108,  188,    0,  188,
    0,  133,    0,    0,  133,    0,  271,  272,  273,   59,
  134,    0,   59,    0,    0,    0,   64,    0,    0,  133,
  144,  133,    0,  191,    0,  191,   59,   59,    0,  291,
    0,   59,    0,    0,    0,    0,    0,   71,    0,    0,
    0,  134,    0,    0,  191,  191,    0,    0,  148,   84,
   85,   86,  133,   87,   88,    0,    0,    0,    0,  108,
    1,   59,  312,   70,    0,    0,    0,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,  133,   23,   24,   25,   26,    0,    0,
    0,    0,    0,   59,   72,    0,   89,   90,    0,    0,
  203,  204,  205,  206,  207,  208,  209,  210,  211,  212,
    0,    0,    0,  144,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   57,    0,  148,   84,   85,
   86,    0,   87,   88,    0,    0,   55,   55,    0,    0,
    0,    0,    0,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,    0,    0,  137,  137,  137,
  137,    0,  372,  374,  376,   89,   90,    0,    0,    0,
    0,    0,    0,    0,  137,    0,    0,    0,    0,    0,
  148,   84,   85,   86,    0,   87,   88,    0,    0,    0,
    0,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,    8,    0,  134,  134,  134,  134,    0,
    0,    0,   17,   18,    0,    0,    0,    0,   89,   90,
   59,    0,  134,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,    0,    0,  133,  133,  133,
  133,  109,    0,    0,    0,    0,  104,    0,  102,    0,
  105,  106,    0,  107,  133,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,    0,    0,   23,
   24,   25,   26,  170,  172,  173,    0,    0,  174,    0,
   60,    0,    0,   60,    0,    0,   69,    0,    0,    0,
    0,  174,  174,    0,    0,    0,    0,   60,   60,    0,
    0,    0,   60,  109,  174,    0,    0,    0,  104,    0,
  102,    0,  105,  106,    0,  107,    0,    0,    0,    0,
    0,    0,    0,    0,  108,    0,    0,  174,    0,  110,
    0,    0,   60,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  174,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  109,    0,    0,   60,    0,  104,    0,  102,    0,
  164,  106,  109,  107,    0,    0,    0,  104,    0,  102,
  109,  264,  106,    0,  107,  104,    0,  102,    0,  105,
  106,    0,  107,  243,    0,    0,  108,    0,    0,    0,
    0,    0,    0,  174,  174,  174,  174,  174,  174,  174,
  174,  174,  174,  174,  174,  174,  174,  174,  174,  174,
  174,  163,    0,    0,    0,    0,    0,    0,    0,  174,
    0,    0,  263,    0,    0,    0,    0,    0,  242,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  174,    0,    0,  108,  148,   84,   85,   86,    0,
   87,   88,    0,    0,    0,  108,    0,  174,    0,    0,
  149,    0,    0,  108,    0,    0,    0,    0,    0,    0,
    0,    1,    0,    0,    0,    0,    0,    0,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   89,   90,   23,   24,   25,   26,    0,
  109,   60,  174,    0,    0,  104,    0,  102,    0,  105,
  106,    0,  107,    0,    0,    0,    0,  148,   84,   85,
   86,    0,   87,   88,    0,    0,  186,  109,    0,  174,
    0,    0,  104,    0,  102,  288,  105,  106,    0,  107,
    0,    0,  174,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   89,   90,   23,   24,   25,
   26,    0,    0,    0,    0,  148,   84,   85,   86,    0,
   87,   88,    0,    0,    0,    0,  148,   84,   85,   86,
    0,   87,   88,  108,  148,   84,   85,   86,    0,   87,
   88,    0,  109,    0,  162,    0,    0,  104,    8,  102,
    0,  105,  106,    0,  107,  262,    0,   17,   18,    8,
  108,    0,    0,   89,   90,    0,    0,    0,   17,   18,
    0,    0,    0,    0,   89,   90,    0,    0,    0,  109,
    0,    0,   89,   90,  104,    0,  102,    0,  325,  106,
    0,  107,    0,    0,  109,    0,    0,    0,    0,  104,
    0,  102,  260,  105,  106,  109,  107,    0,    0,    0,
  104,    0,  102,  109,  105,  106,    0,  107,  104,    0,
  102,    0,  105,  106,    0,  107,    0,    0,    0,    0,
    0,  110,    0,  109,    0,  108,    0,    0,  104,  324,
  102,  373,  105,  106,    0,  107,    0,    0,    0,    0,
    0,    0,    0,    0,  336,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  148,   84,   85,   86,  109,   87,
   88,    0,  108,  104,    0,  102,    0,  105,  106,    0,
  107,    0,    0,    0,    0,    0,    0,  108,    0,    0,
    0,  148,   84,   85,   86,  109,   87,   88,  108,    0,
  104,    0,  102,  347,  365,  106,  108,  107,    0,  109,
    0,    0,   89,   90,  104,    0,  102,  375,  105,  106,
  109,  107,    0,    0,    0,  104,  108,  102,  359,  105,
  106,    0,  107,    0,    0,    0,    0,    0,    0,   89,
   90,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  364,    0,    0,    0,    0,
    0,  108,    0,    0,    0,    0,  148,   84,   85,   86,
    0,   87,   88,    0,  127,  127,  127,  109,  127,    0,
  382,    0,  104,    0,  169,    0,  105,  106,  108,  107,
    0,    0,    0,  127,    0,    0,    0,    0,    0,    0,
    0,    0,  108,  148,   84,   85,   86,    0,   87,   88,
    0,    0,    0,  108,   89,   90,    0,    0,  148,   84,
   85,   86,    0,   87,   88,  127,    0,    0,    0,  148,
   84,   85,   86,    0,   87,   88,    0,  148,   84,   85,
   86,    0,   87,   88,    0,    0,    0,    0,    0,    0,
    0,   89,   90,  249,  321,   44,    0,  148,   84,   85,
   86,    0,   87,   88,  109,    0,   89,   90,    0,  104,
  108,  171,    0,  105,  106,    0,  107,   89,   90,    0,
    0,    0,    0,    0,    0,   89,   90,    0,    0,    0,
    0,    0,  148,   84,   85,   86,  362,   87,   88,    0,
    0,    0,  109,    0,  250,   89,   90,  104,    0,  102,
    0,  105,  106,    0,  107,    0,    0,    0,    0,  148,
   84,   85,   86,    0,   87,   88,    0,    0,    0,    0,
    0,    0,    0,  148,   84,   85,   86,    0,   87,   88,
   89,   90,    0,    0,  148,   84,   85,   86,    0,   87,
   88,    0,    0,    0,  115,  115,  115,  108,  115,    0,
    0,    0,   57,    0,    0,   57,    0,   89,   90,    0,
    0,    0,    0,  115,    0,    0,    0,    0,    0,   57,
   57,   89,   90,    0,   57,    0,    0,    0,  283,  321,
   44,  127,   89,   90,    0,  108,    0,    0,    0,    0,
    0,  148,   84,   85,   86,  115,   87,   88,    0,    0,
    0,    0,    0,    0,   57,    0,    0,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  250,
    0,  127,  127,  127,  127,  156,   57,    0,    0,   89,
   90,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   42,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  148,   84,
   85,   86,    0,   87,   88,    0,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,    0,    0,  148,   84,   85,   86,
    0,   87,   88,    0,    0,    0,   89,   90,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    0,    0,    0,    0,
    0,  115,    0,    0,   89,   90,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   57,   57,    0,    0,    0,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,    0,
    0,  115,  115,  115,  115,    0,    0,    0,    0,    0,
    0,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,    0,
    0,  155,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,    1,    1,    0,    1,    0,
    1,    1,    1,    1,    1,    1,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    1,    1,    1,
    1,    1,   19,   19,    0,    0,   19,   19,   19,   19,
   19,    0,   19,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   19,   19,   19,   19,   19,   19,    1,
   26,   26,    1,    0,   26,   26,   26,   26,   26,    0,
   26,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   26,   26,   26,   26,   26,   26,    0,   19,   19,
    0,    0,    1,    0,    0,    0,    0,    0,    0,    0,
   31,   31,    0,    0,   31,   31,   31,   31,   31,    0,
   31,    0,    0,    0,    0,    0,   26,   26,    0,   19,
   19,   31,   31,   31,    0,   31,   31,    0,    0,   37,
    0,    0,   37,    0,   37,   37,   37,    0,   38,    0,
    0,   38,    0,   38,   38,   38,    0,   26,   26,   37,
   37,   37,    0,   37,   37,    0,   31,   31,   38,   38,
   38,    0,   38,   38,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   37,   37,    0,   31,   31,    0,
    0,    0,    0,   38,   38,    0,    0,   39,    0,    0,
   39,    0,   39,   39,   39,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   37,   37,   39,   39,   39,
    0,   39,   39,    0,   38,   38,    0,    0,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    0,   39,   39,    0,    0,    0,    0,    0,   19,
   19,   19,   19,   19,   19,    0,   19,   19,   19,   19,
   19,   19,   19,   19,   19,   19,   19,   19,    0,    0,
    0,    0,    0,   39,   39,    0,    0,   26,   26,   26,
   26,   26,   26,    0,   26,   26,   26,   26,   26,   26,
   26,   26,   26,   26,   26,   26,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   40,    0,
    0,   40,    0,    0,   40,    0,    0,   31,   31,   31,
   31,   31,   31,    0,   31,   31,    0,    0,   40,   40,
   40,    0,   40,   40,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   37,   37,   37,   37,   37,
   37,    0,   37,   37,   38,   38,   38,   38,   38,   38,
    0,   38,   38,   40,   40,    0,    0,   41,    0,    0,
   41,    0,    0,   41,    0,    0,   42,    0,    0,   42,
    0,    0,   42,    0,    0,    0,    0,   41,   41,   41,
    0,   41,   41,    0,   40,   40,   42,   42,   42,    0,
   42,   42,    0,    0,   43,    0,    0,   43,    0,    0,
   43,    0,    0,   39,   39,   39,   39,   39,   39,    0,
   39,   39,   41,   41,   43,   43,   43,   46,   43,   43,
   46,   42,   42,   46,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   46,   46,   46,
    0,   46,   46,   41,   41,    0,    0,    0,    0,   43,
   43,    0,   42,   42,   47,    0,    0,   47,    0,    0,
   47,    0,   44,    0,    0,   44,    0,    0,   44,    0,
    0,    0,   46,   46,   47,   47,   47,    0,   47,   47,
   43,   43,   44,   44,   44,    0,   44,   44,    0,    0,
    0,   45,    0,    0,   45,    0,    0,   45,    0,    0,
    0,    0,    0,   46,   46,    0,    0,    0,    0,   47,
   47,   45,   45,   45,    0,   45,   45,   44,   44,    0,
   53,    0,    0,   53,   40,   40,   40,   40,   40,   40,
    0,   40,   40,    0,    0,    0,    0,   53,   53,    0,
   47,   47,   53,    0,    0,    0,   45,   45,   44,   44,
   48,    0,    0,   48,    0,    0,   48,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   48,   48,   53,   53,    0,   48,    0,   45,   45,    0,
    0,    0,    0,   41,   41,   41,   41,   41,   41,    0,
   41,   41,   42,   42,   42,   42,   42,   42,    0,   42,
   42,    0,    0,   53,   53,   48,   48,    0,    0,    0,
    0,    0,    0,   49,    0,    0,   49,    0,    0,   49,
    0,    0,   43,   43,   43,   43,    0,   43,   43,    0,
    0,    0,    0,   49,   49,    0,   48,   48,   49,    0,
    0,    0,    0,    0,    0,   46,   46,   46,   46,    0,
   46,   46,   50,    0,   51,   50,    0,   51,   50,    0,
   51,    0,    0,    0,    0,    0,    0,    0,   49,   49,
    0,    0,   50,   50,   51,   51,    0,   50,    0,   51,
    0,    0,   47,   47,   47,   47,    0,   47,   47,   57,
   44,   44,   44,   44,    0,   44,   44,    0,    0,   49,
   49,    0,    0,    0,    0,    0,    0,   50,   50,   51,
   51,   52,   54,    0,   52,   54,    0,   52,    0,   45,
   45,   45,   45,    0,   45,   45,    0,    0,    0,   54,
   54,   52,   52,    0,   54,    0,   52,    0,   50,   50,
   51,   51,    0,    0,    0,    0,    0,    0,    0,    0,
   53,   53,   56,    0,   58,   56,    0,   58,    0,    0,
    0,    0,    0,    0,   54,   54,   52,   52,    0,   56,
   56,   58,   58,    0,   56,    0,   58,    0,    0,    0,
   48,   48,    0,   48,   48,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   54,   54,   52,   52,    0,
    0,    0,    0,    0,   56,    0,   58,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,   56,   56,    0,   58,    0,
    0,    0,    0,   49,   49,    0,   49,   49,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,    0,
    0,    0,   50,   50,    0,   50,   50,   51,   51,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,  332,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   54,   54,   52,   52,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   56,   56,   58,   58,
  };
  protected static readonly short [] yyCheck = {            70,
    0,   70,   40,   79,   44,   96,  104,  105,   46,   80,
   63,   60,   91,   62,  125,   40,  123,   40,   41,  117,
   42,   44,   40,  140,   42,   80,   66,   27,  123,  125,
   30,   31,   32,   33,   49,   44,  142,   40,   41,   42,
  182,   44,   48,   40,   41,   33,   40,   44,   42,   40,
   38,   57,   40,   91,   42,   43,   59,   45,   46,   44,
   44,  140,  317,   40,   41,   42,  164,   44,   91,  252,
   76,   77,   40,   41,   42,   44,   44,  168,  149,   79,
   33,   46,   59,   44,   44,   38,  177,   40,   91,   42,
   43,   59,   45,   40,   91,  166,   61,   91,  102,   59,
   91,  284,   40,   91,   42,  257,   59,  162,   40,   41,
   42,  123,   44,   44,   91,  270,  125,  157,  216,  217,
  218,   59,   44,   91,  195,  242,   91,   59,   59,  200,
   40,  137,  238,  275,  276,  123,  391,  125,  126,   40,
  125,  125,  123,  214,   91,   40,  257,   42,   33,   61,
  257,   41,  192,   38,   44,   40,  125,   42,   43,   91,
   45,  257,  257,  242,  125,  169,  264,  171,   58,   59,
  123,   93,  125,  126,   59,  246,  182,  246,  269,  250,
   36,   91,  280,   44,   29,   44,  190,  191,  259,   41,
   91,  262,   48,   40,   41,   42,   91,   37,   59,   58,
   56,  157,   42,   93,   40,   41,   42,   47,   58,  249,
  223,  224,  225,  226,   43,  257,   45,  266,  267,  272,
   76,   33,   67,  261,  262,  263,   38,  325,   40,   74,
   42,   43,  257,   45,  257,  125,  192,  252,  123,  257,
  125,  126,   44,  283,   91,   58,  317,   59,  317,  249,
  341,  342,  343,  329,  257,   91,  327,   59,  258,  330,
  257,   41,   41,  257,   44,   44,  257,  365,  290,  257,
  258,  259,  260,  352,  262,  263,  347,  299,  300,  350,
  257,   40,  373,  283,  375,  356,   40,  356,   41,  257,
   33,   44,  169,  384,  171,   38,  387,   40,  389,   42,
   43,   40,   45,   40,  257,  258,  259,  260,  257,  262,
  263,  123,  157,  125,  126,   59,   59,  305,  306,  257,
  391,   41,  391,   41,   44,  257,   44,  264,  265,  329,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,   59,  312,
  313,  314,   38,  316,  317,  318,  319,  320,  321,  322,
  323,   94,  257,  258,  259,  260,  290,  262,  263,   41,
  123,   41,   44,  126,   44,  299,  300,   41,   41,   41,
   44,   44,   44,   41,   41,  124,   44,   44,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,   61,  312,  313,  314,
  271,  316,  317,  318,  319,  320,  321,  322,  323,   41,
   41,   41,   44,   44,   93,  257,  258,  259,  260,   44,
  262,  263,  268,  269,   33,  219,  220,  227,  228,   38,
   58,   40,  317,   42,   43,   59,   45,   46,   41,  221,
  222,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  257,
  312,  313,  314,   61,  316,  317,  318,  319,  320,  321,
  322,  323,   91,   93,  257,  258,  259,  260,   61,  262,
  263,  190,  191,  257,  257,   35,  257,   41,   44,   41,
   40,   41,   40,   33,   44,   41,   41,   93,   38,   44,
   40,   93,   42,   43,  123,   45,  125,  126,   41,   59,
   66,   61,   93,   58,   59,   93,  123,   41,   63,  101,
  102,   93,  305,  306,   80,  315,   41,   59,   41,  312,
  313,  314,   41,  316,  317,  318,  319,  320,  321,  322,
  323,   91,   92,   35,  347,  229,  102,  230,   93,   41,
  231,  233,   44,  232,   59,   67,   33,  258,  137,   27,
   -1,   38,  247,   40,   -1,   42,   43,   59,   45,   61,
   -1,   -1,   -1,  123,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,  123,   -1,   35,  126,  169,   -1,  171,
   -1,   41,   -1,   -1,   44,   -1,  178,  179,  180,   41,
   92,   -1,   44,   -1,   -1,   -1,  162,   -1,   -1,   59,
  166,   61,   -1,  169,   -1,  171,   58,   59,   -1,  201,
   -1,   63,   -1,   -1,   -1,   -1,   -1,   35,   -1,   -1,
   -1,  123,   -1,   -1,  190,  191,   -1,   -1,  257,  258,
  259,  260,   92,  262,  263,   -1,   -1,   -1,   -1,  126,
  283,   93,  234,   61,   -1,   -1,   -1,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,   -1,  123,  307,  308,  309,  310,   -1,   -1,
   -1,   -1,   -1,  125,   92,   -1,  305,  306,   -1,   -1,
  273,  274,  275,  276,  277,  278,  279,  280,  281,  282,
   -1,   -1,   -1,  259,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  123,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,  271,  272,   -1,   -1,
   -1,   -1,   -1,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,   -1,   -1,  307,  308,  309,
  310,   -1,  344,  345,  346,  305,  306,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  324,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  290,   -1,  307,  308,  309,  310,   -1,
   -1,   -1,  299,  300,   -1,   -1,   -1,   -1,  305,  306,
  272,   -1,  324,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,   -1,   -1,  307,  308,  309,
  310,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,  324,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,   -1,   -1,  307,
  308,  309,  310,   86,   87,   88,   -1,   -1,   91,   -1,
   41,   -1,   -1,   44,   -1,   -1,  324,   -1,   -1,   -1,
   -1,  104,  105,   -1,   -1,   -1,   -1,   58,   59,   -1,
   -1,   -1,   63,   33,  117,   -1,   -1,   -1,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  126,   -1,   -1,  140,   -1,   59,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  164,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,   -1,  125,   -1,   38,   -1,   40,   -1,
   42,   43,   33,   45,   -1,   -1,   -1,   38,   -1,   40,
   33,   42,   43,   -1,   45,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   46,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   -1,   -1,  216,  217,  218,  219,  220,  221,  222,
  223,  224,  225,  226,  227,  228,  229,  230,  231,  232,
  233,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  242,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   91,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,   -1,   -1,  126,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,  126,   -1,  280,   -1,   -1,
  123,   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  283,   -1,   -1,   -1,   -1,   -1,   -1,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,   -1,
   33,  272,  325,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   59,   33,   -1,  352,
   -1,   -1,   38,   -1,   40,   41,   42,   43,   -1,   45,
   -1,   -1,  365,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,  126,  257,  258,  259,  260,   -1,  262,
  263,   -1,   33,   -1,  286,   -1,   -1,   38,  290,   40,
   -1,   42,   43,   -1,   45,  286,   -1,  299,  300,  290,
  126,   -1,   -1,  305,  306,   -1,   -1,   -1,  299,  300,
   -1,   -1,   -1,   -1,  305,  306,   -1,   -1,   -1,   33,
   -1,   -1,  305,  306,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   -1,   -1,   33,   -1,   -1,   -1,   -1,   38,
   -1,   40,   93,   42,   43,   33,   45,   -1,   -1,   -1,
   38,   -1,   40,   33,   42,   43,   -1,   45,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   59,   -1,   33,   -1,  126,   -1,   -1,   38,   93,
   40,   41,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   33,  262,
  263,   -1,  126,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,   -1,
   -1,  257,  258,  259,  260,   33,  262,  263,  126,   -1,
   38,   -1,   40,  123,   42,   43,  126,   45,   -1,   33,
   -1,   -1,  305,  306,   38,   -1,   40,   41,   42,   43,
   33,   45,   -1,   -1,   -1,   38,  126,   40,   93,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,  305,
  306,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,
   -1,  126,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   40,   41,   42,   33,   44,   -1,
   93,   -1,   38,   -1,   40,   -1,   42,   43,  126,   45,
   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  126,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,  126,  305,  306,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   91,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  305,  306,   40,   41,   42,   -1,  257,  258,  259,
  260,   -1,  262,  263,   33,   -1,  305,  306,   -1,   38,
  126,   40,   -1,   42,   43,   -1,   45,  305,  306,   -1,
   -1,   -1,   -1,   -1,   -1,  305,  306,   -1,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,   41,  262,  263,   -1,
   -1,   -1,   33,   -1,   91,  305,  306,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
  305,  306,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   40,   41,   42,  126,   44,   -1,
   -1,   -1,   41,   -1,   -1,   44,   -1,  305,  306,   -1,
   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,   58,
   59,  305,  306,   -1,   63,   -1,   -1,   -1,   40,   41,
   42,  257,  305,  306,   -1,  126,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   91,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   91,
   -1,  307,  308,  309,  310,   41,  125,   -1,   -1,  305,
  306,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,   -1,   -1,
  307,  308,  309,  310,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,  305,  306,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
   -1,   -1,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,  257,   -1,   -1,  305,  306,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,   -1,   -1,   -1,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   -1,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,   -1,   -1,  307,  308,  309,  310,   -1,
   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,   37,   38,   -1,   40,   -1,
   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   63,   37,   38,   -1,   -1,   41,   42,   43,   44,
   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,   91,
   37,   38,   94,   -1,   41,   42,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   61,   62,   63,   -1,   93,   94,
   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   37,   38,   -1,   -1,   41,   42,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,
  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,   38,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   38,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,  124,  125,   58,
   59,   60,   -1,   62,   63,   -1,   93,   94,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,  124,  125,   -1,
   -1,   -1,   -1,   93,   94,   -1,   -1,   38,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,  124,  125,   -1,   -1,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,  273,  274,
  275,  276,  277,  278,  279,  280,  281,  282,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,  282,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   93,   94,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   58,   59,   60,
   -1,   62,   63,   -1,  124,  125,   58,   59,   60,   -1,
   62,   63,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,   93,   94,   58,   59,   60,   38,   62,   63,
   41,   93,   94,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,
   -1,   62,   63,  124,  125,   -1,   -1,   -1,   -1,   93,
   94,   -1,  124,  125,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   93,   94,   58,   59,   60,   -1,   62,   63,
  124,  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,
   94,   58,   59,   60,   -1,   62,   63,   93,   94,   -1,
   41,   -1,   -1,   44,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,   -1,   58,   59,   -1,
  124,  125,   63,   -1,   -1,   -1,   93,   94,  124,  125,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   93,   94,   -1,   63,   -1,  124,  125,   -1,
   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,   -1,   -1,  124,  125,   93,   94,   -1,   -1,   -1,
   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   -1,   -1,   58,   59,   -1,  124,  125,   63,   -1,
   -1,   -1,   -1,   -1,   -1,  266,  267,  268,  269,   -1,
  271,  272,   38,   -1,   38,   41,   -1,   41,   44,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,
   -1,   -1,   58,   59,   58,   59,   -1,   63,   -1,   63,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,  123,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   93,
   94,   38,   41,   -1,   41,   44,   -1,   44,   -1,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,   58,
   59,   58,   59,   -1,   63,   -1,   63,   -1,  124,  125,
  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  271,  272,   41,   -1,   41,   44,   -1,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   93,   94,   -1,   58,
   59,   58,   59,   -1,   63,   -1,   63,   -1,   -1,   -1,
  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,  124,  125,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   93,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,  124,  125,   -1,  125,   -1,
   -1,   -1,   -1,  268,  269,   -1,  271,  272,   -1,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,   -1,   -1,   -1,
   -1,   -1,  268,  269,   -1,  271,  272,  271,  272,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,  311,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,  271,  272,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,  271,  272,
  };

#line 986 "CParser.jay"

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
