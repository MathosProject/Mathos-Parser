grammar MathLanguage;

math
	: addition
	;

addition
	: multiply ('+' multiply | '-' multiply)*
	;

multiply
	:
	;

atom
	: number
	| '(' addition ')'
	;

number
	: ('0' .. '9')+ ('.' ('0' .. '9')+)?
	;

WS  
    : (' ' | '\t' | '\r'| '\n') {$channel=HIDDEN;}
    ;
