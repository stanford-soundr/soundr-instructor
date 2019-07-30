using System;
using System.Collections.Generic;
using System.Threading;

public class BlockingQueue<T> : IDisposable
{
    private readonly Queue<T> _queue = new Queue<T>();

    private Semaphore _semaphore = new Semaphore(0, int.MaxValue);


    public void Enqueue(T data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        lock (_queue) _queue.Enqueue(data);

        _semaphore.Release();
    }

    public T Dequeue()
    {
        _semaphore.WaitOne();

        lock (_queue) return _queue.Dequeue();
    }

    void IDisposable.Dispose()
    {
        if (_semaphore == null) return;
        _semaphore.Close();
        _semaphore = null;
    }
}