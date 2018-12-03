// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "CParser.jay"
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using CLanguage.Syntax;
using CLanguage.Types;

#pragma warning disable 162

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
    null,"'['",null,"']'","'^'",null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,"'{'","'|'","'}'","'~'",null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"IDENTIFIER","CONSTANT","STRING_LITERAL","SIZEOF","PTR_OP",
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
#line 42 "CParser.jay"
  { yyVal = new VariableExpression((yyVals[0+yyTop]).ToString()); }
  break;
case 2:
#line 43 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 44 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 45 "CParser.jay"
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 46 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 47 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 54 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 58 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 62 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 66 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 70 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 74 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
	}
  break;
case 13:
#line 78 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 82 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 86 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 90 "CParser.jay"
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
#line 112 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 116 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 120 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 124 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 128 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 132 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 136 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 140 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 144 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 145 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 146 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 147 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 154 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 158 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 165 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 169 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 173 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 177 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 185 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 189 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 197 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 201 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 209 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 213 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 217 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 221 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 229 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 233 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 241 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 249 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 257 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 265 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 273 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 280 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 284 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 302 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 303 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 304 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 305 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 306 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 307 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 308 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 309 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 310 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 311 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 312 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 319 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 77:
#line 323 "CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 79:
  case_79();
  break;
case 80:
  case_80();
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
  case_91();
  break;
case 92:
  case_92();
  break;
case 93:
#line 429 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 94:
#line 430 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 95:
#line 431 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 96:
#line 432 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 97:
#line 433 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 98:
#line 437 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 99:
#line 438 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 100:
#line 439 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 101:
#line 440 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 102:
#line 441 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 103:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 104:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 105:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 106:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 107:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 108:
#line 447 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 109:
#line 448 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 112:
#line 451 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 113:
#line 455 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 114:
#line 456 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 115:
#line 457 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 116:
#line 461 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 117:
#line 462 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 118:
#line 463 "CParser.jay"
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
#line 492 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 124:
#line 493 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString()); }
  break;
case 125:
#line 494 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 126:
#line 495 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString()); }
  break;
case 127:
#line 496 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 132:
#line 510 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 133:
#line 517 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 134:
#line 518 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 135:
#line 523 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 136:
#line 524 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 138:
  case_138();
  break;
case 139:
#line 543 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 140:
#line 547 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 141:
#line 551 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 142:
#line 555 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 143:
#line 559 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 144:
#line 563 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 145:
#line 567 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 146:
#line 571 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 147:
  case_147();
  break;
case 148:
  case_148();
  break;
case 149:
  case_149();
  break;
case 150:
#line 599 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 151:
#line 600 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 152:
#line 601 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 153:
#line 602 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 154:
#line 606 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 155:
#line 610 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 156:
#line 614 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 157:
#line 615 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 158:
#line 616 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 159:
#line 623 "CParser.jay"
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
  case_163();
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
  case_167();
  break;
case 168:
#line 684 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 169:
#line 688 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 173:
  case_173();
  break;
case 174:
#line 713 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 175:
#line 717 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 176:
#line 721 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 177:
#line 725 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 178:
#line 729 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 179:
#line 733 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 180:
  case_180();
  break;
case 181:
  case_181();
  break;
case 182:
  case_182();
  break;
case 183:
  case_183();
  break;
case 184:
#line 767 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 185:
#line 771 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 186:
#line 775 "CParser.jay"
  {
		yyVal = yyVals[-2+yyTop];
	}
  break;
case 187:
  case_187();
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
#line 815 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 205:
#line 847 "CParser.jay"
  {
		yyVal = new Block (GetLocation(yyVals[-1+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 206:
#line 851 "CParser.jay"
  {
        yyVal = new Block (GetLocation(yyVals[-2+yyTop]), (List<Statement>)yyVals[-1+yyTop], GetLocation(yyVals[0+yyTop]));
	}
  break;
case 207:
#line 855 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 208:
#line 856 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 211:
#line 868 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 212:
#line 872 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 213:
#line 879 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 214:
#line 883 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 216:
#line 891 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 217:
#line 895 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 218:
#line 899 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock (), null, GetLocation(yyVals[-5+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 219:
#line 903 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock (), null, GetLocation(yyVals[-6+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 220:
#line 907 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 221:
#line 911 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 225:
#line 921 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 226:
#line 925 "CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 227:
  case_227();
  break;
case 228:
  case_228();
  break;
case 231:
  case_231();
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
#line 95 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 101 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 290 "CParser.jay"
{
		if (yyVals[-1+yyTop] is RelationalOp && ((RelationalOp)yyVals[-1+yyTop]) == RelationalOp.Equals) {
			yyVal = new AssignExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
		}
		else {
			var left = (Expression)yyVals[-2+yyTop]; 
			yyVal = new AssignExpression(left, new BinaryExpression (left, (Binop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]));
		}
	}

void case_79()
#line 332 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_80()
#line 338 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		d.InitDeclarators = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_81()
#line 348 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_82()
#line 354 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_83()
#line 360 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_84()
#line 366 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_85()
#line 372 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_86()
#line 378 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_87()
#line 384 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 390 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 399 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_90()
#line 405 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_91()
#line 414 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[0+yyTop];
		yyVal = id;
	}

void case_92()
#line 420 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[-2+yyTop];
		id.Initializer = (Initializer)yyVals[0+yyTop];
		yyVal = id;
	}

void case_119()
#line 468 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_120()
#line 473 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_121()
#line 479 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_122()
#line 484 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_138()
#line 529 "CParser.jay"
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

void case_147()
#line 573 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_148()
#line 580 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDeclaration(n));
		}
		yyVal = d;
	}

