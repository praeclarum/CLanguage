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
//t    "declaration_list : declaration_list declaration",
//t  };
//t public static string getRule (int index) {
//t    return yyRule [index];
//t }
//t}
  protected static readonly string [] yyNames = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"'!'",null,null,null,"'%'","'&'",
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
    "FOR","GOTO","CONTINUE","BREAK","RETURN",
  };

  /** index-checked interface to yyNames[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
//t  public static string yyname (int token) {
//t    if ((token < 0) || (token > yyNames.Length)) return "[illegal]";
//t    string name;
//t    if ((name = yyNames[token]) != null) return name;
//t    return "[unknown]";
//t  }

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
                    && (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == Token.yyErrorCode) {
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
#line 40 "CParser.jay"
  { yyVal = new VariableExpression((yyVals[0+yyTop]).ToString()); }
  break;
case 2:
#line 41 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 42 "CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 43 "CParser.jay"
  { yyVal = ConstantExpression.True; }
  break;
case 5:
#line 44 "CParser.jay"
  { yyVal = ConstantExpression.False; }
  break;
case 6:
#line 45 "CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 7:
#line 52 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 8:
#line 56 "CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 60 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 10:
#line 64 "CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 11:
#line 68 "CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 12:
#line 72 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
	}
  break;
case 13:
#line 76 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 14:
#line 80 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 15:
#line 84 "CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 16:
#line 88 "CParser.jay"
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
#line 110 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 20:
#line 114 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 21:
#line 118 "CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 22:
#line 122 "CParser.jay"
  {
		yyVal = new AddressOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 126 "CParser.jay"
  {
        yyVal = new DereferenceExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 24:
#line 130 "CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 25:
#line 134 "CParser.jay"
  {
		yyVal = new SizeOfExpression((Expression)yyVals[0+yyTop]);
	}
  break;
case 26:
#line 138 "CParser.jay"
  {
		yyVal = new SizeOfTypeExpression((TypeName)yyVals[-1+yyTop]);
	}
  break;
case 27:
#line 142 "CParser.jay"
  { yyVal = Unop.None; }
  break;
case 28:
#line 143 "CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 29:
#line 144 "CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 30:
#line 145 "CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 31:
#line 152 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 156 "CParser.jay"
  {
		yyVal = new CastExpression ((TypeName)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 163 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 34:
#line 167 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 35:
#line 171 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 175 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 38:
#line 183 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 187 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 41:
#line 195 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 199 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 207 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 211 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 46:
#line 215 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 219 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 49:
#line 227 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 231 "CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 239 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 247 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 255 "CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 263 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 60:
#line 271 "CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 61:
#line 278 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 62:
#line 282 "CParser.jay"
  {
		yyVal = new ConditionalExpression ((Expression)yyVals[-4+yyTop], (Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 64:
  case_64();
  break;
case 65:
#line 300 "CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 66:
#line 301 "CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 67:
#line 302 "CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 68:
#line 303 "CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 69:
#line 304 "CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 70:
#line 305 "CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 71:
#line 306 "CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 72:
#line 307 "CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 73:
#line 308 "CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 74:
#line 309 "CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 75:
#line 310 "CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 76:
#line 317 "CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 77:
#line 321 "CParser.jay"
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
#line 427 "CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 94:
#line 428 "CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 95:
#line 429 "CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 96:
#line 430 "CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 97:
#line 431 "CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 98:
#line 435 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 99:
#line 436 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 100:
#line 437 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 101:
#line 438 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 102:
#line 439 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 103:
#line 440 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 104:
#line 441 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 105:
#line 442 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 106:
#line 443 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 107:
#line 444 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 108:
#line 445 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 109:
#line 446 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 112:
#line 449 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 113:
#line 453 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-2+yyTop], (yyVals[-1+yyTop]).ToString(), (Block)yyVals[0+yyTop]); }
  break;
case 114:
#line 454 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], "", (Block)yyVals[0+yyTop]); }
  break;
case 115:
#line 455 "CParser.jay"
  { yyVal = new TypeSpecifier((TypeSpecifierKind)yyVals[-1+yyTop], (yyVals[0+yyTop]).ToString()); }
  break;
case 116:
#line 459 "CParser.jay"
  { yyVal = TypeSpecifierKind.Struct; }
  break;
case 117:
#line 460 "CParser.jay"
  { yyVal = TypeSpecifierKind.Class; }
  break;
case 118:
#line 461 "CParser.jay"
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
#line 490 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 124:
#line 491 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString()); }
  break;
case 125:
#line 492 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, ""); }
  break;
case 126:
#line 493 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString()); }
  break;
case 127:
#line 494 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 132:
#line 508 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 133:
#line 515 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 134:
#line 516 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 135:
#line 521 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 136:
#line 522 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 138:
  case_138();
  break;
case 139:
#line 541 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 140:
#line 545 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 141:
#line 549 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 142:
#line 553 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 143:
#line 557 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 144:
#line 561 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 145:
#line 565 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 146:
#line 569 "CParser.jay"
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
#line 597 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 151:
#line 598 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 152:
#line 599 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 153:
#line 600 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 154:
#line 604 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 155:
#line 608 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 156:
#line 612 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 157:
#line 613 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 158:
#line 614 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 159:
#line 621 "CParser.jay"
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
#line 682 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 169:
#line 686 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 173:
  case_173();
  break;
case 174:
#line 711 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 175:
#line 715 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 176:
#line 719 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 177:
#line 723 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 178:
#line 727 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 179:
#line 731 "CParser.jay"
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
#line 765 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 185:
#line 769 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 186:
#line 773 "CParser.jay"
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
#line 813 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 205:
#line 845 "CParser.jay"
  {
		yyVal = new Block (GetLocation(yyVals[-1+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 206:
#line 849 "CParser.jay"
  {
        yyVal = new Block (GetLocation(yyVals[-2+yyTop]), (List<Statement>)yyVals[-1+yyTop], GetLocation(yyVals[0+yyTop]));
	}
  break;
case 207:
#line 853 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 208:
#line 854 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 211:
#line 866 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 212:
#line 870 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 213:
#line 877 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 214:
#line 881 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 216:
#line 889 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 217:
#line 893 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 218:
#line 897 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock (), null, GetLocation(yyVals[-5+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 219:
#line 901 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock (), null, GetLocation(yyVals[-6+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 220:
#line 905 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 221:
#line 909 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 225:
#line 919 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 226:
#line 923 "CParser.jay"
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
#line 93 "CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_18()
#line 99 "CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_64()
#line 288 "CParser.jay"
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
#line 330 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_80()
#line 336 "CParser.jay"
{
		var d = new MultiDeclaratorStatement ();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		d.InitDeclarators = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_81()
#line 346 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_82()
#line 352 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_83()
#line 358 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_84()
#line 364 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_85()
#line 370 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_86()
#line 376 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_87()
#line 382 "CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_88()
#line 388 "CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_89()
#line 397 "CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_90()
#line 403 "CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_91()
#line 412 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[0+yyTop];
		yyVal = id;
	}

void case_92()
#line 418 "CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[-2+yyTop];
		id.Initializer = (Initializer)yyVals[0+yyTop];
		yyVal = id;
	}

void case_119()
#line 466 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeSpecifiers.Add ((TypeSpecifier)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_120()
#line 471 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeSpecifiers.Add ((TypeSpecifier)yyVals[0+yyTop]);
        yyVal = list;
    }

void case_121()
#line 477 "CParser.jay"
{
        ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers = ((DeclarationSpecifiers)yyVals[0+yyTop]).TypeQualifiers | ((TypeQualifiers)yyVals[-1+yyTop]);
        yyVal = yyVals[0+yyTop];
    }

void case_122()
#line 482 "CParser.jay"
{
        var list = new DeclarationSpecifiers ();
        list.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
        yyVal = list;
    }

void case_138()
#line 527 "CParser.jay"
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
#line 571 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_148()
#line 578 "CParser.jay"
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
#line 588 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_160()
#line 623 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_161()
#line 632 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_162()
#line 638 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_163()
#line 647 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_164()
#line 652 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_165()
#line 657 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_166()
#line 665 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_167()
#line 671 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_173()
#line 697 "CParser.jay"
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
#line 733 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-1+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_181()
#line 740 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_182()
#line 746 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration> ();
		yyVal = d;
	}

void case_183()
#line 753 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_187()
#line 778 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_188()
#line 785 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_189()
#line 793 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_190()
#line 800 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_227()
#line 928 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_228()
#line 933 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_231()
#line 946 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_232()
#line 955 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_233()
#line 966 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_234()
#line 972 "CParser.jay"
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
   63,   63,   64,   64,
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
    4,    3,    1,    2,
  };
   static readonly short [] yyDefRed = {            0,
  112,   93,   94,   95,   96,   97,  132,  157,   99,  100,
  101,  102,  105,  106,  103,  104,  156,  158,   98,  107,
  108,  109,  116,  117,  118,    0,    0,  230,    0,    0,
    0,    0,    0,  110,  111,    0,  227,  229,    0,    0,
  228,  135,    0,    0,   79,    0,   89,    0,    0,    0,
    0,   82,   84,   86,   88,    0,    0,  114,    0,    0,
    0,  128,    0,  154,  152,    0,    0,   80,    0,  233,
    0,  232,    0,    0,    0,    0,    0,  113,    0,    2,
    3,    0,    0,    0,    4,    5,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  205,    0,
    0,   27,   28,   29,   30,  211,    7,    0,    0,   76,
    0,   33,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   63,  209,  197,  210,  196,  198,  199,
  200,  201,    0,  207,    0,    0,  123,    0,  138,  155,
  153,   90,    0,    1,    0,  184,   92,  234,  231,  166,
  149,    0,    0,    0,    0,  161,    0,  146,    0,    0,
    0,  136,    0,    0,   25,    0,   20,   21,   31,   78,
    0,    0,    0,    0,    0,    0,    0,    0,  223,  224,
  225,    0,    0,    0,    0,    0,    0,   22,   23,    0,
  212,    0,   13,   14,    0,    0,    0,   66,   67,   68,
   69,   70,   71,   72,   73,   74,   75,   65,    0,   24,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  206,
  208,  124,    0,  131,  125,  129,    0,    0,    0,  187,
    0,    0,  192,    0,    0,  163,    0,  164,    0,  147,
  148,    0,    0,    0,  145,  141,    0,  140,    0,    0,
  202,    0,    0,    0,  204,    0,    0,    0,    0,    0,
    0,  222,  226,    6,    0,  119,  121,    0,    0,  169,
   77,   12,    9,    0,   17,    0,   11,   64,   34,   35,
   36,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  126,    0,  195,
  185,    0,  188,  191,  193,  180,    0,    0,  174,    0,
    0,    0,    0,    0,  167,  160,  162,    0,    0,  144,
  139,    0,    0,  203,    0,    0,    0,    0,    0,    0,
    0,   32,   10,    0,    8,    0,  194,  186,  189,    0,
  181,  173,  178,  175,  182,    0,  176,    0,    0,  142,
  143,    0,  215,  216,    0,    0,    0,    0,    0,    0,
   18,   62,  190,  183,  179,  177,    0,    0,  220,    0,
  218,    0,   15,    0,  214,  217,  221,  219,   16,
  };
  protected static readonly short [] yyDgoto  = {            27,
  107,  108,  109,  284,  184,  239,  110,  111,  112,  113,
  114,  115,  116,  117,  118,  119,  120,  121,  122,  123,
  124,  209,  171,   28,   71,   46,   30,   31,   32,   33,
   47,   63,  240,   34,   35,   36,  126,  187,   61,   62,
   49,   50,   51,   66,  317,  154,  155,  156,  318,  249,
  241,  242,  243,  127,  128,  129,  130,  131,  132,  133,
  134,   37,   38,   73,
  };
  protected static readonly short [] yySindex = {         2583,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -93, 2583,    0,   66, 2583,
 2583, 2583, 2583,    0,    0,  -88,    0,    0,  -73, -150,
    0,    0,   13,  -37,    0,   75,    0, 1651,  -19,    3,
 -129,    0,    0,    0,    0,   32,   42,    0, -150,  115,
    2,    0,  173,    0,    0,  -37,   13,    0,  712,    0,
   66,    0, 2507,    3, 1586,  414,  -24,    0,  129,    0,
    0, 1235, 1259, 1259,    0,    0, 1267,  135,  182,  224,
  228,  252,  240,   36,  257,  294,  839,  339,    0, 1267,
 1267,    0,    0,    0,    0,    0,    0,   89,  -22,    0,
  377,    0, 1267,  159,   85, -203,  -35,   53,  319,  282,
  256,  112,  -57,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  111,    0,   20, 1267,    0,  -96,    0,    0,
    0,    0,  332,    0,  804,    0,    0,    0,    0,    0,
    0,   25,  381,  156,  400,    0,  -92,    0,  871,  356,
  570,    0,  252,  339,    0,  339,    0,    0,    0,    0,
  397,  252, 1267, 1267, 1267,  147,  480,  403,    0,    0,
    0,  145,  186,  453, 2616, 2616,   26,    0,    0, 1267,
    0,  241,    0,    0,  879, 1267,  251,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1267,    0,
 1267, 1267, 1267, 1267, 1267, 1267, 1267, 1267, 1267, 1267,
 1267, 1267, 1267, 1267, 1267, 1267, 1267, 1267, 1267,    0,
    0,    0,  -86,    0,    0,    0, 1267,  260,   33,    0,
  712,  120,    0, 1441,  893,    0,   30,    0,   71,    0,
    0,  264, 2542,  688,    0,    0, 1267,    0,  958,  431,
    0,  485,  491,  252,    0,  245,  247,  273,  497,  966,
  966,    0,    0,    0,  854,    0,    0, 1469,   82,    0,
    0,    0,    0,  348,    0,   45,    0,    0,    0,    0,
    0,  159,  159,   85,   85, -203, -203, -203, -203,  -35,
  -35,   53,  319,  282,  256,  112,   92,    0,  458,    0,
    0,   14,    0,    0,    0,    0,  513,  515,    0,  989,
  474,   71, 1614, 1129,    0,    0,    0,  486,  488,    0,
    0,  460,  460,    0,  252,  252,  252, 1267, 1148, 1162,
  804,    0,    0, 1267,    0, 1267,    0,    0,    0,  712,
    0,    0,    0,    0,    0,  543,    0, 1198,  493,    0,
    0,  272,    0,    0,  395,  252,  407,  252,  409,   55,
    0,    0,    0,    0,    0,    0,  252,  529,    0,  252,
    0,  252,    0,  780,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  -33,
   56,  487,  494,    0,    0,    0,    0,    0, 1049,    0,
    0,    0,    0,  -28,    0,    0,    0,  146,    0, 1510,
 1322,    0,    0,    0,    0, 1387,    0,    0,    0,   70,
    0,    0,    0,    0,    0,  169,    0,    0,    0,    0,
    0,    0,    0, 1538,    0,    0,    0,    0, 1925,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1952,    0,
 2020,    0,    0, 2048, 2237, 2333,  -21, 2591, 1443, 2630,
  119, 1503,  501,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  146,    0,    0,    0,    0,    0,    0,    0,
    0,  417,    0,    0,  548,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  144,  178,  549,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  419,    0,  475,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  550,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2057, 2116, 2296, 2305, 2356, 2393, 2401, 2430, 2489,
 2552, 2593, 2456, 2650,  489, 2663,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  500,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 1980,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  183,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -83,    0,    0,  271,  254,  -66,  582, 1157,    0,
  152,  333,  167,  354,  368,  375,  367,  378,  379,    0,
  -85,    0,  -95,  126,    1,    0,    0,  -54, 1210,    0,
  537,    7,  -69,    0,    0,    0,  507,  376,  552,  -98,
    5,  -30,    0,  -31,  -71,    0,    0,  363,  -64, -161,
 -264,    0,  382,  -70,    0,  -68,    0,    0,    0,    0,
  476,  590,    0,    0,
  };
  protected static readonly short [] yyTable = {           147,
   29,  170,  146,  153,   44,  229,   81,   81,   81,  160,
   81,  150,  150,  182,  183,  150,   48,  195,   74,   48,
   43,  176,   48,  197,  220,   81,  221,   29,  235,   40,
   52,   53,   54,   55,   57,   48,   48,   48,  308,  236,
  234,   48,   75,  185,  161,  138,  105,  350,   65,   59,
  170,  100,   43,   98,   44,  101,  102,   81,  103,  238,
  216,  217,  150,  233,  244,  278,   44,   44,  196,  244,
  141,   48,   48,  143,  105,  152,  312,  143,  146,  100,
  183,   98,  183,  101,  102,  322,  103,  248,  190,  266,
  267,  268,  261,   76,  260,   83,   83,   83,  384,   83,
  106,  265,   48,   48,  237,   43,   60,   44,  271,  185,
  323,  185,  286,  130,   83,  245,  245,  322,   67,  350,
  245,  278,  280,  281,   45,  254,  137,  214,  285,  215,
  185,  185,  190,   68,  236,  190,  145,  345,  348,  104,
   77,  309,  288,  105,  232,  307,   83,  191,  100,  346,
   98,  170,  101,  102,   57,  103,  247,  311,  246,   57,
   60,  324,   57,   39,   57,  238,   99,  104,   56,  106,
   60,  313,  245,   70,  146,  136,   57,   57,  321,  383,
  314,   57,  125,  120,  120,  120,  163,  328,  190,   91,
  329,  279,  172,  334,  130,  213,  251,    8,  148,  252,
  211,  339,  340,  273,   91,  212,   17,   18,  151,  151,
  237,   57,  151,  139,  228,  213,   74,  122,  122,  122,
  213,  173,  213,   81,  213,  213,  274,  213,  150,  190,
  218,  219,  162,   57,  120,  230,  104,   42,  192,  193,
  194,  213,  349,   57,  152,  146,   48,   48,  247,   48,
   48,  356,    8,  152,  365,  367,  369,  359,  125,  151,
  372,   17,   18,  174,  362,  363,  364,  175,  122,   42,
  144,   80,   81,   82,  146,   83,   84,  371,  152,  177,
  373,   42,  279,  146,  105,  335,   42,  336,  190,  100,
  190,   98,  178,  101,  102,  379,  103,  381,   79,   80,
   81,   82,  270,   83,   84,  213,  385,  213,  213,  387,
  106,  388,   83,  337,  349,  179,  190,  146,   85,   86,
  222,  223,   42,  152,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   85,   86,   23,   24,
   25,   26,  180,   87,   88,   89,  224,   90,   91,   92,
   93,   94,   95,   96,   97,  292,  293,   79,   80,   81,
   82,  105,   83,   84,   57,  225,  100,  104,   98,  226,
  101,  102,  227,  103,  296,  297,  298,  299,  343,   57,
   57,  344,   69,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   85,   86,   23,   24,   25,
   26,  250,   87,   88,   89,  151,   90,   91,   92,   93,
   94,   95,   96,   97,  262,  378,  263,  208,  190,  213,
  213,  213,  213,  253,  213,  213,  105,  380,  256,  382,
  190,  100,  190,   98,  264,  159,  102,  165,  103,  170,
  165,  272,  170,  269,  104,  213,  213,  213,  213,  213,
  213,  213,  213,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  275,  213,  213,  213,  282,  213,  213,
  213,  213,  213,  213,  213,  213,  158,  287,   79,   80,
   81,   82,  105,   83,   84,  171,  310,  100,  171,   98,
  325,  101,  102,  331,  103,  332,   85,   85,   85,   58,
   85,  333,   58,   87,   87,   87,  338,   87,  106,  104,
  172,   61,   58,  172,   61,   85,   58,   58,  294,  295,
  347,   58,   87,  351,   72,  352,   85,   86,   61,   61,
  276,  277,   78,   87,   88,   89,  354,   90,   91,   92,
   93,   94,   95,   96,   97,  300,  301,   85,  360,  149,
  361,   58,  341,  374,   87,  376,  377,  386,  159,  168,
  170,  302,  304,   61,  370,  144,   80,   81,   82,  303,
   83,   84,  105,  142,  305,  104,  306,  100,  231,   98,
  135,  259,  102,   58,  103,  327,   41,    0,    0,    0,
    0,    1,    0,  315,    0,   61,    0,    0,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   85,   86,   23,   24,   25,   26,  198,
  199,  200,  201,  202,  203,  204,  205,  206,  207,    0,
    0,    0,  258,  165,  167,  168,    0,    0,  169,    0,
  144,   80,   81,   82,    0,   83,   84,    0,    0,    0,
    0,  169,  169,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  169,  104,    0,    0,    0,  157,
    0,    0,    0,    8,    0,    0,    0,    0,    0,    0,
    0,    0,   17,   18,    0,    0,    0,  169,   85,   86,
  105,    0,    0,    0,    0,  100,    0,   98,    0,  101,
  102,    0,  103,    0,    0,    0,  144,   80,   81,   82,
  169,   83,   84,   85,  105,    0,    0,    0,    0,  100,
   87,   98,    0,  101,  102,    0,  103,    0,    0,   58,
   58,    0,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   85,   86,   23,   24,   25,   26,
    0,    0,  169,  169,  169,  169,  169,  169,  169,  169,
  169,  169,  169,  169,  169,  169,  169,  169,  169,  169,
    0,    0,  105,  104,    0,    0,    0,  100,  169,   98,
    0,  101,  102,    0,  103,  238,  144,   80,   81,   82,
    0,   83,   84,    0,  145,    0,  105,  104,    0,    0,
  169,  100,    0,   98,    0,  101,  102,    0,  103,  238,
    0,    0,    0,    0,    0,  257,  169,    0,    0,    8,
    0,    0,    0,    0,    0,    0,    0,    0,   17,   18,
  237,  105,    0,    0,   85,   86,  100,    0,   98,    0,
  101,  102,    0,  103,    0,    0,  105,    0,    0,    0,
    0,  100,    0,   98,  237,  101,  102,  181,  103,    0,
    0,  169,  145,  105,  389,  104,    0,    0,  100,    0,
   98,  105,  101,  102,    0,  103,  100,    0,   98,  283,
  101,  102,    0,  103,    0,  105,  145,  169,    0,  104,
  100,    0,   98,    0,  320,  102,    0,  103,    0,  169,
    0,    0,    0,    0,  144,   80,   81,   82,    0,   83,
   84,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  255,  104,    0,    0,    0,  144,   80,
   81,   82,    0,   83,   84,    0,  341,    8,    0,  104,
    0,    0,    0,    0,    0,  319,   17,   18,    0,    0,
  105,    0,   85,   86,    0,  100,  104,   98,  105,  101,
  102,    0,  103,  100,  104,   98,    0,  101,  102,    0,
  103,    0,    0,    0,    0,    0,   85,   86,  104,    0,
    0,  105,    0,    0,  106,    0,  100,    0,   98,    0,
  101,  102,    0,  103,    0,    0,  144,   80,   81,   82,
    0,   83,   84,    0,    0,    0,    0,    0,    0,    0,
  330,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  144,   80,   81,   82,    0,   83,   84,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  353,    0,  104,   85,   86,    0,    0,  127,  127,
  127,  104,  127,    0,    0,  144,   80,   81,   82,    0,
   83,   84,    0,    0,    0,    0,    0,  127,   85,   86,
  144,   80,   81,   82,  104,   83,   84,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  144,   80,   81,
   82,    0,   83,   84,    0,  144,   80,   81,   82,  127,
   83,   84,    0,   85,   86,    0,    0,    0,    0,  144,
   80,   81,   82,    0,   83,   84,    0,    0,   85,   86,
    0,  105,    0,    0,    0,    0,  100,    0,   98,    0,
  358,  102,    0,  103,    0,   85,   86,    0,    0,    0,
  105,    0,    0,   85,   86,  100,    0,   98,  366,  101,
  102,    0,  103,    0,  105,    0,    0,   85,   86,  100,
    0,   98,  368,  101,  102,    0,  103,    0,    0,    0,
    0,    0,    0,    0,  144,   80,   81,   82,    0,   83,
   84,  357,  144,   80,   81,   82,    0,   83,   84,    0,
  105,    0,    0,    0,    0,  100,    0,   98,    0,  101,
  102,    0,  103,    0,    0,  144,   80,   81,   82,    0,
   83,   84,    0,   64,  104,    0,  188,  189,    0,    0,
    0,    0,   85,   86,    0,    0,    0,  105,    0,  210,
   85,   86,  100,  104,  164,  140,  101,  102,    0,  103,
    0,    0,    0,    0,    0,   64,    0,  104,    0,    0,
  375,  105,    0,   85,   86,    0,  100,    0,  166,  105,
  101,  102,    0,  103,  100,  127,   98,  186,  101,  102,
    0,  103,    0,    0,    0,  189,    0,    0,    0,    0,
    0,    0,    0,  104,    0,    0,    0,    0,    0,    0,
    0,  127,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,    0,    0,  127,  127,  127,  127,    0,
  104,  137,  137,    0,    0,  137,   64,  289,  290,  291,
  140,    0,    0,  186,    0,  186,    0,    0,    0,    0,
  137,    0,  137,    0,  104,  144,   80,   81,   82,    0,
   83,   84,  104,    0,  186,  186,    0,    0,    0,    0,
    0,    0,    0,    0,  144,   80,   81,   82,    0,   83,
   84,    0,  137,    0,    0,  189,    0,    0,  144,   80,
   81,   82,    0,   83,   84,    0,  115,  115,  115,    0,
  115,  342,    0,   85,   86,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  137,  115,    0,    0,    0,    0,
    0,    0,   85,   86,  144,   80,   81,   82,    0,   83,
   84,    0,    0,  140,    0,    0,   85,   86,    0,    0,
    0,    0,    0,    0,    0,    0,  189,  115,    0,    0,
  244,  316,   44,   53,    0,    0,   53,    0,    0,    0,
    0,  144,   80,   81,   82,    0,   83,   84,    0,    0,
   53,   53,   85,   86,    0,   53,    0,    0,  278,  316,
   44,    0,    0,    0,  189,  144,   80,   81,   82,    0,
   83,   84,    0,  144,   80,   81,   82,    0,   83,   84,
    0,  245,    0,    0,    0,   53,   53,    0,    0,   85,
   86,    0,    0,   59,    0,    0,   59,    0,    0,    0,
  134,    0,    0,  134,    0,    0,    0,    0,    0,  245,
   59,   59,    0,   85,   86,   59,   53,   53,  134,    0,
  134,   85,   86,    0,    0,    0,    0,    0,  133,    0,
    0,  133,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   59,  133,    0,  133,    0,
    0,    0,    0,    0,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  151,   59,  137,  137,
  137,  137,  134,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  115,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  355,    0,    0,    0,    0,    0,
  133,    0,    0,    0,    0,    0,    0,    0,    0,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,    0,    0,  115,  115,  115,  115,   42,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   69,    0,   53,   53,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   57,   59,   23,   24,   25,   26,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,  134,    0,    0,  134,  134,  134,  134,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  150,    0,  133,  133,  133,  133,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,    0,    0,
   23,   24,   25,   26,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,    0,    0,   23,   24,   25,
   26,    1,    1,    0,    1,    0,    1,    1,    1,    1,
    1,    1,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    1,    1,    1,    1,   19,   19,
    0,    0,   19,   19,   19,   19,   19,    0,   19,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   19,
   19,   19,   19,   19,   19,    1,   26,   26,    1,    0,
   26,   26,   26,   26,   26,    0,   26,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   26,   26,   26,
   26,   26,   26,    0,   19,   19,    0,    0,    1,    0,
    0,    0,    0,    0,    0,    0,   31,   31,    0,    0,
   31,   31,   31,   31,   31,    0,   31,    0,    0,    0,
    0,    0,   26,   26,    0,   19,   19,   31,   31,   31,
    0,   31,   31,    0,    0,   37,    0,    0,   37,    0,
   37,   37,   37,    0,   38,    0,    0,   38,    0,   38,
   38,   38,    0,   26,   26,   37,   37,   37,    0,   37,
   37,    0,   31,   31,   38,   38,   38,    0,   38,   38,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   37,   37,    0,   31,   31,    0,    0,    0,    0,   38,
   38,    0,    0,   39,    0,    0,   39,    0,   39,   39,
   39,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   37,   37,   39,   39,   39,    0,   39,   39,    0,
   38,   38,    0,    0,    0,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    0,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    0,   39,   39,
    0,    0,    0,    0,    0,   19,   19,   19,   19,   19,
   19,    0,   19,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,    0,    0,    0,    0,    0,   39,
   39,    0,    0,   26,   26,   26,   26,   26,   26,    0,
   26,   26,   26,   26,   26,   26,   26,   26,   26,   26,
   26,   26,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   40,    0,    0,   40,    0,    0,
   40,    0,    0,   31,   31,   31,   31,   31,   31,    0,
   31,   31,    0,    0,   40,   40,   40,    0,   40,   40,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   37,   37,   37,   37,   37,   37,    0,   37,   37,
   38,   38,   38,   38,   38,   38,    0,   38,   38,   40,
   40,    0,    0,   41,    0,    0,   41,    0,    0,   41,
    0,    0,   42,    0,    0,   42,    0,    0,   42,    0,
    0,    0,    0,   41,   41,   41,    0,   41,   41,    0,
   40,   40,   42,   42,   42,    0,   42,   42,    0,    0,
   43,    0,    0,   43,    0,    0,   43,    0,    0,   39,
   39,   39,   39,   39,   39,    0,   39,   39,   41,   41,
   43,   43,   43,   46,   43,   43,   46,   42,   42,   46,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   46,   46,   46,    0,   46,   46,   41,
   41,    0,    0,    0,    0,   43,   43,    0,   42,   42,
   47,    0,    0,   47,    0,    0,   47,    0,   44,    0,
    0,   44,    0,    0,   44,    0,    0,    0,   46,   46,
   47,   47,   47,    0,   47,   47,   43,   43,   44,   44,
   44,    0,   44,   44,    0,    0,    0,   45,    0,    0,
   45,    0,    0,   45,    0,    0,    0,    0,    0,   46,
   46,    0,    0,    0,    0,   47,   47,   45,   45,   45,
    0,   45,   45,   44,   44,    0,   54,    0,    0,   54,
   40,   40,   40,   40,   40,   40,    0,   40,   40,    0,
    0,    0,    0,   54,   54,    0,   47,   47,   54,    0,
    0,    0,   45,   45,   44,   44,   49,    0,    0,   49,
    0,    0,   49,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   49,   49,   54,   54,
    0,   49,    0,   45,   45,    0,    0,    0,    0,   41,
   41,   41,   41,   41,   41,    0,   41,   41,   42,   42,
   42,   42,   42,   42,    0,   42,   42,    0,    0,   54,
   54,   49,   49,    0,    0,    0,    0,    0,    0,   50,
    0,    0,   50,    0,    0,   50,    0,    0,   43,   43,
   43,   43,    0,   43,   43,    0,    0,    0,    0,   50,
   50,    0,   49,   49,   50,    0,    0,    0,    0,    0,
    0,   46,   46,   46,   46,    0,   46,   46,   51,   57,
   52,   51,    0,   52,   51,    0,   52,    0,    0,    0,
    0,    0,    0,    0,   50,   50,    0,    0,   51,   51,
   52,   52,    0,   51,    0,   52,    0,    0,   47,   47,
   47,   47,    0,   47,   47,    0,   44,   44,   44,   44,
   55,   44,   44,   55,    0,   50,   50,    0,    0,    0,
    0,    0,    0,   51,   51,   52,   52,   55,   55,    0,
   56,    0,   55,   56,    0,   45,   45,   45,   45,    0,
   45,   45,    0,   60,    0,    0,   60,   56,   56,    0,
    0,    0,   56,    0,   51,   51,   52,   52,    0,    0,
   60,   60,   55,    0,    0,   60,   54,   54,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   56,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   55,   55,   60,   49,   49,    0,   49,
   49,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   56,   56,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   60,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,   50,
   50,    0,   50,   50,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,    0,    0,   23,   24,
   25,   26,  326,    0,    0,    0,    0,    0,    0,    0,
    0,   51,   51,   52,   52,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,    0,    0,   23,
   24,   25,   26,    0,    0,    0,    0,    0,    1,    0,
   55,   55,    0,    0,    0,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   56,   56,   23,   24,   25,   26,    0,    0,    0,    0,
    0,    0,    0,    0,   60,
  };
  protected static readonly short [] yyCheck = {            69,
    0,   87,   69,   75,   42,   63,   40,   41,   42,   76,
   44,   40,   41,   97,   98,   44,   38,   40,   49,   41,
   40,   92,   44,   46,   60,   59,   62,   27,  125,  123,
   30,   31,   32,   33,  123,   29,   58,   59,  125,  138,
  136,   63,   40,   98,   76,   44,   33,  312,   44,  123,
  136,   38,   40,   40,   42,   42,   43,   91,   45,   46,
  264,  265,   91,   44,   40,   40,   42,   42,   91,   40,
   66,   93,   94,   67,   33,   75,   44,   71,  145,   38,
  164,   40,  166,   42,   43,  247,   45,  152,   44,  173,
  174,  175,  163,   91,  161,   40,   41,   42,   44,   44,
   59,  172,  124,  125,   91,   40,  257,   42,  177,  164,
   40,  166,  196,   44,   59,   91,   91,  279,   44,  384,
   91,   40,  187,  190,   59,  157,  125,   43,  195,   45,
  185,  186,   44,   59,  233,   44,  123,   93,  125,  126,
  270,  237,  209,   33,  125,  229,   91,   59,   38,   58,
   40,  237,   42,   43,  123,   45,  152,  125,  152,   41,
  257,   91,   44,  257,  123,   46,  125,  126,  257,   59,
  257,  241,   91,   48,  241,   61,   58,   59,  245,  125,
   61,   63,   57,   40,   41,   42,   58,  254,   44,   44,
  257,  187,   58,  264,  125,   37,   41,  290,   73,   44,
   42,  270,  271,   59,   59,   47,  299,  300,   40,   41,
   91,   93,   44,   41,  272,   33,  247,   40,   41,   42,
   38,   40,   40,  257,   42,   43,   41,   45,  257,   44,
  266,  267,  257,  123,   91,  125,  126,  257,  261,  262,
  263,   59,  312,  125,  244,  312,  268,  269,  244,  271,
  272,  323,  290,  253,  338,  339,  340,  324,  133,   91,
  346,  299,  300,   40,  335,  336,  337,   40,   91,  257,
  257,  258,  259,  260,  341,  262,  263,  344,  278,   40,
  350,  257,  278,  350,   33,   41,  257,   41,   44,   38,
   44,   40,  257,   42,   43,  366,   45,  368,  257,  258,
  259,  260,  177,  262,  263,  123,  377,  125,  126,  380,
   59,  382,  257,   41,  384,   59,   44,  384,  305,  306,
  268,  269,  257,  323,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   59,  312,  313,  314,   38,  316,  317,  318,
  319,  320,  321,  322,  323,  214,  215,  257,  258,  259,
  260,   33,  262,  263,  123,   94,   38,  126,   40,  124,
   42,   43,  271,   45,  218,  219,  220,  221,   41,  271,
  272,   44,   61,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,   41,  312,  313,  314,  257,  316,  317,  318,  319,
  320,  321,  322,  323,  164,   41,  166,   61,   44,  257,
  258,  259,  260,   44,  262,  263,   33,   41,   93,   41,
   44,   38,   44,   40,   58,   42,   43,   41,   45,   41,
   44,   59,   44,  317,  126,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,   41,  312,  313,  314,  257,  316,  317,
  318,  319,  320,  321,  322,  323,   93,  257,  257,  258,
  259,  260,   33,  262,  263,   41,  257,   38,   44,   40,
  257,   42,   43,   93,   45,   41,   40,   41,   42,   41,
   44,   41,   44,   40,   41,   42,   40,   44,   59,  126,
   41,   41,   36,   44,   44,   59,   58,   59,  216,  217,
   93,   63,   59,   41,   48,   41,  305,  306,   58,   59,
  185,  186,   56,  312,  313,  314,   93,  316,  317,  318,
  319,  320,  321,  322,  323,  222,  223,   91,   93,   73,
   93,   93,  123,   41,   91,   93,  315,   59,   41,   41,
   41,  224,  226,   93,  341,  257,  258,  259,  260,  225,
  262,  263,   33,   67,  227,  126,  228,   38,  133,   40,
   59,   42,   43,  125,   45,  253,   27,   -1,   -1,   -1,
   -1,  283,   -1,  242,   -1,  125,   -1,   -1,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  273,
  274,  275,  276,  277,  278,  279,  280,  281,  282,   -1,
   -1,   -1,   93,   82,   83,   84,   -1,   -1,   87,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,  100,  101,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  113,  126,   -1,   -1,   -1,  286,
   -1,   -1,   -1,  290,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  299,  300,   -1,   -1,   -1,  136,  305,  306,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,  257,  258,  259,  260,
  159,  262,  263,  257,   33,   -1,   -1,   -1,   -1,   38,
  257,   40,   -1,   42,   43,   -1,   45,   -1,   -1,  271,
  272,   -1,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,  211,  212,  213,  214,  215,  216,  217,  218,
  219,  220,  221,  222,  223,  224,  225,  226,  227,  228,
   -1,   -1,   33,  126,   -1,   -1,   -1,   38,  237,   40,
   -1,   42,   43,   -1,   45,   46,  257,  258,  259,  260,
   -1,  262,  263,   -1,  123,   -1,   33,  126,   -1,   -1,
  259,   38,   -1,   40,   -1,   42,   43,   -1,   45,   46,
   -1,   -1,   -1,   -1,   -1,  286,  275,   -1,   -1,  290,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  299,  300,
   91,   33,   -1,   -1,  305,  306,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,   91,   42,   43,   59,   45,   -1,
   -1,  320,  123,   33,  125,  126,   -1,   -1,   38,   -1,
   40,   33,   42,   43,   -1,   45,   38,   -1,   40,   41,
   42,   43,   -1,   45,   -1,   33,  123,  346,   -1,  126,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,  358,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,  126,   -1,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,  123,  290,   -1,  126,
   -1,   -1,   -1,   -1,   -1,   93,  299,  300,   -1,   -1,
   33,   -1,  305,  306,   -1,   38,  126,   40,   33,   42,
   43,   -1,   45,   38,  126,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,  305,  306,  126,   -1,
   -1,   33,   -1,   -1,   59,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,  126,  305,  306,   -1,   -1,   40,   41,
   42,  126,   44,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,   59,  305,  306,
  257,  258,  259,  260,  126,  262,  263,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,  258,  259,  260,   91,
  262,  263,   -1,  305,  306,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,  305,  306,
   -1,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,  305,  306,   -1,   -1,   -1,
   33,   -1,   -1,  305,  306,   38,   -1,   40,   41,   42,
   43,   -1,   45,   -1,   33,   -1,   -1,  305,  306,   38,
   -1,   40,   41,   42,   43,   -1,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   93,  257,  258,  259,  260,   -1,  262,  263,   -1,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   44,  126,   -1,  100,  101,   -1,   -1,
   -1,   -1,  305,  306,   -1,   -1,   -1,   33,   -1,  113,
  305,  306,   38,  126,   40,   66,   42,   43,   -1,   45,
   -1,   -1,   -1,   -1,   -1,   76,   -1,  126,   -1,   -1,
   93,   33,   -1,  305,  306,   -1,   38,   -1,   40,   33,
   42,   43,   -1,   45,   38,  257,   40,   98,   42,   43,
   -1,   45,   -1,   -1,   -1,  159,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,   -1,   -1,  307,  308,  309,  310,   -1,
  126,   40,   41,   -1,   -1,   44,  157,  211,  212,  213,
  161,   -1,   -1,  164,   -1,  166,   -1,   -1,   -1,   -1,
   59,   -1,   61,   -1,  126,  257,  258,  259,  260,   -1,
  262,  263,  126,   -1,  185,  186,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   91,   -1,   -1,  259,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,   -1,   40,   41,   42,   -1,
   44,  275,   -1,  305,  306,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  123,   59,   -1,   -1,   -1,   -1,
   -1,   -1,  305,  306,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,  254,   -1,   -1,  305,  306,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  320,   91,   -1,   -1,
   40,   41,   42,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   58,   59,  305,  306,   -1,   63,   -1,   -1,   40,   41,
   42,   -1,   -1,   -1,  358,  257,  258,  259,  260,   -1,
  262,  263,   -1,  257,  258,  259,  260,   -1,  262,  263,
   -1,   91,   -1,   -1,   -1,   93,   94,   -1,   -1,  305,
  306,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   91,
   58,   59,   -1,  305,  306,   63,  124,  125,   59,   -1,
   61,  305,  306,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   59,   -1,   61,   -1,
   -1,   -1,   -1,   -1,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,   41,  125,  307,  308,
  309,  310,  123,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   41,   -1,   -1,   -1,   -1,   -1,
  123,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,  257,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   61,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,   -1,   -1,  307,  308,  309,
  310,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  123,  272,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,   -1,   -1,  307,  308,  309,  310,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  257,   -1,  307,  308,  309,  310,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
   -1,   -1,  307,  308,  309,  310,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,   -1,   -1,
  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,   -1,   -1,  307,  308,  309,
  310,   37,   38,   -1,   40,   -1,   42,   43,   44,   45,
   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   63,   37,   38,
   -1,   -1,   41,   42,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,
   59,   60,   61,   62,   63,   91,   37,   38,   94,   -1,
   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,
   61,   62,   63,   -1,   93,   94,   -1,   -1,  124,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   37,   38,   -1,   -1,
   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,  124,  125,   58,   59,   60,
   -1,   62,   63,   -1,   -1,   38,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   38,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,  124,  125,   58,   59,   60,   -1,   62,
   63,   -1,   93,   94,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   94,   -1,  124,  125,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   38,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   58,   59,   60,   -1,   62,   63,   -1,
  124,  125,   -1,   -1,   -1,  261,  262,  263,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,  273,  274,  275,
  276,  277,  278,  279,  280,  281,  282,   -1,   93,   94,
   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,
  269,   -1,  271,  272,  273,  274,  275,  276,  277,  278,
  279,  280,  281,  282,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  282,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,
  271,  272,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   93,
   94,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,   -1,
  124,  125,   58,   59,   60,   -1,   62,   63,   -1,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   93,   94,
   58,   59,   60,   38,   62,   63,   41,   93,   94,   44,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,  124,
  125,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,  125,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   93,   94,
   58,   59,   60,   -1,   62,   63,  124,  125,   58,   59,
   60,   -1,   62,   63,   -1,   -1,   -1,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,   93,   94,   58,   59,   60,
   -1,   62,   63,   93,   94,   -1,   41,   -1,   -1,   44,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   -1,
   -1,   -1,   -1,   58,   59,   -1,  124,  125,   63,   -1,
   -1,   -1,   93,   94,  124,  125,   38,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   93,   94,
   -1,   63,   -1,  124,  125,   -1,   -1,   -1,   -1,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,  264,  265,
  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,  124,
  125,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,  266,  267,
  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,   58,
   59,   -1,  124,  125,   63,   -1,   -1,   -1,   -1,   -1,
   -1,  266,  267,  268,  269,   -1,  271,  272,   38,  123,
   38,   41,   -1,   41,   44,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   58,   59,
   58,   59,   -1,   63,   -1,   63,   -1,   -1,  266,  267,
  268,  269,   -1,  271,  272,   -1,  266,  267,  268,  269,
   41,  271,  272,   44,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   94,   93,   94,   58,   59,   -1,
   41,   -1,   63,   44,   -1,  266,  267,  268,  269,   -1,
  271,  272,   -1,   41,   -1,   -1,   44,   58,   59,   -1,
   -1,   -1,   63,   -1,  124,  125,  124,  125,   -1,   -1,
   58,   59,   93,   -1,   -1,   63,  271,  272,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   93,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,   -1,   -1,  268,
  269,   -1,  271,  272,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,   -1,   -1,  307,  308,
  309,  310,  311,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  271,  272,  271,  272,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,   -1,   -1,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,  283,   -1,
  271,  272,   -1,   -1,   -1,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  271,  272,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  272,
  };

#line 982 "CParser.jay"

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
public class Token {
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
