// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;

namespace CLanguage
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

  protected const int yyFinal = 26;
//t // Put this array into a separate class so it is only initialized if debugging is actually used
//t // Use MarshalByRefObject to disable inlining
//t class YYRules : MarshalByRefObject {
//t  public static readonly string [] yyRule = {
//t    "$accept : translation_unit",
//t    "primary_expression : IDENTIFIER",
//t    "primary_expression : CONSTANT",
//t    "primary_expression : STRING_LITERAL",
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
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER '{' struct_declaration_list '}'",
//t    "struct_or_union_specifier : struct_or_union '{' struct_declaration_list '}'",
//t    "struct_or_union_specifier : struct_or_union IDENTIFIER",
//t    "struct_or_union : STRUCT",
//t    "struct_or_union : UNION",
//t    "struct_declaration_list : struct_declaration",
//t    "struct_declaration_list : struct_declaration_list struct_declaration",
//t    "struct_declaration : specifier_qualifier_list struct_declarator_list ';'",
//t    "specifier_qualifier_list : type_specifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_specifier",
//t    "specifier_qualifier_list : type_qualifier specifier_qualifier_list",
//t    "specifier_qualifier_list : type_qualifier",
//t    "struct_declarator_list : struct_declarator",
//t    "struct_declarator_list : struct_declarator_list ',' struct_declarator",
//t    "struct_declarator : declarator",
//t    "struct_declarator : ':' constant_expression",
//t    "struct_declarator : declarator ':' constant_expression",
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
//t    "direct_declarator : IDENTIFIER",
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
//t    "$$1 :",
//t    "compound_statement : '{' $$1 '}'",
//t    "$$2 :",
//t    "compound_statement : '{' $$2 block_item_list '}'",
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
//t    "$$3 :",
//t    "iteration_statement : FOR '(' declaration expression_statement ')' $$3 statement",
//t    "$$4 :",
//t    "iteration_statement : FOR '(' declaration expression_statement expression ')' $$4 statement",
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
    "NE_OP","AND_OP","OR_OP","MUL_ASSIGN","DIV_ASSIGN","MOD_ASSIGN",
    "ADD_ASSIGN","SUB_ASSIGN","LEFT_ASSIGN","RIGHT_ASSIGN","AND_ASSIGN",
    "XOR_ASSIGN","OR_ASSIGN","TYPE_NAME","TYPEDEF","EXTERN","STATIC",
    "AUTO","REGISTER","INLINE","RESTRICT","CHAR","SHORT","INT","LONG",
    "SIGNED","UNSIGNED","FLOAT","DOUBLE","CONST","VOLATILE","VOID","BOOL",
    "COMPLEX","IMAGINARY","STRUCT","UNION","ENUM","ELLIPSIS","CASE",
    "DEFAULT","IF","ELSE","SWITCH","WHILE","DO","FOR","GOTO","CONTINUE",
    "BREAK","RETURN",
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
#line 36 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new VariableExpression((yyVals[0+yyTop]).ToString()); }
  break;
case 2:
#line 37 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 3:
#line 38 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new ConstantExpression(yyVals[0+yyTop]); }
  break;
case 4:
#line 39 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 5:
#line 46 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 6:
#line 50 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new ArrayElementExpression((Expression)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 7:
#line 54 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-2+yyTop]);
	}
  break;
case 8:
#line 58 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new FuncallExpression((Expression)yyVals[-3+yyTop], (List<Expression>)yyVals[-1+yyTop]);
	}
  break;
case 9:
#line 62 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new MemberFromReferenceExpression((Expression)yyVals[-2+yyTop], (yyVals[0+yyTop]).ToString());
	}
  break;
case 10:
#line 66 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		throw new NotSupportedException ("Syntax: postfix_expression PTR_OP IDENTIFIER");
	}
  break;
case 11:
#line 70 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostIncrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 12:
#line 74 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PostDecrement, (Expression)yyVals[-1+yyTop]);
	}
  break;
case 13:
#line 78 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list '}'");
	}
  break;
case 14:
#line 82 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '(' type_name ')' '{' initializer_list ',' '}'");
	}
  break;
case 15:
  case_15();
  break;
case 16:
  case_16();
  break;
case 17:
#line 104 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 18:
#line 108 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreIncrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 19:
#line 112 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new UnaryExpression(Unop.PreDecrement, (Expression)yyVals[0+yyTop]);
	}
  break;
case 20:
#line 116 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '&' cast_expression");
	}
  break;
case 21:
#line 120 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		throw new NotSupportedException ("Syntax: '*' cast_expression");
	}
  break;
case 22:
#line 124 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new UnaryExpression((Unop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 23:
#line 128 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		throw new NotSupportedException ("Syntax: SIZEOF unary_expression");
	}
  break;
case 24:
#line 132 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		throw new NotSupportedException ("Syntax: SIZEOF '(' type_name ')'");
	}
  break;
case 25:
#line 136 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Unop.None; }
  break;
case 26:
#line 137 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Unop.Negate; }
  break;
case 27:
#line 138 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Unop.BinaryComplement; }
  break;
case 28:
#line 139 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Unop.Not; }
  break;
case 29:
#line 146 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 30:
#line 150 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 31:
#line 157 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 32:
#line 161 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Multiply, (Expression)yyVals[0+yyTop]);
	}
  break;
case 33:
#line 165 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Divide, (Expression)yyVals[0+yyTop]);
	}
  break;
case 34:
#line 169 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Mod, (Expression)yyVals[0+yyTop]);
	}
  break;
case 36:
#line 177 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Add, (Expression)yyVals[0+yyTop]);
	}
  break;
case 37:
#line 181 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.Subtract, (Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 189 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftLeft, (Expression)yyVals[0+yyTop]);
	}
  break;
case 40:
#line 193 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.ShiftRight, (Expression)yyVals[0+yyTop]);
	}
  break;
case 42:
#line 201 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 43:
#line 205 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThan, (Expression)yyVals[0+yyTop]);
	}
  break;
case 44:
#line 209 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.LessThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 213 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.GreaterThanOrEqual, (Expression)yyVals[0+yyTop]);
	}
  break;
case 47:
#line 221 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.Equals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 48:
#line 225 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new RelationalExpression((Expression)yyVals[-2+yyTop], RelationalOp.NotEquals, (Expression)yyVals[0+yyTop]);
	}
  break;
case 50:
#line 233 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryAnd, (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 241 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryXor, (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 249 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new BinaryExpression((Expression)yyVals[-2+yyTop], Binop.BinaryOr, (Expression)yyVals[0+yyTop]);
	}
  break;
case 56:
#line 257 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.And, (Expression)yyVals[0+yyTop]);
	}
  break;
case 58:
#line 265 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new LogicExpression((Expression)yyVals[-2+yyTop], LogicOp.Or, (Expression)yyVals[0+yyTop]);
	}
  break;
case 62:
  case_62();
  break;
case 63:
#line 288 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = RelationalOp.Equals; }
  break;
case 64:
#line 289 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.Multiply; }
  break;
case 65:
#line 290 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.Divide; }
  break;
case 66:
#line 291 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.Mod; }
  break;
case 67:
#line 292 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.Add; }
  break;
case 68:
#line 293 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.Subtract; }
  break;
case 69:
#line 294 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.ShiftLeft; }
  break;
case 70:
#line 295 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.ShiftRight; }
  break;
case 71:
#line 296 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.BinaryAnd; }
  break;
case 72:
#line 297 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.BinaryXor; }
  break;
case 73:
#line 298 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = Binop.BinaryOr; }
  break;
case 74:
#line 305 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 75:
#line 309 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new SequenceExpression ((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 77:
  case_77();
  break;
case 78:
  case_78();
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
#line 415 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = StorageClassSpecifier.Typedef; }
  break;
case 92:
#line 416 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = StorageClassSpecifier.Extern; }
  break;
case 93:
#line 417 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = StorageClassSpecifier.Static; }
  break;
case 94:
#line 418 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = StorageClassSpecifier.Auto; }
  break;
case 95:
#line 419 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = StorageClassSpecifier.Register; }
  break;
case 96:
#line 423 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "void"); }
  break;
case 97:
#line 424 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "char"); }
  break;
case 98:
#line 425 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "short"); }
  break;
case 99:
#line 426 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "int"); }
  break;
case 100:
#line 427 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "long"); }
  break;
case 101:
#line 428 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "float"); }
  break;
