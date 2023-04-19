grammar Code;

program: NEWLINE? BEGIN NEWLINE? (statement NEWLINE+)* END NEWLINE? EOF;
variable_dec: declaration* NEWLINE?;
line: (declaration | statement | COMMENT) NEWLINE?;

BEGIN: 'BEGIN CODE';
END: 'END CODE';

declaration: NEWLINE? type IDENTIFIER ('=' expression)? (',' IDENTIFIER ('=' expression)?)* ;
type: 'INT' | 'FLOAT' | 'BOOL' | 'CHAR' | 'STRING';
variable: NEWLINE? type IDENTIFIER ('=' (expression))?;
variable_assignment: NEWLINE? type IDENTIFIER NEWLINE?;
assignment: NEWLINE? IDENTIFIER ('=' IDENTIFIER)* '=' expression NEWLINE?;

display: NEWLINE? 'DISPLAY' ':' expression NEWLINE?;
scan: 'SCAN' ':' IDENTIFIER (',' IDENTIFIER)* NEWLINE?;

BEGIN_IF: 'BEGIN IF';
END_IF: 'END IF';
if_block: 'IF' '(' expression ')' BEGIN_IF line* END_IF else_if_block* else_block? NEWLINE?;
else_if_block: 'ELSE IF' '(' expression ')' BEGIN_IF line* END_IF NEWLINE?;
else_block: 'ELSE' BEGIN_IF line* END_IF NEWLINE?;

BEGIN_WHILE: 'BEGIN WHILE';
END_WHILE: 'END WHILE';
BEGIN_DO_WHILE: 'BEGIN DO WHILE';
END_DO_WHILE: 'END DO WHILE';
BEGIN_FOR_LOOP: 'BEGIN FOR';
END_FOR_LOOP: 'END FOR';
while_loop: 'WHILE' '(' expression ')' BEGIN_WHILE line* END_WHILE NEWLINE?;
do_while_loop: 'DO' BEGIN_DO_WHILE line* END_DO_WHILE 'WHILE' '(' expression ')' NEWLINE?;
for_loop: 'FOR' '(' statement ';' expression ';'  additional')' BEGIN_FOR_LOOP line* END_FOR_LOOP NEWLINE?;

constant: INT | FLOAT | BOOL | CHAR | STRING;
INT: [0-9]+;
FLOAT: [0-9]+('.' [0-9]+)?;
BOOL: '"TRUE"' | '"FALSE"' ;
CHAR: '\'' ~('\''|'\\') '\'';
STRING: '"' ~('"')* '"';
ESCAPE_SEQUENCE: '[' . ']';
IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]*;

additional: increment_statement | decrement_statement;

increment_statement : IDENTIFIER '++' NEWLINE? ;
decrement_statement : IDENTIFIER '--' NEWLINE? ;

statement 
	: assignment 
	| if_block 
	| while_loop
	| do_while_loop
	| for_loop
	| display
	| scan
	| declaration
	| variable
	| variable_assignment
	| COMMENT
	| expression
	| increment_statement
	| decrement_statement
	;

expression
	: constant												# constantExpression											
	| IDENTIFIER											# identifierExpression
	| '(' expression ')'									# parenthesisExpression
	| 'NOT' expression										# NOTExpression
	| unary_operator expression								# unaryExpression
	| expression multiply_operator expression				# multiplicativeExpression
	| expression add_operator expression					# additiveExpression
	| expression compare_operator expression				# relationalExpression
	| expression bool_operator expression					# boolOpExpression
	| expression concat_operator expression					# concatOpExpression
	| newline_operator										# newlineOpExpression
	| ESCAPE_SEQUENCE										# escapeSequenceExpression
	;

operator
	: unary_operator
	| add_operator
	| multiply_operator
	| compare_operator
	| bool_operator
	| concat_operator
	| newline_operator
	;

unary_operator: '+' | '-';
add_operator: '+' | '-';
multiply_operator: '*' | '/' | '%';
compare_operator: '>' | '<' | '>=' | '<=' | '==' | '<>';
bool_operator: 'AND' | 'OR';
concat_operator: '&';
newline_operator: '$';

WHITESPACE: [\t\r]+ -> skip;
COMMENT: '#' ~[\n]* -> skip;
NEWLINE: '\r'? '\n'| '\r';