void case_149()
#line 590 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_160()
#line 625 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_161()
#line 634 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_162()
#line 640 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_163()
#line 649 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_164()
#line 654 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_165()
#line 659 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_166()
#line 667 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_167()
#line 673 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_173()
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

void case_180()
#line 735 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-1+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_181()
#line 742 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_182()
#line 748 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration> ();
		yyVal = d;
	}

void case_183()
#line 755 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_187()
#line 780 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_188()
#line 787 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_189()
#line 795 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_190()
#line 802 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_227()
#line 930 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_228()
#line 935 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_231()
#line 948 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_232()
#line 957 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_233()
#line 968 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_234()
#line 974 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_235()
#line 980 "CParser.jay"
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
   47,   47,   48,   48,   48,   46,   46,    5,    5,   49,
   49,   49,   50,   50,   50,   50,   50,   50,   50,   50,
   50,   50,   50,   33,   33,   33,    6,    6,    6,    6,
   51,   52,   52,   53,   53,   54,   54,   54,   54,   54,
   54,   55,   55,   55,   37,   37,   60,   60,   61,   61,
   56,   56,   57,   57,   57,   58,   58,   58,   58,   58,
   58,   59,   59,   59,   59,   59,    0,    0,   62,   62,
   63,   63,   64,   64,   64,   65,   65,
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
    1,    3,    2,    2,    1,    1,    3,    1,    2,    1,
    1,    2,    3,    2,    3,    3,    4,    3,    4,    2,
    3,    3,    4,    1,    3,    4,    1,    2,    3,    4,
    2,    1,    2,    3,    2,    1,    1,    1,    1,    1,
    1,    3,    4,    3,    2,    3,    1,    2,    1,    1,
    1,    2,    5,    7,    5,    5,    7,    6,    7,    6,
    7,    3,    2,    2,    2,    3,    1,    2,    1,    1,
    4,    3,    1,    2,    2,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  112,   93,   94,   95,   96,   97,  132,  157,   99,  100,
  101,  102,  105,  106,  103,  104,  156,  158,   98,  107,
  108,  109,  116,  117,  118,    0,    0,  230,    0,    0,
    0,    0,    0,  110,  111,    0,  227,  229,    0,    0,
  228,  135,    0,    0,   79,    0,   89,    0,    0,    0,
    0,   82,   84,   86,   88,    0,    0,  114,    0,    0,
    0,  128,    0,  154,  152,    0,    0,   80,  236,    0,
  237,  233,    0,  232,    0,    0,    0,    0,    0,    0,
  113,    0,    2,    3,    0,    0,    0,    4,    5,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  205,    0,    0,   27,   28,   29,   30,  211,    7,
    0,    0,   76,    0,   33,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   63,  209,  197,  210,
  196,  198,  199,  200,  201,    0,  207,    0,    0,  123,
    0,  138,  155,  153,   90,    0,    1,    0,  184,   92,
  235,  231,  234,  166,  149,    0,    0,    0,    0,  161,
    0,  146,    0,    0,    0,  136,    0,    0,   25,    0,
   20,   21,   31,   78,    0,    0,    0,    0,    0,    0,
    0,    0,  223,  224,  225,    0,    0,    0,    0,    0,
    0,   22,   23,    0,  212,    0,   13,   14,    0,    0,
    0,   66,   67,   68,   69,   70,   71,   72,   73,   74,
   75,   65,    0,   24,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  206,  208,  124,    0,  131,  125,  129,
    0,    0,    0,  187,    0,    0,  192,    0,    0,  163,
    0,  164,    0,  147,  148,    0,    0,    0,  145,  141,
    0,  140,    0,    0,  202,    0,    0,    0,  204,    0,
    0,    0,    0,    0,    0,  222,  226,    6,    0,  119,
  121,    0,    0,  169,   77,   12,    9,    0,   17,    0,
   11,   64,   34,   35,   36,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  126,    0,  195,  185,    0,  188,  191,  193,  180,
    0,    0,  174,    0,    0,    0,    0,    0,  167,  160,
  162,    0,    0,  144,  139,    0,    0,  203,    0,    0,
    0,    0,    0,    0,    0,   32,   10,    0,    8,    0,
  194,  186,  189,    0,  181,  173,  178,  175,  182,    0,
  176,    0,    0,  142,  143,    0,  215,  216,    0,    0,
    0,    0,    0,    0,   18,   62,  190,  183,  179,  177,
    0,    0,  220,    0,  218,    0,   15,    0,  214,  217,
  221,  219,   16,
  };
  protected static readonly short [] yyDgoto  = {            27,
  110,  111,  112,  288,  188,  243,  113,  114,  115,  116,
  117,  118,  119,  120,  121,  122,  123,  124,  125,  126,
  127,  213,  175,   28,   73,   46,   30,   31,   32,   33,
   47,   63,  244,   34,   35,   36,  129,  191,   61,   62,
   49,   50,   51,   66,  321,  158,  159,  160,  322,  253,
  245,  246,  247,  130,  131,  132,  133,  134,  135,  136,
  137,   37,   38,   75,   76,
  };
  protected static readonly short [] yySindex = {         2635,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -93, 2635,    0,   22, 2635,
 2635, 2635, 2635,    0,    0,  -79,    0,    0,  -60, -220,
    0,    0,  -27,  -25,    0,    7,    0,  495,  -33,   21,
 -215,    0,    0,    0,    0,  -50,   73,    0, -220,   74,
   41,    0,   99,    0,    0,  -25,  -27,    0,    0,  451,
    0,    0,   22,    0, 2549, 2635,   21, 1778,  942, -104,
    0,  154,    0,    0, 1463, 1502, 1502,    0,    0, 1517,
  160,  197,  207,  258,  510,  265,   52,  234,  275, 1083,
  876,    0, 1517, 1517,    0,    0,    0,    0,    0,    0,
  167,  -34,    0,  139,    0, 1517,  282,  142,   61,  -22,
  133,  273,  227,  215,   70,  -40,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  146,    0,   43, 1517,    0,
 -101,    0,    0,    0,    0,  286,    0, 1017,    0,    0,
    0,    0,    0,    0,    0,   58,  313,   27,  340,    0,
  -49,    0, 1222,  295,  953,    0,  510,  876,    0,  876,
    0,    0,    0,    0,  353,  510, 1517, 1517, 1517,  111,
  648,  398,    0,    0,    0,  172,  107,  420, 1495, 1495,
    3,    0,    0, 1517,    0,  214,    0,    0, 1259, 1517,
  233,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 1517,    0, 1517, 1517, 1517, 1517, 1517, 1517,
 1517, 1517, 1517, 1517, 1517, 1517, 1517, 1517, 1517, 1517,
 1517, 1517, 1517,    0,    0,    0,  -90,    0,    0,    0,
 1517,  235,   49,    0,  451,  149,    0, 1692, 1274,    0,
  -32,    0,   56,    0,    0,  238, 2606, 1031,    0,    0,
 1517,    0, 1311,  404,    0,  458,  460,  510,    0,  232,
  266,  299,  494, 1328, 1328,    0,    0,    0, 1336,    0,
    0, 1730,  110,    0,    0,    0,    0,  307,    0,   12,
    0,    0,    0,    0,    0,  282,  282,  142,  142,   61,
   61,   61,   61,  -22,  -22,  133,  273,  227,  215,   70,
   84,    0,  454,    0,    0,  -24,    0,    0,    0,    0,
  513,  518,    0, 1363,  467,   56, 1588, 1398,    0,    0,
    0,  468,  472,    0,    0,  428,  428,    0,  510,  510,
  510, 1517, 1387, 1425, 1017,    0,    0, 1517,    0, 1517,
    0,    0,    0,  451,    0,    0,    0,    0,    0,  525,
    0, 1450,  474,    0,    0,  257,    0,    0,  311,  510,
  366,  510,  381,   83,    0,    0,    0,    0,    0,    0,
  510,  514,    0,  510,    0,  510,    0,   32,    0,    0,
    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  930,
  965, 1106, 1351,    0,    0,    0,    0,    0, 1560,    0,
    0,    0,    0,  -15,    0,    0,    0,  253,    0,  439,
  551,    0,    0,    0,    0, 1636,    0,    0,    0,   90,
    0,    0,    0,    0,    0,   89,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  593,    0,    0,    0,
    0, 2052,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2079,    0, 2147,    0,    0, 2175, 2364, 2460,   45,
  959,   78,  985,   -5,  546,  562,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  253,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  382,    0,    0,  534,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  179,  262,
  537,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  383,    0,  429,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  539,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 2184, 2243, 2423, 2432, 2483,
 2520, 2528, 2557,  538, 2616, 2679,  505, 2583,   51, 1269,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  431,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 2107,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  219,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  470,    0,    0,   36,  236,  -68,  874,  -56,    0,
  267,  324,  175,  261,  355,  356,  354,  357,  361,    0,
  -87,    0,  -98,  569,    1,    0,    0,   24, 1645,    0,
  522,   17,  -70,    0,    0,    0,  252,  368,  535,  -91,
  -39,  -29,    0,  -37,  -74,    0,    0,  343,   26,  -86,
 -267,    0,  362,  -85,    0, -112,    0,    0,    0,    0,
  466,  580,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           150,
   29,  149,  174,  157,   65,  199,   43,  248,  108,  180,
  164,  201,   43,  103,   44,  101,   44,  104,  105,   77,
  106,  242,  233,  239,  150,  150,  144,   29,  150,   40,
   52,   53,   54,   55,  312,   57,   60,  224,   57,  225,
  238,  165,  282,   57,   44,   48,  192,  193,  354,  240,
   67,  174,   57,   57,   80,  194,  200,   57,  249,  214,
   78,   43,   59,   44,  108,   68,  241,  255,  275,  103,
  256,  101,   57,  104,  105,  150,  106,  242,  156,  149,
   45,  265,   48,  146,  141,   48,  237,   57,   48,  146,
  269,   58,  316,  249,   58,  327,  264,  248,  148,   44,
  352,  107,   48,   48,  349,  108,  193,   48,   58,   58,
  103,   79,  101,   58,  104,  105,  251,  106,   53,   57,
  354,   53,  241,  258,  189,  285,  388,  194,  151,  151,
  289,  109,  151,  130,  139,   53,   53,   48,   48,  142,
   53,  350,  313,   58,  292,  240,  328,  278,  249,  282,
  194,  283,  166,  174,  148,   60,  393,  107,  293,  294,
  295,  343,  344,   39,  326,  140,   60,  236,   48,   48,
   53,   53,  250,  315,  317,   58,  149,   56,  108,  151,
  325,  252,  338,  103,  218,  101,  219,  104,  105,  332,
  106,  189,  333,  189,  242,   57,  326,  102,  107,  212,
  249,   53,   53,  266,  109,  267,  193,  387,  251,  318,
  194,  167,  189,  189,  130,  194,  284,  176,  120,  120,
  120,   77,  346,   42,   42,  195,  196,  197,  198,   42,
  277,  232,  147,   83,   84,   85,  177,   86,   87,  241,
    8,  150,  283,  222,  223,  353,  178,  149,  156,   17,
   18,  213,  360,  366,  367,  368,  213,  156,  213,  363,
  213,  213,  376,  213,    8,   57,   57,  193,   57,  120,
  234,  107,  339,   17,   18,  194,  149,  213,   42,  375,
   88,   89,  156,  377,  383,  149,  385,   58,  147,   83,
   84,   85,  183,   86,   87,  389,   91,  179,  391,   74,
  392,  122,  122,  122,  181,  193,  340,   81,  182,  194,
  228,   91,   48,   48,   42,   48,   48,  353,  217,  149,
  229,   58,   58,  215,  220,  221,  152,  156,  216,   82,
   83,   84,   85,  184,   86,   87,   88,   89,  230,  341,
  231,  213,  194,  213,  213,  151,   70,  347,   53,   53,
  348,  382,  122,  254,  194,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   88,   89,   23,
   24,   25,   26,  257,   90,   91,   92,  260,   93,   94,
   95,   96,   97,   98,   99,  100,  300,  301,  302,  303,
  226,  227,   82,   83,   84,   85,  384,   86,   87,  194,
  268,  202,  203,  204,  205,  206,  207,  208,  209,  210,
  211,  386,  165,  170,  194,  165,  170,  273,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   88,   89,   23,   24,   25,   26,  276,   90,   91,   92,
  279,   93,   94,   95,   96,   97,   98,   99,  100,  171,
  286,  172,  171,  134,  172,  213,  213,  213,  213,  134,
  213,  213,  134,  108,  296,  297,  304,  305,  103,  291,
  101,  314,  104,  105,  329,  106,  335,  134,  336,  134,
  337,  213,  213,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  213,  213,  213,  213,  213,  213,   71,
  213,  213,  213,  342,  213,  213,  213,  213,  213,  213,
  213,  213,  108,  298,  299,   54,  351,  103,   54,  101,
  345,  104,  105,  355,  106,   70,  280,  281,  356,  358,
  364,  134,   54,   54,  365,  378,  380,   54,  109,  186,
  187,  381,  390,  148,  159,   49,  107,  168,   49,  170,
  374,   49,  306,  308,  307,  137,   59,  309,  145,   59,
  137,  137,  310,  138,  137,   49,   49,   54,   54,  331,
   49,  235,   61,   59,   59,   61,   41,  319,   59,  137,
    0,  137,    0,    0,    0,    0,   72,   57,    0,   61,
   61,    0,    0,    0,    0,  128,    0,  133,   54,   54,
   49,   49,   57,  133,    0,  107,  133,  187,   59,  187,
    0,  137,    0,  151,  153,    0,  270,  271,  272,    0,
    0,  133,    0,  133,   61,    0,    0,    0,    0,    0,
    0,   49,   49,    0,    0,    0,    0,    0,    0,  290,
   59,    0,    0,  137,    0,    0,    0,    0,    0,    0,
  108,    0,    0,    0,    0,  103,   61,  101,    0,  104,
  105,    0,  106,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  311,    0,  128,    0,  109,  147,   83,   84,
   85,    0,   86,   87,    0,  133,    0,    0,    0,    0,
    0,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,    0,    0,  134,  134,  134,  134,  274,
    0,    0,    0,    0,    0,   88,   89,    0,    0,    0,
    0,    0,  134,    0,    0,    0,   82,   83,   84,   85,
    0,   86,   87,  107,    0,   54,   54,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,   49,   49,    0,   49,   49,
    0,  369,  371,  373,   88,   89,    0,   59,   69,    0,
    0,   90,   91,   92,    0,   93,   94,   95,   96,   97,
   98,   99,  100,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,    0,    0,  137,  137,  137,
  137,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  137,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,    0,    0,  133,
  133,  133,  133,    0,  147,   83,   84,   85,  108,   86,
   87,    0,    0,  103,    0,  101,  133,  104,  105,    0,
  106,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   88,   89,   23,   24,   25,   26,  169,  171,
  172,    0,    0,  173,    0,    0,    0,    0,    0,   81,
   81,   81,    0,   81,  108,    0,  173,  173,    0,  103,
    0,  101,    0,  163,  105,  108,  106,    0,   81,  173,
  103,    0,  101,    0,  263,  105,   51,  106,    0,   51,
    0,  107,   51,    0,   83,   83,   83,    0,   83,    0,
    0,    0,  173,    0,    0,    0,   51,   51,    0,    0,
   81,   51,    0,   83,    0,   55,    0,    0,   55,    0,
    0,    0,    0,    0,  162,    0,  173,    0,    0,    0,
    0,    0,   55,   55,    0,  262,    0,   55,    0,  108,
    0,   51,   51,    0,  103,   83,  101,    0,  104,  105,
    0,  106,  242,  108,    0,    0,    0,  107,  103,    0,
  101,    0,  104,  105,    0,  106,    0,   55,  107,    0,
    0,    0,   51,   51,    0,    0,    0,    0,  173,  173,
  173,  173,  173,  173,  173,  173,  173,  173,  173,  173,
  173,  173,  173,  173,  173,  173,    0,  241,   55,   55,
    0,    0,    0,    0,  173,  108,    0,    0,    0,    0,
  103,    0,  101,    0,  104,  105,    0,  106,    0,    0,
    0,    0,  147,   83,   84,   85,  173,   86,   87,  148,
    0,  185,  107,    0,    0,   85,   85,   85,    0,   85,
    0,    0,  173,    0,    0,    0,  107,    0,    1,    0,
    0,    0,    0,    0,   85,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   88,   89,   23,   24,   25,   26,   81,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   85,  173,  147,   83,
   84,   85,    0,   86,   87,    0,    0,    0,  107,  147,
   83,   84,   85,    0,   86,   87,    0,    0,    0,    0,
    0,   83,    0,  173,    0,    0,    0,  161,    0,   51,
   51,    8,    0,    0,    0,  173,    0,    0,  261,    0,
   17,   18,    8,    0,    0,    0,   88,   89,    0,    0,
    0,   17,   18,    0,  108,   55,   55,   88,   89,  103,
    0,  101,    0,  104,  105,    0,  106,    0,    0,    0,
    0,    0,    0,  147,   83,   84,   85,    0,   86,   87,
    0,    0,    0,    0,    0,    0,    0,  147,   83,   84,
   85,  108,   86,   87,    0,    0,  103,    0,  101,  287,
  104,  105,    0,  106,    0,    0,  108,    0,    0,   60,
    0,  103,   60,  101,  259,  324,  105,    0,  106,    0,
    8,   88,   89,    0,    0,    0,   60,   60,    0,   17,
   18,   60,    0,    0,    0,   88,   89,    0,    0,  147,
   83,   84,   85,  108,   86,   87,    0,  107,  103,    0,
  101,    0,  104,  105,    0,  106,    0,    0,    0,    0,
  108,   60,   85,    0,    0,  103,  323,  101,  108,  104,
  105,    0,  106,  103,    0,  101,    0,  104,  105,    0,
  106,    0,    0,    0,  107,    0,  109,   88,   89,    0,
   87,   87,   87,   60,   87,  108,    0,    0,    0,  107,
  103,    0,  101,  334,  104,  105,    0,  106,    0,   87,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  108,
    0,    0,    0,    0,  103,    0,  101,  370,  104,  105,
  108,  106,    0,    0,    0,  103,  107,  101,    0,  362,
  105,   87,  106,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  107,    0,  357,    0,  108,  345,    0,
    0,  107,  103,    0,  101,  372,  104,  105,    0,  106,
    0,    0,    0,    0,    0,    0,    0,    0,  147,   83,
   84,   85,  108,   86,   87,    0,    0,  103,  107,  101,
  361,  104,  105,    0,  106,  108,    0,    0,    0,    0,
  103,    0,  168,    0,  104,  105,    0,  106,    0,    0,
    0,    0,  107,    0,    0,  147,   83,   84,   85,    0,
   86,   87,    0,  107,    0,    0,   88,   89,    0,    0,
  147,   83,   84,   85,  108,   86,   87,    0,    0,  103,
   60,  170,  379,  104,  105,    0,  106,    0,    0,  108,
  107,    0,    0,    0,  103,    0,  101,    0,  104,  105,
    0,  106,    0,   88,   89,    0,    0,  147,   83,   84,
   85,    0,   86,   87,    0,  107,    0,    0,   88,   89,
    0,    0,    0,    0,  147,   83,   84,   85,  107,   86,
   87,    0,  147,   83,   84,   85,    0,   86,   87,  127,
  127,  127,    0,  127,    0,    0,    0,   87,    0,    0,
    0,    0,    0,    0,    0,   88,   89,    0,  127,  147,
   83,   84,   85,    0,   86,   87,    0,  107,  359,    0,
    0,    0,   88,   89,    0,    0,    0,    0,    0,    0,
   88,   89,  107,  147,   83,   84,   85,    0,   86,   87,
  127,    0,    0,    0,  147,   83,   84,   85,    0,   86,
   87,    0,    0,    0,    0,    0,    0,   88,   89,    0,
    0,    0,    0,    0,    0,  115,  115,  115,    0,  115,
    0,  147,   83,   84,   85,    0,   86,   87,   64,    0,
    0,   88,   89,    0,  115,    0,    0,    0,    0,    0,
    0,    0,   88,   89,    0,    0,  147,   83,   84,   85,
  143,   86,   87,    0,    0,    0,    0,    0,    0,  147,
   83,   84,   85,   64,   86,   87,  115,    0,    0,   88,
   89,  248,  320,   44,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  190,    0,    0,    0,    0,
    0,    0,    0,    0,   88,   89,    0,    0,  147,   83,
   84,   85,    0,   86,   87,    0,    0,   88,   89,  282,
  320,   44,    0,  147,   83,   84,   85,    1,   86,   87,
    0,    0,  249,    0,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,   64,   88,   89,    0,  143,
    0,    0,  190,    0,  190,    0,  127,    0,  155,    0,
  249,   88,   89,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  190,  190,    0,    0,    0,    0,    0,
    0,    0,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,  127,    0,    0,  127,  127,  127,  127,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,  115,    0,   23,   24,   25,   26,    0,    0,
    0,    0,  143,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
    0,    0,  115,  115,  115,  115,    0,    0,   42,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,    0,    0,   23,   24,
   25,   26,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,  154,    0,   23,   24,   25,   26,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,    0,    0,   23,   24,   25,   26,    1,    1,
    0,    1,    0,    1,    1,    1,    1,    1,    1,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    1,    1,    1,    1,   19,   19,    0,    0,   19,
   19,   19,   19,   19,    0,   19,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   19,   19,   19,   19,
   19,   19,    1,   26,   26,    1,    0,   26,   26,   26,
   26,   26,    0,   26,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   26,   26,   26,   26,   26,   26,
    0,   19,   19,    0,    0,    1,    0,    0,    0,    0,
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
   39,    0,    0,   39,    0,   39,   39,   39,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   37,   37,
   39,   39,   39,    0,   39,   39,    0,   38,   38,    0,
    0,    0,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    0,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    0,   39,   39,    0,    0,    0,
    0,    0,   19,   19,   19,   19,   19,   19,    0,   19,
   19,   19,   19,   19,   19,   19,   19,   19,   19,   19,
   19,    0,    0,    0,    0,    0,   39,   39,    0,    0,
   26,   26,   26,   26,   26,   26,    0,   26,   26,   26,
   26,   26,   26,   26,   26,   26,   26,   26,   26,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   40,    0,    0,   40,    0,    0,   40,    0,    0,
   31,   31,   31,   31,   31,   31,    0,   31,   31,    0,
    0,   40,   40,   40,    0,   40,   40,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   37,   37,
   37,   37,   37,   37,    0,   37,   37,   38,   38,   38,
   38,   38,   38,    0,   38,   38,   40,   40,    0,    0,
   41,    0,    0,   41,    0,    0,   41,    0,    0,   42,
    0,    0,   42,    0,    0,   42,    0,    0,    0,    0,
   41,   41,   41,    0,   41,   41,    0,   40,   40,   42,
   42,   42,    0,   42,   42,    0,    0,   43,    0,    0,
   43,    0,    0,   43,    0,    0,   39,   39,   39,   39,
   39,   39,    0,   39,   39,   41,   41,   43,   43,   43,
   46,   43,   43,   46,   42,   42,   46,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   46,   46,   46,    0,   46,   46,   41,   41,    0,    0,
    0,    0,   43,   43,    0,   42,   42,   47,    0,    0,
   47,    0,    0,   47,    0,   44,    0,    0,   44,    0,
    0,   44,    0,    0,    0,   46,   46,   47,   47,   47,
    0,   47,   47,   43,   43,   44,   44,   44,    0,   44,
   44,    0,    0,    0,   45,    0,    0,   45,    0,    0,
   45,    0,    0,    0,    0,    0,   46,   46,    0,    0,
    0,    0,   47,   47,   45,   45,   45,    0,   45,   45,
   44,   44,    0,   56,    0,    0,   56,   40,   40,   40,
   40,   40,   40,    0,   40,   40,    0,    0,    0,    0,
   56,   56,    0,   47,   47,   56,    0,    0,    0,   45,
   45,   44,   44,   50,    0,    0,   50,    0,    0,   50,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   57,    0,   50,   50,   56,    0,    0,   50,    0,
   45,   45,    0,    0,    0,    0,   41,   41,   41,   41,
   41,   41,    0,   41,   41,   42,   42,   42,   42,   42,
   42,    0,   42,   42,    0,    0,   56,   56,   50,   50,
    0,    0,    0,    0,    0,    0,   52,    0,    0,   52,
    0,    0,   52,    0,    0,   43,   43,   43,   43,    0,
   43,   43,    0,    0,    0,    0,   52,   52,    0,   50,
   50,   52,    0,    0,    0,    0,    0,    0,   46,   46,
   46,   46,    0,   46,   46,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   52,   52,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   47,   47,   47,   47,    0,
   47,   47,    0,   44,   44,   44,   44,    0,   44,   44,
    0,    0,   52,   52,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   45,   45,   45,   45,    0,   45,   45,    0,
    0,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   56,   56,   23,   24,   25,   26,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   50,   50,    0,   50,   50,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,  330,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,    0,    0,    0,    0,   52,
   52,
  };
  protected static readonly short [] yyCheck = {            70,
    0,   70,   90,   78,   44,   40,   40,   40,   33,   95,
   79,   46,   40,   38,   42,   40,   42,   42,   43,   49,
   45,   46,   63,  125,   40,   41,   66,   27,   44,  123,
   30,   31,   32,   33,  125,   41,  257,   60,   44,   62,
  139,   79,   40,  123,   42,   29,  103,  104,  316,  141,
   44,  139,   58,   59,  270,   44,   91,   63,   91,  116,
   40,   40,  123,   42,   33,   59,   91,   41,  181,   38,
   44,   40,  123,   42,   43,   91,   45,   46,   78,  148,
   59,  167,   38,   67,   44,   41,   44,   93,   44,   73,
  176,   41,   44,   91,   44,   40,  165,   40,  123,   42,
  125,  126,   58,   59,   93,   33,  163,   63,   58,   59,
   38,   91,   40,   63,   42,   43,  156,   45,   41,  125,
  388,   44,   91,  161,  101,  194,   44,   44,   40,   41,
  199,   59,   44,   44,   61,   58,   59,   93,   94,   41,
   63,   58,  241,   93,  213,  237,   91,   41,   91,   40,
   44,  191,  257,  241,  123,  257,  125,  126,  215,  216,
  217,  274,  275,  257,  251,  125,  257,  125,  124,  125,
   93,   94,  156,  125,  245,  125,  245,  257,   33,   91,
  249,  156,  268,   38,   43,   40,   45,   42,   43,  258,
   45,  168,  261,  170,   46,  123,  283,  125,  126,   61,
   91,  124,  125,  168,   59,  170,  263,  125,  248,   61,
   44,   58,  189,  190,  125,   44,  191,   58,   40,   41,
   42,  251,  279,  257,  257,   59,  261,  262,  263,  257,
   59,  272,  257,  258,  259,  260,   40,  262,  263,   91,
  290,  257,  282,  266,  267,  316,   40,  316,  248,  299,
  300,   33,  327,  339,  340,  341,   38,  257,   40,  328,
   42,   43,  350,   45,  290,  271,  272,  324,  123,   91,
  125,  126,   41,  299,  300,   44,  345,   59,  257,  348,
  305,  306,  282,  354,  370,  354,  372,   36,  257,  258,
  259,  260,   59,  262,  263,  381,   44,   40,  384,   48,
  386,   40,   41,   42,   40,  362,   41,   56,  257,   44,
   38,   59,  268,  269,  257,  271,  272,  388,   37,  388,
   94,  271,  272,   42,  264,  265,   75,  327,   47,  257,
  258,  259,  260,   59,  262,  263,  305,  306,  124,   41,
  271,  123,   44,  125,  126,  257,   61,   41,  271,  272,
   44,   41,   91,   41,   44,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,   44,  312,  313,  314,   93,  316,  317,
  318,  319,  320,  321,  322,  323,  222,  223,  224,  225,
  268,  269,  257,  258,  259,  260,   41,  262,  263,   44,
   58,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,   41,   41,   41,   44,   44,   44,  317,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,   59,  312,  313,  314,
   41,  316,  317,  318,  319,  320,  321,  322,  323,   41,
  257,   41,   44,   35,   44,  257,  258,  259,  260,   41,
  262,  263,   44,   33,  218,  219,  226,  227,   38,  257,
   40,  257,   42,   43,  257,   45,   93,   59,   41,   61,
   41,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,   35,
  312,  313,  314,   40,  316,  317,  318,  319,  320,  321,
  322,  323,   33,  220,  221,   41,   93,   38,   44,   40,
  123,   42,   43,   41,   45,   61,  189,  190,   41,   93,
   93,  123,   58,   59,   93,   41,   93,   63,   59,  100,
  101,  315,   59,  123,   41,   38,  126,   41,   41,   41,
  345,   44,  228,  230,  229,   35,   41,  231,   67,   44,
   40,   41,  232,   59,   44,   58,   59,   93,   94,  257,
   63,  136,   41,   58,   59,   44,   27,  246,   63,   59,
   -1,   61,   -1,   -1,   -1,   -1,   48,  123,   -1,   58,
   59,   -1,   -1,   -1,   -1,   57,   -1,   35,  124,  125,
   93,   94,  123,   41,   -1,  126,   44,  168,   93,  170,
   -1,   91,   -1,   75,   76,   -1,  177,  178,  179,   -1,
   -1,   59,   -1,   61,   93,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,  200,
  125,   -1,   -1,  123,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   38,  125,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  233,   -1,  136,   -1,   59,  257,  258,  259,
  260,   -1,  262,  263,   -1,  123,   -1,   -1,   -1,   -1,
   -1,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,   -1,   -1,  307,  308,  309,  310,  181,
   -1,   -1,   -1,   -1,   -1,  305,  306,   -1,   -1,   -1,
   -1,   -1,  324,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,  126,   -1,  271,  272,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,  268,  269,   -1,  271,  272,
   -1,  342,  343,  344,  305,  306,   -1,  272,  324,   -1,
   -1,  312,  313,  314,   -1,  316,  317,  318,  319,  320,
  321,  322,  323,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,   -1,   -1,  307,  308,  309,
  310,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  324,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,   -1,   -1,  307,
  308,  309,  310,   -1,  257,  258,  259,  260,   33,  262,
  263,   -1,   -1,   38,   -1,   40,  324,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,   85,   86,
   87,   -1,   -1,   90,   -1,   -1,   -1,   -1,   -1,   40,
   41,   42,   -1,   44,   33,   -1,  103,  104,   -1,   38,
   -1,   40,   -1,   42,   43,   33,   45,   -1,   59,  116,
   38,   -1,   40,   -1,   42,   43,   38,   45,   -1,   41,
   -1,  126,   44,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,   -1,  139,   -1,   -1,   -1,   58,   59,   -1,   -1,
   91,   63,   -1,   59,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   93,   -1,  163,   -1,   -1,   -1,
   -1,   -1,   58,   59,   -1,   93,   -1,   63,   -1,   33,
   -1,   93,   94,   -1,   38,   91,   40,   -1,   42,   43,
   -1,   45,   46,   33,   -1,   -1,   -1,  126,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   93,  126,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,  215,  216,
  217,  218,  219,  220,  221,  222,  223,  224,  225,  226,
  227,  228,  229,  230,  231,  232,   -1,   91,  124,  125,
   -1,   -1,   -1,   -1,  241,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,  257,  258,  259,  260,  263,  262,  263,  123,
   -1,   59,  126,   -1,   -1,   40,   41,   42,   -1,   44,
   -1,   -1,  279,   -1,   -1,   -1,  126,   -1,  283,   -1,
   -1,   -1,   -1,   -1,   59,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  257,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   91,  324,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,  126,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,
   -1,  257,   -1,  350,   -1,   -1,   -1,  286,   -1,  271,
  272,  290,   -1,   -1,   -1,  362,   -1,   -1,  286,   -1,
  299,  300,  290,   -1,   -1,   -1,  305,  306,   -1,   -1,
   -1,  299,  300,   -1,   33,  271,  272,  305,  306,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   33,  262,  263,   -1,   -1,   38,   -1,   40,   41,
   42,   43,   -1,   45,   -1,   -1,   33,   -1,   -1,   41,
   -1,   38,   44,   40,   93,   42,   43,   -1,   45,   -1,
  290,  305,  306,   -1,   -1,   -1,   58,   59,   -1,  299,
  300,   63,   -1,   -1,   -1,  305,  306,   -1,   -1,  257,
  258,  259,  260,   33,  262,  263,   -1,  126,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   33,   93,  257,   -1,   -1,   38,   93,   40,   33,   42,
   43,   -1,   45,   38,   -1,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,  126,   -1,   59,  305,  306,   -1,
   40,   41,   42,  125,   44,   33,   -1,   -1,   -1,  126,
   38,   -1,   40,   93,   42,   43,   -1,   45,   -1,   59,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   41,   42,   43,
   33,   45,   -1,   -1,   -1,   38,  126,   40,   -1,   42,
   43,   91,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  126,   -1,   93,   -1,   33,  123,   -1,
   -1,  126,   38,   -1,   40,   41,   42,   43,   -1,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,   33,  262,  263,   -1,   -1,   38,  126,   40,
   93,   42,   43,   -1,   45,   33,   -1,   -1,   -1,   -1,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,  126,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,  126,   -1,   -1,  305,  306,   -1,   -1,
  257,  258,  259,  260,   33,  262,  263,   -1,   -1,   38,
  272,   40,   93,   42,   43,   -1,   45,   -1,   -1,   33,
  126,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   -1,  305,  306,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  126,   -1,   -1,  305,  306,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,  126,  262,
  263,   -1,  257,  258,  259,  260,   -1,  262,  263,   40,
   41,   42,   -1,   44,   -1,   -1,   -1,  257,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  305,  306,   -1,   59,  257,
  258,  259,  260,   -1,  262,  263,   -1,  126,   41,   -1,
   -1,   -1,  305,  306,   -1,   -1,   -1,   -1,   -1,   -1,
  305,  306,  126,  257,  258,  259,  260,   -1,  262,  263,
   91,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,   -1,   -1,  305,  306,   -1,
   -1,   -1,   -1,   -1,   -1,   40,   41,   42,   -1,   44,
   -1,  257,  258,  259,  260,   -1,  262,  263,   44,   -1,
   -1,  305,  306,   -1,   59,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  305,  306,   -1,   -1,  257,  258,  259,  260,
   66,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   79,  262,  263,   91,   -1,   -1,  305,
  306,   40,   41,   42,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  101,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  305,  306,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,  305,  306,   40,
   41,   42,   -1,  257,  258,  259,  260,  283,  262,  263,
   -1,   -1,   91,   -1,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,  161,  305,  306,   -1,  165,
   -1,   -1,  168,   -1,  170,   -1,  257,   -1,   41,   -1,
   91,  305,  306,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  189,  190,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,   -1,   -1,  307,  308,  309,  310,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  257,   -1,  307,  308,  309,  310,   -1,   -1,
   -1,   -1,  258,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
   -1,   -1,  307,  308,  309,  310,   -1,   -1,  257,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,   -1,   -1,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  257,   -1,  307,  308,  309,  310,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,   -1,   -1,  307,  308,  309,  310,   37,   38,
   -1,   40,   -1,   42,   43,   44,   45,   46,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   63,   37,   38,   -1,   -1,   41,
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
   38,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
   58,   59,   60,   -1,   62,   63,   -1,  124,  125,   -1,
   -1,   -1,  261,  262,  263,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,  273,  274,  275,  276,  277,  278,
  279,  280,  281,  282,   -1,   93,   94,   -1,   -1,   -1,
   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  282,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   58,   59,   60,   -1,   62,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   93,   94,   -1,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   58,   59,   60,   -1,   62,   63,   -1,  124,  125,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,   93,   94,   58,   59,   60,
   38,   62,   63,   41,   93,   94,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   -1,   62,   63,  124,  125,   -1,   -1,
   -1,   -1,   93,   94,   -1,  124,  125,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   -1,   93,   94,   58,   59,   60,
   -1,   62,   63,  124,  125,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   93,   94,   58,   59,   60,   -1,   62,   63,
   93,   94,   -1,   41,   -1,   -1,   44,  264,  265,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,
   58,   59,   -1,  124,  125,   63,   -1,   -1,   -1,   93,
   94,  124,  125,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  123,   -1,   58,   59,   93,   -1,   -1,   63,   -1,
  124,  125,   -1,   -1,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,  124,  125,   93,   94,
   -1,   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,  266,  267,  268,  269,   -1,
  271,  272,   -1,   -1,   -1,   -1,   58,   59,   -1,  124,
  125,   63,   -1,   -1,   -1,   -1,   -1,   -1,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  266,  267,  268,  269,   -1,
  271,  272,   -1,  266,  267,  268,  269,   -1,  271,  272,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  271,  272,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  268,  269,   -1,  271,  272,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
   -1,   -1,  307,  308,  309,  310,  311,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,   -1,   -1,   -1,   -1,  271,
  272,
  };

#line 995 "CParser.jay"

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
