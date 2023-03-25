grammar Code;

program: 'BEGIN CODE' NEWLINE variable* executable_code 'END CODE';
variable_dec: declaration NEWLINE;
executable_code: statement NEWLINE;

statement: declaration | assignment | comment | function_call | if_statement | while_loop | print;
declaration: NEWLINE IDENTIFIER variable (',' variable)*;
type: 'INT' | 'FLOAT' | 'BOOL' | 'CHAR' | 'STRING';
variable: IDENTIFIER ('=' (expression))?;
assignment: IDENTIFIER ('=' IDENTIFIER)* '=' expression;
function_call: IDENTIFIER (display | scan);
arguments: expression (',' expression)*;

display: 'DISPLAY' ':' expression;
scan: 'SCAN' ':' IDENTIFIER (',' IDENTIFIER)*;

if_statement: if_block else_if_block* else_block? 'END IF';
if_block: 'IF' comparison 'THEN' executable_code;
else_if_block: 'ELSE IF' comparison 'THEN' executable_code;
else_block: 'ELSE' executable_code;
comparison: expression comparison_operator expression;
comparison_operator: '>' | '<' | '>=' | '<=' | '=' | '<>';

while_loop: 'WHILE' comparison 'DO' executable_code 'END WHILE';

print: 'PRINT' expression;

comment: '#' ~( '\r' | '\n' | '\r\n' )*;
constant: INT | FLOAT | BOOL | CHAR | STRING;
INT: [0-9]+;
FLOAT: [0-9]+('.' [0-9]+)?;
BOOL: 'TRUE' | 'FALSE';
CHAR: '\'' ~('\''|'\\') '\'';
STRING: '"' ~('"')* '"';
ESCAPE_SEQUENCE: '\\' . ;
IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]*;

expression
	: constant 
	| IDENTIFIER 
	| '(' expression ')'
	| 'NOT' expression
	| expression unary_operator expression
	| expression add_operator expression
	| expression multiply_operator expression
	| expression compare_operator expression
	| expression bool_operator expression
	| expression concat_operator expression
	| expression newline_operator expression
	;

unary_operator: '+' | '-';
add_operator: '+' | '-';
multiply_operator: '*' | '/' | '%';
compare_operator: '>' | '<' | '>=' | '<=' | '=' | '<>';
bool_operator: 'AND' | 'OR';
concat_operator: '&';
newline_operator: '$';

WS: [ \t]+ -> skip;
NEWLINE: '\n';
