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
            IMultiChildSyntaxItem multiChildSyntaxItem => multiChildSyntaxItem.ChildNodes,
            //For an ISingleChildSyntaxItem, there is only one child node and it is stored in the Child property.
            ISingleChildSyntaxItem singleChildSyntaxItem => new[] { singleChildSyntaxItem.ChildNode },
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
                foreach (ISyntaxItem child in multiChildSyntaxItem.ChildNodes) {
                    child.Compress();
                }
                List<ISyntaxItem> newChildNodes = multiChildSyntaxItem.ChildNodes.ToList();
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
                multiChildSyntaxItem.ChildNodes = newChildNodes.ToArray();
                break;
            //When the syntax item is an ISingleChildSyntaxItem, the child node is recursively compressed.
            case ISingleChildSyntaxItem singleChildSyntaxItem:
                singleChildSyntaxItem.ChildNode.Compress();
                break;
        }
    }

    /// <summary>
    /// Returns the repeating generic operand that is nested inside the repeating operator.
    /// </summary>
    /// <param name="repeatingOperator">The repeating operator to search for a repeating generic operand inside of.</param>
    /// <param name="depthOfRepeatingGenericOperand">The depth that the repeating generic operand was found at.</param>
    /// <returns>The repeating generic operand that is nested inside of the given repeating operator.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is not exactly one repeating generic operand nested inside of the given repeating operator.</exception>
    public static GenericOperand GetRepeatingGenericOperand(this RepeatingOperator repeatingOperator, out int depthOfRepeatingGenericOperand) {
        //The repeating generic operand can be directly nested within the repeating operator.
        //If this is the case, then the repeating generic operand should be returned.
        if (repeatingOperator.ChildNode is GenericOperand { IsRepeating: true } repeatingGenericOperand) {
            //If the repeating generic operand is the child of the repeating operator, then the depth of the repeating generic operand is 0.
            depthOfRepeatingGenericOperand = 0;
            return repeatingGenericOperand;
        }
           
        //Get all the generic operands nested inside the repeating operators child node.
        GenericOperand[] foundRepeatingGenericOperands = repeatingOperator.ChildNode.GetChildNodes().OfType<GenericOperand>().Where(genericOperand => genericOperand.IsRepeating).ToArray();
        //There can only be one repeating generic operand within a repeating operator.
        if(foundRepeatingGenericOperands.Length != 1)
            throw new InvalidOperationException("Repeating operators must have exactly one repeating generic operand nested inside of them.");
        //If the repeating generic operand is one of the children of the child of the repeating operator, then the depth of the repeating generic operand is 1
        depthOfRepeatingGenericOperand = 1;
        //If there is only one repeating generic operand, then return it.
        return foundRepeatingGenericOperands.First();
    }
}