case 102:
#line 429 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "double"); }
  break;
case 103:
#line 430 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "signed"); }
  break;
case 104:
#line 431 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "unsigned"); }
  break;
case 105:
#line 432 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "bool"); }
  break;
case 106:
#line 433 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "complex"); }
  break;
case 107:
#line 434 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Builtin, "imaginary"); }
  break;
case 110:
#line 437 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new TypeSpecifier(TypeSpecifierKind.Typename, (yyVals[0+yyTop]).ToString()); }
  break;
case 137:
#line 497 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = FunctionSpecifier.Inline; }
  break;
case 138:
#line 504 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new PointerDeclarator((Pointer)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
	}
  break;
case 139:
#line 505 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 140:
#line 510 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new IdentifierDeclarator((yyVals[0+yyTop]).ToString()); }
  break;
case 141:
  case_141();
  break;
case 142:
#line 526 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 143:
#line 530 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 144:
#line 534 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 145:
#line 538 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 146:
#line 542 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-5+yyTop], (TypeQualifiers)yyVals[-3+yyTop], (Expression)yyVals[-1+yyTop], true);
	}
  break;
case 147:
#line 546 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-4+yyTop], (TypeQualifiers)yyVals[-2+yyTop], null, false);
	}
  break;
case 148:
#line 550 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 149:
#line 554 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 150:
  case_150();
  break;
case 151:
  case_151();
  break;
case 152:
  case_152();
  break;
case 153:
#line 582 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None); }
  break;
case 154:
#line 583 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[0+yyTop]); }
  break;
case 155:
#line 584 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new Pointer(TypeQualifiers.None, (Pointer)yyVals[0+yyTop]); }
  break;
case 156:
#line 585 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = new Pointer((TypeQualifiers)yyVals[-1+yyTop], (Pointer)yyVals[0+yyTop]); }
  break;
case 157:
#line 589 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 158:
#line 593 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = (TypeQualifiers)(yyVals[-1+yyTop]) | (TypeQualifiers)(yyVals[0+yyTop]);
	}
  break;
case 159:
#line 597 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = TypeQualifiers.Const; }
  break;
case 160:
#line 598 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = TypeQualifiers.Restrict; }
  break;
case 161:
#line 599 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { yyVal = TypeQualifiers.Volatile; }
  break;
case 162:
#line 606 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
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
  case_168();
  break;
case 169:
  case_169();
  break;
case 170:
  case_170();
  break;
case 176:
  case_176();
  break;
case 177:
#line 690 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 178:
#line 694 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 179:
#line 698 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-2+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 180:
#line 702 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, (Expression)yyVals[-1+yyTop], false);
	}
  break;
case 181:
#line 706 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator(null, TypeQualifiers.None, null, false);
	}
  break;
case 182:
#line 710 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = MakeArrayDeclarator((Declarator)yyVals[-3+yyTop], TypeQualifiers.None, null, false);
	}
  break;
case 183:
  case_183();
  break;
case 184:
  case_184();
  break;
case 185:
  case_185();
  break;
case 186:
  case_186();
  break;
case 187:
#line 744 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new ExpressionInitializer((Expression)yyVals[0+yyTop]);
	}
  break;
case 188:
#line 748 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 189:
#line 752 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
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
#line 792 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new InitializerDesignation((List<InitializerDesignator>)yyVals[-1+yyTop]);
	}
  break;
case 208:
#line 821 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { StartBlock(GetLocation(yyVals[0+yyTop])); }
  break;
case 209:
#line 824 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = EndBlock(GetLocation(yyVals[0+yyTop]));
	}
  break;
case 210:
#line 825 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { StartBlock(GetLocation(yyVals[0+yyTop])); }
  break;
case 211:
#line 828 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = EndBlock(GetLocation(yyVals[0+yyTop]));
	}
  break;
case 214:
  case_214();
  break;
case 215:
  case_215();
  break;
case 216:
#line 854 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = null;
	}
  break;
case 217:
#line 858 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new ExpressionStatement((Expression)yyVals[-1+yyTop]);
	}
  break;
case 218:
#line 865 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-4+yyTop]));
	}
  break;
case 219:
#line 869 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new IfStatement((Expression)yyVals[-4+yyTop], (Statement)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], GetLocation(yyVals[-6+yyTop]));
	}
  break;
case 221:
#line 877 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new WhileStatement(false, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop]);
	}
  break;
case 222:
#line 881 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new WhileStatement(true, (Expression)yyVals[-2+yyTop], (Statement)yyVals[-5+yyTop]);
	}
  break;
case 223:
#line 885 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new ForStatement((ExpressionStatement)yyVals[-3+yyTop], ((ExpressionStatement)yyVals[-2+yyTop]).Expression, (Statement)yyVals[0+yyTop], _currentBlock, GetLocation(yyVals[-5+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 224:
#line 889 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new ForStatement((ExpressionStatement)yyVals[-4+yyTop], ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Expression)yyVals[-2+yyTop], (Statement)yyVals[0+yyTop], _currentBlock, GetLocation(yyVals[-6+yyTop]), GetLocation(yyVals[0+yyTop]));
	}
  break;
case 225:
#line 890 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { StartBlock(GetLocation(yyVals[-4+yyTop])); }
  break;
case 226:
  case_226();
  break;
case 227:
#line 896 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  { StartBlock(GetLocation(yyVals[-5+yyTop])); }
  break;
case 228:
  case_228();
  break;
case 232:
#line 911 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new ReturnStatement ();
	}
  break;
case 233:
#line 915 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
  {
		yyVal = new ReturnStatement ((Expression)yyVals[-1+yyTop]);
	}
  break;
case 234:
  case_234();
  break;
case 235:
  case_235();
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
void case_15()
#line 87 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = new List<Expression>();
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_16()
#line 93 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = (List<Expression>)yyVals[-2+yyTop];
		l.Add((Expression)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_62()
#line 276 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		if (yyVals[-1+yyTop] is RelationalOp && ((RelationalOp)yyVals[-1+yyTop]) == RelationalOp.Equals) {
			yyVal = new AssignExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
		}
		else {
			var left = (Expression)yyVals[-2+yyTop]; 
			yyVal = new AssignExpression(left, new BinaryExpression (left, (Binop)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]));
		}
	}

void case_77()
#line 318 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new MultiDeclaration();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_78()
#line 324 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new MultiDeclaration();
		d.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		d.InitDeclarators = (List<InitDeclarator>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_79()
#line 334 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.StorageClassSpecifier = (StorageClassSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_80()
#line 340 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.StorageClassSpecifier = ds.StorageClassSpecifier | (StorageClassSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_81()
#line 346 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[0+yyTop]);
		yyVal = ds;
	}

void case_82()
#line 352 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeSpecifiers.Add((TypeSpecifier)yyVals[-1+yyTop]);
		yyVal = ds;
	}

void case_83()
#line 358 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.TypeQualifiers = (TypeQualifiers)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_84()
#line 364 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.TypeQualifiers = (TypeQualifiers)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_85()
#line 370 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = new DeclarationSpecifiers();
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[0+yyTop];
		yyVal = ds;
	}

void case_86()
#line 376 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var ds = (DeclarationSpecifiers)yyVals[0+yyTop];
		ds.FunctionSpecifier = (FunctionSpecifier)yyVals[-1+yyTop];
		yyVal = ds;
	}

void case_87()
#line 385 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var idl = new List<InitDeclarator>();
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_88()
#line 391 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var idl = (List<InitDeclarator>)yyVals[-2+yyTop];
		idl.Add((InitDeclarator)yyVals[0+yyTop]);
		yyVal = idl;
	}

void case_89()
#line 400 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[0+yyTop];
		yyVal = id;
	}

void case_90()
#line 406 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var id = new InitDeclarator();
		id.Declarator = (Declarator)yyVals[-2+yyTop];
		id.Initializer = (Initializer)yyVals[0+yyTop];
		yyVal = id;
	}

void case_141()
#line 512 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
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
#line 556 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDecl>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_151()
#line 563 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = new List<ParameterDecl>();
		foreach (var n in (List<string>)yyVals[-1+yyTop]) {
			d.Parameters.Add(new ParameterDecl(n));
		}
		yyVal = d;
	}

void case_152()
#line 573 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDecl>();
		yyVal = d;
	}

