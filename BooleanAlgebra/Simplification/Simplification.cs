using System;
using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Identifiers;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Simplification {
    public class Simplification {
        public SyntaxItem StartSyntaxItem { get; }
        public List<Tuple<SyntaxItem, string>> Simplifications { get; }

        public Simplification(SyntaxItem startSyntaxItem) {
            StartSyntaxItem = startSyntaxItem;
            Simplifications = new List<Tuple<SyntaxItem, string>>();

            SyntaxItem? previous = startSyntaxItem;
            string? reason = null;
            bool didSimplify;
            do {
                if(reason is not null && previous is not null && reason != "0")
                    Simplifications.Add(new Tuple<SyntaxItem, string>(previous, reason));
                didSimplify = SimplifySyntaxItem(previous, out previous, out reason);
            } while (didSimplify);
        }

        private bool IsSyntaxItemMatch(SyntaxItem syntaxItem1, SyntaxItem syntaxItem2, ref SyntaxItem? arbitrarySyntaxItems) {
            if (syntaxItem1.Value != syntaxItem2.Value && syntaxItem1 is not ArbitrarySyntaxItem && syntaxItem2 is not ArbitrarySyntaxItem) return false;
            switch (syntaxItem1) {
                case ArbitrarySyntaxItem when arbitrarySyntaxItems is null:
                    arbitrarySyntaxItems = syntaxItem2;
                    return true;
                case ArbitrarySyntaxItem:
                    return syntaxItem2.Equals(arbitrarySyntaxItems);
            }

            switch (syntaxItem2) {
                case ArbitrarySyntaxItem when arbitrarySyntaxItems is null:
                    arbitrarySyntaxItems = syntaxItem1;
                    return true;
                case ArbitrarySyntaxItem:
                    return syntaxItem1.Equals(arbitrarySyntaxItems);
            }

            if (syntaxItem1.GetSyntaxItems().Length != syntaxItem2.GetSyntaxItems().Length) return false;

            for (int i = 0; i < syntaxItem1.GetSyntaxItems().Length; i++) {
                if (!IsSyntaxItemMatch(syntaxItem1.GetSyntaxItems()[i], syntaxItem2.GetSyntaxItems()[i], ref arbitrarySyntaxItems))
                    return false;
            }

            return true;
        }
        
        private bool GetSyntaxItemArbitrarySyntax(SyntaxItem syntaxItem1, SyntaxItem syntaxItem2, out SyntaxItem output) {
           // if (!IsSyntaxItemMatch(syntaxItem1, syntaxItem2)) return null;

           if (syntaxItem1 is ArbitrarySyntaxItem) {
               output = syntaxItem2;
               return true;
           }

           if (syntaxItem2 is ArbitrarySyntaxItem) {
               output = syntaxItem1;
               return true;
           }

           for (int i = 0; i < syntaxItem1.GetSyntaxItems().Length; i++) {
                if(GetSyntaxItemArbitrarySyntax(syntaxItem1.GetSyntaxItems()[i], syntaxItem2.GetSyntaxItems()[i], out output))
                    return true;
           }

            output = new ArbitrarySyntaxItem("UNKNOWN");
            return false;
        }

        private bool SimplifySyntaxItem(SyntaxItem syntaxItem, out SyntaxItem simplifiedSyntaxItem, out string reason) {
            simplifiedSyntaxItem = syntaxItem;

            if (true) {
                foreach (SimplificationRules simplificationRules in SimplificationRules.GetSimplificationRules()) {
                    SyntaxItem? syntaxItems = null;
                    if (simplifiedSyntaxItem is BinaryOperator binaryOperator) {
                        foreach (BinaryOperator constituentBinaryOperator in binaryOperator.GetConstituentBinaryOperators()) {
                            syntaxItems = null;
                            if (!IsSyntaxItemMatch(simplificationRules.SyntaxItem, constituentBinaryOperator,
                                ref syntaxItems)) {
                                syntaxItems = null;
                                List<SyntaxItem> tempReverse = constituentBinaryOperator.SyntaxItems;
                                tempReverse.Reverse();
                                BinaryOperator bin = new(constituentBinaryOperator.Value, tempReverse);
                                if (!IsSyntaxItemMatch(simplificationRules.SyntaxItem, bin,
                                    ref syntaxItems)) continue;
                                SyntaxItem newSimplifiedSyntaxItema;
                                if (simplificationRules.Equivalence is ArbitrarySyntaxItem) {
                                    if (!GetSyntaxItemArbitrarySyntax(constituentBinaryOperator,
                                        simplificationRules.SyntaxItem, out newSimplifiedSyntaxItema)) {
                                        newSimplifiedSyntaxItema = simplificationRules.Equivalence;
                                    }
                                } else {
                                    newSimplifiedSyntaxItema = simplificationRules.Equivalence;
                                }
                                
                                List<SyntaxItem> tempSyntaxItemsa = binaryOperator.SyntaxItems.ToList();
                                tempSyntaxItemsa.Remove(constituentBinaryOperator.SyntaxItems[0]);
                                tempSyntaxItemsa.Remove(constituentBinaryOperator.SyntaxItems[1]);
                                tempSyntaxItemsa.Insert(0, newSimplifiedSyntaxItema);
                                simplifiedSyntaxItem = tempSyntaxItemsa.Count > 1
                                    ? new BinaryOperator(binaryOperator.Value, tempSyntaxItemsa)
                                    : newSimplifiedSyntaxItema;
                                reason = simplificationRules.Message;
                                return true;
                            }
                            SyntaxItem newSimplifiedSyntaxItem;
                            if (simplificationRules.Equivalence is ArbitrarySyntaxItem) {
                                if (!GetSyntaxItemArbitrarySyntax(constituentBinaryOperator,
                                    simplificationRules.SyntaxItem, out newSimplifiedSyntaxItem)) {
                                    newSimplifiedSyntaxItem = simplificationRules.Equivalence;
                                }
                            } else {
                                newSimplifiedSyntaxItem = simplificationRules.Equivalence;
                            }

                            List<SyntaxItem> tempSyntaxItems = binaryOperator.SyntaxItems.ToList();
                           tempSyntaxItems.Remove(constituentBinaryOperator.SyntaxItems[0]);
                            tempSyntaxItems.Remove(constituentBinaryOperator.SyntaxItems[1]);
                            tempSyntaxItems.Insert(0, newSimplifiedSyntaxItem);
                            simplifiedSyntaxItem = tempSyntaxItems.Count > 1
                                ? new BinaryOperator(binaryOperator.Value, tempSyntaxItems)
                                : newSimplifiedSyntaxItem;
                            reason = simplificationRules.Message;
                            return true;
                        }
                    } else {
                        if (!IsSyntaxItemMatch(simplificationRules.SyntaxItem, simplifiedSyntaxItem, ref syntaxItems))
                            continue;
                        if (simplificationRules.Equivalence is ArbitrarySyntaxItem) {
                            if (!GetSyntaxItemArbitrarySyntax(syntaxItem,
                                simplificationRules.SyntaxItem, out simplifiedSyntaxItem)) {
                                simplifiedSyntaxItem = simplificationRules.Equivalence;
                            }
                        } else {
                            simplifiedSyntaxItem = simplificationRules.Equivalence;
                        }
                        reason = simplificationRules.Message;
                        return true;
                    }
                }
            }

            if (simplifiedSyntaxItem is GroupingOperator groupingOperator) {
                if (SimplifySyntaxItem(groupingOperator.SyntaxItem, out SyntaxItem synItem, out reason)) {
                    simplifiedSyntaxItem = synItem;
                    return true;
                }

                simplifiedSyntaxItem = groupingOperator.SyntaxItem;
                reason = "0";
                return true;
            }

            if (simplifiedSyntaxItem is BinaryOperator binaryOperatorSyn) {
                SyntaxItem[] syntaxItems = binaryOperatorSyn.GetSyntaxItems();
                for (int i = 0; i < syntaxItems.Length; i++) {
                    if (SimplifySyntaxItem(syntaxItems[i], out SyntaxItem tempSyntaxItem, out reason)) {
                        syntaxItems[i] = tempSyntaxItem;
                        simplifiedSyntaxItem = new BinaryOperator(binaryOperatorSyn.Value, syntaxItems);
                        return true;
                    }
                }
            }

            if (simplifiedSyntaxItem is UnaryOperator unaryOperator) {
                if (SimplifySyntaxItem(unaryOperator.SyntaxItem, out SyntaxItem tempSyntaxItem, out reason)) {
                    simplifiedSyntaxItem = new UnaryOperator(unaryOperator.Value, tempSyntaxItem);
                    return true;
                }
            }


            reason = "";
            return false;
        }

        public override string ToString() {
            string returnValue = $"[Start, [{StartSyntaxItem}]]";
            for (int i = 0; i < Simplifications.Count; i++) {
                returnValue += $"{Environment.NewLine}[Simplification {i + 1}, [{Simplifications[i].Item1}], [{Simplifications[i].Item2}]]";
            }

            return returnValue;
        }
    }
}