namespace CubeRobot.Models.RubiksCube;

internal class CubeRotationHelper(int size, CubeFaceColor[][,] blocks)
{
    private readonly int _size = size;
    private readonly int _lastIndex = size - 1;
    private readonly CubeFaceColor[][,] _blocks = blocks;

    public void RotateFaceClockwise(CubeFace face)
    {
        int faceIndex = (int)face;

        // Right <- Up <- Left <- Down (clockwise direction)
        for (int i = 0; i < _size - 1; i++)
        {
            CubeFaceColor temp = _blocks[faceIndex][_lastIndex - i, _lastIndex];

            _blocks[faceIndex][_lastIndex - i, _lastIndex] = _blocks[faceIndex][0, _lastIndex - i];
            _blocks[faceIndex][0, _lastIndex - i] = _blocks[faceIndex][i, 0];
            _blocks[faceIndex][i, 0] = _blocks[faceIndex][_lastIndex, i];
            _blocks[faceIndex][_lastIndex, i] = temp;
        }

        UpdateAdjacentFacesClockwise(face);
    }

    public void RotateFaceCounterclockwise(CubeFace face)
    {
        int faceIndex = (int)face;

        // Right <- Down <- Left <- Up (counter-clockwise direction)
        for (int i = 0; i < _size - 1; i++)
        {
            CubeFaceColor temp = _blocks[faceIndex][i, _lastIndex];

            _blocks[faceIndex][i, _lastIndex] = _blocks[faceIndex][_lastIndex, _lastIndex - i];
            _blocks[faceIndex][_lastIndex, _lastIndex - i] = _blocks[faceIndex][_lastIndex - i, 0];
            _blocks[faceIndex][_lastIndex - i, 0] = _blocks[faceIndex][0, i];
            _blocks[faceIndex][0, i] = temp;
        }

        UpdateAdjacentFacesCounterclockwise(face);
    }

    private void UpdateAdjacentFacesClockwise(CubeFace face)
    {
        UpdateAdjacentFaces(face, [3, 2, 1, 0]);    // Rotate blocks: Down <- Right <- Up <- Left
    }

    private void UpdateAdjacentFacesCounterclockwise(CubeFace face)
    {
        UpdateAdjacentFaces(face, [3, 0, 1, 2]);    // Rotate blocks: Down <- Left <- Up <- Right
    }

    private void UpdateAdjacentFaces(CubeFace face, int[] rotationOrder)
    {
        CubeFace[] adjacentFaces = face.AdjacentFaces();
        var iterators = face.AdjacentIterators();

        for (int i = 0; i < _size; i++)
        {
            CubeFaceColor temp = _blocks[(int)adjacentFaces[rotationOrder[0]]][iterators[rotationOrder[0]](i, _lastIndex).y, iterators[rotationOrder[0]](i, _lastIndex).x];

            for (int j = 0; j < rotationOrder.Length - 1; j++)
            {
                int currentFaceIndex = (int)adjacentFaces[rotationOrder[j]];
                int nextFaceIndex = (int)adjacentFaces[rotationOrder[j + 1]];

                (int currentY, int currentX) = iterators[rotationOrder[j]](i, _lastIndex);
                (int nextY, int nextX) = iterators[rotationOrder[j + 1]](i, _lastIndex);

                _blocks[currentFaceIndex][currentY, currentX] = _blocks[nextFaceIndex][nextY, nextX];
            }

            _blocks[(int)adjacentFaces[rotationOrder[^1]]][iterators[rotationOrder[^1]](i, _lastIndex).y, iterators[rotationOrder[^1]](i, _lastIndex).x] = temp;
        }
    }
}