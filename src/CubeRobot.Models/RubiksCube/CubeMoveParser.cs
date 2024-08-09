using System.Text;

namespace CubeRobot.Models.RubiksCube;

public static class CubeMoveParser
{
    /// <summary>
    /// Converts <paramref name="movesInNotation"/> to array of <see cref="CubeMove"/>
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <paramref name="movesInNotation"/> is empty or contains illegal character
    /// </exception>
    public static CubeMove[] ParseMoves(string movesInNotation)
    {
        ArgumentException.ThrowIfNullOrEmpty(movesInNotation);

        List<CubeMove> moves = [];
        string preparedMovesInNotation = PrepareMovesString(movesInNotation);

        for (int i = 0; i < movesInNotation.Length; i++)
        {
            StringBuilder currentMoveBuilder = new(preparedMovesInNotation[i].ToString());

            // Try parse long moves (longer than 1 letter)
            if (i + 1 < preparedMovesInNotation.Length)
            {
                char nextChar = preparedMovesInNotation[i + 1];

                if (nextChar == '\'')
                {
                    currentMoveBuilder.Append(CubeMoveExtensions.PrimeMoveModifierString);
                    i++;
                }
                else if (nextChar == '2')
                {
                    currentMoveBuilder.Append(CubeMoveExtensions.DoubleMoveModifierString);
                    i++;
                }
            }

            if (!Enum.TryParse(currentMoveBuilder.ToString(), out CubeMove move))
                throw new ArgumentException($"Move not recognised: \'{currentMoveBuilder}\'");

            moves.Add(move);
        }

        return [.. moves];
    }

    private static string PrepareMovesString(string originalString)
    {
        string[] splitMoves = originalString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(splitMoves);
    }
}