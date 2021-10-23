namespace BooleanAlgebra.Simplification;
public class Simplification {

    /*     public SyntaxItem StartSyntaxItem { get; }
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
                 didSimplify = SimplifySyntaxItem(previous, out previous, out reason);
             } while (didSimplify);
         }

         private bool SimplifySyntaxItem(SyntaxItem syntaxItem, out SyntaxItem simplifiedSyntaxItem,
             out string reason) {
             if (syntaxItem is BinaryOperator binaryOperator) {
                 if (syntaxItem.Value == "OR") {
                     if (binaryOperator.SyntaxItems.Contains(new Operand("1"))) {
                         simplifiedSyntaxItem = new Operand("1");
                         reason = "Annulment Law";
                         return true;
                     } else if (binaryOperator.SyntaxItems.Contains(new Operand("0"))) {
                         SyntaxItem[] removedSyntaxItems = binaryOperator.SyntaxItems.Where(x => !x.Equals(new Operand("0"))).ToArray();
                         if (removedSyntaxItems.Length == 1) {
                             simplifiedSyntaxItem = removedSyntaxItems[0];
                         } else {
                             simplifiedSyntaxItem = new BinaryOperator(binaryOperator.Value, removedSyntaxItems);
                         }
                         reason = "Identity Law";
                         return true;
                     } else {
                         foreach (SyntaxItem syntaxItemx in binaryOperator.SyntaxItems) {
                             if (syntaxItemx is UnaryOperator {Value: "NOT"} nextUnaryOperator) {
                                 if (nextUnaryOperator.SyntaxItem is Operand operand) {
                                     if (binaryOperator.SyntaxItems.Contains(operand)) {
                                         simplifiedSyntaxItem = new Operand("1");
                                         reason = "Complement Law";
                                         return true;
                                     }
                                 } else if(nextUnaryOperator.SyntaxItem is UnaryOperator unaryOperatorx) {
                                     if (binaryOperator.SyntaxItems.Contains(unaryOperatorx)) {
                                         simplifiedSyntaxItem = new Operand("1");
                                         reason = "Complement Law";
                                         return true;
                                     }
                                 } else if (nextUnaryOperator.SyntaxItem is BinaryOperator binaryOperatorx) {
                                     bool hasFoundNonMatch = false;
                                     foreach (SyntaxItem synItem in binaryOperatorx.SyntaxItems) {
                                         if (!binaryOperator.SyntaxItems.Contains(synItem)) {
                                             hasFoundNonMatch = true;
                                             break;
                                         }
                                     }

                                     if (hasFoundNonMatch) {
                                         if (binaryOperator.SyntaxItems.Contains(nextUnaryOperator.SyntaxItem)) {
                                             simplifiedSyntaxItem = new Operand("1");
                                             reason = "Complement Law";
                                             return true;
                                         }
                                     }

                                     if (!hasFoundNonMatch) {
                                         simplifiedSyntaxItem = new Operand("1");
                                         reason = "Complement Law";
                                         return true;
                                     }
                                 }
                             }
                         }
                     }
                 } else if (syntaxItem.Value == "AND") {
                     if (binaryOperator.SyntaxItems.Contains(new Operand("0"))) {
                         simplifiedSyntaxItem = new Operand("0");
                         reason = "Annulment Law";
                         return true;
                     } else if (binaryOperator.SyntaxItems.Contains(new Operand("1"))) {
                         SyntaxItem[] removedSyntaxItems = binaryOperator.SyntaxItems.Where(x => !x.Equals(new Operand("1"))).ToArray();
                         if (removedSyntaxItems.Length == 1) {
                             simplifiedSyntaxItem = removedSyntaxItems[0];
                         } else {
                             simplifiedSyntaxItem = new BinaryOperator(binaryOperator.Value, removedSyntaxItems);
                         }
                         reason = "Identity Law";
                         return true;
                     } else {
                         foreach (SyntaxItem syntaxItemx in binaryOperator.SyntaxItems) {
                             if (syntaxItemx is UnaryOperator {Value: "NOT"} nextUnaryOperator) {
                                 if (nextUnaryOperator.SyntaxItem is Operand operand) {
                                     if (binaryOperator.SyntaxItems.Contains(operand)) {
                                         simplifiedSyntaxItem = new Operand("1");
                                         reason = "Complement Law";
                                         return true;
                                     }
                                 } else if(nextUnaryOperator.SyntaxItem is UnaryOperator unaryOperatorx) {
                                     if (binaryOperator.SyntaxItems.Contains(unaryOperatorx)) {
                                         simplifiedSyntaxItem = new Operand("1");
                                         reason = "Complement Law";
                                         return true;
                                     }
                                 } else if (nextUnaryOperator.SyntaxItem is BinaryOperator binaryOperatorx) {
                                     bool hasFoundNonMatch = false;
                                     foreach (SyntaxItem synItem in binaryOperatorx.SyntaxItems) {
                                         if (!binaryOperator.SyntaxItems.Contains(synItem)) {
                                             hasFoundNonMatch = true;
                                             break;
                                         }
                                     }

                                     if (hasFoundNonMatch) {
                                         if (binaryOperator.SyntaxItems.Contains(nextUnaryOperator.SyntaxItem)) {
                                             simplifiedSyntaxItem = new Operand("1");
                                             reason = "Complement Law";
                                             return true;
                                         }
                                     }

                                     if (!hasFoundNonMatch) {
                                         simplifiedSyntaxItem = new Operand("1");
                                         reason = "Complement Law";
                                         return true;
                                     }
                                 }
                             }
                         }
                     }
                 }

                 SyntaxItem[] syntaxItems = binaryOperator.SyntaxItems.Distinct().ToArray();
                 if (!binaryOperator.SyntaxItems.SequenceEqual(syntaxItems)) {
                     if (syntaxItems.Length == 1) {
                         simplifiedSyntaxItem = syntaxItems[0];
                     } else {
                         simplifiedSyntaxItem = new BinaryOperator(binaryOperator.Value, syntaxItems);
                     }
                     reason = "Idempotent Law";
                     return true;
                 }

                 for (int i = 0; i < syntaxItems.Length; i++) {
                     if (SimplifySyntaxItem(syntaxItems[i], out syntaxItems[i], out reason)) {
                         simplifiedSyntaxItem = new BinaryOperator(binaryOperator.Value, syntaxItems);
                         return true;
                     }
                 }
             }

             if (syntaxItem is UnaryOperator unaryOperator) {
                 if (unaryOperator.Equals(new UnaryOperator("NOT", new Operand("0")))) {
                     simplifiedSyntaxItem = new Operand("1");
                     reason = "NOT 0 is 1";
                     return true;
                 }

                 if (unaryOperator.Equals(new UnaryOperator("NOT", new Operand("1")))) {
                     simplifiedSyntaxItem = new Operand("0");
                     reason = "NOT 1 is 0";
                     return true;
                 }

                 if (unaryOperator.Value == "NOT" && unaryOperator.SyntaxItem is UnaryOperator {Value: "NOT"} nextUnaryOperator) {
                     simplifiedSyntaxItem = nextUnaryOperator.SyntaxItem;
                     reason = "Involution Law";
                     return true;
                 }

                 if (SimplifySyntaxItem(unaryOperator.SyntaxItem, out SyntaxItem tempSimplifiedSyntaxItem, out reason)) {
                     simplifiedSyntaxItem = new UnaryOperator(unaryOperator.Value, tempSimplifiedSyntaxItem);
                     return true;
                 }

                 if (unaryOperator.Value == "NOT" && unaryOperator.SyntaxItem is BinaryOperator nextBinaryOperator) {
                     simplifiedSyntaxItem = nextBinaryOperator.Value == "OR" 
                         ? new BinaryOperator("AND", nextBinaryOperator.SyntaxItems.Select(synItem => new UnaryOperator("NOT", synItem)).Cast<SyntaxItem>().ToList()) 
                         : new BinaryOperator("OR", nextBinaryOperator.SyntaxItems.Select(synItem => new UnaryOperator("NOT", synItem)).Cast<SyntaxItem>().ToList());

                     reason = "De Morgan's Law";
                     return true;
                 }
             }


             simplifiedSyntaxItem = syntaxItem;
             reason = "";
             return false;
         }

         public override string ToString() {
             string returnValue = $"[Start, [{StartSyntaxItem}]]";
             for (int i = 0; i < Simplifications.Count; i++) {
                 returnValue += $"{Environment.NewLine}[Simplification {i + 1}, [{Simplifications[i].Item1}], [{Simplifications[i].Item2}]]";
             }

             return returnValue;
         }*/
}