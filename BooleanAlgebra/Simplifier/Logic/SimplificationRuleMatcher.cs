namespace BooleanAlgebra.Simplifier.Logic; 
/// <summary>
/// Provides a set of methods to match a given syntax tree against a given pattern match tree.
/// </summary>
public static class SimplificationRuleMatcher {
    /// <summary>
    /// Returns a list of matches that satisfy both the given syntax tree and the given pattern match tree as well as any previously found matches.
    /// </summary>
    /// <param name="syntaxTree">A syntax tree that the pattern match tree is compared against.</param>
    /// <param name="patternMatchTree">A pattern match tree that the syntax tree is compared against</param>
    /// <param name="currentMatches">A previously found matches that the given syntax tree must also but matched against.</param>
    /// <returns>A list of matches that satisfy both the given syntax tree and the given pattern match tree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxTree"/> or <paramref name="patternMatchTree"/> is null.</exception>
    public static List<Matches> GetAllMatches(ISyntaxItem syntaxTree, ISyntaxItem patternMatchTree, Matches? currentMatches = null) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (patternMatchTree is null) throw new ArgumentNullException(nameof(patternMatchTree));
        //If the syntax tree does not have the same identifier as the pattern match tree, then there is no match.
        if (!syntaxTree.IsIdentifierEqual(patternMatchTree)) return new List<Matches>();
        //If the matches is currently null, then create a new matches object.
        currentMatches ??= new Matches();
        //Get the child nodes from the syntax tree and pattern match tree      
        ISyntaxItem[] syntaxTreeChildNodes = syntaxTree.GetChildNodes();
        ISyntaxItem[] patternMatchChildNodes = patternMatchTree.GetChildNodes();
        //Return the direct substitutes from the child nodes of the syntax tree and pattern match tree as well as the previous matches.
        return GetDirectSubstitutes(syntaxTreeChildNodes, patternMatchChildNodes, currentMatches);
    }

    /// <summary>
    /// Returns a list of matches that satisfy both the collection of given syntax items and the collection of given pattern match syntax items for the direct substitutes.
    /// </summary>
    /// <param name="syntaxTreeChildNodes">A collection of syntax items that the collection of pattern match items is compared against.</param>
    /// <param name="patternMatchChildNodes">A collection of pattern match items that the collection of syntax items is compared against.</param>
    /// <param name="currentMatches">A previously found matches that the given syntax items must also but matched against.</param>
    /// <returns>A list of matches that satisfy both the collection of given syntax items and the collection of given pattern match syntax items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxTreeChildNodes"/> or <paramref name="patternMatchChildNodes"/> or <paramref name="currentMatches"/> is null.</exception>
    private static List<Matches> GetDirectSubstitutes(IReadOnlyCollection<ISyntaxItem> syntaxTreeChildNodes, IReadOnlyCollection<ISyntaxItem> patternMatchChildNodes, Matches currentMatches) {
        if (syntaxTreeChildNodes is null) throw new ArgumentNullException(nameof(syntaxTreeChildNodes));
        if (patternMatchChildNodes is null) throw new ArgumentNullException(nameof(patternMatchChildNodes));
        if (currentMatches is null) throw new ArgumentNullException(nameof(currentMatches));
        List<Matches> availableMatches = new();
        //Get the distinct child nodes of the syntax tree and pattern match tree.
        //This removes unnecessary operations as checking the same child node more than once is not necessary.
        List<SyntaxItemCountPair> syntaxTreeChildNodeCounts = syntaxTreeChildNodes.GetSyntaxItemCounts();
        List<SyntaxItemCountPair> patternMatchTreeChildNodeCounts = patternMatchChildNodes.GetSyntaxItemCounts();

        //Only iterate over each child node of the syntax tree to prevent unnecessary operations.
        foreach (ISyntaxItem syntaxTreeChildNode in syntaxTreeChildNodeCounts
                     .Select(syntaxTreeChildNode => syntaxTreeChildNode.SyntaxItem)) {
            //Only iterate over each child node of the pattern match tree to prevent unnecessary operations.
            //Only non repeating operators should be checked for direct substitutes.
            foreach (ISyntaxItem patternMatchTreeChildNode in patternMatchTreeChildNodeCounts
                         .Select(patternMatchTreeChildNode => patternMatchTreeChildNode.SyntaxItem)
                         .Where(patternMatchTreeChildNode => patternMatchTreeChildNode is not RepeatingOperator)) {
                //Create a copy of the syntax tree child nodes and remove the current child node.
                List<ISyntaxItem> syntaxTreeChildNodesCopy = syntaxTreeChildNodes.ToList();
                syntaxTreeChildNodesCopy.Remove(syntaxTreeChildNode);
                //Create a copy of the pattern match tree child nodes and remove the current child node.
                List<ISyntaxItem> patternMatchChildNodesCopy = patternMatchChildNodes.ToList();
                patternMatchChildNodesCopy.Remove(patternMatchTreeChildNode);
                
                //Depending on the type of the child node, a different method is called.
                switch (patternMatchTreeChildNode) {
                    //When the generic operand already has a specified direct substitute, then ensure that the direct substitute is the same as the generic operand.
                    case GenericOperand genericOperand when currentMatches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out ISyntaxItem? foundDirectSubstitute): {
                        //If the found direct substitute is not equal to the syntax tree child node, then there is no match.
                        //If there is no match, then the current matches object is not added to the possible matches.
                        if (foundDirectSubstitute.Equals(syntaxTreeChildNode)) {
                            //Add all the available matches from the child nodes of the syntax tree and syntax tree child nodes.
                            availableMatches.AddRange(GetDirectSubstitutes(syntaxTreeChildNodesCopy, patternMatchChildNodesCopy, currentMatches));
                        }
                        break;
                    }
                    //When the pattern match tree child node is a generic operand that has not already found a substitute.
                    case GenericOperand genericOperand: {
                        //Create a new list of direct substitutes with a new direct substitute for the current generic operand.
                        List<DirectSubstitute> newDirectSubstitutes = currentMatches.DirectSubstitutes.ToList();
                        newDirectSubstitutes.Add(new DirectSubstitute(genericOperand, syntaxTreeChildNode));
                        //Add the new list of direct substitutes to a new matches object.
                        Matches newMatches = new(newDirectSubstitutes, currentMatches.RepeatingSubstitutes);
                        //Add all the available matches where the current matches would correctly match the syntax tree child nodes.
                        availableMatches.AddRange(GetDirectSubstitutes(syntaxTreeChildNodesCopy, patternMatchChildNodesCopy, newMatches));
                        break;
                    }
                    //When the pattern match tree child node is an operand but not a generic operand.
                    case Operand: {
                        //Ensure that the syntax tree child node and pattern match tree child node are equal.
                        //If they are not equal, the current matches object is not added to the possible matches.
                        if (syntaxTreeChildNode.Equals(patternMatchTreeChildNode)) {
                            //Add all the available matches where the current matches would correctly match the syntax tree child nodes.
                            availableMatches.AddRange(GetDirectSubstitutes(syntaxTreeChildNodesCopy, patternMatchChildNodesCopy, currentMatches));
                        }
                        break;
                    }
                    //When the pattern match tree child node is not an operand. (e.g. a binary or unary operator)
                    //In this case, the current child nodes will likely both have their own child nodes.
                    default: {
                        //Get all the matches that are suitable at deeper levels within the tree structure.
                        foreach (Matches newMatches in GetAllMatches(syntaxTreeChildNode, patternMatchTreeChildNode, currentMatches)) {
                            //Check which of the possible matches that are suitable at deeper levels are also suitable at the current level.
                            //Add all the available matches that are both suitable at the current level and at deeper levels.
                            availableMatches.AddRange(GetDirectSubstitutes(syntaxTreeChildNodesCopy, patternMatchChildNodesCopy, newMatches));
                        }
                        break;
                    }
                }
            }
        }

        //If the only remaining child nodes of the pattern match tree are repeating operators, then the direct substitutes have been exhausted.
        if (patternMatchTreeChildNodeCounts.All(patternMatchCountPair => patternMatchCountPair.SyntaxItem is RepeatingOperator)) {
            //If there are no longer any syntax items in the syntax tree, the optional repeat substitutes cannot be matched.
            //In this circumstance, the current matches object added to the available matches.
            if (!syntaxTreeChildNodes.Any()) {
                availableMatches.Add(currentMatches);
            //If there are still syntax items in the syntax tree, they must be matched to a repeat substitute for the match to be valid.
            } else {
                //Add all the matches where the current matches would correctly match the syntax tree child nodes.
                availableMatches.AddRange(GetRepeatingSubstitutes(syntaxTreeChildNodeCounts, patternMatchTreeChildNodeCounts, currentMatches));
            }
        }
        
        //Return all of the available matches.
        return availableMatches;
    }

    /// <summary>
    /// Returns a list of matches that satisfy both the collection of given syntax items and the collection of given pattern match syntax items for the repeating substitutes.
    /// </summary>
    /// <param name="syntaxTreeChildNodeCounts">A collection of syntax items (with their respected counts) that the collection of pattern match items is compared against.</param>
    /// <param name="patternMatchTreeChildNodeCounts">A collection of pattern match items (with their respected counts) that the collection of syntax items is compared against.</param>
    /// <param name="currentMatches">The previously found matches that the given syntax items must also but matched against.</param>
    /// <returns>A list of matches that satisfy both the collection of given syntax items and the collection of given pattern match syntax items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxTreeChildNodeCounts"/> or <paramref name="patternMatchTreeChildNodeCounts"/> or <paramref name="currentMatches"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a discovered matches object does not contain a substitute for a repeating generic operand.</exception>
    private static List<Matches> GetRepeatingSubstitutes(List<SyntaxItemCountPair> syntaxTreeChildNodeCounts, List<SyntaxItemCountPair> patternMatchTreeChildNodeCounts, Matches currentMatches) {
        if (syntaxTreeChildNodeCounts is null) throw new ArgumentNullException(nameof(syntaxTreeChildNodeCounts));
        if (patternMatchTreeChildNodeCounts is null) throw new ArgumentNullException(nameof(patternMatchTreeChildNodeCounts));
        if (currentMatches is null) throw new ArgumentNullException(nameof(currentMatches));
        List<Matches> availableMatches = new();
        //Gets all of the available repeating operators that each syntax tree child node can be matched against.
        IEnumerable<RepeatingOperator> availableRepeatingOperators = patternMatchTreeChildNodeCounts
            .Select(patternMatchTreeChildNode => patternMatchTreeChildNode.SyntaxItem)
            .OfType<RepeatingOperator>();
        //Iterate over the available repeating operators, the first repeating operator that matches a given syntax tree child node is the best match.
        foreach (RepeatingOperator availableRepeatingOperator in availableRepeatingOperators) {
            //Get the generic operand that is nested within the repeating operator.
            GenericOperand repeatingGenericOperand = availableRepeatingOperator.GetRepeatingGenericOperand(out int depthOfRepeatingGenericOperand);
            List<ISyntaxItem> matchedSyntaxItems = new();
            //Iterate over the syntax tree child nodes to determine which syntax tree child nodes match the repeating operator.
            for (int i = syntaxTreeChildNodeCounts.Count - 1; i >= 0; i--) {
                //Get the syntax tree child nodes at the depth of the repeating generic operand.
                ISyntaxItem[] syntaxTreeChildNodes = depthOfRepeatingGenericOperand == 0 
                    ? new[] { syntaxTreeChildNodeCounts[i].SyntaxItem } 
                    : syntaxTreeChildNodeCounts[i].SyntaxItem.GetChildNodes();
                //Directly substitute the repeating operator with the syntax tree child nodes.
                List<Matches> possibleMatches = GetDirectSubstitutes(syntaxTreeChildNodes, new[] { availableRepeatingOperator.ChildNode }, currentMatches);
                //If there are no possible matches, then the syntax tree child node cannot be matched to the repeating operator.
                if(possibleMatches.Count == 0)
                    continue;
                //Get a substitute for the current repeating generic operand that matches the repeating generic operand.
                if (!possibleMatches[0].TryGetDirectSubstituteFromGenericOperand(repeatingGenericOperand, out ISyntaxItem? substitute))
                    //If this does occur, it means that there is a programming error as the match should always contain the repeating generic operand.
                    throw new InvalidOperationException("The possible match did not contain a substitute for the repeating generic operand.");
                //Add the substitute to the list of matched syntax items the number of times that it occurred in the original syntax tree at the current depth.
                matchedSyntaxItems.AddRepeated(substitute, syntaxTreeChildNodeCounts[i].Count);
                //Remove the original syntax tree child node as it has been matched.
                syntaxTreeChildNodeCounts.RemoveAt(i);
            }

            //If there are no syntax items that match the current repeating operator, then continue to the next repeating operator.
            if(matchedSyntaxItems.Count == 0)
                continue;

            //Create a new matches object with the existing substitutes from the current matches object.
            Matches newMatches = currentMatches.Clone();
            if (newMatches.TryGetRepeatingSubstituteFromGenericOperand(repeatingGenericOperand, out List<ISyntaxItem>? foundSyntaxItems)) {
                //If the current matches object contains a substitute for the current repeating generic operand, then add the matched syntax items to the existing substitute.
                foundSyntaxItems.AddRange(matchedSyntaxItems);
            } else {
                //If the current matches object does not contain a substitute for the current repeating generic operand, then add the matched syntax items to a new substitute.
                newMatches.RepeatingSubstitutes.Add(new RepeatingSubstitute(repeatingGenericOperand, matchedSyntaxItems));
            }
            //Add the new matches object to the list of available matches.
            availableMatches.Add(newMatches);
        }

        //Return all of the available matches.
        return availableMatches;
    }
}