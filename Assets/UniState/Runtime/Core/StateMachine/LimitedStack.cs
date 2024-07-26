namespace UniState
{
    public class LimitedStack<T>
    {
        private readonly  T[] _items;
        private int _topIndex = 0;
        private int _bottomIndex = 0;
        private int _maxSize;
        // count: (_topIndex - _bottomIndex) % _maxSize
        // top index: (_topIndex - 1) % _maxSize
        // isNotEmpty: _topIndex != _bottomIndex

        public LimitedStack(int maxSize)
        {
            _maxSize = maxSize;
            _items = new T[_maxSize];
        }

        public T Push(T element)
        {
            // If max capacity reached
            // remove one from bottom
            // no need to clear as it will be replaced right away
            if (_topIndex != _bottomIndex && (_topIndex - _bottomIndex) % _maxSize == 0)
            {
                _bottomIndex++;
            }

            _topIndex++;
            _items[(_topIndex-1)%_maxSize] = element;
            return element;
        }

        public T Peek() => _topIndex != _bottomIndex ? _items[(_topIndex-1)%_maxSize] : default(T);

        public T Pop()
        {
            var result = Peek();

            if (_topIndex != _bottomIndex)
            {
                _items[(_topIndex-1)%_maxSize] = default(T);
                _topIndex--;
            }

            return result;
        }
    }
}