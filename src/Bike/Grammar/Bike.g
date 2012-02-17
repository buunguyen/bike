grammar Bike;

/****************************** PARSER RULES ******************************/
program
	: sourceElements EOF
	;
	
sourceElements
	: sourceElement (sourceElement)*
	;
	
sourceElement
	: functionDeclaration 
	| statement
	;
	
functionDeclaration
	: 'func' Identifier formalParameterList functionBody
	;
	
functionExpression
	: 'func' Identifier? formalParameterList functionBody
	;
	
formalParameterList
	: '(' (formalParameter (',' formalParameter)*)? ')'
	;

formalParameter
	: ('*')? Identifier
	;

functionBody
	: ('{' sourceElements '}')
	;

statement
	: statementBlock
	| loadStatement
	| variableStatement
	| emptyStatement
	| expressionStatement
	| ifStatement
	| whileStatement
	| forInStatement
	| nextStatement
	| breakStatement
	| returnStatement
	| switchStatement
	| throwStatement
	| tryStatement
	;
	
statementBlock
	: '{' statementList? '}'
	;
	
statementList
	: statement ( statement)*
	;
	
loadStatement
	: 'load' assignmentExpression ';'
	;
	
variableStatement
	: 'var' variableDeclarationList ';'
	;
	
variableDeclarationList
	: variableDeclaration ( ',' variableDeclaration)*
	;
	
variableDeclaration
	: Identifier initialiser?
	;
	
initialiser
	: '=' assignmentExpression
	;
	
emptyStatement
	: ';'
	;
	
expressionStatement
	: expression ';' // ';' is optional if next is '};
	;
	
ifStatement
	: 'if' '(' assignmentExpression ')' statement ( 'else' statement)?
	;
	
whileStatement
	: 'while' '(' assignmentExpression ')' statement
	;
	
forInStatement
	: 'for' '(' forInStatementInitialiserPart 'in' assignmentExpression ')' statement
	;
	
forInStatementInitialiserPart
	: leftHandSideExpression 
	| 'var' variableDeclaration
	;

nextStatement
	: 'next' ';'
	;

breakStatement
	: 'break' ';'
	;

returnStatement
	: 'return' assignmentExpression? ';'
	;
	
switchStatement
	: 'switch' '(' assignmentExpression ')' caseBlock
	;
	
caseBlock
	: '{' ( caseClause)* ( defaultClause)? '}'
	;

caseClause
	: 'case' expression ':' statementList?
	;
	
defaultClause
	: 'default' ':' statementList?
	;
	
throwStatement
	: 'throw' assignmentExpression ';'
	;

tryStatement
	: 'try' statementBlock (finallyClause | rescueClause ( finallyClause)?)
	;
    
rescueClause
	: 'rescue' Identifier? statementBlock
	;
	
finallyClause
	: 'finally' statementBlock
	;

expression
	: assignmentExpression ( ',' assignmentExpression)*
	;
	
assignmentExpression
	: conditionalExpression
	| 'exec' assignmentExpression
	| leftHandSideExpression AssignmentOperator assignmentExpression
	;
		
conditionalExpression
	: logicalORExpression ( '?' assignmentExpression ':' assignmentExpression)?
	;

leftHandSideExpression
	: memberExpression (arguments ( callExpressionSuffix)*)? 
	;

memberExpression
	: (primaryExpression | functionExpression) ( memberExpressionSuffix)* 
	;
	
memberExpressionSuffix
	: indexSuffix
	| propertyReferenceSuffix
	| typeDescriptorSuffix
	;
	
callExpressionSuffix
	: arguments
	| indexSuffix
	| propertyReferenceSuffix
	| typeDescriptorSuffix
	;

arguments
	: '(' ( argument ( ',' argument)*)? ')'
	;
	
argument
	: ('*')? assignmentExpression
	;
	
typeDescriptorSuffix
	: '<' typeDescriptor ( ',' typeDescriptor)* '>'
	;

typeDescriptor
	: Identifier ( '.' Identifier)* ('<' typeDescriptor ( ',' typeDescriptor)* '>')*
	;

indexSuffix
	: '[' expression ']'
	;	
	
propertyReferenceSuffix
	: '.' Identifier
	;

logicalORExpression
	: logicalANDExpression ( '||' logicalANDExpression)*
	;
	
logicalANDExpression
	: equalityExpression ( '&&' equalityExpression)*
	;
	
equalityExpression
	: relationalExpression ( ('==' | '=') relationalExpression)*
	;

relationalExpression
	: additiveExpression ( ('<' | '>' | '<=' | '>=' | 'is') additiveExpression)*
	;

additiveExpression
	: multiplicativeExpression ( ('+' | '-') multiplicativeExpression)*
	;

multiplicativeExpression
	: unaryExpression ( ('*' | '/' | '%') unaryExpression)*
	;

unaryExpression
	: postfixExpression
	| ('++' | '--' | '+' | '-' | '') unaryExpression
	;
	
postfixExpression
	: leftHandSideExpression ('++' | '--')?
	;

primaryExpression
	: 'this'
	| Identifier
	| literal
	| arrayLiteral
	| objectLiteral
	| '(' expression ')'
	;
	
arrayLiteral
	: '[' assignmentExpression? ( ',' ( assignmentExpression)?)* ']'
	| '[' assignmentExpression '.' '.' assignmentExpression ']'
	;
    
objectLiteral
	: '{' propertyNameAndValue ( ',' propertyNameAndValue)* '}'
	;
	
propertyNameAndValue
	: propertyName ':' assignmentExpression
	;

propertyName
	: Identifier
	| StringLiteral
	| NumericLiteral
	;

literal
	: 'null'
	| 'true'
	| 'false'
	| StringLiteral
	| NumericLiteral
	;



/****************************** LEXER RULES ******************************/
AssignmentOperator
	: '=' | '*=' | '/=' | '%=' | '+=' | '-=' | '||=' | '&&='
	;
	
Identifier 
	: ('a'..'z'|'A'..'Z'|'_'|'$') ('a'..'z'|'A'..'Z'|'0'..'9'|'_'|'$'|'')*
  	;

NumericLiteral
	: DecimalDigit+ ('.' DecimalDigit+)?
	;

fragment DecimalDigit
	: ('0'..'9')
	;

StringLiteral
  	: '"' ( EscSeq | ~('\\'|'"') )* '"'
	| '\'' ( EscSeq | ~('\''|'\\') ) '\''
  	;

fragment EscSeq
  	:  '\\' ('b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
  	;
	
Comment
	: '#!' ( .)* '!#' 
	| '#' ~('\n' | '\r' | '\r\n')* 
	;

WhiteSpace
	: ('\t' | ' ' | '\n' | '\r' | '\r\n')	
	;