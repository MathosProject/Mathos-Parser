grammar MathLanguage;

addSubtract
	: multiplyDivide ((Add | Subtract) multiplyDivide)*
	;

multiplyDivide
	: atom ((Multiply | Divide) atom)*
	;

atom
	: Number
	| Letter
	;

Add
	: '+'
	;

Subtract
	: '-'
	;

Multiply
	: '*'
	;

Divide
	: '/'
	;

Number
	: [0-9]+ ('.' [0-9]+)?
	;

Letter
	: [A-Za-z]
	;

WS  
    : [ \r\n\t] + -> channel (HIDDEN)
    ;
