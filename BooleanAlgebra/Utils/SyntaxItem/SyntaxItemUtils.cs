using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

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
    /// <param name="syntaxItem">The syntax item that is to be compressed.</param>
    /// <example>Converts (('a' and 'b') and 'c') into ('a' and 'b' and 'c').</example>
    public static void Compress(this ISyntaxItem syntaxItem) {
        //Depending on the type of the syntax item, the compression is done differently.
        switch (syntaxItem) {
            //When the syntax item is an IMultipleChildSyntaxItem, the child nodes are compressed recursively.
            case IMultiChildSyntaxItem multiChildSyntaxItem:
                //Iterates over the current child nodes and recursively compresses each child nodes child nodes.
                foreach (ISyntaxItem child in multiChildSyntaxItem.Children) {
                    child.Compress();
                }
                List<ISyntaxItem> newChildNodes = multiChildSyntaxItem.Children.ToList();
                //Iterates over the current child nodes and merges the child nodes that are of the same type.
                for (int i = newChildNodes.Count - 1; i >= 0; i--) {
                    //The child nodes can only be merged if they have the same identifier.
                    if (!syntaxItem.IsIdentifierEqual(newChildNodes[i]))
                        continue;
                    //Merge the child nodes of the current child node with the current child node.
                    newChildNodes.AddRange(newChildNodes[i].GetChildNodes());
                    //Remove the current child node as its child nodes have been merged with the current child node.
                    newChildNodes.RemoveAt(i);
                }
                //Updates the child nodes of the current syntax item.
                multiChildSyntaxItem.Children = newChildNodes.ToArray();
                break;
            //When the syntax item is an ISingleChildSyntaxItem, the child node is recursively compressed.
            case ISingleChildSyntaxItem singleChildSyntaxItem:
                singleChildSyntaxItem.Child.Compress();
                break;
        }
    }
}