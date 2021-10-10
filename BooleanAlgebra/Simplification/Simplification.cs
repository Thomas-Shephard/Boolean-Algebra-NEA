using System;
using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Simplification {
    public class Simplification {
        public SyntaxItem StartSyntaxItem { get; }
        public List<Tuple<SyntaxItem, string>> Simplifications { get; }

        public Simplification(SyntaxItem startSyntaxItem) {
            StartSyntaxItem = startSyntaxItem;
            Simplifications = new List<Tuple<SyntaxItem, string>>();

            SyntaxItem previous = startSyntaxItem;
            string? reason = null;
            bool didSimplify = false;
            do {
                if(reason is not null)
                    Simplifications.Add(new Tuple<SyntaxItem, string>(previous, reason));
               // didSimplify = SimplifySyntaxItem(previous, out previous, out reason);
            } while (didSimplify);
        }

    /*    private bool IsSyntaxItemMatch(SyntaxItem syntaxItem1, SyntaxItem syntaxItem2, ref List<Tuple<SyntaxItem, string>> arbitrarySyntaxItems) {
            if (syntaxItem1.Value != syntaxItem2.Value && syntaxItem1 is not ArbitrarySyntaxItem && syntaxItem2 is not ArbitrarySyntaxItem) return false;
            
            
            if (syntaxItem1 is ArbitrarySyntaxItem) {
                Tuple<SyntaxItem, string>? tuple = arbitrarySyntaxItems.FirstOrDefault(x => x.Item2.Equals(syntaxItem1.Value));
                if (tuple is not null) return tuple.Item1.Equals(syntaxItem2);
                arbitrarySyntaxItems.Add(new Tuple<SyntaxItem, string>(syntaxItem2, syntaxItem1.Value));
                return true;
            }
            
            if (syntaxItem2 is ArbitrarySyntaxItem) {
                Tuple<SyntaxItem, string>? tuple = arbitrarySyntaxItems.FirstOrDefault(x => x.Item2.Equals(syntaxItem2.Value));
                if (tuple is not null) return tuple.Item1.Equals(syntaxItem1);
                arbitrarySyntaxItems.Add(new Tuple<SyntaxItem, string>(syntaxItem1, syntaxItem2.Value));
                return true;
            }

            if (syntaxItem1.GetSyntaxItems().Length != syntaxItem2.GetSyntaxItems().Length) return false;

            for (int i = 0; i < syntaxItem1.GetSyntaxItems().Length; i++) {
                if (!IsSyntaxItemMatch(syntaxItem1.GetSyntaxItems()[i], syntaxItem2.GetSyntaxItems()[i], ref arbitrarySyntaxItems))
                    return false;
            }

            return true;
        }

        private SyntaxItem SubstituteArbitrarySyntaxItems(SyntaxItem syntaxItem, List<Tuple<SyntaxItem, string>> arbitrarySyntaxItems) {
            if (arbitrarySyntaxItems == null) throw new ArgumentNullException(nameof(arbitrarySyntaxItems));
            switch (syntaxItem) {
                case ArbitrarySyntaxItem arbitrarySyntaxItem:
                    return arbitrarySyntaxItems.FirstOrDefault(x => x.Item2 == arbitrarySyntaxItem.Value)?.Item1 ?? throw new Exception();
                case BinaryOperator binaryOperator:
                    binaryOperator.SyntaxItem1 = SubstituteArbitrarySyntaxItems(binaryOperator.SyntaxItem1, arbitrarySyntaxItems);
                    binaryOperator.SyntaxItem2 = SubstituteArbitrarySyntaxItems(binaryOperator.SyntaxItem2, arbitrarySyntaxItems);
                    return binaryOperator;
                case UnaryOperator unaryOperator:
                    unaryOperator.SyntaxItem = SubstituteArbitrarySyntaxItems(unaryOperator.SyntaxItem, arbitrarySyntaxItems);
                    return unaryOperator;
                default:
                    return syntaxItem;
            }
        }

        private bool SimplifySyntaxItem(SyntaxItem syntaxItem, out SyntaxItem simplifiedSyntaxItem, out string reason) {
            simplifiedSyntaxItem = syntaxItem;
            switch (syntaxItem) {
                case BinaryOperator binaryOperatorSyn: {
                    if (SimplifySyntaxItem(binaryOperatorSyn.SyntaxItem1, out SyntaxItem temp, out reason)) {
                        simplifiedSyntaxItem = new BinaryOperator(binaryOperatorSyn.Value, temp, binaryOperatorSyn.SyntaxItem2);
                        return true;
                    }
                    if (SimplifySyntaxItem(binaryOperatorSyn.SyntaxItem2, out temp, out reason)) {
                        simplifiedSyntaxItem = new BinaryOperator(binaryOperatorSyn.Value, binaryOperatorSyn.SyntaxItem1, temp);
                        return true;
                    }

                    break;
                }
                case UnaryOperator unaryOperator when SimplifySyntaxItem(unaryOperator.SyntaxItem, out SyntaxItem tempSyntaxItem, out reason):
                    simplifiedSyntaxItem = new UnaryOperator(unaryOperator.Value, tempSyntaxItem);
                    return true;
            }
            
            foreach (SimplificationRule simplificationRule in SimplificationRule.GetSimplificationRules()) {
                List<Tuple<SyntaxItem, string>> arbitrarySyntaxItems = new();
                if (!IsSyntaxItemMatch(simplifiedSyntaxItem, simplificationRule.LeftHandSide, ref arbitrarySyntaxItems))
                    continue;
                simplifiedSyntaxItem = SubstituteArbitrarySyntaxItems(simplificationRule.RightHandSide, arbitrarySyntaxItems);
                reason = simplificationRule.Message;
                return true;
            }

            
            reason = "";
            return false;
        }*/

        public override string ToString() {
            string returnValue = $"[Start, [{StartSyntaxItem}]]";
            for (int i = 0; i < Simplifications.Count; i++) {
                returnValue += $"{Environment.NewLine}[Simplification {i + 1}, [{Simplifications[i].Item1}], [{Simplifications[i].Item2}]]";
            }

            return returnValue;
        }
    }
}