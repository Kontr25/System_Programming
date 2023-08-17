using System;

namespace Lesson_10
{
    [Serializable]
    public class Ingredient
    {
        public string Name;
        public int Amount = 1;
        public IngredientUnit Unit;
    }
    public enum IngredientUnit
    {
        Spoon, Cup, Bowl, Piece
    }
}