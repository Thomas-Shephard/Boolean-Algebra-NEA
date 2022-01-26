namespace BooleanAlgebra.Utils.SyntaxItem; 
/// <summary>
/// Provides a set of methods that make it easier to work with syntax items.
/// </summary>
public static class SyntaxItemUtils {
    /// <summary>
    /// Compares two given syntax items identifiers for equality and returns true if they are equal.
    /// </summary>
    /// <param name="syntaxItem">The first syntax item that should be compared.</param>
    /// <param name="otherSyntaxItem">The second syntax item that should be compared.</param>
    /// <returns>True if the identifiers of both syntax items are equal.</returns>
    public static bool IsIdentifierEqual(this ISyntaxItem syntaxItem, ISyntaxItem otherSyntaxItem) {
        return syntaxItem.Identifier.Equals(otherSyntaxItem.Identifier);
    }
    
    /// <summary>
    /// Provides the child nodes of a given syntax item.
    /// </summary>
    /// <param name="syntaxItem">The specified syntax item to find the child nodes from.</param>
    /// <returns>An array of the child nodes that a given syntax item has.</returns>
    public static ISyntaxItem[] GetChildNodes(this ISyntaxItem syntaxItem) {
        //The child nodes from a syntax item are found differently depending on the type of the syntax item.
        return syntaxItem switch {
            //For an IMultiSyntaxItem, there is more than one child node and it is stored in the Children property.
            IMultiChildSyntaxItem multiChildSyntaxItem => multiChildSyntaxItem.Children,
            //For an ISingleChildSyntaxItem, there is only one child node and it is stored in the Child property.
            ISingleChildSyntaxItem singleChildSyntaxItem => new[] { singleChildSyntaxItem.Child },
            //For any other implementation of ISyntaxItem, there are no child nodes.
            _ => Array.Empty<ISyntaxItem>()
        };
    }

    /// <summary>
    /// Compresses a given syntax item to allow for the simplification rules to be applied appropriately.
    /// </summary>
    /// <param name="syntaxItem">The syntax item that should be compressed.</param>
    /// <returns>A new syntax item that is the compressed equivalent of the previous syntax item.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="syntaxItem"/> is of an unrecognised type.</exception>
    /// <example>Converts (('a' and 'b') and 'c') into ('a' and 'b' and 'c').</example>
    public static ISyntaxItem Compress(this ISyntaxItem syntaxItem) {
        //Depending on the type of the syntax item, the compression is done differently.
        switch (syntaxItem) {
            //When the syntax item is an IMultipleChildSyntaxItem, the child nodes are compressed recursively.
            case IMultiChildSyntaxItem multiChildSyntaxItem:
                List<ISyntaxItem> newChildNodes = new();
                //Iterate through the current child nodes and compress them.
                foreach (ISyntaxItem childNode in multiChildSyntaxItem.Children) {
                    //Add the recursively compressed child node the list of new child nodes.
                    newChildNodes.Add(childNode.Compress());
                }
                
                //Iterate over the new child nodes and check if any can be merged with the current child nodes.
                //For example, (('a' and 'b') and 'c') can be merged into ('a' and 'b' and 'c').
                for (int i = newChildNodes.Count - 1; i >= 0; i--) {
                    ISyntaxItem compressedChildNode = newChildNodes[i];
                    if (!syntaxItem.IsIdentifierEqual(compressedChildNode))
                        //The current compressed child node can only be merged with the current child nodes if the identifiers are the same.
                        continue;
                    //If the current compressed child node can be merged with the current child nodes, merge them.
                    //The merge process involves adding the child nodes of the current compressed child node to the current child nodes.
                    newChildNodes.AddRange(compressedChildNode.GetChildNodes());
                    //The current compressed child node is removed from the list of new child nodes as its child nodes have been added to the current child nodes.
                    newChildNodes.RemoveAt(i);
                }
                
                //Return a new multi child syntax item with the new child nodes as its child nodes.
                return multiChildSyntaxItem switch {
                    BinaryOperator => new BinaryOperator(syntaxItem.Identifier, newChildNodes.ToArray()),
                    //If none of the above are true, the type of the syntax item was not recognised and an exception is thrown.
                    _ => throw new InvalidOperationException("The IMultipleChildSyntaxItem was not recognised.")
                };
            //When the syntax item is an ISingleChildSyntaxItem, the child node is recursively compressed.
            case ISingleChildSyntaxItem singleChildSyntaxItem:
                //Return a new single child syntax item with the compressed child node as the child node.
                return singleChildSyntaxItem switch {
                    RepeatingOperator => new RepeatingOperator(syntaxItem.Identifier, singleChildSyntaxItem.Child.Compress()),
                    UnaryOperator => new UnaryOperator(syntaxItem.Identifier, singleChildSyntaxItem.Child.Compress()),
                    //If none of the above are true, the type of the syntax item was not recognised and an exception is thrown.
                    _ => throw new InvalidOperationException("The ISingleChildSyntaxItem was not recognised.")
                };
            //When the syntax item is not a multiple or single daughter syntax item, it cannot be compressed.
            default:
                return syntaxItem;
        }
    }
}