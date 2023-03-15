grammar CodeGrammar;

// Define the tokens
INT: 'INT';
CHAR: 'CHAR';
BOOL: 'BOOL';
TRUE: 'TRUE';
FALSE: 'FALSE';
DISPLAY: 'DISPLAY:';
END: 'END';
ASSIGN: '=';
SEMI: ';';
COMMA: ',';
LPAREN: '(';
RPAREN: ')';
LBRACK: '[';
RBRACK: ']';
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';
EQ: '==';
NEQ: '!=';
GT: '>';
LT: '<';
GTE: '>=';
LTE: '<=';
AND: '&';
OR: '|';
NOT: '!';

// Define the grammar rules
code: statement*;

statement: variable_declaration
          | assignment_statement
          | display_statement
          | comment
          ;

variable_declaration: data_type identifier (ASSIGN expression)? SEMI;

data_type: INT | CHAR | BOOL;

identifier: ID;

assignment_statement: identifier ASSIGN expression SEMI;

expression: literal
           | identifier
           | expression (PLUS | MINUS | MULT | DIV | MOD) expression
           | LPAREN expression RPAREN
           | expression (EQ | NEQ | GT | LT | GTE | LTE) expression
           | expression (AND | OR) expression
           | NOT expression
           ;

literal: INT_LITERAL
        | CHAR_LITERAL
        | TRUE
        | FALSE
        ;

display_statement: DISPLAY expression (COMMA expression)* SEMI;

comment: '#' (~[\r\n])*;

// Define the lexer rules
INT_LITERAL: DIGIT+;
CHAR_LITERAL: '\'' (~'\'' | '\\' .)* '\'';
ID: LETTER (LETTER | DIGIT | '_')*;

fragment DIGIT: [0-9];
fragment LETTER: [a-zA-Z];