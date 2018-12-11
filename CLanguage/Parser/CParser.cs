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
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-1+yyTop]); }
  break;
case 124:
#line 493 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-3+yyTop]).ToString(), (Block)yyVals[-1+yyTop]); }
  break;
case 125:
#line 494 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, "", (Block)yyVals[-2+yyTop]); }
  break;
case 126:
#line 495 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[-4+yyTop]).ToString(), (Block)yyVals[-2+yyTop]); }
  break;
case 127:
#line 496 "CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Enum, (yyVals[0+yyTop]).ToString()); }
  break;
case 128:
  case_128();
  break;
case 129:
  case_129();
  break;
case 130:
#line 518 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[0+yyTop]);
    }
  break;
case 131:
#line 522 "CParser.jay"
  {
        yyVal = new EnumeratorStatement ((string)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
    }
  break;
case 132:
#line 526 "CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 133:
#line 533 "CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 134:
#line 534 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 135:
#line 539 "CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 136:
#line 540 "CParser.jay"
  { yyVal = ((IdentifierDeclarator)(yyVals[-2+yyTop])).Push ((yyVals[0+yyTop]).ToString()); }
  break;
case 138:
  case_138();
  break;
case 139:
#line 559 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 140:
#line 563 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 141:
#line 567 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 142:
#line 571 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 143:
#line 575 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 144:
#line 579 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 145:
#line 583 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 146:
#line 587 "CParser.jay"
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
#line 615 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 151:
#line 616 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 152:
#line 617 "CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 153:
#line 618 "CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 154:
#line 622 "CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 155:
#line 626 "CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 156:
#line 630 "CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 157:
#line 631 "CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 158:
#line 632 "CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 159:
#line 639 "CParser.jay"
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
#line 700 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[0+yyTop], null);
    }
  break;