void case_163()
#line 608 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = (List<ParameterDecl>)yyVals[-2+yyTop];
		l.Add(new VarParameter());
		yyVal = l;
	}

void case_164()
#line 617 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = new List<ParameterDecl>();
		l.Add((ParameterDecl)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_165()
#line 623 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = (List<ParameterDecl>)yyVals[-2+yyTop];
		l.Add((ParameterDecl)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_166()
#line 632 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var p = new ParameterDecl((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_167()
#line 637 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var p = new ParameterDecl((DeclarationSpecifiers)yyVals[-1+yyTop], (Declarator)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_168()
#line 642 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var p = new ParameterDecl((DeclarationSpecifiers)yyVals[0+yyTop]);
		yyVal = p;
	}

void case_169()
#line 650 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = new List<string>();
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_170()
#line 656 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = (List<string>)yyVals[-2+yyTop];
		l.Add((yyVals[0+yyTop]).ToString());
		yyVal = l;
	}

void case_176()
#line 676 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
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

void case_183()
#line 712 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-1+yyTop];
		d.Parameters = new List<ParameterDecl>();
		yyVal = d;
	}

void case_184()
#line 719 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new FunctionDeclarator();
		d.Parameters = (List<ParameterDecl>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_185()
#line 725 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-2+yyTop];
		d.Parameters = new List<ParameterDecl> ();
		yyVal = d;
	}

void case_186()
#line 732 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var d = new FunctionDeclarator();
		d.InnerDeclarator = (Declarator)yyVals[-3+yyTop];
		d.Parameters = (List<ParameterDecl>)yyVals[-1+yyTop];
		yyVal = d;
	}

void case_190()
#line 757 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_191()
#line 764 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = new StructuredInitializer();
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_192()
#line 772 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-2+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_193()
#line 779 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = (StructuredInitializer)yyVals[-3+yyTop];
		var i = (Initializer)yyVals[0+yyTop];
		i.Designation = (InitializerDesignation)yyVals[-1+yyTop];
		l.Add(i);
		yyVal = l;
	}

void case_214()
#line 838 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop], _currentBlock);
		yyVal = null;
	}

void case_215()
#line 843 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		if (yyVal != null) {
			_currentBlock.AddStatement((Statement)yyVals[0+yyTop]);
		}
	}

void case_226()
#line 891 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		AddDeclaration(yyVals[-4+yyTop], _currentBlock);
		var b = EndBlock(GetLocation(yyVals[0+yyTop]));		
		yyVal = new ForStatement(b, ((ExpressionStatement)yyVals[-3+yyTop]).Expression, (Statement)yyVals[0+yyTop]);
	}

void case_228()
#line 897 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		AddDeclaration(yyVals[-5+yyTop], _currentBlock);
		var b = EndBlock(GetLocation(yyVals[-1+yyTop]));		
		yyVal = new ForStatement(b, ((ExpressionStatement)yyVals[-4+yyTop]).Expression, (Expression)yyVals[-3+yyTop], (Statement)yyVals[0+yyTop]);
	}

void case_234()
#line 920 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_235()
#line 925 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		AddDeclaration(yyVals[0+yyTop]);
		yyVal = _tu;
	}

void case_238()
#line 938 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-3+yyTop];
		f.Declarator = (Declarator)yyVals[-2+yyTop];
		f.ParameterDeclarations = (List<Declaration>)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_239()
#line 947 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var f = new FunctionDefinition();
		f.Specifiers = (DeclarationSpecifiers)yyVals[-2+yyTop];
		f.Declarator = (Declarator)yyVals[-1+yyTop];
		f.Body = (Block)yyVals[0+yyTop];
		yyVal = f;
	}

void case_240()
#line 958 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = new List<Declaration>();
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

void case_241()
#line 964 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"
{
		var l = (List<Declaration>)yyVals[-1+yyTop];
		l.Add((Declaration)yyVals[0+yyTop]);
		yyVal = l;
	}

