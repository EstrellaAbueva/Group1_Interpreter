﻿grammar Code;

program: NEWLINE? BEGIN NEWLINE statement* NEWLINE END;
variable_dec: declaration* NEWLINE?;
executable_code: statement* NEWLINE?;
line: (declaration | statement | COMMENT) NEWLINE;

BEGIN: 'BEGIN CODE';
END: 'END CODE';

declaration: NEWLINE? type IDENTIFIER ('=' expression)? (',' IDENTIFIER ('=' expression)?)* ;
type: 'INT' | 'FLOAT' | 'BOOL' | 'CHAR' | 'STRING';
variable: IDENTIFIER ('=' (expression))?;
assignment: type IDENTIFIER '=' expression NEWLINE;
function_call: IDENTIFIER (display | scan);
arguments: expression (',' expression)*;

display: NEWLINE? 'DISPLAY' ':' expression*;
scan: 'SCAN' ':' IDENTIFIER (',' IDENTIFIER)*;

if_statement: if_block else_if_block* else_block? 'END IF';
if_block: 'IF' comparison 'THEN' executable_code;
else_if_block: 'ELSE IF' comparison 'THEN' executable_code;
else_block: 'ELSE' executable_code;
comparison: expression* comparison_operator expression*;
comparison_operator: '>' | '<' | '>=' | '<=' | '=' | '<>';

while_loop: 'WHILE' comparison 'DO' executable_code 'END WHILE';

constant: INT | FLOAT | BOOL | CHAR | STRING;
INT: [0-9]+;
FLOAT: [0-9]+('.' [0-9]+)?;
BOOL: 'TRUE' | 'FALSE';
CHAR: '\'' ~('\''|'\\') '\'';
STRING: '"' ~('"')* '"';
ESCAPE_SEQUENCE: '\\' . ;
IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]*;

statement 
	: assignment
	| function_call 
	| if_statement 
	| while_loop
	| display
	| scan
	| COMMENT
	| declaration											
	;

expression
	: constant												# constantExpression											
	| IDENTIFIER											# identifierExpression
	| '(' expression ')'									# parenthesisExpression
	| 'NOT' expression										# NOTExpression
	| expression unary_operator expression					# unaryExpression
	| expression add_operator expression					# addOpExpression
	| expression multiply_operator expression				# multiplyOpExpression
	| expression compare_operator expression				# compareOpExpression
	| expression bool_operator expression					# boolOpExpression
	| expression concat_operator expression					# concatOpExpression
	;

unary_operator: '+' | '-';
add_operator: '+' | '-';
multiply_operator: '*' | '/' | '%';
compare_operator: '>' | '<' | '>=' | '<=' | '=' | '<>';
bool_operator: 'AND' | 'OR';
concat_operator: '&';
newline_operator: '$';

WS: [ \t]+ -> skip;
COMMENT: '#' ~[\r\n]* -> skip ;
NEWLINE: '\n';
