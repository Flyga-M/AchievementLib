using AchievementLib.Pack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AchievementLib.Reset
{
    /// <summary>
    /// Manages <see cref="ResetData"/> and fires the <see cref="Reset"/> event, when a reset occures.
    /// </summary>
    public class ResetManager : IDisposable
    {
        private SafeList<ResetData> _data = new SafeList<ResetData>();

        private Timer _timer;
        private bool _disposed;

        /// <summary>
        /// Fires, when a reset happens.
        /// </summary>
        public event EventHandler<string> Reset;

        private void OnReset(string id)
        {
            Reset?.Invoke(this, id);
        }

        /// <inheritdoc/>
        public ResetManager()
        {

        }

        /// <inheritdoc/>
        public ResetManager(IEnumerable<ResetData> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Any())
            {
                throw new ArgumentException($"{nameof(data)} must have at least one element.", nameof(data));
            }
            
            _data.AddRange(data);
            RecalculateTimer();
        }

        private void RecalculateTimer()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            if (!_data.Any())
            {
                return;
            }

            // .ToArray() is needed, because SafeList<T>.CopyTo is not suported and will throw an exception
            IEnumerable<ResetData> sortedByNext = _data.ToArray().OrderBy(reset => reset.NextOccurence);
            IEnumerable<ResetData> next = sortedByNext.Where(reset => reset.NextOccurence == sortedByNext.First().NextOccurence);

            TimeSpan nextExpiration = next.First().UntilNextOccurence;
            if (nextExpiration < TimeSpan.Zero)
            {
                nextExpiration = TimeSpan.FromSeconds(2);
            }

            _timer = new Timer(state => HandleReset(next), null, nextExpiration + TimeSpan.FromMilliseconds(200), Timeout.InfiniteTimeSpan);
        }

        private void HandleReset(IEnumerable<ResetData> resets)
        {
            foreach(ResetData reset in resets)
            {
                OnReset(reset.Id);
            }

            RecalculateTimer();
        }

        /// <summary>
        /// Determines whether the reset with the given <paramref name="id"/> has 
        /// occured at least once since the <paramref name="date"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns><see langword="true"/>, if the reset with the given 
        /// <paramref name="id"/> has occured at least once since
        /// the <paramref name="date"/>. Otherwise <see langword="false"/>.</returns>
        public bool ResetOccured(string id, DateTime date)
        {
            ResetData resetData = _data.Where(reset => reset.Id == id).FirstOrDefault();

            if (resetData == null)
            {
                return false;
            }

            return date < resetData.LastOccurence;
        }

        /// <summary>
        /// Attempts to remove add the <paramref name="data"/> to the <see cref="ResetManager"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns><see langword="true"/>, if the <paramref name="data"/> was successfully added. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool TryAdd(ResetData data)
        {
            if (_data.Any(date => data.Id == data.Id))
            {
                return false;
            }

            _data.Add(data);
            RecalculateTimer();

            return true;
        }

        /// <summary>
        /// Attempts to remove the reset with the <paramref name="id"/> from the <see cref="ResetManager"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see langword="true"/>, if the reset with the <paramref name="id"/> was successfully removed. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool TryRemove(string id)
        {
            if (!_data.Any(date => date.Id == id))
            {
                return false;
            }

            bool eval = _data.Remove(_data.Where(date => date.Id == id).First());
            
            if (eval)
            {
                RecalculateTimer();
            }

            return eval;
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                }

                _data?.Clear();
                _data = null;
                _disposed = true;
            }
        }

        /// <inheritdoc/>
        ~ResetManager()
        {
            Dispose(disposing: false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