#line default
   static readonly short [] yyLhs  = {              -1,
    1,    1,    1,    1,    3,    3,    3,    3,    3,    3,
    3,    3,    3,    3,    4,    4,    8,    8,    8,    8,
    8,    8,    8,    8,   10,   10,   10,   10,    9,    9,
   11,   11,   11,   11,   12,   12,   12,   13,   13,   13,
   14,   14,   14,   14,   14,   15,   15,   15,   16,   16,
   17,   17,   18,   18,   19,   19,   20,   20,   21,   21,
    7,    7,   22,   22,   22,   22,   22,   22,   22,   22,
   22,   22,   22,    2,    2,   23,   24,   24,   25,   25,
   25,   25,   25,   25,   25,   25,   26,   26,   31,   31,
   27,   27,   27,   27,   27,   28,   28,   28,   28,   28,
   28,   28,   28,   28,   28,   28,   28,   28,   28,   28,
   34,   34,   34,   36,   36,   37,   37,   38,   39,   39,
   39,   39,   40,   40,   41,   41,   41,   35,   35,   35,
   35,   35,   42,   42,   43,   43,   30,   32,   32,   45,
   45,   45,   45,   45,   45,   45,   45,   45,   45,   45,
   45,   45,   44,   44,   44,   44,   46,   46,   29,   29,
   29,   47,   47,   49,   49,   50,   50,   50,   48,   48,
    5,    5,   51,   51,   51,   52,   52,   52,   52,   52,
   52,   52,   52,   52,   52,   52,   33,   33,   33,    6,
    6,    6,    6,   53,   54,   54,   55,   55,   56,   56,
   56,   56,   56,   56,   57,   57,   57,   63,   58,   65,
   58,   64,   64,   66,   66,   59,   59,   60,   60,   60,
   61,   61,   61,   61,   67,   61,   68,   61,   62,   62,
   62,   62,   62,    0,    0,   69,   69,   70,   70,   71,
   71,
  };
   static readonly short [] yyLen = {           2,
    1,    1,    1,    3,    1,    4,    3,    4,    3,    3,
    2,    2,    6,    7,    1,    3,    1,    2,    2,    2,
    2,    2,    2,    4,    1,    1,    1,    1,    1,    4,
    1,    3,    3,    3,    1,    3,    3,    1,    3,    3,
    1,    3,    3,    3,    3,    1,    3,    3,    1,    3,
    1,    3,    1,    3,    1,    3,    1,    3,    1,    5,
    1,    3,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    3,    1,    2,    3,    1,    2,
    1,    2,    1,    2,    1,    2,    1,    3,    1,    3,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    5,    4,    2,    1,    1,    1,    2,    3,    2,    1,
    2,    1,    1,    3,    1,    2,    3,    4,    5,    5,
    6,    2,    1,    3,    1,    3,    1,    2,    1,    1,
    3,    5,    4,    4,    6,    6,    5,    4,    3,    4,
    4,    3,    1,    2,    2,    3,    1,    2,    1,    1,
    1,    1,    3,    1,    3,    2,    2,    1,    1,    3,
    1,    2,    1,    1,    2,    3,    2,    3,    3,    4,
    3,    4,    2,    3,    3,    4,    1,    3,    4,    1,
    2,    3,    4,    2,    1,    2,    3,    2,    1,    1,
    1,    1,    1,    1,    3,    4,    3,    0,    3,    0,
    4,    1,    2,    1,    1,    1,    2,    5,    7,    5,
    5,    7,    6,    7,    0,    7,    0,    8,    3,    2,
    2,    2,    3,    1,    2,    1,    1,    4,    3,    1,
    2,
  };
   static readonly short [] yyDefRed = {            0,
  110,   91,   92,   93,   94,   95,  137,  160,   97,   98,
   99,  100,  103,  104,  101,  102,  159,  161,   96,  105,
  106,  107,  114,  115,    0,    0,  237,    0,    0,    0,
    0,    0,  108,  109,    0,  234,  236,    0,    0,  235,
  140,    0,    0,   77,    0,   87,    0,    0,    0,   80,
   82,   84,   86,    0,    0,    0,    0,    0,  133,    0,
  157,  155,    0,    0,   78,    0,    0,  240,    0,  239,
    0,    0,    0,    0,    0,    0,    0,    0,  116,    0,
    0,    0,  128,    0,  141,  158,  156,   88,    0,    0,
    0,    1,    2,    3,    0,    0,    0,    0,    0,    0,
    0,   25,   26,   27,   28,    5,    0,  187,    0,   31,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   61,   90,  241,  238,  169,  152,    0,    0,    0,
    0,  164,    0,  149,    0,    0,    0,    0,  119,  121,
  112,  117,    0,    0,    0,  123,  129,    0,   29,   76,
  136,  130,  134,  209,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  216,    0,   74,  214,
  215,  199,  200,  201,  202,  203,  204,    0,  212,    0,
   23,    0,   18,   19,    0,    0,    0,    0,    0,    0,
  190,    0,    0,  195,   20,   21,    0,   11,   12,    0,
    0,    0,   64,   65,   66,   67,   68,   69,   70,   71,
   72,   73,   63,    0,   22,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  166,    0,  167,    0,
  150,  151,    0,    0,    0,  148,  144,    0,  143,    0,
    0,  111,  126,    0,    0,  118,  131,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  230,  231,  232,    0,
    0,  217,  211,  213,    0,    0,    4,    0,    0,    0,
  172,    0,  198,  188,    0,  191,  194,  196,   10,    7,
    0,   15,    0,    9,   62,   32,   33,   34,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  183,    0,    0,  177,    0,    0,
    0,    0,    0,  170,  163,  165,    0,    0,  147,  142,
  127,  124,  205,    0,  207,    0,    0,    0,    0,    0,
    0,  229,  233,   75,    0,    0,    0,   30,  197,  189,
  192,    0,    8,    0,    6,    0,  184,  176,  181,  178,
  185,    0,  179,    0,    0,  145,  146,  206,    0,    0,
    0,    0,    0,    0,    0,  193,   16,   60,  186,  182,
  180,    0,  220,  221,    0,  225,    0,    0,    0,   13,
    0,    0,    0,    0,  227,  223,    0,   14,  219,  222,
  226,    0,  224,  228,
  };
  protected static readonly short [] yyDgoto  = {            26,
  106,  168,  107,  291,  186,  190,  169,  109,  110,  111,
  112,  113,  114,  115,  116,  117,  118,  119,  120,  121,
  122,  214,  151,   27,   69,   45,   29,   30,   31,   32,
   46,   60,  191,   33,   34,   35,   78,   79,   80,  145,
  146,   58,   59,   48,   49,   63,  316,  130,  131,  132,
  317,  240,  192,  193,  194,  171,  172,  173,  174,  175,
  176,  177,   90,  178,   91,  179,  394,  402,   36,   37,
   71,
  };
  protected static readonly short [] yySindex = {         2522,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  -84, 2522,    0,   25, 2522, 2522,
 2522, 2522,    0,    0,  -71,    0,    0,  -59, -186,    0,
    0,  -13,  -39,    0,  146,    0, 1113,  -22,   72,    0,
    0,    0,    0,   -4, 2548, -186,   81,   13,    0,  131,
    0,    0,  -39,  -13,    0,    0, 2094,    0,   25,    0,
 1433,   72, 1077,  483, 2548, 2548, 2548,  326,    0,   39,
   36, 2424,    0,  -81,    0,    0,    0,    0,  118,   92,
  255,    0,    0,    0, 2445, 2469, 2469,  631, 2154, 2424,
 2424,    0,    0,    0,    0,    0,  -25,    0,  184,    0,
 2424,  338,  -26,  -23,  312,  -69,  168,  134,  119,   -9,
  -49,    0,    0,    0,    0,    0,    0,   49,  212,  210,
  228,    0,   88,    0,    5,  191,  995, 2370,    0,    0,
    0,    0, 2424,  232,  153,    0,    0,  -72,    0,    0,
    0,    0,    0,    0,  241, 2424,  243,  269,  271,  277,
  396,  316,   58,  293,  310, 1295,    0,  154,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   45,    0,  631,
    0,  631,    0,    0,  250,  341,  161, 2424,  132,   55,
    0, 2094,   98,    0,    0,    0,  158,    0,    0, 2171,
 2424,  171,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2424,    0, 2424, 2424, 2424, 2424, 2424,
 2424, 2424, 2424, 2424, 2424, 2424, 2424, 2424, 2424, 2424,
 2424, 2424, 2424, 2424,  863, 2185,    0,    9,    0,  124,
    0,    0,  178, 2464,  652,    0,    0, 2424,    0, 2196,
  295,    0,    0, 2424,   39,    0,    0,  396,  361,  396,
 2424, 2424, 2424,  129,  544,  378,    0,    0,    0,  211,
 2424,    0,    0,    0,  407,  413,    0, 2217,  720,  125,
    0,  373,    0,    0, 2117,    0,    0,    0,    0,    0,
  275,    0,   -7,    0,    0,    0,    0,    0,  338,  338,
  -26,  -26,  -23,  -23,  -23,  -23,  312,  312,  -69,  168,
  134,  119,   -9,   85,    0,  426,  427,    0, 2241,  401,
  124,  929, 2265,    0,    0,    0,  405,  416,    0,    0,
    0,    0,    0,  396,    0,  335,  399,  466,  480, 2283,
 2283,    0,    0,    0,  408,  408, 2154,    0,    0,    0,
    0, 2094,    0, 2424,    0, 2424,    0,    0,    0,    0,
    0,  493,    0, 2326,  431,    0,    0,    0,  396,  396,
  396, 2424, 2345, 2382,   62,    0,    0,    0,    0,    0,
    0,  224,    0,    0,  467,    0,  486,  396,  488,    0,
 2140,  396,  503,  396,    0,    0,  396,    0,    0,    0,
    0,  396,    0,    0,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  -31,  116,
  990, 1082,    0,    0,    0,    0,    0,  695,    0,    0,
    0,    0,   18,    0,    0,    0,  230,    0,  904,    0,
    0,    0,    0,  811,    0,    0,   69,    0,    0,    0,
    0,    0,   61,    0,    0,  108,    0,    0,    0,    0,
    0, 1013,    0,    0,    0,  -17,  605,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  230,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1387,    0, 1478,    0,
    0, 1501, 1715, 1784, 1899, 2024, 1587, 2474,  547,   10,
 1794,    0,    0,    0,    0,    0,    0,  492,    0,    0,
  525,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  233,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1229,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  534,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  549,    0,  551,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  539,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1524, 1547,
 1738, 1761, 1807, 1830, 1853, 1876, 1978, 2001, 2047, 2472,
 2565, 2518,   52,    0,    0,    0,    0,    0,    0,    0,
  563,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1424,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  187,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,
  };
  protected static readonly short [] yyGindex = {            0,
    0,  -52,    0,    0,   91,  234,  -67,  501,  -95,    0,
   59,  162,  207,  225,  356,  364,  368,  379,  367,    0,
  -80,    0,  -61,  -16,    4,    0,    0,  716, 2558,    0,
  550,   -8,  -66,    0,    0,    0,  538,   16,   41,    0,
  380,  577,   40,  -21,  -36,   63,  -65,    0,    0,  390,
    8, -146, -259,    0,  444,  490,    0,  137, -149,    0,
    0,    0,    0,    0,    0,  460,    0,    0,  613,    0,
    0,
  };
  protected static readonly short [] yyTable = {           108,
  123,  150,   43,   28,  195,  196,  136,  129,   79,   79,
   79,   72,   79,  234,  200,  215,  219,   42,  220,   47,
  202,   62,  120,  120,  120,  352,   42,   79,   43,   28,
   68,  108,   50,   51,   52,   53,  271,  105,   39,  196,
  120,   87,  100,  152,   98,  185,  101,  102,  235,  103,
   57,   55,  257,   57,  124,   89,   84,  153,  153,   79,
   89,  153,  150,   56,   42,  201,   43,   57,   57,  251,
   57,  144,   57,  120,  170,  150,  128,  105,   42,  148,
   43,  253,  100,   44,   98,  355,  101,  102,  235,  103,
   43,  321,   58,  142,  259,   58,  143,  246,  285,  236,
  154,  154,   57,  167,  154,  391,  238,  150,  153,   58,
   58,   73,  135,  270,   58,  341,  139,  140,   75,  237,
  296,  297,  298,  153,  108,  286,  282,  185,  271,  185,
  104,  352,  292,  321,   57,  239,  137,   83,  187,  236,
  210,   82,  356,  189,   58,  210,  295,  210,  293,  210,
  210,  154,  210,  142,  196,   81,   81,   81,  287,   81,
  147,  170,   74,  322,  279,  280,  210,   66,  320,  273,
  104,   85,   38,  150,   81,   57,   58,  327,   67,  284,
  328,  314,  348,   70,   57,   54,  390,  153,  188,   64,
  373,  374,  331,  135,  281,  245,  255,  271,  227,  228,
  279,   72,   43,  344,   65,  229,   81,  125,  336,  337,
  338,  256,  272,  238,  323,  236,  154,  108,  351,  218,
  187,  233,  187,  196,  218,   79,  218,  230,  218,  218,
  210,  218,  208,  210,   41,  197,  198,  199,  128,  120,
  221,  222,  231,   41,  213,  218,  144,  128,  340,    8,
  242,  236,  241,  243,  271,  365,  362,  280,   17,   18,
  232,   92,   93,   94,   95,   41,   96,   97,  196,  343,
  275,  244,  276,   89,  153,  378,  125,  299,  300,  108,
   57,   41,  128,  247,  108,  376,  377,  105,   89,  254,
  277,  125,  100,  271,   98,   41,  101,  102,  258,  103,
  260,  155,   93,   94,   95,   41,   96,   97,  261,  218,
  262,  218,  218,  167,  266,  353,  263,  154,  354,  385,
  387,  389,   58,  108,  351,  128,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,
   25,  267,  156,  157,  158,  265,  159,  160,  161,  162,
  163,  164,  165,  166,  210,  210,  210,  210,  268,  210,
  210,  225,   81,  226,  218,  369,    8,   66,  271,  216,
  104,  278,  301,  302,  217,   17,   18,  330,  283,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,  289,  210,  210,  210,  334,  210,
  210,  210,  210,  210,  210,  210,  210,  294,  105,  303,
  304,  305,  306,  100,  324,   98,  342,  101,  102,  370,
  103,  339,  271,  218,  218,  218,  218,  345,  218,  218,
  141,  307,  308,  346,  167,  203,  204,  205,  206,  207,
  208,  209,  210,  211,  212,  349,  357,  358,  218,  218,
  218,  218,  218,  218,  218,  218,  218,  218,  218,  218,
  218,  218,  218,  218,  218,  218,  218,  218,  218,  218,
  218,  218,  218,  360,  218,  218,  218,  366,  218,  218,
  218,  218,  218,  218,  218,  218,  371,  393,  367,  271,
  271,  155,   93,   94,   95,  105,   96,   97,   66,  372,
  100,  104,   98,  381,  135,  102,  395,  103,  397,  271,
  347,  271,  168,  379,  392,  168,    1,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,
   25,  400,  156,  157,  158,  162,  159,  160,  161,  162,
  163,  164,  165,  166,  171,  134,  105,  223,  224,  173,
  375,  100,  149,   98,  309,  101,  102,   55,  103,  173,
   55,  174,  173,  310,  174,  181,  183,  184,  311,  313,
  149,  149,  167,  175,   55,   55,  175,    1,  104,   55,
  312,  149,  138,   88,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,
   24,   25,   81,  326,  332,  149,  288,  274,   40,   55,
    0,    0,    0,  149,  122,  122,  122,    0,    0,    0,
  264,    0,  155,   93,   94,   95,  149,   96,   97,    0,
    0,    0,  122,  105,    0,    0,    0,    0,  100,  104,
   98,   55,  101,  102,    0,  103,    0,    0,    0,    0,
    0,    0,    0,    0,  105,    0,    0,    0,  149,  100,
    0,   98,    0,  101,  102,  122,  103,    0,    0,    0,
    0,    0,    0,  156,  157,  158,    0,  159,  160,  161,
  162,  163,  164,  165,  166,    0,  149,  149,  149,  149,
  149,  149,  149,  149,  149,  149,  149,  149,  149,  149,
  149,  149,  149,  149,  132,  132,  132,    0,  132,   92,
   93,   94,   95,    0,   96,   97,    0,  333,    0,  335,
  149,    0,  132,  132,  149,    0,  104,    0,    0,  279,
  315,   43,    0,    0,    0,    0,    0,  133,    0,    0,
   76,    8,    0,    0,    0,    0,    0,  104,  149,    0,
   17,   18,    0,    0,    0,  132,    0,    0,    0,    0,
   76,   76,   76,   76,    0,    0,    0,    0,    0,    0,
   92,   93,   94,   95,    0,   96,   97,    0,    0,    0,
  236,    0,    0,   76,    0,    0,   55,   55,    0,  149,
    0,    0,    0,  368,    0,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
  113,  113,  113,   76,  113,    0,  149,    0,  382,  383,
  384,  122,    0,    0,  149,    0,    0,    0,  113,  113,
    0,    0,    0,    0,    0,    0,    0,  396,    0,    0,
    0,  399,    0,  401,    0,    0,  403,   92,   93,   94,
   95,  404,   96,   97,    0,   76,    0,   76,    0,    0,
    0,  113,  235,  315,   43,    0,    0,    0,   92,   93,
   94,   95,    1,   96,   97,    0,    0,    0,    0,    8,
    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,
   19,   20,   21,   22,   23,   24,   25,    0,    0,    0,
    8,    0,    0,    0,  139,    0,    0,  139,    0,   17,
   18,  132,    0,  236,    0,    0,    0,    0,    0,    0,
    0,  139,  139,    0,  139,    0,    0,    0,    0,  361,
    0,    0,    0,    0,    0,    0,  132,  132,  132,  132,
  132,  132,  132,  132,  132,  132,  132,  132,  132,  132,
  132,  132,  132,  132,  132,  132,  132,  132,  132,  132,
  132,    1,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,   24,   25,  139,  105,    0,   83,
   83,   83,  100,   83,   98,    0,  250,  102,    0,  103,
    0,    0,    0,    0,    0,    0,    0,    0,   83,    0,
    0,    0,    0,  138,    0,    0,  138,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  113,    0,    0,
  138,  138,    0,  138,    0,    0,    0,    0,    0,    0,
   83,    0,    0,    0,    0,    0,    0,  249,    0,    0,
    0,    0,  113,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,  113,
  113,  113,  113,  113,  113,  113,  113,  127,    0,   41,
  104,   85,   85,   85,    0,   85,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  138,    0,    0,    0,    0,
   85,    0,    0,    0,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,    0,
    0,    0,   85,   67,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  139,  139,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
  139,  139,  139,  139,  139,  139,  139,  139,  139,  139,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,
   21,   22,   23,   24,   25,   66,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   83,    0,    0,    0,
    0,   92,   93,   94,   95,    0,   96,   97,    0,    0,
    0,    0,    0,    0,    0,    1,    1,    0,    1,    0,
    1,    1,    1,    1,    1,    1,    0,    0,    0,  248,
    0,    0,    0,    8,    0,    0,    0,    1,    1,    1,
    1,    1,   17,   18,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,  138,
  138,  138,  138,  138,  138,  138,  138,  138,  138,    1,
    0,    0,    1,    0,    0,    0,    0,  105,    0,    0,
    0,    0,  100,  126,   98,    0,  101,  102,   85,  103,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    1,  269,    0,    0,    0,    0,    1,    2,
    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,
   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,
   23,   24,   25,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,    0,
  104,    0,    0,   17,   17,    0,    0,   17,   17,   17,
   17,   17,    0,   17,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   17,   17,   17,   17,   17,   17,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   24,   24,    0,    0,   24,   24,   24,   24,   24,    0,
   24,    0,    0,    0,    0,    0,    0,    0,    0,   17,
   17,   24,   24,   24,   24,   24,   24,    0,    0,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
   17,   17,    0,    0,   29,   29,   24,   24,   29,   29,
   29,   29,   29,    0,   29,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   29,   29,   29,   35,   29,
   29,   35,    0,   35,   35,   35,    0,   24,   24,    0,
    0,   92,   93,   94,   95,   66,   96,   97,   35,   35,
   35,   36,   35,   35,   36,    0,   36,   36,   36,    0,
   29,   29,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   36,   36,   36,   37,   36,   36,   37,    0,   37,
   37,   37,    0,   35,   35,    0,    0,    0,    0,    0,
    0,   29,   29,    0,   37,   37,   37,    0,   37,   37,
    0,    0,    0,    0,    0,    0,   36,   36,    0,    0,
    0,    0,    0,    0,   35,   35,    0,   51,    0,    0,
   51,    0,    0,    0,    0,    0,    0,    0,    0,   37,
   37,    0,    0,    0,   51,   51,    0,   36,   36,   51,
   17,   17,   17,   17,   17,   17,   17,   17,   17,   17,
   17,   17,   17,   17,   17,   17,   17,   17,    0,    0,
   37,   37,    0,    0,    0,    0,    0,    0,    0,   51,
   51,    0,    0,    0,    0,    0,    0,   24,   24,   24,
   24,   24,   24,   24,   24,   24,   24,   24,   24,   24,
   24,   24,   24,   24,   24,    0,    0,    0,    0,    0,
   51,   51,    0,    0,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,   18,   19,   20,   21,   22,   23,   24,   25,    0,
    0,   29,   29,   29,   29,   29,   29,   29,   29,    0,
    0,    0,   38,    0,    0,   38,    0,    0,   38,    0,
    0,    0,    0,    0,   35,   35,   35,   35,   35,   35,
   35,   35,   38,   38,   38,   39,   38,   38,   39,    0,
    0,   39,    0,    0,    0,    0,    0,   36,   36,   36,
   36,   36,   36,   36,   36,   39,   39,   39,   40,   39,
   39,   40,    0,    0,   40,    0,    0,   38,   38,    0,
   37,   37,   37,   37,   37,   37,   37,   37,   40,   40,
   40,   41,   40,   40,   41,    0,    0,   41,    0,    0,
   39,   39,    0,    0,   59,    0,    0,   59,   38,   38,
    0,   41,   41,   41,   44,   41,   41,   44,    0,    0,
   44,   59,   59,   40,   40,    0,   51,   51,    0,    0,
    0,   39,   39,    0,   44,   44,   44,   45,   44,   44,
   45,    0,    0,   45,    0,    0,   41,   41,    0,    0,
    0,    0,    0,    0,   40,   40,   59,   45,   45,   45,
   42,   45,   45,   42,    0,    0,   42,    0,    0,   44,
   44,    0,    0,    0,    0,    0,    0,   41,   41,    0,
   42,   42,   42,   43,   42,   42,   43,    0,   59,   43,
    0,    0,   45,   45,    0,    0,    0,    0,    0,    0,
   44,   44,    0,   43,   43,   43,   46,   43,   43,   46,
    0,    0,   46,    0,    0,   42,   42,    0,    0,    0,
    0,    0,    0,   45,   45,    0,   46,   46,    0,    0,
    0,   46,    0,    0,    0,    0,    0,    0,   43,   43,
    0,    0,    0,    0,    0,    0,   42,   42,   38,   38,
   38,   38,   38,   38,   38,   38,    0,    0,    0,    0,
    0,   46,   46,    0,    0,    0,    0,    0,    0,   43,
   43,   39,   39,   39,   39,   39,   39,   39,   39,    0,
    0,    0,    0,    0,    0,   47,    0,    0,   47,    0,
    0,   47,   46,   46,   40,   40,   40,   40,   40,   40,
   40,   40,    0,    0,    0,   47,   47,    0,   48,    0,
   47,   48,    0,    0,   48,    0,    0,    0,    0,   41,
   41,   41,   41,   41,   41,    0,    0,    0,   48,   48,
    0,   49,    0,   48,   49,    0,    0,   49,    0,    0,
   47,   47,   44,   44,   44,   44,   44,   44,    0,    0,
    0,   49,   49,    0,   50,    0,   49,   50,    0,    0,
   50,    0,    0,   48,   48,   45,   45,   45,   45,   45,
   45,   47,   47,    0,   50,   50,    0,    0,    0,   50,
    0,    0,    0,    0,    0,    0,   49,   49,   42,   42,
   42,   42,   42,   42,   48,   48,  105,    0,    0,    0,
    0,  100,    0,   98,    0,  101,  102,    0,  103,   50,
   50,   43,   43,   43,   43,   43,   43,   49,   49,  105,
    0,    0,    0,    0,  100,    0,   98,    0,  101,  102,
    0,  103,  189,    0,    0,    0,   46,   46,   46,   46,
   50,   50,  105,    0,    0,    0,    0,  100,    0,   98,
    0,  101,  102,    0,  103,  189,  105,    0,    0,    0,
    0,  100,    0,   98,    0,  101,  102,    0,  103,  189,
    0,    0,    0,  105,    0,    0,    0,  188,  100,    0,
   98,  290,  101,  102,    0,  103,   99,  105,    0,  104,
    0,    0,  100,    0,   98,    0,  319,  102,  105,  103,
  188,    0,    0,  100,    0,   98,    0,  101,  102,   99,
  103,  350,  104,    0,  188,   47,   47,   47,   47,  105,
    0,    0,    0,    0,  100,    0,   98,    0,  101,  102,
    0,  103,   99,    0,  398,  104,    0,    0,   48,   48,
   48,   48,    0,  105,    0,    0,   99,  318,  100,  104,
   98,    0,  101,  102,    0,  103,    0,    0,  329,    0,
    0,    0,    0,   49,   49,    0,  104,  105,    0,    0,
    0,    0,  100,    0,   98,    0,  364,  102,    0,  103,
  104,    0,    0,    0,    0,  105,   50,   50,    0,    0,
  100,  104,   98,    0,  101,  102,    0,  103,    0,    0,
    0,    0,    0,  359,    0,    0,    0,    0,    0,  347,
    0,  167,  104,    0,    0,    0,    0,    0,    0,    0,
   92,   93,   94,   95,    0,   96,   97,  363,  105,    0,
    0,    0,    0,  100,    0,   98,  104,  101,  102,    0,
  103,    0,    0,   92,   93,   94,   95,  105,   96,   97,
    0,    0,  100,    0,   98,  386,  101,  102,    0,  103,
  104,    0,    0,    0,    0,    0,   92,   93,   94,   95,
    0,   96,   97,    0,    0,    0,    0,    0,  104,    0,
   92,   93,   94,   95,  105,   96,   97,    0,  380,  100,
    0,   98,  388,  101,  102,    0,  103,   92,   93,   94,
   95,    0,   96,   97,    0,    0,    0,    0,    0,    0,
    0,   92,   93,   94,   95,    0,   96,   97,    0,    0,
    0,  104,   92,   93,   94,   95,  105,   96,   97,    0,
    0,  100,    0,   98,    0,  101,  102,    0,  103,    0,
  104,    0,    0,   92,   93,   94,   95,  105,   96,   97,
    0,    0,  100,    0,  180,    0,  101,  102,    0,  103,
    0,    0,    0,    0,  252,    0,    0,   92,   93,   94,
   95,  105,   96,   97,    0,    0,  100,  104,  182,    0,
  101,  102,   52,  103,   53,   52,    0,   53,    0,    0,
    0,   92,   93,   94,   95,    0,   96,   97,    0,   52,
   52,   53,   53,    0,   52,    0,   53,    0,    0,   92,
   93,   94,   95,    0,   96,   97,    0,    0,    0,  104,
    0,    0,    0,    0,    0,    0,    0,    0,   56,    0,
    0,   56,    0,    0,   52,   52,   53,    0,    0,    0,
  104,    0,    0,    0,    0,   56,   56,    0,    0,    0,
   56,    0,   92,   93,   94,   95,    0,   96,   97,    0,
    0,    0,    0,    0,  104,   52,   52,   53,   53,    0,
   61,   92,   93,   94,   95,   54,   96,   97,   54,    0,
   56,    0,   77,    0,    0,    0,    0,    0,    0,    0,
   86,    0,   54,   54,    0,    0,    0,   54,    0,    0,
    0,   61,   77,   77,   77,   77,    0,    0,   92,   93,
   94,   95,   56,   96,   97,    0,    0,    0,    0,    0,
    0,    1,    0,    0,    0,   77,    0,   54,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,
   20,   21,   22,   23,   24,   25,    0,    0,    0,    0,
   92,   93,   94,   95,    0,   96,   97,    0,   54,   54,
   61,    0,    0,    0,   86,   77,    0,    0,    0,    0,
    0,   92,   93,   94,   95,    0,   96,   97,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   92,   93,   94,   95,    0,
   96,   97,    0,    0,    0,    0,    0,   77,    0,   77,
    0,   52,   52,   53,   53,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,
  325,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   56,   56,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   86,    1,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
   18,   19,   20,   21,   22,   23,   24,   25,    0,    1,
    0,    0,    0,    0,   54,   54,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,
   22,   23,   24,   25,
  };
  protected static readonly short [] yyCheck = {            67,
   67,   82,   42,    0,  100,  101,   74,   73,   40,   41,
   42,   48,   44,   63,   40,  111,   43,   40,   45,   28,
   46,   43,   40,   41,   42,  285,   40,   59,   42,   26,
   47,   99,   29,   30,   31,   32,   44,   33,  123,  135,
   58,   63,   38,  125,   40,   98,   42,   43,   40,   45,
   41,  123,  125,   44,   71,   64,   44,   40,   41,   91,
   69,   44,  143,  123,   40,   91,   42,   58,   59,  137,
  257,   80,   63,   91,   91,  156,   73,   33,   40,   44,
   42,  143,   38,   59,   40,   93,   42,   43,   40,   45,
   42,  238,   41,   78,  156,   44,   58,   93,   44,   91,
   40,   41,   93,   59,   44,   44,  128,  188,   91,   58,
   59,   40,   44,  166,   63,  265,   76,   77,  123,  128,
  216,  217,  218,   84,  192,  192,  188,  180,   44,  182,
  126,  391,  200,  280,  125,  128,   74,  125,   98,   91,
   33,   61,   58,   46,   93,   38,  214,   40,  201,   42,
   43,   91,   45,  138,  250,   40,   41,   42,   61,   44,
  125,  178,   91,   40,   40,  187,   59,  123,  236,  125,
  126,   41,  257,  254,   59,  257,  125,  245,   61,  125,
  248,  234,  278,   47,  257,  257,  125,  148,   91,   44,
  340,  341,  254,  125,  187,  133,   44,   44,  268,  269,
   40,  238,   42,  271,   59,   38,   91,   71,  261,  262,
  263,   59,   59,  235,   91,   91,  125,  285,  285,   33,
  180,  271,  182,  319,   38,  257,   40,   94,   42,   43,
  123,   45,  125,  126,  257,  261,  262,  263,  235,  257,
  264,  265,  124,  257,   61,   59,  255,  244,  265,  289,
   41,   91,   41,   44,   44,  323,  322,  279,  298,  299,
  270,  257,  258,  259,  260,  257,  262,  263,  364,   59,
  180,   44,  182,   44,  257,  356,   44,  219,  220,  347,
  271,  257,  279,   93,  352,  352,  354,   33,   59,   58,
   41,   59,   38,   44,   40,  257,   42,   43,   58,   45,
   58,  257,  258,  259,  260,  257,  262,  263,   40,  123,
   40,  125,  126,   59,  257,   41,   40,  257,   44,  372,
  373,  374,  271,  391,  391,  322,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,   59,  308,  309,  310,   40,  312,  313,  314,  315,
  316,  317,  318,  319,  257,  258,  259,  260,   59,  262,
  263,   60,  257,   62,   37,   41,  289,  123,   44,   42,
  126,   41,  221,  222,   47,  298,  299,   93,  257,  282,
  283,  284,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  257,  308,  309,  310,   58,  312,
  313,  314,  315,  316,  317,  318,  319,  257,   33,  223,
  224,  225,  226,   38,  257,   40,   59,   42,   43,   41,
   45,  313,   44,  257,  258,  259,  260,   41,  262,  263,
  125,  227,  228,   41,   59,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,   93,   41,   41,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,   93,  308,  309,  310,   93,  312,  313,
  314,  315,  316,  317,  318,  319,   41,   41,   93,   44,
   44,  257,  258,  259,  260,   33,  262,  263,  123,   40,
   38,  126,   40,   93,   42,   43,   41,   45,   41,   44,
  123,   44,   41,   41,  311,   44,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,   59,  308,  309,  310,   41,  312,  313,  314,  315,
  316,  317,  318,  319,   41,   93,   33,  266,  267,   41,
  347,   38,   82,   40,  229,   42,   43,   41,   45,   41,
   44,   41,   44,  230,   44,   95,   96,   97,  231,  233,
  100,  101,   59,   41,   58,   59,   44,  282,  126,   63,
  232,  111,   75,   64,  289,  290,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,   56,  244,  255,  135,  193,  178,   26,   93,
   -1,   -1,   -1,  143,   40,   41,   42,   -1,   -1,   -1,
  161,   -1,  257,  258,  259,  260,  156,  262,  263,   -1,
   -1,   -1,   58,   33,   -1,   -1,   -1,   -1,   38,  126,
   40,  125,   42,   43,   -1,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,  188,   38,
   -1,   40,   -1,   42,   43,   91,   45,   -1,   -1,   -1,
   -1,   -1,   -1,  308,  309,  310,   -1,  312,  313,  314,
  315,  316,  317,  318,  319,   -1,  216,  217,  218,  219,
  220,  221,  222,  223,  224,  225,  226,  227,  228,  229,
  230,  231,  232,  233,   40,   41,   42,   -1,   44,  257,
  258,  259,  260,   -1,  262,  263,   -1,  258,   -1,  260,
  250,   -1,   58,   59,  254,   -1,  126,   -1,   -1,   40,
   41,   42,   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,
   55,  289,   -1,   -1,   -1,   -1,   -1,  126,  278,   -1,
  298,  299,   -1,   -1,   -1,   91,   -1,   -1,   -1,   -1,
   75,   76,   77,   78,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,
   91,   -1,   -1,   98,   -1,   -1,  270,  271,   -1,  319,
   -1,   -1,   -1,  334,   -1,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
   40,   41,   42,  138,   44,   -1,  356,   -1,  369,  370,
  371,  257,   -1,   -1,  364,   -1,   -1,   -1,   58,   59,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  388,   -1,   -1,
   -1,  392,   -1,  394,   -1,   -1,  397,  257,  258,  259,
  260,  402,  262,  263,   -1,  180,   -1,  182,   -1,   -1,
   -1,   91,   40,   41,   42,   -1,   -1,   -1,  257,  258,
  259,  260,  282,  262,  263,   -1,   -1,   -1,   -1,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,   -1,   -1,   -1,
  289,   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,  298,
  299,  257,   -1,   91,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   -1,   61,   -1,   -1,   -1,   -1,   41,
   -1,   -1,   -1,   -1,   -1,   -1,  282,  283,  284,  285,
  286,  287,  288,  289,  290,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  282,  283,  284,  285,  286,  287,  288,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  123,   33,   -1,   40,
   41,   42,   38,   44,   40,   -1,   42,   43,   -1,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   -1,
   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,
   58,   59,   -1,   61,   -1,   -1,   -1,   -1,   -1,   -1,
   91,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,  282,  283,  284,  285,  286,  287,  288,  289,
  290,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,   41,   -1,  257,
  126,   40,   41,   42,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  123,   -1,   -1,   -1,   -1,
   59,   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   -1,
   -1,   -1,   91,   61,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  282,  283,  284,  285,  286,  287,  288,  289,  290,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  123,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   37,   38,   -1,   40,   -1,
   42,   43,   44,   45,   46,   47,   -1,   -1,   -1,  285,
   -1,   -1,   -1,  289,   -1,   -1,   -1,   59,   60,   61,
   62,   63,  298,  299,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   91,
   -1,   -1,   94,   -1,   -1,   -1,   -1,   33,   -1,   -1,
   -1,   -1,   38,  257,   40,   -1,   42,   43,  257,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,   59,   -1,   -1,   -1,   -1,  282,  283,
  284,  285,  286,  287,  288,  289,  290,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   -1,
  126,   -1,   -1,   37,   38,   -1,   -1,   41,   42,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   58,   59,   60,   61,   62,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   37,   38,   -1,   -1,   41,   42,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   58,   59,   60,   61,   62,   63,   -1,   -1,  261,
  262,  263,  264,  265,  266,  267,  268,  269,  270,  271,
  272,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  124,  125,   -1,   -1,   37,   38,   93,   94,   41,   42,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   38,   62,
   63,   41,   -1,   43,   44,   45,   -1,  124,  125,   -1,
   -1,  257,  258,  259,  260,  123,  262,  263,   58,   59,
   60,   38,   62,   63,   41,   -1,   43,   44,   45,   -1,
   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   58,   59,   60,   38,   62,   63,   41,   -1,   43,
   44,   45,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   58,   59,   60,   -1,   62,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   -1,   58,   59,   -1,  124,  125,   63,
  264,  265,  266,  267,  268,  269,  270,  271,  272,  273,
  274,  275,  276,  277,  278,  279,  280,  281,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,  270,  271,  272,  273,  274,  275,  276,
  277,  278,  279,  280,  281,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,  282,  283,  284,  285,  286,  287,
  288,  289,  290,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,   -1,
   -1,  264,  265,  266,  267,  268,  269,  270,  271,   -1,
   -1,   -1,   38,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,  264,  265,  266,  267,  268,  269,
  270,  271,   58,   59,   60,   38,   62,   63,   41,   -1,
   -1,   44,   -1,   -1,   -1,   -1,   -1,  264,  265,  266,
  267,  268,  269,  270,  271,   58,   59,   60,   38,   62,
   63,   41,   -1,   -1,   44,   -1,   -1,   93,   94,   -1,
  264,  265,  266,  267,  268,  269,  270,  271,   58,   59,
   60,   38,   62,   63,   41,   -1,   -1,   44,   -1,   -1,
   93,   94,   -1,   -1,   41,   -1,   -1,   44,  124,  125,
   -1,   58,   59,   60,   38,   62,   63,   41,   -1,   -1,
   44,   58,   59,   93,   94,   -1,  270,  271,   -1,   -1,
   -1,  124,  125,   -1,   58,   59,   60,   38,   62,   63,
   41,   -1,   -1,   44,   -1,   -1,   93,   94,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   93,   58,   59,   60,
   38,   62,   63,   41,   -1,   -1,   44,   -1,   -1,   93,
   94,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   58,   59,   60,   38,   62,   63,   41,   -1,  125,   44,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   58,   59,   60,   38,   62,   63,   41,
   -1,   -1,   44,   -1,   -1,   93,   94,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   58,   59,   -1,   -1,
   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,  264,  265,
  266,  267,  268,  269,  270,  271,   -1,   -1,   -1,   -1,
   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,  264,  265,  266,  267,  268,  269,  270,  271,   -1,
   -1,   -1,   -1,   -1,   -1,   38,   -1,   -1,   41,   -1,
   -1,   44,  124,  125,  264,  265,  266,  267,  268,  269,
  270,  271,   -1,   -1,   -1,   58,   59,   -1,   38,   -1,
   63,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,  266,
  267,  268,  269,  270,  271,   -1,   -1,   -1,   58,   59,
   -1,   38,   -1,   63,   41,   -1,   -1,   44,   -1,   -1,
   93,   94,  266,  267,  268,  269,  270,  271,   -1,   -1,
   -1,   58,   59,   -1,   38,   -1,   63,   41,   -1,   -1,
   44,   -1,   -1,   93,   94,  266,  267,  268,  269,  270,
  271,  124,  125,   -1,   58,   59,   -1,   -1,   -1,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,  266,  267,
  268,  269,  270,  271,  124,  125,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   93,
   94,  266,  267,  268,  269,  270,  271,  124,  125,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   -1,   45,   46,   -1,   -1,   -1,  268,  269,  270,  271,
  124,  125,   33,   -1,   -1,   -1,   -1,   38,   -1,   40,
   -1,   42,   43,   -1,   45,   46,   33,   -1,   -1,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   46,
   -1,   -1,   -1,   33,   -1,   -1,   -1,   91,   38,   -1,
   40,   41,   42,   43,   -1,   45,  123,   33,   -1,  126,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   33,   45,
   91,   -1,   -1,   38,   -1,   40,   -1,   42,   43,  123,
   45,  125,  126,   -1,   91,  268,  269,  270,  271,   33,
   -1,   -1,   -1,   -1,   38,   -1,   40,   -1,   42,   43,
   -1,   45,  123,   -1,  125,  126,   -1,   -1,  268,  269,
  270,  271,   -1,   33,   -1,   -1,  123,   93,   38,  126,
   40,   -1,   42,   43,   -1,   45,   -1,   -1,   93,   -1,
   -1,   -1,   -1,  270,  271,   -1,  126,   33,   -1,   -1,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
  126,   -1,   -1,   -1,   -1,   33,  270,  271,   -1,   -1,
   38,  126,   40,   -1,   42,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,  123,
   -1,   59,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   93,   33,   -1,
   -1,   -1,   -1,   38,   -1,   40,  126,   42,   43,   -1,
   45,   -1,   -1,  257,  258,  259,  260,   33,  262,  263,
   -1,   -1,   38,   -1,   40,   41,   42,   43,   -1,   45,
  126,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,  126,   -1,
  257,  258,  259,  260,   33,  262,  263,   -1,   93,   38,
   -1,   40,   41,   42,   43,   -1,   45,  257,  258,  259,
  260,   -1,  262,  263,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,  126,  257,  258,  259,  260,   33,  262,  263,   -1,
   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
  126,   -1,   -1,  257,  258,  259,  260,   33,  262,  263,
   -1,   -1,   38,   -1,   40,   -1,   42,   43,   -1,   45,
   -1,   -1,   -1,   -1,  125,   -1,   -1,  257,  258,  259,
  260,   33,  262,  263,   -1,   -1,   38,  126,   40,   -1,
   42,   43,   41,   45,   41,   44,   -1,   44,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   58,
   59,   58,   59,   -1,   63,   -1,   63,   -1,   -1,  257,
  258,  259,  260,   -1,  262,  263,   -1,   -1,   -1,  126,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   93,   94,   93,   -1,   -1,   -1,
  126,   -1,   -1,   -1,   -1,   58,   59,   -1,   -1,   -1,
   63,   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,
   -1,   -1,   -1,   -1,  126,  124,  125,  124,  125,   -1,
   43,  257,  258,  259,  260,   41,  262,  263,   44,   -1,
   93,   -1,   55,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   63,   -1,   58,   59,   -1,   -1,   -1,   63,   -1,   -1,
   -1,   74,   75,   76,   77,   78,   -1,   -1,  257,  258,
  259,  260,  125,  262,  263,   -1,   -1,   -1,   -1,   -1,
   -1,  282,   -1,   -1,   -1,   98,   -1,   93,  289,  290,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,   -1,   -1,   -1,   -1,
  257,  258,  259,  260,   -1,  262,  263,   -1,  124,  125,
  133,   -1,   -1,   -1,  137,  138,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,   -1,  262,  263,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,   -1,
  262,  263,   -1,   -1,   -1,   -1,   -1,  180,   -1,  182,
   -1,  270,  271,  270,  271,  282,  283,  284,  285,  286,
  287,  288,  289,  290,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  245,  282,  283,  284,  285,  286,  287,  288,
  289,  290,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,   -1,  282,
   -1,   -1,   -1,   -1,  270,  271,  289,  290,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,
  };

