namespace BooleanAlgebra.Utils; 

public static class SyntaxItemUtils {
    public static bool IsShallowMatch(this SyntaxItem syntaxItem, SyntaxItem otherSyntaxItem) {
        return syntaxItem.GetType() == otherSyntaxItem.GetType()
               && syntaxItem.Value == otherSyntaxItem.Value;
    }

    public static SyntaxItem Compress(this SyntaxItem syntaxItem) {
        for (int i = 0; i < syntaxItem.DaughterItems.Count; i++) {
            syntaxItem.DaughterItems[i] = syntaxItem.DaughterItems[i].Compress();
        }

        if (syntaxItem is not BinaryOperator) return syntaxItem;
        
        bool hasFoundMatch;
        do {
            hasFoundMatch = false;

            for (int i = syntaxItem.DaughterItems.Count - 1; i >= 0; i--) {
                if(!syntaxItem.IsShallowMatch(syntaxItem.DaughterItems[i]))
                    continue;
                SyntaxItem tempSyntaxItem = syntaxItem.DaughterItems[i];
                
                syntaxItem.DaughterItems.RemoveAt(i);
                foreach (SyntaxItem daughterItem in tempSyntaxItem.DaughterItems) {
                    syntaxItem.DaughterItems.Add(daughterItem);
                }
                
                hasFoundMatch = true;
            }
        } while (hasFoundMatch);
            
        return syntaxItem;
    }
}