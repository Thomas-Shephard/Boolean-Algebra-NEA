using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Utils; 

public static class SyntaxItemUtils {
    public static bool IsShallowMatch(this ISyntaxItem syntaxItem, ISyntaxItem otherSyntaxItem) {
        return syntaxItem.GetType() == otherSyntaxItem.GetType()
               && syntaxItem.Identifier.Equals(otherSyntaxItem.Identifier);
    }
    
    public static ISyntaxItem[] GetDaughterItems(this ISyntaxItem syntaxItem) {
        return syntaxItem switch {
            IMultipleDaughterSyntaxItem multipleDaughterSyntaxItem => multipleDaughterSyntaxItem.Daughters,
            ISingleDaughterSyntaxItem singleDaughterSyntaxItem => new[] { singleDaughterSyntaxItem.Daughter },
            _ => Array.Empty<ISyntaxItem>()
        };
    }

    public static ISyntaxItem Compress(this ISyntaxItem syntaxItem) {
        switch (syntaxItem) {
            case IMultipleDaughterSyntaxItem multipleDaughterSyntaxItem:
                List<ISyntaxItem> compressedDaughterSyntaxItems = multipleDaughterSyntaxItem.Daughters.Select(daughter => daughter.Compress()).ToList();
                
                for (int i = compressedDaughterSyntaxItems.Count - 1; i >= 0; i--) {
                    ISyntaxItem currentCompressedDaughterSyntaxItem = compressedDaughterSyntaxItems[i];
                    if (currentCompressedDaughterSyntaxItem is not IMultipleDaughterSyntaxItem compressedMultipleDaughterSyntaxItem || !syntaxItem.IsShallowMatch(currentCompressedDaughterSyntaxItem))
                        continue;
                    compressedDaughterSyntaxItems.AddRange(compressedMultipleDaughterSyntaxItem.Daughters);
                    compressedDaughterSyntaxItems.RemoveAt(i);
                }

                return multipleDaughterSyntaxItem switch {
                    BinaryOperator => new BinaryOperator(syntaxItem.Identifier, compressedDaughterSyntaxItems.ToArray()),
                    _ => throw new ArgumentException()
                };
            case ISingleDaughterSyntaxItem singleDaughterSyntaxItem:
                ISyntaxItem compressedDaughterSyntaxItem = singleDaughterSyntaxItem.Daughter.Compress();
                return singleDaughterSyntaxItem switch {
                    RepeatingOperator => new RepeatingOperator(syntaxItem.Identifier, compressedDaughterSyntaxItem),
                    UnaryOperator => new UnaryOperator(syntaxItem.Identifier, compressedDaughterSyntaxItem),
                    _ => throw new ArgumentException()
                };
            default:
                return syntaxItem;
        }
    }
}