#line 974 "/Users/fak/Projects/Circuit/CLanguage/CLanguage/CParser.jay"

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
 class Token {
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
  public const int AND_OP = 270;
  public const int OR_OP = 271;
  public const int MUL_ASSIGN = 272;
  public const int DIV_ASSIGN = 273;
  public const int MOD_ASSIGN = 274;
  public const int ADD_ASSIGN = 275;
  public const int SUB_ASSIGN = 276;
  public const int LEFT_ASSIGN = 277;
  public const int RIGHT_ASSIGN = 278;
  public const int AND_ASSIGN = 279;
  public const int XOR_ASSIGN = 280;
  public const int OR_ASSIGN = 281;
  public const int TYPE_NAME = 282;
  public const int TYPEDEF = 283;
  public const int EXTERN = 284;
  public const int STATIC = 285;
  public const int AUTO = 286;
  public const int REGISTER = 287;
  public const int INLINE = 288;
  public const int RESTRICT = 289;
  public const int CHAR = 290;
  public const int SHORT = 291;
  public const int INT = 292;
  public const int LONG = 293;
  public const int SIGNED = 294;
  public const int UNSIGNED = 295;
  public const int FLOAT = 296;
  public const int DOUBLE = 297;
  public const int CONST = 298;
  public const int VOLATILE = 299;
  public const int VOID = 300;
  public const int BOOL = 301;
  public const int COMPLEX = 302;
  public const int IMAGINARY = 303;
  public const int STRUCT = 304;
  public const int UNION = 305;
  public const int ENUM = 306;
  public const int ELLIPSIS = 307;
  public const int CASE = 308;
  public const int DEFAULT = 309;
  public const int IF = 310;
  public const int ELSE = 311;
  public const int SWITCH = 312;
  public const int WHILE = 313;
  public const int DO = 314;
  public const int FOR = 315;
  public const int GOTO = 316;
  public const int CONTINUE = 317;
  public const int BREAK = 318;
  public const int RETURN = 319;
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