case 169:
#line 704 "CParser.jay"
  {
        yyVal = new TypeName ((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
    }
  break;
case 173:
  case_173();
  break;
case 174:
#line 729 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 175:
#line 733 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 176:
#line 737 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 177:
#line 741 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 178:
#line 745 "CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 179:
#line 749 "CParser.jay"
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
#line 783 "CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 185:
#line 787 "CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 186:
#line 791 "CParser.jay"
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
#line 831 "CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 205:
#line 863 "CParser.jay"
  {
		yyVal = new Block (Compiler.VariableScope.Local);
	}
  break;
case 206:
#line 867 "CParser.jay"
  {
        yyVal = new Block (Compiler.VariableScope.Local, (List<Statement>)yyVals[-1+yyTop]);
	}
  break;
case 207:
#line 871 "CParser.jay"
  { yyVal = new List<Statement> (1) { (Statement)yyVals[0+yyTop] }; }
  break;
case 208:
#line 872 "CParser.jay"
  { ((List<Statement>)yyVals[-1+yyTop]).Add ((Statement)yyVals[0+yyTop]); yyVal = yyVals[-1+yyTop]; }
  break;
case 211:
#line 884 "CParser.jay"
  {
		yyVal = null;
	}
  break;
case 212:
#line 888 "CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 213:
#line 895 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 214:
#line 899 "CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 216:
#line 907 "CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 217:
#line 911 "CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[-5+yyTop]).ToBlock ());
	}
  break;
case 218:
#line 915 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 219:
#line 919 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 220:
#line 923 "CParser.jay"
  {
		yyVal = new ForStatement((Statement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 221:
#line 927 "CParser.jay"
  {
        yyVal = new ForStatement((Statement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], ((Statement)yyVals[0+yyTop]).ToBlock ());
	}
  break;
case 225:
#line 937 "CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 226:
#line 941 "CParser.jay"
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

void case_128()
#line 501 "CParser.jay"
{
        var l = new Block (Compiler.VariableScope.Global);
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_129()
#line 507 "CParser.jay"
{
        var l = (Block)yyVals[-2+yyTop];
        l.AddStatement((Statement)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_138()
#line 545 "CParser.jay"
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
#line 589 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_148()
#line 596 "CParser.jay"
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
#line 606 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_160()
#line 641 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_161()
#line 650 "CParser.jay"
{
		var l = new List<ParameterDeclaration>();
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_162()
#line 656 "CParser.jay"
{
		var l = (List<ParameterDeclaration>)yyVals[-2+yyTop];
		l.Add((ParameterDeclaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_163()
#line 665 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_164()
#line 670 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_165()
#line 675 "CParser.jay"
{
		var p = new ParameterDeclaration((DeclarationSpecifiers)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_166()
#line 683 "CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_167()
#line 689 "CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_173()
#line 715 "CParser.jay"
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
#line 751 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-1+yyTop];
		d.Parameters = new List<ParameterDeclaration>();
		yyVal = d;
	}

void case_181()
#line 758 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_182()
#line 764 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDeclaration> ();
		yyVal = d;
	}

void case_183()
#line 771 "CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDeclaration>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_187()
#line 796 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_188()
#line 803 "CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_189()
#line 811 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_190()
#line 818 "CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_227()
#line 946 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_228()
#line 951 "CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_231()
#line 964 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_232()
#line 973 "CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_233()
#line 984 "CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_234()
#line 990 "CParser.jay"
{
        var l = new List<Declaration>();
        l.Add((Declaration)yyVals[0+yyTop]);
        yyVal = l;
    }

void case_235()
#line 996 "CParser.jay"
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
   63,   63,   64,   64,   64,   65,   65,   65,
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
    4,    3,    1,    2,    2,    1,    1,    1,
  };
   static readonly short [] yyDefRed = {            0,
  112,   93,   94,   95,   96,   97,  132,  157,   99,  100,
  101,  102,  105,  106,  103,  104,  156,  158,   98,  107,
  108,  109,  116,  117,  118,    0,    0,  230,    0,    0,
    0,    0,    0,  110,  111,    0,  227,  229,    0,    0,
  228,  135,    0,    0,   79,    0,   89,    0,    0,    0,
    0,   82,   84,   86,   88,    0,    0,  114,    0,    0,
    0,  128,    0,  154,  152,    0,    0,   80,  236,    0,
  237,  238,  233,    0,  232,    0,    0,    0,    0,    0,
    0,  113,    0,    2,    3,    0,    0,    0,    4,    5,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  205,    0,    0,   27,   28,   29,   30,  211,
    7,    0,    0,   76,    0,   33,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   63,  209,  197,
  210,  196,  198,  199,  200,  201,    0,  207,    0,    0,
  123,    0,  138,  155,  153,   90,    0,    1,    0,  184,
   92,  235,  231,  234,  166,  149,    0,    0,    0,    0,
  161,    0,  146,    0,    0,    0,  136,    0,    0,   25,
    0,   20,   21,   31,   78,    0,    0,    0,    0,    0,
    0,    0,    0,  223,  224,  225,    0,    0,    0,    0,
    0,    0,   22,   23,    0,  212,    0,   13,   14,    0,
    0,    0,   66,   67,   68,   69,   70,   71,   72,   73,
   74,   75,   65,    0,   24,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  206,  208,  124,    0,  131,  125,
  129,    0,    0,    0,  187,    0,    0,  192,    0,    0,
  163,    0,  164,    0,  147,  148,    0,    0,    0,  145,
  141,    0,  140,    0,    0,  202,    0,    0,    0,  204,
    0,    0,    0,    0,    0,    0,  222,  226,    6,    0,
  119,  121,    0,    0,  169,   77,   12,    9,    0,   17,
    0,   11,   64,   34,   35,   36,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  126,    0,  195,  185,    0,  188,  191,  193,
  180,    0,    0,  174,    0,    0,    0,    0,    0,  167,
  160,  162,    0,    0,  144,  139,    0,    0,  203,    0,
    0,    0,    0,    0,    0,    0,   32,   10,    0,    8,
    0,  194,  186,  189,    0,  181,  173,  178,  175,  182,
    0,  176,    0,    0,  142,  143,    0,  215,  216,    0,
    0,    0,    0,    0,    0,   18,   62,  190,  183,  179,
  177,    0,    0,  220,    0,  218,    0,   15,    0,  214,
  217,  221,  219,   16,
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
  protected static readonly short [] yySindex = {         2579,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -93, 2579,    0,  -34, 2579,
 2579, 2579, 2579,    0,    0,  -88,    0,    0,  -54, -219,
    0,    0,  -16,  -24,    0,   13,    0,  496,  -19,   34,
 -193,    0,    0,    0,    0,  -29,   74,    0, -219,  144,
   -3,    0,  106,    0,    0,  -24,  -16,    0,    0, 1039,
    0,    0,    0,  -34,    0, 2505, 2579,   34, 1694,  924,
  -48,    0,  167,    0,    0, 1463, 1471, 1471,    0,    0,
 1528,  169,  178,  206,  214,  511,  280,   55,  267,  269,
 1130,  830,    0, 1528, 1528,    0,    0,    0,    0,    0,
    0,  152,  -33,    0,  140,    0, 1528,  252,   43,  160,
   37,   86,  297,  246,  218,  114,  -27,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  147,    0,   23, 1528,
    0,  -83,    0,    0,    0,    0,  328,    0,  456,    0,
    0,    0,    0,    0,    0,    0,  -26,  361,  207,  368,
    0,  -57,    0, 1074,  336,  983,    0,  511,  830,    0,
  830,    0,    0,    0,    0,  400,  511, 1528, 1528, 1528,
  145,  692,  416,    0,    0,    0,  158,  230,  435, 2607,
 2607,  125,    0,    0, 1528,    0,  227,    0,    0, 1161,
 1528,  228,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 1528,    0, 1528, 1528, 1528, 1528, 1528,
 1528, 1528, 1528, 1528, 1528, 1528, 1528, 1528, 1528, 1528,
 1528, 1528, 1528, 1528,    0,    0,    0,  -82,    0,    0,
    0, 1528,  235,   26,    0, 1039,  122,    0, 1617, 1193,
    0,  -31,    0,   66,    0,    0,  238, 2550,  448,    0,
    0, 1528,    0, 1215,  394,    0,  459,  494,  511,    0,
  236,  257,  272,  457, 1232, 1232,    0,    0,    0, 1271,
    0,    0, 1645,   88,    0,    0,    0,    0,  281,    0,
   20,    0,    0,    0,    0,    0,  252,  252,   43,   43,
  160,  160,  160,  160,   37,   37,   86,  297,  246,  218,
  114,  154,    0,  452,    0,    0,  -23,    0,    0,    0,
    0,  507,  509,    0, 1282,  462,   66, 1722, 1321,    0,
    0,    0,  471,  474,    0,    0,  429,  429,    0,  511,
  511,  511, 1528, 1358, 1389,  456,    0,    0, 1528,    0,
 1528,    0,    0,    0, 1039,    0,    0,    0,    0,    0,
  527,    0, 1443,  476,    0,    0,  258,    0,    0,  286,
  511,  300,  511,  312,   38,    0,    0,    0,    0,    0,
    0,  511,  513,    0,  511,    0,  511,    0,   33,    0,
    0,    0,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  519,
  565,  965, 1005,    0,    0,    0,    0,    0, 1513,    0,
    0,    0,    0,   90,    0,    0,    0,  163,    0,  594,
  552,    0,    0,    0,    0, 1569,    0,    0,    0,   41,
    0,    0,    0,    0,    0,  308,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  636,    0,    0,
    0,    0, 1996,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 2023,    0, 2091,    0,    0, 2119, 2308, 2404,
   46, 2623,   79,  978,   -4, 1154,  559,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  163,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  357,    0,    0,  534,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    4,
   10,  535,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  359,    0,  367,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  540,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2128, 2187, 2367, 2376,
 2427, 2464, 2472, 2501,  539, 2560, 2662, 1621, 1400,   52,
 1699,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  382,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 2051,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  220,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  521,    0,    0,  -17,  239,  -68,  927,  -56,    0,
  208,   84,   85,  244,  355,  364,  358,  363,  366,    0,
  -87,    0, -101,  514,    1,    0,    0,   24, 1590,    0,
  537,   62,  -70,    0,    0,    0,  510,  283,  542,  -95,
  -39,  -20,    0,  -18,  -76,    0,    0,  350,   29,  -64,
 -254,    0,  365,  -85,    0, -126,    0,    0,    0,    0,
  477,  588,    0,    0,    0,
  };
  protected static readonly short [] yyTable = {           151,
   29,  150,  158,  175,   65,   43,  200,   44,  249,  109,
  181,  165,  202,  249,  104,   44,  102,   44,  105,  106,
   43,  107,  243,   43,   45,   44,  145,   29,   78,   40,
   52,   53,   54,   55,   57,  234,   57,   60,  239,   57,
  142,  240,  313,  120,  120,  120,  241,  193,  194,  122,
  122,  122,  175,   57,   57,  276,   67,  201,   57,  250,
  215,  166,  355,  195,  250,  109,  238,  242,   59,  317,
  104,   68,  102,   79,  105,  106,   81,  107,  243,  157,
  150,  389,  266,   48,  130,  219,   48,  220,   57,   48,
   48,  270,   58,   57,  120,   58,  225,  265,  226,  149,
  122,  353,  108,   48,   48,  328,  109,  194,   48,   58,
   58,  104,  350,  102,   58,  105,  106,  252,  107,   53,
   57,  141,   53,  242,   80,  190,  286,  283,  147,  150,
  150,  290,  110,  150,  355,  147,   53,   53,   48,   48,
  314,   53,  241,  259,   58,  293,  143,  237,  344,  345,
  316,  267,  284,  268,  175,  149,  329,  394,  108,  294,
  295,  296,  388,   39,  283,  130,   44,  243,   56,   48,
   48,   53,   53,   60,   60,  318,   58,  150,  250,  109,
  150,  326,  319,  339,  104,  253,  102,  327,  105,  106,
  333,  107,  190,  334,  190,  195,   57,  195,  103,  108,
  213,  195,   53,   53,  140,  110,   91,  194,  167,  252,
  196,  351,  242,  190,  190,  250,  278,  178,  251,  327,
  285,   91,   42,  347,  168,   42,  177,  197,  198,  199,
   42,   78,    8,  148,   84,   85,   86,   42,   87,   88,
   42,   17,   18,  284,  233,  179,  354,  256,  150,  157,
  257,  361,  213,  180,  367,  368,  369,  213,  157,  213,
  364,  213,  213,  377,  213,    8,   57,   57,  194,   57,
  279,  235,  108,  195,   17,   18,  340,  150,  213,  195,
  376,   89,   90,  157,  378,  384,  150,  386,  218,  148,
   84,   85,   86,  216,   87,   88,  390,  341,  217,  392,
  195,  393,  223,  224,  299,  300,  194,  301,  302,  303,
  304,  183,  342,   48,   48,  195,   48,   48,  354,  182,
  150,  348,   58,   58,  349,  184,  383,  185,  157,  195,
   83,   84,   85,   86,  229,   87,   88,   89,   90,  230,
  385,  231,  213,  195,  213,  213,  150,  151,  151,   53,
   53,  151,  387,  227,  228,  195,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   89,   90,
   23,   24,   25,   26,  232,   91,   92,   93,   70,   94,
   95,   96,   97,   98,   99,  100,  101,  165,  151,  170,
  165,  255,  170,   83,   84,   85,   86,  171,   87,   88,
  171,  258,  203,  204,  205,  206,  207,  208,  209,  210,
  211,  212,  172,  221,  222,  172,  297,  298,  261,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   89,   90,   23,   24,   25,   26,  269,   91,   92,
   93,  274,   94,   95,   96,   97,   98,   99,  100,  101,
  305,  306,  281,  282,  277,  280,  213,  213,  213,  213,
  109,  213,  213,  287,  292,  104,  336,  102,  109,  105,
  106,  315,  107,  104,  330,  102,  343,  105,  106,  337,
  107,  243,  213,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  213,  213,  213,  213,  213,  213,  213,
   71,  213,  213,  213,  338,  213,  213,  213,  213,  213,
  213,  213,  213,  109,  352,   58,  242,  356,  104,  357,
  102,  346,  105,  106,  359,  107,   70,   75,   81,   81,
   81,   73,   81,  365,  151,   82,  366,  379,  381,  110,
  129,  391,  382,  108,  159,  168,   49,   81,  149,   49,
  170,  108,   49,  307,  375,  153,  137,   72,  309,  152,
  154,  137,  137,  308,  310,  137,   49,   49,  311,   61,
  139,   49,   61,  146,   83,   83,   83,  332,   83,   81,
  137,  320,  137,  236,   41,    0,   61,   61,   57,    0,
    0,  187,  188,   83,    0,    0,    0,    0,  134,    0,
    0,   49,   49,   57,  134,    0,  108,  134,    0,    0,
    0,    0,  137,  137,    0,    0,    0,    0,    0,    0,
  129,   61,  134,    0,  134,   83,    0,    0,    0,    0,
    0,    0,   49,   49,    0,    0,    0,    0,    0,    0,
  133,    0,    0,    0,  137,    0,  133,    0,    0,  133,
    0,    0,    0,   61,    0,  134,    0,    0,    0,  188,
    0,  188,    0,    0,  133,  275,  133,    0,  271,  272,
  273,    0,    0,    0,  148,   84,   85,   86,    0,   87,
   88,    0,  148,   84,   85,   86,  134,   87,   88,    0,
    0,  291,    0,    0,  109,    0,    0,  133,    0,  104,
    0,  102,    0,  105,  106,    0,  107,    8,    0,    0,
    0,    0,    0,    0,    0,    0,   17,   18,    0,    0,
  110,    0,   89,   90,  312,    0,    0,    0,  133,    0,
   89,   90,    0,    0,    0,    0,    0,   83,   84,   85,
   86,    0,   87,   88,    0,   81,    0,    0,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
    0,    0,   23,   24,   25,   26,   49,   49,    0,   49,
   49,    0,    0,    0,    0,   89,   90,  108,    0,   69,
    0,   83,   91,   92,   93,    0,   94,   95,   96,   97,
   98,   99,  100,  101,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,  137,  137,  137,  137,
  137,  137,  137,  137,  137,  137,    0,    0,  137,  137,
  137,  137,  109,  370,  372,  374,    0,  104,    0,  102,
    0,  105,  106,    0,  107,  137,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,  134,  134,
  134,  134,  134,  134,  134,  134,  134,  134,    0,    0,
  134,  134,  134,  134,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  134,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
    0,    0,  133,  133,  133,  133,    0,    0,  148,   84,
   85,   86,    0,   87,   88,  108,  109,    0,    0,  133,
    0,  104,    0,  102,    0,  164,  106,    0,  107,    0,
    0,    0,    0,    0,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   89,   90,   23,   24,
   25,   26,    0,    0,   85,   85,   85,    0,   85,    0,
    0,    0,  170,  172,  173,  109,  163,  174,   55,    0,
  104,   55,  102,   85,  264,  106,    0,  107,    0,    0,
  174,  174,    0,    0,    0,   55,   55,    0,    0,    0,
   55,    0,    0,  174,   87,   87,   87,    0,   87,  108,
    0,    0,    0,    0,    0,   85,    0,    0,    0,    0,
    0,    0,    0,   87,    0,    0,  174,    0,    0,    0,
   55,  109,    0,    0,    0,  263,  104,    0,  102,    0,
  105,  106,    0,  107,    0,    0,  148,   84,   85,   86,
  174,   87,   88,    0,    0,   87,    0,    0,    0,    0,
    0,   55,   55,    0,    0,    0,  109,    0,  108,    0,
    0,  104,    1,  102,    0,  105,  106,    0,  107,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   89,   90,   23,   24,   25,   26,
    0,    0,  174,  174,  174,  174,  174,  174,  174,  174,
  174,  174,  174,  174,  174,  174,  174,  174,  174,  174,
    0,  149,  109,    0,  108,    0,  260,  104,  174,  102,
    0,  105,  106,    0,  107,    0,    0,    0,    0,    0,
  148,   84,   85,   86,    0,   87,   88,    0,  186,    0,
  174,    0,    0,  109,   59,    0,    0,   59,  104,  108,
  102,  288,  105,  106,    0,  107,  174,    0,    0,  162,
    0,   59,   59,    8,    0,    0,   59,    0,    0,    0,
    0,   85,   17,   18,    0,  109,    0,    0,   89,   90,
  104,    0,  102,    0,  325,  106,    0,  107,    0,  148,
   84,   85,   86,    0,   87,   88,   59,  109,   55,   55,
    0,  174,  104,    0,  102,  108,  105,  106,    0,  107,
    0,   87,    0,    0,  109,    0,    0,    0,  262,  104,
    0,  102,    8,  105,  106,    0,  107,  174,   59,    0,
    0,   17,   18,    0,    0,  324,  108,   89,   90,  174,
  110,    0,    0,    0,    0,  148,   84,   85,   86,    0,
   87,   88,    0,  109,    0,    0,    0,  335,  104,    0,
  102,    0,  105,  106,  109,  107,    0,    0,  108,  104,
    0,  102,    0,  105,  106,    0,  107,    0,    0,    0,
  148,   84,   85,   86,    0,   87,   88,    0,    0,    0,
  108,    0,    0,   89,   90,    0,    0,    0,    0,    0,
    0,    0,    0,  109,    0,    0,    0,  108,  104,    0,
  102,    0,  363,  106,    0,  107,    0,    0,    0,    0,
    0,    0,    0,    0,  358,    0,    0,    0,   89,   90,
    0,    0,    0,    0,    0,    0,  148,   84,   85,   86,
  109,   87,   88,  346,    0,  104,  108,  102,  371,  105,
  106,    0,  107,    0,    0,    0,    0,  108,    0,    0,
    0,    0,    0,  362,    0,    0,    0,  148,   84,   85,
   86,  109,   87,   88,    0,   59,  104,    0,  102,  373,
  105,  106,    0,  107,   89,   90,    0,    0,    0,    0,
   56,    0,    0,   56,    0,    0,  108,    0,    0,  148,
   84,   85,   86,    0,   87,   88,    0,   56,   56,    0,
    0,    0,   56,    0,    0,   89,   90,    0,    0,    0,
    0,  148,   84,   85,   86,  109,   87,   88,    0,    0,
  104,    0,  102,  108,  105,  106,    0,  107,  148,   84,
   85,   86,   56,   87,   88,  109,    0,   89,   90,    0,
  104,    0,  169,  109,  105,  106,    0,  107,  104,    0,
  171,    0,  105,  106,  108,  107,    0,    0,    0,   89,
   90,    0,    0,   56,   56,    0,    0,  148,   84,   85,
   86,    0,   87,   88,    0,  380,   89,   90,  148,   84,
   85,   86,    0,   87,   88,    0,    0,    0,    0,    0,
    0,    0,  127,  127,  127,    0,  127,    0,    0,    0,
  109,    0,    0,    0,    0,  104,    0,  102,  108,  105,
  106,  127,  107,    0,    0,   89,   90,  148,   84,   85,
   86,    0,   87,   88,    0,    0,   89,   90,  108,    0,
    0,    0,    0,    0,    0,    0,  108,    0,    0,    0,
    0,    0,    0,  127,    0,    0,    0,    0,  115,  115,
  115,    0,  115,    0,  148,   84,   85,   86,    0,   87,
   88,    0,    0,    0,    0,   89,   90,  115,    0,    0,
    0,    0,    0,   64,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  148,   84,   85,   86,    0,
   87,   88,    0,  108,    0,  144,  249,  321,   44,  115,
    0,   54,   89,   90,   54,    0,    0,    0,    0,   64,
   56,   56,    0,    0,    0,    0,    0,    0,   54,   54,
    0,    0,    0,   54,  283,  321,   44,    0,    0,    0,
    0,  191,    0,   89,   90,    0,    0,    0,    0,  148,
   84,   85,   86,    0,   87,   88,    0,  250,    0,    0,
    0,    0,    0,   54,   54,    0,    0,    0,    0,  148,
   84,   85,   86,    0,   87,   88,    0,  148,   84,   85,
   86,    0,   87,   88,  156,  250,    0,    0,    0,   60,
    0,    0,   60,    0,   54,   54,    0,   89,   90,    0,
    0,   64,    0,    0,    0,  144,   60,   60,  191,    0,
  191,   60,  360,    0,    0,    0,    0,   89,   90,  127,
    0,    0,    0,    0,    0,   89,   90,    0,    0,  191,
  191,    0,    0,    0,  148,   84,   85,   86,    0,   87,
   88,   60,    0,    0,    0,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,  127,  127,  127,
  127,  127,  127,  127,  127,  127,  127,    0,    0,  127,
  127,  127,  127,   60,    0,  115,    0,    0,    0,    0,
    0,    0,   89,   90,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  144,    0,
    0,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,  115,  115,  115,  115,  115,  115,  115,
  115,  115,  115,   42,    0,  115,  115,  115,  115,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   54,   54,    0,    0,    0,    0,    0,    0,    1,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
  155,   23,   24,   25,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   60,    0,    0,    0,    0,    0,    1,    2,    3,    4,
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
   31,    0,   31,   31,    0,    0,   37,    0,    0,   37,
    0,   37,   37,   37,    0,   38,    0,    0,   38,    0,
   38,   38,   38,    0,   26,   26,   37,   37,   37,    0,
   37,   37,    0,   31,   31,   38,   38,   38,    0,   38,
   38,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   37,   37,    0,   31,   31,    0,    0,    0,    0,
   38,   38,    0,    0,   39,    0,    0,   39,    0,   39,
   39,   39,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   37,   37,   39,   39,   39,    0,   39,   39,
    0,   38,   38,    0,    0,    0,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    0,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    0,   39,
   39,    0,    0,    0,    0,    0,   19,   19,   19,   19,
   19,   19,    0,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   19,    0,    0,    0,    0,    0,
   39,   39,    0,    0,   26,   26,   26,   26,   26,   26,
    0,   26,   26,   26,   26,   26,   26,   26,   26,   26,
   26,   26,   26,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   40,    0,    0,   40,    0,
    0,   40,    0,    0,   31,   31,   31,   31,   31,   31,
    0,   31,   31,    0,    0,   40,   40,   40,    0,   40,
   40,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   37,   37,   37,   37,   37,   37,    0,   37,
   37,   38,   38,   38,   38,   38,   38,    0,   38,   38,
   40,   40,    0,    0,   41,    0,    0,   41,    0,    0,
   41,    0,    0,   42,    0,    0,   42,    0,    0,   42,
    0,    0,    0,    0,   41,   41,   41,    0,   41,   41,
    0,   40,   40,   42,   42,   42,    0,   42,   42,    0,
    0,   43,    0,    0,   43,    0,    0,   43,    0,    0,
   39,   39,   39,   39,   39,   39,    0,   39,   39,   41,
   41,   43,   43,   43,   46,   43,   43,   46,   42,   42,
   46,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   46,   46,   46,    0,   46,   46,
   41,   41,    0,    0,    0,    0,   43,   43,    0,   42,
   42,   47,    0,    0,   47,    0,    0,   47,    0,   44,
    0,    0,   44,    0,    0,   44,    0,    0,    0,   46,
   46,   47,   47,   47,    0,   47,   47,   43,   43,   44,
   44,   44,    0,   44,   44,    0,    0,    0,   45,    0,
    0,   45,    0,    0,   45,    0,    0,    0,    0,    0,
   46,   46,    0,    0,    0,    0,   47,   47,   45,   45,
   45,    0,   45,   45,   44,   44,    0,    0,    0,    0,
    0,   40,   40,   40,   40,   40,   40,    0,   40,   40,
    0,    0,    0,    0,    0,    0,    0,   47,   47,    0,
    0,    0,    0,   45,   45,   44,   44,   50,    0,    0,
   50,    0,    0,   50,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   50,   50,    0,
    0,    0,   50,    0,   45,   45,    0,   57,    0,    0,
   41,   41,   41,   41,   41,   41,    0,   41,   41,   42,
   42,   42,   42,   42,   42,    0,   42,   42,    0,    0,
    0,    0,   50,   50,    0,    0,    0,    0,    0,    0,
   51,    0,    0,   51,    0,    0,   51,    0,    0,   43,
   43,   43,   43,    0,   43,   43,    0,    0,    0,    0,
   51,   51,    0,   50,   50,   51,    0,    0,    0,    0,
    0,    0,   46,   46,   46,   46,    0,   46,   46,   52,
    0,    0,   52,    0,    0,   52,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   51,   51,    0,    0,   52,
   52,    0,    0,    0,   52,    0,    0,    0,    0,   47,
   47,   47,   47,    0,   47,   47,    0,   44,   44,   44,
   44,    0,   44,   44,    0,    0,   51,   51,    0,    0,
    0,    0,    0,    0,   52,   52,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   45,   45,   45,   45,
    0,   45,   45,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   52,   52,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,    0,
    0,   23,   24,   25,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   50,   50,    0,
   50,   50,    1,    2,    3,    4,    5,    6,    7,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,    0,    0,   23,   24,   25,   26,
  331,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,    0,    0,   23,   24,   25,   26,    1,
    0,    0,    0,   51,   51,    0,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,    0,    0,   23,   24,   25,   26,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   52,   52,
  };
  protected static readonly short [] yyCheck = {            70,
    0,   70,   79,   91,   44,   40,   40,   42,   40,   33,
   96,   80,   46,   40,   38,   42,   40,   42,   42,   43,
   40,   45,   46,   40,   59,   42,   66,   27,   49,  123,
   30,   31,   32,   33,  123,   63,   41,  257,  140,   44,
   44,  125,  125,   40,   41,   42,  142,  104,  105,   40,
   41,   42,  140,   58,   59,  182,   44,   91,   63,   91,
  117,   80,  317,   44,   91,   33,   44,   91,  123,   44,
   38,   59,   40,   40,   42,   43,  270,   45,   46,   79,
  149,   44,  168,   38,   44,   43,   41,   45,   93,   44,
   29,  177,   41,  123,   91,   44,   60,  166,   62,  123,
   91,  125,  126,   58,   59,   40,   33,  164,   63,   58,
   59,   38,   93,   40,   63,   42,   43,  157,   45,   41,
  125,  125,   44,   91,   91,  102,  195,   40,   67,   40,
   41,  200,   59,   44,  389,   74,   58,   59,   93,   94,
  242,   63,  238,  162,   93,  214,   41,  125,  275,  276,
  125,  169,  192,  171,  242,  123,   91,  125,  126,  216,
  217,  218,  125,  257,   40,  125,   42,   46,  257,  124,
  125,   93,   94,  257,  257,  246,  125,  246,   91,   33,
   91,  250,   61,  269,   38,  157,   40,  252,   42,   43,
  259,   45,  169,  262,  171,   44,  123,   44,  125,  126,
   61,   44,  124,  125,   61,   59,   44,  264,  257,  249,
   59,   58,   91,  190,  191,   91,   59,   40,  157,  284,
  192,   59,  257,  280,   58,  257,   58,  261,  262,  263,
  257,  252,  290,  257,  258,  259,  260,  257,  262,  263,
  257,  299,  300,  283,  272,   40,  317,   41,  317,  249,
   44,  328,   33,   40,  340,  341,  342,   38,  258,   40,
  329,   42,   43,  351,   45,  290,  271,  272,  325,  123,
   41,  125,  126,   44,  299,  300,   41,  346,   59,   44,
  349,  305,  306,  283,  355,  371,  355,  373,   37,  257,
  258,  259,  260,   42,  262,  263,  382,   41,   47,  385,
   44,  387,  266,  267,  221,  222,  363,  223,  224,  225,
  226,  257,   41,  268,  269,   44,  271,  272,  389,   40,
  389,   41,  271,  272,   44,   59,   41,   59,  328,   44,
  257,  258,  259,  260,   38,  262,  263,  305,  306,   94,
   41,  124,  123,   44,  125,  126,  257,   40,   41,  271,
  272,   44,   41,  268,  269,   44,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  271,  312,  313,  314,   61,  316,
  317,  318,  319,  320,  321,  322,  323,   41,   91,   41,
   44,   41,   44,  257,  258,  259,  260,   41,  262,  263,
   44,   44,  273,  274,  275,  276,  277,  278,  279,  280,
  281,  282,   41,  264,  265,   44,  219,  220,   93,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,   58,  312,  313,
  314,  317,  316,  317,  318,  319,  320,  321,  322,  323,
  227,  228,  190,  191,   59,   41,  257,  258,  259,  260,
   33,  262,  263,  257,  257,   38,   93,   40,   33,   42,
   43,  257,   45,   38,  257,   40,   40,   42,   43,   41,
   45,   46,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   35,  312,  313,  314,   41,  316,  317,  318,  319,  320,
  321,  322,  323,   33,   93,   36,   91,   41,   38,   41,
   40,  123,   42,   43,   93,   45,   61,   48,   40,   41,
   42,   48,   44,   93,  257,   56,   93,   41,   93,   59,
   57,   59,  315,  126,   41,   41,   38,   59,  123,   41,
   41,  126,   44,  229,  346,   76,   35,   92,  231,   76,
   77,   40,   41,  230,  232,   44,   58,   59,  233,   41,
   59,   63,   44,   67,   40,   41,   42,  258,   44,   91,
   59,  247,   61,  137,   27,   -1,   58,   59,  123,   -1,
   -1,  101,  102,   59,   -1,   -1,   -1,   -1,   35,   -1,
   -1,   93,   94,  123,   41,   -1,  126,   44,   -1,   -1,
   -1,   -1,   91,   92,   -1,   -1,   -1,   -1,   -1,   -1,
  137,   93,   59,   -1,   61,   91,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   35,   -1,   -1,   -1,  123,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,  125,   -1,   92,   -1,   -1,   -1,  169,
   -1,  171,   -1,   -1,   59,  182,   61,   -1,  178,  179,
  180,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,  257,  258,  259,  260,  123,  262,  263,   -1,
   -1,  201,   -1,   -1,   33,   -1,   -1,   92,   -1,   38,
   -1,   40,   -1,   42,   43,   -1,   45,  290,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  299,  300,   -1,   -1,
   59,   -1,  305,  306,  234,   -1,   -1,   -1,  123,   -1,
  305,  306,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,  257,   -1,   -1,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
   -1,   -1,  307,  308,  309,  310,  268,  269,   -1,  271,
  272,   -1,   -1,   -1,   -1,  305,  306,  126,   -1,  324,
   -1,  257,  312,  313,  314,   -1,  316,  317,  318,  319,
  320,  321,  322,  323,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,   -1,   -1,  307,  308,
  309,  310,   33,  343,  344,  345,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,  324,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,   -1,   -1,
  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  324,  283,  284,
  285,  286,  287,  288,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
   -1,   -1,  307,  308,  309,  310,   -1,   -1,  257,  258,
  259,  260,   -1,  262,  263,  126,   33,   -1,   -1,  324,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,   -1,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   40,   41,   42,   -1,   44,   -1,
   -1,   -1,   86,   87,   88,   33,   93,   91,   41,   -1,
   38,   44,   40,   59,   42,   43,   -1,   45,   -1,   -1,
  104,  105,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,
   63,   -1,   -1,  117,   40,   41,   42,   -1,   44,  126,
   -1,   -1,   -1,   -1,   -1,   91,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   -1,   -1,  140,   -1,   -1,   -1,
   93,   33,   -1,   -1,   -1,   93,   38,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  257,  258,  259,  260,
  164,  262,  263,   -1,   -1,   91,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   33,   -1,  126,   -1,
   -1,   38,  283,   40,   -1,   42,   43,   -1,   45,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,  216,  217,  218,  219,  220,  221,  222,  223,
  224,  225,  226,  227,  228,  229,  230,  231,  232,  233,
   -1,  123,   33,   -1,  126,   -1,   93,   38,  242,   40,
   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   59,   -1,
  264,   -1,   -1,   33,   41,   -1,   -1,   44,   38,  126,
   40,   41,   42,   43,   -1,   45,  280,   -1,   -1,  286,
   -1,   58,   59,  290,   -1,   -1,   63,   -1,   -1,   -1,
   -1,  257,  299,  300,   -1,   33,   -1,   -1,  305,  306,
   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,  257,
  258,  259,  260,   -1,  262,  263,   93,   33,  271,  272,
   -1,  325,   38,   -1,   40,  126,   42,   43,   -1,   45,
   -1,  257,   -1,   -1,   33,   -1,   -1,   -1,  286,   38,
   -1,   40,  290,   42,   43,   -1,   45,  351,  125,   -1,
   -1,  299,  300,   -1,   -1,   93,  126,  305,  306,  363,
   59,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   33,   -1,   -1,   -1,   93,   38,   -1,
   40,   -1,   42,   43,   33,   45,   -1,   -1,  126,   38,
   -1,   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
  126,   -1,   -1,  305,  306,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   33,   -1,   -1,   -1,  126,   38,   -1,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,  305,  306,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   33,  262,  263,  123,   -1,   38,  126,   40,   41,   42,
   43,   -1,   45,   -1,   -1,   -1,   -1,  126,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,  257,  258,  259,
  260,   33,  262,  263,   -1,  272,   38,   -1,   40,   41,
   42,   43,   -1,   45,  305,  306,   -1,   -1,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,  126,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   58,   59,   -1,
   -1,   -1,   63,   -1,   -1,  305,  306,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   33,  262,  263,   -1,   -1,
   38,   -1,   40,  126,   42,   43,   -1,   45,  257,  258,
  259,  260,   93,  262,  263,   33,   -1,  305,  306,   -1,
   38,   -1,   40,   33,   42,   43,   -1,   45,   38,   -1,
   40,   -1,   42,   43,  126,   45,   -1,   -1,   -1,  305,
  306,   -1,   -1,  124,  125,   -1,   -1,  257,  258,  259,
  260,   -1,  262,  263,   -1,   93,  305,  306,  257,  258,
  259,  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   40,   41,   42,   -1,   44,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   38,   -1,   40,  126,   42,
   43,   59,   45,   -1,   -1,  305,  306,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,  305,  306,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,   -1,   -1,
   -1,   -1,   -1,   91,   -1,   -1,   -1,   -1,   40,   41,
   42,   -1,   44,   -1,  257,  258,  259,  260,   -1,  262,
  263,   -1,   -1,   -1,   -1,  305,  306,   59,   -1,   -1,
   -1,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,  126,   -1,   66,   40,   41,   42,   91,
   -1,   41,  305,  306,   44,   -1,   -1,   -1,   -1,   80,
  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,
   -1,   -1,   -1,   63,   40,   41,   42,   -1,   -1,   -1,
   -1,  102,   -1,  305,  306,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   91,   -1,   -1,
   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,  257,  258,  259,
  260,   -1,  262,  263,   41,   91,   -1,   -1,   -1,   41,
   -1,   -1,   44,   -1,  124,  125,   -1,  305,  306,   -1,
   -1,  162,   -1,   -1,   -1,  166,   58,   59,  169,   -1,
  171,   63,   41,   -1,   -1,   -1,   -1,  305,  306,  257,
   -1,   -1,   -1,   -1,   -1,  305,  306,   -1,   -1,  190,
  191,   -1,   -1,   -1,  257,  258,  259,  260,   -1,  262,
  263,   93,   -1,   -1,   -1,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,   -1,   -1,  307,
  308,  309,  310,  125,   -1,  257,   -1,   -1,   -1,   -1,
   -1,   -1,  305,  306,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  259,   -1,
   -1,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  257,   -1,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
  257,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  272,   -1,   -1,   -1,   -1,   -1,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,   -1,   -1,
  307,  308,  309,  310,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,   -1,   -1,  307,  308,
  309,  310,   37,   38,   -1,   40,   -1,   42,   43,   44,
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
   93,   94,   -1,   -1,   38,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   58,   59,   60,   -1,   62,   63,
   -1,  124,  125,   -1,   -1,   -1,  261,  262,  263,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,  273,  274,
  275,  276,  277,  278,  279,  280,  281,  282,   -1,   93,
   94,   -1,   -1,   -1,   -1,   -1,  264,  265,  266,  267,
  268,  269,   -1,  271,  272,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  282,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  282,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,  264,  265,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   58,   59,   60,   -1,   62,
   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  264,  265,  266,  267,  268,  269,   -1,  271,
  272,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   93,   94,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,
   -1,  124,  125,   58,   59,   60,   -1,   62,   63,   -1,
   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,   93,
   94,   58,   59,   60,   38,   62,   63,   41,   93,   94,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   -1,   62,   63,
  124,  125,   -1,   -1,   -1,   -1,   93,   94,   -1,  124,
  125,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   93,
   94,   58,   59,   60,   -1,   62,   63,  124,  125,   58,
   59,   60,   -1,   62,   63,   -1,   -1,   -1,   38,   -1,
   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   93,   94,   58,   59,
   60,   -1,   62,   63,   93,   94,   -1,   -1,   -1,   -1,
   -1,  264,  265,  266,  267,  268,  269,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   93,   94,  124,  125,   38,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   -1,
   -1,   -1,   63,   -1,  124,  125,   -1,  123,   -1,   -1,
  264,  265,  266,  267,  268,  269,   -1,  271,  272,  264,
  265,  266,  267,  268,  269,   -1,  271,  272,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,
   38,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,  266,
  267,  268,  269,   -1,  271,  272,   -1,   -1,   -1,   -1,
   58,   59,   -1,  124,  125,   63,   -1,   -1,   -1,   -1,
   -1,   -1,  266,  267,  268,  269,   -1,  271,  272,   38,
   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   58,
   59,   -1,   -1,   -1,   63,   -1,   -1,   -1,   -1,  266,
  267,  268,  269,   -1,  271,  272,   -1,  266,  267,  268,
  269,   -1,  271,  272,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  266,  267,  268,  269,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,   -1,
   -1,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  268,  269,   -1,
  271,  272,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,   -1,   -1,  307,  308,  309,  310,
  311,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,   -1,   -1,  307,  308,  309,  310,  283,
   -1,   -1,   -1,  271,  272,   -1,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,   -1,   -1,  307,  308,  309,  310,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  271,  272,
  };

#line 1012 "CParser.jay"

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
