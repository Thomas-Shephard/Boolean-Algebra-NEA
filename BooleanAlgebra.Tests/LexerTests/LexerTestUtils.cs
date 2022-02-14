namespace BooleanAlgebra.Tests.LexerTests; 
public static class LexerTestUtils {
    public static Identifier GetIdentifierFromValue(string identifierValue) {
        //Finds an identifier that matches the given value.
        //If no identifier is found, an exception is thrown.
        if(!IdentifierUtils.TryGetIdentifierFromSymbol(identifierValue, false, out Identifier? matchedIdentifier))
            throw new Exception("Invalid identifier");
        return matchedIdentifier;
    }
    
    
    public static Identifier GetOperandIdentifierFromValue(string operandValue) {
        //Finds an identifier that matches the given value.
        //If the identifier found is not of an operand, an exception is thrown.
        Identifier matchedIdentifier = GetIdentifierFromValue(operandValue);
        if(matchedIdentifier.IdentifierType != IdentifierType.OPERAND)  
            throw new Exception("Identifier is not an operand");
        return matchedIdentifier;
    }
    
    public static Lexeme GenerateContextFreeLexeme(Identifier identifier, int startIndex, int endIndex) {
        //Creates a new lexeme with the given identifier and the given start and end index.
        return new Lexeme(identifier, new LexemePosition(startIndex, endIndex));
    }
    
    public static Lexeme GenerateContextualLexeme(Identifier identifier, int startIndex, int endIndex, string context) {
        //Creates a new contextual lexeme with the given identifier, start and end index and the given context.
        return new ContextualLexeme(identifier, new LexemePosition(startIndex, endIndex), context);
    }
}