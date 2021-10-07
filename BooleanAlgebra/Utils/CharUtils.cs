namespace BooleanAlgebra.Utils {
    public static class CharUtils {
        public static bool IsLetter(this char character) {
            return character >= 65 && character <= 90 || //A-Z 
                   character >= 97 && character <= 122;  //a-z
        }
        
        public static bool IsDigit(this char character) {
            return character >= 48 && character <= 57 || //0-9
                   character == 46;                      //.
        }
        
        public static bool IsVariable(this char character) {
            return character.IsLetter() ||
                   character.IsDigit()  ||
                   character == 95; //_
        }

        public static bool IsOtherCharacter(this char character) {
            return !(character.IsLetter() 
                     || character.IsDigit() 
                     || character.IsVariable());
        }
    }
}