grammar Code;

program: NEWLINE? BEGIN NEWLINE? (line)* NEWLINE? END NEWLINE? EOF;
variable_dec: declaration*;
line: (declaration | statement | COMMENT) NEWLINE+;

BEGIN: 'BEGIN CODE';
END: 'END CODE';

declaration: type IDENTIFIER ('=' expression)? (',' IDENTIFIER ('=' expression)?)* ;
type: 'INT' | 'FLOAT' | 'BOOL' | 'CHAR' | 'STRING';
variable: type IDENTIFIER ('=' (expression))?;
variable_assignment: type IDENTIFIER;
assignment: IDENTIFIER ('=' IDENTIFIER)* '=' expression;
function_call: IDENTIFIER (display | scan);
arguments: expression (',' expression)*;

display: 'DISPLAY' ':' expression;
scan: 'SCAN' ':' IDENTIFIER (',' IDENTIFIER)*;

BEGIN_IF: 'BEGIN IF';
END_IF: 'END IF';
if_block: 'IF' '(' expression ')' NEWLINE? BEGIN_IF NEWLINE? line* NEWLINE? END_IF NEWLINE? else_if_block* else_block? NEWLINE?;
else_if_block: 'ELSE IF' '(' expression ')' NEWLINE? BEGIN_IF NEWLINE? line* NEWLINE? END_IF NEWLINE?;
else_block: 'ELSE' NEWLINE? BEGIN_IF NEWLINE? line* NEWLINE? END_IF NEWLINE?;

BEGIN_WHILE: 'BEGIN WHILE';
END_WHILE: 'END WHILE';
BEGIN_DO_WHILE: 'BEGIN DO WHILE';
END_DO_WHILE: 'END DO WHILE';
BEGIN_FOR_LOOP: 'BEGIN FOR';
END_FOR_LOOP: 'END FOR';
while_loop: 'WHILE' '(' expression ')' NEWLINE? BEGIN_WHILE NEWLINE? line* NEWLINE? END_WHILE NEWLINE?;
do_while_loop: 'DO' NEWLINE? BEGIN_DO_WHILE NEWLINE? line* NEWLINE? END_DO_WHILE NEWLINE? 'WHILE' '(' expression ')' NEWLINE?;
for_loop: 'FOR' '(' statement ';' expression ';'  additional')' NEWLINE? BEGIN_FOR_LOOP NEWLINE? line* NEWLINE? END_FOR_LOOP NEWLINE?;

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
	| function_call 
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